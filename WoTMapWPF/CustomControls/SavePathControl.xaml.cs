using System;
using System.Collections.Generic;
using System.Linq;
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
