using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Notepad;

namespace TAO3.OutputDestinations
{
    internal class NotepadOutputDestination : IOutputDestination
    {
        private readonly INotepadService _notepad;

        public string Name => "notepad";

        public NotepadOutputDestination(INotepadService notepad)
        {
            _notepad = notepad;
        }

        public Task SetTextAsync(string text, KernelInvocationContext context)
        {
            _notepad.SetText(text);
            return Task.CompletedTask;
        }
    }
}
