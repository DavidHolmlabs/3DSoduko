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
    }
}
