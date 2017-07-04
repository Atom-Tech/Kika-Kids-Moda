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
    /// Interaction logic for Cliente.xaml
    /// </summary>
    public partial class Cliente : Page
    {
        int op = 0;
        bool cpfValido = false;
        Model.Cliente Cli = new Model.Cliente();

        public Cliente()
        {
            InitializeComponent();
            DataContext = this;
        }

        public async Task UpdateList()
        {
            Lista.ItemsSource = null;
            Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateList();
            AtivarCampos(false);
        }

        private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Cliente && Lista.SelectedItem != null)
            {
                Cli = (Model.Cliente)Lista.SelectedItem;
                CampoNome.Text = Cli.Nome;
                CampoEnd.Text = Cli.Endereco;
                CampoCPF.Text = Cli.CPF;
                CampoEmail.Text = Cli.Email;
                CampoRG.Text = Cli.RG;
                CampoTel.Text = Cli.Telefone;
            }
            AtivarCampos(false);
        }

        private async void CampoBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBusca.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
            }
            else
            {
                Lista.ItemsSource = (await Synchro.tbCliente.ReadAsync()).Where(c => c.Nome.Contains(CampoBusca.Text));
            }
        }

        private void CampoCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            cpfValido = Validar.CPF(CampoCPF.Text);
            CampoCPF.BorderBrush = cpfValido ? Brushes.Green : Brushes.Red;
            if (cpfValido) Cli.CPF = CampoCPF.Text;
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            Cli = new Model.Cliente();
            CampoNome.Text = "";
            CampoEnd.Text = "";
            CampoCPF.Text = "";
            CampoEmail.Text = "";
            CampoRG.Text = "";
            CampoTel.Text = "";
            AtivarCampos(true);
        }

        public void AtivarCampos(bool vf)
        {
            CampoRG.IsEnabled = vf;
            CampoNome.IsEnabled = vf;
            CampoEnd.IsEnabled = vf;
            CampoCPF.IsEnabled = vf;
            CampoTel.IsEnabled = vf;
            CampoEmail.IsEnabled = vf;
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (Cli.Id != null)
            {
                op = 2;
                AtivarCampos(true);
            }
        }

        private async void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (Cli.Id != null)
            {
                var message = MessageBox.Show("Tem certeza que deseja deletar? Não será possivel recuperar depois", "Aviso", MessageBoxButton.YesNo);
                if (message == MessageBoxResult.Yes)
                {
                    await Control.ClienteControl.Delete(Cli);
                    Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
                }
            }
        }

        private void CampoRG_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.RG = CampoRG.Text;
        }

        private void CampoNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Nome = CampoNome.Text;
        }

        private void CampoEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Endereco = CampoEnd.Text;
        }

        private void CampoTel_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Telefone = CampoTel.Text;
        }

        private void CampoEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Email = CampoEmail.Text;
        }

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await SemUnique())
                {
                    if (await InsertUpdate())
                        Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
                }
                else
                {
                    MessageBox.Show("Não pode haver Clientes com o mesmo email e/ou CPF");
                }
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
        }


        public async Task<bool> SemUnique()
        {
            return (await Synchro.tbCliente.ReadAsync()).Where(v => v.Email == Cli.Email && v.Id != Cli?.Id).Count() == 0
                && (await Synchro.tbCliente.ReadAsync()).Where(v => v.CPF == Cli.CPF && v.Id != Cli?.Id).Count() == 0;
        }


        public async Task<bool> InsertUpdate()
        {
            switch (op)
            {
                case 1: //Novo
                    await Control.ClienteControl.Insert(Cli);
                    return true;
                case 2: //Alterar
                    await Control.ClienteControl.Update(Cli);
                    return true;
            }
            return false;
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoTel.IsMaskCompleted && CampoEnd.Text != ""
                && cpfValido && CampoRG.IsMaskCompleted;
        
    }
}
