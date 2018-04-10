using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;

namespace SodukoGui
{
    /// <summary>
    /// Interaction logic for SingleSquare.xaml
    /// </summary>
    public partial class SingleSquare : UserControl
    {
        private const double thinBorder = 0.5;
        private const double thickBorder = 2.5;
        public SingleSquare(int x, int y)
        {
            InitializeComponent();
            double top = thinBorder;
            double bottom = thinBorder;
            if (x == 0) top = thickBorder;
            if ((x + 1) % 3 == 0)
                bottom = thickBorder;

            double left = thinBorder;
            double right = thinBorder;
            if (y == 0) left = thickBorder;
            if ((y + 1) % 3 == 0)
                right = thickBorder;

            System.Windows.Thickness border = new System.Windows.Thickness(left, top, right, bottom);
            this.BorderThickness = border;
            BorderBrush = new SolidColorBrush(Colors.DarkOliveGreen);
        }
    }
}
