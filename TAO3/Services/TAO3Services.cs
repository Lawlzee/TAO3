using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Services.Clipboard;
using TAO3.Services.Keyboard;
using TAO3.Services.Notepad;
using TAO3.Services.Toast;

namespace TAO3.Services
{
    public class TAO3Services
    {
        public IExcelService Excel { get; }
        public INotepadService Notepad { get; }
        public IKeyboardService Keyboard { get; }
        public IClipboardService Clipboard { get; }
        public IToastService Toast { get; }
        public IFormatConverterService FormatConverter { get; }
        public IInputSourceService InputSource { get; }

        public TAO3Services(IExcelService excel, INotepadService notepad, IKeyboardService keyboard, IClipboardService clipboard, IToastService toast, IFormatConverterService formatConverter, IInputSourceService inputSource)
        {
            Excel = excel;
            Notepad = notepad;
            Keyboard = keyboard;
            Clipboard = clipboard;
            Toast = toast;
            FormatConverter = formatConverter;
            InputSource = inputSource;
        }
    }
}
