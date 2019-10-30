using System;
using System.Runtime.InteropServices;

namespace X11
{

    public enum Window: ulong
    {
        None=0,
    }

    public enum KeyCode: byte
    {
    }

    public enum KeySym : long
    {
        NoSymbol = 0,
        XK_r = 0x0072,
    }

    public enum Button : uint
    {
        LEFT = 1,
        MIDDLE = 2,
        RIGHT = 3,
        FOUR = 4,
        FIVE = 5,
    }

    public enum GrabMode : int
    {
        Sync = 0,
        Async = 1,
    }

    public enum KeyButtonMask: uint
    {
        ShiftMask = (1<<0),
        LockMask = (1<<1),
        ControlMask = (1<<2),
        Mod1Mask = (1<<3),
        Mod2Mask = (1<<4),
        Mod3Mask = (1<<5),
        Mod4Mask = (1<<6),
        Mod5Mask = (1<<7),
        Button1Mask = (1<<8),
        Button2Mask = (1<<9),
        Button3Mask = (1<<10),
        Button4Mask = (1<<11),
        Button5Mask = (1<<12),
        AnyModifier = (1<<15),
    }


    public partial class Xlib
    {
        /// <summary>
        ///        The XGrabButton function establishes a passive grab.  In the future, the pointer is actively grabbed (as for
        ///XGrabPointer), the last-pointer-grab time is set to the time at which the button was pressed(as transmitted
        /// in the ButtonPress event), and the ButtonPress event is reported if all of the following conditions are true:
        /// (1) The pointer is not grabbed, and the specified button is logically pressed when the specified modifier
        ///       keys are logically down, and no other buttons or modifier keys are logically down.
        /// (2) The grab_window contains the pointer.
        /// (3)  The confine_to window (if any) is viewable.
        /// (4)  A passive grab on the same button/key combination does not exist on any ancestor of grab_window.
        /// The interpretation of the remaining arguments is as for XGrabPointer.
        /// This request overrides all previous grabs by the same client on the same button/key combinations on the same
        /// window.  A modifiers of AnyModifier is equivalent to issuing the grab request for all possible modifier combi‐
        /// nations (including the combination of no modifiers). 
        /// </summary>
        /// 
        /// <param name="display">Connected display where pointer is located</param>
        /// <param name="button">Button that is to be grabbed</param>
        /// <param name="modifiers">Set of keymasks in addition to the button</param>
        /// <param name="grab_window">Specify the grab window</param>
        /// <param name="owner_events">Should pointer events be reported as usual or with respect to the grab window</param>
        /// <param name="event_mask">Which pointer events should be reported to the client</param>
        /// <param name="pointer_mode">Specify processing of further events</param>
        /// <param name="keyboard_mode">Specify processing of further keyboard events</param>
        /// <param name="confine_to">Window to confine pointer to or None (0)</param>
        /// <param name="cursor">Specify the cursor to display or None (0)</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XGrabButton(IntPtr display, Button button, KeyButtonMask modifiers, Window grab_window,
        bool owner_events, EventMask event_mask, GrabMode pointer_mode, GrabMode keyboard_mode, Window confine_to, FontCursor cursor);

        [DllImport("libX11.so.6")]
        public static extern Status XGrabPointer(IntPtr display, Window grab_window, bool owner_events, EventMask event_mask, 
            GrabMode pointer_mode, GrabMode keyboard_mode, Window confine_to, Cursor cursor, ulong time);

        [DllImport("libX11.so.6")]
        public static extern Status XUngrabPointer(IntPtr display, ulong time);

        [DllImport("libX11.so.6")]
        public static extern Status XGrabKey(IntPtr display, KeyCode keycode, KeyButtonMask modifiers, Window grab_window, 
            bool owner_events, GrabMode pointer_mode, GrabMode keyboard_mode);

        [DllImport("libX11.so.6")]
        public static extern Status XUngrabKey(IntPtr display, KeyCode keycode, KeyButtonMask modifiers, Window grab_window);

        [DllImport("libX11.so.6")]
        public static extern Status XChangeActivePointerGrab(IntPtr display, EventMask event_mask, Cursor cursor, ulong time);

        [DllImport("libX11.so.6")]
        public static extern KeySym XStringToKeysym(string str);

        [DllImport("libX11.so.6")]
        public static extern string XKeysymToString(KeySym keysym);

        [DllImport("libX11.so.6")]
        public static extern KeySym XKeycodeToKeysym(IntPtr display, KeyCode keycode, int index);

        [DllImport("libX11.so.6")]
        public static extern KeyCode XKeysymToKeycode(IntPtr display, KeySym keysym);

        /// <summary>
        ///  The XUngrabButton function releases the passive button/key combination on the specified window if it was
        ///  grabbed by this client.A modifiers of AnyModifier is equivalent to issuing the ungrab request for all possi‐
        ///  ble modifier combinations, including the combination of no modifiers.A button of AnyButton is equivalent to
        ///  issuing the request for all possible buttons.XUngrabButton has no effect on an active grab.
        /// </summary>
        /// <param name="display">Connected display to act upon</param>
        /// <param name="button">Button to release</param>
        /// <param name="modifiers">Additional keyboard modifiers</param>
        /// <param name="grab_button">Specify the grab window.</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern Status XUngrabButton(IntPtr display, Button button, KeyButtonMask modifiers, Window grab_window);

        [DllImport("libX11.so.6")]
        public static extern IntPtr XGetKeyboardMapping(IntPtr display, X11.KeyCode keyCode, int keycode_count, IntPtr keysyms_per_keycode_return);

        [DllImport("libX11.so.6")]
        public static extern int XChangeKeyboardMapping(IntPtr display, X11.KeyCode keyCode, int keysyms_per_keycode, IntPtr keysys, int num_codes);

        [DllImport("libX11.so.6")]
        public static extern void XDisplayKeycodes(IntPtr display, IntPtr min, IntPtr max);
    }


}
