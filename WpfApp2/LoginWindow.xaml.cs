using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp2
{
    public partial class LoginWindow : Window
    {
        Nochnie_Volki_OrenburgEntities db;
        private int loginAttempts = 0;
        private string captchaText;

        public LoginWindow()
        {
            InitializeComponent();
            db = new Nochnie_Volki_OrenburgEntities();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, требуется ли капча
                if (loginAttempts >= 3)
                {
                    if (string.IsNullOrEmpty(CaptchaTextBox.Text) || CaptchaTextBox.Text != captchaText)
                    {
                        MessageBox.Show("Неверная капча.");
                        return;
                    }
                }

                // Хэшируем введённый пароль
                string hashedPassword = HashPassword(Pass.Password);

                // Ищем пользователя в базе данных
                var Active_User = db.Auth.FirstOrDefault(x => x.Login == Log.Text && x.Password == hashedPassword);
                if (Active_User != null)
                {
                    MessageBox.Show($"Добро пожаловать, {Log.Text}!");
                    var Log_To_Selection = new TableSelectionWindow();
                    this.Close();
                    Log_To_Selection.Show();
                }
                else
                {
                    loginAttempts++;
                    MessageBox.Show("Неверные логин или пароль.");
                    if (loginAttempts >= 3)
                    {
                        ShowCaptcha();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при попытке входа.");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private void ShowCaptcha()
        {
            // Генерация капчи
            captchaText = GenerateCaptcha();
            CaptchaImage.Source = CreateCaptchaImage(captchaText);
            CaptchaImage.Visibility = Visibility.Visible;
            CaptchaTextBox.Visibility = Visibility.Visible;
        }

        private string GenerateCaptcha()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private BitmapSource CreateCaptchaImage(string captchaText)
        {
            int width = 200;
            int height = 50;

            // Создаем визуальный объект для рисования
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Задаем цвет фона
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(255, 204, 204)), null, new Rect(0, 0, width, height));

                // Создаем шрифт и задаем его свойства
                Typeface typeface = new Typeface("Arial");
                Random random = new Random();
                double angle;
                for (int i = 0; i < captchaText.Length; i++)
                {
                    angle = random.Next(-30, 30); // Поворот букв
                    FormattedText formattedText = new FormattedText(
                        captchaText[i].ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        24,
                        Brushes.Black);

                    // Позиция для каждой буквы
                    double x = 10 + (i * 30) + random.Next(-5, 5); // Плавающая позиция
                    double y = random.Next(5, 15); // Плавающая позиция по вертикали

                    // Рисуем букву
                    drawingContext.PushTransform(new RotateTransform(angle, x, y));
                    drawingContext.DrawText(formattedText, new Point(x, y));
                    drawingContext.Pop();
                }

                // Добавляем помехи
                for (int i = 0; i < 10; i++)
                {
                    double x = random.Next(0, width);
                    double y = random.Next(0, height);
                    drawingContext.DrawEllipse(Brushes.Gray, null, new Point(x, y), 2, 2);
                }
            }

            // Создаем BitmapSource из визуального объекта
            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(); // Переход на окно регистрации
            registerWindow.Show();
            this.Close();
        }
    }
}
