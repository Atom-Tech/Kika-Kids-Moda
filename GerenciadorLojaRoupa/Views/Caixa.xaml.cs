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

namespace GerenciadorLojaRoupa.Views
{
    /// <summary>
    /// Interaction logic for Caixa.xaml
    /// </summary>
    public partial class Caixa : Page
    {
        public Caixa()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Main.HM.Visibility = Visibility.Visible;
        }
    }
}
