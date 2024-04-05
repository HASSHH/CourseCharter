using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WoTMapWPF.CustomControls
{
    public class LoadPathControlViewModel : INotifyPropertyChanged
    {
        private PathFileDefinition? selectedPath;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<PathFileDefinition> Paths { get; set; } = new ObservableCollection<PathFileDefinition>();
        public PathFileDefinition? SelectedPath { get => selectedPath; set { selectedPath = value; OnPropertyChanged("SelectedPath"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
