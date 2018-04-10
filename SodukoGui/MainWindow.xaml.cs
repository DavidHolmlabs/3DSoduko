using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SodukoGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainBoardViewModel mainBoardViewModel => DataContext as MainBoardViewModel;

        public MainWindow()
        {
            InitializeComponent();
            var mainBoardViewModel = new MainBoardViewModel();

            mainBoard.FillSquares(mainBoardViewModel);

            DataContext = mainBoardViewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainBoardViewModel.Close();
        }

        private void RadioButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainBoardViewModel newVM = e.NewValue as MainBoardViewModel;
            if (newVM == null)
                throw new ArgumentException();

            newVM.PropertyChanged += HookupEvent;
            if (e.OldValue != null && e.OldValue is MainBoardViewModel)
            {
                MainBoardViewModel oldVM = e.OldValue as MainBoardViewModel;
            }
        }

        private void HookupEvent(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainBoardViewModel.FixedDimension))
            {
                if (mainBoardViewModel.FixedDimension == 0)
                    DimensionX.IsChecked = true;
                if (mainBoardViewModel.FixedDimension == 1)
                    DimensionY.IsChecked = true;
                if (mainBoardViewModel.FixedDimension == 2)
                    DimensionZ.IsChecked = true;

            }
            if (e.PropertyName == nameof(MainBoardViewModel.FixedIndex))
            {
                string elementName = "Index" + (mainBoardViewModel.FixedIndex + 1);
                (FindName(elementName) as RadioButton).IsChecked = true;
            }
        }
    }
}
