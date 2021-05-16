using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Windows;

namespace TAO3.Services.Notepad
{
    internal class NppProcess
    {
        public Process Process { get; }
        public IntPtr NppHandle => Process.MainWindowHandle;
        public IntPtr ScintillaMainHandle { get; }
        public IntPtr ScintillaSecondHandle { get; }

        public NppProcess(Process process, IntPtr scintillaMainHandle, IntPtr scintillaSecondHandle)
        {
            Process = process;
            ScintillaMainHandle = scintillaMainHandle;
            ScintillaSecondHandle = scintillaSecondHandle;
        }

        //https://community.notepad-plus-plus.org/topic/17992/how-to-get-the-scintilla-view0-view1-hwnds/8?lang=en-US
        public static NppProcess Create(Process process)
        {
            IntPtr scintillaMainHandle = IntPtr.Zero;
            IntPtr scintillaSecondHandle = IntPtr.Zero;

            User32.EnumWindowProc childProc = EnumWindow;
            User32.EnumChildWindows(process.MainWindowHandle, childProc, IntPtr.Zero);

            return new NppProcess(
                process,
                scintillaMainHandle,
                scintillaSecondHandle);

            bool EnumWindow(IntPtr hWnd, IntPtr lParam)
            {
                const string scintillaClassName = "Scintilla";

                StringBuilder sb = new StringBuilder(scintillaClassName.Length + 1);
                User32.GetClassNameW(hWnd, sb, scintillaClassName.Length + 1);

                if (scintillaClassName == sb.ToString())
                {
                    if (User32.GetParent(hWnd) == process.MainWindowHandle)
                    {
                        if (scintillaMainHandle == IntPtr.Zero)
                        {
                            scintillaMainHandle = hWnd;
                        }
                        else if (scintillaSecondHandle == IntPtr.Zero)
                        {
                            scintillaSecondHandle = hWnd;
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
