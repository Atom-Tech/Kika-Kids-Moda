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
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Page
    {
        bool mudarSenha = false;
        bool cpfValido = false;
        Model.Usuario User;

        public Config()
        {
            InitializeComponent();
            DataContext = this;
            User = Main.usuarioLogado;
            AtivarGrupo(false);
        }

        private void CampoCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            cpfValido = Validar.CPF(CampoCPF.Text);
            CampoCPF.BorderBrush = cpfValido ? Brushes.Green : Brushes.Red;
            if (cpfValido) User.CPF = CampoCPF.Text;
        }

        private void CampoLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.Login = CampoLogin.Text;
        }

        private void CampoNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.Nome = CampoNome.Text;
        }

        private void CampoEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.Endereco = CampoEnd.Text;
        }

        private void CampoEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            User.Email = CampoEmail.Text;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AtivarGrupo(true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AtivarGrupo(false);
        }

        public void AtivarGrupo(bool vf)
        {
            mudarSenha = vf;
            CampoSenhaAntiga.IsEnabled = vf;
            CampoSenhaNova.IsEnabled = vf;
            CampoConfirmar.IsEnabled = vf;
        }

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await SemUnique())
                {
                    if (await InsertUpdate())
                    {
                        Main.usuarioLogado = User;
                        MessageBox.Show("Usuário alterado com sucesso");
                    }
                }
                else
                {
                    MessageBox.Show("Não pode haver usuários com o mesmo login, email e/ou CPF");
                }
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
        }


        public async Task<bool> SemUnique()
        {
            return (await Synchro.tbUsuario.ReadAsync()).Where(p => p.Login == User.Login && p.Id != User?.Id).Count() == 0
                && (await Synchro.tbUsuario.ReadAsync()).Where(p => p.Email == User.Email && p.Id != User?.Id).Count() == 0
                && (await Synchro.tbUsuario.ReadAsync()).Where(p => p.CPF == User.CPF && p.Id != User?.Id).Count() == 0;
        }


        public async Task<bool> InsertUpdate()
        {
            Criptografar c = new Criptografar();
            if (mudarSenha)
            {
                if (await VerificarSenha())
                {
                    if (CampoSenhaNova.Password == CampoConfirmar.Password)
                    {
                        if (Criptografar.ValidarSenha(CampoSenhaNova.Password))
                        {
                            User.Senha = c.EncryptToString(CampoSenhaNova.Password);
                            await Control.UsuarioControl.Update(User);
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("A senha deve conter no mínimo 8 caracteres, tendo pelo menos uma letra minúscula," +
                                " uma letra maíuscula e um número");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Senhas diferentes");
                    }
                }
                else
                {
                    MessageBox.Show("Senha Incorreta");
                }
            }
            else
            {
                await Control.UsuarioControl.Update(User);
                return true;
            }
            return false;
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoLogin.Text != "" && CampoEnd.Text != ""
                && cpfValido;

        public async Task<bool> VerificarSenha()
        {
            Criptografar cg = new Criptografar();
            return (await Synchro.tbUsuario.ReadAsync()).Where(c => c.Id == User.Id).First().Senha
                == cg.EncryptToString(CampoSenhaAntiga.Password);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CampoLogin.Text = User.Login;
            CampoNome.Text = User.Nome;
            CampoEnd.Text = User.Endereco;
            CampoCPF.Text = User.CPF;
            CampoEmail.Text = User.Email;
        }
    }
}
