using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using System.IO;
using WCABNetwork.Cab.IdentityService.Infrastructures.Extensions;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Middlewares;

public class CustomRateLimitMiddleWare : RateLimitMiddleware<CustomIpRateLimitProcessor>
{
    private readonly RequestDelegate _next;
    private readonly IOptions<IpRateLimitOptions> _ipOptions;
    private readonly IOptions<RateLimitOptions> _options;
    private readonly IRateLimitConfiguration _config;
    private readonly IRateLimitCounterStore _counterStore;
    private readonly ILogger<CustomRateLimitMiddleWare> _logger;

    public CustomRateLimitMiddleWare(RequestDelegate next,
        IProcessingStrategy processingStrategy,
        IOptions<IpRateLimitOptions> options,
        IIpPolicyStore policyStore,
        IRateLimitConfiguration config, 
        IRateLimitCounterStore counterStore,
        ILogger<CustomRateLimitMiddleWare> logger
        )
        : base(next, options?.Value, new CustomIpRateLimitProcessor(options?.Value, policyStore, processingStrategy, config, counterStore), config)
    {
        _next = next;
        _config = config;
        _logger = logger;
    }

    protected override void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
    {
        _logger.LogInformation($"IP:{httpContext.Connection.RemoteIpAddress};PATH:{httpContext.Request.Path.ToString().ToLowerInvariant()}");
    }
}

public abstract class RateLimitMiddleware<TProcessor>
        where TProcessor : ICustomRateLimitProcessor
{
    private readonly RequestDelegate _next;
    private readonly TProcessor _processor;
    private readonly RateLimitOptions _options;
    private readonly IRateLimitConfiguration _config;

    protected RateLimitMiddleware(
        RequestDelegate next,
        RateLimitOptions options,
        TProcessor processor,
        IRateLimitConfiguration config)
    {
        _next = next;
        _options = options;
        _processor = processor;
        _config = config;
        _config.RegisterResolvers();
    }

    public async Task Invoke(HttpContext context)
    {
        // check if rate limiting is enabled
        if (_options == null)
        {
            await _next.Invoke(context);
            return;
        }

        // compute identity from request
        var identity = await ResolveIdentityAsync(context);

        // check white list
        if (_processor.IsWhitelisted(identity))
        {
            await _next.Invoke(context);
            return;
        }

        var rules = await _processor.GetMatchingRulesAsync(identity, context.RequestAborted);

        var rulesDict = new Dictionary<RateLimitRule, RateLimitCounter>();
        bool modify = false;
        string modifiedMessage = string.Empty;

        #region First checked
        foreach (var rule in rules)
        {
            var rateLimitCounter = new RateLimitCounter();
            if (rule.Limit > 0)
            {
                // find counter
                rateLimitCounter = await _processor.GetCounterAsync(identity, rule, context.RequestAborted);
                // check if key expired
                if (rateLimitCounter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                {
                    continue;
                }
                // check if limit is reached
                if (rateLimitCounter.Count >= rule.Limit)
                {
                    //compute retry after value
                    var retryAfter = rateLimitCounter.Timestamp.RetryAfterFrom(rule);

                    // log blocked request
                    LogBlockedRequest(context, identity, rateLimitCounter, rule);

                    if (_options.RequestBlockedBehaviorAsync != null)
                    {
                        await _options.RequestBlockedBehaviorAsync(context, identity, rateLimitCounter, rule);
                    }

                    if (!rule.MonitorMode)
                    {
                        context.Response.StatusCode = _options.QuotaExceededResponse?.StatusCode ?? _options.HttpStatusCode;
                        context.Response.ContentType = _options.QuotaExceededResponse?.ContentType ?? "text/plain";
                        // break execution
                        modifiedMessage = ReturnQuotaExceededResponse(context, rule, retryAfter);
                        await context.Response.WriteAsync(modifiedMessage);
                        return;
                    }
                }
            }
            // if limit is zero or less, block the request.
            else if (rule.Limit <= 0)
            {
                // log blocked request
                LogBlockedRequest(context, identity, rateLimitCounter, rule);

                if (!rule.MonitorMode)
                {
                    // break execution (Int32 max used to represent infinity)
                    modifiedMessage = ReturnQuotaExceededResponse(context, rule, int.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    await context.Response.WriteAsync(modifiedMessage);
                    return;
                }
            }
        }
        #endregion

        #region After passing 1st check: prepare to check condition and modify response
        var response = context.Response;
        var originBody = response.Body;
        using var newBody = new MemoryStream();
        response.Body = newBody;
        #endregion

        await _next(context);

        #region Rate limit condition checking and response modifying
        foreach (var rule in rules)
        {
            var rateLimitCounter = new RateLimitCounter();

            if (rule.Limit > 0
                && (context.Request.Path.ToString().ToLowerInvariant().Contains("/api/v1/accounts/check")
                    || (context.Request.Path.ToString().ToLowerInvariant().Contains("/api/v1/accounts/password") && context.Request.Method == "DELETE")
                    || (context.Response.StatusCode is 200 && context.Request.Path.ToString().ToLowerInvariant().Contains("/api/v1/accounts/register"))
                    || (context.Response.StatusCode is 400 or 401 or 403 or 404 && !context.Request.Path.ToString().ToLowerInvariant().Contains("/api/v1/accounts/register"))))
            {
                // increment counter
                rateLimitCounter = await _processor.ProcessRequestAsync(identity, rule, context.RequestAborted);
                // check if key expired
                if (rateLimitCounter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                {
                    continue;
                }
                // check if limit is reached
                if (rateLimitCounter.Count > rule.Limit)
                {
                    //compute retry after value
                    var retryAfter = rateLimitCounter.Timestamp.RetryAfterFrom(rule);

                    // log blocked request
                    LogBlockedRequest(context, identity, rateLimitCounter, rule);

                    if (_options.RequestBlockedBehaviorAsync != null)
                    {
                        await _options.RequestBlockedBehaviorAsync(context, identity, rateLimitCounter, rule);
                    }

                    if (!rule.MonitorMode)
                    {
                        context.Response.StatusCode = _options.QuotaExceededResponse?.StatusCode ?? _options.HttpStatusCode;
                        context.Response.ContentType = _options.QuotaExceededResponse?.ContentType ?? "text/plain";
                        // break execution
                        modifiedMessage = ReturnQuotaExceededResponse(context, rule, retryAfter);
                        modify = true;
                    }
                }
                rulesDict.Add(rule, rateLimitCounter);
            }
            // if limit is zero or less, block the request.
            else if (rule.Limit <= 0)
            {
                // log blocked request
                LogBlockedRequest(context, identity, rateLimitCounter, rule);

                if (!rule.MonitorMode)
                {
                    // break execution (Int32 max used to represent infinity)
                    modifiedMessage = ReturnQuotaExceededResponse(context, rule, int.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    modify = true;
                }
            }
        }

        // set X-Rate-Limit headers for the longest period
        if (rulesDict.Any() && !_options.DisableRateLimitHeaders)
        {
            var rule = rulesDict.OrderByDescending(x => x.Key.PeriodTimespan).FirstOrDefault();
            var headers = _processor.GetRateLimitHeaders(rule.Value, rule.Key, context.RequestAborted);

            headers.Context = context;

            context.Response.OnStarting(SetRateLimitHeaders, state: headers);
        }
        
        if(modify)
            await ModifyResponseAsync(response, modifiedMessage);
        #endregion

        newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originBody);
        response.Body = originBody;
    }

    public virtual async Task<ClientRequestIdentity> ResolveIdentityAsync(HttpContext httpContext)
    {
        string clientIp = null;
        string clientId = null;

        if (_config.ClientResolvers?.Any() == true)
        {
            foreach (var resolver in _config.ClientResolvers)
            {
                clientId = await resolver.ResolveClientAsync(httpContext);

                if (!string.IsNullOrEmpty(clientId))
                {
                    break;
                }
            }
        }

        if (_config.IpResolvers?.Any() == true)
        {
            foreach (var resolver in _config.IpResolvers)
            {
                clientIp = resolver.ResolveIp(httpContext);

                if (!string.IsNullOrEmpty(clientIp))
                {
                    break;
                }
            }
        }
        var path = httpContext.Request.Path.ToString().ToLowerInvariant();
        return new ClientRequestIdentity
        {
            ClientIp = clientIp,
            Path = path == "/"
                ? path
                : path.TrimEnd('/'),
            HttpVerb = httpContext.Request.Method.ToLowerInvariant(),
            ClientId = clientId ?? "anon"
        };
    }

    public virtual string ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
    {
        //Use Endpoint QuotaExceededResponse
        if (rule.QuotaExceededResponse != null)
        {
            _options.QuotaExceededResponse = rule.QuotaExceededResponse;
        }
        var message = string.Format(
            _options.QuotaExceededResponse?.Content ??
            _options.QuotaExceededMessage ??
            "API calls quota exceeded! maximum admitted {0} per {1}.",
            rule.Limit,
            rule.PeriodTimespan.HasValue ? FormatPeriodTimespan(rule.PeriodTimespan.Value) : rule.Period, retryAfter);
        if (!_options.DisableRateLimitHeaders)
        {
            httpContext.Response.Headers["Retry-After"] = retryAfter;
        }

        return message;
    }

    private static string FormatPeriodTimespan(TimeSpan period)
    {
        var sb = new StringBuilder();

        if (period.Days > 0)
        {
            sb.Append($"{period.Days}d");
        }

        if (period.Hours > 0)
        {
            sb.Append($"{period.Hours}h");
        }

        if (period.Minutes > 0)
        {
            sb.Append($"{period.Minutes}m");
        }

        if (period.Seconds > 0)
        {
            sb.Append($"{period.Seconds}s");
        }

        if (period.Milliseconds > 0)
        {
            sb.Append($"{period.Milliseconds}ms");
        }

        return sb.ToString();
    }

    protected abstract void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule);

    private Task SetRateLimitHeaders(object rateLimitHeaders)
    {
        var headers = (RateLimitHeaders)rateLimitHeaders;

        headers.Context.Response.Headers["X-Rate-Limit-Limit"] = headers.Limit;
        headers.Context.Response.Headers["X-Rate-Limit-Remaining"] = headers.Remaining;
        headers.Context.Response.Headers["X-Rate-Limit-Reset"] = headers.Reset;

        return Task.CompletedTask;
    }

    private async Task ModifyResponseAsync(HttpResponse response, string modifiedResponse)
    {
        var stream = response.Body;
        using var reader = new StreamReader(stream, leaveOpen: true);
        string originalResponse = await reader.ReadToEndAsync();
        stream.SetLength(0);
        using var writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(modifiedResponse);
        await writer.FlushAsync();
        response.ContentLength = stream.Length;
    }
}