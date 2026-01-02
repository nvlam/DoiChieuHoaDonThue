using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoiChieuHoaDonThue
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _file1Path;
        private string _file2Path;
        private ObservableCollection<CompareResult> _results;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnBrowseThue_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls"
            };

            if (picker.ShowDialog() == true)
            {
                _file1Path = picker.FileName;
                txtTepCongThue.Text = _file1Path;
            }
        }

        private async void btnBrowseMisa_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls"
            };

            if (picker.ShowDialog() == true)
            {
                _file2Path = picker.FileName;
                txtTepMisa.Text = _file2Path;
            }
        }

        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
         
            if (string.IsNullOrWhiteSpace(_file1Path) ||
                string.IsNullOrWhiteSpace(_file2Path))
            {
                MessageBox.Show(
                    "Please select both Excel files before comparing.",
                    "Missing file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // 1. Read Excel files (existing method)
            var reader = new ExcelFileReaderService();
            var dt1 = reader.ReadExcelToDataTable(_file1Path);
            var dt2 = reader.ReadExcelToDataTable(_file2Path);

            if (dt1 == null || dt2 == null)
                return;

            // 2. Load mapping from JSON
            //var mapping = ExcelMappingLoader.Load("Config/excel_mapping.json");
            string configPath = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory,"Config","excel_mapping.json");

            var mapping = ExcelMappingLoader.Load(configPath);


            // 3. Compare
            var compareService = new ExcelCompareService();
            var compareList = compareService.Compare(dt1, dt2, mapping);

            // 4. Bind to DataGrid
            _results = new ObservableCollection<CompareResult>(compareList);
            DataGridKetQua.ItemsSource = _results;

            // 5. Show summary
            lblKetQua.Content = $"Đối chiếu xong. Có {_results.Count} dòng khác nhau.";
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (_results == null || _results.Count == 0)
            {
                MessageBox.Show(
                    "Không có dữ liệu để xuất Excel.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel file (*.xlsx)|*.xlsx",
                FileName = $"KetQuaDoiChieu_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var exporter = new ExcelExportService();
                exporter.ExportCompareResults(_results, dialog.FileName);

                MessageBox.Show(
                    "Xuất file Excel thành công!",
                    "Hoàn tất",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi xuất Excel:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}