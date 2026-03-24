using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StratFlux.Models;
using StratFlux.Services;

namespace StratFlux
{
    [Authorize]
    public class BacktestsHub : Hub
    {
        private UserManager<StratUser> _userManager;
        private BacktestsNotificationService _notificationService;

        public BacktestsHub(UserManager<StratUser> userManager, BacktestsNotificationService notificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User != null)
            {
                if (Context.User.Identity != null)
                {
                    if (Context.User.Identity.Name != null)
                    {
                        StratUser user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
                        
                        if (user != null)
                        {
                            _notificationService.AddConnection(user.Id, Context.ConnectionId);
                        }
                    }
                }
            }

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User != null)
            {
                if (Context.User.Identity != null)
                {
                    if (Context.User.Identity.Name != null)
                    {
                        _notificationService.RemoveConnection(Context.User.Identity.Name);
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
