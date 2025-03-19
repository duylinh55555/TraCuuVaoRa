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
using OfficeOpenXml;

namespace TraCuuVaoRa_WPF
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainScreen : UserControl
    {
        static private DateTime startDate = DateTime.Now.AddDays(-7);
        static private DateTime endDate = DateTime.Now;
        static private TimeSpan startTime;
        static private TimeSpan endTime;
        static private DataContext? context;

        public MainScreen(DataContext dataContext)
        {
            context = dataContext;
            startTime = context.timeSpans[0];
            endTime = context.timeSpans[7];

            InitializeComponent();

            dateSelector.SetDate(startDate, endDate);
            SetTime(startTime, endTime);
        }

        private void SetTime(TimeSpan startTime, TimeSpan endTime)
        {
            for(int i = 0; i < 24; i++)
            {
                startHourComboBox.Items.Add(i.ToString("D2")); // D2: 2 digits
                endHourComboBox.Items.Add(i.ToString("D2")); // D2: 2 digits
            }

            for (int i = 0; i < 60; i++)
            {
                startMinuteComboBox.Items.Add(i.ToString("D2"));
                endMinuteComboBox.Items.Add(i.ToString("D2"));
            }

            startHourComboBox.SelectedIndex = startTime.Hours;
            endHourComboBox.SelectedIndex = endTime.Hours;
            startMinuteComboBox.SelectedIndex = startTime.Minutes;
            endMinuteComboBox.SelectedIndex = endTime.Minutes;
        }

        private void GetTime()
        {
            string startHour = startHourComboBox.SelectedItem.ToString() ?? "0";
            string startMinute = startMinuteComboBox.SelectedItem.ToString() ?? "0";
            string endHour = endHourComboBox.SelectedItem.ToString() ?? "0";
            string endMinute = endMinuteComboBox.SelectedItem.ToString() ?? "0";
            startTime = new TimeSpan(int.Parse(startHour), int.Parse(startMinute), 0);
            endTime = new TimeSpan(int.Parse(endHour), int.Parse(endMinute), 0);
        }

        #region UI Interaction
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // Get Date Range
            startDate = dateSelector.StartDate();
            endDate = dateSelector.EndDate();

            // Get Time Range
            GetTime();

            // Connect to Server & Query Data
            if (context != null)
            {
                bool isConnected = context.CheckDatabaseConnection(context.dataSource, context.userId, context.password);

                if (!isConnected)
                    MessageBox.Show("Không thể kết nối tới database.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    var query = from car in context.Car
                                join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                join part in context.Part on car.IDPart equals part.ID
                                where car.TimeStart.HasValue && car.TimeStart.Value.TimeOfDay >= startTime && car.TimeStart.Value.TimeOfDay <= endTime
                                && car.TimeStart.Value.Date >= startDate.Date && car.TimeStart.Value.Date <= endDate.Date
                                orderby car.TimeStart descending
                                select new
                                {
                                    car,
                                    vethang,
                                    part,
                                    TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                    TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                };

                    var result = query.ToList();

                    // Add index to each item
                    var indexedResult = result.Select((item, index) => new
                    {
                        Index = index + 1,
                        item.car,
                        item.vethang,
                        item.part,
                        item.TimeStartFormatted,
                        item.TimeEndFormatted
                    }).ToList();

                    vehiclePersonDataGrid.ItemsSource = indexedResult;

                    // Show, Hide Export Button
                    exportXlsxButton.Visibility = result.Any() ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void vehiclePersonDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Get the selected column
            string column = e.Column.SortMemberPath ?? string.Empty;

            ICollectionView view = CollectionViewSource.GetDefaultView(vehiclePersonDataGrid.ItemsSource);

            ListSortDirection direction = e.Column.SortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            view.SortDescriptions.Clear();

            if (view != null)
            {
                switch (column)
                {
                    case "vethang.HoTen":
                        var data = vehiclePersonDataGrid.ItemsSource.Cast<dynamic>().ToList();
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
                        vehiclePersonDataGrid.ItemsSource = data;
                        break;

                    case "TimeStartFormatted":
                    case "TimeEndFormatted":
                        var timeData = vehiclePersonDataGrid.ItemsSource.Cast<dynamic>().ToList();
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
                        vehiclePersonDataGrid.ItemsSource = timeData;
                        break;

                    default:
                        view.SortDescriptions.Add(new SortDescription(column, direction));
                        break;
                }

                e.Column.SortDirection = direction;

                e.Handled = true;
            }
        }
        
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            vehiclePersonDataGrid.Items.Filter = item =>
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

        private void exportXlsxButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Save an Excel File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                var filteredItems = vehiclePersonDataGrid.Items.Cast<dynamic>().ToList();

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

        // Set Image
        private void vehiclePersonDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string xeVaoImageFileName = "";
            string xeVaoImageDate = "";

            string xeRaImageFileName = "";
            string xeRaImageDate = "";

            if (vehiclePersonDataGrid.SelectedItem != null)
            {
                dynamic selectedItem = vehiclePersonDataGrid.SelectedItem;
                xeVaoImageFileName = selectedItem.car.Images + ".jpg";
                xeVaoImageDate = selectedItem.car.TimeStart?.ToString("MM-dd-yyyy") ?? "";

                xeRaImageFileName = selectedItem.car.Images2 + ".jpg";
                xeRaImageDate = selectedItem.car.TimeEnd?.ToString("MM-dd-yyyy") ?? "";
            }

            string xeVaoPath = System.IO.Path.Combine(context?.xeVaoImageFolderUrl ?? string.Empty, xeVaoImageDate, xeVaoImageFileName);
            string xeRaPath = System.IO.Path.Combine(context?.xeRaImageFolderUrl ?? string.Empty, xeRaImageDate, xeRaImageFileName);

            if (File.Exists(xeVaoPath))
                xeVao_CustomImage.Source = new BitmapImage(new Uri(xeVaoPath, UriKind.Absolute));
            else
                xeVao_CustomImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/blankimage.png", UriKind.Absolute));

            if (File.Exists(xeRaPath))
                xeRa_CustomImage.Source = new BitmapImage(new Uri(xeRaPath, UriKind.Absolute));
            else
                xeRa_CustomImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/blankimage.png", UriKind.Absolute));
        }
        #endregion

    }
}
