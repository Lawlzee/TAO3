using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Notepad;

namespace TAO3.IO
{
    internal class NotepadIO : ISource, IDestination
    {
        private readonly INotepadService _notepad;
        public string Name => "notepad";

        public IReadOnlyList<string> Aliases => new[] { "Notepad", "notepad++", "Notepad++", "npp", "n++" };

        public NotepadIO(INotepadService notepad)
        {
            _notepad = notepad;
        }

        public Task<string> GetTextAsync()
        {
            return Task.Run(_notepad.GetText);
        }

        public Task SetTextAsync(string text)
        {
            _notepad.SetText(text);
            return Task.CompletedTask;
        }
    }
}
