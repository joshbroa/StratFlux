using Microsoft.AspNetCore.SignalR;
using StratFlux.Models;

namespace StratFlux.Services
{
    public class BacktestsNotificationService
    {
        private IHubContext<BacktestsHub> _hubContext;
        private Dictionary<string, string> _connections;

        public BacktestsNotificationService(IHubContext<BacktestsHub> hubContext)
        {
            _hubContext = hubContext;
            _connections = new Dictionary<string, string>();
        }

        public void AddConnection(string userId, string connectionId)
        {
            if (_connections.ContainsKey(userId))
            {
                _connections.Remove(userId);
            }

            _connections.Add(userId, connectionId);
        }

        public void RemoveConnection(string userId)
        {
            if (_connections.ContainsKey(userId))
            {
                _connections.Remove(userId);
            }
        }

        public async Task NotifyBacktestComplete(string userId, string generalResultsId)
        {
            for (int i = 0; i < 10; i++)
            {
                if (_connections.ContainsKey(userId))
                {
                    await _hubContext.Clients.Client(_connections[userId]).SendAsync("BacktestComplete", generalResultsId);
                    return;
                }

                Thread.Sleep(5000);
            }
        }

        public async Task NotifyBacktestFailed(string userId, string errorMessage)
        {
            for (int i = 0; i < 10; i++)
            {
                if (_connections.ContainsKey(userId))
                {
                    await _hubContext.Clients.Client(_connections[userId]).SendAsync("BacktestFailed", errorMessage);
                    return;
                }

                Thread.Sleep(5000);
            }
        }
    }
}
