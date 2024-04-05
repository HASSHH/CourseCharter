using System;
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

        public void ResetControl()
        {
            SavePathControlViewModel model = (SavePathControlViewModel)DataContext;
            model.Name = "path_name";
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
