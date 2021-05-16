using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Windows
{
    internal static class User32
    {
        /// <summary>
        /// Get the scroll information of a scroll bar or window with scroll bar
        /// @see https://msdn.microsoft.com/en-us/library/windows/desktop/bb787537(v=vs.85).aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ScrollInfo
        {
            /// <summary>
            /// Specifies the size, in bytes, of this structure. The caller must set this to sizeof(SCROLLINFO).
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// Specifies the scroll bar parameters to set or retrieve.
            /// @see ScrollInfoMask
            /// </summary>
            public uint fMask;
            /// <summary>
            /// Specifies the minimum scrolling position.
            /// </summary>
            public int nMin;
            /// <summary>
            /// Specifies the maximum scrolling position.
            /// </summary>
            public int nMax;
            /// <summary>
            /// Specifies the page size, in device units. A scroll bar uses this value to determine the appropriate size of the proportional scroll box.
            /// </summary>
            public uint nPage;
            /// <summary>
            /// Specifies the position of the scroll box.
            /// </summary>
            public int nPos;
            /// <summary>
            /// Specifies the immediate position of a scroll box that the user is dragging. 
            /// An application can retrieve this value while processing the SB_THUMBTRACK request code. 
            /// An application cannot set the immediate scroll position; the SetScrollInfo function ignores this member.
            /// </summary>
            public int nTrackPos;
        }

        /// <summary>
        /// Used for the ScrollInfo fMask
        /// SIF_ALL             => Combination of SIF_PAGE, SIF_POS, SIF_RANGE, and SIF_TRACKPOS.
        /// SIF_DISABLENOSCROLL => This value is used only when setting a scroll bar's parameters. If the scroll bar's new parameters make the scroll bar unnecessary, disable the scroll bar instead of removing it.
        /// SIF_PAGE            => The nPage member contains the page size for a proportional scroll bar.
        /// SIF_POS             => The nPos member contains the scroll box position, which is not updated while the user drags the scroll box.
        /// SIF_RANGE           => The nMin and nMax members contain the minimum and maximum values for the scrolling range.
        /// SIF_TRACKPOS        => The nTrackPos member contains the current position of the scroll box while the user is dragging it.
        /// </summary>
        public enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }

        public enum ScrollInfoBar
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, out IntPtr lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.LPStr)] out string[] wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, out IntPtr wParam, IntPtr lParam);

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), new IntPtr(lParam));
        }

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, out int lParam)
        {
            IntPtr outVal;
            IntPtr retval = SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), out outVal);
            lParam = outVal.ToInt32();
            return retval;
        }

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, int lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, wParam, new IntPtr(lParam));
        }

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam)
        {
            return SendMessage(hWnd, (UInt32)Msg, new IntPtr(wParam), lParam);
        }

        public const int MF_BYCOMMAND = 0;
        public const int MF_CHECKED = 8;
        public const int MF_UNCHECKED = 0;

        [DllImport("user32")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32")]
        public static extern int CheckMenuItem(IntPtr hmenu, int uIDCheckItem, int uCheck);

        /// <summary>
        /// @see https://msdn.microsoft.com/en-us/library/windows/desktop/bb787583(v=vs.85).aspx
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nBar"></param>
        /// <param name="scrollInfo"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int GetScrollInfo(IntPtr hwnd, int nBar, ref ScrollInfo scrollInfo);

        public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassNameW(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    }
}
