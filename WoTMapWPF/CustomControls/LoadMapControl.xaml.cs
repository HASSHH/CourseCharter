using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WoTMapWPF.CustomControls
{
    /// <summary>
    /// Interaction logic for LoadMapControl.xaml
    /// </summary>
    public partial class LoadMapControl : UserControl
    {
        public event EventHandler? LoadButtonClicked;
        public event EventHandler? DeleteRequested;

        public LoadMapControl()
        {
            InitializeComponent();
        }

        public MapFileDefinition Selected { get => ((LoadMapControlViewModel)DataContext).SelectedMap; }

        public void ResetControl(List<MapFileDefinition> maps)
        {
            LoadMapControlViewModel model = (LoadMapControlViewModel)DataContext;
            model.SelectedMap = null;
            model.Maps.Clear();
            maps.Sort((a, b) => a == null ? 1 : a.Name.CompareTo(b.Name));
            foreach (MapFileDefinition map in maps)
                model.Maps.Add(map);
            //also recolor preview button images in case the theme was changed
            model.RecolorBitmaps();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        private async void PreviewButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Button)sender).Content is Image)
            {
                Image image = (Image)((Button)sender).Content;
                if (image.Tag == null)
                {
                    image.Tag = new object();
                    MapFileDefinition map = (MapFileDefinition)((Button)sender).DataContext;
                    int imageHeight = (int)image.Height;
                    string saveLocation = (string)App.Current.Resources["SaveLocation"];
                    string imageLocation = $"{saveLocation}\\maps\\{map.ImageMD5}\\map_image{map.ImageExt}";
                    BitmapImage loadedImage = await Task.Run(() => LoadImage(imageLocation, imageHeight));
                    if (loadedImage != null)
                        image.Source = loadedImage;
                    else
                        image.Tag = null;
                }
            }
        }

        private BitmapImage? LoadImage(string path, int decodeHeight)
        {
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = fileStream;
                    bitmapImage.DecodePixelHeight = decodeHeight;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
