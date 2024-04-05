using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace WoTMapWPF.CustomControls
{
    public class LoadMapControlViewModel : INotifyPropertyChanged
    {
        private MapFileDefinition selectedMap;
        private WriteableBitmap previewOnBitmap;
        private WriteableBitmap previewOffBitmap;

        public event PropertyChangedEventHandler? PropertyChanged;

        public LoadMapControlViewModel()
        {
            int imageHeight = 64;
            BitmapImage bitmapImage = new BitmapImage();
            Uri uri = new Uri("../Res/preview_off.png", UriKind.Relative);
            StreamResourceInfo sri = App.GetResourceStream(uri);
            using (Stream stream = sri.Stream)
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.DecodePixelHeight = imageHeight;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                previewOffBitmap = new WriteableBitmap(bitmapImage);
            }
            uri = new Uri("../Res/preview_on.png", UriKind.Relative);
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
                previewOnBitmap = new WriteableBitmap(bitmapImage);
            }
            RecolorBitmaps();
        }

        public ObservableCollection<MapFileDefinition> Maps { get; set; } = new ObservableCollection<MapFileDefinition>();
        public MapFileDefinition SelectedMap { get => selectedMap; set { selectedMap = value; OnPropertyChanged("SelectedMap"); } }
        public WriteableBitmap PreviewOnBitmap { get => previewOnBitmap; set { previewOnBitmap = value; OnPropertyChanged("PreviewOnBitmap"); } }
        public WriteableBitmap PreviewOffBitmap { get => previewOffBitmap; set { previewOffBitmap = value; OnPropertyChanged("PreviewOffBitmap"); } }

        public void RecolorBitmaps()
        {
            SolidColorBrush brush = (SolidColorBrush)App.Current.Resources["ThemeColorText"];
            if (PreviewOffBitmap != null)
                BitmapColorChanger.ChangeColorKeepAlpha(PreviewOffBitmap, brush.Color);
            if (PreviewOnBitmap != null)
                BitmapColorChanger.ChangeColorKeepAlpha(PreviewOnBitmap, brush.Color);
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
