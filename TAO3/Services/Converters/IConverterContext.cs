using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;

namespace TAO3.Converters
{
    public interface IConverterContext
    {
        string VariableName { get; }
        Task<string> GetTextAsync();
    }

    public interface IConverterContext<TSettings> : IConverterContext
    {
        TSettings? Settings { get; }
    }

    internal class ConverterContext<TSettings> : IConverterContext<TSettings>
    {
        private readonly Func<Task<string>> _getTextAsync;
        private string? _text;
        private bool _textInitialized = false;

        public string VariableName { get; }
        public TSettings? Settings { get; }

        public ConverterContext(
            string name,
            TSettings? settings,
            Func<Task<string>> getTextAsync)
        {
            VariableName = name;
            Settings = settings;
            _getTextAsync = getTextAsync;

        }

        public async Task<string> GetTextAsync()
        {
            if (!_textInitialized)
            {
                _text = await _getTextAsync();
                _textInitialized = true;
            }
            return _text!;
        }
    }
}
