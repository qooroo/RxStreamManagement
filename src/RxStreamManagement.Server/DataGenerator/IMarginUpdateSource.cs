using System;

namespace RxStreamManagement.Server.DataGenerator
{
    public interface IMarginUpdateSource
    {
        IObservable<MarginUpdate> MarginUpdateStream();
    }
}