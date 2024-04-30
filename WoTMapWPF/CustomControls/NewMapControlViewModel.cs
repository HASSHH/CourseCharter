using System.Collections.Generic;
using System.ComponentModel;

namespace WoTMapWPF.CustomControls
{
    public class NewMapControlViewModel : INotifyPropertyChanged
    {
        private string name;
        private string unitLabel;
        private int sampleUnits;
        private int samplePixels;
        private double unitsPerPixel;
        private string imageFileName;
        private string imageFilePath;
        private string imageMD5;
        private List<string> nameSuggestionValues;

        public event PropertyChangedEventHandler? PropertyChanged;

        public NewMapControlViewModel()
        {
            PropertyChanged += NewMapWindowViewModel_PropertyChanged;
        }

        public string Name { get => name; set { name = value; OnPropertyChanged("Name"); } }
        public string UnitLabel { get => unitLabel; set { unitLabel = value; OnPropertyChanged("UnitLabel"); } }
        public int SampleUnits { get => sampleUnits; set { sampleUnits = value; OnPropertyChanged("SampleUnits"); } }
        public int SamplePixels { get => samplePixels; set { samplePixels = value; OnPropertyChanged("SamplePixels"); } }
        public double UnitsPerPixel { get => unitsPerPixel; set { unitsPerPixel = value; OnPropertyChanged("UnitsPerPixel"); } }
        public string ImageFileName { get => imageFileName; set { imageFileName = value; OnPropertyChanged("ImageFileName"); } }
        public string ImageFilePath { get => imageFilePath; set { imageFilePath = value; OnPropertyChanged("ImageFilePath"); } }
        public string ImageMD5 { get => imageMD5; set { imageMD5 = value; OnPropertyChanged("ImageMD5"); } }
        public List<string> NameSuggestionValues { get => nameSuggestionValues; set { nameSuggestionValues = value; OnPropertyChanged("NameSuggestionValues"); } }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void NewMapWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SampleUnits":
                    {
                        if (sampleUnits < 1)
                            sampleUnits = 1;
                        UnitsPerPixel = (double)sampleUnits / samplePixels;
                        break;
                    }
                case "SamplePixels":
                    {
                        if (samplePixels < 1)
                            samplePixels = 1;
                        UnitsPerPixel = (double)sampleUnits / samplePixels;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
