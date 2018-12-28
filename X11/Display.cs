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


    }
}
