using Microsoft.DotNet.Interactive;
using System;
using System.Threading.Tasks;
using TAO3.InputSources;

namespace TAO3.Converters
{
    public sealed class ConvertionContextProvider
    {
        private readonly IConverter _converter;
        private readonly IInputSource _inputSource;

        internal ConvertionContextProvider(IConverter converter, IInputSource inputSource)
        {
            _converter = converter;
            _inputSource = inputSource;
        }

        public IConverterContext<TSettings> Invoke<TSettings>(string name, string settings, bool verbose, KernelInvocationContext context)
        {
            return new ConverterContext<TSettings>(_converter, name, settings, verbose, context, () => _inputSource.GetTextAsync( context));
        }

        public IConverterContext<object> Invoke(string name, string settings, bool verbose, KernelInvocationContext context)
        {
            return new ConverterContext<object>(_converter, name, settings, verbose, context, () => _inputSource.GetTextAsync(context));
        }
    }
}
