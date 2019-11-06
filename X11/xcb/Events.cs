using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace X11
{
    public partial class xcb
    {
        public enum Event : byte
        {
            KeyPress = 2,
            KeyRelease = 3,
            ButtonPress = 4,
            ButtonRelease = 5,
            MotionNotify = 6,
            EnterNotify = 7,
            LeaveNotify = 8,
            FocusIn = 9,
            FocusOut = 10,
            KeymapNotify = 11,
            Expose = 12,
            GraphicsExpose = 13,
            NoExpose = 14,
            VisibilityNotify = 15,
            CreateNotify = 16,
            DestroyNotify = 17,
            UnmapNotify = 18,
            MapNotify = 19,
            MapRequest = 20,
            ReparentNotify = 21,
            ConfigureNotify = 22,
            ConfigureRequest = 23,
            GravityNotify = 24,
            ResizeRequest = 25,
            CirculateNotify = 26,
            CirculateRequest = 27,
            PropertyNotify = 28,
            SelectionClear = 29,
            SelectionRequest = 30,
            SelectionNotify = 31,
            ColormapNotify = 32,
            ClientMessage = 33,
            MappingNotify = 34,
            GenericEvent = 35,
            LASTEvent = 36
        }

        [StructLayout(LayoutKind.Sequential, Size=32)]
        public unsafe struct generic_event
        {
            public Event response_type;
            private byte pad0;
            public UInt16 sequence;
            private fixed UInt32 pad[7];
            public UInt32 full_sequence;
        }

        [StructLayout(LayoutKind.Sequential, Size = 32)]
        public unsafe struct create_notify_event
        {
            public Event response_type;
            public byte pad0;
            public UInt16 sequence;
            public Window parent;
            public Window window;
            public Int16 x;
            public Int16 y;
            public UInt16 width;
            public UInt16 height;
            public UInt16 border_width;
            public byte override_redirect;
            public byte pad1;

            public static explicit operator create_notify_event(generic_event v)
            {
                return *(create_notify_event*)&v;
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 32)]
        public unsafe struct destroy_notify_event
        {
            public Event response_type;
            public byte pad0;
            public UInt16 sequence;
            public Window @event;
            public Window window;

            public static explicit operator destroy_notify_event(generic_event v)
            {
                return *(destroy_notify_event*)&v;
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 32)]
        public unsafe struct map_notify_event
        {
            public Event response_type;
            public byte pad0;
            public UInt16 sequence;
            public Window @event;
            public Window window;
            public byte override_redirect;
            private fixed byte pad1[3];

            public static explicit operator map_notify_event(generic_event v)
            {
                return *(map_notify_event*)&v;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct map_request_event
        {
            public Event response_type;
            public byte pad0;
            public UInt16 sequence;
            public Window @event;
            public Window window;


            public static explicit operator map_request_event(generic_event v)
            {
                return *(map_request_event*)&v;
            }
        }
    }
}
