package com.example.selectandprocessimage

import android.content.Intent
import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.provider.MediaStore
import com.example.selectandprocessimage.databinding.ActivityMainBinding
import org.opencv.android.OpenCVLoader

class MainActivity : AppCompatActivity() {
    private lateinit var binding: ActivityMainBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        // OpenCV 로드. OpenCV 라이브러리 사용하기 전에 반드시 실행해야 함.
        OpenCVLoader.initDebug()

        binding.imageView.setOnClickListener {
            val intent = Intent(Intent.ACTION_PICK)
            intent.data = MediaStore.Images.Media.EXTERNAL_CONTENT_URI
            intent.type = "image/*"
            startActivityForResult(intent, 0)
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)

        if (requestCode == 0) {
            val proj = arrayOf(MediaStore.Files.FileColumns.DATA)
            val cursor = data?.data?.let { contentResolver.query(it, proj, null, null, null) }
            cursor?.moveToNext()
            val index = cursor?.getColumnIndex(proj[0])
            val path = index?.let { cursor?.getString(it) }
            cursor?.close()

            // 이미지 불러오기
            val imageBitmap = BitmapFactory.decodeFile(path)
            // 이미지 처리하는 자바 메소드
            val bilateralImageBitmap = ImageProcessor.processImage1(imageBitmap)

            binding.imageView.setImageBitmap(imageBitmap)
            binding.bilateralImageView.setImageBitmap(bilateralImageBitmap)
        }
    }
}