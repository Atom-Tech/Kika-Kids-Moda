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
    /// Interaction logic for Venda.xaml
    /// </summary>
    public partial class Venda : Page
    {
        int op = 0;
        Model.Venda Vend = new Model.Venda();

        public Venda()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListaProdutos.ItemsSource = (await Synchro.tbRetirada.ReadAsync())
                .Where(r => !r.Retornado);
            ListaClientes.ItemsSource = await Synchro.tbCliente.ReadAsync();
            Lista.ItemsSource = await Synchro.tbVenda.ReadAsync();
            AtivarCampos(false);
        }

        private async void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Venda && Lista.SelectedItem != null)
            {
                Vend = (Model.Venda)Lista.SelectedItem;
                await Vend.Load();
                CampoMetodo.SelectedIndex = Vend.FormaPagamento == "Crédito" ? 0 : 1;
                CampoQuantidade.Maximum = Vend.Produto.Quantidade;
                CampoQuantidade.Value = Vend.QuantidadeProduto;
                CampoData.Text = Vend.Data;
                for (int i = 0; i < ListaProdutos.Items.Count; i++)
                {
                    var r = (Model.Retirada)ListaProdutos.Items[i];
                    var p = await r.GetProduto();
                    if (p.Codigo == Vend.Produto.Codigo)
                    {
                        ListaProdutos.SelectedIndex = i;
                        break;
                    }
                }
                foreach (Model.Cliente c in ListaClientes.Items)
                {
                    if (c.CPF == Vend.Cliente.CPF)
                    {
                        ListaClientes.SelectedItem = c;
                        break;
                    }
                }
            }
            AtivarCampos(false);
        }

        private async void ListaProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaProdutos.SelectedItem is Model.Retirada && ListaProdutos.SelectedItem != null)
            {
                Vend.Produto = await ((Model.Retirada)ListaProdutos.SelectedItem).GetProduto();
                Vend.CodigoProduto = Vend.Produto.Codigo;
                CampoQuantidade.Maximum = ((Model.Retirada)ListaProdutos.SelectedItem).Quantidade;
                if (CampoQuantidade.Maximum < CampoQuantidade.Value) CampoQuantidade.Value = CampoQuantidade.Maximum;
                CalcularValorTotal();
            }
        }

        private void ListaClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaClientes.SelectedItem is Model.Cliente && ListaClientes.SelectedItem != null)
            {
                Vend.Cliente = (Model.Cliente)ListaClientes.SelectedItem;
                Vend.CPFCliente = Vend.Cliente.CPF;
            }
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            Vend = new Model.Venda()
            {
                Data = DateTime.Today.ToShortDateString()
            };
            CampoQuantidade.Value = 0;
            CampoMetodo.SelectedIndex = 0;
            AtivarCampos(true);
            ListaProdutos.SelectedIndex = -1;
            ListaClientes.SelectedIndex = -1;
        }

        public void AtivarCampos(bool vf)
        {
            CampoQuantidade.IsEnabled = vf;
            ListaProdutos.IsEnabled = vf;
            ListaClientes.IsEnabled = vf;
            CampoMetodo.IsEnabled = vf;
        }
        
        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await InsertUpdate())
                    Lista.ItemsSource = await Synchro.tbVenda.ReadAsync();
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
                    await Control.VendaControl.Insert(Vend);
                    await ReduzirQuantidade();
                    return true;
            }
            return false;
        }

        public async Task ReduzirQuantidade()
        {
            var produto = Vend.Produto;
            produto.Quantidade -= Vend.QuantidadeProduto;
            await Synchro.tbProduto.UpdateAsync(produto);
            var retirado = (Model.Retirada)ListaProdutos.SelectedItem;
            retirado.Quantidade -= Vend.QuantidadeProduto;
            await Synchro.tbRetirada.UpdateAsync(retirado);
        }

        public bool VerificarCamposVazios() => CampoQuantidade.Value != 0
            && Vend.CodigoProduto != null && Vend.CPFCliente != null;

        private void CampoQuantidade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (CampoQuantidade.Maximum < CampoQuantidade.Value) CampoQuantidade.Value = CampoQuantidade.Maximum;
            Vend.QuantidadeProduto = CampoQuantidade.Value.Value;
            CalcularValorTotal();
        }

        public void CalcularValorTotal()
        {
            if (Vend.Produto != null)
            {
                Vend.Valor = Vend.QuantidadeProduto * Vend.Produto.Valor;
                CampoValorTotal.Value = Vend.Valor;
            }
        }

        private void CampoMetodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vend.FormaPagamento = CampoMetodo.SelectedIndex == 0 ? "Crédito" : "Débito";
        }
    }
}
