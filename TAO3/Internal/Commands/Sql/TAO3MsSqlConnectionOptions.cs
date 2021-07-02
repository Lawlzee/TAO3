using Microsoft.DotNet.Interactive.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands.Sql
{
    internal class TAO3MsSqlConnectionOptions : KernelConnectionOptions
    {
        public string? ConnectionString { get; set; }
    }
}
