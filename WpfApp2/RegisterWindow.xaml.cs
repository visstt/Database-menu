using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        Nochnie_Volki_OrenburgEntities db;

        public RegisterWindow()
        {
            InitializeComponent();
            db = new Nochnie_Volki_OrenburgEntities();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Log.Text) && !string.IsNullOrEmpty(Pass.Password))
                {
                    // Хэшируем пароль перед сохранением
                    string hashedPassword = HashPassword(Pass.Password);

                    Auth newUser = new Auth
                    {
                        Login = Log.Text,
                        Password = hashedPassword
                    };

                    db.Auth.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Пользователь зарегистрирован!");
                    var loginWindow = new LoginWindow();
                    this.Close();
                    loginWindow.Show();
                }
                else
                {
                    MessageBox.Show("Введите логин и пароль.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка при регистрации.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            this.Close();
            loginWindow.Show();
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
    }
}
