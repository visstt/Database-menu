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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using OfficeOpenXml; 
using Microsoft.Win32;

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
    private Nochnie_Volki_OrenburgEntities db;
        public TableManagementWindow(string tableName)
    {
            InitializeComponent();
            db = new Nochnie_Volki_OrenburgEntities(); // Создаем экземпляр контекста базы данных
            selectedTable = tableName;
            string connectionString = "Server=DESKTOP-PFBB33O;Database=Nochnie_Volki_Orenburg;Trusted_Connection=True;"; // Строка подключения
            connection = new SqlConnection(connectionString);
            LoadData();
        }

    // Загрузка данных выбранной таблицы
    private void LoadData()
    {
        try
        {
            connection.Open();
            string query = $"SELECT * FROM {selectedTable}";
            dataAdapter = new SqlDataAdapter(query, connection);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            tableData = new DataTable();
            dataAdapter.Fill(tableData);
            dgTableData.ItemsSource = tableData.DefaultView;

            LoadCustomerData();
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


    // Загрузка клиентов в ComboBox
    private void LoadCustomerData()
    {
            var customers = db.Customer.Select(c => c.LastName).Distinct().ToList();

            // Добавляем пункт "Все клиенты" для сброса фильтра
            CustomerComboBox.Items.Add("Все клиенты");

            foreach (var customer in customers)
            {
                CustomerComboBox.Items.Add(customer);
            }

            CustomerComboBox.SelectedIndex = 0;
        }


        // Фильтрация по клиенту
        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCustomer = CustomerComboBox.SelectedItem?.ToString();

            string query;

            if (selectedCustomer == "Все клиенты")
            {
                query = $"SELECT * FROM {selectedTable}"; // Загружаем все записи, если выбран пункт "Все клиенты"
            }
            else
            {
                query = $"SELECT * FROM {selectedTable} WHERE LastName = @customer"; // Фильтруем по фамилии
            }

            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                // Добавляем параметр @customer, если выбран конкретный клиент
                if (selectedCustomer != "Все клиенты")
                {
                    command.Parameters.AddWithValue("@customer", selectedCustomer);
                }

                dataAdapter = new SqlDataAdapter(command);
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

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        Document pdfDoc = new Document();
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();

                        
                        PdfPTable pdfTable = new PdfPTable(dgTableData.Columns.Count);
                        foreach (DataGridColumn column in dgTableData.Columns)
                        {
                            
                            pdfTable.AddCell(new Phrase(column.Header.ToString()));
                        }

                        foreach (var item in dgTableData.Items)
                        {
                            // Assuming item is a DataRowView
                            DataRowView rowView = item as DataRowView;
                            if (rowView != null)
                            {
                                foreach (var cell in rowView.Row.ItemArray)
                                {
                                    pdfTable.AddCell(new Phrase(cell.ToString()));
                                }
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        stream.Close();
                        MessageBox.Show("PDF exported successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to PDF: " + ex.Message);
            }
        }

        // Кнопка "Назад"
        private void BackToSelection_Click(object sender, RoutedEventArgs e)
    {
        var selectionWindow = new TableSelectionWindow();
        selectionWindow.Show();
        this.Close();
    }
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
        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Data");

                        // Заголовки
                        for (int i = 0; i < dgTableData.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = dgTableData.Columns[i].Header.ToString();
                        }

                        // Данные
                        for (int i = 0; i < tableData.Rows.Count; i++)
                        {
                            for (int j = 0; j < tableData.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 2, j + 1].Value = tableData.Rows[i][j].ToString();
                            }
                        }

                        // Сохранение файла
                        FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                        excelPackage.SaveAs(excelFile);
                    }
                    MessageBox.Show("Data exported to Excel successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Excel: " + ex.Message);
            }
        }

        private void ImportFromExcelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx";
                if (openFileDialog.ShowDialog() == true)
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(openFileDialog.FileName)))
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0]; // Получаем первый лист
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        // Создаем новый DataTable
                        DataTable dt = new DataTable();

                        // Заголовки
                        for (int i = 1; i <= colCount; i++)
                        {
                            dt.Columns.Add(worksheet.Cells[1, i].Value.ToString());
                        }

                        // Данные
                        for (int i = 2; i <= rowCount; i++)
                        {
                            DataRow row = dt.NewRow();
                            for (int j = 1; j <= colCount; j++)
                            {
                                row[j - 1] = worksheet.Cells[i, j].Value?.ToString();
                            }
                            dt.Rows.Add(row);
                        }

                        // Обновляем DataGrid
                        tableData = dt;
                        dgTableData.ItemsSource = tableData.DefaultView;
                        MessageBox.Show("Data imported from Excel successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error importing from Excel: " + ex.Message);
            }
        }

        // Методы для экспорта и импорта CSV
        private void ExportToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Запись заголовков
                        for (int i = 0; i < dgTableData.Columns.Count; i++)
                        {
                            writer.Write(dgTableData.Columns[i].Header);
                            if (i < dgTableData.Columns.Count - 1) writer.Write(",");
                        }
                        writer.WriteLine();

                        // Запись данных
                        foreach (var item in dgTableData.Items)
                        {
                            DataRowView rowView = item as DataRowView;
                            if (rowView != null)
                            {
                                for (int i = 0; i < rowView.Row.ItemArray.Length; i++)
                                {
                                    writer.Write(rowView.Row.ItemArray[i]);
                                    if (i < rowView.Row.ItemArray.Length - 1) writer.Write(",");
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                    MessageBox.Show("CSV экспортирован успешно!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта в CSV: " + ex.Message);
            }
        }

        private void ImportFromCsvButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    DataTable importedTable = new DataTable();
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        string[] headers = reader.ReadLine()?.Split(',');
                        if (headers == null) throw new Exception("Файл пустой или поврежден.");

                        // Создание столбцов таблицы
                        foreach (string header in headers)
                        {
                            importedTable.Columns.Add(header.Trim());
                        }

                        // Заполнение данных
                        while (!reader.EndOfStream)
                        {
                            string[] row = reader.ReadLine()?.Split(',');
                            if (row != null)
                            {
                                importedTable.Rows.Add(row);
                            }
                        }
                    }
                    dgTableData.ItemsSource = importedTable.DefaultView;
                    MessageBox.Show("CSV импортирован успешно!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка импорта из CSV: " + ex.Message);
            }
        }


    }
}


