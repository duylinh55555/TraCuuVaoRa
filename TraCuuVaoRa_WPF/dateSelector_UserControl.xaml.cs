using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
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

namespace TraCuuVaoRa_WPF
{
    /// <summary>
    /// Interaction logic for dateSelector_UserControl.xaml
    /// </summary>
    public partial class dateSelector_UserControl : UserControl
    {
        static private DateTime startDate { get; set; }
        static private DateTime endDate { get; set; }

        public dateSelector_UserControl()
        {
            InitializeComponent();
        }

        public DateTime StartDate()
        {
            return startDate;
        }

        public DateTime EndDate()
        {
            return endDate;
        }

        public void SetDate(DateTime start, DateTime end)
        {
            startDate = start;
            endDate = end;
            startDateButton.Content = startDate.ToString("dd/MM/yyyy");
            endDateButton.Content = endDate.ToString("dd/MM/yyyy");
        }

        private void startDateButton_Click(object sender, RoutedEventArgs e)
        {
            startDateCalendarPopup.IsOpen = !startDateCalendarPopup.IsOpen;
        }

        private void endDateButton_Click(object sender, RoutedEventArgs e)
        {
            endDateCalendarPopup.IsOpen = !endDateCalendarPopup.IsOpen;
        }

        private void startDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDateCalendar.SelectedDate.HasValue)
            {
                startDate = startDateCalendar.SelectedDate.Value;
                startDateButton.Content = startDate.ToString("dd/MM/yyyy");
                startDateCalendarPopup.IsOpen = false;
            }
        }

        private void endDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (endDateCalendar.SelectedDate.HasValue)
            {
                endDate = endDateCalendar.SelectedDate.Value;
                endDateButton.Content = endDate.ToString("dd/MM/yyyy");
                endDateCalendarPopup.IsOpen = false;
            }
        }
    }
}
