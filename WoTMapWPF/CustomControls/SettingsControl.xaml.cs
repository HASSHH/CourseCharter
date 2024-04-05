using ColorPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

namespace WoTMapWPF.CustomControls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            SettingsControlViewModel viewModel = (SettingsControlViewModel)DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            //preselect the correct theme
            SelectCorrectListViewThemeItem();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SettingsControlViewModel viewModel = (SettingsControlViewModel)DataContext;
            switch (e.PropertyName)
            {
                case "IsPathAutosaveEnabled":
                    ((App)(App.Current)).ChangeUserSetting("IsPathAutosaveEnabled", viewModel.IsPathAutosaveEnabled);
                    break;
                case "LineStippleFactor":
                    ((App)(App.Current)).ChangeUserSetting("LineStippleFactor", viewModel.LineStippleFactor);
                    break;
                case "LineWidth":
                    ((App)(App.Current)).ChangeUserSetting("LineWidth", viewModel.LineWidth);
                    break;
                case "PinSize":
                    ((App)(App.Current)).ChangeUserSetting("PinSize", viewModel.PinSize);
                    break;
            }
        }

        private void ColorPickerA_ColorChanged(object sender, RoutedEventArgs e)
        {
            Color selectedColor = ((SquarePicker)sender).SelectedColor;
            SettingsControlViewModel viewModel = (SettingsControlViewModel)DataContext;
            WriteableBitmap writeableBitmap = viewModel.PinBitmap;
            ChangeColorSetting("PinColor", selectedColor, writeableBitmap);
        }

        private void ColorPickerB_ColorChanged(object sender, RoutedEventArgs e)
        {
            Color selectedColor = ((SquarePicker)sender).SelectedColor;
            SettingsControlViewModel viewModel = (SettingsControlViewModel)DataContext;
            WriteableBitmap writeableBitmap = viewModel.PinSelectedBitmap;
            ChangeColorSetting("PinSelectedColor", selectedColor, writeableBitmap);
        }

        private void ColorPickerC_ColorChanged(object sender, RoutedEventArgs e)
        {
            Color selectedColor = ((SquarePicker)sender).SelectedColor;
            SettingsControlViewModel viewModel = (SettingsControlViewModel)DataContext;
            WriteableBitmap writeableBitmap = viewModel.DashedPathBitmap;
            ChangeColorSetting("DashedPathColor", selectedColor, writeableBitmap);
        }
        private void ChangeColorSetting(string settingName, Color newColor, WriteableBitmap writeableBitmap)
        {
            ((App)(App.Current)).ChangeUserSetting(settingName, new SolidColorBrush(newColor));
            BitmapColorChanger.ChangeColorKeepAlpha(writeableBitmap, newColor);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmActionWindow caw = new ConfirmActionWindow("Are you sure you want to restore default settings?");
            if (caw.ShowDialog().GetValueOrDefault())
            {
                ((App)App.Current).RestoreDefaultSettings();
                SettingsControlViewModel viewModel = (SettingsControlViewModel)DataContext;
                viewModel.InitializeViewBinding();
                ColorPickerA.SelectedColor = viewModel.ColorA;
                ColorPickerB.SelectedColor = viewModel.ColorB;
                ColorPickerC.SelectedColor = viewModel.ColorC;
                LineStippleControl.Reload();
                //apply and select correct theme
                SelectCorrectListViewThemeItem();
            }
        }

        private void SelectCorrectListViewThemeItem()
        {
            if (App.Current.Resources.Contains("ThemeName"))
            {
                string themeName = (string)App.Current.Resources["ThemeName"];
                ListViewItem item = ThemeListView.Items.Cast<ListViewItem>().Where(e => (string)e.Tag == themeName).FirstOrDefault();
                if (item != null)
                    ThemeListView.SelectedItem = item;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem selectedItem = (ListViewItem)listView.SelectedItem;
            if (selectedItem != null)
            {
                string themeName = (string)selectedItem.Tag;
                ((App)App.Current).ChangeTheme(themeName);
                ((App)(App.Current)).ChangeUserSetting("ThemeName", themeName);
            }
        }

        private void GLLineStipplePatternControl_PatternChanged(object sender, short e)
        {
            ((App)(App.Current)).ChangeUserSetting("LineStipplePattern", e);
        }
    }
}
