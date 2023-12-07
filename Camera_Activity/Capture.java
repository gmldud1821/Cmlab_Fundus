package com.example.fundus;

import static android.Manifest.permission.CAMERA;
import static android.Manifest.permission.READ_EXTERNAL_STORAGE;
import static android.Manifest.permission.WRITE_EXTERNAL_STORAGE;


import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.AppCompatButton;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import android.content.ContentResolver;
import android.content.ContentValues;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.hardware.Camera;
import android.hardware.camera2.CameraManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.provider.MediaStore;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.WindowManager;
import android.widget.FrameLayout;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.ToggleButton;

import com.google.android.material.floatingactionbutton.FloatingActionButton;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.OutputStream;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

public class Capture extends AppCompatActivity {

    //권한
    private final int REQUEST_CODE_PERMISSIONS = 1001;
    private final String[] REQUIRED_PERMISSIONS = new String[]{"android.permission.CAMERA", "android.permission.WRITE_EXTERNAL_STORAGE"};


    static {
        System.loadLibrary("gpio");
    }

    public Constants.ShotMode shotMode;


    //camera 소스
    public CameraSurfaceView cameraSurfaceView;

    //저장 리졸버
    public  ContentResolver contentResolver;


    //레이아웃 소스
    public ToggleButton single_toggle, continuous_toggle, nir_toggle, white_toggle, left_toggle, right_toggle;
    public AppCompatButton motor_left_button, motor_right_button, up_button, down_button, capture_back_button;
    public ImageButton capture_button;
    public TextView led_value_text, patient_info_text;

    public ImageView ivBitmap; // image view bitmap (result image)
    public LinearLayout llBottom; // 버튼 있는 리니어 레이아웃.
    public FloatingActionButton btnReject, btnAccept;

   //byte[] currentData;

    Bitmap bitmap;

    int NirValue = 0;
    int WhiteValue = 0;

    private Boolean Running = true;



    Thread TimeThread = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);//하단바 없애기
        setContentView(R.layout.activity_capture);

        //액티비티 부여
        Constants.capture = this;
        //권한
        requestPermission();
        //카메라 class 선언
        cameraSurfaceView = new CameraSurfaceView(this);
        //View 설정.
        ((FrameLayout) findViewById(R.id.camera_surfaceView)).addView(cameraSurfaceView);

        //변수 ID 지정--------------------------------------------------------------------------------
        single_toggle = findViewById(R.id.single_toggle);
        continuous_toggle = findViewById(R.id.continuous_toggle);
        nir_toggle = findViewById(R.id.nir_toggle);
        white_toggle = findViewById(R.id.white_toggle);
        left_toggle = findViewById(R.id.left_toggle);
        right_toggle= findViewById(R.id.right_toggle);

        motor_left_button = findViewById(R.id.motor_left_button);
        motor_right_button = findViewById(R.id.motor_right_button);
        up_button = findViewById(R.id.up_button);
        down_button = findViewById(R.id.down_button);
        capture_back_button = findViewById(R.id.capture_back_button);
        led_value_text = findViewById(R.id.led_value_text);
        patient_info_text = findViewById(R.id.patient_info_text);

        capture_button = findViewById(R.id.capture_button);

        ivBitmap = findViewById(R.id.ivBitmap);
        llBottom = findViewById(R.id.llBottom);
        btnReject = findViewById(R.id.btnReject);
        btnAccept = findViewById(R.id.btnAccept);

        //리스너 지정-------------------------------------------------------------------------------
        capture_button.setOnClickListener(button_listener);
        capture_back_button.setOnClickListener(button_listener);
        btnAccept.setOnClickListener(button_listener);
        btnReject.setOnClickListener(button_listener);

        single_toggle.setOnClickListener(toggle_listener);
        continuous_toggle.setOnClickListener(toggle_listener);
        nir_toggle.setOnClickListener(toggle_listener);
        white_toggle.setOnClickListener(toggle_listener);
        left_toggle.setOnClickListener(toggle_listener);
        right_toggle.setOnClickListener(toggle_listener);

        motor_left_button.setOnTouchListener(motor_listener);
        motor_right_button.setOnTouchListener(motor_listener);
        up_button.setOnTouchListener(led_listener);
        down_button.setOnTouchListener(led_listener);


        //상태 변수 지정
        shotMode = Constants.ShotMode.Single;
    }


    //촬영 및 저장 ------------------------------------------------------------------------------------------------------------------------------------------------

    public void takePic(){
        if (white_toggle.isChecked()){
            Toast.makeText(getApplicationContext(), "Please white OFF And Nir ON", Toast.LENGTH_SHORT).show();
        }
        else{
            // 샷 + White 팡
            cameraSurfaceView.camera.takePicture(shutterCallback, rawCallback, jpegCallback);

            byte[] data1 = {(byte)'L', (byte) 'W', (byte) WhiteValue};
            Constants.splashActivity.serialPortUtil.sendData(data1);
        }
    }

    int pic_count = 0;



    Camera.ShutterCallback shutterCallback = new Camera.ShutterCallback() {
        public void onShutter() {


        }
    };
    Camera.PictureCallback rawCallback = new Camera.PictureCallback() {
        public void onPictureTaken(byte[] data, Camera camera) {
            //Log.d(TAG, "onPictureTaken - raw with data = " + ((data != null) ? data.length : " NULL"));

        }
    };

    byte[] image_data;

    Camera.PictureCallback jpegCallback = new Camera.PictureCallback() {
        public void onPictureTaken(byte[] data, Camera camera) {

            if (shotMode == Constants.ShotMode.Single) {
                //White 꺼주기
                byte[] data1 = {(byte) 'L', (byte) 'W', (byte) 0};
                Constants.splashActivity.serialPortUtil.sendData(data1);

                // 이미지 크기 설정.
                int w = camera.getParameters().getPictureSize().width;
                int h = camera.getParameters().getPictureSize().height;
                //int orientation = calculatePreviewOrientation(mCameraInfo, mDisplayOrientation);

                image_data = data;

                //byte array 를 bitmap 으로 변환
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.inPreferredConfig = Bitmap.Config.ARGB_8888;
                bitmap = BitmapFactory.decodeByteArray(data, 0, data.length, options);

                //이미지를 디바이스 방향으로 회전
                Matrix matrix = new Matrix();
                matrix.setRotate(180);


                bitmap = Bitmap.createBitmap(bitmap, 0, 0, w, h, matrix, true);
                ivBitmap.setImageBitmap(bitmap);
                showAcceptedRejectedButton(true);

                Toast.makeText(getApplicationContext(), "Capture Success", Toast.LENGTH_SHORT).show();
            }
            else if (shotMode == Constants.ShotMode.Continuous){
                Log.d("연속촬영", "촬영 한 루트 돌았음.");
                pic_count++;
                if (pic_count <= 10 ){
                    Log.d("연속촬영", "촬영 시작.");
                    byteList.add(data);
                    cameraSurfaceView.camera.startPreview();
                    cameraSurfaceView.camera.takePicture(shutterCallback, rawCallback, jpegCallback);

                }
                else if (pic_count > 10){
                    Toast.makeText(getApplicationContext(),"Saving Images. Please Wait.", Toast.LENGTH_LONG).show();
                    for (int i = 0; i < byteList.size(); i++){
                        Log.d("연속촬영", "변환 중" + String.valueOf(i));
                        image_data = byteList.get(i);
                        con_bitmap = BitmapFactory.decodeByteArray(image_data, 0, image_data.length);
                        //이미지를 디바이스 방향으로 회전
                        Matrix matrix = new Matrix();
                        matrix.setRotate(180);
                        con_bitmap = Bitmap.createBitmap(con_bitmap, 0, 0, 3264, 2448, matrix, true);
                        bitmapList.add(con_bitmap);

                    }

                    image_data = null;
                    con_bitmap = null;
                    pic_count = 0;
                    byteList.clear();

                    ivBitmap.setImageBitmap(bitmapList.get(0));
                    showAcceptedRejectedButton(true);
                    /*saveAllImages();*/
                }

            }
        }
    };


    //촬영 후 X 버튼, 체크 버튼 설정
    private void showAcceptedRejectedButton(boolean show) {
        if (show == true) {

            //저장 선택 여부 버튼
            llBottom.setVisibility(View.VISIBLE);

            motor_left_button.setVisibility(View.GONE);
            motor_right_button.setVisibility(View.GONE);
            left_toggle.setVisibility(View.GONE);
            right_toggle.setVisibility(View.GONE);
            up_button.setVisibility(View.GONE);
            down_button.setVisibility(View.GONE);
            capture_button.setVisibility(View.GONE);
        } else {

            ivBitmap.setVisibility(View.INVISIBLE);
            llBottom.setVisibility(View.GONE);

            motor_left_button.setVisibility(View.VISIBLE);
            motor_right_button.setVisibility(View.VISIBLE);
            left_toggle.setVisibility(View.VISIBLE);
            right_toggle.setVisibility(View.VISIBLE);
            up_button.setVisibility(View.VISIBLE);
            down_button.setVisibility(View.VISIBLE);
            capture_button.setVisibility(View.VISIBLE);

            cameraSurfaceView.camera.startPreview();

        }
    }

    private void SaveImage(Bitmap result_bitmap){

        ContentValues values = new ContentValues();

        values.put(MediaStore.Images.Media.RELATIVE_PATH,"Pictures/" + "patient" );

        String fileName = new SimpleDateFormat("yyyy-MM-dd_HH-mm", Locale.US).format(new Date());
        values.put(MediaStore.Images.Media.DISPLAY_NAME, fileName+".jpg");

        values.put(MediaStore.Images.Media.MIME_TYPE, "image/jpg");
        values.put(MediaStore.Images.Media.DATE_ADDED, System.currentTimeMillis()/1000);
        values.put(MediaStore.Images.Media.DATE_TAKEN, System.currentTimeMillis());
        values.put(MediaStore.Images.Media.ORIENTATION,180);

        contentResolver = getContentResolver();
        Uri uri = contentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values);

        if (uri != null){
            OutputStream outputStream = null;
            try {
                outputStream = contentResolver.openOutputStream(uri);

                result_bitmap.compress(Bitmap.CompressFormat.JPEG,100,outputStream);
                outputStream.close();


            } catch (FileNotFoundException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }


    boolean ConStart = false;
    Bitmap con_bitmap;


    private List<byte[]> byteList = new ArrayList<>();
    private List<Bitmap> bitmapList = new ArrayList<>();

    public void takePic_con(){

        cameraSurfaceView.camera.takePicture(shutterCallback, rawCallback, jpegCallback);
    }


    private void saveAllImages() {

        ContentValues values = new ContentValues();

        //파일 경로
        values.put(MediaStore.Images.Media.RELATIVE_PATH,"Pictures/"+ "patient");

        for (int i = 0; i < bitmapList.size(); i++) {


            String ImageName = new SimpleDateFormat("yyyy-mm-dd_HH-mm", Locale.KOREA).format(new Date());
            values.put(MediaStore.Images.Media.DISPLAY_NAME,ImageName+ String.valueOf(i) + ".jpg");
            values.put(MediaStore.Images.Media.MIME_TYPE,"image/jpg");
            values.put(MediaStore.Images.Media.DATE_ADDED,System.currentTimeMillis()/1000);
            values.put(MediaStore.Images.Media.DATE_TAKEN,System.currentTimeMillis());
            //values.put(MediaStore.Images.Media.ORIENTATION,180);

            contentResolver = getContentResolver();
            Uri uri = contentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI,values);

            try {
                OutputStream outputStream = contentResolver.openOutputStream(uri);
                bitmapList.get(i).compress(Bitmap.CompressFormat.JPEG, 100, outputStream);

                outputStream.close();

            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        Toast.makeText(getApplicationContext(),"사진 저장 완료", Toast.LENGTH_SHORT).show();


        // 이미지 저장 후 비트맵 리스트를 초기화.
        byteList.clear();

        bitmapList.clear();


        cameraSurfaceView.camera.startPreview();
    }



// 버튼 리스너 -------------------------------------------------------------------------------------------------------
    View.OnClickListener button_listener = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            switch (v.getId()){
                case R.id.capture_button:{
                    if (shotMode == Constants.ShotMode.Single) {
                        //NIR 먼저 꺼주기
                        byte[] data = {(byte) 'L', (byte) 'N', (byte) 0};
                        Constants.splashActivity.serialPortUtil.sendData(data);

                        takePic();
                    }
                    else if (shotMode == Constants.ShotMode.Continuous){
                        takePic_con();
                    }

                }
                break;

                case R.id.capture_back_button:{
                    Intent intent = new Intent(getApplicationContext(), MainActivity.class);
                    startActivity(intent);
                    finish();
                }
                break;

                case R.id.btnReject: {
                    showAcceptedRejectedButton(false);
                    bitmap = null;
                }
                break;

                case R.id.btnAccept:{
                    if (shotMode == Constants.ShotMode.Single) {
                        if (bitmap == null) {
                           // 사진 없음
                        } else {

                            SaveImage(bitmap);
                            showAcceptedRejectedButton(false);
                        }
                    }
                    else if (shotMode == Constants.ShotMode.Continuous){
                        if (bitmapList == null) {
                            // 사진 없음.
                        } else {
                            saveAllImages();
                            showAcceptedRejectedButton(false);
                        }
                    }
                }
                break;
            }

        }
    };

    View.OnTouchListener led_listener = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            switch (v.getId()){
                case R.id.up_button:{
                    switch (event.getAction()){
                        case MotionEvent.ACTION_DOWN:{
                            if (nir_toggle.isChecked()){
                                NirValue++;
                                 if (NirValue >= 15){
                                     NirValue = 15;
                                 }
                                 led_value_text.setText("NIR: " + String.valueOf(NirValue));
                                byte[] data = {(byte)'L', (byte) 'N', (byte) NirValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                            else if (white_toggle.isChecked()){
                                WhiteValue++;
                                if (WhiteValue >= 15){
                                    WhiteValue = 15;
                                }
                                led_value_text.setText("WHITE: " + String.valueOf(WhiteValue));
                                byte[] data = {(byte)'L', (byte) 'W', (byte) WhiteValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                        }break;
                        case MotionEvent.ACTION_UP:{
                            if (nir_toggle.isChecked()){
                                byte[] data = {(byte)'L', (byte) 'N', (byte) NirValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                            else if (white_toggle.isChecked()){
                                byte[] data = {(byte)'L', (byte) 'W', (byte) WhiteValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                        }
                        break;
                    }
                }
                break;

                case R.id.down_button:{
                    switch (event.getAction()){
                        case MotionEvent.ACTION_DOWN:{
                            if (nir_toggle.isChecked()){
                                NirValue--;
                                if (NirValue <= 0){
                                    NirValue = 0;
                                }
                                led_value_text.setText("NIR: " + String.valueOf(NirValue));
                                byte[] data = {(byte)'L', (byte) 'N', (byte) NirValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                            else if (white_toggle.isChecked()){
                                WhiteValue--;
                                if (WhiteValue <= 0){
                                    WhiteValue = 0;
                                }
                                led_value_text.setText("WHITE: " + String.valueOf(WhiteValue));
                                byte[] data = {(byte)'L', (byte) 'W', (byte) WhiteValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                        }break;
                        case MotionEvent.ACTION_UP:{
                            if (nir_toggle.isChecked()){
                                byte[] data = {(byte)'L', (byte) 'N', (byte) NirValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                            else if (white_toggle.isChecked()){
                                byte[] data = {(byte)'L', (byte) 'W', (byte) WhiteValue};
                                Constants.splashActivity.serialPortUtil.sendData(data);
                            }
                        }
                        break;
                    }

                }
                break;
            }
            return false;
        }
    };

    View.OnTouchListener motor_listener = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View v, MotionEvent event) {
            switch (v.getId()){
                case R.id.motor_left_button:{
                    switch (event.getAction()){
                        case MotionEvent.ACTION_DOWN:{

                            byte[] data = {(byte)'M', (byte) 'L', (byte) 1};
                            Constants.splashActivity.serialPortUtil.sendData(data);

                        }break;

                        case MotionEvent.ACTION_UP:{

                            byte[] data = {(byte)'M', (byte) 'L', (byte) 0};
                            Constants.splashActivity.serialPortUtil.sendData(data);

                        }
                        break;
                    }
                }
                break;

                case R.id.motor_right_button:{
                    switch (event.getAction()){
                        case MotionEvent.ACTION_DOWN:{

                            byte[] data = {(byte)'M', (byte) 'R', (byte) 1};
                            Constants.splashActivity.serialPortUtil.sendData(data);

                        }break;

                        case MotionEvent.ACTION_UP:{

                            byte[] data = {(byte)'M', (byte) 'R', (byte) 0};
                            Constants.splashActivity.serialPortUtil.sendData(data);

                        }
                        break;
                    }
                }
                break;
            }
            return false;
        }
    };



    View.OnClickListener toggle_listener = new View.OnClickListener() {
        @Override
        public void onClick(View v) {
            boolean checked = ((ToggleButton) v).isChecked();
            switch (v.getId()){
                case R.id.single_toggle:{
                    if (checked){
                        shotMode = Constants.ShotMode.Single;
                        single_toggle.setChecked(true);
                        continuous_toggle.setChecked(false);
                    }
                    else{
                        shotMode = Constants.ShotMode.Continuous;
                        single_toggle.setChecked(false);
                        continuous_toggle.setChecked(true);
                    }
                }
                break;

                case R.id.continuous_toggle:{
                    if (checked){
                        shotMode = Constants.ShotMode.Continuous;
                        single_toggle.setChecked(false);
                        continuous_toggle.setChecked(true);
                    }
                    else{
                        shotMode = Constants.ShotMode.Single;
                        single_toggle.setChecked(true);
                        continuous_toggle.setChecked(false);
                    }
                }
                break;

                case R.id.nir_toggle:{
                    if (checked){
                        led_value_text.setText("NIR: " + String.valueOf(NirValue));

                        nir_toggle.setChecked(true);
                        white_toggle.setChecked(false);

                        //Nir 값 전송
                        byte[] data = {(byte)'L', (byte) 'N', (byte) NirValue};
                        Constants.splashActivity.serialPortUtil.sendData(data);
                    }
                    else{
                        led_value_text.setText("WHITE: " + String.valueOf(WhiteValue));

                        nir_toggle.setChecked(false);
                        white_toggle.setChecked(true);

                        byte[] data = {(byte)'L', (byte) 'N', (byte) 0};
                        Constants.splashActivity.serialPortUtil.sendData(data);
                    }
                }
                break;

                case R.id.white_toggle:{
                    if (checked){
                        led_value_text.setText("WHITE: " + String.valueOf(WhiteValue));

                        nir_toggle.setChecked(false);
                        white_toggle.setChecked(true);


                        //White 값 전송
                        byte[] data = {(byte)'L', (byte) 'W', (byte) WhiteValue};
                        Constants.splashActivity.serialPortUtil.sendData(data);
                    }
                    else{
                        led_value_text.setText("Nir: " + String.valueOf(NirValue));

                        nir_toggle.setChecked(true);
                        white_toggle.setChecked(false);

                        //White 끔 전송
                        byte[] data = {(byte)'L', (byte) 'W', (byte) 0};
                        Constants.splashActivity.serialPortUtil.sendData(data);

                    }
                }
                break;

                case R.id.left_toggle:{
                    if (checked){
                        mEyeIdx = false;
                        //ivBitmap.setImageBitmap(mLeft);
                    }
                    else{
                        mEyeIdx = true;
                        //ivBitmap.setImageBitmap(mRight);
                    }

                }
                break;

                case R.id.right_toggle:{
                    if (checked){
                        mEyeIdx = true;
                        ivBitmap.setImageBitmap(mRight);
                    }
                    else{
                        mEyeIdx = false;
                        ivBitmap.setImageBitmap(mLeft);
                    }
                }
                break;
            }

        }
    };

    Bitmap mRight, mLeft;
    boolean mFlag = false;
    // True Right
    // False Left
    boolean mEyeIdx = false;

    public enum Eye{
        Left, Right
    }
    private void createMask(){
        //mRight = createCircle(Eye.Right,3264, 2448);
        //mLeft = createCircle(Eye.Left,3264,2448);
        mFlag = true;
        if (!mEyeIdx){
            ivBitmap.setImageBitmap(mLeft);
        }
        else{
            ivBitmap.setImageBitmap(mRight);
        }
    }




    //연속 촬영...? -----------------------------------------------
    boolean Fin = false;
    boolean pic = true;
    int i = 0;
    public class PicThread implements Runnable{
        @Override
        public void run() {
            while(true){
                while (pic){
                    Message msg = new Message();
                    i++;
                    if (ConStart){
                        if (i % 10 == 0){
                            pic_count++;
                            if (pic_count < 9){
                                Fin = false;
                                cameraSurfaceView.camera.takePicture(shutterCallback, rawCallback, jpegCallback);
                            }
                            else if (pic_count >= 9){
                                Fin = true;
                                cameraSurfaceView.camera.takePicture(shutterCallback, rawCallback, jpegCallback);
                                ConStart = false;
                                pic_count = 0;
                                TimeThread.interrupt();
                            }
                        }
                    }

                    try {
                        Thread.sleep(10);
                    }
                    catch (InterruptedException e){
                        e.printStackTrace();
                        return;
                    }
                }
            }
        }
    }



    //권한 설정 --------------------------------------------------------------------------------------------------------------------------------------------------
    public void requestPermission() {
        ActivityCompat.requestPermissions(this, new String[]{CAMERA,READ_EXTERNAL_STORAGE,WRITE_EXTERNAL_STORAGE}, REQUEST_CODE_PERMISSIONS);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == REQUEST_CODE_PERMISSIONS){
            if (allPermissionsGranted()){
            }
            else{
                Toast.makeText(this, "사용자가 권한을 부여하지 않음.", Toast.LENGTH_SHORT).show();
                this.finish();
            }
        }
    }

    private boolean allPermissionsGranted(){
        for (String permission : REQUIRED_PERMISSIONS){
            if (ContextCompat.checkSelfPermission(this, permission) != PackageManager.PERMISSION_GRANTED){
                return false;
            }
        }
        return true;
    }


    //하단바 숨김
    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        if (hasFocus) {
            getWindow().getDecorView().setSystemUiVisibility(
                    View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                            | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                            | View.SYSTEM_UI_FLAG_FULLSCREEN
                            | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY);
        }
    }
}