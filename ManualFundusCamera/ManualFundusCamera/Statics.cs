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
        public enum VideoCaptureProperties
        {
            CAP_PROP_POS_MSEC = 0, //!< Current position of the video file in milliseconds.
            CAP_PROP_POS_FRAMES = 1, //!< 0-based index of the frame to be decoded/captured next.
            CAP_PROP_POS_AVI_RATIO = 2, //!< Relative position of the video file: 0=start of the film, 1=end of the film.
            CAP_PROP_FRAME_WIDTH = 3, //!< Width of the frames in the video stream.
            CAP_PROP_FRAME_HEIGHT = 4, //!< Height of the frames in the video stream.
            CAP_PROP_FPS = 5, //!< Frame rate.
            CAP_PROP_FOURCC = 6, //!< 4-character code of codec. see VideoWriter::fourcc .
            CAP_PROP_FRAME_COUNT = 7, //!< Number of frames in the video file.
            CAP_PROP_FORMAT = 8, //!< Format of the %Mat objects (see Mat::type()) returned by VideoCapture::retrieve().
                                 //!< Set value -1 to fetch undecoded RAW video streams (as Mat 8UC1).
            CAP_PROP_MODE = 9, //!< Backend-specific value indicating the current capture mode.
            CAP_PROP_BRIGHTNESS = 10, //!< Brightness of the image (only for those cameras that support).
            CAP_PROP_CONTRAST = 11, //!< Contrast of the image (only for cameras).
            CAP_PROP_SATURATION = 12, //!< Saturation of the image (only for cameras).
            CAP_PROP_HUE = 13, //!< Hue of the image (only for cameras).
            CAP_PROP_GAIN = 14, //!< Gain of the image (only for those cameras that support).
            CAP_PROP_EXPOSURE = 15, //!< Exposure (only for those cameras that support).
            CAP_PROP_CONVERT_RGB = 16, //!< Boolean flags indicating whether images should be converted to RGB. <br/>
                                       //!< *GStreamer note*: The flag is ignored in case if custom pipeline is used. It's user responsibility to interpret pipeline output.
            CAP_PROP_WHITE_BALANCE_BLUE_U = 17, //!< Currently unsupported.
            CAP_PROP_RECTIFICATION = 18, //!< Rectification flag for stereo cameras (note: only supported by DC1394 v 2.x backend currently).
            CAP_PROP_MONOCHROME = 19,
            CAP_PROP_SHARPNESS = 20,
            CAP_PROP_AUTO_EXPOSURE = 21, //!< DC1394: exposure control done by camera, user can adjust reference level using this feature.
            CAP_PROP_GAMMA = 22,
            CAP_PROP_TEMPERATURE = 23,
            CAP_PROP_TRIGGER = 24,
            CAP_PROP_TRIGGER_DELAY = 25,
            CAP_PROP_WHITE_BALANCE_RED_V = 26,
            CAP_PROP_ZOOM = 27,
            CAP_PROP_FOCUS = 28,
            CAP_PROP_GUID = 29,
            CAP_PROP_ISO_SPEED = 30,
            CAP_PROP_BACKLIGHT = 32,
            CAP_PROP_PAN = 33,
            CAP_PROP_TILT = 34,
            CAP_PROP_ROLL = 35,
            CAP_PROP_IRIS = 36,
            CAP_PROP_SETTINGS = 37, //!< Pop up video/camera filter dialog (note: only supported by DSHOW backend currently. The property value is ignored)
            CAP_PROP_BUFFERSIZE = 38,
            CAP_PROP_AUTOFOCUS = 39,
            CAP_PROP_SAR_NUM = 40, //!< Sample aspect ratio: num/den (num)
            CAP_PROP_SAR_DEN = 41, //!< Sample aspect ratio: num/den (den)
            CAP_PROP_BACKEND = 42, //!< Current backend (enum VideoCaptureAPIs). Read-only property
            CAP_PROP_CHANNEL = 43, //!< Video input or Channel Number (only for those cameras that support)
            CAP_PROP_AUTO_WB = 44, //!< enable/ disable auto white-balance
            CAP_PROP_WB_TEMPERATURE = 45, //!< white-balance color temperature
            CAP_PROP_CODEC_PIXEL_FORMAT = 46,    //!< (read-only) codec's pixel format. 4-character code - see VideoWriter::fourcc . Subset of [AV_PIX_FMT_*](https://github.com/FFmpeg/FFmpeg/blob/master/libavcodec/raw.c) or -1 if unknown
            CAP_PROP_BITRATE = 47, //!< (read-only) Video bitrate in kbits/s
        };

        public enum Part { Retina, Cornea };
        public enum ErrorInitializeCameras { None, RetinaNotOpen, CorneaNotOpen };
        public enum ErrorShowCameraFrame { None, CannotRead };
        public enum ErrorSetCameraParam { None, Failed };

        // C++로 만든 dll에 있는 함수들을 불러온다.

#if DEBUG
        private const string dllFileName = @"..\..\..\x64\Debug\ManualFundusCameraDll.dll";
#else
        private const string dllFileName = @"..\..\..\x64\Release\ManualFundusCameraDll.dll";
#endif

        /// <summary>
        /// 카메라 열기
        /// </summary>
        /// <returns>에러 여부</returns>
        [DllImport(dllFileName)]
        extern public static ErrorInitializeCameras initializeCameras();

        /// <summary>
        /// 카메라 닫기
        /// </summary>
        [DllImport(dllFileName)]
        extern public static void closeCameras();

        /// <summary>
        /// 카메라 영상을 출력할 윈도우 열기
        /// </summary>
        /// <param name="windowHandle">카메라 영상을 출력할 윈도우 핸들</param>
        /// <param name="shotWindowHandle">캡처한 이미지를 출력할 윈도우 핸들</param>
        [DllImport(dllFileName)]
        extern public static void initializeWindow(IntPtr windowHandle, IntPtr shotWindowHandle);

        /// <summary>
        /// 카메라 영상을 출력하는 윈도우 닫기
        /// </summary>
        [DllImport(dllFileName)]
        extern public static void closeWindow();

        /// <summary>
        /// 카메라 한 프레임 출력
        /// </summary>
        /// <param name="part">망막인지 각막인지</param>
        /// <param name="x">출력할 영역의 왼쪽 위 꼭짓점의 x좌표</param>
        /// <param name="y">출력할 영역의 왼쪽 위 꼭짓점의 y좌표</param>
        /// <param name="width">출력할 영역의 가로 길이</param>
        /// <param name="height">출력할 영역의 세로 길이</param>
        /// <returns>에러 여부</returns>
        [DllImport(dllFileName)]
        extern public static ErrorShowCameraFrame showCameraFrame(Part part, int x, int y, int width, int height);

        /// <summary>
        /// 출력한 이미지 지우기
        /// </summary>
        /// <param name="x">지울 영역의 왼쪽 위 꼭짓점의 x좌표</param>
        /// <param name="y">지울 영역의 왼쪽 위 꼭짓점의 y좌표</param>
        /// <param name="width">지울 영역의 가로 길이</param>
        /// <param name="height">지울 영역의 세로 길이</param>
        [DllImport(dllFileName)]
        extern public static void clearImage(int x, int y, int width, int height);

        /// <summary>
        /// 이미지 캡처
        /// </summary>
        /// <param name="x">캡처한 이미지를 출력할 영역의 x좌표</param>
        /// <param name="y">캡처한 이미지를 출력할 영역의 y좌표</param>
        /// <param name="width">캡처한 이미지를 출력할 영역의 가로 길이</param>
        /// <param name="height">캡처한 이미지를 출력할 영역의 세로 길이</param>
        /// <param name="pX">처리한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 x좌표</param>
        /// <param name="pY">처리한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 y좌표</param>
        /// <param name="pWidth">처리한 이미지를 출력할 영역의 가로 길이</param>
        /// <param name="pHeight">처리한 이미지를 출력할 영역의 세로 길이</param>
        /// <param name="fileName">처리한 이미지를 저장할 파일</param>
        [DllImport(dllFileName)]
        extern public static void captureImage(int x, int y, int width, int height, int pX, int pY, int pWidth, int pHeight, string fileName);

        [DllImport(dllFileName)]
        extern public static ErrorSetCameraParam setCameraParam(Part part, VideoCaptureProperties prop, int value);

        [DllImport(dllFileName)]
        extern public static int getCameraParam(Part part, VideoCaptureProperties prop);

        public const string UrlForUploadingImage = "http://175.112.57.221:1114/addRetinaImage";
        public const string DateTimeFormatForImageFileName = "yyyy-MM-dd_HH-mm-ss";
        public const string JpgExt = ".jpg";
        public const string DoubleQuotationMarks = "\"";
        public const string Blank = "";
        public const string UrlForGettingImage = "http://175.112.57.221:1114/getRetinaImage";
        public const string Id = "id";
        public const string Url = "url";

        public const char Comma = ',';
        public const char QuestionMarks = '?';
        public const char And = '&';
        public const char Equal = '=';

        public const int SleepTimeWhenCapturing = 1000;
    }
}
