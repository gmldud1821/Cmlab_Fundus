package com.example.selectandprocessimage;

import android.graphics.Bitmap;

import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.CLAHE;
import org.opencv.imgproc.Imgproc;

import java.util.ArrayList;

public class ImageProcessor {
    // opencv를 사용하기 위해 build.gradle에 다음을 추가해야 합니다.
    // implementation 'com.quickbirdstudios:opencv:4.5.3.0'

    // 이미지 처리하는 메소드
    public static Bitmap processImage(Bitmap bitmap) {
        // 비트맵을 Mat으로 변환
        Mat image = new Mat();
        Utils.bitmapToMat(bitmap, image);
        Imgproc.cvtColor(image, image, Imgproc.COLOR_RGBA2RGB);

        // 가우시안 블러링 수행. 이미지를 흐리게 하는 것. 숫자를 조절하면 흐리게 하는 정도가 바뀐다. 많이 흐릴수록 결과 이미지가 더 선명해지지만 노이즈도 선명해지니 주의.
        Mat gaussianBlurImage = new Mat();
        Size ksize = new Size(7.0, 7.0);
        Imgproc.GaussianBlur(image, gaussianBlurImage, ksize, 10.0);

        // 원본 이미지에서 가우시안 블러링한 이미지를 뺀다. 이미지를 선명하게 하는 것. k값이 클수록 더 선명해지지만 노이즈도 선명해지니 주의.
        int k = 2;
        Mat unsharpMaskImage = new Mat();
        Core.addWeighted(image, k + 1, gaussianBlurImage, -k, 0.0, unsharpMaskImage);

        // 노이즈 제거
        Mat bilateralImage = new Mat();
        Imgproc.bilateralFilter(unsharpMaskImage, bilateralImage, 10, 10.0, 10.0);

        // Mat을 비트맵으로 변환
        Bitmap bilateralImageBitmap = Bitmap.createBitmap(
                bilateralImage.cols(),
                bilateralImage.rows(),
                Bitmap.Config.ARGB_8888
        );
        Utils.matToBitmap(bilateralImage, bilateralImageBitmap);

        return bilateralImageBitmap;
    }

    public static Bitmap processImage1(Bitmap bitmap) {
        // 비트맵을 Mat으로 변환
        Mat image = new Mat();
        Utils.bitmapToMat(bitmap, image);
        Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2BGR);

        Mat gaussianBlurImage = new Mat();
        Size ksize = new Size(11.0, 11.0);
        Imgproc.GaussianBlur(image, gaussianBlurImage, ksize, 11.0);

        Mat blurredSubtractedImage = new Mat();
        Core.addWeighted(image, 5, gaussianBlurImage, -4.3, 0.0, blurredSubtractedImage);

        ArrayList<Mat> bgrImage = new ArrayList<>(blurredSubtractedImage.channels());
        Core.split(blurredSubtractedImage, bgrImage);

        CLAHE clahe = Imgproc.createCLAHE(2, new Size(30, 30));
        Mat[] bgrClaheImage = new Mat[image.channels()];
        for (int i = 0; i < image.channels(); i++) {
            bgrClaheImage[i] = new Mat();
            clahe.apply(bgrImage.get(i), bgrClaheImage[i]);
        }

        Mat gClaheImage = new Mat();
        ArrayList<Mat> images = new ArrayList<>();
        images.add(bgrImage.get(0));
        images.add(bgrClaheImage[1]);
        images.add(bgrImage.get(2));
        Core.merge(images, gClaheImage);

        Mat bilateralImage = new Mat();
        Imgproc.bilateralFilter(gClaheImage, bilateralImage, 21, 45, 45);

        Core.split(bilateralImage, bgrImage);
        Mat[] bgrMulImage = new Mat[3];
        for (int i = 0; i < 3; i++) {
            bgrMulImage[i] = new Mat();
        }
        Core.multiply(bgrImage.get(1), new Scalar(1.1), bgrMulImage[0]);
        Core.multiply(bgrImage.get(1), new Scalar(1.2), bgrMulImage[1]);
        Core.add(bgrImage.get(2), new Scalar(60), bgrMulImage[2]);
        Mat[] tempBgrImage = {bgrMulImage[0], bgrMulImage[1], bgrMulImage[2]};

        Mat mergedImage1 = new Mat();
        Mat mergedImage2 = new Mat();
        images.set(0, bgrImage.get(0));
        images.set(1, bgrImage.get(1));
        images.set(2, tempBgrImage[2]);
        Core.merge(images, mergedImage1);

        images.set(0, tempBgrImage[0]);
        images.set(1, tempBgrImage[1]);
        Core.merge(images, mergedImage2);

        Mat mergedImage = new Mat();
        Core.addWeighted(mergedImage1, 0.15, mergedImage2, 0.9, 0, mergedImage);

        // Mat을 비트맵으로 변환
        Bitmap mergedImageBitmap = Bitmap.createBitmap(
                bilateralImage.cols(),
                bilateralImage.rows(),
                Bitmap.Config.ARGB_8888
        );
        Utils.matToBitmap(mergedImage, mergedImageBitmap);

        return mergedImageBitmap;
    }
}
