using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace X11
{
    public partial class xcb
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct xcb_void_cookie_t
        {
            uint sequence;
        }

        [DllImport("libxcb.so")]
        public static extern xcb_void_cookie_t xcb_change_window_attributes(IntPtr Connection, UInt32 Window, UInt32 ValueMask, IntPtr ValueList);

        [StructLayout(LayoutKind.Sequential, Size = (9 * sizeof(UInt32)))]
        public struct xcb_generic_error_t
        {
            byte response_type;
            byte error_code;
            UInt16 sequence;
            UInt32 resource_id;
            UInt16 minor_code;
            byte major_code;
            // followed by several bytes worth of padding
        }

        [DllImport("libxcb.so")]
        public static extern ref xcb_generic_error_t xcb_request_check(IntPtr Connection, xcb_void_cookie_t Cookie);
    }
}
