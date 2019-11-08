using System;
using System.Runtime.InteropServices;

namespace X11
{
    public class XTest
    {
        /// <summary>
        /// Requests that the server simulate a mouse movement to the desired co-ordinates on the nominated display.
        /// </summary>
        /// <param name="display">Target display</param>
        /// <param name="screen_number">screen on which to act (-1 meaning the current screen)</param>
        /// <param name="x">Target x-coordinate</param>
        /// <param name="y">Target y-coordinate</param>
        /// <param name="delay">Delay in milliseconds prior to performing the action</param>
        /// <returns></returns>
        [DllImport("libXtst.so")]
        public static extern int XTestFakeMotionEvent(IntPtr display, int screen_number, int x, int y, ulong delay);

        /// <summary>
        /// Request that the server simulate a button press/depress at the current pointer location.
        /// </summary>
        /// <param name="display">Target display</param>
        /// <param name="button">Mouse button to simualte pressing</param>
        /// <param name="is_press">true indicates the button is to be pressed, false means released</param>
        /// <param name="delay">delay in milliseconds before performing the action</param>
        /// <returns></returns>
        [DllImport("libXtst.so")]
        public static extern int XTestFakeButtonEvent(IntPtr display, Button button, int is_press, ulong delay);

        /// <summary>
        /// Request that the server simulate keystrokes.
        /// </summary>
        /// <param name="display">Target display</param>
        /// <param name="code">The key code indicating the key to press</param>
        /// <param name="is_press">true = press, false = depress</param>
        /// <param name="delay">delay in milliseconds before performing the action</param>
        /// <returns></returns>
        [DllImport("libXtst.so")]
        public static extern int XTestFakeKeyEvent(IntPtr display, X11.KeyCode code, bool is_press, ulong delay);
    }
}
