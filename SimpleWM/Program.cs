using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using X11;

namespace SimpleWM
{
    public class WindowManager
    {
        private IntPtr display;
        private ulong root;
        private readonly Dictionary<ulong, ulong> ClientWindows = new Dictionary<ulong, ulong>();

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
            // Debug only
            // Xlib.XSynchronize(this.display, true);
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
                X.EventMask.SubstructureRedirectMask | X.EventMask.SubstructureNotifyMask);
            Xlib.XSync(this.display, false);

            Xlib.XDefineCursor(this.display, this.root, Xlib.XCreateFontCursor(this.display, X.Cursor.XC_left_ptr));


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

        public void AddFrame(ulong child)
        {
            if (this.ClientWindows.ContainsKey(child))
                return; // Window has already been framed.

            Xlib.XGetWindowAttributes(this.display, child, out var attr);

            var frame = Xlib.XCreateSimpleWindow(this.display, this.root, attr.x, attr.y, attr.width, attr.height,
                3, GetPixelByName("goldenrod"), GetPixelByName("dark slate grey"));


            Xlib.XSelectInput(this.display, frame, X.EventMask.ButtonPressMask | X.EventMask.ButtonReleaseMask);
            // TODO - ideally the cursor would be some sort of singleton type
            Xlib.XDefineCursor(this.display, frame, Xlib.XCreateFontCursor(this.display, X.Cursor.XC_sizing));


            //Xlib.XUngrabButton(this.display, X.Button.LEFT, (1 << 15), child);

            Console.WriteLine($"(AddFrame) Created frame {frame} for window {child}");

            Xlib.XReparentWindow(this.display, child, frame, 0, 0);
            Xlib.XMapWindow(this.display, frame);
            // Ensure the child window survives the untimely death of the window manager.
            Xlib.XAddToSaveSet(this.display, child);

            this.ClientWindows[child] = frame;// Track the new window and its frame.

        }

        public void RemoveFrame(ulong child)
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

            ulong ReturnedParent = 0, ReturnedRoot = 0;
            ulong[] ChildWindows = new ulong[0];
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
                    case (int)X.Event.DestroyNotify:
                        var destroy_event = Marshal.PtrToStructure<X11.XDestroyWindowEvent>(ev);
                        OnDestroyNotify(destroy_event);
                        break;
                    case (int)X.Event.CreateNotify:
                        var create_event = Marshal.PtrToStructure<X11.XCreateNotifyEvent>(ev);
                        OnCreateNotify(create_event);
                        break;
                    case (int)X.Event.MapNotify:
                        var map_notify = Marshal.PtrToStructure<X11.XMapNotifyEvent>(ev);
                        OnMapNotify(map_notify);
                        break;
                    case (int)X.Event.MapRequest:
                        var map_event = Marshal.PtrToStructure<X11.XMapRequestEvent>(ev);
                        OnMapRequest(map_event);
                        break;
                    case (int)X.Event.ConfigureRequest:
                        var cfg_event = Marshal.PtrToStructure<X11.XConfigureRequestEvent>(ev);
                        OnConfigureRequest(cfg_event);
                        break;
                    case (int)X.Event.UnmapNotify:
                        var unmap_event = Marshal.PtrToStructure<X11.XUnmapEvent>(ev);
                        OnUnmapNotify(unmap_event);
                        break;
                    case (int)X.Event.ReparentNotify:
                        var reparent_event = Marshal.PtrToStructure<X11.XReparentNotifyEvent>(ev);
                        OnReparentNotify(reparent_event);
                        break;
                    case (int)X.Event.ButtonPress:
                        var button_press_event = Marshal.PtrToStructure<X11.XButtonEvent>(ev);
                        OnButtonPressEvent(button_press_event);
                        break;
                    default:
                        Console.WriteLine($"Event type: { Enum.GetName(typeof(X.Event), xevent.type)}");
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
