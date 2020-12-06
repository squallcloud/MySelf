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

namespace InchSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double m_BaseInch = 0.0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (double.TryParse(textBox.Text, out double baseInch)) {
                m_BaseInch = baseInch;

                resizeAllRect();
            }
        }

        private void TargetInch1TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (double.TryParse(textBox.Text, out double targetInch))
            {
                resizeRect(TargetBorder1, targetInch);
            }
        }

        private void TargetInch2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (double.TryParse(textBox.Text, out double targetInch))
            {
                resizeRect(TargetBorder2, targetInch);
            }
        }

        void resizeRect(Border border, double targetInch)
        {
            if (m_BaseInch == 0)
            {
                return;
            }

            double ratio = targetInch / m_BaseInch;

            var screenSize = this.RenderSize;
            double w = screenSize.Width;
            double h = screenSize.Height;

            border.Width = w * ratio;
            border.Height = h * ratio;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var screenSize = this.RenderSize;
            double w = screenSize.Width;
            double h = screenSize.Height;

            var textBlock = sender as TextBlock;

            textBlock.Text = $"{(int)w}x{(int)h}";
        }

        private void TargetInch1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(int.TryParse(e.Text, out int result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void TargetInch2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(int.TryParse(e.Text, out int result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void BaseInch_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(int.TryParse(e.Text, out int result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            resizeAllRect();
        }

        void resizeAllRect()
        {
            {
                var screenSize = this.RenderSize;
                double w = screenSize.Width;
                double h = screenSize.Height;

                ScreenSize.Text = $"{(int)w}x{(int)h}";
            }

            {
                if (double.TryParse(TargetInch1.Text, out double targetInch))
                {
                    resizeRect(TargetBorder1, targetInch);
                }
            }

            {
                if (double.TryParse(TargetInch2.Text, out double targetInch))
                {
                    resizeRect(TargetBorder2, targetInch);
                }
            }
        }
    }
}
