#include "camera.h"

cv::VideoCapture cameras[NumberOfCameras];
cv::Mat frame[NumberOfCameras];

HWND windowHandle;
HDC windowDc;
HWND shotWindowHandle;
HDC shotWindowDc;
BITMAPINFO bitmapInfo;

/// <summary>
/// 카메라 열기
/// </summary>
/// <returns>에러 여부</returns>
ErrorInitializeCameras initializeCameras()
{
	// 카메라 두대(망막카메라, 각막카메라) 열기. 망막카메라와 각막카메라가 바뀐 경우 인덱스를 바꾸면 됨

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
/// 카메라 닫기
/// </summary>
void closeCameras()
{
	for (int i = 0; i < NumberOfCameras; i++)
	{
		cameras[i].release();
	}
}

/// <summary>
/// 이미지를 출력할 윈도우 가져오기
/// </summary>
/// <param name="windowHandle">카메라 영상을 출력할 윈도우 핸들. C#에서 생성한 윈도우를 가져온다.</param>
/// <param name="shotWindowHandle">캡처한 이미지를 출력할 윈도우 핸들. C#에서 생성한 윈도우를 가져온다.</param>
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
/// 윈도우 해제
/// </summary>
void closeWindow()
{
	ReleaseDC(windowHandle, windowDc);
	ReleaseDC(shotWindowHandle, shotWindowDc);
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
	bool ret = cameras[(int)part].read(frame[(int)part]);
	if (!ret)
	{
		return ErrorShowCameraFrame::CannotRead;
	}

	showImage(frame[(int)part], x, y, width, height, windowDc);

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
	cv::Mat blackImage(height, width, CV_8UC3, BlackScalar);
	showImage(blackImage, x, y, width, height, windowDc);
}

/// <summary>
/// 이미지 처리
/// </summary>
/// <param name="img">처리할 이미지</param>
/// <returns>처리한 이미지</returns>
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
/// 이미지 캡처
/// </summary>
/// <param name="x">캡처한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 x좌표</param>
/// <param name="y">캡처한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 y좌표</param>
/// <param name="width">캡처한 이미지를 출력할 영역의 가로 길이</param>
/// <param name="height">캡처한 이미지를 출력할 영역의 세로 길이</param>
/// <param name="pX">처리한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 x좌표</param>
/// <param name="pY">처리한 이미지를 출력할 영역의 왼쪽 위 꼭짓점 y좌표</param>
/// <param name="pWidth">처리한 이미지를 출력할 영역의 가로 길이</param>
/// <param name="pHeight">처리한 이미지를 출력할 영역의 세로 길이</param>
/// <param name="fileName">처리한 이미지를 저장할 파일</param>
void captureImage(int x, int y, int width, int height, int pX, int pY, int pWidth, int pHeight, char* fileName)
{
	cv::Mat processedImage = processImage(frame[(int)Part::Retina]);
	cv::imwrite(fileName, processedImage);

	showImage(frame[(int)Part::Retina], x, y, width, height, shotWindowDc);
	showImage(processedImage, pX, pY, pWidth, pHeight, shotWindowDc);
}

/// <summary>
/// 카메라 파라메터 설정
/// </summary>
/// <param name="part">망막인지 각막인지</param>
/// <param name="prop">파라메터 종류</param>
/// <param name="value">파라메터 값</param>
/// <returns>에러 여부</returns>
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
/// 카메라 파라메터 값 읽기
/// </summary>
/// <param name="part">망막인지 각막인지</param>
/// <param name="prop">파라메터 종류</param>
/// <returns>파라메터 값</returns>
int getCameraParam(Part part, cv::VideoCaptureProperties prop)
{
	return cameras[(int)part].get(prop);
}