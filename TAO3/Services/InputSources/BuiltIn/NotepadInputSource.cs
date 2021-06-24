using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Notepad;

namespace TAO3.InputSources
{
    internal class NotepadInputSource : IInputSource
    {
        private readonly INotepadService _notepad;
        public string Name => "notepad";

        public NotepadInputSource(INotepadService notepad)
        {
            _notepad = notepad;
        }

        public Task<string> GetTextAsync()
        {
            return Task.Run(_notepad.GetText);
        }
    }
}
