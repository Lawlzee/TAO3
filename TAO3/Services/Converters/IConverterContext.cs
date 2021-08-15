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

    public interface IConverterContext<TSettings>
    {
        string VariableName { get; }
        CSharpKernel CSharpKernel { get; }
        TSettings? Settings { get; set; }
        Task<string> GetTextAsync();
        Task SubmitCodeAsync(string code);
        Task DefaultHandleCommandAsync();
        Task<string> CreatePrivateVariableAsync(object? value, Type type);
    }

    internal class ConverterContext<TSettings> : IConverterContext<TSettings>
    {
        private readonly string _settingsName;
        private readonly bool _verbose;
        private readonly KernelInvocationContext _context;
        private readonly Func<Task<string>> _getTextAsync;
        private string? _text;
        private bool _textInitialized = false;
        private readonly IConverter _converter;

        public string VariableName { get; }
        public TSettings? Settings { get; set; }

        public CSharpKernel CSharpKernel { get; }

        public ConverterContext(
            IConverter converter,
            string name,
            string settings,
            bool verbose,
            KernelInvocationContext context,
            Func<Task<string>> getTextAsync)
        {
            _converter = converter;
            VariableName = name;
            _verbose = verbose;
            _settingsName = settings;
            _context = context;
            _getTextAsync = getTextAsync;

            CSharpKernel = context.GetCSharpKernel();

            if (!string.IsNullOrEmpty(_settingsName) && CSharpKernel.TryGetVariable(_settingsName, out TSettings s))
            {
                Settings = s;
            }
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

        public async Task SubmitCodeAsync(string code)
        {
            if (_verbose)
            {
                _context.Display(code);
            }

            await CSharpKernel.SubmitCodeAsync(code);
        }

        public async Task DefaultHandleCommandAsync()
        {
            string text = await GetTextAsync();
            object? result = _converter.Deserialize<ExpandoObject>(text, Settings);

            await SubmitCodeAsync($"{_converter.DefaultType} {VariableName} = null;");
            CSharpKernel.ScriptState.GetVariable(VariableName).Value = result;
        }

        public async Task<string> CreatePrivateVariableAsync(object? value, Type type)
        {
            string name = $"__internal_{Guid.NewGuid().ToString("N")}";
            await CSharpKernel.SetVariableAsync(name, value, type);
            return name;
}
    }
}
