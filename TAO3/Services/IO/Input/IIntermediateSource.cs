using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;
using TAO3.TypeProvider;

namespace TAO3.IO
{
    public record ProvideSourceArguments(
        string Id,
        string Text);


    public record DeserializeArguments(
        string Id,
        Type Type);

    public interface IIntermediateSource<TOptions> : ISource
    {
        Task<IDomType> ProvideTypeAsync(TOptions options, Func<ProvideSourceArguments, Task<IDomType>> inferChildTypeAsync);
        Task<T> GetAsync<T>(TOptions options, Func<DeserializeArguments, object?> deserializeChild);
    }
}
