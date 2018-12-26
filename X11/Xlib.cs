using System;
using System.Runtime.InteropServices;

namespace X11
{
    using XID = UInt64;
    using Window = UInt64;
    using Colormap = UInt64;

    public enum Pixmap : int {XYBitmap = 0, XYPixmap = 1, ZPixmap = 2 };

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

    [StructLayout(LayoutKind.Sequential)]
    public struct XImage
    {
        public int width, height;
        public int xoffset;
        public int format;
        public IntPtr data;
        public int byte_order;
        public int bitmap_unit;
        public int bitmap_bit_order;
        public int bitmap_pad;
        public int depth;
        public int bytes_per_line;
        public int bits_per_pixel;
        public ulong red_mask;
        public ulong green_mask;
        public ulong blue_mask;
        public IntPtr obdata;
        private struct funcs
        {
            IntPtr create_image;
            IntPtr destroy_image;
            IntPtr get_pixel;
            IntPtr put_pixel;
            IntPtr sub_image;
            IntPtr add_pixel;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XWindowAttributes
    {
        public int x, y;
        public uint width, height;
        public int border_width;
        public int depth;
        public IntPtr visual;
        public Window root;
        public int @class;
        public int bit_gravity;
        public int win_gravity;
        public int backing_store;
        public ulong backing_planes;
        public ulong backing_pixel;
        public bool save_under;
        public Colormap colormap; 
        public bool map_installed;
        public int map_state;
        public long all_event_masks;
        public long your_event_masks;
        public long do_not_propagate_mask;
        public bool override_redirect;
        public IntPtr screen;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24*sizeof(long)))]
    public struct XEvent
    {
        public int type;
        public ulong serial;
        public bool send_event;
        public IntPtr display;
    }

    [StructLayout(LayoutKind.Sequential, Size = (24*sizeof(long)))]
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
    public struct XMapNotifyEvent
    {
        public int type;
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window @event;
        public Window window;
        public bool override_redirect; /* boolean, is override set... */
        }

    [StructLayout(LayoutKind.Sequential, Size = (24*sizeof(long)))]
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
        public ulong  time;      /* milliseconds */
        public int x, y;       /* pointer x, y coordinates in event window */
        public int x_root, y_root; /* coordinates relative to root */
        public uint state; /* key or button mask */
        public uint button;    /* detail */
        public bool same_screen;
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

    [StructLayout(LayoutKind.Sequential, Size = (24*sizeof(long)))]
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

    [StructLayout(LayoutKind.Sequential)]
    public struct XCreateNotifyEvent
    {
        public int type;       /* CreateNotify */
        public ulong serial;   /* # of last request processed by server */
        public bool send_event;    /* true if this came from a SendEvent request */
        public IntPtr display;   /* Display the event was read from */
        public Window parent;      /* parent of the window */
        public Window window;      /* window id of window created */
        public int x, y;       /* window location */
        public int width, height;  /* size of window */
        public int border_width;   /* border width */
        public bool override_redirect;	/* creation should be overridden */
    }

    /// <summary>
    /// Raised to notify the X window manager that a window has changed parents.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XReparentNotifyEvent
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
    public struct XWindowChanges
    {
        public int x, y;
        public int width, height;
        public int border_width;
        public Window sibling;
        public int stack_mode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XColor
    {
        public ulong pixel;
        public ushort red, green, blue;
        public byte flags;  /* do_red, do_green, do_blue */
        public byte pad;
    }

    public class Xlib
    {
        /// <summary>
        /// Used e.g. with XGetImage
        /// </summary>
        public const ulong AllPlanes = 0xffffffffffffffff;



        /// <summary>
        /// Initiate a connection to the name X session.
        /// (or respect the DISPLAY environment variable if the display parameter is null).
        /// </summary>
        /// <param name="display">X session connection string</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XOpenDisplay(string display);

        /// <summary>
        /// Free an unmanaged display pointer (created with XOpenDisplay)
        /// </summary>
        /// <param name="display">Display pointer to free</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XCloseDisplay(IntPtr display);

        /// <summary>
        /// Retrieve the Window ID corresponding to the displays root window.
        /// </summary>
        /// <param name="display">Pointer to an open X display</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Window XDefaultRootWindow(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern void XGetWindowAttributes(IntPtr display, XID window, out XWindowAttributes attributes );

        /// <summary>
        /// Returns a pointer which can be marshalled to an XImage object for field access in managed code.
        /// This should be freed after use with the XDestroyImage function (from Xutil).
        /// </summary>
        /// <param name="display">Pointer to an open X display</param>
        /// <param name="drawable">Window ID to capture</param>
        /// <param name="x">X-offset for capture region</param>
        /// <param name="y">Y-offset for capture region</param>
        /// <param name="width">Width of capture region</param>
        /// <param name="height">Height of capture region</param>
        /// <param name="plane_mask"></param>
        /// <param name="format">One of XYBitmap, XYPixmap, ZPixmap</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern ref XImage XGetImage(IntPtr display, Window drawable, int x, int y,
            uint width, uint height, ulong plane_mask, Pixmap format);

        [DllImport("libX11.so.6")]
        public static extern IntPtr XSetErrorHandler(XErrorHandlerDelegate del);

        [DllImport("libX11.so.6")]
        public static extern int XSelectInput(IntPtr display, ulong window, long event_mask);

        [DllImport("libX11.so.6")]
        public static extern int XSync(IntPtr display, bool discard);

        [DllImport("libX11.so.6")]
        public static extern int XNextEvent(IntPtr display, IntPtr ev);

        [DllImport("libX11.so.6")]
        public static extern int XQueryTree(IntPtr display, Window window, ref ulong WinRootReturn,
            ref ulong WinParentReturn, ref ulong[] ChildrenReturn, ref uint nChildren);

        [DllImport("libX11.so.6")]
        public static extern Window XCreateSimpleWindow(IntPtr display, Window parent, int x, int y,
            uint width, uint height, uint border_width, ulong border_colour, ulong background_colour);

        [DllImport("libX11.so.6")]
        public static extern int XDestroyWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XReparentWindow(IntPtr display, Window window, Window parent, int x, int y);

        [DllImport("libX11.so.6")]
        public static extern int XMapWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XUnmapWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XAddToSaveSet(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XRemoveFromSaveSet(IntPtr dispay, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XConfigureWindow(IntPtr display, Window window, ulong value_mask, ref XWindowChanges changes);

        [DllImport("libX11.so.6")]
        public static extern int XGetErrorText(IntPtr display, int code, IntPtr description, int length);

        [DllImport("libX11.so.6")]
        public static extern IntPtr XSynchronize(IntPtr display, bool onoff);

        [DllImport("libX11.so.6")]
        public static extern int XGrabServer(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern int XUngrabServer(IntPtr display);

        [DllImport("libX11.so.6")]
        public static extern IntPtr XDisplayName(string display);

        [DllImport("libX11.so.6")]
        public static extern int XGrabButton(IntPtr display, uint button, uint modifiers, Window grab_window,
            bool owner_events, uint event_mask, int pointer_mode, int keyboard_mode, Window confine_to, ulong cursor);

        [DllImport("libX11.so.6")]
        public static extern int XUngrabButton(IntPtr display, uint button, uint modifiers, Window grab_button);

        [DllImport("libX11.so.6")]
        public static extern int XDefineCursor(IntPtr display, Window window, ulong cursor);

        [DllImport("libX11.so.6")]
        public static extern int XUndefineCursor(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern ulong XCreateFontCursor(IntPtr display, ulong cursor_id);

        [DllImport("libX11.so.6")]
        public static extern ulong XDefaultColormap(IntPtr display, int screen);

        [DllImport("libX11.so.6")]
        public static extern int XParseColor(IntPtr display, ulong Colormap, string spec, ref XColor xcolor_return);

        [DllImport("libX11.so.6")]
        public static extern int XAllocColor(IntPtr display, ulong Colormap, ref XColor screen_in_out);

        [DllImport("libX11.so.6")]
        public static extern int XSetWindowBackground(IntPtr display, Window window, ulong pixel);

        [DllImport("libX11.so.6")]
        public static extern int XClearWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XMoveWindow(IntPtr display, Window window, int x, int y);
    }
}
