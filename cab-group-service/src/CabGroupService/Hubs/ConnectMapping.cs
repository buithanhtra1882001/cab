namespace CabGroupService.Hubs
{
    public class ConnectionMapping<T>
    {
        public static readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(IEnumerable<T> keys)
        {
            var allConnections = new List<string>();

            foreach (var key in keys)
            {
                if (_connections.TryGetValue(key, out HashSet<string> connections))
                {
                    allConnections.AddRange(connections);
                }

            }

            return allConnections;
        }

        public void Remove(string connectionId)
        {
            lock (_connections)
            {
                var targetUserConnection = _connections.Where(x => x.Value.Contains(connectionId)).FirstOrDefault();
                var connections = targetUserConnection.Value;

                if (connections is null)
                {
                    return;
                }
                lock (connections)
                {
                    connections?.Remove(connectionId);
                    if (connections is null || connections.Count == 0)
                    {
                        _connections.Remove(targetUserConnection.Key, out _);
                    }
                }
            }
        }

    }

}
