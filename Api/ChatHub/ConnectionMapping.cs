using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.ChatHub
{
    public static class ConnectionMapping
    {
        private static readonly Dictionary<string, HashSet<string>> _connections = new();

        public static void Add(string userId, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(userId, out var connections))
                {
                    connections = new HashSet<string>();
                    _connections[userId] = connections;
                }
                connections.Add(connectionId);
            }
        }

        public static void Remove(string userId, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(userId, out var connections)) return;

                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _connections.Remove(userId);
                }
            }
        }

        public static IEnumerable<string> GetConnections(string userId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }
    }
}