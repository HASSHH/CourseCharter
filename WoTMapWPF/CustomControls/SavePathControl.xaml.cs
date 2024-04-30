using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WoTMapWPF.CustomControls
{
    /// <summary>
    /// Interaction logic for SavePathControl.xaml
    /// </summary>
    public partial class SavePathControl : UserControl
    {
        public event EventHandler? SaveButtonClicked;

        public SavePathControl()
        {
            InitializeComponent();
        }

        public string PathName { get => (DataContext as SavePathControlViewModel).Name; }

        public void ResetControl(List<string> existingPaths)
        {
            SavePathControlViewModel model = (SavePathControlViewModel)DataContext;
            model.Name = "path_name";
            for(int i = 0; i < existingPaths.Count; i++)
                existingPaths[i] = System.IO.Path.GetFileNameWithoutExtension(existingPaths[i]);
            model.NameSuggestionValues = existingPaths;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
