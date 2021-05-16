using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Windows;
using TAO3.Services.Notepad.Facade;

namespace TAO3.Services.Notepad
{
    public interface INotepadService
    {
        string[] Tabs { get; }
        void Start();
        void FileNew();
        string GetText();
    }

    internal class NotepadService : INotepadService
    {
        private NppProcess? _nppProcess;

        public string[] Tabs => GetTabs();

        public void Start()
        {
            Process.Start("cmd.exe", "/C start notepad++");
        }

        public void FileNew()
        {
            NppProcess npp = GetNppOrThrow();

            User32.SendMessage(npp.NppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, 0, (IntPtr)NppMenuCmd.IDM_FILE_NEW);
        }

        //https://stackoverflow.com/questions/573814/retrieve-text-from-a-scintilla-control-using-sendmessage
        public string GetText()
        {
            NppProcess npp = GetNppOrThrow();

            int length = User32.SendMessage(npp.ScintillaMainHandle, (uint)SciMsg.SCI_GETTEXTLENGTH, 0, 0).ToInt32();

            if (length == 0)
            {
                return "";
            }

            IntPtr memPtr = Kernel32.VirtualAllocEx(npp.Process.Handle, IntPtr.Zero, (IntPtr)(length + 1), Kernel32.AllocationType.Reserve | Kernel32.AllocationType.Commit, Kernel32.MemoryProtection.ReadWrite);
            try
            {
                User32.SendMessage(npp.ScintillaMainHandle, (uint)SciMsg.SCI_GETTEXT, length + 1, memPtr);

                byte[] textBuffer = new byte[length + 1];

                Kernel32.ReadProcessMemory(npp.Process.Handle, memPtr, textBuffer, length + 1, out IntPtr bytesRead);
                return Encoding.UTF8.GetString(textBuffer).TrimEnd('\0');
            }
            finally
            {
                Kernel32.VirtualFreeEx(npp.Process.Handle, memPtr, length + 1, Kernel32.FreeType.Release);
            }
        }

        //https://community.notepad-plus-plus.org/topic/18290/external-sendmessage-to-notepad-for-nppm_getopenfilenames-and-other-tchar/2
        //to do: refactor
        private string[] GetTabs()
        {
            NppProcess npp = GetNppOrThrow();

            int fileCount = (int)User32.SendMessage(npp.NppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, (int)NppMsg.ALL_OPEN_FILES);

            if (fileCount == 0)
            {
                return Array.Empty<string>();
            }
            IntPtr[]? filesPtr = null;
            IntPtr memPtr = IntPtr.Zero;

            int size = IntPtr.Size * fileCount;
            try
            {
                int pathSize = 1024;

                filesPtr = Enumerable.Range(0, fileCount)
                    .Select(x => Kernel32.VirtualAllocEx(npp.Process.Handle, IntPtr.Zero, (IntPtr)pathSize, Kernel32.AllocationType.Reserve | Kernel32.AllocationType.Commit, Kernel32.MemoryProtection.ReadWrite))
                    .ToArray();

                byte[] filesPtrArrayBytes = new byte[size];
                Buffer.BlockCopy(filesPtr, 0, filesPtrArrayBytes, 0, filesPtrArrayBytes.Length);

                memPtr = Kernel32.VirtualAllocEx(npp.Process.Handle, IntPtr.Zero, (IntPtr)size, Kernel32.AllocationType.Reserve | Kernel32.AllocationType.Commit, Kernel32.MemoryProtection.ReadWrite);

                Kernel32.WriteProcessMemory(npp.Process.Handle, memPtr, filesPtrArrayBytes, size, out IntPtr lpNumberOfBytesWritten);

                int count = (int)User32.SendMessage(npp.NppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, memPtr, (IntPtr)fileCount);

                byte[] buffer = new byte[pathSize];

                string[] files = filesPtr
                    .Select(filePtr =>
                    {
                        Kernel32.ReadProcessMemory(npp.Process.Handle, filePtr, buffer, pathSize, out IntPtr bytesRead);
                        return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
                    })
                    .ToArray();

                return files;

            }
            finally
            {
                if (filesPtr != null)
                {
                    foreach (IntPtr filePtr in filesPtr)
                    {
                        Kernel32.VirtualFreeEx(npp.Process.Handle, filePtr, size, Kernel32.FreeType.Release);
                    }

                    Kernel32.VirtualFreeEx(npp.Process.Handle, memPtr, size, Kernel32.FreeType.Release);
                }
            }
        }

        private NppProcess GetNppOrThrow()
        {
            if (_nppProcess != null)
            {
                return _nppProcess;
            }

            Process[] processes = Process.GetProcessesByName("notepad++");

            if (processes.Length > 0)
            {
                Process process = processes[0];

                process.EnableRaisingEvents = true;
                process.Exited += (o, e) => _nppProcess = null;

                _nppProcess = NppProcess.Create(process);
                return _nppProcess;
            }

            throw new Exception("notepad++ is closed");
        }
    }
}
