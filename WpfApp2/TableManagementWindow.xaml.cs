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
    /// Логика взаимодействия для TableManagementWindow.xaml
    /// </summary>
    public partial class TableManagementWindow : Window
    {
        private SqlConnection connection;
        private string selectedTable;
        private DataTable tableData;
        private SqlDataAdapter dataAdapter;

        public TableManagementWindow(string tableName)
        {
            InitializeComponent();
            selectedTable = tableName;
            string connectionString = "Server=DESKTOP-B39KRR8;Database=Nochnie_Volki_Orenburg;Trusted_Connection=True;"; // Строка подключения
            connection = new SqlConnection(connectionString);
            LoadTableData();
        }

        // Загрузка данных выбранной таблицы
        private void LoadTableData()
        {
            try
            {
                connection.Open();
                dataAdapter = new SqlDataAdapter($"SELECT * FROM {selectedTable}", connection);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
                tableData = new DataTable();
                dataAdapter.Fill(tableData);
                dgTableData.ItemsSource = tableData.DefaultView;
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

        // Добавление новой записи
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DataRow newRow = tableData.NewRow();
            // Здесь можно добавить логику для ввода данных пользователем
            tableData.Rows.Add(newRow);
            SaveChanges();
        }

        // Обновление данных
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        // Удаление выбранной записи
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgTableData.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)dgTableData.SelectedItem;
                selectedRow.Row.Delete();
                SaveChanges();
            }
        }

        // Сохранение изменений в базу данных
        private void SaveChanges()
        {
            try
            {
                connection.Open();
                dataAdapter.Update(tableData);
                MessageBox.Show("Changes saved successfully.");
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
        private void BackToSelection_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно TableSelectionWindow
            TableSelectionWindow selectionWindow = new TableSelectionWindow();
            selectionWindow.Show();

            // Закрываем текущее окно TableManagementWindow
            this.Close();
        }
    }
}
