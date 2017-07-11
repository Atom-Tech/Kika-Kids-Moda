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
    /// Interaction logic for Vendedor.xaml
    /// </summary>
    public partial class Vendedor : Page
    {
        int op = 0;
        bool cpfValido = false;
        Model.Vendedor Vend = new Model.Vendedor();
        string cpfAntigo = "";

        public Vendedor()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Lista.ItemsSource = await Synchro.tbVendedor.ReadAsync();
            AtivarCampos(false);
        }

        private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Vendedor && Lista.SelectedItem != null)
            {
                Vend = (Model.Vendedor)Lista.SelectedItem;
                CampoNome.Text = Vend.Nome;
                CampoEnd.Text = Vend.Endereco;
                CampoCPF.Text = Vend.CPF;
                CampoEmail.Text = Vend.Email;
                CampoRG.Text = Vend.RG;
                CampoTel.Text = Vend.Telefone;
                cpfAntigo = Vend.CPF;
            }
            AtivarCampos(false);
        }

        private async void CampoBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBusca.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbVendedor.ReadAsync();
            }
            else
            {
                Lista.ItemsSource = (await Synchro.tbVendedor.ReadAsync()).Where(c => c.Nome.Contains(CampoBusca.Text));
            }
        }

        private void CampoCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            cpfValido = Validar.CPF(CampoCPF.Text);
            CampoCPF.BorderBrush = cpfValido ? Brushes.Green : Brushes.Red;
            if (cpfValido) Vend.CPF = CampoCPF.Text;
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            Vend = new Model.Vendedor();
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
            BotaoSalvar.IsEnabled = vf;
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (Vend.Id != null)
            {
                op = 2;
                AtivarCampos(true);
            }
        }

        public async Task<bool> HasForeign()
        {
            return (await Synchro.tbRetirada.ReadAsync()).Where(v => v.CPFVendedor == Vend.CPF).Count() > 0;
        }


        private async void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (!await HasForeign())
            {
                if (Vend.Id != null)
                {
                    var message = MessageBox.Show("Tem certeza que deseja deletar? Não será possivel recuperar depois", "Aviso", MessageBoxButton.YesNo);
                    if (message == MessageBoxResult.Yes)
                    {
                        await Control.VendedorControl.Delete(Vend);
                        Lista.ItemsSource = await Synchro.tbVendedor.ReadAsync();
                        MessageBox.Show("Vendedor deletado com sucesso!");
                    }
                }
            }
            else
                MessageBox.Show("Esse vendedor já retirou produtos");
        }

        private void CampoRG_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vend.RG = CampoRG.Text;
        }

        private void CampoNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vend.Nome = CampoNome.Text;
        }

        private void CampoEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vend.Endereco = CampoEnd.Text;
        }

        private void CampoTel_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vend.Telefone = CampoTel.Text;
        }

        private void CampoEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            Vend.Email = CampoEmail.Text;
        }

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await SemUnique())
                {
                    if (await InsertUpdate())
                    {
                        Lista.ItemsSource = await Synchro.tbVendedor.ReadAsync();
                        AtivarCampos(false);
                        op = 0;
                    }
                }
                else
                {
                    MessageBox.Show("Não pode haver vendedores com o mesmo email e/ou CPF");
                }
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
        }


        public async Task<bool> SemUnique()
        {
            return (await Synchro.tbVendedor.ReadAsync()).Where(v => v.Email == Vend.Email && v.Id != Vend?.Id).Count() == 0
                && (await Synchro.tbVendedor.ReadAsync()).Where(v => v.CPF == Vend.CPF && v.Id != Vend?.Id).Count() == 0;
        }


        public async Task<bool> InsertUpdate()
        {
            switch (op)
            {
                case 1: //Novo
                    await Control.VendedorControl.Insert(Vend);
                    MessageBox.Show("Vendedor inserido com sucesso!");
                    return true;
                case 2: //Alterar
                    await Control.VendedorControl.Update(Vend);
                    await UpdateCPF();
                    MessageBox.Show("Vendedor alterado com sucesso!");                
                    return true;
            }
            return false;
        }

        public async Task UpdateCPF()
        {
            var retiradas = await Synchro.tbRetirada.ReadAsync();
            foreach (var retirada in retiradas)
            {
                if (retirada.CPFVendedor == cpfAntigo)
                {
                    retirada.CPFVendedor = Vend.CPF;
                    await Control.RetiradaControl.Update(retirada);
                }
            }
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoTel.IsMaskCompleted && CampoEnd.Text != ""
                && cpfValido && CampoRG.IsMaskCompleted;
    }
}
