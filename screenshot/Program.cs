using System;
using System.Runtime.InteropServices;
using X11;

namespace screenshot
{
    class Program
    {
        static void Main(string[] args)
        {
            var display = Xlib.XOpenDisplay(null);
            var root = Xlib.XDefaultRootWindow(display);

            Xlib.XGetWindowAttributes(display, root, out var attr);
            Console.WriteLine($"Display geometry: {attr.width} x {attr.height}");

            var Image = Xlib.XGetImage(display, root, 0, 0, attr.width, attr.height, Xlib.AllPlanes, Pixmap.ZPixmap);
            Console.WriteLine($"Got an image {Image.width} x {Image.height}, depth {Image.depth}");

            var mat = OpenCV.CV.cvCreateMat(Image.height, Image.width, OpenCV.CV.CV_8UC4);
            OpenCV.CV.cvInitMatHeader(mat, Image.height, Image.width, OpenCV.CV.CV_8UC4, Image.data);
            OpenCV.CV.cvShowImage("test", mat);
            OpenCV.CV.cvWaitKey();

            Xutil.XDestroyImage(ref Image);
            Xlib.XCloseDisplay(display);
        }
    }
}
