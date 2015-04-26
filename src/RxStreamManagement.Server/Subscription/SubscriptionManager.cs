using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using RxStreamManagement.Server.DataGenerator;

namespace RxStreamManagement.Server.Subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IMarginUpdateSource _marginUpdateSource;
        private readonly Dictionary<string, Subject<MarginUpdate>> _marginUpdatesByAccount;
        private readonly Dictionary<string, SerialDisposable> _clientSubscriptions;

        public SubscriptionManager(IMarginUpdateSource marginUpdateSource)
        {
            _marginUpdateSource = marginUpdateSource;
            _marginUpdatesByAccount = new Dictionary<string, Subject<MarginUpdate>>();
            _clientSubscriptions = new Dictionary<string, SerialDisposable>();
        }

        public void Initialise()
        {
            _marginUpdateSource.MarginUpdateStream()
                .Subscribe(u =>
                {
                    if (!_marginUpdatesByAccount.ContainsKey(u.Account))
                    {
                        var accountStream = new Subject<MarginUpdate>();
                        accountStream.Publish().RefCount();

                        _marginUpdatesByAccount.Add(u.Account, accountStream);
                    }
                    _marginUpdatesByAccount[u.Account].OnNext(u);
                });
        }

        public void RegisterClientSubscription(
            string clientId,
            IEnumerable<string> accounts,
            Action<IList<MarginUpdate>> action)
        {
            if (!_clientSubscriptions.ContainsKey(clientId))
            {
                _clientSubscriptions.Add(clientId, new SerialDisposable());
            }

            _clientSubscriptions[clientId].Disposable = _marginUpdatesByAccount
                .Where(x => accounts.ToList().Contains(x.Key))
                .Select(x => x.Value)
                .CombineLatest()
                .Subscribe(action);
        }
    }
}
