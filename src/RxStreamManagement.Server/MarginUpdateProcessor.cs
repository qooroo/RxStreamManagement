using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace RxStreamManagement.Server
{
    public static class MarginUpdateProcessor
    {
        public static IObservable<int> MaxValueRollingBuffer(
            this IObservable<MarginUpdate> source,
            TimeSpan bufferLength,
            TimeSpan processFrequency,
            IScheduler scheduler)
        {
            return Observable.Create<int>(observer =>
            {
                var initial = source.Take(bufferLength, scheduler)
                    .Scan(0, (a, s) => Math.Max(a, s.Margin))
                    .Buffer(processFrequency, scheduler)
                    .Where(buffer => buffer.Any())
                    .Select(buffer => buffer.Last());

                var windowed = source
                    .Buffer(bufferLength, processFrequency, scheduler)
                    .Where(buffer => buffer.Any())
                    .Select(buffer => buffer.Max(i => i.Margin));

                return initial.Merge(windowed, scheduler).Subscribe(observer.OnNext);
            });
        }
    }
}
