using System;
using System.Collections.Generic;
using System.Linq;
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

        public void GetMarginUpdates(Tuple<string, IEnumerable<string>> accountsForId)
        {
            _subscriptionManager.RegisterClientSubscription(
                Context.ConnectionId+accountsForId.Item1,
                accountsForId.Item2,
                updates => Clients.Caller.update(Tuple.Create(accountsForId.Item1, updates.Select(u => u.Margin).Average())));
        }
    }
}
