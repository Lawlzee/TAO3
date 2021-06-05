using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Windows;
using TAO3.Notepad.Internal;

namespace TAO3.Notepad
{
    public interface INotepadService : IDisposable
    {
        string[] Tabs { get; }
        void Start();
        void FileNew();
        string GetText();
        void SetText(string text);
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

            IntPtr memPtr = npp.VirtualAllocEx(length + 1);
            try
            {
                User32.SendMessage(npp.ScintillaMainHandle, (uint)SciMsg.SCI_GETTEXT, length + 1, memPtr);

                byte[] textBuffer = npp.ReadProcessMemory(memPtr, length + 1);
                return Encoding.UTF8.GetString(textBuffer).TrimEnd('\0');
            }
            finally
            {
                npp.VirtualFreeEx(memPtr, length + 1);
            }
        }

        public void SetText(string text)
        {
            text += "\0";

            NppProcess npp = GetNppOrThrow();

            int length = Encoding.UTF8.GetByteCount(text);

            IntPtr memPtr = npp.VirtualAllocEx(length + 1);
            try
            {
                npp.WriteProcessMemory(memPtr, Encoding.UTF8.GetBytes(text));
                User32.SendMessage(npp.ScintillaMainHandle, (uint)SciMsg.SCI_SETTEXT, 0, memPtr);
            }
            finally
            {
                npp.VirtualFreeEx(memPtr, length);
            }
        }

        //https://community.notepad-plus-plus.org/topic/18290/external-sendmessage-to-notepad-for-nppm_getopenfilenames-and-other-tchar/2
        private string[] GetTabs()
        {
            NppProcess npp = GetNppOrThrow();

            int fileCount = (int)User32.SendMessage(npp.NppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, (int)NppMsg.ALL_OPEN_FILES);

            if (fileCount == 0)
            {
                return Array.Empty<string>();
            }

            IntPtr[]? filesPtr = null;
            IntPtr fileNamesPtr = IntPtr.Zero;

            int fileNamePtrsSize = IntPtr.Size * fileCount;
            const int pathSize = 1024;
            try
            {
                filesPtr = npp.VirtualAllocEx(pathSize, fileCount);

                byte[] filesPtrArrayBytes = new byte[fileNamePtrsSize];
                Buffer.BlockCopy(filesPtr, 0, filesPtrArrayBytes, 0, filesPtrArrayBytes.Length);

                fileNamesPtr = npp.VirtualAllocEx(fileNamePtrsSize);
                npp.WriteProcessMemory(fileNamesPtr, filesPtrArrayBytes);

                int count = (int)User32.SendMessage(npp.NppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, fileNamesPtr, (IntPtr)fileCount);

                string[] files = npp.ReadProcessMemory(filesPtr, pathSize)
                    .Select(buffer => Encoding.Unicode.GetString(buffer).TrimEnd('\0'))
                    .ToArray();

                return files;

            }
            finally
            {
                if (filesPtr != null)
                {
                    npp.VirtualFreeEx(filesPtr, fileNamePtrsSize);
                    npp.VirtualFreeEx(fileNamesPtr, fileNamePtrsSize);
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

        public void Dispose()
        {

        }

        
    }
}
