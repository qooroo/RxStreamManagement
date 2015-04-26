using System;
using System.Reactive.Linq;

namespace RxStreamManagement.Server.DataGenerator
{
    public class MarginUpdatetGenerator
    {
        private readonly Random _random = new Random();
        public IObservable<MarginUpdate> GenerateMarginUpdateStream(TimeSpan interval)
        {
            return Observable.Create<MarginUpdate>(observer =>
            {
                return Observable.Interval(interval)
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
