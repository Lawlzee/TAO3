using System.Reflection;
using TAO3.Converters;
using TAO3.TypeProvider;

namespace TAO3.Internal.Commands.Input;

//Must be public, because the type is used in a SubmitCodeCommand
public interface IConverterDeserializerAdapter<TSettings>
{
    private static MethodInfo _deserializeMethodInfo = typeof(IConverterDeserializerAdapter<TSettings>)
        .GetMethods()
        .Where(x => x.IsGenericMethod)
        .Where(x => x.Name == nameof(Deserialize))
        .First();

    T Deserialize<T>(string text, TSettings? settings);

    //todo: optimise
    object? Deserialize(Type type, string text, TSettings? settings)
    {
        return _deserializeMethodInfo.MakeGenericMethod(type)
           .Invoke(this, new object?[] { text, settings });
    }
}

internal interface IConverterTypeProviderAdapter<TSettings, TCommandParameters>
    : IConverterDeserializerAdapter<TSettings>
{
    IDomCompiler DomCompiler { get; }
    Task<IDomType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args);
}

internal class DefaultConverterTypeProviderAdapter<T, TSettings, TCommandParameters>
    : IConverterTypeProviderAdapter<TSettings, TCommandParameters>
{
    private readonly IConverter<T, TSettings> _converter;

    public IDomCompiler DomCompiler { get; }

    public DefaultConverterTypeProviderAdapter(IConverter<T, TSettings> converter)
    {
        _converter = converter;
        DomCompiler = new DomCompiler(
            _converter.Format,
            IDomSchematizer.Default,
            IDomSchemaSerializer.Default);
    }

    public Task<IDomType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args)
    {
        return Task.FromResult<IDomType>(new DomClassReference(typeof(T)));
    }

    public TResult Deserialize<TResult>(string text, TSettings? settings)
    {
        return (TResult)(object)_converter.Deserialize(text, settings)!;
    }
}

internal class ConverterTypeProviderAdapter<TSettings, TCommandParameters>
    : IConverterTypeProviderAdapter<TSettings, TCommandParameters>
{
    private readonly IConverterTypeProvider<TSettings, TCommandParameters> _converter;
    public IDomCompiler DomCompiler => _converter.DomCompiler;

    public ConverterTypeProviderAdapter(IConverterTypeProvider<TSettings, TCommandParameters> converter)
    {
        _converter = converter;
    }

    public Task<IDomType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args)
    {
        return _converter.ProvideTypeAsync(context, args);
    }

    public T Deserialize<T>(string text, TSettings? settings)
    {
        return _converter.Deserialize<T>(text, settings);
    }
}
