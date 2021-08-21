using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{
    public interface ISource
    {
        string Name { get; }
        IReadOnlyList<string> Aliases { get; }
    }

    public interface ISource<TOptions> : ISource
    {
        Task<string> GetTextAsync(TOptions options);
    }
}
