using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace X11
{
    public enum PixmapFormat : int { XYBitmap = 0, XYPixmap = 1, ZPixmap = 2 }; 

    public enum Direction: int
    {
        RaiseLowest=0,
        LowerHighest=1,
    }

    public enum ChangeMode: int
    {
        SetModeInsert = 0,
        SetModeDelete = 1,
    }

    public enum RevertFocus: int
    {
        RevertToNone =0,
        RevertToPointerRoot =1,
        RevertToParent=2,
    }

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

    [StructLayout(LayoutKind.Sequential)]
    public struct XSetWindowAttributes
    {
        public Pixmap background_pixmap;   /* background or None or ParentRelative */
        public ulong background_pixel;     /* background pixel */
        public Pixmap border_pixmap;       /* border of the window */
        public ulong border_pixel; /* border pixel value */
        public int bit_gravity;            /* one of bit gravity values */
        public int win_gravity;            /* one of the window gravity values */
        public int backing_store;          /* NotUseful, WhenMapped, Always */
        public ulong backing_planes;/* planes to be preseved if possible */
        public ulong backing_pixel;/* value to use in restoring planes */
        public bool save_under;            /* should bits under be saved? (popups) */
        public EventMask event_mask;            /* set of events that should be saved */
        public EventMask do_not_propagate_mask; /* set of events that should not propagate */
        public bool override_redirect;     /* boolean value for override-redirect */
        public Colormap colormap;          /* color map to be associated with window */
        public Cursor cursor;
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

    public partial class Xlib
    {
        /// <summary>
        /// The XGetWindowAttributes function returns the current attributes for the specified window to an XWindowAt‐
        /// tributes structure.It returns a nonzero status on success; otherwise, it returns a zero status.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="window"></param>
        /// <param name="attributes"></param>
        [DllImport("libX11.so.6")]
        public static extern Status XGetWindowAttributes(IntPtr display, Window window, out XWindowAttributes attributes);

        [DllImport("libX11.so.6")]
        public static extern Status XDestroyWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XReparentWindow(IntPtr display, Window window, Window parent, int x, int y);

        [DllImport("libX11.so.6")]
        public static extern Status XAddToSaveSet(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XRemoveFromSaveSet(IntPtr dispay, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XChangeSaveSet(IntPtr display, Window window, ChangeMode change_mode);

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
            uint width, uint height, ulong plane_mask, PixmapFormat format);


        [DllImport("libX11.so.6")]
        public static extern int XSelectInput(IntPtr display, Window window, EventMask event_mask);

        [DllImport("libX11.so.6")]
        public static extern int XQueryTree(IntPtr display, Window window, ref Window WinRootReturn,
            ref Window WinParentReturn, ref IntPtr ChildrenReturn, ref uint nChildren);

        [DllImport("libX11.so.6")]
        public static extern Window XCreateSimpleWindow(IntPtr display, Window parent, int x, int y,
            uint width, uint height, uint border_width, ulong border_colour, ulong background_colour);

        [DllImport("libX11.so.6")]
        public static extern Window XCreateWindow(IntPtr display, Window parent, int x, int y, uint width,
            uint height, uint border_width, int depth, uint @class, IntPtr visual, ulong valuemask,
              ref XSetWindowAttributes attributes);
        
        [DllImport("libX11.so.6")]
        public static extern int XMapWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XUnmapWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XConfigureWindow(IntPtr display, Window window, ulong value_mask, ref XWindowChanges changes);

        [DllImport("libX11.so.6")]
        public static extern int XSetWindowBackground(IntPtr display, Window window, ulong pixel);

        [DllImport("libX11.so.6")]
        public static extern Status XSetWindowBorder(IntPtr display, Window window, ulong border_pixel);

        [DllImport("libX11.so.6")]
        public static extern int XClearWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern int XMoveWindow(IntPtr display, Window window, int x, int y);

        [DllImport("libX11.so.6")]
        public static extern Status XResizeWindow(IntPtr display, Window window, uint width, uint height);

        [DllImport("libX11.so.6")]
        public static extern Status XMoveResizeWindow(IntPtr display, Window window, int x, int y, uint width, uint height);

        [DllImport("libX11.so.6")]
        public static extern Status XSetWindowBorderWidth(IntPtr display, Window window, uint width);

        [DllImport("libX11.so.6")]
        public static extern Status XSetInputFocus(IntPtr display, Window focus, RevertFocus revert_to, long time);

        [DllImport("libX11.so.6")]
        public static extern Status XGetInputFocus(IntPtr display, ref Window focus_return, ref RevertFocus revert_to_return);

        [DllImport("libX11.so.6")]
        public static extern Status XRaiseWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XLowerWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XCirculateSubwindows(IntPtr display, Window window, Direction direction);

        [DllImport("libX11.so.6")]
        public static extern Status XCirculateSubwindowsUp(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XCirculateSubwindowsDown(IntPtr display, Window window);

        [DllImport("libX11.so.6")]
        public static extern Status XRestackWindows(IntPtr display, IntPtr windows, int nwindows);
    }
}
