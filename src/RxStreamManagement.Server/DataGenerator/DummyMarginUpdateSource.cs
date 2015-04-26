using System;
using System.Reactive.Linq;

namespace RxStreamManagement.Server.DataGenerator
{
    public class DummyMarginUpdateSource : IMarginUpdateSource
    {
        private readonly Random _random = new Random();
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

        public IObservable<MarginUpdate> MarginUpdateStream()
        {
            return Observable.Create<MarginUpdate>(observer =>
            {
                return Observable.Interval(_interval)
                    .Subscribe(_ =>
                        observer.OnNext(new MarginUpdate
                        {
                            Account = _random.Next(3).ToString(),
                            Margin = _random.Next(100)
                        }));
            });
        }

        public static MarginUpdate WithMargin(int margin)
        {
            return new MarginUpdate { Margin = margin };
        }
    }
}
