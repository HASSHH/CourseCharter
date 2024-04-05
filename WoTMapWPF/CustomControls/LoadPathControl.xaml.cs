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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WoTMapWPF.CustomControls
{
    /// <summary>
    /// Interaction logic for LoadPathControl.xaml
    /// </summary>
    public partial class LoadPathControl : UserControl
    {
        public event EventHandler? LoadButtonClicked;
        public event EventHandler? DeleteRequested;

        public LoadPathControl()
        {
            InitializeComponent();
        }

        public PathFileDefinition? SelectedPath { get => ((LoadPathControlViewModel)DataContext).SelectedPath; }

        public void ResetControl(List<PathFileDefinition> paths)
        {
            LoadPathControlViewModel model = (LoadPathControlViewModel)DataContext;
            model.SelectedPath = null;
            model.Paths.Clear();
            paths.Sort((a,b) => a == null ? 1 : a.Name.CompareTo(b.Name));
            foreach (PathFileDefinition path in paths)
                model.Paths.Add(path);

        }
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
