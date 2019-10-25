using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace X11
{
    public partial class xcb
    {
        /// <summary>
        /// Establish an XCB connection to X11
        /// </summary>
        /// <param name="DisplayName">Name of the display to connect to. If NULL, connect to the default display</param>
        /// <param name="ScreenNumber">Pointer to the screen number to connect to. Defaults to zero where this is NULL</param>
        /// <returns>A pointer to the connection object. Always returns non-null; check the result with xcb_connection_has_error()</returns>
        [DllImport("libxcb.so")]
        public static extern IntPtr xcb_connect(string DisplayName, IntPtr ScreenNumber);

        /// <summary>
        /// Establish an XCB connection to X11
        /// </summary>
        /// <param name="DisplayName">Name of the display to connect to. If NULL, connect to the default display</param>
        /// <param name="ScreenNumber">Pointer to the screen number to connect to. Defaults to zero where this is NULL</param>
        /// <returns>A pointer to the connection object. Always returns non-null; check the result with xcb_connection_has_error()</returns>
        [DllImport("libxcb.so")]
        public static extern IntPtr xcb_connect(IntPtr DisplayName, IntPtr ScreenNumber);

        /// <summary>
        /// Return codes for xcb_connection_has_error()
        /// </summary>
        public enum XCBConnectionError : int
        {
            SUCCESS = 0,
            CONN_ERROR = 1,
            CON_CLOSED_EXTENSION_NOT_SUPPORTED = 2,
            CON_CLOSED_MEM_INSUFFICIENT = 3,
            CON_CLOSED_REQ_LEN_EXCEEDED = 4,
            CON_CLOSED_PARSE_ERROR = 5,
            CON_CLOSED_INVALID_SCREEN = 6,
        }

        /// <summary>
        /// Check whether an XCB connection was successfully established, or not.
        /// </summary>
        /// <param name="Connection">Pointer to a connection object returned by xcb_connect()</param>
        /// <returns>0 on success, >0 on failure.</returns>
        [DllImport("libxcb.so")]
        public static extern XCBConnectionError xcb_connection_has_error(IntPtr Connection);
    }
}
