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

namespace KikaKidsModa.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            CampoLogin.Focus();
        }

        private async void BotaoLogin_Click(object sender, RoutedEventArgs e)
        {
            Criptografar cp = new Criptografar();
            if (CampoLogin.Text != "" && CampoSenha.Password != "")
            {
                if ((await Synchro.tbUsuario.ReadAsync()).Where(c => c.Login == CampoLogin.Text
                   && c.Senha == cp.EncryptToString(CampoSenha.Password)).Count() > 0)
                {
                    var u = (await Synchro.tbUsuario.ReadAsync()).Where(c => c.Login == CampoLogin.Text
                        && c.Senha == cp.EncryptToString(CampoSenha.Password)).First();
                    Main.usuarioLogado = u;
                    if (u.NivelAcesso == "Administrador")
                    {
                        Main.HMUser.Visibility = Visibility.Visible;
                        Main.HMRel.Visibility = Visibility.Visible;
                        Main.MainFrame.Navigate(new Caixa());
                    }   
                    else
                    {
                        Main.HMUser.Visibility = Visibility.Collapsed;
                        Main.HMRel.Visibility = Visibility.Collapsed;
                        Main.HM.Content[0].IsSelected = true;
                        Main.HM.Content[10].IsSelected = false;
                        Main.MainFrame.Navigate(new Home());
                        Main.HM.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    MessageBox.Show("Login e/ou senha incorreto(a)", "Aviso");
                }
            }
            else
            {
                MessageBox.Show("Digite um login e uma senha", "Aviso");
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Main.HM.Visibility = Visibility.Collapsed;
        }
        
    }
}
