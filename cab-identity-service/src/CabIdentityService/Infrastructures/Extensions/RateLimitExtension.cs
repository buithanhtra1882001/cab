using AspNetCoreRateLimit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Extensions;

public interface ICustomRateLimitProcessor : IRateLimitProcessor
{
    Task<RateLimitCounter> GetCounterAsync(ClientRequestIdentity requestIdentity, RateLimitRule rule, CancellationToken cancellationToken = default);
}

public class CustomRateLimitProcessor : RateLimitProcessor
{
    private readonly RateLimitOptions _options;

    public CustomRateLimitProcessor(RateLimitOptions options) : base(options)
    {
        _options = options;
    }

}

public class CustomIpRateLimitProcessor : CustomRateLimitProcessor, ICustomRateLimitProcessor
{
    private readonly IpRateLimitOptions _options;
    private readonly IRateLimitStore<IpRateLimitPolicies> _policyStore;
    private readonly IProcessingStrategy _processingStrategy;
    private readonly ICounterKeyBuilder _counterKeyBuilder;
    private readonly IRateLimitConfiguration _config;
    private readonly IRateLimitCounterStore _counterStore;

    public CustomIpRateLimitProcessor(
            IpRateLimitOptions options,
            IIpPolicyStore policyStore,
            IProcessingStrategy processingStrategy,
            IRateLimitConfiguration config,
            IRateLimitCounterStore counterStore)
        : base(options)
    {
        _options = options;
        _policyStore = policyStore;
        _counterKeyBuilder = new IpCounterKeyBuilder(options);
        _processingStrategy = processingStrategy;
        _config = config;
        _counterStore = counterStore;
    }

    public async Task<RateLimitCounter> GetCounterAsync(ClientRequestIdentity requestIdentity, RateLimitRule rule, CancellationToken cancellationToken = default)
    {
        var counter = new RateLimitCounter
        {
            Timestamp = DateTime.UtcNow,
            Count = 0
        };
        var key = _counterKeyBuilder.Build(requestIdentity, rule);

        if (_options.EnableEndpointRateLimiting && _config.EndpointCounterKeyBuilder != null)
        {
            key += _config.EndpointCounterKeyBuilder.Build(requestIdentity, rule);
        }

        var bytes = Encoding.UTF8.GetBytes(key);

        using var algorithm = SHA1.Create();
        var hash = algorithm.ComputeHash(bytes);

        var counterId = Convert.ToBase64String(hash);

        var entry = await _counterStore.GetAsync(counterId, cancellationToken);

        if (entry.HasValue)
        {
            // entry has not expired
            if (entry.Value.Timestamp + rule.PeriodTimespan.Value >= DateTime.UtcNow)
            {
                // increment request count
                var totalCount = entry.Value.Count;

                // deep copy
                counter = new RateLimitCounter
                {
                    Timestamp = entry.Value.Timestamp,
                    Count = totalCount
                };
            }
        }

        return counter;
    }

    public async Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity, CancellationToken cancellationToken = default)
    {
        var policies = await _policyStore.GetAsync($"{_options.IpPolicyPrefix}", cancellationToken);

        var rules = new List<RateLimitRule>();

        if (policies?.IpRules?.Any() == true)
        {
            // search for rules with IP intervals containing client IP
            var matchPolicies = policies.IpRules.Where(r => IpParser.ContainsIp(r.Ip, identity.ClientIp));

            foreach (var item in matchPolicies)
            {
                rules.AddRange(item.Rules);
            }
        }

        return GetMatchingRules(identity, rules);
    }

    public async Task<RateLimitCounter> ProcessRequestAsync(ClientRequestIdentity requestIdentity, RateLimitRule rule, CancellationToken cancellationToken = default)
    {
        return await _processingStrategy.ProcessRequestAsync(requestIdentity, rule, _counterKeyBuilder, _options, cancellationToken);
    }
}