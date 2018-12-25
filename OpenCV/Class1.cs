using System;
using System.Runtime.InteropServices;

namespace OpenCV
{
    public class CV
    {
        public const int CV_8UC4 = 24;

        [DllImport("libopencv_core.so")]
        public static extern IntPtr cvCreateMat(int rows, int cols, int type);

        [DllImport("libopencv_core.so")]
        public static extern IntPtr cvInitMatHeader(IntPtr mat, int rows, int cols,
            int type, IntPtr data, int step = 0x7fffffff);

        [DllImport("libopencv_core.so")]
        public static extern void cvSetData(IntPtr arr, IntPtr data, int step);

        [DllImport("libopencv_highgui.so")]
        public static extern void cvShowImage(string name, IntPtr image);

        [DllImport("libopencv_highgui.so")]
        public static extern int cvWaitKey(int delay = 0);
    }
}
