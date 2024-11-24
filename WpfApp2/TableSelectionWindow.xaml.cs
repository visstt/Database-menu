using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для TableSelectionWindow.xaml
    /// </summary>
    public partial class TableSelectionWindow : Window
    {
        private SqlConnection connection;

        public TableSelectionWindow()
        {
            InitializeComponent();
            // Инициализация подключения к базе данных
            string connectionString = "Server=DESKTOP-PFBB33O;Database=Nochnie_Volki_Orenburg;Trusted_Connection=True;"; // Добавьте вашу строку подключения
            connection = new SqlConnection(connectionString);
            LoadTableList();
        }
        private void GenerateQRCode_Click(object sender, RoutedEventArgs e)
        {
            QRCodeGeneratorWindow qrCodeWindow = new QRCodeGeneratorWindow();
            qrCodeWindow.Show();
        }
        // Метод для загрузки списка таблиц
        private void LoadTableList()
        {
            try
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row[2].ToString();
                    TableList.Items.Add(tableName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // Переход на окно управления таблицей
        private void SelectTable_Click(object sender, RoutedEventArgs e)
        {
            string selectedTable = TableList.SelectedItem?.ToString();
            if (selectedTable != null)
            {
                TableManagementWindow tableWindow = new TableManagementWindow(selectedTable);
                tableWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a table.");
            }
        }
    }
}
