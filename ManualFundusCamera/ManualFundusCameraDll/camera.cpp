#include "camera.h"

cv::VideoCapture cameras[NumberOfCameras];
cv::Mat frame[NumberOfCameras];

HWND windowHandle;
HDC windowDc;
HWND shotWindowHandle;
HDC shotWindowDc;
BITMAPINFO bitmapInfo;

/// <summary>
/// ī�޶� ����
/// </summary>
/// <returns>���� ����</returns>
ErrorInitializeCameras initializeCameras()
{
	// ī�޶� �δ�(����ī�޶�, ����ī�޶�) ����. ����ī�޶�� ����ī�޶� �ٲ� ��� �ε����� �ٲٸ� ��

	cameras[(int)Part::Retina].open(0, cv::CAP_DSHOW);
	if (!cameras[(int)Part::Retina].isOpened())
	{
		return ErrorInitializeCameras::RetinaNotOpen;
	}
	cameras[(int)Part::Retina].set(cv::CAP_PROP_AUTOFOCUS, 0);
	cameras[(int)Part::Retina].set(cv::CAP_PROP_AUTO_EXPOSURE, 0);
	cameras[(int)Part::Retina].set(cv::CAP_PROP_AUTO_WB, 0);

	cameras[(int)Part::Cornea].open(1, cv::CAP_DSHOW);
	if (!cameras[(int)Part::Cornea].isOpened())
	{
		return ErrorInitializeCameras::CorneaNotOpen;
	}
	cameras[(int)Part::Cornea].set(cv::CAP_PROP_AUTOFOCUS, 0);
	cameras[(int)Part::Cornea].set(cv::CAP_PROP_AUTO_EXPOSURE, 0);
	cameras[(int)Part::Cornea].set(cv::CAP_PROP_AUTO_WB, 0);

	return ErrorInitializeCameras::None;
}

/// <summary>
/// ī�޶� �ݱ�
/// </summary>
void closeCameras()
{
	for (int i = 0; i < NumberOfCameras; i++)
	{
		cameras[i].release();
	}
}

/// <summary>
/// �̹����� ����� ������ ��������
/// </summary>
/// <param name="windowHandle">ī�޶� ������ ����� ������ �ڵ�. C#���� ������ �����츦 �����´�.</param>
/// <param name="shotWindowHandle">ĸó�� �̹����� ����� ������ �ڵ�. C#���� ������ �����츦 �����´�.</param>
void initializeWindow(HWND windowHandle, HWND shotWindowHandle)
{
	::windowHandle = windowHandle;
	windowDc = GetDC(windowHandle);
	SetStretchBltMode(windowDc, COLORONCOLOR);

	::shotWindowHandle = shotWindowHandle;
	shotWindowDc = GetDC(shotWindowHandle);
	SetStretchBltMode(shotWindowDc, COLORONCOLOR);

	bitmapInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bitmapInfo.bmiHeader.biPlanes = BiPlanes;
	bitmapInfo.bmiHeader.biBitCount = BiBitCount;
	bitmapInfo.bmiHeader.biCompression = BI_RGB;
}

/// <summary>
/// ������ ����
/// </summary>
void closeWindow()
{
	ReleaseDC(windowHandle, windowDc);
	ReleaseDC(shotWindowHandle, shotWindowDc);
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
	bool ret = cameras[(int)part].read(frame[(int)part]);
	if (!ret)
	{
		return ErrorShowCameraFrame::CannotRead;
	}

	showImage(frame[(int)part], x, y, width, height, windowDc);

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
	cv::Mat blackImage(height, width, CV_8UC3, BlackScalar);
	showImage(blackImage, x, y, width, height, windowDc);
}

/// <summary>
/// �̹��� ó��
/// </summary>
/// <param name="img">ó���� �̹���</param>
/// <returns>ó���� �̹���</returns>
cv::Mat processImage(cv::Mat& img)
{
	cv::Mat blurredSubstractedImage, result;
	cv::Mat image = img.clone();
	cv::Mat blurredImage;
	cv::GaussianBlur(image, blurredImage, cv::Size(11, 11), 11);
	blurredSubstractedImage = 5.0 * image - 4.3 * blurredImage;
	cv::Mat* bgrImage = new cv::Mat[blurredSubstractedImage.channels()];
	cv::split(blurredSubstractedImage, bgrImage);
	cv::Ptr<cv::CLAHE> clahe = cv::createCLAHE(2, cv::Size(30, 30));
	cv::Mat* BGRClaheImage = new cv::Mat[image.channels()];
	for (int i = 0; i < image.channels(); i++)
	{
		clahe->apply(bgrImage[i], BGRClaheImage[i]);
	}
	cv::Mat GClaheImage;
	cv::Mat images[] = { bgrImage[0], BGRClaheImage[1], bgrImage[2] };
	cv::merge(images, image.channels(), GClaheImage);

	cv::Mat bilateralImage;
	cv::bilateralFilter(GClaheImage, bilateralImage, 21, 45, 45);

	cv::split(bilateralImage, bgrImage);
	cv::Mat tempBGRImage[] = { bgrImage[1].clone() * 1.1, bgrImage[1] * 1.2, bgrImage[2] + 60 };

	cv::Mat mergedImage1;
	cv::Mat mergedImage2;
	cv::Mat imagesToMerge[] = { bgrImage[0],bgrImage[1],tempBGRImage[2] };
	cv::merge(imagesToMerge, image.channels(), mergedImage1);

	imagesToMerge[0] = tempBGRImage[0];
	imagesToMerge[1] = tempBGRImage[1];
	cv::merge(imagesToMerge, image.channels(), mergedImage2);

	cv::Mat mergedImage = 0.15 * mergedImage1 + 0.9 * mergedImage2;

	delete[] bgrImage;
	delete[] BGRClaheImage;

	return mergedImage;
}

/// <summary>
/// �̹��� ĸó
/// </summary>
/// <param name="x">ĸó�� �̹����� ����� ������ ���� �� ������ x��ǥ</param>
/// <param name="y">ĸó�� �̹����� ����� ������ ���� �� ������ y��ǥ</param>
/// <param name="width">ĸó�� �̹����� ����� ������ ���� ����</param>
/// <param name="height">ĸó�� �̹����� ����� ������ ���� ����</param>
/// <param name="pX">ó���� �̹����� ����� ������ ���� �� ������ x��ǥ</param>
/// <param name="pY">ó���� �̹����� ����� ������ ���� �� ������ y��ǥ</param>
/// <param name="pWidth">ó���� �̹����� ����� ������ ���� ����</param>
/// <param name="pHeight">ó���� �̹����� ����� ������ ���� ����</param>
/// <param name="fileName">ó���� �̹����� ������ ����</param>
void captureImage(int x, int y, int width, int height, int pX, int pY, int pWidth, int pHeight, char* fileName)
{
	cv::Mat processedImage = processImage(frame[(int)Part::Retina]);
	cv::imwrite(fileName, processedImage);

	showImage(frame[(int)Part::Retina], x, y, width, height, shotWindowDc);
	showImage(processedImage, pX, pY, pWidth, pHeight, shotWindowDc);
}

/// <summary>
/// ī�޶� �Ķ���� ����
/// </summary>
/// <param name="part">�������� ��������</param>
/// <param name="prop">�Ķ���� ����</param>
/// <param name="value">�Ķ���� ��</param>
/// <returns>���� ����</returns>
ErrorSetCameraParam setCameraParam(Part part, cv::VideoCaptureProperties prop, int value)
{
	bool ret = cameras[(int)part].set(prop, value);
	if (ret)
	{
		return ErrorSetCameraParam::None;
	}
	else
	{
		return ErrorSetCameraParam::Failed;
	}
}

/// <summary>
/// ī�޶� �Ķ���� �� �б�
/// </summary>
/// <param name="part">�������� ��������</param>
/// <param name="prop">�Ķ���� ����</param>
/// <returns>�Ķ���� ��</returns>
int getCameraParam(Part part, cv::VideoCaptureProperties prop)
{
	return cameras[(int)part].get(prop);
}