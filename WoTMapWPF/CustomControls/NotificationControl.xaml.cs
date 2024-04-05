using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : UserControl
    {
        private CancellationTokenSource cancellationTokenSource;

        public NotificationControl()
        {
            InitializeComponent();
            cancellationTokenSource = new CancellationTokenSource();
        }

        public bool IsError { get => (bool)GetValue(IsErrorProperty); set => SetValue(IsErrorProperty, value); }

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.RegisterAttached("IsError", typeof(bool), typeof(NotificationControl), new PropertyMetadata(false));

        public void ShowNotification(string message)
        {
            IsError = false;
            Show(message);
        }

        public void ShowError(string message)
        {
            IsError = true;
            Show(message);
        }

        public void ShowNotificationAndHide(string message, int delayMilliseconds = 3000)
        {
            IsError = false;
            ShowAndHide(message, delayMilliseconds);
        }
        public void ShowErrorAndHide(string message, int delayMilliseconds = 3000)
        {
            IsError = true;
            ShowAndHide(message, delayMilliseconds);
        }

        private void Show(string message)
        {
            cancellationTokenSource.Cancel();
            NotificationControlViewModel model = (NotificationControlViewModel)DataContext;
            model.Text = message;
            Visibility = Visibility.Visible;
        }

        private async void ShowAndHide(string message, int delayMilliseconds)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            NotificationControlViewModel model = (NotificationControlViewModel)DataContext;
            model.Text = message;
            Visibility = Visibility.Visible;
            await Task.Delay(delayMilliseconds);
            if(!cancellationToken.IsCancellationRequested)
                Visibility = Visibility.Hidden;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
