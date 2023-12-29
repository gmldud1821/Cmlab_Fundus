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
/// ī�޶� ����
/// </summary>
/// <returns>���� ����</returns>
ErrorInitializeCameras initializeCameras()
{
	// ī�޶� �δ�(����ī�޶�, ����ī�޶�) ����
	for (int i = 0; i < NumberOfCameras; i++)
	{
		mCameras[i].open(i, cv::CAP_DSHOW);
		if (!mCameras[i].isOpened())
		{
			return ErrorInitializeCameras::NotOpen;
		}
	}

	// ����ī�޶�� ����ī�޶� �ٲ� ��� �ε����� �ٲٸ� ��
	cameras[(int)Part::Retina] = mCameras;
	cameras[(int)Part::Cornea] = mCameras + 1;

	return ErrorInitializeCameras::None;
}

/// <summary>
/// ī�޶� �ݱ�
/// </summary>
void closeCameras()
{
	for (int i = 0; i < NumberOfCameras; i++)
	{
		mCameras[i].release();
	}
}

/// <summary>
/// �̹����� ����� ������ ��������
/// </summary>
/// <param name="windowHandle">������ �ڵ�. C#���� ������ �����츦 �����´�.</param>
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
/// ������ ����
/// </summary>
void closeWindow()
{
	ReleaseDC(windowHandle, windowDc);
}

/// <summary>
/// ī�޶� �� ������ ���
/// </summary>
/// <param name="part">�������� ��������</param>
/// <param name="x">����� ������ ���� �� �������� x��ǥ</param>
/// <param name="y">����� ������ ���� �� �������� y��ǥ</param>
/// <param name="width">����� ������ ���� ����</param>
/// <param name="height">����� ������ ���� ����</param>
/// <returns>���� ����</returns>
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
/// ��µ� �̹��� �����
/// </summary>
/// <param name="x">���� ������ ���� �� �������� x��ǥ</param>
/// <param name="y">���� ������ ���� �� �������� y��ǥ</param>
/// <param name="width">���� ������ ���� ����</param>
/// <param name="height">���� ������ ���� ����</param>
void clearImage(int x, int y, int width, int height)
{
	cv::Mat blackImage(height, width, CV_8UC3, cv::Scalar(0, 0, 0));
	showImage(blackImage, x, y, width, height, windowDc);
}

/// <summary>
/// �̹��� ���
/// </summary>
/// <param name="image">����� �̹���</param>
/// <param name="x">����� ������ ���� �� �������� x��ǥ</param>
/// <param name="y">����� ������ ���� �� �������� y��ǥ</param>
/// <param name="width">����� ������ ���� ����</param>
/// <param name="height">����� ������ ���� ����</param>
/// <param name="windowDc">����� �������� DC</param>
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
/// �̹��� ĸó
/// </summary>
/// <param name="x">ĸó�� �̹����� ����� ������ ���� �� ������ x��ǥ</param>
/// <param name="y">ĸó�� �̹����� ����� ������ ���� �� ������ y��ǥ</param>
/// <param name="width">ĸó�� �̹����� ����� ������ ���� ����</param>
/// <param name="height">ĸó�� �̹����� ����� ������ ���� ����</param>
void captureImage(int x, int y, int width, int height)
{
	showImage(frame, x, y, width, height, shotWindowDc);
}

/// <summary>
/// ĸó�� �̹����� ����� ������ ��������
/// </summary>
/// <param name="windowHandle">������ �ڵ�</param>
void initializeShotWindow(HWND windowHandle)
{
	::shotWindowHandle = windowHandle;
	shotWindowDc = GetDC(windowHandle);
	SetStretchBltMode(shotWindowDc, COLORONCOLOR);
}

/// <summary>
/// ĸó�� �̹����� ����� ������ ����
/// </summary>
void closeShotWindow()
{
	ReleaseDC(shotWindowHandle, shotWindowDc);
}