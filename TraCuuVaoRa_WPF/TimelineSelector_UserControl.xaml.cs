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
    /// Interaction logic for TimelineSelector_UserControl.xaml
    /// </summary>
    public partial class TimelineSelector_UserControl : UserControl
    {
        public TimelineSelector_UserControl()
        {
            InitializeComponent();

            for (int i = 0; i < 24; i++)
            {
                startHourComboBox.Items.Add(i.ToString("D2")); // D2: 2 digits
                endHourComboBox.Items.Add(i.ToString("D2")); // D2: 2 digits
            }

            for (int i = 0; i < 60; i++)
            {
                startMinuteComboBox.Items.Add(i.ToString("D2"));
                endMinuteComboBox.Items.Add(i.ToString("D2"));
            }
        }

        public void SetTime(TimeSpan startTime, TimeSpan endTime)
        {
            startHourComboBox.SelectedIndex = startTime.Hours;
            endHourComboBox.SelectedIndex = endTime.Hours;
            startMinuteComboBox.SelectedIndex = startTime.Minutes;
            endMinuteComboBox.SelectedIndex = endTime.Minutes;
        }

        public void GetTime(out TimeSpan startTime, out TimeSpan endTime)
        {
            string startHour = startHourComboBox.SelectedItem.ToString() ?? "0";
            string startMinute = startMinuteComboBox.SelectedItem.ToString() ?? "0";
            string endHour = endHourComboBox.SelectedItem.ToString() ?? "0";
            string endMinute = endMinuteComboBox.SelectedItem.ToString() ?? "0";

            startTime = new TimeSpan(int.Parse(startHour), int.Parse(startMinute), 0);
            endTime = new TimeSpan(int.Parse(endHour), int.Parse(endMinute), 0);
        }
    }
}
