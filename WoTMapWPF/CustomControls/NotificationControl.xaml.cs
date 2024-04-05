using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        /// <summary>
        /// Show a message using the "notification colors"
        /// </summary>
        /// <param name="message"></param>
        public void ShowNotification(string message)
        {
            IsError = false;
            Show(message);
        }

        /// <summary>
        /// Show a message using the "error colors"
        /// </summary>
        /// <param name="message"></param>
        public void ShowError(string message)
        {
            IsError = true;
            Show(message);
        }

        /// <summary>
        /// Show a message using the "notification colors" and hides it after some time
        /// </summary>
        /// <param name="message"></param>
        public void ShowNotificationAndHide(string message, int delayMilliseconds = 3000)
        {
            IsError = false;
            ShowAndHide(message, delayMilliseconds);
        }

        /// <summary>
        /// Show a message using the "error colors" and hides it after some time
        /// </summary>
        /// <param name="message"></param>
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
            if (!cancellationToken.IsCancellationRequested)
                Visibility = Visibility.Hidden;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
