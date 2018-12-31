using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using X11;

namespace SimpleWM
{
    public class WindowGroup
    {
        public Window title;
        public Window child;
        public Window frame;
    }

    public class WMCursors
    {
        public Cursor DefaultCursor;
        public Cursor FrameCursor;
        public Cursor TitleCursor;
    }

    public enum MouseMoveType
    {
        TitleDrag,
        TopLeftFrameDrag,
        TopRightFrameDrag,
        BottomLeftFrameDrag,
        BottomRightFrameDrag,
        RightFrameDrag,
        TopFrameDrag,
        LeftFrameDrag,
        BottomFrameDrag,
    }

    public class MouseMovement
    {
        public MouseMoveType Type { get; private set; }
        public int MotionStartX { get; set; } = 0;
        public int MotionStartY { get; set; } = 0;
        public int WindowOriginPointX { get; private set; } = 0;
        public int WindowOriginPointY { get; private set; } = 0;

        public MouseMovement( MouseMoveType type, int Motion_X, int Motion_Y, int Window_X, int Window_Y )
        {
            Type = type;
            MotionStartX = Motion_X;
            MotionStartY = Motion_Y;
            WindowOriginPointX = Window_X;
            WindowOriginPointY = Window_Y;
        }

    }

    public class WindowManager
    {
        private IntPtr display;
        private Window root;
        private WMCursors Cursors = new WMCursors();
        private readonly Dictionary<Window, WindowGroup> WindowIndexByClient = new Dictionary<Window, WindowGroup>();
        private readonly Dictionary<Window, WindowGroup> WindowIndexByFrame = new Dictionary<Window, WindowGroup>();
        private readonly Dictionary<Window, WindowGroup> WindowIndexByTitle = new Dictionary<Window, WindowGroup>();

        private MouseMovement MouseMovement;

        public XErrorHandlerDelegate OnError;

        public int ErrorHandler(IntPtr display, ref XErrorEvent ev)
        {
            var description = Marshal.AllocHGlobal(1024);
            Xlib.XGetErrorText(this.display, ev.error_code, description, 1024);
            var desc = Marshal.PtrToStringAnsi(description);
            Console.WriteLine($"X11 Error: {desc}");
            Marshal.FreeHGlobal(description);
            return 0;
        }

        public WindowManager()
        {
            this.display = Xlib.XOpenDisplay(null);

            if (display == IntPtr.Zero)
            {
                Console.WriteLine("Unable to open the default X display");
                Environment.Exit(1);
            }
            else
            {
                var pDisplayText = Xlib.XDisplayName(null);
                var DisplayText = Marshal.PtrToStringAnsi(pDisplayText);
                Console.WriteLine($"Connecting to {DisplayText}");
            }

            this.root = Xlib.XDefaultRootWindow(display);
            OnError = this.ErrorHandler;

            Xlib.XSetErrorHandler(OnError);
            Xlib.XSelectInput(this.display, this.root,
                EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask);
            Xlib.XSync(this.display, false);

            this.Cursors.DefaultCursor = Xlib.XCreateFontCursor(this.display, FontCursor.XC_left_ptr);
            this.Cursors.TitleCursor = Xlib.XCreateFontCursor(this.display, FontCursor.XC_fleur);
            this.Cursors.FrameCursor = Xlib.XCreateFontCursor(this.display, FontCursor.XC_sizing);

            Xlib.XDefineCursor(this.display, this.root, this.Cursors.DefaultCursor);
            

            // Names: see https://en.wikipedia.org/wiki/X11_color_names
            string BackgroundColor = "black";
            Xlib.XSetWindowBackground(this.display, this.root, GetPixelByName(BackgroundColor));
            Xlib.XClearWindow(this.display, this.root); // force a redraw with the new background color
            Console.WriteLine($"Set background color to {BackgroundColor}");
        }

        public ulong GetPixelByName(string name)
        {
            var screen = Xlib.XDefaultScreen(this.display);
            XColor color = new XColor();
            if (0 == Xlib.XParseColor(this.display, Xlib.XDefaultColormap(this.display, screen), name, ref color))
            {
                Console.WriteLine($"Invalid Color {name}");
            }
       
            if (0 == Xlib.XAllocColor(this.display, Xlib.XDefaultColormap(this.display, screen), ref color))
            {
                Console.WriteLine($"Failed to allocate color {name}");
            }

            return color.pixel;
        }

        public void AddFrame(Window child)
        {
            const int frame_width = 3;
            const int title_height = 20;
            const int inner_border = 1;

            if (this.WindowIndexByClient.ContainsKey(child))
                return; // Window has already been framed.

            var Name = String.Empty;
            Xlib.XFetchName(this.display, child, ref Name);
            Console.WriteLine($"Framing {Name}");

            Xlib.XGetWindowAttributes(this.display, child, out var attr);
            var title = Xlib.XCreateSimpleWindow(this.display, this.root, attr.x, attr.y, attr.width - (2 * inner_border),
                (title_height - 2 * inner_border), inner_border, GetPixelByName("slate grey"), GetPixelByName("light slate grey"));

            // Try to keep the child window in the same place, unless this would push the window decorations off screen.
            var adjusted_x_loc = (attr.x - frame_width < 0) ? 0 : attr.x - frame_width;
            var adjusted_y_loc = (attr.y - (title_height + frame_width) < 0) ? 0 : (attr.y - (title_height + frame_width));

            var frame = Xlib.XCreateSimpleWindow(this.display, this.root, adjusted_x_loc,
                adjusted_y_loc, attr.width, attr.height + title_height,
                3, GetPixelByName("dark slate grey"), GetPixelByName("black"));

            Xlib.XSelectInput(this.display, title, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask 
                | EventMask.Button1MotionMask | EventMask.ExposureMask);
            Xlib.XSelectInput(this.display, frame, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask 
                | EventMask.Button1MotionMask | EventMask.FocusChangeMask | EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask);

            Xlib.XDefineCursor(this.display, title, this.Cursors.TitleCursor);
            Xlib.XDefineCursor(this.display, frame, this.Cursors.FrameCursor);

            Console.WriteLine($"(AddFrame) Created frame {frame} for window {child}");
            Xlib.XReparentWindow(this.display, title, frame, 0, 0);
            Xlib.XReparentWindow(this.display, child, frame, 0, title_height);
            Xlib.XMapWindow(this.display, title);
            Xlib.XMapWindow(this.display, frame);
            // Ensure the child window survives the untimely death of the window manager.
            Xlib.XAddToSaveSet(this.display, child);

            // Grab left click events from the client, so we can focus & raise on click
            SetFocusTrap(child);

            var wg = new WindowGroup { child = child, frame = frame, title = title };
            this.WindowIndexByClient[child] = wg;
            this.WindowIndexByTitle[title] = wg;
            this.WindowIndexByFrame[frame] = wg;
        }

        private void SetFocusTrap(Window child)
        {
            Xlib.XGrabButton(this.display, Button.LEFT, KeyButtonMask.AnyModifier, child, false,
                            EventMask.ButtonPressMask, GrabMode.Async, GrabMode.Async, 0, 0);
        }

        public void RemoveFrame(Window child)
        {

            if (!this.WindowIndexByClient.ContainsKey(child))
            {
                Console.WriteLine($"(RemoveFrame) Not unframing non-client window {child}");
                return; // Do not attempt to unframe a window we have not framed.
            }
            var frame = WindowIndexByClient[child].frame;

            Xlib.XUnmapWindow(this.display, frame);
            Xlib.XDestroyWindow(this.display, frame);

            this.WindowIndexByClient.Remove(child); // Cease tracking the window/frame pair.
        }

        void OnMapRequest(X11.XMapRequestEvent ev)
        {          
            AddFrame(ev.window);
            Xlib.XMapWindow(this.display, ev.window);
        }

        void OnButtonPressEvent(X11.XButtonEvent ev)
        {
            var client = ev.window;
            if (WindowIndexByClient.ContainsKey(ev.window) && ev.button == (uint)Button.LEFT)
            {
                LeftClickClientWindow(ev);
            }

            else if(WindowIndexByTitle.ContainsKey(ev.window) && ev.button == (uint)Button.LEFT)
            {
                LeftClickTitleBar(ev);
                client = WindowIndexByTitle[ev.window].child;
            }

            else if( WindowIndexByFrame.ContainsKey(ev.window) && ev.button == (uint)Button.LEFT)
            {
                LeftClickFrame(ev);
                client = WindowIndexByFrame[ev.window].child;
            }
            FocusAndRaiseWindow(client);
        }

        private void LeftClickTitleBar(XButtonEvent ev)
        {
            Window frame;
            var wg = WindowIndexByTitle[ev.window];

            frame = wg.frame;
            var child = wg.child;
            FocusAndRaiseWindow(child);
            Xlib.XGetWindowAttributes(this.display, frame, out var attr);
            this.MouseMovement = new MouseMovement(MouseMoveType.TitleDrag, ev.x_root, ev.y_root, attr.x, attr.y);
            return;
        }

        private void LeftClickFrame(XButtonEvent ev)
        {
            Xlib.XGetWindowAttributes(this.display, ev.window, out var attr);

            var control_width = (attr.width / 2) <= 40 ? attr.width / 2 : 40;
            var control_height = (attr.height / 2) <= 40 ? attr.width / 2 : 40;

            if( ev.x >= attr.width - control_width) // right side
            {
                if( ev.y >= attr.height - control_height )
                {
                    this.MouseMovement = new MouseMovement(MouseMoveType.BottomRightFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
                }
                else if (ev.y <= control_height)
                {
                    this.MouseMovement = new MouseMovement(MouseMoveType.TopRightFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
                }
                else
                {
                    this.MouseMovement = new MouseMovement(MouseMoveType.RightFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
                }
            }
            else if (ev.x <= control_width)
            {
                if (ev.y >= attr.height - control_height)
                {
                    this.MouseMovement = new MouseMovement(MouseMoveType.BottomLeftFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
                }
                else if (ev.y <= control_height)
                {
                    this.MouseMovement = new MouseMovement(MouseMoveType.TopLeftFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
                }
                else
                {
                    this.MouseMovement = new MouseMovement(MouseMoveType.LeftFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
                }
            }
            else if (ev.y >= attr.height / 2)
            {
                this.MouseMovement = new MouseMovement(MouseMoveType.BottomFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
            }
            else
            {
                this.MouseMovement = new MouseMovement(MouseMoveType.TopFrameDrag, ev.x_root, ev.y_root, attr.x, attr.y);
            }
            return;
        }

        private void UpdateWindowTitle(Window titlebar)
        {
            var client = WindowIndexByTitle[titlebar].child;
            var name = String.Empty;
            Xlib.XFetchName(this.display, client, ref name);
            Xlib.XDrawString(this.display, titlebar, Xlib.XDefaultGC(this.display, Xlib.XDefaultScreen(this.display)), 2, 13,
    Encoding.ASCII.GetBytes(name), name.Length);
        }

        private void  LeftClickClientWindow(XButtonEvent ev)
        {
            Window frame = WindowIndexByClient[ev.window].frame;
            // Release control of the left button to this application
            UnsetFocusTrap(ev.window);
            // Raise and focus it
            FocusAndRaiseWindow(ev.window);
            return;
        }

        private void FocusAndRaiseWindow(Window focus)
        {
            if (WindowIndexByClient.ContainsKey(focus))
            {
                var frame = WindowIndexByClient[focus].frame;
                Xlib.XSetInputFocus(this.display, focus, RevertFocus.RevertToNone, 0);
                Xlib.XRaiseWindow(this.display, frame);
            }
        }

        private void UnsetFocusTrap(Window w)
        {
            Xlib.XUngrabButton(this.display, Button.LEFT, KeyButtonMask.AnyModifier, w);
        }

        void OnMotionEvent(X11.XMotionEvent ev)
        {
            if (WindowIndexByTitle.ContainsKey(ev.window))
            {
                LeftDragTitle(ev);
                return;
            }
            if( WindowIndexByFrame.ContainsKey(ev.window) )
            {
                LeftDragFrame(ev);
                return;
            }
        }

        private void LeftDragTitle(XMotionEvent ev)
        {
            // Move the window, after converting co-ordinates into offsets relative to the origin point of motion
            var new_y = ev.y_root - this.MouseMovement.MotionStartY;
            var new_x = ev.x_root - this.MouseMovement.MotionStartX;
            Xlib.XMoveWindow(this.display, WindowIndexByTitle[ev.window].frame, 
                this.MouseMovement.WindowOriginPointX + new_x, this.MouseMovement.WindowOriginPointY + new_y);
        }

        private void LeftDragFrame(XMotionEvent ev)
        {
            var frame = ev.window;
            var title = WindowIndexByFrame[frame].title;
            var client = WindowIndexByFrame[frame].child;

            var y_delta = 0;
            var x_delta = 0;

            var w_delta = 0;
            var h_delta = 0;

            var t = this.MouseMovement.Type;

            // Stretch to the right, or compress left, no lateral relocation of window origin.
            if (t == MouseMoveType.RightFrameDrag
                || t == MouseMoveType.TopRightFrameDrag
                || t == MouseMoveType.BottomRightFrameDrag)
            {
                w_delta = ev.x_root - this.MouseMovement.MotionStartX; // width change
            }
            // Stretch down, or compress upwards, no vertical movement of the window origin.
            if (t == MouseMoveType.BottomFrameDrag
                || t == MouseMoveType.BottomRightFrameDrag
                || t == MouseMoveType.BottomLeftFrameDrag)
            {
                h_delta = ev.y_root - this.MouseMovement.MotionStartY;
            }
            // Combine vertical stretch with movement of the window origin.
            if (t == MouseMoveType.TopFrameDrag
                || t == MouseMoveType.TopRightFrameDrag
                || t == MouseMoveType.TopLeftFrameDrag)
            {
                h_delta = this.MouseMovement.MotionStartY - ev.y_root;
                y_delta = -h_delta;
            }
            // Combined left stretch with movement of the window origin
            if (t == MouseMoveType.LeftFrameDrag
                || t == MouseMoveType.TopLeftFrameDrag
                || t == MouseMoveType.BottomLeftFrameDrag)
            {
                w_delta = this.MouseMovement.MotionStartX - ev.x_root;
                x_delta = - w_delta;
            }

            //// Resize and move the frame
            Xlib.XGetWindowAttributes(this.display, frame, out var attr);
            var new_width = (uint)(attr.width + w_delta);
            var new_height = (uint)(attr.height + h_delta);
            Xlib.XMoveResizeWindow(this.display, frame, attr.x + x_delta, attr.y + y_delta, new_width, new_height);

            //// Resize and move the title bar
            Xlib.XGetWindowAttributes(this.display, title, out attr);
            new_width = (uint)(attr.width + w_delta);
            new_height = (uint)attr.height;
            Xlib.XResizeWindow(this.display, title, new_width, new_height);

            //// Resize and move the client window bar
            Xlib.XGetWindowAttributes(this.display, client, out attr);
            new_width = (uint)(attr.width + w_delta);
            new_height = (uint)(attr.height + h_delta);
            Xlib.XResizeWindow(this.display, client, new_width, new_height);

            this.MouseMovement.MotionStartX = ev.x_root;
            this.MouseMovement.MotionStartY = ev.y_root;
        }

        void OnMapNotify(X11.XMapEvent ev)
        {
            Console.WriteLine($"(MapNotifyEvent) Window {ev.window} has been mapped.");
        }

        void OnConfigureRequest(X11.XConfigureRequestEvent ev)
        {
            var changes = new X11.XWindowChanges
            {
                x = ev.x,
                y = ev.y,
                width = ev.width,
                height = ev.height,
                border_width = ev.border_width,
                sibling = ev.above,
                stack_mode = ev.detail
            };

            if( this.WindowIndexByClient.ContainsKey(ev.window))
            {
                // Resize the frame
                Xlib.XConfigureWindow(this.display, this.WindowIndexByClient[ev.window].frame, ev.value_mask, ref changes);
            }
            // Resize the window
            Xlib.XConfigureWindow(this.display, ev.window, ev.value_mask, ref changes);
        }

        void OnUnmapNotify(X11.XUnmapEvent ev)
        {
            if(ev.@event == this.root)
            {
                Console.WriteLine($"(OnUnmapNotify) Window {ev.window} has been reparented to root");
                return;
            }
            if (!this.WindowIndexByClient.ContainsKey(ev.window))
                return; // Don't unmap a window we don't own.

            RemoveFrame(ev.window);
        }

        void OnFocusOutEvent(X11.XFocusChangeEvent ev)
        {
            var title = WindowIndexByFrame[ev.window].title;
            var frame = ev.window;
            Xlib.XSetWindowBorder(this.display, frame, GetPixelByName("dark slate grey"));
            Xlib.XSetWindowBackground(this.display, title, GetPixelByName("slate grey") );
            Xlib.XSetWindowBorder(this.display, title, GetPixelByName("light slate grey"));
            Xlib.XClearWindow(this.display, title);
            UpdateWindowTitle(title);

            SetFocusTrap(WindowIndexByFrame[ev.window].child);
        }

        void OnFocusInEvent(X11.XFocusChangeEvent ev)
        {
            var title = WindowIndexByFrame[ev.window].title;
            var frame = ev.window;
            Xlib.XSetWindowBorder(this.display, frame, GetPixelByName("dark goldenrod"));
            Xlib.XSetWindowBackground(this.display, title, GetPixelByName("yellow"));
            Xlib.XSetWindowBorder(this.display, title, GetPixelByName("gold"));
            Xlib.XClearWindow(this.display, title); // Force colour update

            UpdateWindowTitle(title); //Redraw the title, purged by clearing.
        }

        void OnDestroyNotify(X11.XDestroyWindowEvent ev)
        {
            Console.WriteLine($"(OnDestroyNotify) Destroyed {ev.window}");
        }

        void OnReparentNotify(X11.XReparentEvent ev)
        {
            return; // Never seems to be interesting and is often duplicated.
        }

        void OnCreateNotify(X11.XCreateWindowEvent ev)
        {
            Console.WriteLine($"(OnCreateNotify) Created event {ev.window}, parent {ev.parent}");
        }

        public int Run()
        {
            IntPtr ev = Marshal.AllocHGlobal( 24* sizeof(long) );

            Window ReturnedParent = 0, ReturnedRoot = 0;
            

            Xlib.XGrabServer(this.display); // Lock the server during initialization
            var r = Xlib.XQueryTree(this.display, this.root, ref ReturnedRoot, ref ReturnedParent,
                out var ChildWindows);

            Console.WriteLine($"Reparenting and framing pre-existing child windows: {ChildWindows.Count}");
            for (var i = 0; i < ChildWindows.Count; i++)
            {
                Console.WriteLine($"Framing child {i}, {ChildWindows[i]}");
                AddFrame(ChildWindows[i]);
            }
            Xlib.XUngrabServer(this.display); // Release the lock on the server.

            while ( true )
            {

                var rv = Xlib.XNextEvent(this.display, ev);
                var xevent = Marshal.PtrToStructure<X11.XAnyEvent>(ev);

                switch (xevent.type)
                {
                    case (int)Event.DestroyNotify:
                        var destroy_event = Marshal.PtrToStructure<X11.XDestroyWindowEvent>(ev);
                        OnDestroyNotify(destroy_event);
                        break;
                    case (int)Event.CreateNotify:
                        var create_event = Marshal.PtrToStructure<X11.XCreateWindowEvent>(ev);
                        OnCreateNotify(create_event);
                        break;
                    case (int)Event.MapNotify:
                        var map_notify = Marshal.PtrToStructure<X11.XMapEvent>(ev);
                        OnMapNotify(map_notify);
                        break;
                    case (int)Event.MapRequest:
                        var map_event = Marshal.PtrToStructure<X11.XMapRequestEvent>(ev);
                        OnMapRequest(map_event);
                        break;
                    case (int)Event.ConfigureRequest:
                        var cfg_event = Marshal.PtrToStructure<X11.XConfigureRequestEvent>(ev);
                        OnConfigureRequest(cfg_event);
                        break;
                    case (int)Event.UnmapNotify:
                        var unmap_event = Marshal.PtrToStructure<X11.XUnmapEvent>(ev);
                        OnUnmapNotify(unmap_event);
                        break;
                    case (int)Event.ReparentNotify:
                        var reparent_event = Marshal.PtrToStructure<X11.XReparentEvent>(ev);
                        OnReparentNotify(reparent_event);
                        break;
                    case (int)Event.ButtonPress:
                        var button_press_event = Marshal.PtrToStructure<X11.XButtonEvent>(ev);
                        OnButtonPressEvent(button_press_event);
                        break;
                    case (int)Event.MotionNotify:
                        // We only want the newest motion event in order to reduce perceived lag
                        while (Xlib.XCheckMaskEvent(this.display, EventMask.Button1MotionMask, ev)) { }
                        var motion_event = Marshal.PtrToStructure<X11.XMotionEvent>(ev);
                        OnMotionEvent(motion_event);
                        break;
                    case (int)Event.FocusOut:
                        var focus_out_event = Marshal.PtrToStructure<X11.XFocusChangeEvent>(ev);
                        OnFocusOutEvent(focus_out_event);
                        break;
                    case (int)Event.FocusIn:
                        var focus_in_event = Marshal.PtrToStructure<X11.XFocusChangeEvent>(ev);
                        OnFocusInEvent(focus_in_event);
                        break;
                    case (int)Event.ConfigureNotify:
                        break;
                    case (int)Event.Expose:
                        var expose_event = Marshal.PtrToStructure<X11.XExposeEvent>(ev);
                        UpdateWindowTitle(expose_event.window);
                        break;
                    default:
                        Console.WriteLine($"Event type: { Enum.GetName(typeof(Event), xevent.type)}");
                        break;
                }
            }
            Marshal.FreeHGlobal(ev);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var WM = new WindowManager();
            WM.Run();
        }
    }
}
