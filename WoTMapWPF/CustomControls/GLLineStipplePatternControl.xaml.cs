using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for GLLineStipplePatternControl.xaml
    /// </summary>
    public partial class GLLineStipplePatternControl : UserControl
    {
        GLLineStipplePatternControlViewModel viewModel;

        public event EventHandler<short>? PatternChanged;

        public GLLineStipplePatternControl()
        {
            InitializeComponent();
            viewModel = (GLLineStipplePatternControlViewModel)DataContext;
            PopulateBitContainer();
            viewModel.PropertyChanged += OnPropertyChanged;
        }

        public void Reload()
        {
            viewModel.InitializeBitList();
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StipplePattern")
                PatternChanged?.Invoke(this, viewModel.StipplePattern);
        }

        private void PopulateBitContainer()
        {
            Grid bitGrid;
            for (int i = 0; i < 16; i++)
            {
                bitGrid = new Grid();
                Grid.SetColumn(bitGrid, i);
                Binding tagBinding = new Binding("Value");
                tagBinding.Source = viewModel.BitList[i];
                bitGrid.SetBinding(Grid.TagProperty, tagBinding);
                bitGrid.SetResourceReference(Grid.StyleProperty, "BitGridStyle");
                bitGrid.PreviewMouseLeftButtonDown += BitGrid_PreviewMouseLeftButtonDown;
                bitGrid.PreviewMouseRightButtonDown += BitGrid_PreviewMouseRightButtonDown;
                bitGrid.MouseEnter += BitGrid_MouseEnter;
                BitContainer.Children.Add(bitGrid);
            }
            
        }

        private void BitGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ActivateBit(sender);
        }

        private void BitGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DeactivateBit(sender);
        }

        private void BitGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ActivateBit(sender);
            else if (e.RightButton == MouseButtonState.Pressed)
                DeactivateBit(sender);
        }

        private void ActivateBit(object bitContainer)
        {
            Grid bitGrid = (Grid)bitContainer;
            int position = Grid.GetColumn(bitGrid);
            if (viewModel.BitList[position].Value == false)
                viewModel.BitList[position].Value = true;
        }

        private void DeactivateBit(object bitContainer)
        {
            Grid bitGrid = (Grid)bitContainer;
            int position = Grid.GetColumn(bitGrid);
            if (viewModel.BitList[position].Value == true)
                viewModel.BitList[position].Value = false;
        }
    }
}
