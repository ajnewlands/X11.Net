using System;
using System.Collections.Generic;
using System.Text;

namespace X11
{
    public class X
    {
        public const ulong None = 0;

        public class Cursor
        {
            public const ulong XC_left_ptr = 68;
            public const ulong XC_sizing = 120;
        }

        public class EventMask
        {
            /// <summary>
            /// These are the input event masks from X.h
            /// </summary>
            public const long NoEventMask = 0L;
            public const long KeyPressMask = (1L << 0);
            public const long KeyReleaseMask = (1L << 1);
            public const long ButtonPressMask = (1L << 2);
            public const long ButtonReleaseMask = (1L << 3);
            public const long EnterWindowMask = (1L << 4);
            public const long LeaveWindowMask = (1L << 5);
            public const long PointerMotionMask = (1L << 6);
            public const long PointerMotionHintMask = (1L << 7);
            public const long Button1MotionMask = (1L << 8);
            public const long Button2MotionMask = (1L << 9);
            public const long Button3MotionMask = (1L << 10);
            public const long Button4MotionMask = (1L << 11);
            public const long Button5MotionMask = (1L << 12);
            public const long ButtonMotionMask = (1L << 13);
            public const long KeymapStateMask = (1L << 14);
            public const long ExposureMask = (1L << 15);
            public const long VisibilityChangeMask = (1L << 16);
            public const long StructureNotifyMask = (1L << 17);
            public const long ResizeRedirectMask = (1L << 18);
            public const long SubstructureNotifyMask = (1L << 19);
            public const long SubstructureRedirectMask = (1L << 20);
            public const long FocusChangeMask = (1L << 21);
            public const long PropertyChangeMask = (1L << 22);
            public const long ColormapChangeMask = (1L << 23);
            public const long OwnerGrabButtonMask = (1L << 24);
        }

        public enum Event: int
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

        public class Button
        {
            public const uint LEFT = 1;
            public const uint MIDDLE = 2;
            public const uint RIGHT = 3;
            public const uint UP = 4;
            public const uint DOWN = 5;
        }

        public class GrabMode
        {
            public const int Sync = 0;
            public const int Async = 1;
        }
    }
}
