
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using QRCoder; 

namespace WpfApp2
{
    public partial class QRCodeGeneratorWindow : Window
    {
        public QRCodeGeneratorWindow()
        {
            InitializeComponent();
        }

        private void GenerateQRCode_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlTextBox.Text;
            if (!string.IsNullOrEmpty(url))
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(20);
                QRCodeImage.Source = BitmapToImageSource(qrCodeImage);
            }
            else
            {
                MessageBox.Show("Please enter a URL.");
            }
        }

        private BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }
    }
}