using TAO3.TypeProvider;

namespace TAO3.IO;

public record ProvideSourceArguments(
    string Id,
    string Name,
    string Text);


public record DeserializeArguments(
    string Id,
    Type Type);

public interface IIntermediateSource<TOptions> : ISource
{
    Task<IDomType> ProvideTypeAsync(string variableName, TOptions options, Func<ProvideSourceArguments, Task<IDomType>> inferChildTypeAsync);
    Task<T> GetAsync<T>(TOptions options, Func<DeserializeArguments, object?> deserializeChild);
}
