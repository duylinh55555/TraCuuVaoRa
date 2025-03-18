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

namespace TraCuuVaoRa_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static DataContext context = new DataContext();

    static BeLateScreen beLateScreen = new BeLateScreen(context);
    static MainScreen mainScreen = new MainScreen(context);
    static ConfigScreen configScreen = new ConfigScreen(context);

    public MainWindow()
    {
        InitializeComponent();
        mainContentControl.Content = beLateScreen;
    }

    private void beLateButton_Click(object sender, RoutedEventArgs e)
    {
        mainContentControl.Content = beLateScreen;
    }

    private void timeSearchButton_Click(object sender, RoutedEventArgs e)
    {
        mainContentControl.Content = mainScreen;
    }

    private void configButton_Click(object sender, RoutedEventArgs e)
    {
        mainContentControl.Content = configScreen;
    }

    private void statisticButton_Click(object sender, RoutedEventArgs e)
    {
        //mainContentControl.Content = 
    }
}