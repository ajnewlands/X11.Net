using System;
using System.Runtime.InteropServices;

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
