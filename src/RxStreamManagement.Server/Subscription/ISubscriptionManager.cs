using System;
using System.Collections.Generic;

namespace RxStreamManagement.Server.Subscription
{
    public interface ISubscriptionManager
    {
        void Initialise();

        void RegisterClientSubscription(
            string clientId,
            IEnumerable<string> accounts,
            Action<IList<MarginUpdate>> action);
    }
}