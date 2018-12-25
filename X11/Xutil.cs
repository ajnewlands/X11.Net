using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace X11
{
    public class Xutil
    {
        /// <summary>
        /// Free a previously allocated XImage.
        /// </summary>
        /// <param name="XImage">The XImage structure to free.</param>
        /// <returns>non-zero on success, zero on failure.</returns>
        //[DllImport("libX11.so.6")]
        //public static extern int XDestroyImage(ref XImage XImage);
        public static int XDestroyImage(ref XImage xImage)
        {
            Marshal.FreeHGlobal(xImage.data);
            Marshal.FreeHGlobal(xImage.obdata);
            return 0;
        }
    }
}
