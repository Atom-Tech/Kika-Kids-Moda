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
    /// Interaction logic for Retirada.xaml
    /// </summary>
    public partial class Retirada : Page
    {
        int op = 0;
        Model.Retirada Ret = new Model.Retirada();

        public Retirada()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListaProdutos.ItemsSource = await Synchro.tbProduto.ReadAsync();
            ListaVendedores.ItemsSource = await Synchro.tbVendedor.ReadAsync();
            Lista.ItemsSource = await Synchro.tbRetirada.ReadAsync();
            AtivarCampos(false);
        }

        private async void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Retirada && Lista.SelectedItem != null)
            {
                Ret = (Model.Retirada)Lista.SelectedItem;
                await Ret.Load();
                CampoQuantidade.Maximum = Ret.Produto.Quantidade;
                CampoQuantidade.Value = Ret.Quantidade;
                CampoValor.Value = Ret.ValorEntrada;
                CampoData.Text = Ret.Data;
                Retornado.IsChecked = Ret.Retornado;
                foreach (Model.Produto p in ListaProdutos.Items)
                {
                    if (p.Codigo == Ret.Produto.Codigo)
                    {
                        ListaProdutos.SelectedItem = p;
                        break;
                    }
                }
                foreach (Model.Vendedor v in ListaVendedores.Items)
                {
                    if (v.CPF == Ret.Vendedor.CPF)
                    {
                        ListaVendedores.SelectedItem = v;
                        break;
                    }
                }
            }
            AtivarCampos(false);
        }

        private void ListaProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaProdutos.SelectedItem is Model.Produto && ListaProdutos.SelectedItem != null)
            {
                Ret.Produto = (Model.Produto)ListaProdutos.SelectedItem;
                Ret.CodigoProduto = Ret.Produto.Codigo;
                CampoQuantidade.Maximum = Ret.Produto.Quantidade;
                if (CampoQuantidade.Maximum < CampoQuantidade.Value) CampoQuantidade.Value = CampoQuantidade.Maximum;
            }
        }

        private void ListaVendedores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaVendedores.SelectedItem is Model.Vendedor && ListaVendedores.SelectedItem != null)
            {
                Ret.Vendedor = (Model.Vendedor)ListaVendedores.SelectedItem;
                Ret.CPFVendedor = Ret.Vendedor.CPF;
            }
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            Ret = new Model.Retirada()
            {
                Data = DateTime.Today.ToShortDateString()
            };
            CampoQuantidade.Value = 0;
            CampoValor.Value = 0;
            AtivarCampos(true);
            ListaProdutos.SelectedIndex = -1;
            ListaVendedores.SelectedIndex = -1;
        }

        public void AtivarCampos(bool vf)
        {
            CampoQuantidade.IsEnabled = vf;
            CampoValor.IsEnabled = vf;
            ListaProdutos.IsEnabled = vf;
            ListaVendedores.IsEnabled = vf;
            Retornado.IsEnabled = vf;
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (Ret.Id != null)
            {
                op = 2;
                AtivarCampos(true);
            }
        }

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await InsertUpdate())
                    Lista.ItemsSource = await Synchro.tbRetirada.ReadAsync();
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
        }


        public async Task<bool> InsertUpdate()
        {
            switch (op)
            {
                case 1: //Novo
                    await Control.RetiradaControl.Insert(Ret);
                    return true;
                case 2: //Alterar
                    await Control.RetiradaControl.Update(Ret);
                    return true;
            }
            return false;
        }

        public bool VerificarCamposVazios() => CampoQuantidade.Value != 0 && CampoValor.Value != 0
            && Ret.CodigoProduto != null && Ret.CPFVendedor != null;

        private void CampoQuantidade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (CampoQuantidade.Maximum < CampoQuantidade.Value) CampoQuantidade.Value = CampoQuantidade.Maximum;
            Ret.Quantidade = CampoQuantidade.Value.Value;
        }

        private void CampoValor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Ret.ValorEntrada = CampoValor.Value.Value;
        }
        

        private void Retornado_Checked(object sender, RoutedEventArgs e)
        {
            Ret.Retornado = true;
        }

        private void Retornado_Unchecked(object sender, RoutedEventArgs e)
        {
            Ret.Retornado = false;
        }
    }
}
