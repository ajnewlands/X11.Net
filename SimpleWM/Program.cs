using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using X11;

namespace SimpleWM
{
    public class SimpleLogger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
        }

        public LogLevel Level;

        public SimpleLogger(LogLevel level)
        {
            this.Level = level;
        }

        private void Write(string message, LogLevel message_level)
        {
            if (Level <= message_level)
                Console.WriteLine($"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt")} {message_level} {message}");

        }

        public void Debug(string message)
        {
            Write(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            Write(message, LogLevel.Info);
        }

        public void Warn(string message)
        {
            Write(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            Write(message, LogLevel.Error);
        }
    }

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

    public class WMColours
    {
        public ulong ActiveFrameColor;
        public ulong ActiveTitleColor;
        public ulong ActiveTitleBorder;
        public ulong InactiveFrameColor;
        public ulong InactiveTitleColor;
        public ulong InactiveTitleBorder;
        public ulong DesktopBackground;
        public ulong WindowBackground;
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
        private SimpleLogger Log;
        private IntPtr display;
        private Window root;
        private WMCursors Cursors = new WMCursors();
        private WMColours Colours = new WMColours();
        private readonly Dictionary<Window, WindowGroup> WindowIndexByClient = new Dictionary<Window, WindowGroup>();
        private readonly Dictionary<Window, WindowGroup> WindowIndexByFrame = new Dictionary<Window, WindowGroup>();
        private readonly Dictionary<Window, WindowGroup> WindowIndexByTitle = new Dictionary<Window, WindowGroup>();
        private MouseMovement MouseMovement;

        public XErrorHandlerDelegate OnError;

        public int ErrorHandler(IntPtr display, ref XErrorEvent ev)
        {
            if (ev.error_code == 10) // BadAccess, i.e. another window manager has already claimed those privileges.
            {
                Log.Error("X11 denied access to window manager resources - another window manager is already running");
                Environment.Exit(1);
            }

            // Other runtime errors and warnings.
            var description = Marshal.AllocHGlobal(1024);
            Xlib.XGetErrorText(this.display, ev.error_code, description, 1024);
            var desc = Marshal.PtrToStringAnsi(description);
            Log.Warn($"X11 Error: {desc}");
            Marshal.FreeHGlobal(description);
            return 0;
        }

        public WindowManager( SimpleLogger.LogLevel level )
        {
            this.Log = new SimpleLogger( level );
            var pDisplayText = Xlib.XDisplayName(null);
            var DisplayText = Marshal.PtrToStringAnsi(pDisplayText);
            if (DisplayText == String.Empty)
            {
                Log.Error("No display configured for X11; check the value of the DISPLAY variable is set correctly");
                Environment.Exit(1);
            }

            Log.Info($"Connecting to X11 Display {DisplayText}");
            this.display = Xlib.XOpenDisplay(null);

            if (display == IntPtr.Zero)
            {
                Log.Error("Unable to open the default X display");
                Environment.Exit(1);
            }

            this.root = Xlib.XDefaultRootWindow(display);
            OnError = this.ErrorHandler;

            Xlib.XSetErrorHandler(OnError);
            // This will trigger a bad access error if another window manager is already running
            Xlib.XSelectInput(this.display, this.root,
                EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask |
                EventMask.ButtonPressMask | EventMask.KeyPressMask);

            Xlib.XSync(this.display, false);
            
            // Setup cursors
            this.Cursors.DefaultCursor = Xlib.XCreateFontCursor(this.display, FontCursor.XC_left_ptr);
            this.Cursors.TitleCursor = Xlib.XCreateFontCursor(this.display, FontCursor.XC_fleur);
            this.Cursors.FrameCursor = Xlib.XCreateFontCursor(this.display, FontCursor.XC_sizing);
            Xlib.XDefineCursor(this.display, this.root, this.Cursors.DefaultCursor);

            // Setup colours
            this.Colours.DesktopBackground = GetPixelByName("black");
            this.Colours.WindowBackground = GetPixelByName("white");
            this.Colours.InactiveTitleBorder = GetPixelByName("light slate grey");
            this.Colours.InactiveTitleColor = GetPixelByName("slate grey");
            this.Colours.InactiveFrameColor = GetPixelByName("dark slate grey");
            this.Colours.ActiveFrameColor = GetPixelByName("dark goldenrod");
            this.Colours.ActiveTitleColor = GetPixelByName("gold");
            this.Colours.ActiveTitleBorder = GetPixelByName("saddle brown");
            
            Xlib.XSetWindowBackground(this.display, this.root, this.Colours.DesktopBackground);
            Xlib.XClearWindow(this.display, this.root); // force a redraw with the new background color
        }

        private ulong GetPixelByName(string name)
        {
            var screen = Xlib.XDefaultScreen(this.display);
            XColor color = new XColor();
            if (0 == Xlib.XParseColor(this.display, Xlib.XDefaultColormap(this.display, screen), name, ref color))
            {
                Log.Error($"Invalid Color {name}");
            }
       
            if (0 == Xlib.XAllocColor(this.display, Xlib.XDefaultColormap(this.display, screen), ref color))
            {
                Log.Error($"Failed to allocate color {name}");
            }

            return color.pixel;
        }

        private void AddFrame(Window child)
        {
            const int frame_width = 3;
            const int title_height = 20;
            const int inner_border = 1;

            if (this.WindowIndexByClient.ContainsKey(child))
                return; // Window has already been framed.

            var Name = String.Empty;
            Xlib.XFetchName(this.display, child, ref Name);
            Log.Debug($"Framing {Name}");

            Xlib.XGetWindowAttributes(this.display, child, out var attr);
            var title = Xlib.XCreateSimpleWindow(this.display, this.root, attr.x, attr.y, attr.width - (2 * inner_border),
                (title_height - 2 * inner_border), inner_border,this.Colours.InactiveTitleColor, this.Colours.InactiveTitleBorder);

            // Try to keep the child window in the same place, unless this would push the window decorations off screen.
            var adjusted_x_loc = (attr.x - frame_width < 0) ? 0 : attr.x - frame_width;
            var adjusted_y_loc = (attr.y - (title_height + frame_width) < 0) ? 0 : (attr.y - (title_height + frame_width));

            var frame = Xlib.XCreateSimpleWindow(this.display, this.root, adjusted_x_loc,
                adjusted_y_loc, attr.width, attr.height + title_height,
                3, this.Colours.InactiveFrameColor, this.Colours.WindowBackground);

            Xlib.XSelectInput(this.display, title, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask 
                | EventMask.Button1MotionMask | EventMask.ExposureMask);
            Xlib.XSelectInput(this.display, frame, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask 
                | EventMask.Button1MotionMask | EventMask.FocusChangeMask | EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask);

            Xlib.XDefineCursor(this.display, title, this.Cursors.TitleCursor);
            Xlib.XDefineCursor(this.display, frame, this.Cursors.FrameCursor);

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

        private void RemoveFrame(Window child)
        {

            if (!this.WindowIndexByClient.ContainsKey(child))
            {
                return; // Do not attempt to unframe a window we have not framed.
            }
            var frame = WindowIndexByClient[child].frame;

            Xlib.XUnmapWindow(this.display, frame);
            Xlib.XDestroyWindow(this.display, frame);

            this.WindowIndexByClient.Remove(child); // Cease tracking the window/frame pair.
        }

        private void SetFocusTrap(Window child)
        {
            Xlib.XGrabButton(this.display, Button.LEFT, KeyButtonMask.AnyModifier, child, false,
                            EventMask.ButtonPressMask, GrabMode.Async, GrabMode.Async, 0, 0);
        }

        private void UnsetFocusTrap(Window w)
        {
            Xlib.XUngrabButton(this.display, Button.LEFT, KeyButtonMask.AnyModifier, w);
        }


        private void OnMapRequest(X11.XMapRequestEvent ev)
        {          
            AddFrame(ev.window);
            Xlib.XMapWindow(this.display, ev.window);
        }

        private void OnButtonPressEvent(X11.XButtonEvent ev)
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

        private void OnExposeEvent(X11.XExposeEvent ev)
        {
            if (this.WindowIndexByTitle.ContainsKey(ev.window))
            {
                UpdateWindowTitle(ev.window);
            }
        }

        private void UpdateWindowTitle(Window titlebar)
        {
            var client = WindowIndexByTitle[titlebar].child;
            var name = String.Empty;
            if(Xlib.XFetchName(this.display, client, ref name) != Status.Failure)
                Xlib.XDrawString(this.display, titlebar, Xlib.XDefaultGC(this.display, Xlib.XDefaultScreen(this.display)), 2, 13,
                    name, name.Length);
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
            if (this.MouseMovement == null)
                return;

            // If we hit the screen edges, snap to edge
            Xlib.XGetWindowAttributes(this.display, this.root, out var attr);
            if (ev.y_root == attr.height - 1 // Snap to bottom
                ||ev.y_root == 0 // snap to top
                ||ev.x_root == attr.width - 1 // snap to right
                || ev.x_root == 0)  // snap left
            {
                var frame = this.WindowIndexByTitle[ev.window].frame;
                SnapFrameToEdge(frame, ev.x_root, ev.y_root, attr.width, attr.height);
                return;
            }

            // Move the window, after converting co-ordinates into offsets relative to the origin point of motion
            var new_y = ev.y_root - this.MouseMovement.MotionStartY;
            var new_x = ev.x_root - this.MouseMovement.MotionStartX;
            Xlib.XMoveWindow(this.display, WindowIndexByTitle[ev.window].frame, 
                this.MouseMovement.WindowOriginPointX + new_x, this.MouseMovement.WindowOriginPointY + new_y);
        }

        private void SnapFrameToEdge(Window frame, int x, int y, uint w, uint h )
        {
            var title = this.WindowIndexByFrame[frame].title;
            var client = this.WindowIndexByFrame[frame].child;

            Xlib.XGetWindowAttributes(this.display, title, out var t_attr);
            var t_h = t_attr.height;
            Xlib.XGetWindowAttributes(this.display, frame, out var f_attr);
            var border_w = (uint)f_attr.border_width;
            int f_y = 0, f_x = 0;

            if (x ==0 || x == w-1)
            { // Vertical half screen sized window
                if (x == w - 1)
                    f_x = (int)w / 2;

                Xlib.XMoveResizeWindow(this.display, frame, f_x, 0, w / 2, h - (2*border_w) );
                Xlib.XMoveResizeWindow(this.display, title, 0, 0, w / 2, t_h);
                Xlib.XMoveResizeWindow(this.display, client, 0, (int)t_h, w / 2, (h - t_h) - 2*border_w);
            }
            else
            { // Horizontal half screen sized window
                if (y == h - 1)
                    f_y = (int)h / 2;

                Xlib.XMoveResizeWindow(this.display, frame, 0, f_y, w, h/2 - (2* border_w));
                Xlib.XMoveResizeWindow(this.display, title, 0, 0, w, t_h);
                Xlib.XMoveResizeWindow(this.display, client, 0, (int)t_h, w, (h/2) - t_h - 2*border_w);
            }
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
            Log.Debug($"(MapNotifyEvent) Window {ev.window} has been mapped.");
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
                Log.Debug($"(OnUnmapNotify) Window {ev.window} has been reparented to root");
                return;
            }
            if (!this.WindowIndexByClient.ContainsKey(ev.window))
                return; // Don't unmap a window we don't own.

            RemoveFrame(ev.window);
        }

        // Annoyingly, this event fires when an application quits itself, resuling in some bad window errors.
        void OnFocusOutEvent(X11.XFocusChangeEvent ev)
        {
            var title = WindowIndexByFrame[ev.window].title;
            var frame = ev.window;
            if (Status.Failure == Xlib.XSetWindowBorder(this.display, frame, this.Colours.InactiveTitleBorder))
                return; // If the windows have been destroyed asynchronously, cut this short.
            Xlib.XSetWindowBackground(this.display, title, this.Colours.InactiveTitleColor );
            Xlib.XSetWindowBorder(this.display, title, this.Colours.InactiveFrameColor);
            Xlib.XClearWindow(this.display, title);
            UpdateWindowTitle(title);

            SetFocusTrap(WindowIndexByFrame[ev.window].child);
        }

        void OnFocusInEvent(X11.XFocusChangeEvent ev)
        {
            var title = WindowIndexByFrame[ev.window].title;
            var frame = ev.window;
            Xlib.XSetWindowBorder(this.display, frame, this.Colours.ActiveFrameColor);
            Xlib.XSetWindowBackground(this.display, title, this.Colours.ActiveTitleColor);
            Xlib.XSetWindowBorder(this.display, title, this.Colours.ActiveTitleBorder);
            Xlib.XClearWindow(this.display, title); // Force colour update

            UpdateWindowTitle(title); //Redraw the title, purged by clearing.
        }

        void OnDestroyNotify(X11.XDestroyWindowEvent ev)
        {
            if (WindowIndexByClient.ContainsKey(ev.window))
                WindowIndexByClient.Remove(ev.window);
            else if (WindowIndexByFrame.ContainsKey(ev.window))
                WindowIndexByFrame.Remove(ev.window);
            else if (WindowIndexByTitle.ContainsKey(ev.window))
                WindowIndexByTitle.Remove(ev.window);
            Log.Debug($"(OnDestroyNotify) Destroyed {ev.window}");
        }

        void OnReparentNotify(X11.XReparentEvent ev)
        {
            return; // Never seems to be interesting and is often duplicated.
        }

        void OnCreateNotify(X11.XCreateWindowEvent ev)
        {
            Log.Debug($"(OnCreateNotify) Created event {ev.window}, parent {ev.parent}");
        }

        public int Run()
        {
            IntPtr ev = Marshal.AllocHGlobal( 24* sizeof(long) );

            Window ReturnedParent = 0, ReturnedRoot = 0;
            

            Xlib.XGrabServer(this.display); // Lock the server during initialization
            var r = Xlib.XQueryTree(this.display, this.root, ref ReturnedRoot, ref ReturnedParent,
                out var ChildWindows);

            Log.Debug($"Reparenting and framing pre-existing child windows: {ChildWindows.Count}");
            for (var i = 0; i < ChildWindows.Count; i++)
            {
                Log.Debug($"Framing child {i}, {ChildWindows[i]}");
                AddFrame(ChildWindows[i]);
            }
            Xlib.XUngrabServer(this.display); // Release the lock on the server.


            while ( true )
            {
                Xlib.XNextEvent(this.display, ev);
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
                    case (int)Event.ButtonRelease:
                        this.MouseMovement = null;
                        break;
                    case (int)Event.MotionNotify:
                        // We only want the newest motion event in order to reduce perceived lag
                        while (Xlib.XCheckMaskEvent(this.display, EventMask.Button1MotionMask, ev)) { /* skip over */ }
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
                        OnExposeEvent(expose_event);
                        break;
                    default:
                        this.Log.Debug($"Event type: { Enum.GetName(typeof(Event), xevent.type)}");
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
            var WM = new WindowManager( SimpleLogger.LogLevel.Info );
            WM.Run();
        }
    }
}
