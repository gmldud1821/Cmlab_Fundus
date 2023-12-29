#include "camera.h"

cv::VideoCapture mCameras[NumberOfCameras];
cv::VideoCapture* cameras[NumberOfCameras];

HWND windowHandle;
HDC windowDc;
BITMAPINFO bitmapInfo;

HWND shotWindowHandle;
HDC shotWindowDc;

cv::Mat frame;

/// <summary>
/// 카메라 열기
/// </summary>
/// <returns>에러 여부</returns>
ErrorInitializeCameras initializeCameras()
{
	// 카메라 두대(망막카메라, 각막카메라) 열기
	for (int i = 0; i < NumberOfCameras; i++)
	{
		mCameras[i].open(i, cv::CAP_DSHOW);
		if (!mCameras[i].isOpened())
		{
			return ErrorInitializeCameras::NotOpen;
		}
	}

	// 망막카메라와 각막카메라가 바뀐 경우 인덱스를 바꾸면 됨
	cameras[(int)Part::Retina] = mCameras;
	cameras[(int)Part::Cornea] = mCameras + 1;

	return ErrorInitializeCameras::None;
}

/// <summary>
/// 카메라 닫기
/// </summary>
void closeCameras()
{
	for (int i = 0; i < NumberOfCameras; i++)
	{
		mCameras[i].release();
	}
}

/// <summary>
/// 이미지를 출력할 윈도우 가져오기
/// </summary>
/// <param name="windowHandle">윈도우 핸들. C#에서 생성한 윈도우를 가져온다.</param>
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

/// <summary>
/// 윈도우 해제
/// </summary>
void closeWindow()
{
	ReleaseDC(windowHandle, windowDc);
}

/// <summary>
/// 카메라 한 프레임 출력
/// </summary>
/// <param name="part">망막인지 각막인지</param>
/// <param name="x">출력할 영역의 왼쪽 위 꼭짓점의 x좌표</param>
/// <param name="y">출력할 영역의 왼쪽 위 꼭짓점의 y좌표</param>
/// <param name="width">출력할 영역의 가로 길이</param>
/// <param name="height">출력할 영역의 세로 길이</param>
/// <returns>에러 여부</returns>
ErrorShowCameraFrame showCameraFrame(Part part, int x, int y, int width, int height)
{
	bool ret = cameras[(int)part]->read(frame);
	if (!ret)
	{
		return ErrorShowCameraFrame::CannotRead;
	}

	showImage(frame, x, y, width, height, windowDc);

	return ErrorShowCameraFrame::None;
}

/// <summary>
/// 출력된 이미지 지우기
/// </summary>
/// <param name="x">지울 영역의 왼쪽 위 꼭짓점의 x좌표</param>
/// <param name="y">지울 영역의 왼쪽 위 꼭짓점의 y좌표</param>
/// <param name="width">지울 영역의 가로 길이</param>
/// <param name="height">지울 영역의 세로 길이</param>
void clearImage(int x, int y, int width, int height)
{
	cv::Mat blackImage(height, width, CV_8UC3, cv::Scalar(0, 0, 0));
	showImage(blackImage, x, y, width, height, windowDc);
}

/// <summary>
/// 이미지 출력
/// </summary>
/// <param name="image">출력할 이미지</param>
/// <param name="x">출력할 영역의 왼쪽 위 꼭짓점의 x좌표</param>
/// <param name="y">출력할 영역의 왼쪽 위 꼭짓점의 y좌표</param>
/// <param name="width">출력할 영역의 가로 길이</param>
/// <param name="height">출력할 영역의 세로 길이</param>
/// <param name="windowDc">출력할 윈도우의 DC</param>
void showImage(cv::Mat& image, int x, int y, int width, int height, HDC windowDc)
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

/// <summary>
/// 이미지 캡처
/// </summary>
/// <param name="x">캡처한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 x좌표</param>
/// <param name="y">캡처한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 y좌표</param>
/// <param name="width">캡처한 이미지를 출력할 영역의 가로 길이</param>
/// <param name="height">캡처한 이미지를 출력할 영역의 세로 길이</param>
void captureImage(int x, int y, int width, int height)
{
	showImage(frame, x, y, width, height, shotWindowDc);
}

/// <summary>
/// 캡처한 이미지를 출력할 윈도우 가져오기
/// </summary>
/// <param name="windowHandle">윈도우 핸들</param>
void initializeShotWindow(HWND windowHandle)
{
	::shotWindowHandle = windowHandle;
	shotWindowDc = GetDC(windowHandle);
	SetStretchBltMode(shotWindowDc, COLORONCOLOR);
}

/// <summary>
/// 캡처한 이미지를 출력한 윈도우 해제
/// </summary>
void closeShotWindow()
{
	ReleaseDC(shotWindowHandle, shotWindowDc);
}