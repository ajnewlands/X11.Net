using System;
using System.Runtime.InteropServices;

namespace X11
{
    public enum Colormap : ulong { }

    public enum XcmsColor : ulong { }

    public enum XcmsColorFormat : ulong
    {
        XcmsUndefinedFormat = 0x00000000,
        XcmsCIEXYZFormat = 0x00000001,
        XcmsCIEuvYFormat = 0x00000002,
        XcmsCIExyYFormat = 0x00000003,
        XcmsCIELabFormat = 0x00000004,
        XcmsCIELuvFormat = 0x00000005,
        XcmsTekHVCFormat = 0x00000006,
        XcmsRGBFormat = 0x80000000,
        XcmsRGBiFormat  = 0x80000001,
    }

    public enum ColormapAlloc : int
    {
        AllocNone = 0,
        AllocAll = 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XColor
    {
        public ulong pixel;
        public ushort red, green, blue;
        public byte flags;  /* do_red, do_green, do_blue */
        public byte pad;
    }

    public partial class Xlib
    {
        /// <summary>
        /// Returns the default colormap ID for allocation on the specified screen. 
        /// Most routine allocations of color should be made out of this colormap.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="screen_number">Target screen</param>
        /// <returns>Default color map for screen</returns>
        [DllImport("libX11.so.6")]
        public static extern Colormap XDefaultColormap(IntPtr display, int screen_number);

        /// <summary>
        /// The XCreateColormap function creates a colormap of the specified visual type for the screen on which the spec‐
        /// ified window resides and returns the colormap ID associated with it.Note that the specified window is only
        /// used to determine the screen.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Window used to determine screen</param>
        /// <param name="visual">Desired visual type</param>
        /// <param name="alloc">Allocation mode</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Colormap XCreateColormap(IntPtr display, Window window, IntPtr visual, ColormapAlloc alloc);

        /// <summary>
        /// The XCopyColormapAndFree function creates a colormap of the same visual type and for the same screen as the
        /// specified colormap and returns the new colormap ID.It also moves all of the client's existing allocation
        /// from the specified colormap to the new colormap with their color values intact and their read-only or writable
        /// characteristics intact and frees those entries in the specified colormap.Color values in other entries in
        /// the new colormap are undefined.If the specified colormap was created by the client with alloc set to Allo‐
        /// cAll, the new colormap is also created with AllocAll, all color values for all entries are copied from the
        /// specified colormap, and then all entries in the specified colormap are freed.If the specified colormap was
        /// not created by the client with AllocAll, the allocations to be moved are all those pixels and planes that have
        /// been allocated by the client using XAllocColor, XAllocNamedColor, XAllocColorCells, or XAllocColorPlanes and
        /// that have not been freed since they were allocated.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Source colormap</param>
        /// <returns>New colormap</returns>
        [DllImport("libX11.so.6")]
        public static extern Colormap XCopyColormapAndFree(IntPtr display, Colormap colormap);

        /// <summary>
        /// The XFreeColormap function deletes the association between the colormap resource ID and the colormap and frees
        /// the colormap storage.However, this function has no effect on the default colormap for a screen.If the
        /// specified colormap is an installed map for a screen, it is uninstalled (see XUninstallColormap).  If the spec‐
        /// ified colormap is defined as the colormap for a window (by XCreateWindow, XSetWindowColormap, or XChangeWin‐
        /// dowAttributes), XFreeColormap changes the colormap associated with the window to None and generates a Col‐
        /// ormapNotify event.  X does not define the colors displayed for a window with a colormap of None.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to free</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XFreeColormap(IntPtr display, Colormap colormap);


        /// <summary>
        /// The XParseColor function looks up the string name of a color with respect to the screen associated with the
        /// specified colormap.It returns the exact color value.If the color name is not in the Host Portable Charac‐
        /// ter Encoding, the result is implementation-dependent. Use of uppercase or lowercase does not matter. XParse‐
        /// Color returns nonzero if the name is resolved; otherwise, it returns zero.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Desired colormap</param>
        /// <param name="spec">Color name</param>
        /// <param name="xcolor_return">returned color</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XParseColor(IntPtr display, Colormap colormap, string spec, ref XColor xcolor_return);

        /// <summary>
        /// The XAllocColor function allocates a read-only colormap entry corresponding to the closest RGB value supported
        /// by the hardware. XAllocColor returns the pixel value of the color closest to the specified RGB elements sup‐
        /// ported by the hardware and returns the RGB value actually used.The corresponding colormap cell is read-only.
        /// In addition, XAllocColor returns nonzero if it succeeded or zero if it failed. Multiple clients that request
        /// the same effective RGB value can be assigned the same read-only entry, thus allowing entries to be shared.
        ///
        /// When the last client deallocates a shared cell, it is deallocated.XAllocColor does not use or affect the
        /// flags in the XColor structure.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="screen_in_out">Allocated colour</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XAllocColor(IntPtr display, Colormap colormap, ref XColor screen_in_out);

        /// <summary>
        /// The XAllocColorCells function allocates read/write color cells.  The number of colors must be positive and the
        /// number of planes nonnegative, or a BadValue error results.If ncolors and nplanes are requested, then ncolors
        /// pixels and nplane plane masks are returned.No mask will have any bits set to 1 in common with any other mask
        /// or with any of the pixels.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="contig">Specify planes must be contiguous</param>
        /// <param name="plane_masks_return">Array of returned plane masks</param>
        /// <param name="nplanes">Number of plane masks to return</param>
        /// <param name="pixels_return">Returned array of pixels</param>
        /// <param name="npixels">Number of pixels to return</param>
        /// <returns>Zero on failures</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XAllocColorCells(IntPtr display, Colormap colormap, bool contig, ulong[] plane_masks_return,
              uint nplanes, ulong[] pixels_return, uint npixels);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="contig"></param>
        /// <param name="pixels_return"></param>
        /// <param name="ncolors"></param>
        /// <param name="nreds"></param>
        /// <param name="ngreens"></param>
        /// <param name="nblues"></param>
        /// <param name="rmask_return"></param>
        /// <param name="gmask_return"></param>
        /// <param name="bmask_return"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XAllocColorPlanes(IntPtr display, Colormap colormap, bool contig, 
            ulong[] pixels_return, int ncolors, int nreds, int ngreens, int nblues, ref ulong rmask_return, 
            ref ulong gmask_return, ref ulong bmask_return);

        /// <summary>
        /// The XcmsAllocColor function is similar to XAllocColor except the color can be specified in any format. The
        /// XcmsAllocColor function ultimately calls XAllocColor to allocate a read-only color cell(colormap entry) with
        /// the specified color. XcmsAllocColor first converts the color specified to an RGB value and then passes this
        /// to XAllocColor. XcmsAllocColor returns the pixel value of the color cell and the color specification actually
        /// allocated. This returned color specification is the result of converting the RGB value returned by XAlloc‐
        /// Color into the format specified with the result_format argument. If there is no interest in a returned color
        /// specification, unnecessary computation can be bypassed if result_format is set to XcmsRGBFormat. The corre‐
        /// sponding colormap cell is read-only. If this routine returns XcmsFailure, the color_in_out color specifica‐
        /// tion is left unchanged.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Specified colormap</param>
        /// <param name="color_in_out">Colour to convert</param>
        /// <param name="result_format">Target Colour format</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsAllocColor(IntPtr display, Colormap colormap, ref XcmsColor color_in_out,
            XcmsColorFormat result_format);

        /// <summary>
        /// The XcmsAllocNamedColor function is similar to XAllocNamedColor except that the color returned can be in any
        /// format specified.This function ultimately calls XAllocColor to allocate a read-only color cell with the
        /// color specified by a color string.  The color string is parsed into an XcmsColor structure (see XcmsLookup‐
        /// Color), converted to an RGB value, and finally passed to XAllocColor.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Specified colormap</param>
        /// <param name="color_string">Name of the colour to use</param>
        /// <param name="color_screen_return">Nearest colour mapping for screen</param>
        /// <param name="color_exact_return">Exact colour mapping</param>
        /// <param name="result_format">Desired result format</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsAllocNamedColor(IntPtr display, Colormap colormap, IntPtr color_string, 
            ref XcmsColor color_screen_return, ref XcmsColor color_exact_return, XcmsColorFormat result_format);

        /// <summary>
        /// The XQueryColor function returns the current RGB value for the pixel in the XColor structure and sets the
        /// DoRed, DoGreen, and DoBlue flags.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="def_in_out">Reference colour</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XQueryColor(IntPtr display, Colormap colormap, ref XColor def_in_out);

        /// <summary>
        /// The XcmsQueryColor function obtains the RGB value for the pixel value in the pixel member of the specified
        /// XcmsColor structure and then converts the value to the target format as specified by the result_format argu‐
        /// ment.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="color_in_out"></param>
        /// <param name="result_format"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsQueryColor (IntPtr display, Colormap colormap, ref XcmsColor color_in_out, 
            XcmsColorFormat result_format);

        /// <summary>
        /// The XcmsQuery‐
        /// Colors function obtains the RGB values for pixel values in the pixel members of XcmsColor structures and then
        /// converts the values to the target format as specified by the result_format argument.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="colors_in_out"></param>
        /// <param name="ncolors"></param>
        /// <param name="result_format"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsQueryColors(IntPtr display, Colormap colormap, XcmsColor[] colors_in_out,
            uint ncolors, XcmsColorFormat result_format);

        /// <summary>
        /// The XcmsLookupColor function looks up the string name of a color with respect to the screen associated with
        /// the specified colormap.It returns both the exact color values and the closest values provided by the screen
        /// with respect to the visual type of the specified colormap.The values are returned in the format specified by
        /// result_format.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="color_string"></param>
        /// <param name="color_exact_return"></param>
        /// <param name="color_screen_return"></param>
        /// <param name="result_format"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsLookupColor(IntPtr display, Colormap colormap, IntPtr color_string, 
            ref XcmsColor color_exact_return, ref XcmsColor color_screen_return, XcmsColorFormat result_format);

        /// <summary>
        /// The XQueryColors function returns the RGB value for each pixel in each
        /// XColor structure and sets the DoRed, DoGreen, and DoBlue flags in each structure.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="defs_in_out">Reference colours</param>
        /// <param name="ncolors">Number of colours in the array</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XQueryColors(IntPtr display, Colormap colormap, XColor[] defs_in_out, int ncolors);

        /// <summary>
        /// The XLookupColor function looks up the string name of a color with respect to the screen associated with the
        /// specified colormap.It returns both the exact color values and the closest values provided by the screen with
        /// respect to the visual type of the specified colormap.  If the color name is not in the Host Portable Character 
        /// Encoding, the result is implementation-dependent. Use of uppercase or lowercase does not matter.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="color_name">Name of the desired colour</param>
        /// <param name="exact_def_return">Returned exact colormap</param>
        /// <param name="screen_def_return">Returned nearest colormap for screen</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XLookupColor(IntPtr display, Colormap colormap, IntPtr color_name,
            ref XColor exact_def_return, ref XColor screen_def_return);

        /// <summary>
        /// The XStoreColors function changes the colormap entries of the pixel values specified in the pixel members of
        /// the XColor structures.You specify which color components are to be changed by setting DoRed, DoGreen, and/or
        /// DoBlue in the flags member of the XColor structures.If the colormap is an installed map for its screen, the
        /// changes are visible immediately.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="color">Array of colours to update</param>
        /// <param name="ncolors">Number of colours to update</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XStoreColors(IntPtr display, Colormap colormap, XColor[] color, int ncolors);

        /// <summary>
        /// The XStoreColor function changes the colormap entry of the pixel value specified in the pixel member of the
        /// XColor structure.You specified this value in the pixel member of the XColor structure.This pixel value
        /// must be a read/write cell and a valid index into the colormap.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="color">Color to replace</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XStoreColor(IntPtr display, Colormap colormap, ref XColor color);

        /// <summary>
        ///  The XcmsStoreColor function converts the color specified in the XcmsColor structure into RGB values.  It then
        /// uses this RGB specification in an XColor structure, whose three flags (DoRed, DoGreen, and DoBlue) are set, in
        /// a call to XStoreColor to change the color cell specified by the pixel member of the XcmsColor structure.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsStoreColor(IntPtr display, Colormap colormap, ref XcmsColor color);

        /// <summary>
        /// The XcmsStoreColors function converts the colors specified in the array of XcmsColor structures into RGB
        /// values and then uses these RGB specifications in XColor structures, whose three flags(DoRed, DoGreen, and
        /// DoBlue) are set, in a call to XStoreColors to change the color cells specified by the pixel member of the 
        /// corresponding XcmsColor structure.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="colors"></param>
        /// <param name="ncolors"></param>
        /// <param name="compression_flags_return"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsStoreColors(IntPtr display, Colormap colormap, XcmsColor[] colors, 
            int ncolors, bool[] compression_flags_return);

        /// <summary>
        /// The XStoreNamedColor function looks up the named color with respect to the screen associated with the colormap
        /// and stores the result in the specified colormap.The pixel argument determines the entry in the colormap.
        /// The flags argument determines which of the red, green, and blue components are set.You can set this member
        /// to the bitwise inclusive OR of the bits DoRed, DoGreen, and DoBlue.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Colormap to use</param>
        /// <param name="color">Colour to lookup by name</param>
        /// <param name="pixel">RGB components</param>
        /// <param name="flags">Which of RGB components to store</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XStoreNamedColor(IntPtr display, Colormap colormap, IntPtr color,
            ulong pixel, int flags);

        /// <summary>
        /// The XFreeColors function frees the cells represented by pixels whose values are in the pixels array. The
        /// planes argument should not have any bits set to 1 in common with any of the pixels. The set of all pixels is
        /// produced by ORing together subsets of the planes argument with the pixels. The request frees all of these
        /// pixels that were allocated by the client (using XAllocColor, XAllocNamedColor, XAllocColorCells, and XAlloc‐
        /// ColorPlanes). Note that freeing an individual pixel obtained from XAllocColorPlanes may not actually allow it
        /// to be reused until all of its related pixels are also freed. Similarly, a read-only entry is not actually
        /// freed until it has been freed by all clients, and if a client allocates the same read-only entry multiple
        /// times, it must free the entry that many times before the entry is actually freed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="colormap">Specified colormap</param>
        /// <param name="pixels">Array of pixels representing cells to free</param>
        /// <param name="npixels">Number of pixels in array</param>
        /// <param name="planes"></param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XFreeColors(IntPtr display, Colormap colormap, ulong[] pixels, 
            int npixels, ulong planes);


        /// <summary>
        /// The XcmsConvertColors function converts the color specifications in the specified array of XcmsColor
        /// structures from their current format to a single target format, using the specified CCC.
        /// </summary>
        /// <param name="ccc"></param>
        /// <param name="colors_in_out"></param>
        /// <param name="ncolors"></param>
        /// <param name="target_format"></param>
        /// <param name="compression_flags_return"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsConvertColors(IntPtr ccc, XcmsColor[] colors_in_out, uint ncolors, 
            XcmsColorFormat target_format, bool[] compression_flags_return);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Status XcmsCompressionProc(IntPtr CCC, XcmsColor[] colors_int_out, uint ncolors,
            uint index, bool[] compression_flags_return);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Status XcmsWhiteAdjustProc(IntPtr ccc, ref XcmsColor initial_white_point,
            ref XcmsColor target_white_point, XcmsColorFormat target_format, XcmsColor[] colors_in_out,
            uint ncolors, bool[] compression_flags_return);

        /// <summary>
        /// The XcmsSetWhitePoint function changes the Client White Point in the specified CCC.  Note that the pixel 
        /// member is ignored and that the color specification is left unchanged upon return.  The format for the new
        /// whitepoint must be XcmsCIEXYZFormat, XcmsCIEuvYFormat, XcmsCIExyYFormat, or XcmsUndefinedFormat.
        /// </summary>
        /// <param name="ccc"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XcmsSetWhitePoint(IntPtr ccc, ref XcmsColor color);

        /// <summary>
        /// The XcmsSetWhiteAdjustProc function first sets the white point adjustment procedure and client data in the
        // specified CCC with the newly specified procedure and client data and then returns the old procedure.
        /// </summary>
        /// <param name="ccc"></param>
        /// <param name="white_adjust_proc"></param>
        /// <param name="client_data"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern XcmsWhiteAdjustProc XcmsSetWhiteAdjustProc(IntPtr ccc, XcmsWhiteAdjustProc white_adjust_proc,
            IntPtr client_data);

        /// <summary>
        /// The XcmsCreateCCC function creates a CCC for the specified display, screen, and visual.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="screen_number"></param>
        /// <param name="visual"></param>
        /// <param name="client_white_point"></param>
        /// <param name="compression_proc"></param>
        /// <param name="compression_client_data"></param>
        /// <param name="white_adjust_proc"></param>
        /// <param name="white_adjust_client_data"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XcmsCreateCCC(IntPtr display, int screen_number, IntPtr visual, 
            ref XcmsColor client_white_point, XcmsCompressionProc compression_proc, IntPtr compression_client_data,
            XcmsWhiteAdjustProc white_adjust_proc, IntPtr white_adjust_client_data);

        /// <summary>
        /// The XcmsFreeCCC function frees the memory used for the specified CCC.  Note that default CCCs and those 
        /// currently associated with colormaps are ignored.
        /// </summary>
        /// <param name="ccc"></param>
        [DllImport("libX11.so.6")]
        public static extern void XcmsFreeCCC(IntPtr ccc);

        /// <summary>
        /// The XcmsCCCOfColormap function returns the CCC associated with the specified colormap. Once obtained, the CCC
        /// attributes can be queried or modified.Unless the CCC associated with the specified colormap is changed with
        /// XcmsSetCCCOfColormap, this CCC is used when the specified colormap is used as an argument to color functions.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XcmsCCCOfColormap(IntPtr display, Colormap colormap);

        /// <summary>
        /// The XcmsSetCCCOfColormap function changes the CCC associated with the specified colormap. It returns the CCC
        /// previously associated with the colormap. If they are not used again in the application, CCCs should be freed
        /// by calling XcmsFreeCCC. Several colormaps may share the same CCC without restriction; this includes the CCCs
        /// generated by Xlib with each colormap. Xlib, however, creates a new CCC with each new colormap.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="colormap"></param>
        /// <param name="ccc"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XcmsSetCCCOfColormap(IntPtr display, Colormap colormap, IntPtr ccc);

        /// <summary>
        /// The XcmsDefaultCCC function returns the default CCC for the specified screen.  
        /// Its visual is the default visual of the screen.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="screen_number"></param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern IntPtr XcmsDefaultCCC(IntPtr display, int screen_number);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsTekHVCQueryMaxC(IntPtr ccc, double hue, double value, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsTekHVCQueryMaxV(IntPtr ccc, double hue, double chroma, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsTekHVCQueryMaxVC(IntPtr ccc, double hue, ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsTekHVCQueryMaxVSamples(IntPtr ccc, double hue, XcmsColor[] colors_return,
            uint nsamples);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsTekHVCQueryMinV(IntPtr ccc, double hue, double chroma,
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELabQueryMaxC(IntPtr ccc, double hue_angle, double L_star, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELabQueryMaxL(IntPtr ccc, double hue_angle, double chroma, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELabQueryMaxLC(IntPtr ccc, double hue_angle, ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELabQueryMinL(IntPtr ccc, double hue_angle, double chroma, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELuvQueryMaxC(IntPtr ccc, double hue_angle, double L_star, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELuvQueryMaxL(IntPtr ccc, double hue_angle, double chroma, 
            ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELuvQueryMaxLC(IntPtr ccc, double hue_angle, ref XcmsColor color_return);

        [DllImport("libX11.so.6")]
        public static extern Status XcmsCIELuvQueryMinL(IntPtr ccc, double hue_angle, double chroma,
            ref XcmsColor color_return);
    }
}
