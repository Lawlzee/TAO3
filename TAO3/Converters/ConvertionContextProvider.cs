using Microsoft.DotNet.Interactive;
using System;
using System.Threading.Tasks;
using TAO3.Services;

namespace TAO3.Converters
{
    public sealed class ConvertionContextProvider
    {
        private readonly IConverter _converter;
        private readonly IInputSourceService _inputSource;

        internal ConvertionContextProvider(IConverter converter, IInputSourceService inputSourceService)
        {
            _converter = converter;
            _inputSource = inputSourceService;
        }

        public IConverterContext<TSettings> Invoke<TSettings>(string source, string name, string settings, bool verbose, KernelInvocationContext context)
        {
            return new ConverterContext<TSettings>(_converter, name, settings, verbose, context, () => _inputSource.GetTextAsync(source, context));
        }

        public IConverterContext<object> Invoke(string source, string name, string settings, bool verbose, KernelInvocationContext context)
        {
            return new ConverterContext<object>(_converter, name, settings, verbose, context, () => _inputSource.GetTextAsync(source, context));
        }
    }
}
