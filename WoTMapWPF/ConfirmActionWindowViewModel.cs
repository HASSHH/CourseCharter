using System.ComponentModel;

namespace WoTMapWPF
{
    internal class ConfirmActionWindowViewModel : INotifyPropertyChanged
    {
        public string message = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Message { get => message; set { message = value; OnPropertyChanged("Message"); } }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
