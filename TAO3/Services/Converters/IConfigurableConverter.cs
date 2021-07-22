using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IConfigurableConverter
    {
        void Configure(Command command);
    }
}
