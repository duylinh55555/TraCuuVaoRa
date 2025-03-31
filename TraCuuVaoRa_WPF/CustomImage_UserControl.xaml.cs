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
    /// Interaction logic for CustomImage_UserControl.xaml
    /// </summary>
    public partial class CustomImage_UserControl : UserControl
    {
        private double zoomFactor = 2.0;
        private int zoomSize = 400;

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(ImageSource),
                typeof(CustomImage_UserControl),
                new PropertyMetadata(null, OnSourceChanged));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CustomImage_UserControl;
            if (control != null)
            {
                control.originalImage.Source = e.NewValue as ImageSource;
                control.zoomedImage.Source = e.NewValue as ImageSource;
            }
        }

        public CustomImage_UserControl()
        {
            InitializeComponent();
        }

        public void OriginalImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(originalImage);
            BitmapSource originalBitmap = (BitmapSource)originalImage.Source;

            if (originalBitmap != null)
            {
                int x = (int)(mousePos.X * originalBitmap.PixelWidth / originalImage.ActualWidth);
                int y = (int)(mousePos.Y * originalBitmap.PixelHeight / originalImage.ActualHeight);

                int zoomX = (int)(x - zoomSize / zoomFactor / 2);
                int zoomY = (int)(y - zoomSize / zoomFactor / 2);

                if (zoomX < 0) zoomX = 0;
                if (zoomY < 0) zoomY = 0;
                if (zoomX + zoomSize / zoomFactor > originalBitmap.PixelWidth) zoomX = (int)(originalBitmap.PixelWidth - zoomSize / zoomFactor);
                if (zoomY + zoomSize / zoomFactor > originalBitmap.PixelHeight) zoomY = (int)(originalBitmap.PixelHeight - zoomSize / zoomFactor);

                Int32Rect zoomRect = new Int32Rect(zoomX, zoomY, (int)(zoomSize / zoomFactor), (int)(zoomSize / zoomFactor));
                CroppedBitmap croppedBitmap = new CroppedBitmap(originalBitmap, zoomRect);

                TransformedBitmap scaledBitmap = new TransformedBitmap(croppedBitmap, new ScaleTransform(zoomFactor, zoomFactor));
                zoomedImage.Source = scaledBitmap;
                zoomedImage.Width = zoomSize;
                zoomedImage.Height = zoomSize;

                zoomPopup.IsOpen = true;
            }
            else
            {
                zoomPopup.IsOpen = false;
            }
        }

        private void OriginalImage_MouseLeave(object sender, MouseEventArgs e)
        {
            zoomPopup.IsOpen = false;
        }
    }
}
