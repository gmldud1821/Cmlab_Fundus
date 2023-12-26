#include "camera.h"

cv::VideoCapture mCameras[NumberOfCameras];
cv::VideoCapture* cameras[NumberOfCameras];

HWND windowHandle;
HDC windowDc;
BITMAPINFO bitmapInfo;

ErrorInitializeCameras initializeCameras()
{
	for (int i = 0; i < NumberOfCameras; i++)
	{
		mCameras[i].open(i, cv::CAP_DSHOW);
		if (!mCameras[i].isOpened())
		{
			return ErrorInitializeCameras::NotOpen;
		}
	}

	cameras[(int)Part::Retina] = mCameras;
	cameras[(int)Part::Cornea] = mCameras + 1;

	return ErrorInitializeCameras::None;
}

void closeCameras()
{
	for (int i = 0; i < NumberOfCameras; i++)
	{
		mCameras[i].release();
	}
}

void initializeWindow(HWND windowHandle)
{
	::windowHandle = windowHandle;
	windowDc = GetDC(windowHandle);
	SetStretchBltMode(windowDc, COLORONCOLOR);

	bitmapInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bitmapInfo.bmiHeader.biPlanes = 1;
	bitmapInfo.bmiHeader.biBitCount = 24;
	bitmapInfo.bmiHeader.biCompression = BI_RGB;
}

void closeWindow()
{
	ReleaseDC(windowHandle, windowDc);
}

ErrorShowCameraFrame showCameraFrame(Part part, int x, int y, int width, int height)
{
	cv::Mat frame;
	bool ret = cameras[(int)part]->read(frame);
	if (!ret)
	{
		return ErrorShowCameraFrame::CannotRead;
	}

	showImage(frame, x, y, width, height);

	return ErrorShowCameraFrame::None;
}

void clearImage(int x, int y, int width, int height)
{
	cv::Mat blackImage(height, width, CV_8UC3, cv::Scalar(0, 0, 0));
	showImage(blackImage, x, y, width, height);
}

void showImage(cv::Mat& image, int x, int y, int width, int height)
{
	HDC memoryDc = CreateCompatibleDC(windowDc);
	HBITMAP imageBitmap = CreateDIBitmap(windowDc, &bitmapInfo.bmiHeader, CBM_INIT, image.data, &bitmapInfo, DIB_RGB_COLORS);
	HBITMAP oldBitmap = (HBITMAP)SelectObject(memoryDc, imageBitmap);

	bitmapInfo.bmiHeader.biWidth = image.cols;
	bitmapInfo.bmiHeader.biHeight = -image.rows;

	StretchBlt(windowDc, x, y, width, height, memoryDc, 0, 0, image.cols, image.rows, SRCCOPY);
	SelectObject(memoryDc, oldBitmap);
	DeleteObject(imageBitmap);
	DeleteDC(memoryDc);
}