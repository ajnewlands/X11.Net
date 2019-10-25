using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace X11
{
    public partial class Xmu
    {
        [DllImport("libXmu.so.6")]
        public static extern Atom XmuInternAtom(IntPtr display, IntPtr atomPtr);

        [DllImport("libXmu.so.6")]
        public static extern IntPtr XmuMakeAtom(String name);

    }
}
