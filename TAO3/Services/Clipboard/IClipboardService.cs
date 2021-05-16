using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Clipboard
{
    public interface IClipboardService
    {
        Task ClearAsync();
        Task<string> GetTextAsync();
        Task SetTextAsync(string text);
    }
}
