using System;
using System.Reactive.Linq;

namespace RxStreamManagement.Server.DataGenerator
{
    public class MarginUpdatetGenerator
    {
        private readonly Random _random = new Random();
        public IObservable<MarginUpdate> MarginUpdateStream()
        {
            return Observable.Create<MarginUpdate>(observer =>
            {
                return Observable.Interval(TimeSpan.FromMilliseconds(1000))
                    .Subscribe(_ =>
                        observer.OnNext(new MarginUpdate
                        {
                            Account = _random.Next(3).ToString(),
                            Margin = _random.Next(100)
                        }));
            });
        } 

    }
}
