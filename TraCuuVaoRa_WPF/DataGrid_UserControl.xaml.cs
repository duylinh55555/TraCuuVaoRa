using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace TraCuuVaoRa_WPF
{
    /// <summary>
    /// Interaction logic for DataGrid_UserControl.xaml
    /// </summary>
    public partial class DataGrid_UserControl : UserControl
    {
        public DataContext dataContext;

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        // Dependency Property for Image1 (xeVao)
        public static readonly DependencyProperty xeVao_ImageSourceProperty =
            DependencyProperty.Register(
                "xeVao_CustomImage",
                typeof(ImageSource),
                typeof(DataGrid_UserControl),
                new PropertyMetadata(null, OnImageSource1Changed));

        public ImageSource xeVao_CustomImage
        {
            get { return (ImageSource)GetValue(xeVao_ImageSourceProperty); }
            set { SetValue(xeVao_ImageSourceProperty, value); }
        }

        // Dependency Property for Image2 (xeRa)
        public static readonly DependencyProperty xeRa_ImageSourceProperty =
            DependencyProperty.Register(
                "xeRa_CustomImage",
                typeof(ImageSource),
                typeof(DataGrid_UserControl),
                new PropertyMetadata(null, OnImageSource2Changed));

        public ImageSource xeRa_CustomImage
        {
            get { return (ImageSource)GetValue(xeRa_ImageSourceProperty); }
            set { SetValue(xeRa_ImageSourceProperty, value); }
        }

        private static void OnImageSource1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGrid_UserControl;
            control?.UpdateImageSource1();
        }

        private static void OnImageSource2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGrid_UserControl;
            control?.UpdateImageSource2();
        }

        public DataGrid_UserControl()
        {
            InitializeComponent();
        }

        private void UpdateImageSource1()
        {
            System.Diagnostics.Debug.WriteLine($"XeVao image updated to {xeVao_CustomImage}");
        }

        private void UpdateImageSource2()
        {
            System.Diagnostics.Debug.WriteLine($"XeRa image updated to {xeRa_CustomImage}"); 
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Get the selected column
            string column = e.Column.SortMemberPath ?? string.Empty;

            ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

            ListSortDirection direction = e.Column.SortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            view.SortDescriptions.Clear();

            if (view != null)
            {
                switch (column)
                {
                    case "vethang.HoTen":
                        var data = dataGrid.ItemsSource.Cast<dynamic>().ToList();
                        if (direction == ListSortDirection.Ascending)
                        {
                            data = data.OrderBy(item =>
                            {
                                string hoTen = item.vethang.HoTen;
                                if (hoTen != null)
                                {
                                    if (hoTen.EndsWith(" "))
                                        hoTen = hoTen.TrimEnd();
                                    return string.Join(" ", hoTen.Split(' ').Reverse()).ToLower();
                                }
                                return string.Empty;
                            }).ToList();
                        }
                        else
                        {
                            data = data.OrderByDescending(item =>
                            {
                                string hoTen = item.vethang.HoTen;
                                if (hoTen != null)
                                {
                                    if (hoTen.EndsWith(" "))
                                        hoTen = hoTen.TrimEnd();
                                    return string.Join(" ", hoTen.Split(' ').Reverse()).ToLower();
                                }
                                return string.Empty;
                            }).ToList();
                        }
                        dataGrid.ItemsSource = data;
                        break;

                    case "TimeStartFormatted":
                    case "TimeEndFormatted":
                        var timeData = dataGrid.ItemsSource.Cast<dynamic>().ToList();
                        var timeProperty = column == "TimeStartFormatted" ? "TimeStartFormatted" : "TimeEndFormatted";
                        if (direction == ListSortDirection.Ascending)
                        {
                            timeData = timeData.OrderBy(item =>
                            {
                                if (!string.IsNullOrEmpty(item.GetType().GetProperty(timeProperty)?.GetValue(item)?.ToString()))
                                {
                                    return DateTime.ParseExact(item.GetType().GetProperty(timeProperty)?.GetValue(item)?.ToString(), "dd/MM/yyyy HH:mm:ss", null);
                                }
                                return DateTime.MinValue;
                            }).ToList();
                        }
                        else
                        {
                            timeData = timeData.OrderByDescending(item =>
                            {
                                if (!string.IsNullOrEmpty(item.GetType().GetProperty(timeProperty)?.GetValue(item)?.ToString()))
                                {
                                    return DateTime.ParseExact(item.GetType().GetProperty(timeProperty)?.GetValue(item)?.ToString(), "dd/MM/yyyy HH:mm:ss", null);
                                }
                                return DateTime.MinValue;
                            }).ToList();
                        }
                        dataGrid.ItemsSource = timeData;
                        break;

                    default:
                        view.SortDescriptions.Add(new SortDescription(column, direction));
                        break;
                }

                e.Column.SortDirection = direction;

                e.Handled = true;
            }
        }

        public void Search_DataGrid(string searchText)
        {
            dataGrid.Items.Filter = item =>
            {
                if (item is { })
                {
                    var car = (item as dynamic).car;
                    var vethang = (item as dynamic).vethang;
                    return car.Digit.ToLower().Contains(searchText)
                        || vethang.HoTen.ToLower().Contains(searchText)
                        || vethang.STT.ToLower().Contains(searchText)
                        || vethang.CanHo.ToLower().Contains(searchText)
                        || vethang.HieuXe.ToLower().Contains(searchText)
                        || car.Computer.ToLower().Contains(searchText);
                }
                return false;
            };
        }

        // Set Image
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);

            string xeVaoImageFileName = "";
            string xeVaoImageDate = "";

            string xeRaImageFileName = "";
            string xeRaImageDate = "";

            if (dataGrid.SelectedItem != null)
            {
                dynamic selectedItem = dataGrid.SelectedItem;
                xeVaoImageFileName = selectedItem.car.Images + ".jpg";
                xeVaoImageDate = selectedItem.car.TimeStart?.ToString("MM-dd-yyyy") ?? "";

                xeRaImageFileName = selectedItem.car.Images2 + ".jpg";
                xeRaImageDate = selectedItem.car.TimeEnd?.ToString("MM-dd-yyyy") ?? "";
            }

            string xeVaoPath = System.IO.Path.Combine(dataContext?.xeVaoImageFolderUrl ?? string.Empty, xeVaoImageDate, xeVaoImageFileName);
            string xeRaPath = System.IO.Path.Combine(dataContext?.xeRaImageFolderUrl ?? string.Empty, xeRaImageDate, xeRaImageFileName);

            if (File.Exists(xeVaoPath))
                xeVao_CustomImage = new BitmapImage(new Uri(xeVaoPath, UriKind.Absolute));
            else
                xeVao_CustomImage = new BitmapImage(new Uri("pack://application:,,,/Images/blankimage.png", UriKind.Absolute));

            if (File.Exists(xeRaPath))
                xeRa_CustomImage = new BitmapImage(new Uri(xeRaPath, UriKind.Absolute));
            else
                xeRa_CustomImage = new BitmapImage(new Uri("pack://application:,,,/Images/blankimage.png", UriKind.Absolute));

        }

        public void ExportToFile()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Save an Excel File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                var filteredItems = dataGrid.Items.Cast<dynamic>().ToList();

                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Data");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Số thẻ";
                    worksheet.Cells[1, 2].Value = "Biển số";
                    worksheet.Cells[1, 3].Value = "Họ tên";
                    worksheet.Cells[1, 4].Value = "Đơn vị";
                    worksheet.Cells[1, 5].Value = "Cấp bậc";
                    worksheet.Cells[1, 6].Value = "Thời gian vào";
                    worksheet.Cells[1, 7].Value = "Thời gian ra";
                    worksheet.Cells[1, 8].Value = "Cổng";

                    // Add data
                    for (int i = 0; i < filteredItems.Count; i++)
                    {
                        var car = filteredItems[i].car;
                        var vethang = filteredItems[i].vethang;

                        worksheet.Cells[i + 2, 1].Value = vethang.STT;
                        worksheet.Cells[i + 2, 2].Value = car.Digit;
                        worksheet.Cells[i + 2, 3].Value = vethang.HoTen;
                        worksheet.Cells[i + 2, 4].Value = vethang.CanHo;
                        worksheet.Cells[i + 2, 5].Value = vethang.HieuXe;
                        worksheet.Cells[i + 2, 6].Value = car.TimeStart?.ToString("dd/MM/yyyy HH:mm:ss");
                        worksheet.Cells[i + 2, 7].Value = car.TimeEnd?.ToString("dd/MM/yyyy HH:mm:ss");
                        worksheet.Cells[i + 2, 8].Value = car.Computer;
                    }

                    // Save the file
                    package.SaveAs(new FileInfo(filePath));
                }

                MessageBox.Show("Xuất file thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
