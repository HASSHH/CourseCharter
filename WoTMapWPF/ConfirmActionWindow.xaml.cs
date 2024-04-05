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
using System.Windows.Shapes;

namespace WoTMapWPF
{
    /// <summary>
    /// Interaction logic for ConfirmActionWindow.xaml
    /// </summary>
    public partial class ConfirmActionWindow : Window
    {
        public ConfirmActionWindow(string message)
        {
            InitializeComponent();
            ((ConfirmActionWindowViewModel)DataContext).Message = message;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= false;
        }
    }
}
