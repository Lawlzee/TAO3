using System;
using TAO3.Converters;

namespace TAO3.Internal.Services
{
    internal interface IFormatConverterService : IDisposable
    {
        IObservable<IConverterServiceEvent> Events { get; }
        IConverter? TryGetConverter(string format);
        void RegisterConverter(IConverter converter);
        bool UnregisterConverter(string format);
    }
}
