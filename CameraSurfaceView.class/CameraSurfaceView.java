package com.example.fundus;

import android.content.Context;
import android.hardware.Camera;
import android.hardware.camera2.CameraCharacteristics;
import android.util.AttributeSet;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.hardware.camera2.CameraAccessException;
import android.hardware.camera2.CameraDevice;
import android.hardware.camera2.CameraManager;
import android.hardware.camera2.CaptureRequest;

import androidx.annotation.NonNull;

import java.io.IOException;
import java.util.List;

public class CameraSurfaceView extends SurfaceView implements SurfaceHolder.Callback {

    public SurfaceHolder surfaceHolder;
    public Camera camera;

    private CameraManager cameraManager;
    private CameraDevice cameraDevice;
    private CaptureRequest.Builder captureRequestBuilder;



    public CameraSurfaceView(Context context) {
        super(context);

        init(context);
    }

    public CameraSurfaceView(Context context, AttributeSet attrs) {
        super(context, attrs);

        init(context);
    }

    // SurfaceHolder 를 불러옴. 초기화를 위한 메서드
    public void init(Context context){

        //surfaceVie 의 SurfaceHolder 인스턴스를 연결해주고 있는 것읋 확인할 수 있음.
        surfaceHolder = getHolder();
        surfaceHolder.addCallback(this);
        //camera.setDisplayOrientation(90);
        // 버퍼 타입으로 추가
        //surfaceHolder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);
    }

    @Override
    public void surfaceCreated(@NonNull SurfaceHolder holder) {

        // 카메라 객체를 참조하여 변수에 할당
        camera = Camera.open();
       // camera.getParameters().setAutoExposureLock(true);






        try {
            // Camera 객체에 이 서피스뷰를 미리보기로 하도록 설정
            camera.setPreviewDisplay(holder);

        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    String valid_value_from_list;
    @Override
    public void surfaceChanged(@NonNull SurfaceHolder holder, int format, int width, int height) {

        Camera.Parameters parameters = camera.getParameters();


        //사진 해상도 설정
        parameters.setPictureSize(3264,2448);

        camera.setDisplayOrientation(180);

       /* Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
        Camera.getCameraInfo(0,cameraInfo);
        camera = getCameraInstance();*/


        try {
            camera.setPreviewDisplay(holder);
        } catch (IOException e) {
            e.printStackTrace();
        }
        parameters.setRotation(180);

        camera.setParameters(parameters);

        camera.startPreview();

    }

    /**Camera 개체의 인스턴스(instance)를 얻는 안전한 방법.. */
    public static Camera getCameraInstance(){
        Camera c = null;
        try {
            c = Camera.open(); // Camera 인스턴스(instance)를 가져오려고 합니다
        }
        catch (Exception e){
            // 카메라를 사용할 수 없습니다(사용 중 또는 존재하지 않음)
        }
        return c; // returns null if camera is unavailable
    }

    @Override
    public void surfaceDestroyed(@NonNull SurfaceHolder holder) {
        camera.stopPreview();
        camera.release();
        camera = null;
    }

    // camera 미리보기 중인 화면을 캡처
    public boolean capture(Camera.PictureCallback callback){
        if(camera != null){
            camera.takePicture(null, null, callback);
            return true;
        } else {
            return false;
        }
    }




}
