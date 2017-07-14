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
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Media.Animation;

namespace KikaKidsModa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        private bool max = false;
        public static bool x = false;
        public static Frame MainFrame;
        public static HamburgerMenu.HamburgerMenu HM;
        public static HamburgerMenu.HamburgerMenuItem HMUser;
        public static HamburgerMenu.HamburgerMenuItem HMRel;
        public static double entrada = 0.00;
        public static Model.Usuario usuarioLogado;
        public static bool HasInternet;
        Atualizar a;

        public Main()
        {
            InitializeComponent();
            a = new Atualizar(barraProgresso, porcentagem, versao);
            MainFrame = Root;
            HM = Hamburger;
            HMUser = HMuser;
            HMRel = HMRelatorio;
            HasInternet = CheckInternet();
            if (HasInternet)
            {
                MensagemSync.Text = "Conectado";
                MensagemSync.Foreground = Brushes.LightGreen;
            }
            else
            {
                MensagemSync.Text = "Desconectado";
                MensagemSync.Foreground = Brushes.LightPink;
            }
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            if (a.PossuiAtualizacao)
            {
                Storyboard s = (Storyboard)this.Resources["UpdateBlink"];
                s.Begin();
            }
        }

        public static bool CheckInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private async void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            HasInternet = e.IsAvailable;
            if (HasInternet)
            {
                await Synchro.SyncAsync();
                Dispatcher.Invoke(() =>
                {
                    MensagemSync.Text = "Conectado";
                    MensagemSync.Foreground = Brushes.LightGreen;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MensagemSync.Text = "Desconectado";
                    MensagemSync.Foreground = Brushes.LightPink;
                });
            }
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
            if (!max)
            {
                maximize.Visibility = Visibility.Collapsed;
                restore.Visibility = Visibility.Visible;
                height = Height;
                width = Width;
                max = true;
                Top = 0;
                Left = 0;
                Height = SystemParameters.WorkArea.Height;
                Width = SystemParameters.WorkArea.Width;
            }
            else
            {
                maximize.Visibility = Visibility.Visible;
                restore.Visibility = Visibility.Collapsed;
                Height = height;
                Width = width;
                max = false;
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
            Root.Navigate(new Home());
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

        private void Config_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Config());
        }

        private void Relatorios_Selected(object sender, RoutedEventArgs e)
        {
            Root.Navigate(new Relatorios());
        }

        private void botaoAtualizar_Click(object sender, RoutedEventArgs e)
        {
            if (a != null)
            {
                try
                {
                    if (barraProgresso.Visibility == Visibility.Hidden)
                    {
                        if (a.PossuiAtualizacao)
                        {
                            if (a.ArquivoExiste())
                                a.Instalar();
                            else
                                a.FazerDownload();
                        }
                        else
                        {
                            MessageBox.Show("Não há atualizações por enquanto");
                        }
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("O nome remoto não pôde ser resolvido"))
                {
                    MessageBox.Show("Você não tem internet");
                }
            }
        }


        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

    }
}
