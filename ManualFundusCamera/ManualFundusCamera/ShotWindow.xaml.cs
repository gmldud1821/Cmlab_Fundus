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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
    }
}
