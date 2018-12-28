using System;
using System.Runtime.InteropServices;
namespace X11
{

    [StructLayout(LayoutKind.Sequential)]
    public struct XErrorEvent
    {
        public int type;
        public IntPtr display;
        public XID resourceid;
        public ulong serial;
        public char error_code;
        public char request_code;
        public char minor_code;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int XErrorHandlerDelegate(IntPtr display, ref XErrorEvent ev);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int XIOErrorHandlerDelegate(IntPtr display);

    public partial class Xlib
    {
        /// <summary>
        /// Define the function to be called when the X server reports an error
        /// </summary>
        /// <param name="del">Function delegate to call on error</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XSetErrorHandler(XErrorHandlerDelegate del);

        /// <summary>
        /// The XSetIOErrorHandler sets the fatal I/O error handler.  Xlib calls the program's supplied error handler if
        /// any sort of system call error occurs(for example, the connection to the server was lost).  This is assumed to
        /// be a fatal condition, and the called routine should not return.  If the I/O error handler does return, the
        /// client process exits.
        /// </summary>
        /// <param name="del">Error handler function delegate</param>
        /// <returns>Previous error handler</returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XSetIOErrorHandler(XIOErrorHandlerDelegate del);

        /// <summary>
        /// Return the error description corresponding to the error code
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="code">Error code to lookup</param>
        /// <param name="description">Pre-allocated buffer to hold the output description</param>
        /// <param name="length">Length of the descrition buffer</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XGetErrorText(IntPtr display, int code, IntPtr description, int length);

        /// <summary>
        /// The XGetErrorDatabaseText function returns a null-terminated message (or the default message) from the error
        /// message database.Xlib uses this function internally to look up its error messages.  The text in the
        /// default_string argument is assumed to be in the encoding of the current locale, and the text stored in the
        /// buffer_return argument is in the encoding of the current locale.
        /// The name argument should generally be the name of your application.The message argument should indicate
        /// which type of error message you want.  If the name and message are not in the Host Portable Character Encod‐
        /// ing, the result is implementation-dependent.Xlib uses three predefined ``application names'' to report
        /// errors.In these names, uppercase and lowercase matter.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="name">Application name</param>
        /// <param name="message">Error type</param>
        /// <param name="default_string">Default error message if none exists in the database</param>
        /// <param name="buffer_return">Pre-allocated buffer to hold returned message</param>
        /// <param name="length">Length of the buffer</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XGetErrorDatabaseText(IntPtr display, string name, string message, string default_string,
            IntPtr buffer_return, int length);
    }
}
