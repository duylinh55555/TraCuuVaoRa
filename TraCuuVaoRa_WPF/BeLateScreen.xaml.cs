using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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
    /// Interaction logic for BeLateScreen.xaml
    /// </summary>
    public partial class BeLateScreen : UserControl
    {
        private bool isLateMorning = false;
        private bool isEarlyNoon = false;
        private bool isLateAfternoon = false;
        private bool isEarlyAfternoon = false;

        static private DateTime startDate = DateTime.Now.AddDays(-7);
        static private DateTime endDate = DateTime.Now;
        static private DataContext? context;
        static private TimeSpan[] timeSpans;

        public BeLateScreen(DataContext dataContext)
        {
            context = dataContext;
            timeSpans = context.timeSpans;

            InitializeComponent();

            dateSelector.SetDate(startDate, endDate);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // Get Date Range
            startDate = dateSelector.StartDate();
            endDate = dateSelector.EndDate();

            // Get Time Range
            isLateMorning = lateMorning_CheckBox.IsChecked ?? true;
            isEarlyNoon = earlyNoon_CheckBox.IsChecked ?? true;
            isLateAfternoon = lateAfternoon_CheckBox.IsChecked ?? false;
            isEarlyAfternoon = earlyAfternoon_CheckBox.IsChecked ?? false;

            // Connect to Server & Query Data
            if (context != null)
            {
                bool isConnected = context.CheckDatabaseConnection(context.dataSource, context.userId, context.password);

                if (!isConnected)
                    MessageBox.Show("Không thể kết nối tới database.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    var result = new List<dynamic>();

                    if (isLateMorning)
                    {
                        var query = from car in context.Car
                                    join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                    join part in context.Part on car.IDPart equals part.ID
                                    where car.TimeStart.HasValue
                                       && car.TimeStart.Value.Date >= startDate.Date
                                       && car.TimeStart.Value.Date <= endDate.Date
                                       && car.TimeStart.Value.TimeOfDay >= timeSpans[0]
                                       && car.TimeStart.Value.TimeOfDay <= timeSpans[1]
                                    select new
                                    {
                                        car,
                                        vethang,
                                        part,
                                        TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                        TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                    };
                        result.AddRange(query.ToList());
                    }

                    if (isEarlyNoon)
                    {
                        var query2 = from car in context.Car
                                     join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                     join part in context.Part on car.IDPart equals part.ID
                                     where car.TimeEnd.HasValue
                                        && car.TimeEnd.Value.Date >= startDate.Date
                                        && car.TimeEnd.Value.Date <= endDate.Date
                                        && car.TimeEnd.Value.TimeOfDay >= timeSpans[4]
                                        && car.TimeEnd.Value.TimeOfDay <= timeSpans[5]
                                     select new
                                     {
                                         car,
                                         vethang,
                                         part,
                                         TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                         TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                     };
                        result.AddRange(query2.ToList());
                    }

                    if (isLateAfternoon)
                    {
                        var query3 = from car in context.Car
                                     join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                     join part in context.Part on car.IDPart equals part.ID
                                     where car.TimeStart.HasValue
                                        && car.TimeStart.Value.Date >= startDate.Date
                                        && car.TimeStart.Value.Date <= endDate.Date
                                        && car.TimeStart.Value.TimeOfDay >= timeSpans[2]
                                        && car.TimeStart.Value.TimeOfDay <= timeSpans[3]
                                     select new
                                     {
                                         car,
                                         vethang,
                                         part,
                                         TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                         TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                     };
                        result.AddRange(query3.ToList());
                    }


                    if (isEarlyAfternoon)
                    {
                        var query4 = from car in context.Car
                                     join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                     join part in context.Part on car.IDPart equals part.ID
                                     where car.TimeEnd.HasValue
                                        && car.TimeEnd.Value.Date >= startDate.Date
                                        && car.TimeEnd.Value.Date <= endDate.Date
                                        && car.TimeEnd.Value.TimeOfDay >= timeSpans[6]
                                        && car.TimeEnd.Value.TimeOfDay <= timeSpans[7]
                                     select new
                                     {
                                         car,
                                         vethang,
                                         part,
                                         TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                         TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                     };
                        result.AddRange(query4.ToList());
                    }

                    // Updated query with order by TimeStart
                    if (isLateMorning)
                    {
                        var query = from car in context.Car
                                    join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                    join part in context.Part on car.IDPart equals part.ID
                                    where car.TimeStart.HasValue
                                       && car.TimeStart.Value.Date >= startDate.Date
                                       && car.TimeStart.Value.Date <= endDate.Date
                                       && car.TimeStart.Value.TimeOfDay >= timeSpans[0]
                                       && car.TimeStart.Value.TimeOfDay <= timeSpans[1]
                                    select new
                                    {
                                        car,
                                        vethang,
                                        part,
                                        TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                        TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                    };
                        result.AddRange(query.ToList());
                    }

                    if (isEarlyNoon)
                    {
                        var query2 = from car in context.Car
                                     join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                     join part in context.Part on car.IDPart equals part.ID
                                     where car.TimeEnd.HasValue
                                        && car.TimeEnd.Value.Date >= startDate.Date
                                        && car.TimeEnd.Value.Date <= endDate.Date
                                        && car.TimeEnd.Value.TimeOfDay >= timeSpans[4]
                                        && car.TimeEnd.Value.TimeOfDay <= timeSpans[5]
                                     select new
                                     {
                                         car,
                                         vethang,
                                         part,
                                         TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                         TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                     };
                        result.AddRange(query2.ToList());
                    }

                    if (isLateAfternoon)
                    {
                        var query3 = from car in context.Car
                                     join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                     join part in context.Part on car.IDPart equals part.ID
                                     where car.TimeStart.HasValue
                                        && car.TimeStart.Value.Date >= startDate.Date
                                        && car.TimeStart.Value.Date <= endDate.Date
                                        && car.TimeStart.Value.TimeOfDay >= timeSpans[2]
                                        && car.TimeStart.Value.TimeOfDay <= timeSpans[3]
                                     select new
                                     {
                                         car,
                                         vethang,
                                         part,
                                         TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                         TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                     };
                        result.AddRange(query3.ToList());
                    }

                    if (isEarlyAfternoon)
                    {
                        var query4 = from car in context.Car
                                     join vethang in context.VeThang on car.IDVeThang equals vethang.ID
                                     join part in context.Part on car.IDPart equals part.ID
                                     where car.TimeEnd.HasValue
                                        && car.TimeEnd.Value.Date >= startDate.Date
                                        && car.TimeEnd.Value.Date <= endDate.Date
                                        && car.TimeEnd.Value.TimeOfDay >= timeSpans[6]
                                        && car.TimeEnd.Value.TimeOfDay <= timeSpans[7]
                                     select new
                                     {
                                         car,
                                         vethang,
                                         part,
                                         TimeStartFormatted = car.TimeStart.HasValue ? car.TimeStart.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                         TimeEndFormatted = car.TimeEnd.HasValue ? car.TimeEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty
                                     };
                        result.AddRange(query4.ToList());
                    }

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

                    exportXlsxButton.Visibility = result.Any() ? Visibility.Visible : Visibility.Collapsed;
                }
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
    }
}
