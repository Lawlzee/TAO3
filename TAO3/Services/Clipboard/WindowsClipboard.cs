using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TAO3.Clipboard
{
    internal class WindowsClipboard : IClipboardService
    {
        public Subject<object> Subject { get; }
        public IObservable<object> OnClipboardChange => Subject;

        public WindowsClipboard()
        {
            Subject = new Subject<object>(); ;
        }

        public Task SetTextAsync(string text) => SetTextAsync(text, CancellationToken.None);

        public async Task SetTextAsync(string text, CancellationToken cancellation)
        {
            await TryOpenClipboardAsync(cancellation);

            DoSetText(text);
        }

        public void SetText(string text)
        {
            TryOpenClipboard();

            DoSetText(text);
        }

        private void DoSetText(string text)
        {
            EmptyClipboard();
            IntPtr hGlobal = default;
            try
            {
                int byteCount = (text.Length + 1) * 2;
                hGlobal = Marshal.AllocHGlobal(byteCount);

                if (hGlobal == default)
                {
                    ThrowWin32();
                }

                IntPtr target = GlobalLock(hGlobal);

                if (target == IntPtr.Zero)
                {
                    ThrowWin32();
                }

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                }
                finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData((uint)ClipboardFormat.CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
                {
                    ThrowWin32();
                }

                hGlobal = IntPtr.Zero;
            }
            finally
            {
                if (hGlobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hGlobal);
                }

                CloseClipboard();
            }
        }

        private async Task TryOpenClipboardAsync(CancellationToken cancellation)
        {
            int tryLeft = 20;
            while (true)
            {
                if (OpenClipboard(IntPtr.Zero))
                {
                    break;
                }

                if (--tryLeft == 0)
                {
                    ThrowWin32();
                }

                await Task.Delay(50, cancellation);
            }
        }

        private void TryOpenClipboard()
        {
            int tryLeft = 20;
            while (true)
            {
                if (OpenClipboard(IntPtr.Zero))
                {
                    break;
                }

                if (--tryLeft == 0)
                {
                    ThrowWin32();
                }

                Thread.Sleep(50);
            }
        }

        public Task<string?> GetTextAsync() => GetTextAsync(CancellationToken.None);

        public async Task<string?> GetTextAsync(CancellationToken cancellation)
        {
            if (!IsClipboardFormatAvailable((uint)ClipboardFormat.CF_UNICODETEXT))
            {
                return null;
            }
            await TryOpenClipboardAsync(cancellation);

            return DoGetText();
        }

        public string? GetText()
        {
            if (!IsClipboardFormatAvailable((uint)ClipboardFormat.CF_UNICODETEXT))
            {
                return null;
            }
            TryOpenClipboard();

            return DoGetText();
        }

        private string? DoGetText()
        {
            IntPtr handle = IntPtr.Zero;
            IntPtr lockPointer = IntPtr.Zero;

            try
            {
                handle = GetClipboardData((uint)ClipboardFormat.CF_UNICODETEXT);
                if (handle == IntPtr.Zero)
                {
                    return null;
                }

                lockPointer = GlobalLock(handle);
                if (lockPointer == IntPtr.Zero)
                {
                    return null;
                }

                int size = GlobalSize(handle);
                byte[] buffer = new byte[size];

                Marshal.Copy(lockPointer, buffer, 0, size);

                return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
            }
            finally
            {
                if (lockPointer != IntPtr.Zero)
                {
                    GlobalUnlock(handle);
                }

                CloseClipboard();
            }
        }

        public async Task<List<string>> GetFilesAsync()
        {
            var i = 20;

            while (true)
            {
                var hr = OleGetClipboard(out IDataObject dataObject);

                if (hr == 0)
                {
                    List<string>? formats = GetFilesFromOleHGLOBAL(dataObject);
#pragma warning disable CA1416 // Validate platform compatibility
                    Marshal.ReleaseComObject(dataObject);
#pragma warning restore CA1416 // Validate platform compatibility
                    return formats;
                }

                if (--i == 0)
                    Marshal.ThrowExceptionForHR(hr);

                await Task.Delay(50);
            }
        }

        private List<string> GetFilesFromOleHGLOBAL(IDataObject dataObject)
        {
            FORMATETC formatEtc = new FORMATETC
            {
                cfFormat = (short)ClipboardFormat.CF_HDROP,
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                tymed = TYMED.TYMED_HGLOBAL
            };
            if (dataObject.QueryGetData(ref formatEtc) == 0)
            {
                dataObject.GetData(ref formatEtc, out STGMEDIUM medium);
                try
                {
                    if (medium.unionmember != IntPtr.Zero && medium.tymed == TYMED.TYMED_HGLOBAL)
                    {
                        return DoGetFiles(medium.unionmember);
                        /*
                        byte[] data = ReadBytesFromHGlobal(medium.unionmember);

                        if (IsSerializedObject(data))
                        {
                            using (var ms = new MemoryStream(data))
                            {
                                ms.Position = DataObject.SerializedObjectGUID.Length;
                                BinaryFormatter binaryFormatter = new BinaryFormatter();
                                return binaryFormatter.Deserialize(ms);
                            }
                        }
                        return data;*/
                    }
                }
                finally
                {
                    ReleaseStgMedium(ref medium);
                }
            }
            return new List<string>();
        }

        private List<string> DoGetFiles(IntPtr hGlobal)
        {
            List<string> files = new List<string>();
            int fileCount = DragQueryFile(hGlobal, -1, null, 0);
            if (fileCount > 0)
            {
                for (int i = 0; i < fileCount; i++)
                {
                    int pathLen = DragQueryFile(hGlobal, i, null, 0);
                    StringBuilder sb = new StringBuilder(pathLen + 1);

                    if (DragQueryFile(hGlobal, i, sb, sb.Capacity) == pathLen)
                    {
                        files.Add(sb.ToString());
                    }
                }
            }
            return files;
        }

        private void ThrowWin32()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void Clear()
        {
            EmptyClipboard();
        }

        public void Dispose()
        {

        }

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("User32.dll", SetLastError = true)]
        static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern int GlobalSize(IntPtr hMem);

        [StructLayout(LayoutKind.Sequential)]
        public struct FORMATETC
        {
            public short cfFormat;
            public IntPtr ptd;
            [MarshalAs(UnmanagedType.U4)]
            public DVASPECT dwAspect;
            public int lindex;
            [MarshalAs(UnmanagedType.U4)]
            public TYMED tymed;
        };

        ///// <summary>
        ///// The DVASPECT enumeration values specify the desired data or view aspect of the object when drawing or getting data.
        ///// </summary>
        [Flags]
        public enum DVASPECT
        {
            DVASPECT_CONTENT = 1,
            DVASPECT_THUMBNAIL = 2,
            DVASPECT_ICON = 4,
            DVASPECT_DOCPRINT = 8
        }

        // Summary:
        //     Provides the managed definition of the TYMED structure.
        [Flags]
        public enum TYMED
        {
            // Summary:
            //     No data is being passed.
            TYMED_NULL = 0,
            //
            // Summary:
            //     The storage medium is a global memory handle (HGLOBAL). Allocate the global
            //     handle with the GMEM_SHARE flag. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is null, the destination process should use GlobalFree to release
            //     the memory.
            TYMED_HGLOBAL = 1,
            //
            // Summary:
            //     The storage medium is a disk file identified by a path. If the STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is null, the destination process should use OpenFile to delete the
            //     file.
            TYMED_FILE = 2,
            //
            // Summary:
            //     The storage medium is a stream object identified by an IStream pointer. Use
            //     ISequentialStream::Read to read the data. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is not null, the destination process should use IStream::Release to
            //     release the stream component.
            TYMED_ISTREAM = 4,
            //
            // Summary:
            //     The storage medium is a storage component identified by an IStorage pointer.
            //     The data is in the streams and storages contained by this IStorage instance.
            //     If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is not null, the destination process should use IStorage::Release
            //     to release the storage component.
            TYMED_ISTORAGE = 8,
            //
            // Summary:
            //     The storage medium is a Graphics Device Interface (GDI) component (HBITMAP).
            //     If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is null, the destination process should use DeleteObject to delete
            //     the bitmap.
            TYMED_GDI = 16,
            //
            // Summary:
            //     The storage medium is a metafile (HMETAFILE). Use the Windows or WIN32 functions
            //     to access the metafile's data. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is null, the destination process should use DeleteMetaFile to delete
            //     the bitmap.
            TYMED_MFPICT = 32,
            //
            // Summary:
            //     The storage medium is an enhanced metafile. If the System.Runtime.InteropServices.ComTypes.STGMEDIUMSystem.Runtime.InteropServices.ComTypes.STGMEDIUM.pUnkForRelease
            //     member is null, the destination process should use DeleteEnhMetaFile to delete
            //     the bitmap.
            TYMED_ENHMF = 64,
        }

        public struct STGMEDIUM
        {
            public TYMED tymed;
            public IntPtr unionmember;
            [MarshalAs(UnmanagedType.IUnknown)]
            public object? pUnkForRelease;
        }

        public enum DATADIR
        {
            DATADIR_GET = 1,
            DATADIR_SET = 2
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000103-0000-0000-C000-000000000046")]
        public interface IEnumFORMATETC
        {
            [PreserveSig]
            int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] FORMATETC[] rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);
            [PreserveSig]
            int Skip(int celt);
            [PreserveSig]
            int Reset();
            void Clone(out IEnumFORMATETC newEnum);
        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        [Guid("0000010F-0000-0000-C000-000000000046")]
        [ComImport()]
        public interface IAdviseSink
        {
            void OnDataChange(FORMATETC pFormatetc, STGMEDIUM pStgmed);
            void OnViewChange([MarshalAs(UnmanagedType.U4)] int dwAspect, [MarshalAs(UnmanagedType.I4)] int lindex);
            void OnRename([MarshalAs(UnmanagedType.Interface)] object pmk);
            void OnSave();
            void OnClose();
        }

        [Flags]
        public enum ADVF
        {
            ADVF_NODATA = 1,
            ADVF_PRIMEFIRST = 2,
            ADVF_ONLYONCE = 4,
            ADVF_DATAONSTOP = 64,
            ADVFCACHE_NOHANDLER = 8,
            ADVFCACHE_FORCEBUILTIN = 16,
            ADVFCACHE_ONSAVE = 32
        }

        public struct STATDATA
        {
            public FORMATETC formatetc;
            public ADVF advf;
            public IAdviseSink advSink;
            public int connection;
        }

        [ComImport]
        [Guid("00000105-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumSTATDATA
        {

            /// <summary>
            /// Retrieves the next celt items in the enumeration sequence. If there are
            /// fewer than the requested number of elements left in the sequence, it
            /// retrieves the remaining elements. The number of elements actually
            /// retrieved is returned through pceltFetched (unless the caller passed
            /// in NULL for that parameter).
            /// </summary>
            [PreserveSig]
            int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] STATDATA[] rgelt, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] int[] pceltFetched);

            /// <summary>
            /// Skips over the next specified number of elements in the enumeration sequence.
            /// </summary>
            [PreserveSig]
            int Skip(int celt);

            /// <summary>
            /// Resets the enumeration sequence to the beginning.
            /// </summary>
            [PreserveSig]
            int Reset();

            /// <summary>
            /// Creates another enumerator that contains the same enumeration state as
            /// the current one. Using this function, a client can record a particular
            /// point in the enumeration sequence and then return to that point at a
            /// later time. The new enumerator supports the same interface as the original one.
            /// </summary>
            void Clone(out IEnumSTATDATA newEnum);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000010E-0000-0000-C000-000000000046")]
        public interface IDataObject
        {
            void GetData([In] ref FORMATETC format, out STGMEDIUM medium);
            void GetDataHere([In] ref FORMATETC format, ref STGMEDIUM medium);
            [PreserveSig]
            int QueryGetData([In] ref FORMATETC format);
            [PreserveSig]
            int GetCanonicalFormatEtc([In] ref FORMATETC formatIn, out FORMATETC formatOut);
            void SetData([In] ref FORMATETC formatIn, [In] ref STGMEDIUM medium, [MarshalAs(UnmanagedType.Bool)] bool release);
            IEnumFORMATETC EnumFormatEtc(DATADIR direction);
            [PreserveSig]
            int DAdvise([In] ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);
            void DUnadvise(int connection);
            [PreserveSig]
            int EnumDAdvise(out IEnumSTATDATA enumAdvise);
        }

        [DllImport("ole32.dll", PreserveSig = false)]
        static extern int OleGetClipboard(out IDataObject dataObject);

        [DllImport("shell32.dll", BestFitMapping = false, CharSet = CharSet.Auto)]
        static extern int DragQueryFile(IntPtr hDrop, int iFile, StringBuilder? lpszFile, int cch);

        [DllImport("ole32.dll", PreserveSig = false)]
        static extern void ReleaseStgMedium([In] ref STGMEDIUM unnamedParam1);
    }
}
