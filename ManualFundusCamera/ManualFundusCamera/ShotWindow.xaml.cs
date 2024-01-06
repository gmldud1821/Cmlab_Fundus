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

        public string imageFileName;

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == uploadImageButton)
            {
                // 업로드 버튼을 누르면 처리한 이미지를 서버로 전송하고, 서버로부터 ID와 비밀 URL을 받아서 QR 코드 생성
                if (imageFileName == null)
                {
                    MessageBox.Show("촬영하지 않았습니다.");
                }
                else if (!File.Exists(imageFileName))
                {
                    MessageBox.Show("가장 최근에 촬영한 이미지 파일이 존재하지 않습니다.");
                }
                else
                {
                    WebClient client = new WebClient();
                    byte[] responseArray = client.UploadFile(Statics.UrlForUploadingImage, imageFileName);
                    string response = Encoding.ASCII.GetString(responseArray);
                    response = response.Replace(Statics.DoubleQuotationMarks, Statics.Blank);
                    string[] splittedResponse = response.Split(Statics.Comma);
                    string id = splittedResponse[0];
                    string url = splittedResponse[1];
                    BarcodeWriter writer = new BarcodeWriter();
                    writer.Format = BarcodeFormat.QR_CODE;
                    writer.Options.Width = (int)qrImageBorder.Width;
                    writer.Options.Height = (int)qrImageBorder.Height;
                    Bitmap bitmap = writer.Write($"{Statics.UrlForGettingImage}{Statics.QuestionMarks}{Statics.Id}{Statics.Equal}{id}{Statics.And}{Statics.Url}{Statics.Equal}{url}");
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    qrImage.Source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
