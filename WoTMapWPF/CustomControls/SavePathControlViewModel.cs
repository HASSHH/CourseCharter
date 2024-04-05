using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WoTMapWPF.CustomControls
{
    public class SavePathControlViewModel : INotifyPropertyChanged
    {
        private string name = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        public string Name { get => name; set { name = value; OnPropertyChanged("Name"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
