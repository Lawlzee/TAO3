using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Extensions
{
    internal static class CommandExtensions
    {
        public static void AddAliases(this Command command, IEnumerable<string> aliases)
        {
            foreach (var alias in aliases)
            {
                command.AddAlias(alias);
            }
        }
    }
}
