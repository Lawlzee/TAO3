using Microsoft.DotNet.Interactive;
using System;
using System.Threading.Tasks;
using TAO3.IO;

namespace TAO3.Converters
{
    public sealed class ConvertionContextProvider
    {
        private readonly IConverter _converter;
        private readonly ISource _source;

        internal ConvertionContextProvider(IConverter converter, ISource source)
        {
            _converter = converter;
            _source = source;
        }

        internal IConverterContext<TSettings> Invoke<TSettings>(string name, string settings, bool verbose, KernelInvocationContext context)
        {
            return new ConverterContext<TSettings>(_converter, name, settings, verbose, context, () => _source.GetTextAsync());
        }

        internal IConverterContext<object> Invoke(string name, string settings, bool verbose, KernelInvocationContext context)
        {
            return new ConverterContext<object>(_converter, name, settings, verbose, context, () => _source.GetTextAsync());
        }
    }
}
