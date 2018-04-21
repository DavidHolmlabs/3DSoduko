using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SodukoGui
{
    /// <summary>
    /// Interaction logic for DimensionSelector.xaml
    /// </summary>
    public partial class DimensionSelector : UserControl
    {
        MainBoardViewModel mainBoardViewModel => DataContext as MainBoardViewModel;

        public DimensionSelector()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register(
                "Index",
                typeof(int),
                typeof(DimensionSelector),
                new FrameworkPropertyMetadata(
                    0,
                    new PropertyChangedCallback(IndexChanged))
                        { BindsTwoWayByDefault = true,
                        DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        public static readonly DependencyProperty DimensionProperty =
            DependencyProperty.Register(
                "Dimension",
                typeof(int),
                typeof(DimensionSelector),
                new FrameworkPropertyMetadata(
                    0,
                    new PropertyChangedCallback(DimensionChanged))
                        { BindsTwoWayByDefault = true,
                        DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

        private static void DimensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int newvalue = 1 + (int)e.NewValue;
            string radiobutton = "Dimension" + newvalue;
            ((RadioButton)((DimensionSelector)d).FindName(radiobutton)).IsChecked = true;
        }

        private static void IndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int newvalue = 1 + (int)e.NewValue;
            string radiobutton = "Index" + newvalue;
            ((RadioButton)((DimensionSelector)d).FindName(radiobutton)).IsChecked = true;
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public int Dimension
        {
            get { return (int)GetValue(DimensionProperty); }
            set { SetValue(DimensionProperty, value); }
        }

        private void Index_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            int value = int.Parse(r.Content.ToString());
            Index = value - 1;
        }

        private void Dimension_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            string content = r.Content.ToString();
            switch (content)
            {
                case "X":
                    Dimension = 0;
                    break;
                case "Y":
                    Dimension = 1;
                    break;
                case "Z":
                    Dimension = 2;
                    break;
                default:
                    Dimension = 0;
                    break;
            }
        }
    }
}
