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
        private Thread cameraThread;
        private bool shallExitThread;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            retinaAreaX = (int)retinaCameraBorder.Margin.Left;
            retinaAreaY = (int)retinaCameraBorder.Margin.Top;
            retinaAreaWidth = (int)retinaCameraBorder.Width;
            retinaAreaHeight = (int)retinaCameraBorder.Height;

            corneaAreaX = (int)corneaCameraBorder.Margin.Left;
            corneaAreaY = (int)corneaCameraBorder.Margin.Top;
            corneaAreaWidth = (int)corneaCameraBorder.Width;
            corneaAreaHeight = (int)corneaCameraBorder.Height;

            windowHandle = new WindowInteropHelper(this).Handle;

            cameraThread = new Thread(doCameraThread);
            cameraThread.Start();
        }

        private void doCameraThread()
        {
            shallExitThread = false;

            Statics.ErrorInitializeCameras errorInitializeCameras = Statics.initializeCameras();
            if (errorInitializeCameras != Statics.ErrorInitializeCameras.None)
            {
                Console.WriteLine(errorInitializeCameras);
            }

            Statics.initializeWindow(windowHandle);

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
            }

            Statics.closeCameras();
            Statics.closeWindow();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            shallExitThread = true;
        }
    }
}
