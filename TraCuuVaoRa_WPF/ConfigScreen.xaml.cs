using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
    public partial class ConfigScreen : UserControl
    {
        static DataContext? context;
        static TimeSpan[] timeSpans;

        public ConfigScreen(DataContext dataContext)
        {
            context = dataContext;
            timeSpans = context.timeSpans;
            InitializeComponent();
            LoadTime(timeSpans);
            setDefault();
        }

        private void setDefault()
        {
            if (context != null)
            {
                dataSourceTextBox.Text = context.dataSource;
                userIdTextBox.Text = context.userId;
                passwordBox.Password = context.password;
                xeVaoImageFolderTextBox.Text = context.xeVaoImageFolderUrl;
                xeRaImageFolderTextBox.Text = context.xeRaImageFolderUrl;
            }

            string configPath = "config.bin";
            if (System.IO.File.Exists(configPath))
            {
                using (var fs = new System.IO.FileStream(configPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (var br = new System.IO.BinaryReader(fs))
                    {
                        string dataSource = br.ReadString();
                        string userId = br.ReadString();
                        string password = br.ReadString();
                        dataSourceTextBox.Text = dataSource;
                        userIdTextBox.Text = userId;
                        passwordBox.Password = password;

                        TimeSpan temp_time;
                        for (int i = 0; i < timeSpans.Length; i++)
                        {
                            if (TimeSpan.TryParse(br.ReadString(), out temp_time))
                                timeSpans[i] = temp_time;
                        }
                        LoadTime(timeSpans);

                        xeVaoImageFolderTextBox.Text = br.ReadString();
                        xeRaImageFolderTextBox.Text = br.ReadString();
                    }
                }
            }
        }

        private void LoadTime(TimeSpan[] timeSpans)
        {
            if (context != null)
            {
                morningTimelineConfig.SetTime(timeSpans[0], timeSpans[1], timeSpans[2], timeSpans[3]);
                afternoonTimelineConfig.SetTime(timeSpans[4], timeSpans[5], timeSpans[6], timeSpans[7]);
            }
        }

        private void saveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string dataSource = dataSourceTextBox.Text ?? "";
            string userId = userIdTextBox.Text ?? "";
            string password = "";
            string xeVaoUrl = xeVaoImageFolderTextBox.Text ?? "";
            string xeRaUrl = xeRaImageFolderTextBox.Text ?? "";

            // Handle PasswordBox
            SecureString securePassword = passwordBox.SecurePassword;

            if (securePassword != null && securePassword.Length > 0)
                password = ConvertSecureStringToString(securePassword);

            // Get time
            morningTimelineConfig.GetTime(out timeSpans[0], out timeSpans[1], out timeSpans[2], out timeSpans[3]);
            afternoonTimelineConfig.GetTime(out timeSpans[4], out timeSpans[5], out timeSpans[6], out timeSpans[7]);

            // Save config details to file
            string configPath = "config.bin";
            using (var fs = new System.IO.FileStream(configPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                using (var bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(dataSource);
                    bw.Write(userId);
                    bw.Write(password);

                    foreach (var time in timeSpans)
                        bw.Write(time.ToString());

                    bw.Write(xeVaoUrl);
                    bw.Write(xeRaUrl);
                }
            }

            if (context != null)
            {
                context.SetDatabaseCredentials(dataSource, userId, password);
                bool isConnected = context.CheckDatabaseConnection(dataSource, userId, password);
                context.timeSpans = timeSpans;
                context.xeVaoImageFolderUrl = xeVaoUrl;
                context.xeRaImageFolderUrl = xeRaUrl;

                if (isConnected)
                    MessageBox.Show("Lưu thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Lưu thành công. Không kết nối được tới CSDL", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void checkConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string dataSource = dataSourceTextBox.Text ?? "";
            string userId = userIdTextBox.Text ?? "";
            string password = "";

            // Handle PasswordBox
            SecureString securePassword = passwordBox.SecurePassword;

            if (securePassword != null && securePassword.Length > 0)
                password = ConvertSecureStringToString(securePassword);

            if (context != null)
            {
                bool isConnected = context.CheckDatabaseConnection(dataSource, userId, password);

                if (isConnected)
                    MessageBox.Show("Kết nối thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Không thể kết nối.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ConvertSecureStringToString(SecureString secureString)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
