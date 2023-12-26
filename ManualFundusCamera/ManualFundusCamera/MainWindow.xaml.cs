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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(retinaCameraBorder.Margin);
            Console.WriteLine(retinaCameraBorder.Width);

            cameraThread = new Thread(doCameraThread);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void doCameraThread()
        {
            int retinaAreaX = (int)retinaCameraBorder.Margin.Left;
            int retinaAreaY = (int)retinaCameraBorder.Margin.Top;
            int retinaAreaWidth = (int)retinaCameraBorder.Width;
            int retinaAreaHeight = (int)retinaCameraBorder.Height;

            int corneaAreaX = (int)corneaCameraBorder.Margin.Left;
            int corneaAreaY = (int)corneaCameraBorder.Margin.Top;
            int corneaAreaWidth = (int)corneaCameraBorder.Width;
            int corneaAreaHeight = (int)corneaCameraBorder.Height;

            shallExitThread = false;

            Statics.initializeCameras();
            Statics.initializeWindow(new WindowInteropHelper(this).Handle);

            while (!shallExitThread)
            {
                Statics.showCameraFrame(Statics.Part.Retina, retinaAreaX, retinaAreaY, retinaAreaWidth, retinaAreaHeight);
                Statics.showCameraFrame(Statics.Part.Cornea, corneaAreaX, corneaAreaY, corneaAreaWidth, corneaAreaHeight);
            }

            Statics.closeCameras();
            Statics.closeWindow();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            shallExitThread = true;
            Close();
        }
    }
}
