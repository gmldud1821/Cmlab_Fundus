#pragma once

#include <Windows.h>
#include <opencv2/opencv.hpp>

#ifdef _DEBUG
#pragma comment(lib, "opencv_world440d.lib")
#else
#pragma comment(lib, "opencv_world440.lib")
#endif

#define NumberOfCameras 2
#define BiPlanes 1
#define BiBitCount 24
#define BlackScalar cv::Scalar(0, 0, 0)

enum class Part { Retina, Cornea };

enum class ErrorInitializeCameras { None, RetinaNotOpen, CorneaNotOpen };
enum class ErrorShowCameraFrame { None, CannotRead };
enum class ErrorSetCameraParam { None, Failed };

// C#에서 활용할 함수들
extern "C"
{
	__declspec(dllexport) ErrorInitializeCameras initializeCameras();
	__declspec(dllexport) void closeCameras();
	__declspec(dllexport) void initializeWindow(HWND windowHandle, HWND shotWindowHandle);
	__declspec(dllexport) void closeWindow();
	__declspec(dllexport) ErrorShowCameraFrame showCameraFrame(Part part, int x, int y, int width, int height);
	__declspec(dllexport) void clearImage(int x, int y, int width, int height);
	__declspec(dllexport) void captureImage(int x, int y, int width, int height, int pX, int pY, int pWidth, int pHeight, char* fileName);
	__declspec(dllexport) ErrorSetCameraParam setCameraParam(Part part, cv::VideoCaptureProperties prop, int value);
	__declspec(dllexport) int getCameraParam(Part part, cv::VideoCaptureProperties prop);
}