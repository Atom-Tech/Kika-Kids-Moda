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
    /// Interaction logic for Usuario.xaml
    /// </summary>
    public partial class Usuario : Page
    {
        int op = 0;
        bool mudarSenha = false;
        bool cpfValido = false;
        Model.Usuario User = new Model.Usuario();

        public Usuario()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Lista.ItemsSource = await Synchro.tbUsuario.ReadAsync();
            AtivarCampos(false);
        }

        private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Usuario && Lista.SelectedItem != null)
            {
                User = (Model.Usuario)Lista.SelectedItem;
                CampoLogin.Text = User.Login;
                CampoNome.Text = User.Nome;
                CampoEnd.Text = User.Endereco;
                CampoCPF.Text = User.CPF;
                CampoNA.SelectedIndex = User.NivelAcesso == "Administrador" ? 0 : 1;
                CampoEmail.Text = User.Email;
            }
            AtivarCampos(false);
        }

        private async void CampoBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBusca.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbUsuario.ReadAsync();
            }
            else
            {
                Lista.ItemsSource = (await Synchro.tbUsuario.ReadAsync()).Where(c => c.Login.Contains(CampoBusca.Text));
            }
        }

        private void CampoCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            cpfValido = Validar.CPF(CampoCPF.Text);
            CampoCPF.BorderBrush = cpfValido ? Brushes.Green : Brushes.Red;
            if (cpfValido) User.CPF = CampoCPF.Text;
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            User = new Model.Usuario();
            CampoLogin.Text = "";
            CampoNome.Text = "";
            CampoEnd.Text = "";
            CampoCPF.Text = "";
            CampoNA.SelectedIndex = 1;
            CampoEmail.Text = "";
            AtivarCampos(true);
        }

        public void AtivarCampos(bool vf)
        {
            CampoLogin.IsEnabled = vf;
            CampoNome.IsEnabled = vf;
            CampoEnd.IsEnabled = vf;
            CampoCPF.IsEnabled = vf;
            CampoNA.IsEnabled = vf;
            CampoEmail.IsEnabled = vf;
            CheckSenha.IsChecked = false;
            AtivarGrupo(false);
            if (!vf)
            {
                GroupSenha.Visibility = Visibility.Collapsed;
            }
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (User.Id != null)
            {
                op = 2;
                AtivarCampos(true);
                GroupSenha.Visibility = Visibility.Visible;
            }
        }

        private async void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (User.Id != null && Main.usuarioLogado.Id != User.Id)
            {
                var message = MessageBox.Show("Tem certeza que deseja deletar? Não será possivel recuperar depois", "Aviso", MessageBoxButton.YesNo);
                if (message == MessageBoxResult.Yes)
                {
                    await Control.UsuarioControl.Delete(User);
                    Lista.ItemsSource = await Synchro.tbUsuario.ReadAsync();
                }
            }
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

        private void CampoNA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CampoNA.SelectedIndex == 0) User.NivelAcesso = "Administrador";
            if (CampoNA.SelectedIndex == 1) User.NivelAcesso = "Funcionário";
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
                        Lista.ItemsSource = await Synchro.tbUsuario.ReadAsync();
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
            return (await Synchro.tbUsuario.ReadAsync()).Where(p => p.Login == User.Login).Count() == 0
                && (await Synchro.tbUsuario.ReadAsync()).Where(p => p.Email == User.Email).Count() == 0
                && (await Synchro.tbUsuario.ReadAsync()).Where(p => p.CPF == User.CPF).Count() == 0;
        }


        public async Task<bool> InsertUpdate()
        {
            Criptografar c = new Criptografar();
            switch (op)
            {
                case 1: //Novo
                    User.Senha = c.EncryptToString(User.Login + User.CPF.Substring(User.CPF.Length - 2));
                    await Control.UsuarioControl.Insert(User);
                    MessageBox.Show("A senha desse usuário é " + c.DecryptString(User.Senha));
                    return true;
                case 2: //Alterar
                    if ((await Synchro.tbUsuario.ReadAsync()).Where(u => u.NivelAcesso == "Administrador").Count() == 1
                        && CampoNA.Text == "Funcionário" &&
                        (await Synchro.tbUsuario.ReadAsync()).Where(u => u.Id == User.Id).First().NivelAcesso == "Administrador")
                    {
                        MessageBox.Show("Deve haver pelo menos um administrador cadastrado");
                        return false;
                    }
                    else
                    {
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
                    }
                    return false;
            }
            return false;
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoLogin.Text != "" && CampoEnd.Text != ""
                && cpfValido && CampoNA.Text != "";

        public async Task<bool> VerificarSenha()
        {
            Criptografar cg = new Criptografar();
            return (await Synchro.tbUsuario.ReadAsync()).Where(c => c.Id == User.Id).First().Senha
                == cg.EncryptToString(CampoSenhaAntiga.Password);
        }
    }
}
