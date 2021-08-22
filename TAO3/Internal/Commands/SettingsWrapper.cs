using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Commands
{
    internal class SettingsWrapper<TSettings>
    {
        public TSettings? Settings { get; init; }
    }
}
