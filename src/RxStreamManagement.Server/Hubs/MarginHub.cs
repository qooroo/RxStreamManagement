using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RxStreamManagement.Server.Subscription;

namespace RxStreamManagement.Server.Hubs
{
    public class MarginHub : Hub
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public MarginHub(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        public void GetMarginUpdates(IEnumerable<string> accounts)
        {
            _subscriptionManager.RegisterClientSubscription(
                Context.ConnectionId,
                accounts,
                updates => Clients.Caller.update(updates.Select(u => u.Margin).Average()));
        }
    }
}
