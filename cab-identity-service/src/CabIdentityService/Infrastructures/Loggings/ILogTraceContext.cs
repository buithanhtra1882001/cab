using System;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Loggings
{
    public interface ILogTraceContext : IDisposable
    {
        public void SetData(object data);
    }
}