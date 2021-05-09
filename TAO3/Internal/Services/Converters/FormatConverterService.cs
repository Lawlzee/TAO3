using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Internal.Services
{
    internal class FormatConverterService : IFormatConverterService
    {
        private readonly Dictionary<string, IConverter> _converters;
        private readonly Subject<IConverterServiceEvent> _events;
        public IObservable<IConverterServiceEvent> Events => _events;

        public FormatConverterService()
        {
            _converters = new Dictionary<string, IConverter>();
            _events = new();
        }

        public void Register(IConverter converter)
        {
            _converters[converter.Format] = converter;
            _events.OnNext(new ConverterRegisteredEvent(converter));
        }

        public bool UnregisterConverter(string format)
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
