using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace X11
{
    public partial class xcb
    {
        public enum xcb_event_mask : int
        {
            XCB_EVENT_MASK_NO_EVENT = 0,
            XCB_EVENT_MASK_KEY_PRESS = 1,
            XCB_EVENT_MASK_KEY_RELEASE = 2,
            XCB_EVENT_MASK_BUTTON_PRESS = 4,
            XCB_EVENT_MASK_BUTTON_RELEASE = 8,
            XCB_EVENT_MASK_ENTER_WINDOW = 16,
            XCB_EVENT_MASK_LEAVE_WINDOW = 32,
            XCB_EVENT_MASK_POINTER_MOTION = 64,
            XCB_EVENT_MASK_POINTER_MOTION_HINT = 128,
            XCB_EVENT_MASK_BUTTON_1_MOTION = 256,
            XCB_EVENT_MASK_BUTTON_2_MOTION = 512,
            XCB_EVENT_MASK_BUTTON_3_MOTION = 1024,
            XCB_EVENT_MASK_BUTTON_4_MOTION = 2048,
            XCB_EVENT_MASK_BUTTON_5_MOTION = 4096,
            XCB_EVENT_MASK_BUTTON_MOTION = 8192,
            XCB_EVENT_MASK_KEYMAP_STATE = 16384,
            XCB_EVENT_MASK_EXPOSURE = 32768,
            XCB_EVENT_MASK_VISIBILITY_CHANGE = 65536,
            XCB_EVENT_MASK_STRUCTURE_NOTIFY = 131072,
            XCB_EVENT_MASK_RESIZE_REDIRECT = 262144,
            XCB_EVENT_MASK_SUBSTRUCTURE_NOTIFY = 524288,
            XCB_EVENT_MASK_SUBSTRUCTURE_REDIRECT = 1048576,
            XCB_EVENT_MASK_FOCUS_CHANGE = 2097152,
            XCB_EVENT_MASK_PROPERTY_CHANGE = 4194304,
            XCB_EVENT_MASK_COLOR_MAP_CHANGE = 8388608,
            XCB_EVENT_MASK_OWNER_GRAB_BUTTON = 16777216
        }

        public enum xcb_cw_t: int
        {
            XCB_CW_BACK_PIXMAP = 1,
            XCB_CW_BACK_PIXEL = 2,
            XCB_CW_BORDER_PIXMAP = 4,
            XCB_CW_BORDER_PIXEL = 8,
            XCB_CW_BIT_GRAVITY = 16,
            XCB_CW_WIN_GRAVITY = 32,
            XCB_CW_BACKING_STORE = 64,
            XCB_CW_BACKING_PLANES = 128,
            XCB_CW_BACKING_PIXEL = 256,
            XCB_CW_OVERRIDE_REDIRECT = 512,
            XCB_CW_SAVE_UNDER = 1024,
            XCB_CW_EVENT_MASK = 2048,
            XCB_CW_DONT_PROPAGATE = 4096,
            XCB_CW_COLORMAP = 8192,
            XCB_CW_CURSOR = 16384
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct xcb_query_tree_reply_t
        {
            public byte response_type;
            private byte pad0;
            public UInt16 sequence;
            public UInt32 length;
            public Window root;
            public Window parent;
            public UInt16 children_len;
            private fixed byte pad1[14];
        }

        [DllImport("libxcb.so")]
        public static extern xcb_query_tree_cookie_t xcb_query_tree(IntPtr Connection, Window Window);

        [DllImport("libxcb.so")]
        private static extern IntPtr xcb_query_tree_reply
            (IntPtr Connection, xcb_query_tree_cookie_t cookie, ref IntPtr error);

        public unsafe static xcb_query_tree_reply_t? query_tree_reply 
            (IntPtr Connection, xcb_query_tree_cookie_t Cookie, out xcb_generic_error_t? Error)
        {
            IntPtr err = IntPtr.Zero;
            var reply = xcb_query_tree_reply(Connection, Cookie, ref err);
            if (reply == IntPtr.Zero)
            {
                Error = Marshal.PtrToStructure<xcb_generic_error_t>(err);
                return new xcb_query_tree_reply_t();
            }
            else
            {
                Error = new xcb_generic_error_t();
                return Marshal.PtrToStructure<xcb_query_tree_reply_t>(reply);
            }
        }

        [DllImport("libxcb.so")]
        public static extern int xcb_query_tree_children_length(in xcb_query_tree_reply_t Reply);


        [DllImport("libxcb.so")]
        private static extern IntPtr xcb_query_tree_children(in xcb_query_tree_reply_t Reply);

        [StructLayout(LayoutKind.Sequential)]
        public struct xcb_void_cookie_t
        {
            uint sequence;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct xcb_query_tree_cookie_t
        {
            uint sequence;
        }

        [DllImport("libxcb.so")]
        public static extern xcb_void_cookie_t xcb_change_window_attributes(IntPtr Connection, Window Window, UInt32 ValueMask, IntPtr ValueList);

        [StructLayout(LayoutKind.Sequential, Size = (9 * sizeof(UInt32)))]
        public struct xcb_generic_error_t
        {
            public byte response_type;
            public byte error_code;
            public UInt16 sequence;
            public UInt32 resource_id;
            public UInt16 minor_code;
            public byte major_code;
            // followed by several bytes worth of padding
        }

        [DllImport("libxcb.so")]
        private static extern IntPtr xcb_request_check(IntPtr Connection, xcb_void_cookie_t Cookie);
        public static xcb_generic_error_t? request_check(IntPtr Connection, xcb_void_cookie_t Cookie)
        {
            var err = xcb_request_check(Connection, Cookie);
            return (err == IntPtr.Zero) ? new xcb_generic_error_t?() : Marshal.PtrToStructure<xcb_generic_error_t>(err);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct xcb_screen_t
        {
            public Window root;
            public UInt32 default_colormap;
            public UInt32 white_pixel;
            public UInt32 black_pixel;
            public UInt32 current_input_masks;
            public UInt16 width_in_pixels;
            public UInt16 height_in_pixels;
            public UInt16 width_in_millimeters;
            public UInt16 height_in_millimeters;
            public UInt16 min_installed_maps;
            public UInt16 max_installed_maps;
            public UInt32 root_visual;
            public char backing_stores;
            public char save_unders;
            public char root_depth;
            public char allowed_depths_len;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct xcb_screen_iterator_t
        {
            public IntPtr data;
            public int rem;
            public int index;
        }

        [DllImport("libxcb.so")]
        public static extern xcb_void_cookie_t
            xcb_change_window_attributes_checked(IntPtr Connection, Window Window, xcb_cw_t value_mask, ref xcb_event_mask values);

        [DllImport("libxcb.so")]
        private static extern xcb_void_cookie_t xcb_grab_server_checked(IntPtr Connection);
        public static void grab_server(IntPtr Connection)
        {
            var cookie = xcb_grab_server_checked(Connection);
            xcb_generic_error_t? Error;

            if ((Error = xcb.request_check(Connection, cookie)).HasValue)
            {
                throw new Exception($"Failed to grab the X server: code {Error.Value.error_code}");
            }
        }

        [DllImport("libxcb.so")]
        private static extern xcb_void_cookie_t xcb_ungrab_server_checked(IntPtr Connection);
        public static void ungrab_server(IntPtr Connection)
        {
            var cookie = xcb_ungrab_server_checked(Connection);
            xcb_generic_error_t? Error;

            if ((Error = xcb.request_check(Connection, cookie)).HasValue)
            {
                throw new Exception($"Failed to release the X server: code {Error.Value.error_code}");
            }
        }


        public unsafe static int[] Children(IntPtr Connection, Window window)
        {
            xcb_generic_error_t? Error;
            var cookie = xcb_query_tree(Connection, window);
            var qt = query_tree_reply(Connection, cookie, out Error);

            if (qt.HasValue)
            {
                var v = qt.Value;
                var pChildren = xcb_query_tree_children(in v);
                var n = xcb_query_tree_children_length(in v);

                Console.WriteLine($"Window {window} has {n} children");

                var Children = new int[n];
                Marshal.Copy(pChildren, Children, 0, n);
                return Children;
            }
            else
            {
                throw new Exception($"Unable to query tree: error code {Error.Value.error_code}");
            }
        }
    }
}
