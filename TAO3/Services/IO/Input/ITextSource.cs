using Microsoft.DotNet.Interactive;
using System;
using System.CommandLine;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{

    public interface ITextSource<TOptions> : ISource
    {
        Task<string> GetTextAsync(TOptions options);
    }

    public interface ITextSource : ITextSource<Unit>
    {
        Task<string> ITextSource<Unit>.GetTextAsync(Unit options) => GetTextAsync();
        Task<string> GetTextAsync();
    }
}
