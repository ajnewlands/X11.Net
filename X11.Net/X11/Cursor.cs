using System;
using System.Runtime.InteropServices;

namespace X11
{
    public enum FontCursor: ulong
    {
        None = 0,
        XC_fleur = 52,
        XC_left_ptr = 68,
        XC_sizing = 120,
    }

    public enum Pixmap : ulong { }

    public enum Cursor : ulong { }

    public enum Font : ulong { }

    public partial class Xlib
    {
        /// <summary>
        /// If a cursor is set, it will be used when the pointer is in the window.  If the cursor is None, it is equiva‐
        /// lent to XUndefineCursor.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Target window</param>
        /// <param name="cursor">Cursor to use</param>
        /// <returns>zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XDefineCursor(IntPtr display, Window window, Cursor cursor);

        /// <summary>
        /// The XUndefineCursor function undoes the effect of a previous XDefineCursor for this window.  When the pointer
        /// is in the window, the parent's cursor will now be used.  On the root window, the default cursor is restored.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Target window</param>
        /// <returns>zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XUndefineCursor(IntPtr display, Window window);

        /// <summary>
        /// X provides a set of standard cursor shapes in a special font named cursor. Applications are encouraged to use
        /// this interface for their cursors because the font can be customized for the individual display type.The
        /// shape argument specifies which glyph of the standard fonts to use.
        /// The hotspot comes from the information stored in the cursor font.The initial colors of a cursor are a black
        /// foreground and a white background (see XRecolorCursor).
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="shape">Cursor shape to use</param>
        /// <returns>Prepared cursor</returns>
        [DllImport("libX11.so.6")]
        public static extern Cursor XCreateFontCursor(IntPtr display, FontCursor shape);

        /// <summary>
        /// The XCreatePixmapCursor function creates a cursor and returns the cursor ID associated with it.  The fore‐
        /// ground and background RGB values must be specified using foreground_color and background_color, even if the X
        /// server only has a StaticGray or GrayScale screen.The foreground color is used for the pixels set to 1 in the
        /// source, and the background color is used for the pixels set to 0.  Both source and mask, if specified, must
        /// have depth one (or a BadMatch error results) but can have any root.The mask argument defines the shape of
        /// the cursor.The pixels set to 1 in the mask define which source pixels are displayed, and the pixels set to 0
        /// define which pixels are ignored.If no mask is given, all pixels of the source are displayed.The mask, if
        /// present, must be the same size as the pixmap defined by the source argument, or a BadMatch error results.The
        /// hotspot must be a point within the source, or a BadMatch error results.
        /// 
        /// The components of the cursor can be transformed arbitrarily to meet display limitations.The pixmaps can be
        /// freed immediately if no further explicit references to them are to be made.  Subsequent drawing in the source
        /// or mask pixmap has an undefined effect on the cursor.  The X server might or might not make a copy of the
        /// pixmap.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="source">Specifies the shape of the source cursor</param>
        /// <param name="mask">Specifies the cursors bits to display</param>
        /// <param name="foreground_color">RGB values for the foreground color</param>
        /// <param name="background_color">RGB values for the background color</param>
        /// <param name="x">hotspot x co-ordinate relative to the source's origin</param>
        /// <param name="y">hotspot y co-ordinate relative to the source's origin</param>
        /// <returns>Prepared cursor</returns>
        [DllImport("libX11.so.6")]
        public static extern Cursor XCreatePixmapCursor(IntPtr display, Pixmap source, Pixmap mask,
            ref XColor foreground_color, ref XColor background_color, uint x, uint y);

        /// <summary>
        /// The XCreateGlyphCursor function is similar to XCreatePixmapCursor except that the source and mask bitmaps are
        /// obtained from the specified font glyphs.The source_char must be a defined glyph in source_font, or a Bad‐
        /// Value error results.If mask_font is given, mask_char must be a defined glyph in mask_font, or a BadValue
        /// error results.The mask_font and character are optional.  The origins of the source_char and mask_char (if
        /// defined) glyphs are positioned coincidently and define the hotspot.The source_char and mask_char need not
        /// have the same bounding box metrics, and there is no restriction on the placement of the hotspot relative to 
        /// the bounding boxes.If no mask_char is given, all pixels of the source are displayed.You can free the fonts
        /// immediately by calling XFreeFont if no further explicit references to them are to be made.
        /// 
        /// For 2-byte matrix fonts, the 16-bit value should be formed with the byte1 member in the most significant byte
        /// and the byte2 member in the least significant byte.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="source_font"></param>
        /// <param name="mask_font"></param>
        /// <param name="source_char"></param>
        /// <param name="mask_char"></param>
        /// <param name="foreground_color"></param>
        /// <param name="background_color"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Cursor XCreateGlyphCursor(IntPtr display, Font source_font, Font mask_font, uint source_char,
              uint mask_char, ref XColor foreground_color, ref XColor background_color);

        /// <summary>
        /// The XRecolorCursor function changes the color of the specified cursor, and if the cursor is being displayed on
        /// a screen, the change is visible immediately.The pixel members of the XColor structures are ignored; only the
        /// RGB values are used.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="cursor">Cursor to recolour</param>
        /// <param name="foreground_color">New foreground colour</param>
        /// <param name="background_color">New background colour</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XRecolorCursor(IntPtr display, Cursor cursor,
            ref XColor foreground_color, ref XColor background_color);

        /// <summary>
        /// The XFreeCursor function deletes the association between the cursor resource ID and the specified cursor.  The
        /// cursor storage is freed when no other resource references it.The specified cursor ID should not be referred
        /// to again.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="cursor">Formatted cursor</param>
        /// <returns>zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XFreeCursor(IntPtr display, Cursor cursor);

        /// <summary>
        /// Some displays allow larger cursors than other displays.  The XQueryBestCursor function provides a way to find
        /// out what size cursors are actually possible on the display.It returns the largest size that can be dis‐
        /// played.Applications should be prepared to use smaller cursors on displays that cannot support large ones.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="drawable">Drawable indicating screen</param>
        /// <param name="width">width of cursor for which size information is needed</param>
        /// <param name="height">height of cursor for which size information is needed</param>
        /// <param name="width_return">Return width closest to specified width</param>
        /// <param name="height_return">Returned height closest to specified height</param>
        /// <returns>zero on error</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XQueryBestCursor (IntPtr display, Window drawable, uint width, uint height,
            ref uint width_return, ref uint height_return);
    }
}
