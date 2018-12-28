using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using X11;

namespace SimpleWM
{
    public class WindowManager
    {
        private IntPtr display;
        private Window root;
        private readonly Dictionary<Window, Window> ClientWindows = new Dictionary<Window, Window>();
        private readonly Dictionary<Window, Window> TitleToFrameMap = new Dictionary<Window, Window>();
        private int MotionStartX = 0;
        private int MotionStartY = 0;
        private int WindowOriginPointX = 0;
        private int WindowOriginPointY = 0;


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

            Xlib.XDefineCursor(this.display, this.root, 
                Xlib.XCreateFontCursor(this.display, FontCursor.XC_left_ptr));


            // Names: see https://en.wikipedia.org/wiki/X11_color_names
            string BackgroundColor = "black";
            Xlib.XSetWindowBackground(this.display, this.root, GetPixelByName(BackgroundColor));
            Xlib.XClearWindow(this.display, this.root); // force a redraw with the new background color
            Console.WriteLine($"Set background color to {BackgroundColor}");
        }

        public ulong GetPixelByName(string name)
        {
            XColor color = new XColor();
            if (0 == Xlib.XParseColor(this.display, Xlib.XDefaultColormap(this.display, 0), name, ref color))
            {
                Console.WriteLine($"Invalid Color {name}");
            }
       
            if (0 == Xlib.XAllocColor(this.display, Xlib.XDefaultColormap(this.display, 0), ref color))
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

            if (this.ClientWindows.ContainsKey(child))
                return; // Window has already been framed.

            Xlib.XGetWindowAttributes(this.display, child, out var attr);
            var title = Xlib.XCreateSimpleWindow(this.display, this.root, attr.x, attr.y, attr.width - (2 * inner_border),
                (title_height - 2 * inner_border), inner_border, GetPixelByName("gold"), GetPixelByName("yellow"));

            // Try to keep the child window in the same place, unless this would push the window decorations off screen.
            var adjusted_x_loc = (attr.x - frame_width < 0) ? 0 : attr.x - frame_width;
            var adjusted_y_loc = (attr.y - (title_height + frame_width) < 0) ? 0 : (attr.y - (title_height + frame_width));

            var frame = Xlib.XCreateSimpleWindow(this.display, this.root, adjusted_x_loc,
                adjusted_y_loc, attr.width, attr.height + title_height,
                3, GetPixelByName("dark goldenrod"), GetPixelByName("black"));


            Xlib.XSelectInput(this.display, title, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.Button1MotionMask);
            Xlib.XSelectInput(this.display, frame, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask);
            // TODO - ideally the cursor would be some sort of singleton type
            Xlib.XDefineCursor(this.display, title, Xlib.XCreateFontCursor(this.display, FontCursor.XC_fleur ));
            Xlib.XDefineCursor(this.display, frame, Xlib.XCreateFontCursor(this.display, FontCursor.XC_sizing ));

            Console.WriteLine($"(AddFrame) Created frame {frame} for window {child}");
            Xlib.XReparentWindow(this.display, title, frame, 0, 0);
            Xlib.XReparentWindow(this.display, child, frame, 0, title_height);
            Xlib.XMapWindow(this.display, title);
            Xlib.XMapWindow(this.display, frame);
            // Ensure the child window survives the untimely death of the window manager.
            Xlib.XAddToSaveSet(this.display, child);

            this.ClientWindows[child] = frame;// Track the new window and its frame.
            this.TitleToFrameMap[title] = frame;
        }

        public void RemoveFrame(Window child)
        {

            if (!this.ClientWindows.ContainsKey(child))
            {
                Console.WriteLine($"(RemoveFrame) Not unframing non-client window {child}");
                return; // Do not attempt to unframe a window we have not framed.
            }
            var frame = ClientWindows[child];

            Xlib.XUnmapWindow(this.display, frame);
            Xlib.XDestroyWindow(this.display, frame);

            this.ClientWindows.Remove(child); // Cease tracking the window/frame pair.
        }

        void OnMapRequest(X11.XMapRequestEvent ev)
        {          
            AddFrame(ev.window);
            Xlib.XMapWindow(this.display, ev.window);
        }

        void OnButtonPressEvent(X11.XButtonEvent ev)
        {
            Console.WriteLine($"Pressed {ev.button}, window {ev.window}, X {ev.x_root}, Y {ev.y_root}");
            this.MotionStartX = ev.x_root;
            this.MotionStartY = ev.y_root;
            Xlib.XGetWindowAttributes(this.display, this.TitleToFrameMap[ev.window], out var attr);
            this.WindowOriginPointX = attr.x;
            this.WindowOriginPointY = attr.y;
        }

        void OnMotionEvent(X11.XMotionEvent ev)
        {
            // Move the window, after converting co-ordinates into offsets relative to the origin point of motion
            var new_y = ev.y_root - this.MotionStartY;
            var new_x = ev.x_root - this.MotionStartX;
            //Xlib.XGetWindowAttributes(this.display, this.TitleToFrameMap[ev.window], out var attr);
            //this.MotionStartX = ev.x_root;
            //this.MotionStartY = ev.y_root;
            //Xlib.XMoveWindow(this.display, this.TitleToFrameMap[ev.window], attr.x + new_x, attr.y + new_y);
            Xlib.XMoveWindow(this.display, this.TitleToFrameMap[ev.window], this.WindowOriginPointX + new_x, this.WindowOriginPointY + new_y);
        }

        void OnMapNotify(X11.XMapNotifyEvent ev)
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

            if( this.ClientWindows.ContainsKey(ev.window))
            {
                // Resize the frame
                Xlib.XConfigureWindow(this.display, this.ClientWindows[ev.window], ev.value_mask, ref changes);
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
            if (!this.ClientWindows.ContainsKey(ev.window))
                return; // Don't unmap a window we don't own.

            RemoveFrame(ev.window);
        }

        void OnDestroyNotify(X11.XDestroyWindowEvent ev)
        {
            Console.WriteLine($"(OnDestroyNotify) Destroyed {ev.window}");
        }

        void OnReparentNotify(X11.XReparentNotifyEvent ev)
        {
            return; // Never seems to be interesting and is often duplicated.
        }

        void OnCreateNotify(X11.XCreateNotifyEvent ev)
        {
            Console.WriteLine($"(OnCreateNotify) Created event {ev.window}, parent {ev.parent}");
        }

        public int Run()
        {
            IntPtr ev = Marshal.AllocHGlobal( 24* sizeof(long) );

            Window ReturnedParent = 0, ReturnedRoot = 0;
            Window[] ChildWindows = new Window[0];
            uint nChildren = 0;

            //Xlib.XSelectInput(this.display, this.root, X.EventMask.ButtonPressMask);

            Xlib.XGrabServer(this.display); // Lock the server during initialization
            var r = Xlib.XQueryTree(this.display, this.root, ref ReturnedRoot, ref ReturnedParent,
                ref ChildWindows, ref nChildren);

            Console.WriteLine($"Pre-existing child windows: {nChildren}");
            for (var i = 0; i <nChildren; i++)
            {
                AddFrame(ChildWindows[i]);
            }
            Xlib.XUngrabServer(this.display); // Release the lock on the server.

            while ( true )
            {

                var rv = Xlib.XNextEvent(this.display, ev);
                var xevent = Marshal.PtrToStructure<X11.XEvent>(ev);

                switch (xevent.type)
                {
                    case (int)Event.DestroyNotify:
                        var destroy_event = Marshal.PtrToStructure<X11.XDestroyWindowEvent>(ev);
                        OnDestroyNotify(destroy_event);
                        break;
                    case (int)Event.CreateNotify:
                        var create_event = Marshal.PtrToStructure<X11.XCreateNotifyEvent>(ev);
                        OnCreateNotify(create_event);
                        break;
                    case (int)Event.MapNotify:
                        var map_notify = Marshal.PtrToStructure<X11.XMapNotifyEvent>(ev);
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
                        var reparent_event = Marshal.PtrToStructure<X11.XReparentNotifyEvent>(ev);
                        OnReparentNotify(reparent_event);
                        break;
                    case (int)Event.ButtonPress:
                        var button_press_event = Marshal.PtrToStructure<X11.XButtonEvent>(ev);
                        OnButtonPressEvent(button_press_event);
                        break;
                    case (int)Event.MotionNotify:
                        var motion_event = Marshal.PtrToStructure<X11.XMotionEvent>(ev);
                        OnMotionEvent(motion_event);
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
