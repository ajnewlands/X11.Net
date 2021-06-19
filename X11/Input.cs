using System;
using System.Runtime.InteropServices;

namespace X11
{
    public partial class Xlib
    {
        //Locale

        [DllImport("libX11.so.6", CharSet = CharSet.Ansi)]
        public static extern string XSetLocaleModifiers(string modifierList); 

        [DllImport("libX11.so.6")]
        public static extern bool XSupportsLocale();

        //Input Method
        [DllImport("libX11.so.6", CharSet = CharSet.Ansi)]
        public static extern IntPtr XOpenIM(IntPtr display,
                                            IntPtr resourceDatabase,
                                            string res_name,
                                            string res_class);

        [DllImport("libX11.so.6")]
        public static extern X11.Status XCloseIM(IntPtr inputMethod);

        //Input Context
        [DllImport("libX11.so.6")]
        public static extern IntPtr XCreateIC(IntPtr inputMethod,
                                              string inputStylePropName,
                                              int inputStyle,
                                              string windowPropName,
                                              X11.Window xWindow,
                                              IntPtr terminator);

        [DllImport("libX11.so.6")]
        public static extern void XSetICFocus(IntPtr inputContext);

        [DllImport("libX11.so.6")]
        public static extern void XDestroyIC(IntPtr inputContext);

        //String lookup
        [DllImport("libX11.so.6")]
        public static extern int XmbLookupString(IntPtr inputContext,
                                                 ref XKeyEvent keyPressedEvent,
                                                 IntPtr stringBuffer,
                                                 int bufferLen,
                                                 out X11.KeySym keySyms,
                                                 out X11.Status status);

        [DllImport("libX11.so.6")]
        public static extern int Xutf8LookupString(IntPtr inputContext,
                                                   ref XKeyEvent keyPressedEvent,
                                                   IntPtr stringBuffer,
                                                   int bufferLen,
                                                   out X11.KeySym keySym,
                                                   out X11.Status status);

        
        [DllImport("libX11.so.6")]
        public static extern int XwcLookupString(IntPtr inputContext,
                                                 ref XKeyEvent keyPressedEvent,
                                                 IntPtr stringBuffer,
                                                 int bufferLen,
                                                 out X11.KeySym keySyms,
                                                 out X11.Status status);
    }
}