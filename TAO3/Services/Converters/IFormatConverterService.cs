using System;
using TAO3.Converters;

namespace TAO3.Converters
{
    public interface IFormatConverterService : IDisposable
    {
        IObservable<IConverterServiceEvent> Events { get; }
        IConverter? TryGetConverter(string format);
        void Register(IConverter converter);
        bool UnregisterConverter(string format);
    }
}
