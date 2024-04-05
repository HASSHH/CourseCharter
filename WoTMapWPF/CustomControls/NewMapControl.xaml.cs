﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
            ResetControl();
        }

        public NewMapControlViewModel ViewModel { get => (NewMapControlViewModel)DataContext; }

        public void ResetControl()
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
    }
}
