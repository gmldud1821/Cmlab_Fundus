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
using System.Windows.Shapes;

namespace ManualFundusCamera
{
    /// <summary>
    /// CameraParamWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CameraParamWindow : Window
    {
        private MainWindow mainWindow;

        public CameraParamWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_BRIGHTNESS);
            retinaBrightnessTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_BRIGHTNESS);
            corneaBrightnessTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_CONTRAST);
            retinaContrastTextBox.Text = value.ToString();
            
            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_CONTRAST);
            corneaContrastTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_SATURATION);
            retinaSaturationTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_SATURATION);
            corneaSaturationTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_HUE);
            retinaHueTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_HUE);
            corneaHueTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_GAIN);
            retinaGainTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_GAIN);
            corneaGainTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_EXPOSURE);
            retinaExposureTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_EXPOSURE);
            corneaExposureTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_BLUE_U);
            retinaWhiteBalanceBlueTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_BLUE_U);
            corneaWhiteBalanceBlueTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_RED_V);
            retinaWhiteBalanceRedTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_RED_V);
            corneaWhiteBalanceRedTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_SHARPNESS);
            retinaSharpnessTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_SHARPNESS);
            corneaSharpnessTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_GAMMA);
            retinaGammaTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_GAMMA);
            corneaGammaTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_FOCUS);
            retinaFocusTextBox.Text = value.ToString();

            value = Statics.getCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_FOCUS);
            corneaFocusTextBox.Text = value.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void retinaBrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaBrightnessTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_BRIGHTNESS, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaBrightnessTextBox.Text = value.ToString();
        }

        private void corneaBrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaBrightnessTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_BRIGHTNESS, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaBrightnessTextBox.Text = value.ToString();
        }

        private void retinaContrastButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaContrastTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_CONTRAST, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaContrastTextBox.Text = value.ToString();
        }

        private void corneaContrastButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaContrastTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_CONTRAST, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaContrastTextBox.Text = value.ToString();
        }

        private void retinaSaturationButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaSaturationTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_SATURATION, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaSaturationTextBox.Text = value.ToString();
        }

        private void corneaSaturationButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaSaturationTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_SATURATION, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaSaturationTextBox.Text = value.ToString();
        }

        private void retinaHueButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaHueTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_HUE, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaHueTextBox.Text = value.ToString();
        }

        private void corneaHueButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaHueTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_HUE, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaHueTextBox.Text = value.ToString();
        }

        private void retinaGainButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaGainTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_GAIN, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaGainTextBox.Text = value.ToString();
        }

        private void corneaGainButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaGainTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_GAIN, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaGainTextBox.Text = value.ToString();
        }

        private void retinaExposureButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaExposureTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_EXPOSURE, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaExposureTextBox.Text = value.ToString();
        }

        private void corneaExposureButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaExposureTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_EXPOSURE, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaExposureTextBox.Text = value.ToString();
        }

        private void retinaWhiteBalanceBlueButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaWhiteBalanceBlueTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_BLUE_U, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaWhiteBalanceBlueTextBox.Text = value.ToString();
        }

        private void corneaWhiteBalanceBlueButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaWhiteBalanceBlueTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_BLUE_U, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaWhiteBalanceBlueTextBox.Text = value.ToString();
        }

        private void retinaWhiteBalanceRedButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaWhiteBalanceRedTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_RED_V, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaWhiteBalanceRedTextBox.Text = value.ToString();
        }

        private void corneaWhiteBalanceRedButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaWhiteBalanceRedTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_WHITE_BALANCE_RED_V, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaWhiteBalanceRedTextBox.Text = value.ToString();
        }

        private void retinaSharpnessButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaSharpnessTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_SHARPNESS, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaSharpnessTextBox.Text = value.ToString();
        }

        private void corneaSharpnessButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaSharpnessTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_SHARPNESS, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaSharpnessTextBox.Text = value.ToString();
        }

        private void retinaGammaButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaGammaTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_GAMMA, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaGammaTextBox.Text = value.ToString();
        }

        private void corneaGammaButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaGammaTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_GAMMA, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaGammaTextBox.Text = value.ToString();
        }

        private void retinaFocusButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(retinaFocusTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Retina, Statics.VideoCaptureProperties.CAP_PROP_FOCUS, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            retinaFocusTextBox.Text = value.ToString();
        }

        private void corneaFocusButton_Click(object sender, RoutedEventArgs e)
        {
            int value = int.Parse(corneaFocusTextBox.Text);
            int d = int.Parse((string)((Button)sender).Content);
            value += d;
            Statics.ErrorSetCameraParam errorSetCameraParam = Statics.setCameraParam(Statics.Part.Cornea, Statics.VideoCaptureProperties.CAP_PROP_FOCUS, value);
            if (errorSetCameraParam != Statics.ErrorSetCameraParam.None)
            {
                Console.WriteLine(errorSetCameraParam);
            }
            corneaFocusTextBox.Text = value.ToString();
        }
    }
}
