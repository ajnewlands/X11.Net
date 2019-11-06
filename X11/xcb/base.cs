using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace X11
{
    public partial class xcb
    {
        public enum Window : UInt32 { }

        /// <summary>
        /// Establish an XCB connection to X11
        /// </summary>
        /// <param name="DisplayName">Name of the display to connect to. If NULL, connect to the default display</param>
        /// <param name="ScreenNumber">Pointer to the screen number to connect to. Defaults to zero where this is NULL</param>
        /// <returns>A pointer to the connection object. Always returns non-null; check the result with xcb_connection_has_error()</returns>
        [DllImport("libxcb.so")]
        public static extern IntPtr xcb_connect(string DisplayName, IntPtr ScreenNumber);

        [DllImport("libxcb.so")]
        public static extern void xcb_disconnect(IntPtr Connection);

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

        [DllImport("libxcb.so")]
        public static extern IntPtr xcb_get_setup(IntPtr Connection);

        [DllImport("libxcb.so")]
        public static extern xcb_screen_iterator_t xcb_setup_roots_iterator(IntPtr Setup);

        /// <summary>
        /// Block until the next event is received (or the connection to X11 is lost)
        /// </summary>
        /// <param name="Connection">A pointer to an opaque connection structure</param>
        /// <returns>The next event received (or null on disconnection)</returns>
        [DllImport("libxcb.so")]
        private static extern IntPtr xcb_wait_for_event(IntPtr Connection);
        public static generic_event? wait_for_event(IntPtr Connection)
        {
            var e = xcb_wait_for_event(Connection);
            return (e == IntPtr.Zero) ? new generic_event?() : Marshal.PtrToStructure<generic_event>(e);
        }

    }
}
