using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.TypeProvider;

namespace TAO3.Converters
{
    public interface IConverter
    {
        string Format { get; }
        IReadOnlyList<string> Aliases { get; }
        string MimeType { get; }
        Dictionary<string, object> Properties { get; }
    }

    public interface IConverter<T, TSettings> : IConverter
    {
        string Serialize(T value, TSettings? settings);
        T Deserialize(string text, TSettings? settings);
    }

    public interface IConverter<T> : IConverter<T, Unit>
    {
        string Serialize(object? value);
        T Deserialize(string text);

        string IConverter<T, Unit>.Serialize(T value, Unit _) => Serialize(value);
        T IConverter<T, Unit>.Deserialize(string text, Unit _) => Deserialize(text);
    }

    public interface IInputConfigurableConverter<TSettings, TCommandParameters>
    {
        void Configure(Command command);
        TSettings GetDefaultSettings();
        TSettings BindParameters(TSettings settings, TCommandParameters args);
    }

    public interface IConverterTypeProvider<TSettings, TCommandParameters> : 
        IConverter, 
        IInputConfigurableConverter<TSettings, TCommandParameters>
    {
        string Serialize(object? value, TSettings? settings);
        T Deserialize<T>(string text, TSettings? settings);

        IDomCompiler DomCompiler { get; }
        Task<IDomType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args);
    }

    public interface IConverterTypeProvider<TCommandParameters> : IConverterTypeProvider<Unit, TCommandParameters>
    {
        string Serialize(object? value);
        T Deserialize<T>(string text);

        string IConverterTypeProvider<Unit, TCommandParameters>.Serialize(object? value, Unit _) => Serialize(value);
        T IConverterTypeProvider<Unit, TCommandParameters>.Deserialize<T>(string text, Unit _) => Deserialize<T>(text);

        Unit IInputConfigurableConverter<Unit, TCommandParameters>.GetDefaultSettings() => Unit.Default;
        Unit IInputConfigurableConverter<Unit, TCommandParameters>.BindParameters(Unit unit, TCommandParameters args) => unit;
        
        Task<IDomType> IConverterTypeProvider<Unit, TCommandParameters>.ProvideTypeAsync(IConverterContext<Unit> context, TCommandParameters args) => ProvideTypeAsync(context, args);
        Task<IDomType> ProvideTypeAsync(IConverterContext context, TCommandParameters args);

    }

    public interface IConverterTypeProvider : IConverterTypeProvider<Unit>
    {
        void IInputConfigurableConverter<Unit, Unit>.Configure(Command command) { }
        
        Task<IDomType> IConverterTypeProvider<Unit>.ProvideTypeAsync(IConverterContext context, Unit args) => ProvideTypeAsync(context);
        Task<IDomType> ProvideTypeAsync(IConverterContext context);
    }
}
