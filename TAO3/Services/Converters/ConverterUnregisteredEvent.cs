using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Converters
{
    public class ConverterUnregisteredEvent : IConverterServiceEvent
    {
        public IConverter Converter { get; }

        public ConverterUnregisteredEvent(IConverter converter)
        {
            Converter = converter;
        }
    }
}
