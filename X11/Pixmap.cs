using System;
using System.Runtime.InteropServices;

namespace X11
{
    public partial class Xlib
    {
        /// <summary>
        /// The XCreatePixmap() function creates a pixmap of the width, height, and depth you specified and returns a 
        /// pixmap ID that identifies it. It is valid to pass an InputOnly window to the drawable argument. The width
        /// and height arguments must be nonzero, or a BadValue error results. The depth argument must be one of the 
        /// depths supported by the screen of the specified drawable, or a BadValue error results.
        /// The server uses the specified drawable to determine on which screen to create the pixmap. The pixmap can
        /// be used only on this screen and only with other drawables of the same depth (see XCopyPlane() for an 
        /// exception to this rule). The initial contents of the pixmap are undefined.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="drawable">Drawable target for pixmap</param>
        /// <param name="width">Pixmap width</param>
        /// <param name="height">Pixmap height</param>
        /// <param name="depth">Pixmap depth</param>
        /// <returns>Returned Pixmap</returns>
        [DllImport("libX11.so.6")]
        public static extern Pixmap XCreatePixmap(IntPtr display, Window drawable, uint width, uint height, uint depth);

        /// <summary>
        /// The XFreePixmap function first deletes the association between the pixmap ID and the pixmap.  Then, the X
        /// server frees the pixmap storage when there are no references to it.The pixmap should never be referenced
        /// again.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="pixmap">Pixmap to free</param>
        /// <returns>Zero on failure</returns>
        [DllImport("libX11.so.6")]
        public static extern Status XFreePixmap(IntPtr display, Pixmap pixmap);
    }
}
