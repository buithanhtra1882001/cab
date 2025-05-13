using System.Collections.Concurrent;

namespace CabUserService.Infrastructures.Helper
{
    public class CommonHelper
    {
        private static readonly Dictionary<string, ConcurrentDictionary<string, long>> ProtectActionData = new();

        public static void ProtectAction(string action, string entity, int circleInSeconds = 60)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            lock (ProtectActionData)
            {
                if (ProtectActionData.TryGetValue(action, out var requests))
                {
                    if (requests.TryGetValue(entity, out var last))
                    {
                        if (last > now - TimeSpan.FromSeconds(circleInSeconds).TotalMilliseconds)
                        {
                            var remainSeconds = circleInSeconds - (now - last) / 1000;
                            throw new Exception($"Sorry, too many attempts. Please try again in {remainSeconds} seconds.");
                        }

                        requests.TryRemove(entity, out long time);

                        if (requests.IsEmpty)
                            ProtectActionData.Remove(action);
                    }
                    else
                    {
                        requests.TryAdd(entity, now);
                    }
                }
                else
                {
                    var newRequests = new ConcurrentDictionary<string, long>();
                    newRequests.TryAdd(entity, now);
                    ProtectActionData.TryAdd(action, newRequests);
                }
            }
        }
    }
}
