using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Converters
{
    public class ConverterRegisteredEvent : IConverterServiceEvent
    {
        public IConverter Converter { get; }

        public ConverterRegisteredEvent(IConverter converter)
        {
            Converter = converter;
        }
    }
}
