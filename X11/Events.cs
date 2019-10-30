using System;
using System.Runtime.InteropServices;

namespace X11
{
    // See XAtom.h and Xmu/Atoms.h
    public enum Atom : ulong
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Arc = 3,
        Atom = 4,
        Bitmap = 5,
        Cardinal = 6,
		Colormap = 7,
        Cursor = 8,
        CutBuffer0 = 9,
        CutBuffer1 = 10,
        CutBuffer2 = 11,
        CutBuffer3 = 12,
        CutBuffer4 = 13,
        CutBuffer5 = 14,
        CutBuffer6 = 15,
        CutBuffer7 = 16,
        Drawable = 17,
        Font = 18,
        Integer = 19,
        Pixmap = 20,
        Point = 21,
        Rectangle = 22,
        ResourceManager = 23,
        RgbColorMap = 24,
        RgbBestMap = 25,
        RgbBlueMap = 26,
        RgbDefaultMap = 27,
        RgbGrayMap = 28,
        RgbGreenMap = 29,
        RgbRedMap = 30,
        String = 31,
        Visualid = 32,
        Window = 33,
        WmCommand = 34,
        WmHints = 35,
        WmClientMachine = 36,
        WmIconName = 37,
        WmIconSize = 38,
        WmName = 39,
        WmNormalHints = 40,
        WmSizeHints = 41,
        WmZoomHints = 42,
        MinSpace = 43,
        NormSpace = 44,
        MaxSpace = 45,
        EndSpace = 46,
        SuperscriptX = 47,
        SuperscriptY = 48,
        SubscriptX = 49,
        SubscriptY = 50,
        UnderlinePosition = 51,
        UnderlineThickness = 52,
        StrikeoutAscent = 53,
        StrikeoutDescent = 54,
        ItalicAngle = 55,
        XHeight = 56,
        QuadWidth = 57,
        Weight = 58,
        PointSize = 59,
        Resolution = 60,
        Copyright = 61,
        Notice = 62,
        FontName = 63,
        FamilyName = 64,
        FullName = 65,
        CapHeight = 66,
        WmClass = 67,
        WmTransientFor = 68,
		LastPredefined = 68
    }

    public enum MappingType: int
    {
        MappingModifier = 0,
        MappingKeyboard = 1,
        MappingPointer = 2,
    }

    public enum Event : int
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

    public enum EventMask : long
    {
        /// <summary>
        /// These are the input event masks from X.h
        /// </summary>
        NoEventMask = 0L,
        KeyPressMask = (1L << 0),
        KeyReleaseMask = (1L << 1),
        ButtonPressMask = (1L << 2),
        ButtonReleaseMask = (1L << 3),
        EnterWindowMask = (1L << 4),
        LeaveWindowMask = (1L << 5),
        PointerMotionMask = (1L << 6),
        PointerMotionHintMask = (1L << 7),
        Button1MotionMask = (1L << 8),
        Button2MotionMask = (1L << 9),
        Button3MotionMask = (1L << 10),
        Button4MotionMask = (1L << 11),
        Button5MotionMask = (1L << 12),
        ButtonMotionMask = (1L << 13),
        KeymapStateMask = (1L << 14),
        ExposureMask = (1L << 15),
        VisibilityChangeMask = (1L << 16),
        StructureNotifyMask = (1L << 17),
        ResizeRedirectMask = (1L << 18),
        SubstructureNotifyMask = (1L << 19),
        SubstructureRedirectMask = (1L << 20),
        FocusChangeMask = (1L << 21),
        PropertyChangeMask = (1L << 22),
        ColormapChangeMask = (1L << 23),
        OwnerGrabButtonMask = (1L << 24),
    }

    public enum QueueMode: int
    {
        QueuedAlready = 0,
        QueuedAfterReading = 1,
        QueuedAfterFlush = 2,
    }

    public enum NotifyMode: int
    {
        NotifyNormal = 0,
        NotifyGrab = 1,
        NotifyUngrab = 2,
        NotifyWhileGrabbed = 3,
    }

    public enum NotifyDetail: int
    {
        NotifyAncestor = 0,
        NotifyVirtual = 1,
        NotifyInferior = 2,
        NotifyNonlinear = 3,
        NotifyNonlinearVirtual = 4,
        NotifyPointer = 5,
        NotifyPointerRoot = 6,
        NotifyDetailNone = 7,
    }

    public enum PropertyNotification : int
    {
        NewValue = 0,
        Delete = 1
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XAnyEvent
    {
        public int type;
        public ulong serial;
        public bool send_event;
        public IntPtr display;
        public Window window;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XKeyEvent
    {
        public int type;               /* of event */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;          /* "event" window it is reported relative to */
        public Window root;            /* root window that the event occurred on */
        public Window subwindow;       /* child window */
        public ulong time;              /* milliseconds */
        public int x, y;               /* pointer x, y coordinates in event window */
        public int x_root, y_root;     /* coordinates relative to root */
        public uint state;     /* key or button mask */
        public uint keycode;   /* detail */
        public bool same_screen;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XButtonEvent
    {
        public int type;       /* of event */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window window;          /* "event" window it is reported relative to */
        public Window root;            /* root window that the event occurred on */
        public Window subwindow;   /* child window */
        public ulong time;      /* milliseconds */
        public int x, y;       /* pointer x, y coordinates in event window */
        public int x_root, y_root; /* coordinates relative to root */
        public uint state; /* key or button mask */
        public uint button;    /* detail */
        public bool same_screen;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct XMotionEvent
    {
        public int type;               /* of event */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;          /* "event" window reported relative to */
        public Window root;            /* root window that the event occurred on */
        public Window subwindow;       /* child window */
        public ulong time;              /* milliseconds */
        public int x, y;               /* pointer x, y coordinates in event window */
        public int x_root, y_root;     /* coordinates relative to root */
        public uint state;     /* key or button mask */
        public byte is_hint;           /* detail */
        public bool same_screen;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XCrossingEvent
    {
        public int type;               /* of event */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;          /* "event" window reported relative to */
        public Window root;            /* root window that the event occurred on */
        public Window subwindow;       /* child window */
        public ulong time;              /* milliseconds */
        public int x, y;               /* pointer x, y coordinates in event window */
        public int x_root, y_root;     /* coordinates relative to root */
        public NotifyMode mode;               /* NotifyNormal, NotifyGrab, NotifyUngrab */
        public NotifyDetail detail;
        public bool same_screen;       /* same screen flag */
        public bool focus;             /* boolean focus */
        public uint state;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XFocusChangeEvent
    {
        public int type;               /* FocusIn or FocusOut */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;          /* window of event */
        NotifyMode mode; 
        NotifyDetail detail;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XKeymapEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public byte[] key_vector; // 32 byte vector
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XExposeEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public int x, y;
        public int width, height;
        public int count;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XGraphicsExposeEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window drawable;
        public int x, y;
        public int width, height;
        public int count;              /* if non-zero, at least this many more */
        public RequestCodes major_code;         /* core is CopyArea or CopyPlane */
        public int minor_code; // No specific values defined.
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XNoExposeEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window drawable;
        public RequestCodes major_code;         /* core is CopyArea or CopyPlane */
        public int minor_code;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XVisibilityEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public int state;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XCreateWindowEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window parent;          /* parent of the window */
        public Window window;          /* window id of window created */
        public int x, y;               /* window location */
        public int width, height;      /* size of window */
        public int border_width;       /* border width */
        public bool override_redirect;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XMapRequestEvent
    {
        public int type;
        public ulong serial;
        public bool send_event;
        public IntPtr display;
        public Window parent;
        public Window window;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XMapEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window @event;
        public Window window;
        public bool override_redirect; /* boolean, is override set... */
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XUnmapEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window @event;
        public Window window;
        public bool from_configure;
    }

    /// <summary>
    /// Event raised when a window is destroyed, e.g. by an X client
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XDestroyWindowEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window @event;
        public Window window;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XGravityEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window @event;
        public Window window;
        public int x, y;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XResizeRequestEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public int width, height;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XCirculateEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window @event;
        public Window window;
        public int place;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XCirculateRequestEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window parent;
        public Window window;
        public int place;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XPropertyEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public Atom atom;
        public long time;
        public int state;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XSelectionClearEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public Atom selection;
        public long time;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XSelectionRequestEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window owner;
        public Window requestor;
        public Atom selection;
        public Atom target;
        public Atom property;
        public long time;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XSelectionEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window requestor;
        public Atom selection;
        public Atom target;
        public Atom property;          /* ATOM or None */
        public long time;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XColorMapEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public Colormap colormap;      /* COLORMAP or None */
        public bool @new;
        public int state;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XClientMessageEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;
        public Atom message_type;
        public int format;
        public IntPtr data;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XMappingEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;        /* true if this came from a SendEvent request */
        public IntPtr display;       /* Display the event was read from */
        public Window window;          /* unused */
        public MappingType request;            /* one of MappingModifier, MappingKeyboard,
                                   MappingPointer */
        public int first_keycode;      /* first keycode */
        public int count;
    }


    [StructLayout(LayoutKind.Sequential, Size = (24 * sizeof(long)))]
    public struct XConfigureRequestEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window parent;
        public Window window;
        public int x;
        public int y;
        public int width;
        public int height;
        public int border_width;
        public Window above;
        public int detail;     /* Above, Below, TopIf, BottomIf, Opposite */
        public ulong value_mask;
    }


    /// <summary>
    /// Raised to notify the X window manager that a window has changed parents.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XReparentEvent
    {
        public int type;               /* ReparentNotify */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window @event;
        public Window window;
        /// <summary>
        /// Identifies the new parent window.
        /// </summary>
        public Window parent;
        public int x, y;
        public bool override_redirect;
    }


    public partial class Xlib
    {
        /// <summary>
        /// The XNextEvent function copies the first event from the event queue into the specified XEvent structure and
        /// then removes it from the queue.If the event queue is empty, XNextEvent flushes the output buffer and blocks
        ///  until an event is received.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XNextEvent(IntPtr display, IntPtr event_return);

        /// <summary>
        /// The XPeekEvent function returns the first event from the event queue, but it does not remove the event from
        /// the queue.If the queue is empty, XPeekEvent flushes the output buffer and blocks until an event is received.
        /// It then copies the event into the client-supplied XEvent structure without removing it from the event queue.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XPeekEvent(IntPtr display, IntPtr event_return);

        /// <summary>
        /// The XWindowEvent function searches the event queue for an event that matches both the specified window and
        /// event mask.When it finds a match, XWindowEvent removes that event from the queue and copies it into the
        /// specified XEvent structure.The other events stored in the queue are not discarded.If a matching event is
        /// not in the queue, XWindowEvent flushes the output buffer and blocks until one is received.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Window specifier</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns>zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XWindowEvent(IntPtr display, Window window, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XCheckWindowEvent function searches the event queue and then the events available on the server connection
        /// for the first event that matches the specified window and event mask.If it finds a match, XCheckWindowEvent
        /// removes that event, copies it into the specified XEvent structure, and returns True.  The other events stored
        /// in the queue are not discarded.  If the event you requested is not available, XCheckWindowEvent returns False,
        /// and the output buffer will have been flushed.
        /// </summary>
        /// <param name="display">Connected display </param>
        /// <param name="window">Window specifier</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XCheckWindowEvent(IntPtr display, Window window, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XMaskEvent function searches the event queue for the events associated with the specified mask.  When it
        /// finds a match, XMaskEvent removes that event and copies it into the specified XEvent structure.The other
        /// events stored in the queue are not discarded.If the event you requested is not in the queue, XMaskEvent
        /// flushes the output buffer and blocks until one is received.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XMaskEvent(IntPtr display, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XCheckMaskEvent function searches the event queue and then any events available on the server connection
        /// for the first event that matches the specified mask.If it finds a match, XCheckMaskEvent removes that event,
        /// copies it into the specified XEvent structure, and returns True.  The other events stored in the queue are not
        /// discarded.  If the event you requested is not available, XCheckMaskEvent returns False, and the output buffer
        /// will have been flushed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern bool XCheckMaskEvent(IntPtr display, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XCheckTypedEvent function searches the event queue and then any events available on the server connection
        /// for the first event that matches the specified type.If it finds a match, XCheckTypedEvent removes that
        /// event, copies it into the specified XEvent structure, and returns True. The other events in the queue are not
        /// discarded.  If the event is not available, XCheckTypedEvent returns False, and the output buffer will have
        /// been flushed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="type">Event type</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern bool XCheckTypedEvent(IntPtr display, Event type, IntPtr event_return);

        /// <summary>
        /// The XCheckTypedWindowEvent function searches the event queue and then any events available on the server con‐
        /// nection for the first event that matches the specified type and window. If it finds a match, XCheckTypedWin‐
        /// dowEvent removes the event from the queue, copies it into the specified XEvent structure, and returns True.
        /// The other events in the queue are not discarded.If the event is not available, XCheckTypedWindowEvent
        /// returns False, and the output buffer will have been flushed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Window specifier</param>
        /// <param name="type">Event type</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern bool XCheckTypedWindowEvent(IntPtr display, Window window, Event type, IntPtr event_return);

        /// <summary>
        /// The XSync function flushes the output buffer and then waits until all requests have been received and pro‐
        /// cessed by the X server.Any errors generated must be handled by the error handler.For each protocol error
        /// received by Xlib, XSync calls the client application's error handling routine (see section 11.8.2).  Any
        /// events generated by the server are enqueued into the library's event queue.
        /// Finally, if you passed False, XSync does not discard the events in the queue.  If you passed True, XSync dis‐
        /// cards all events in the queue, including those events that were on the queue before XSync was called.  Client
        /// applications seldom need to call XSync.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="discard">Discard queued events</param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XSync(IntPtr display, bool discard);

        /// <summary>
        /// The XFlush function flushes the output buffer.  Most client applications need not use this function because
        /// the output buffer is automatically flushed as needed by calls to XPending, XNextEvent, and XWindowEvent.
        /// Events generated by the server may be enqueued into the library's event queue.
        /// </summary>
        /// <param name="display">Connected display </param>
        /// <returns>Zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XFlush(IntPtr display);

        /// <summary>
        /// If mode is QueuedAlready, XEventsQueued returns the number of events already in the event queue (and never
        /// performs a system call).  If mode is QueuedAfterFlush, XEventsQueued returns the number of events already in
        /// the queue if the number is nonzero.If there are no events in the queue, XEventsQueued flushes the output
        /// buffer, attempts to read more events out of the application's connection, and returns the number read.  If
        /// mode is QueuedAfterReading, XEventsQueued returns the number of events already in the queue if the number is
        /// nonzero.If there are no events in the queue, XEventsQueued attempts to read more events out of the applica‐
        /// tion's connection without flushing the output buffer and returns the number read.
        ///
        /// XEventsQueued always returns immediately without I/O if there are events already in the queue.XEventsQueued
        /// with mode QueuedAfterFlush is identical in behavior to XPending.XEventsQueued with mode QueuedAlready is
        /// identical to the XQLength function.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="mode">Mode of operation</param>
        /// <returns>Number of queued events</returns>
        [DllImport("libX11.so.6")]
        public static extern int XEventsQueued(IntPtr display, QueueMode mode);

        /// <summary>
        /// The XPending function returns the number of events that have been received from the X server but have not been
        /// removed from the event queue.XPending is identical to XEventsQueued with the mode QueuedAfterFlush speci‐
        /// fied.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Number of events pending removal from the queue</returns>
        [DllImport("libX11.so.6")]
        public static extern int XPending(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern Status XSendEvent(IntPtr display, Window window, bool propagate, long event_mask, IntPtr event_send);

        [DllImport("libX11.so.6")]
        public static extern X11.Atom XInternAtom(IntPtr display, string name, bool only_if_exists);

        [DllImport("libX11.so.6")]
        public static extern String XGetAtomName(IntPtr display, X11.Atom atom);
    }
}
