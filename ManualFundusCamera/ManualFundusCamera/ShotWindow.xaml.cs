using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZXing;

namespace ManualFundusCamera
{
    /// <summary>
    /// ShotWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShotWindow : Window
    {
        private MainWindow mainWindow;

        public int rawImageAreaX;
        public int rawImageAreaY;
        public int rawImageAreaWidth;
        public int rawImageAreaHeight;
        public int processedImageAreaX;
        public int processedImageAreaY;
        public int processedImageAreaWidth;
        public int processedImageAreaHeight;
        public IntPtr windowHandle;

        public ShotWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 캡처한 이미지를 출력할 영역 정보

            rawImageAreaX = (int)rawImageBorder.Margin.Left;
            rawImageAreaY = (int)rawImageBorder.Margin.Top;
            rawImageAreaWidth = (int)rawImageBorder.Width;
            rawImageAreaHeight = (int)rawImageBorder.Height;

            processedImageAreaX = (int)processedImageBorder.Margin.Left;
            processedImageAreaY = (int)processedImageBorder.Margin.Top;
            processedImageAreaWidth = (int)processedImageBorder.Width;
            processedImageAreaHeight = (int)processedImageBorder.Height;

            // 윈도우 핸들
            windowHandle = new WindowInteropHelper(this).Handle;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void uploadImage(string imageFile)
        {
            WebClient client = new WebClient();
            byte[] responseArray = client.UploadFile(Statics.urlForUploadingImage, imageFile);
            string response = Encoding.ASCII.GetString(responseArray);
            response = response.Replace("\"", "");
            string[] splittedResponse = response.Split(',');
            string id = splittedResponse[0];
            string url = splittedResponse[1];
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            Dispatcher.Invoke(() =>
            {
                writer.Options.Width = (int)qrImageBorder.Width;
                writer.Options.Height = (int)qrImageBorder.Height;
                Bitmap bitmap = writer.Write($"http://175.112.57.221:1114/getRetinaImage?id={id}&url={url}");
                IntPtr hBitmap = bitmap.GetHbitmap();
                qrImage.Source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            });
        }
    }
}
