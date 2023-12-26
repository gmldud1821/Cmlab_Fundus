using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManualFundusCamera
{
    public static class Statics
    {
        public enum Part { Retina, Cornea };
        public enum ErrorInitializeCameras { None, NotOpen };
        public enum ErrorShowCameraFrame { None, CannotRead };

#if DEBUG
        private const string dllFileName = "..\\..\\x64\\Debug\\ManualFundusCameraDll.dll";
#else
        private const string dllFileName = "..\\..\\x64\\Release\\ManualFundusCameraDll.dll";
#endif

        [DllImport(dllFileName)]
        extern public static ErrorInitializeCameras initializeCameras();

        [DllImport(dllFileName)]
        extern public static void closeCameras();

        [DllImport(dllFileName)]
        extern public static void initializeWindow(IntPtr windowHandle);

        [DllImport(dllFileName)]
        extern public static void closeWindow();

        [DllImport(dllFileName)]
        extern public static ErrorShowCameraFrame showCameraFrame(Part part, int x, int y, int width, int height);

        [DllImport(dllFileName)]
        extern public static void clearImage(int x, int y, int width, int height);
    }
}
