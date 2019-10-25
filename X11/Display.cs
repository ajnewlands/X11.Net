using System;
using System.Runtime.InteropServices;

namespace X11
{
    public enum CloseDownMode: int
    {
        DestroyAll = 0,
        RetainPermanent = 1,
        RetainTemporary = 2,
    }



    public enum Planes: ulong
    {
        AllPlanes = 0xffffffffffffffff,
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Screen
    {
        IntPtr ext_data;     /* hook for extension to hang data */
        IntPtr display;/* back pointer to display structure */
        Window root;            /* Root window id. */
        int width, height;      /* width and height of screen */
        int mwidth, mheight;    /* width and height of  in millimeters */
        int ndepths;            /* number of depths possible */
        IntPtr depths;          /* list of allowable depths on the screen */
        int root_depth;         /* bits per pixel */
        IntPtr root_visual;    /* root visual */
        IntPtr default_gc;          /* GC for the root root visual */
        Colormap cmap;          /* default color map */
        ulong white_pixel;
        ulong black_pixel;      /* White and Black pixel values */
        int max_maps, min_maps; /* max and min color maps */
        int backing_store;      /* Never, WhenMapped, Always */
        bool save_unders;
        long root_input_mask;
    }

    public partial class Xlib
    {
        /// <summary>
        /// Initiate a connection to the name X session.
        /// (or respect the DISPLAY environment variable if the display parameter is null).
        /// </summary>
        /// <param name="display">X session connection string in format hostname:number.screen_number</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XOpenDisplay(string display);

        /// <summary>
        /// Free an unmanaged display pointer (created with XOpenDisplay)
        /// </summary>
        /// <param name="display">Display pointer to free</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XCloseDisplay(IntPtr display);

        /// <summary>
        /// The XSetCloseDownMode() defines what will happen to the client's resources at connection close. 
        /// A connection starts in DestroyAll mode.
        /// 
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="close_mode">New close down mode setting</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XSetCloseDownMode(IntPtr display, CloseDownMode close_mode);

        /// <summary>
        /// The XKillClient function forces a close down of the client that created the resource if a valid resource is
        /// specified.If the client has already terminated in either RetainPermanent or RetainTemporary mode, all of the
        /// client's resources are destroyed.  If AllTemporary is specified, the resources of all clients that have termi‐
        /// nated in RetainTemporary are destroyed(see section 2.5).  This permits implementation of window manager
        /// facilities that aid debugging.A client can set its close-down mode to RetainTemporary.  If the client then
        /// crashes, its windows would not be destroyed.  The programmer can then inspect the application's window tree
        /// and use the window manager to destroy the zombie windows.
        /// </summary>
        /// <param name="display">Connect display</param>
        /// <param name="resource">Resource specifying client to kill</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XKillClient(IntPtr display, Window resource);

        /// <summary>
        /// Return the black pixel value for the specified screen
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">target screen</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern ulong XBlackPixel(IntPtr display, int screen_number);

        /// <summary>
        /// Return the white pixel value for the specified screen
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">target screen</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern ulong XWhitePixel(IntPtr display, int screen_number);

        /// <summary>
        /// Returns the connection number (i.e. file descriptor) corresponding to the named display
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>File descriptor or equivalent</returns>
        [DllImport("libX11.so.6")]
        public static extern int XConnectionNumber(IntPtr display);


        /// <summary>
        /// Returns the depth (number of planes) of the default root window on the specified screen.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen</param>
        /// <returns>Depth in planes</returns>
        [DllImport("libX11.so.6")]
        public static extern int XDefaultDepth(IntPtr display, int screen_number);

        /// <summary>
        /// The XListDepths() function returns the array of depths that are available on the specified screen.
        /// If the specified screen_number is valid and sufficient memory for the array can be allocated, XListDepths() 
        /// sets count_return to the number of available depths. Otherwise, it does not set count_return and returns NULL.
        /// To release the memory allocated for the array of depths, use XFree().
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <param name="count_return">Returned array of depths</param>
        /// <returns>null on error otherwise array of depths</returns>
        [DllImport("libX11.so.6")]
        public static extern ref int XListDepths(IntPtr display, int screen_number, ref int count_return);

        /// <summary>
        /// Both return the default graphics context for the root window of the specified screen. This GC is created for
        /// the convenience of simple applications and contains the default GC components with the foreground and 
        /// background pixel values initialized to the black and white pixels for the screen, respectively. You can 
        /// modify its contents freely because it is not used in any Xlib function. This GC should never be freed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <returns>Reference to the default graphics context</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XDefaultGC(IntPtr display, int screen_number);



        /// <summary>
        /// Retrieve the Window ID corresponding to the displays root window.
        /// </summary>
        /// <param name="display">Pointer to an open X display</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Window XDefaultRootWindow(IntPtr display);

        /// <summary>
        /// Returns the root window for the specified display
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Window XRootWindow(IntPtr display, int screen_number);

        /// <summary>
        /// Returns a pointer to the default screen
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Pointer to the default screen</returns>
        [DllImport("libX11.so.6")]
        public static extern ref Screen XDefaultScreenOfDisplay(IntPtr display);

        /// <summary>
        /// Returns a pointer to the indicated screen
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <returns>Pointer to the specified screen</returns>
        [DllImport("libX11.so.6")]
        public static extern ref Screen XScreenOfDisplay(IntPtr display, int screen_number);

        /// <summary>
        /// Returns the default screen number for the specified display
        /// </summary>
        /// <param name="display"></param>
        /// <returns>Default screen number</returns>
        [DllImport("libX11.so.6")]
        public static extern int XDefaultScreen(IntPtr display);

        /// <summary>
        /// Return the number of available screens on the connected display
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Number of available screens</returns>
        [DllImport("libX11.so.6")]
        public static extern int XScreenCount(IntPtr display);


        /// <summary>
        /// Returns a pointer to the default visual for the specified screen
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <returns>Pointer to the default visual</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XDefaultVisual(IntPtr display, int screen_number);

        /// <summary>
        /// Returns the number of entries in the default colour map
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <returns>Number of entries in the colour map</returns>
        [DllImport("libX11.so.6")]
        public static extern int XDisplayCells(IntPtr display, int screen_number);

        /// <summary>
        /// Returns the depth of the root window of the specified screen.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen number</param>
        /// <returns>Depth of the root window</returns>
        [DllImport("libX11.so.6")]
        public static extern int XDisplayPlanes(IntPtr display, int screen_number);

        /// <summary>
        /// Both return a pointer to the string that was passed to XOpenDisplay() when the current display was opened.
        /// On POSIX-conformant systems, if the passed string was NULL, these return the value of the DISPLAY 
        /// environment variable when the current display was opened.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Pointer to the character buffer containing the display string</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XDisplayString(IntPtr display);

        /// <summary>
        /// The XExtendedMaxRequestSize() function returns zero if the specified display does not support an 
        /// extended-length protocol encoding; otherwise, it returns the maximum request size (in 4-byte units) 
        /// supported by the server using the extended-length encoding.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Zero or else maximum extended request size</returns>
        [DllImport("libX11.so.6")]
        public static extern long XExtendedMaxRequestSize(IntPtr display);

        /// <summary>
        /// The XMaxRequestSize() function returns the maximum request size (in 4-byte units) supported by the server 
        /// without using an extended-length protocol encoding. 
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Maximum request length in 4-byte units</returns>
        [DllImport("libX11.so.6")]
        public static extern long XMaxRequestSize(IntPtr display);

        /// <summary>
        ///  Retrieves the full serial number of the last request known by Xlib to have been processed by the X server.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Serial number of the last processed request</returns>
        [DllImport("libX11.so.6")]
        public static extern ulong XLastKnownRequestProcessed(IntPtr display);

        /// <summary>
        /// Retrieve the full serial number that is to be used for the next request
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Serial number of the next request to process</returns>
        [DllImport("libX11.so.6")]
        public static extern ulong XNextRequest(IntPtr display);

        /// <summary>
        /// Returns a pointer to a character buffer holding the vendor identification string for the connected server.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Pointer to a C string containing the vendor ID.</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XServerVendor(IntPtr display);

        /// <summary>
        /// Returns the vendor defined version number for this X server.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>version number</returns>
        [DllImport("libX11.so.6")]
        public static extern int XVendorRelease(IntPtr display);

        /// <summary>
        /// Returns the major version number (11) of the X protocol associated with the connected display
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>X protocol major version number</returns>
        [DllImport("libX11.so.6")]
        public static extern int XProtocolVersion(IntPtr display);

        /// <summary>
        /// Returns the minor revision number (e.g. 6) of the X protocol associated with the connected display
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>X protocol minor revision number</returns>
        [DllImport("libX11.so.6")]
        public static extern int XProtocolRevision(IntPtr display);

        /// <summary>
        /// Returns the length of the event queue for the connected display
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Current queue length</returns>
        [DllImport("libX11.so.6")]
        public static extern int XQLength(IntPtr display);

        /// <summary>
        /// The XSynchronize function returns the previous after function.  If onoff is True, XSynchronize turns on
        /// synchronous behavior.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="onoff">Enable or disable synchronous behaviour</param>
        /// <returns>The 'after' function</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XSynchronize(IntPtr display, bool onoff);

        /// <summary>
        /// After function signature
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int XAfterFunctionDelegate();

        /// <summary>
        /// Set the after function
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="after_function">New after function delegate</param>
        /// <returns>The previous after function</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XSetAfterFunction(IntPtr display, XAfterFunctionDelegate after_function);

        [DllImport("libX11.so.6")]
        public static extern int XGrabServer(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern int XUngrabServer(IntPtr display);

        /// <summary>
        ///  The XDisplayName function returns the name of the display that XOpenDisplay would attempt to use.  If a NULL
        /// string is specified, XDisplayName looks in the environment for the display and returns the display name that
        /// XOpenDisplay would attempt to use.This makes it easier to report to the user precisely which display the
        /// program attempted to open when the initial connection attempt failed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Pointer to a character buffer containing the display name</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XDisplayName(string display);

        [DllImport("libX11.so.6")]
        public static extern int XChangeProperty(IntPtr display, Window window, Atom property, Atom type, int format, int mode, IntPtr data, int nelements);

    }
}
