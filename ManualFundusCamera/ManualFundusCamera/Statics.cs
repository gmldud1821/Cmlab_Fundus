﻿using System;
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
        /// <param name="windowHandle">윈도우 핸들</param>
        [DllImport(dllFileName)]
        extern public static void initializeWindow(IntPtr windowHandle);

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
        [DllImport(dllFileName)]
        extern public static void captureImage(int x, int y, int width, int height);

        /// <summary>
        /// 캡처한 이미지를 출력할 윈도우 열기
        /// </summary>
        /// <param name="windowHandle">윈도우 핸들</param>
        [DllImport(dllFileName)]
        extern public static void initializeShotWindow(IntPtr windowHandle);

        /// <summary>
        /// 캡처한 이미지를 출력한 윈도우 해제
        /// </summary>
        [DllImport(dllFileName)]
        extern public static void closeShotWindow();
    }
}
