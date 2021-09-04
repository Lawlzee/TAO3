using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using TAO3.Converters;

namespace TAO3.Converters
{
    public interface IConverterService : IDisposable
    {
        IReadOnlyCollection<IConverter> Converters { get; }
        IObservable<IConverterEvent> Events { get; }
        IConverter? TryGetConverter(string format);
        void Register<T, TSettings>(IConverter<T, TSettings> converter);
        void Register<TSettings, TCommandParameters>(IConverterTypeProvider<TSettings, TCommandParameters> converter);
        bool Remove(string format);
    }

    public class ConverterService : IConverterService
    {
        private readonly Dictionary<string, IConverter> _converters;
        private readonly ReplaySubject<IConverterEvent> _events;
        public IObservable<IConverterEvent> Events => _events;
        public IReadOnlyCollection<IConverter> Converters => _converters.Values;

        public ConverterService()
        {
            _converters = new(StringComparer.OrdinalIgnoreCase);
            _events = new();
        }

        public void Register<T, TSettings>(IConverter<T, TSettings> converter)
        {
            DoRegister(converter);
        }

        public void Register<TSettings, TCommandParameters>(IConverterTypeProvider<TSettings, TCommandParameters> converter)
        {
            DoRegister(converter);
        }

        private void DoRegister(IConverter converter)
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
