using KikaKidsModa.Views;
using KikaKidsModa;
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

namespace KikaKidsModa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        public static bool x = false;
        public static Frame MainFrame;
        public static HamburgerMenu.HamburgerMenu HM;
        public static HamburgerMenu.HamburgerMenuItem HMUser;
        public static double entrada = 0.00;
        public static Model.Usuario usuarioLogado;

        public Main()
        {
            InitializeComponent();
            MainFrame = Root;
            HM = Hamburger;
            HMUser = HMuser;
        }

        private async void Janela_ContentRendered(object sender, EventArgs e)
        {
            await Synchro.InitLocalStoreAsync();
            Loading.Visibility = Visibility.Collapsed;
            Root.Navigate(new Login());
        }

        double width, height;

        private void minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void fechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                maximize.Visibility = Visibility.Collapsed;
                restore.Visibility = Visibility.Visible;
                height = Height;
                width = Width;
                WindowState = WindowState.Maximized;
            }
            else
            {
                maximize.Visibility = Visibility.Visible;
                restore.Visibility = Visibility.Collapsed;
                Height = height;
                Width = width;
                WindowState = WindowState.Normal;
            }
        }

        private bool _isMouseDown = false;

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && this.WindowState == WindowState.Maximized)
            {
                _isMouseDown = false;
                this.WindowState = WindowState.Normal;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            this.DragMove();
        }
        
        private void Home_Selected(object sender, RoutedEventArgs e)
        {
            if (x) Root.Navigate(new Home());
        }
        
        private void AbrirFecharCaixa_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Caixa(false));
        }

        private void TrocarUsuario_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Login());
        }

        private void Usuarios_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Usuario());
        }

        private void Produtos_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Produto());
        }

        private void Vendedores_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Vendedor());
        }

        private void Retirada_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Retirada());
        }

        private void Vender_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Venda());
        }

        private void Clientes_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Cliente());
        }

        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

    }
}
