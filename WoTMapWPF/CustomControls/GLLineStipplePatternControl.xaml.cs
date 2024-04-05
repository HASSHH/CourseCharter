using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WoTMapWPF.CustomControls
{
    /// <summary>
    /// A control that show a line formed of 16 squares that represent the bits of a short value.
    /// The 0 and 1 values are represented by different colors according to the theme.
    /// Initialized by the short value of "LineStipplePattern" resource.
    /// LMB click on a "bit square" will set its value to 1, RMB will set it to 0.
    /// PatternChanged event will fire when a change is made.
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

        /// <summary>
        /// Reset view to match value of "LineStipplePattern" resource
        /// </summary>
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
