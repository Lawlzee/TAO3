using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using TAO3.Converters;

namespace TAO3.Converters
{
    public interface IFormatConverterService : IDisposable
    {
        IObservable<IConverterEvent> Events { get; }
        IConverter? TryGetConverter(string format);
        void Register(IConverter converter);
        bool Remove(string format);
    }

    public class FormatConverterService : IFormatConverterService
    {
        private readonly Dictionary<string, IConverter> _converters;
        private readonly ReplaySubject<IConverterEvent> _events;
        public IObservable<IConverterEvent> Events => _events;

        public FormatConverterService()
        {
            _converters = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register(IConverter converter)
        {
            _converters[converter.Format] = converter;
            _events.OnNext(new ConverterRegisteredEvent(converter));
        }

        public bool Remove(string format)
        {
            IConverter? converter = _converters.GetValueOrDefault(format);
            if (converter == null)
            {
                return false;
            }

            _converters.Remove(format);
            _events.OnNext(new ConverterUnregisteredEvent(converter!));

            return true;
        }

        public IConverter? TryGetConverter(string format)
        {
            return _converters.GetValueOrDefault(format);
        }

        public void Dispose()
        {
            _events.Dispose();
        }
    }
}
