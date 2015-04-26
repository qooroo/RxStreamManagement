using System;
using System.Linq;
using System.Reactive.Linq;

namespace RxStreamManagement.Server
{
    public static class MarginUpdateProcessor
    {
        public static IObservable<int> HighestRolling(
            this IObservable<MarginUpdate> src,
            TimeSpan bufferLength,
            TimeSpan processFrequency)
        {
            return Observable.Create<int>(observer =>
            {
                var initial = src.Take(bufferLength).Scan(0, (a, s) => Math.Max(a, s.Margin));

                var windowed = src
                    .Buffer(bufferLength, processFrequency)
                    .Select(buffer => buffer.Max(i => i.Margin));

                return initial.Merge(windowed).Subscribe(observer.OnNext);
            });
        }
    }
}
