using System.Windows.Controls;

namespace SodukoGui
{
    /// <summary>
    /// Interaction logic for MainBoard.xaml
    /// </summary>
    public partial class MainBoard : UserControl
    {
        public MainBoard()
        {
            InitializeComponent();
        }

        internal void FillSquares(MainBoardViewModel mainBoardViewModel)
        {
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    SingleSquareViewModel ssvm = mainBoardViewModel.GetSingleSquareViewModel(i, j);
                    MainBoardGrid.Children.Add(new SingleSquare(j, i) { DataContext = ssvm });
                }
            }
        }
    }
}
