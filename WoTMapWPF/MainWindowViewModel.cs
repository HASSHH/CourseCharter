using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WoTMapWPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string distanceUnit;
        private double distanceUnitsPerPixel;
        private string mapImageMD5;
        private string mapName;
        private string windowTitleBase;
        private Path path;
        private Path? oldPath;

        public MainWindowViewModel()
        {
            distanceUnit = string.Empty;
            distanceUnitsPerPixel = 1;
            mapImageMD5 = string.Empty;
            mapName = string.Empty;
            windowTitleBase = string.Empty;
            path = new Path();
            oldPath = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string DistanceUnit { get => distanceUnit; set { distanceUnit = value; OnPropertyChanged("DistanceUnit"); } }
        public double DistanceUnitsPerPixel { get => distanceUnitsPerPixel; set { distanceUnitsPerPixel = value; OnPropertyChanged("DistanceUnitsPerPixel"); } }
        public string MapImageMD5 { get => mapImageMD5; set { mapImageMD5 = value; OnPropertyChanged("MapImageMD5"); } }
        public string MapName { get => mapName; set { mapName = value; OnPropertyChanged("MapName"); } }
        public Path Path { get => path; set { OldPath = path; path = value; OnPropertyChanged("Path"); } }
        public Path? OldPath { get => oldPath; set { oldPath = value; OnPropertyChanged("OldPath"); } }
        public string WindowTitleBase { get => windowTitleBase; set { windowTitleBase = value; OnPropertyChanged("WindowTitleBase"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
