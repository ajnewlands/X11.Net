using System;
using System.Runtime.InteropServices;

namespace X11
{
    public partial class Xlib
    {
        /// <summary>
        /// The XGetWindowAttributes function returns the current attributes for the specified window to an XWindowAt‐
        /// tributes structure.It returns a nonzero status on success; otherwise, it returns a zero status.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="window"></param>
        /// <param name="attributes"></param>
        [DllImport("libX11.so.6")]
        public static extern Status XGetWindowAttributes(IntPtr display, Window window, out XWindowAttributes attributes);
    }
}
