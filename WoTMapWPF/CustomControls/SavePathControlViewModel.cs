using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WoTMapWPF.CustomControls
{
    public class SavePathControlViewModel : INotifyPropertyChanged
    {
        private string name = string.Empty;
        private List<string> nameSuggestionValues;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get => name; set { name = value; OnPropertyChanged("Name"); } }
        public List<string> NameSuggestionValues { get => nameSuggestionValues; set { nameSuggestionValues = value; OnPropertyChanged("NameSuggestionValues"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
