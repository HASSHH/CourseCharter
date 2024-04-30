using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WoTMapWPF.CustomControls
{
    /// <summary>
    /// Interaction logic for NewMapControl.xaml
    /// </summary>
    public partial class NewMapControl : UserControl
    {
        public event EventHandler? SaveButtonClicked;

        public NewMapControl()
        {
            InitializeComponent();
            ResetControl(new List<string>());
        }

        public NewMapControlViewModel ViewModel { get => (NewMapControlViewModel)DataContext; }

        public void ResetControl(List<string> existingMaps)
        {
            NewMapControlViewModel model = ViewModel;
            model.ImageFileName = "-no image selected-";
            model.ImageFilePath = string.Empty;
            model.ImageMD5 = string.Empty;
            model.Name = "map_name";
            model.UnitsPerPixel = 1;
            model.SamplePixels = 1;
            model.SampleUnits = 1;
            model.UnitLabel = "km";
            MapImage.Source = null;
            for (int i = 0; i < existingMaps.Count; i++)
                existingMaps[i] = System.IO.Path.GetFileNameWithoutExtension(existingMaps[i]);
            model.NameSuggestionValues = existingMaps;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                if (ofd.CheckFileExists)
                {
                    try
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(ofd.FileName);
                        bitmapImage.DecodePixelHeight = (int)MapImage.Height;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                        MapImage.Source = bitmapImage;
                        NewMapControlViewModel viewModel = DataContext as NewMapControlViewModel;
                        using (MD5 md5 = MD5.Create())
                        {
                            using (FileStream stream = File.OpenRead(ofd.FileName))
                            {
                                byte[] hashBytes = md5.ComputeHash(stream);
                                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                                viewModel.ImageMD5 = hashString;
                            }
                        }
                        viewModel.ImageFilePath = ofd.FileName;
                        viewModel.ImageFileName = ofd.SafeFileName;
                    }
                    catch { }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Filter that allows only alphanum characters.
        /// Handler for TextChanged event of a TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlphaNumFilter_TextChanged(object sender, EventArgs e)
        {
            TextBox textboxSender = (TextBox)sender;
            int cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Z _.-]", "");
            textboxSender.SelectionStart = cursorPosition;
        }

        /// <summary>
        /// Filter that allows only numerical characters.
        /// Handler for TextChanged event of a TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumFilter_TextChanged(object sender, EventArgs e)
        {
            TextBox textboxSender = (TextBox)sender;
            int cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9]", "");
            textboxSender.SelectionStart = cursorPosition;
        }

        /// <summary>
        /// Filter out characters that are not valid for a file's name.
        /// Handler for TextChanged event of a TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textboxSender = (TextBox)sender;
            int cursorPosition = textboxSender.SelectionStart;
            char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            char[] invalidPathChars = System.IO.Path.GetInvalidPathChars();
            char[] invalidChars = invalidFileNameChars.Concat(invalidPathChars).ToArray();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in textboxSender.Text)
                if (!invalidChars.Contains(c))
                    stringBuilder.Append(c);
            textboxSender.Text = stringBuilder.ToString();
            textboxSender.SelectionStart = cursorPosition;
        }
    }
}
