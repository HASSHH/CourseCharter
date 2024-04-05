using System.ComponentModel;

namespace WoTMapWPF.CustomControls
{
    internal class NotificationControlViewModel : INotifyPropertyChanged
    {
        private string text;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Text { get => text; set { text = value; OnPropertyChanged("Text"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
