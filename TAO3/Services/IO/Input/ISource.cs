using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{
    public interface ISource
    {
        string Name { get; }
        IReadOnlyList<string> Aliases { get; }

        Task<string> GetTextAsync();
    }
}
