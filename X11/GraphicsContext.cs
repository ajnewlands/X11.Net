using System;
using System.Runtime.InteropServices;

namespace X11
{        
    public enum GCComponents: ulong
    {
         GCFunction = (1L<<0),
         GCPlaneMask = (1L<<1),
         GCForeground = (1L<<2),
         GCBackground = (1L<<3),
         GCLineWidth = (1L<<4),
         GCLineStyle = (1L<<5),
         GCCapStyle = (1L<<6),
         GCJoinStyle = (1L<<7),
         GCFillStyle = (1L<<8),
         GCFillRule = (1L<<9),
         GCTile = (1L<<10),
         GCStipple = (1L<<11),
         GCTileStipXOrigin = (1L<<12),
         GCTileStipYOrigin = (1L<<13),
         GCFont = (1L<<14),
         GCSubwindowMode = (1L<<15),
         GCGraphicsExposures = (1L<<16),
         GCClipXOrigin = (1L<<17),
         GCClipYOrigin = (1L<<18),
         GCClipMask = (1L<<19),
         GCDashOffset = (1L<<20),
         GCDashList = (1L<<21),
         GCArcMode = (1L<<22),
    }

    public enum LineStyle: int
    {
        LineSolid = 0,
        LineOnOffDash = 1,
        LineDoubleDash = 2,
    }

    public enum CapStyle: int
    {
        CapNotLast = 0,
        CapButt = 1,
        CapRound = 2,
        CapProjecting = 3,
    }

    public enum JoinStyle: int
    {
        JoinMiter = 0,
        JoinRound = 1,
        JoinBevel = 2,
    }

    public enum FillStyle: int
    {
        FillSolid = 0,
        FillTiled = 1,
        FillStippled = 2,
        FillOpaqueStippled = 3,
    }

    public enum FillRule: int
    {
        EvenOddRule = 0,
        WindingRule = 1,
    }

    public enum SubwindowMode: int
    {
        ClipByChildren = 0,
        IncludeInferiors = 1,
    }

    public enum ArcMode: int
    {
        /// <summary>
        /// join endpoints of arc
        /// </summary>
        ArcChord = 0, 
        /// <summary>
        /// join endpoints to center of arc
        /// </summary>
        ArcPieSlice = 1,
    }

    public enum BestSizeClass: int
    {
        /// <summary>
        /// largest size that can be displayed
        /// </summary>
        CursorShape = 0,
        /// <summary>
        /// size tiled fastest
        /// </summary>
        TileShape = 1,
        /// <summary>
        /// Size stippled fastest
        /// </summary>
        StippleShape = 2,
    }

    public enum ClipOrdering: int
    {
        Unsorted = 0,
        YSorted = 1,
        YXSorted = 2,
        YXBanded = 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XRectangle {
        short x, y;
        ushort width, height;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct XGCValues
    {
        int function;       /* logical operation */
        ulong plane_mask;/* plane mask */
        ulong foreground;/* foreground pixel */
        ulong background;/* background pixel */
        int line_width;     /* line width */
        LineStyle line_style;     /* LineSolid, LineOnOffDash, LineDoubleDash */
        CapStyle cap_style;      /* CapNotLast, CapButt, CapRound, CapProjecting */
        JoinStyle join_style;     /* JoinMiter, JoinRound, JoinBevel */
        FillStyle fill_style;     /* FillSolid, FillTiled, FillStippled, FillOpaeueStippled */
        FillRule fill_rule;      /* EvenOddRule, WindingRule */
        ArcMode arc_mode;       /* ArcChord, ArcPieSlice */
        Pixmap tile;        /* tile pixmap for tiling operations */
        Pixmap stipple;     /* stipple 1 plane pixmap for stipping */
        int ts_x_origin;    /* offset for tile or stipple operations */
        int ts_y_origin;
        Font font;          /* default text font for text operations */
        SubwindowMode subwindow_mode;     /* ClipByChildren, IncludeInferiors */
        bool graphics_exposures;/* boolean, should exposures be generated */
        int clip_x_origin;  /* origin for clipping */
        int clip_y_origin;
        Pixmap clip_mask;   /* bitmap clipping; other calls for rects */
        int dash_offset;    /* patterned/dashed line information */
        byte dashes;
    }

    public partial class Xlib
    {
        /// <summary>
        /// vThe XCreateGC function creates a graphics context and returns a GC.  The GC can be used with any destination
        /// drawable having the same root and depth as the specified drawable.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="drawable"></param>
        /// <param name="valuemask"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XCreateGC(IntPtr display, Window drawable, GCComponents valuemask, ref XGCValues values);

        /// <summary>
        /// The XCopyGC function copies the specified components from the source GC to the destination GC.  
        /// The source and destination GCs must have the same root and depth.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="src"></param>
        /// <param name="valuemask"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XCopyGC(IntPtr display, IntPtr src, GCComponents valuemask, IntPtr dest);

        [DllImport("libX11.so.6")]
        public static extern Status XChangeGC(IntPtr display, IntPtr gc, ulong valuemask, ref XGCValues values);

        [DllImport("libX11.so.6")]
        public static extern Status XGetGCValues(IntPtr display, IntPtr gc, ulong valuemask,
            ref XGCValues values_return);

        [DllImport("libX11.so.6")]
        public static extern Status XFreeGC(IntPtr display,  IntPtr gc);

        [DllImport("libX11.so.6")]
        public static extern XID XGContextFromGC(IntPtr gc);

        /// <summary>
        /// Force sending of GC changes immediately.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="gc"></param>
        [DllImport("libX11.so.6")]
        public static extern void _XFlushGCCache(IntPtr display, IntPtr gc);

        [DllImport("libX11.so.6")]
        public static extern Status XSetState(IntPtr display, IntPtr gc, ulong foreground, ulong background, int function,
              ulong plane_mask);

        [DllImport("libX11.so.6")]
        public static extern Status XSetFunction(IntPtr display, IntPtr gc, int function);

        [DllImport("libX11.so.6")]
        public static extern Status XSetPlaneMask(IntPtr display, IntPtr gc, ulong plane_mask);

        [DllImport("libX11.so.6")]
        public static extern Status XSetForeground(IntPtr display, IntPtr gc, ulong foreground);

        [DllImport("libX11.so.6")]
        public static extern Status XSetBackground(IntPtr display, IntPtr gc, ulong background);

        [DllImport("libX11.so.6")]
        public static extern Status XSetLineAttributes(IntPtr display, IntPtr gc, uint line_width, 
            LineStyle line_style, CapStyle cap_style, JoinStyle join_style);

        [DllImport("libX11.so.6")]
        public static extern Status XSetDashes(IntPtr display, IntPtr gc, int dash_offset, byte[] dash_list, int n);

        [DllImport("libX11.so.6")]
        public static extern Status XSetFillStyle(IntPtr display, IntPtr gc, FillStyle fill_style);

        [DllImport("libX11.so.6")]
        public static extern Status XSetFillRule(IntPtr display, IntPtr gc, FillRule fill_rule);

        [DllImport("libX11.so.6")]
        public static extern Status XQueryBestSize(IntPtr display, BestSizeClass best_size_class, Window which_screen, 
            uint width, uint height, ref uint width_return, ref uint height_return);

        [DllImport("libX11.so.6")]
        public static extern Status XQueryBestTile(IntPtr display, Window which_screen, uint width, uint height,
               ref uint width_return, ref uint height_return);

        [DllImport("libX11.so.6")]
        public static extern Status XQueryBestStipple(IntPtr display, Window which_screen, uint width, uint height,
               ref uint width_return, ref uint height_return);

        [DllImport("libX11.so.6")]
        public static extern Status XSetTile(IntPtr display, IntPtr gc, Pixmap tile);

        [DllImport("libX11.so.6")]
        public static extern Status XSetStipple(IntPtr display, IntPtr gc, Pixmap stipple);

        [DllImport("libX11.so.6")]
        public static extern Status XSetTSOrigin(IntPtr display, IntPtr gc, int ts_x_origin, int ts_y_origin);

        [DllImport("libX11.so.6")]
        public static extern Status XSetFont(IntPtr display, IntPtr gc, Font font);

        [DllImport("libX11.so.6")]
        public static extern Status XSetClipOrigin(IntPtr display, IntPtr gc, int clip_x_origin, int clip_y_origin);

        [DllImport("libX11.so.6")]
        public static extern Status XSetClipMask(IntPtr display, IntPtr gc, Pixmap pixmap);

        [DllImport("libX11.so.6")]
        public static extern Status XSetClipRectangles(IntPtr display, IntPtr gc, int clip_x_origin, int clip_y_origin,
            XRectangle[] rectangles, int n, ClipOrdering ordering);

        [DllImport("libX11.so.6")]
        public static extern Status XSetArcMode(IntPtr display, IntPtr gc, ArcMode arc_mode);

        [DllImport("libX11.so.6")]
        public static extern Status XSetSubwindowMode(IntPtr display, IntPtr gc, SubwindowMode subwindow_mode);

        [DllImport("libX11.so.6")]
        public static extern Status XSetGraphicsExposures(IntPtr display, IntPtr gc, bool graphics_exposures);
    }
}
