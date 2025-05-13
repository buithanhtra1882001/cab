using Serilog.Context;
using System;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Loggings
{
    public class SerilogTraceContext : ILogTraceContext
    {
        private const string TraceContext = "TraceContext";
        private IDisposable _logContext { get; set; }

        // LogContext won't push data in a call context when below method was called in async functions
        // See: https://github.com/serilog/serilog/issues/1130
        // Please put this method in top of your functions as a call context
        public void SetData(object data)
        {
            _logContext = LogContext.PushProperty(TraceContext, data, true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _logContext?.Dispose();
        }
    }
}