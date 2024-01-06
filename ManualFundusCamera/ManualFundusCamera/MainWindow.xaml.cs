using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Threading;

namespace ManualFundusCamera
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShotWindow shotWindow;
        private CameraParamWindow cameraParamWindow;

        private Thread cameraThread;
        private bool shallExitThread;
        private bool shallCapture;
        private int retinaAreaX;
        private int retinaAreaY;
        private int retinaAreaWidth;
        private int retinaAreaHeight;
        private int corneaAreaX;
        private int corneaAreaY;
        private int corneaAreaWidth;
        private int corneaAreaHeight;
        private IntPtr windowHandle;

        public MainWindow()
        {
            InitializeComponent();
        }

        // 프로그램 시작할 때 실행하는 메소드
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            shotWindow = new ShotWindow(this);
            shotWindow.Show();

            cameraParamWindow = new CameraParamWindow(this);
            cameraParamWindow.Show();

            // 카메라 영상을 출력하는 영역 정보

            retinaAreaX = (int)retinaCameraBorder.Margin.Left;
            retinaAreaY = (int)retinaCameraBorder.Margin.Top;
            retinaAreaWidth = (int)retinaCameraBorder.Width;
            retinaAreaHeight = (int)retinaCameraBorder.Height;

            corneaAreaX = (int)corneaCameraBorder.Margin.Left;
            corneaAreaY = (int)corneaCameraBorder.Margin.Top;
            corneaAreaWidth = (int)corneaCameraBorder.Width;
            corneaAreaHeight = (int)corneaCameraBorder.Height;

            // 카메라 영상을 출력할 윈도우. 즉 현재 윈도우
            windowHandle = new WindowInteropHelper(this).Handle;

            // 카메라를 작동시키는 스레드
            cameraThread = new Thread(doCameraThread);
            cameraThread.Start();
        }

        private void doCameraThread()
        {
            shallExitThread = false;
            shallCapture = false;

            Statics.ErrorInitializeCameras errorInitializeCameras = Statics.initializeCameras();
            if (errorInitializeCameras != Statics.ErrorInitializeCameras.None)
            {
                Console.WriteLine(errorInitializeCameras);
            }

            Statics.initializeWindow(windowHandle, shotWindow.windowHandle);

            // 루프를 돌며 카메라 영상을 출력한다.
            while (!shallExitThread)
            {
                Statics.ErrorShowCameraFrame errorShowCameraFrame = Statics.showCameraFrame(Statics.Part.Retina, retinaAreaX, retinaAreaY, retinaAreaWidth, retinaAreaHeight);
                if (errorShowCameraFrame != Statics.ErrorShowCameraFrame.None)
                {
                    Console.WriteLine(errorShowCameraFrame);
                }

                errorShowCameraFrame = Statics.showCameraFrame(Statics.Part.Cornea, corneaAreaX, corneaAreaY, corneaAreaWidth, corneaAreaHeight);
                if (errorShowCameraFrame != Statics.ErrorShowCameraFrame.None)
                {
                    Console.WriteLine(errorShowCameraFrame);
                }

                if (shallCapture)
                {
                    shotWindow.imageFileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.jpg";
                    Statics.captureImage(shotWindow.rawImageAreaX, shotWindow.rawImageAreaY, shotWindow.rawImageAreaWidth, shotWindow.rawImageAreaHeight, shotWindow.processedImageAreaX, shotWindow.processedImageAreaY, shotWindow.processedImageAreaWidth, shotWindow.processedImageAreaHeight, shotWindow.imageFileName);
                    shallCapture = false;
                    Thread.Sleep(1000);
                }
            }

            Statics.closeCameras();
            Statics.closeWindow();
        }

        // 프로그램 종료시 카메라 루프에서 빠져나온다.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            shallExitThread = true;
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == closeButton)
            {
                // 종료 버튼을 누르면 프로그램을 종료한다.
                Close();
            }
            else if (sender == shotButton)
            {
                // 촬영 버튼을 누르면 망막 카메라를 캡처한다.
                shallCapture = true;
            }
        }
    }
}
