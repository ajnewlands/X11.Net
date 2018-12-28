using System;
using System.Runtime.InteropServices;

namespace X11
{
    public enum Event : int
    {
        KeyPress = 2,
        KeyRelease = 3,
        ButtonPress = 4,
        ButtonRelease = 5,
        MotionNotify = 6,
        EnterNotify = 7,
        LeaveNotify = 8,
        FocusIn = 9,
        FocusOut = 10,
        KeymapNotify = 11,
        Expose = 12,
        GraphicsExpose = 13,
        NoExpose = 14,
        VisibilityNotify = 15,
        CreateNotify = 16,
        DestroyNotify = 17,
        UnmapNotify = 18,
        MapNotify = 19,
        MapRequest = 20,
        ReparentNotify = 21,
        ConfigureNotify = 22,
        ConfigureRequest = 23,
        GravityNotify = 24,
        ResizeRequest = 25,
        CirculateNotify = 26,
        CirculateRequest = 27,
        PropertyNotify = 28,
        SelectionClear = 29,
        SelectionRequest = 30,
        SelectionNotify = 31,
        ColormapNotify = 32,
        ClientMessage = 33,
        MappingNotify = 34,
        GenericEvent = 35,
        LASTEvent = 36
    }

    public enum EventMask : long
    {
        /// <summary>
        /// These are the input event masks from X.h
        /// </summary>
        NoEventMask = 0L,
        KeyPressMask = (1L << 0),
        KeyReleaseMask = (1L << 1),
        ButtonPressMask = (1L << 2),
        ButtonReleaseMask = (1L << 3),
        EnterWindowMask = (1L << 4),
        LeaveWindowMask = (1L << 5),
        PointerMotionMask = (1L << 6),
        PointerMotionHintMask = (1L << 7),
        Button1MotionMask = (1L << 8),
        Button2MotionMask = (1L << 9),
        Button3MotionMask = (1L << 10),
        Button4MotionMask = (1L << 11),
        Button5MotionMask = (1L << 12),
        ButtonMotionMask = (1L << 13),
        KeymapStateMask = (1L << 14),
        ExposureMask = (1L << 15),
        VisibilityChangeMask = (1L << 16),
        StructureNotifyMask = (1L << 17),
        ResizeRedirectMask = (1L << 18),
        SubstructureNotifyMask = (1L << 19),
        SubstructureRedirectMask = (1L << 20),
        FocusChangeMask = (1L << 21),
        PropertyChangeMask = (1L << 22),
        ColormapChangeMask = (1L << 23),
        OwnerGrabButtonMask = (1L << 24),
    }

    public enum QueueMode: int
    {
        QueuedAlready = 0,
        QueuedAfterReading = 1,
        QueuedAfterFlush = 2,
    }

    public partial class Xlib
    {
        /// <summary>
        /// The XNextEvent function copies the first event from the event queue into the specified XEvent structure and
        /// then removes it from the queue.If the event queue is empty, XNextEvent flushes the output buffer and blocks
        ///  until an event is received.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XNextEvent(IntPtr display, IntPtr event_return);

        /// <summary>
        /// The XPeekEvent function returns the first event from the event queue, but it does not remove the event from
        /// the queue.If the queue is empty, XPeekEvent flushes the output buffer and blocks until an event is received.
        /// It then copies the event into the client-supplied XEvent structure without removing it from the event queue.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XPeekEvent(IntPtr display, IntPtr event_return);

        /// <summary>
        /// The XWindowEvent function searches the event queue for an event that matches both the specified window and
        /// event mask.When it finds a match, XWindowEvent removes that event from the queue and copies it into the
        /// specified XEvent structure.The other events stored in the queue are not discarded.If a matching event is
        /// not in the queue, XWindowEvent flushes the output buffer and blocks until one is received.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Window specifier</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XWindowEvent(IntPtr display, Window window, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XCheckWindowEvent function searches the event queue and then the events available on the server connection
        /// for the first event that matches the specified window and event mask.If it finds a match, XCheckWindowEvent
        /// removes that event, copies it into the specified XEvent structure, and returns True.  The other events stored
        /// in the queue are not discarded.  If the event you requested is not available, XCheckWindowEvent returns False,
        /// and the output buffer will have been flushed.
        /// </summary>
        /// <param name="display">Connected display </param>
        /// <param name="window">Window specifier</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XCheckWindowEvent(IntPtr display, Window window, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XMaskEvent function searches the event queue for the events associated with the specified mask.  When it
        /// finds a match, XMaskEvent removes that event and copies it into the specified XEvent structure.The other
        /// events stored in the queue are not discarded.If the event you requested is not in the queue, XMaskEvent
        /// flushes the output buffer and blocks until one is received.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XMaskEvent(IntPtr display, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XCheckMaskEvent function searches the event queue and then any events available on the server connection
        /// for the first event that matches the specified mask.If it finds a match, XCheckMaskEvent removes that event,
        /// copies it into the specified XEvent structure, and returns True.  The other events stored in the queue are not
        /// discarded.  If the event you requested is not available, XCheckMaskEvent returns False, and the output buffer
        /// will have been flushed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="mask">Event mask</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XCheckMaskEvent(IntPtr display, EventMask mask, IntPtr event_return);

        /// <summary>
        /// The XCheckTypedEvent function searches the event queue and then any events available on the server connection
        /// for the first event that matches the specified type.If it finds a match, XCheckTypedEvent removes that
        /// event, copies it into the specified XEvent structure, and returns True. The other events in the queue are not
        /// discarded.  If the event is not available, XCheckTypedEvent returns False, and the output buffer will have
        /// been flushed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="type">Event type</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XCheckTypedEvent(IntPtr display, Event type, IntPtr event_return);

        /// <summary>
        /// The XCheckTypedWindowEvent function searches the event queue and then any events available on the server con‐
        /// nection for the first event that matches the specified type and window. If it finds a match, XCheckTypedWin‐
        /// dowEvent removes the event from the queue, copies it into the specified XEvent structure, and returns True.
        /// The other events in the queue are not discarded.If the event is not available, XCheckTypedWindowEvent
        /// returns False, and the output buffer will have been flushed.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="window">Window specifier</param>
        /// <param name="type">Event type</param>
        /// <param name="event_return">Pre-allocated buffer to hold the returned event</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XCheckTypedWindowEvent(IntPtr display, Window window, Event type, IntPtr event_return);

        /// <summary>
        /// The XSync function flushes the output buffer and then waits until all requests have been received and pro‐
        /// cessed by the X server.Any errors generated must be handled by the error handler.For each protocol error
        /// received by Xlib, XSync calls the client application's error handling routine (see section 11.8.2).  Any
        /// events generated by the server are enqueued into the library's event queue.
        /// Finally, if you passed False, XSync does not discard the events in the queue.  If you passed True, XSync dis‐
        /// cards all events in the queue, including those events that were on the queue before XSync was called.  Client
        /// applications seldom need to call XSync.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="discard">Discard queued events</param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XSync(IntPtr display, bool discard);

        /// <summary>
        /// The XFlush function flushes the output buffer.  Most client applications need not use this function because
        /// the output buffer is automatically flushed as needed by calls to XPending, XNextEvent, and XWindowEvent.
        /// Events generated by the server may be enqueued into the library's event queue.
        /// </summary>
        /// <param name="display">Connected display </param>
        /// <returns></returns>
        [DllImport("libX11.so.6")]
        public static extern int XFlush(IntPtr display);

        /// <summary>
        /// If mode is QueuedAlready, XEventsQueued returns the number of events already in the event queue (and never
        /// performs a system call).  If mode is QueuedAfterFlush, XEventsQueued returns the number of events already in
        /// the queue if the number is nonzero.If there are no events in the queue, XEventsQueued flushes the output
        /// buffer, attempts to read more events out of the application's connection, and returns the number read.  If
        /// mode is QueuedAfterReading, XEventsQueued returns the number of events already in the queue if the number is
        /// nonzero.If there are no events in the queue, XEventsQueued attempts to read more events out of the applica‐
        /// tion's connection without flushing the output buffer and returns the number read.
        ///
        /// XEventsQueued always returns immediately without I/O if there are events already in the queue.XEventsQueued
        /// with mode QueuedAfterFlush is identical in behavior to XPending.XEventsQueued with mode QueuedAlready is
        /// identical to the XQLength function.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <param name="mode">Mode of operation</param>
        /// <returns>Number of queued events</returns>
        [DllImport("libX11.so.6")]
        public static extern int XEventsQueued(IntPtr display, QueueMode mode);

        /// <summary>
        /// The XPending function returns the number of events that have been received from the X server but have not been
        /// removed from the event queue.XPending is identical to XEventsQueued with the mode QueuedAfterFlush speci‐
        /// fied.
        /// </summary>
        /// <param name="display">Connected display</param>
        /// <returns>Number of events pending removal from the queue</returns>
        [DllImport("libX11.so.6")]
        public static extern int XPending(IntPtr display);
    }
}
