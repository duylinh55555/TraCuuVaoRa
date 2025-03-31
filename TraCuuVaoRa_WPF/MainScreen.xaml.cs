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
using Microsoft.EntityFrameworkCore;
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

        enum NotificateType
        {
            Success,
            Error,
            Warning
        }

        public MainScreen(DataContext dataContext)
        {
            context = dataContext;

            startTime = context.timeSpans[0];
            endTime = context.timeSpans[7];

            InitializeComponent();

            dateSelector.SetDate(startDate, endDate);
            SetTime(startTime, endTime);

            // default image
            vehiclePersonDataGrid.xeRa_CustomImage = new BitmapImage(new Uri("pack://application:,,,/Images/blankimage.png", UriKind.Absolute));
            vehiclePersonDataGrid.xeVao_CustomImage = new BitmapImage(new Uri("pack://application:,,,/Images/blankimage.png", UriKind.Absolute));

            vehiclePersonDataGrid.dataContext = dataContext;
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

                    vehiclePersonDataGrid.dataGrid.ItemsSource = indexedResult;

                    // Show, Hide Export Button
                    exportXlsxButton.Visibility = result.Any() ? Visibility.Visible : Visibility.Collapsed;

                    var serverDateTime = context.Database.SqlQueryRaw<DateTime>("SELECT GETDATE() AS Value").Single();
                    Notificate(NotificateType.Success, serverDateTime);
                }
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLower();
            vehiclePersonDataGrid.Search_DataGrid(searchText);
        }

        private void exportXlsxButton_Click(object sender, RoutedEventArgs e)
        {
            vehiclePersonDataGrid.ExportToFile();
        }

        private void Notificate(NotificateType type, DateTime serverDateTime)
        {
            switch (type)
            {
                case NotificateType.Success:
                    notificateTextBlock.Text = $"Tra cứu thành công (Dữ liệu được cập nhật lúc {serverDateTime:HH:mm:ss dd/MM/yy})"; 
                    notificateTextBlock.Foreground = Brushes.Green;
                    break;

                case NotificateType.Error:
                    notificateTextBlock.Text = "Không thể kết nối tới CSDL";
                    notificateTextBlock.Foreground = Brushes.Red;
                    break;

                case NotificateType.Warning:
                    notificateTextBlock.Text = ""; 
                    notificateTextBlock.Foreground = Brushes.Yellow;
                    break;

                default:
                    break;
            }
        }
        #endregion

    }
}
