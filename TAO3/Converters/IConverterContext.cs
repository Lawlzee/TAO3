using Microsoft.AspNetCore.Connections;
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

namespace TAO3.Converters
{

    public interface IConverterContext<TSettings>
    {
        IConverter Converter { get; }
        string VariableName { get; }
        CSharpKernel CSharpKernel { get; }
        TSettings? Settings { get; set; }
        Task<string> GetTextAsync();
        Task SubmitCodeAsync(string code);
        Task DefaultHandle();
        Task<string> CreatePrivateVariable(object? value, Type type);
    }

    internal class ConverterContext<TSettings> : IConverterContext<TSettings>
    {
        private readonly string _settingsName;
        private readonly bool _verbose;
        private readonly KernelInvocationContext _context;
        private readonly Func<Task<string>> _getTextAsync;
        private string? _text;

        public IConverter Converter { get; }
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
            Converter = converter;
            VariableName = name;
            _verbose = verbose;
            _settingsName = settings;
            _context = context;
            _getTextAsync = getTextAsync;

            CSharpKernel = (CSharpKernel)context.HandlingKernel.FindKernel("csharp");

            if (!string.IsNullOrEmpty(_settingsName) && CSharpKernel.TryGetVariable(_settingsName, out TSettings s))
            {
                Settings = s;
            }
        }

        public async Task<string> GetTextAsync()
        {
            if (_text == null)
            {
                _text = await _getTextAsync();
            }
            return _text;
        }

        public async Task SubmitCodeAsync(string code)
        {
            if (_verbose)
            {
                _context.Display(code, null);
            }

            await CSharpKernel.SubmitCodeAsync(code);
        }

        public async Task DefaultHandle()
        {
            string text = await GetTextAsync();
            object? result = Converter.Deserialize<ExpandoObject>(text, Settings);

            await SubmitCodeAsync($"{Converter.DefaultType} {VariableName} = null;");
            CSharpKernel.ScriptState.GetVariable(VariableName).Value = result;
        }

        public async Task<string> CreatePrivateVariable(object? value, Type type)
        {
            string name = $"__internal_{Guid.NewGuid().ToString("N")}";
            await CSharpKernel.SetVariableAsync(name, value, type);
            return name;
}
    }
}
