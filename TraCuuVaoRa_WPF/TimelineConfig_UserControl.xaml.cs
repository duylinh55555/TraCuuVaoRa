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
    /// Interaction logic for TimelineConfig_UserControl.xaml
    /// </summary>
    public partial class TimelineConfig_UserControl : UserControl
    {
        public TimelineConfig_UserControl()
        {
            InitializeComponent();
        }

        public void SetTime(TimeSpan lateStartTime, TimeSpan lateEndTime, TimeSpan earlyStartTime, TimeSpan earlyEndTime)
        {
            lateTimelineSelector.SetTime(lateStartTime, lateEndTime);
            earlyTimelineSelector.SetTime(earlyStartTime, earlyEndTime);
        }

        public void GetTime(out TimeSpan lateStartTime, out TimeSpan lateEndTime, out TimeSpan earlyStartTime, out TimeSpan earlyEndTime)
        {
            lateTimelineSelector.GetTime(out lateStartTime, out lateEndTime);
            earlyTimelineSelector.GetTime(out earlyStartTime, out earlyEndTime);
        }
    }
}
