#pragma once

#include <Windows.h>
#include <opencv2/opencv.hpp>

#ifdef _DEBUG
#pragma comment(lib, "opencv_world440d.lib")
#else
#pragma comment(lib, "opencv_world440.lib")
#endif

#define NumberOfCameras 2

enum class Part { Retina, Cornea };

enum class ErrorInitializeCameras { None, NotOpen };
enum class ErrorShowCameraFrame { None, CannotRead };

// C#에서 활용할 함수들
extern "C"
{
	__declspec(dllexport) ErrorInitializeCameras initializeCameras();
	__declspec(dllexport) void closeCameras();
	__declspec(dllexport) void initializeWindow(HWND windowHandle);
	__declspec(dllexport) void closeWindow();
	__declspec(dllexport) ErrorShowCameraFrame showCameraFrame(Part part, int x, int y, int width, int height);
	__declspec(dllexport) void clearImage(int x, int y, int width, int height);
	__declspec(dllexport) void captureImage(int x, int y, int width, int height);
	__declspec(dllexport) void initializeShotWindow(HWND windowHandle);
	__declspec(dllexport) void closeShotWindow();
}

void showImage(cv::Mat& image, int x, int y, int width, int height, HDC windowDc);