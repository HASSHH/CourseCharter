using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace WoTMapWPF.CustomControls
{
    internal class SettingsControlViewModel : INotifyPropertyChanged
    {
        private WriteableBitmap pinBitmap;
        private WriteableBitmap pinSelectedBitmap;
        private WriteableBitmap dashedPathBitmap;
        private Color colorA;
        private Color colorB;
        private Color colorC;
        private bool isPathAutosaveEnabled;
        private int lineStippleFactor;
        private double lineWidth;
        private double pinSize;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsControlViewModel()
        {
            InitializeViewBinding();
            int imageHeight = 300;
            BitmapImage bitmapImage = new BitmapImage();
            Uri uri = new Uri("../Res/pinA.png", UriKind.Relative);
            StreamResourceInfo sri = App.GetResourceStream(uri);
            using (Stream stream = sri.Stream)
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.DecodePixelHeight = imageHeight;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                pinBitmap = new WriteableBitmap(bitmapImage);
            }
            uri = new Uri("../Res/pinB.png", UriKind.Relative);
            sri = App.GetResourceStream(uri);
            using (Stream stream = sri.Stream)
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.DecodePixelHeight = imageHeight;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                pinSelectedBitmap = new WriteableBitmap(bitmapImage);
            }
            uri = new Uri("../Res/dashed_path.png", UriKind.Relative);
            sri = App.GetResourceStream(uri);
            using (Stream stream = sri.Stream)
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.DecodePixelHeight = imageHeight;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                dashedPathBitmap = new WriteableBitmap(bitmapImage);
            }
        }

        public WriteableBitmap PinBitmap { get => pinBitmap; set { pinBitmap = value; OnPropertyChanged("PinBitmap"); } }
        public WriteableBitmap PinSelectedBitmap { get => pinSelectedBitmap; set { pinSelectedBitmap = value; OnPropertyChanged("PinSelectedBitmap"); } }
        public WriteableBitmap DashedPathBitmap { get => dashedPathBitmap; set { dashedPathBitmap = value; OnPropertyChanged("DashedPathBitmap"); } }
        public Color ColorA { get => colorA; set { colorA = value; OnPropertyChanged("ColorA"); } }
        public Color ColorB { get => colorB; set { colorB = value; OnPropertyChanged("ColorB"); } }
        public Color ColorC { get => colorC; set { colorC = value; OnPropertyChanged("ColorC"); } }
        public int LineStippleFactor { get => lineStippleFactor; set { lineStippleFactor = value; OnPropertyChanged("LineStippleFactor"); } }
        public double LineWidth { get => lineWidth; set { lineWidth = value; OnPropertyChanged("LineWidth"); } }
        public double PinSize { get => pinSize; set { pinSize = value; OnPropertyChanged("PinSize"); } }
        public bool IsPathAutosaveEnabled { get => isPathAutosaveEnabled; set { isPathAutosaveEnabled = value; OnPropertyChanged("IsPathAutosaveEnabled"); } }

        public void InitializeViewBinding()
        {
            ColorA = ((SolidColorBrush)App.Current.Resources["PinColor"]).Color;
            ColorB = ((SolidColorBrush)App.Current.Resources["PinSelectedColor"]).Color;
            ColorC = ((SolidColorBrush)App.Current.Resources["DashedPathColor"]).Color;
            if (App.Current.Resources.Contains("IsPathAutosaveEnabled"))
                IsPathAutosaveEnabled = (bool)App.Current.Resources["IsPathAutosaveEnabled"];
            LineStippleFactor = (int)App.Current.Resources["LineStippleFactor"];
            LineWidth = (double)App.Current.Resources["LineWidth"];
            PinSize = (double)App.Current.Resources["PinSize"];
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
