using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.IO
{
    public interface IDestination
    {
        public string Name { get; }
        IReadOnlyList<string> Aliases { get; }

        public Task SetTextAsync(string text);
    }
}
