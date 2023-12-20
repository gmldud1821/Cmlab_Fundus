#include <opencv2/opencv.hpp>

#ifdef _DEBUG
#pragma comment(lib, "opencv_world440d.lib")
#else
#pragma comment(lib, "opencv_world440.lib")
#endif

cv::Mat processImage(cv::Mat& img)
{
	cv::Mat blurredSubstractedImage, result;
	cv::Mat image = img.clone();
	cv::Mat blurredImage;
	cv::GaussianBlur(image, blurredImage, cv::Size(11, 11), 11);      // 21,21,15
	blurredSubstractedImage = 5.0 * image - 4.3 * blurredImage;       // 6 , 4.5
	cv::Mat* bgrImage = new cv::Mat[blurredSubstractedImage.channels()];    // blurredSubstractedImage.channels
	cv::split(blurredSubstractedImage, bgrImage);                           // blurredSubstractedImage
	cv::Ptr<cv::CLAHE> clahe = cv::createCLAHE(2, cv::Size(30, 30));
	cv::Mat* BGRClaheImage = new cv::Mat[image.channels()];
	for (int i = 0; i < image.channels(); i++)
	{
		clahe->apply(bgrImage[i], BGRClaheImage[i]);
	}
	cv::Mat GClaheImage;
	cv::Mat images[] = { bgrImage[0], BGRClaheImage[1], bgrImage[2] };     //BGRClaheImage[1]
	cv::merge(images, image.channels(), GClaheImage);

	cv::Mat bilateralImage;
	cv::bilateralFilter(GClaheImage, bilateralImage, 21, 45, 45);

	cv::split(bilateralImage, bgrImage);
	cv::Mat tempBGRImage[] = { bgrImage[1].clone() * 1.1, bgrImage[1] * 1.2, bgrImage[2] + 60 };    // 1.5     20

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

int main()
{
	cv::Mat image = cv::imread("C:\\Users\\JangSH\\OneDrive\\문서\\카카오톡 받은 파일\\KakaoTalk_20231129_095848346_03.jpg");

	cv::Mat processedImage = processImage(image);

	cv::resize(image, image, cv::Size(), 0.25, 0.25);
	cv::resize(processedImage, processedImage, cv::Size(), 0.25, 0.25);

	cv::imshow("image", image);
	cv::imshow("processedImage", processedImage);

	int ret = cv::waitKey(0);

	return 0;
}