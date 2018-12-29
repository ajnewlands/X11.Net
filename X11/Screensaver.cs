using System;
using System.Runtime.InteropServices;

namespace X11
{
    public enum ScreenSaverExposures: int
    {
        DontAllowExposures = 0,
        AllowExposures = 1,
        DefaultExposures = 2,
    }

    public enum ScreenSaverMode: int
    {
        ScreenSaverReset = 0,
        ScreenSaverActive = 1,
    }

    public enum ScreenSaverBlanking: int
    {
        DontPreferBlanking = 0,
        PreferBlanking = 1,
        DefaultBlanking = 2,
    }

    public partial class Xlib
    {
        [DllImport("libX11.so.6")]
        public static extern Status XSetScreenSaver(IntPtr display, int timeout, int interval, ScreenSaverBlanking prefer_blanking, 
            ScreenSaverExposures allow_exposures);

        [DllImport("libX11.so.6")]
        public static extern Status XForceScreenSaver(IntPtr display, ScreenSaverMode mode);

        [DllImport("libX11.so.6")]
        public static extern Status XActivateScreenSaver(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern Status XResetScreenSaver(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern Status XGetScreenSaver(IntPtr display, ref int timeout_return, ref int interval_return, 
            ref ScreenSaverBlanking prefer_blanking_return, ref ScreenSaverExposures allow_exposures_return);
    }
}
