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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClosedXML.Excel;
using Microsoft.Win32;
using MongoDB.Driver.Encryption;
namespace CarParkingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Person> people = new List<Person>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportExcel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx"
            };

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    people.Clear();

                    using (var workbook = new XLWorkbook(ofd.FileName))
                    {
                        var ws = workbook.Worksheets.First();

                        // Bắt đầu từ dòng 2 (dòng 1 là header: Name, Age)
                        foreach (var row in ws.RowsUsed().Skip(1))
                        {
                            var name = row.Cell(1).GetString();
                            var ageText = row.Cell(2).GetString();
                            if (int.TryParse(ageText, out int age))
                            {
                                people.Add(new Person { Name = name, Age = age });
                            }
                        }
                    }

                    dataGrid.ItemsSource = null; // Reset trước
                    dataGrid.ItemsSource = people;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đọc file Excel: " + ex.Message);
                }
            }
        }
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (people == null || people.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = "Exported_People.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("People");

                        // Header
                        ws.Cell(1, 1).Value = "Name";
                        ws.Cell(1, 2).Value = "Age";

                        // Data
                        for (int i = 0; i < people.Count; i++)
                        {
                            ws.Cell(i + 2, 1).Value = people[i].Name;
                            ws.Cell(i + 2, 2).Value = people[i].Age;
                        }

                        workbook.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi export: " + ex.Message);
                }
            }
        }
    }
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
