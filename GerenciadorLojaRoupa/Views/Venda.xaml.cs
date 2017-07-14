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
        public static string ClienteCadastrado = null;
        int op = 0;
        Model.Venda Vend = new Model.Venda();

        public Venda()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CampoPrestacao.Minimum = DateTime.Today;
            ListaProdutos.ItemsSource = await Synchro.tbProduto.ReadAsync();
            ListaClientes.ItemsSource = await Synchro.tbCliente.ReadAsync();
            Lista.ItemsSource = await Synchro.tbVenda.ReadAsync();
            AtivarCampos(false);
        }

        private async void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Venda && Lista.SelectedItem != null)
            {
                ListaProdutos.SelectedIndex = -1;
                ListaClientes.SelectedIndex = -1;
                Vend = (Model.Venda)Lista.SelectedItem;
                await Vend.Load();
                CampoMetodo.SelectedIndex = Vend.FormaPagamento == "Crédito" ? 0 : 1;
                CampoQuantidade.Maximum = Vend.Produto?.Quantidade;
                CampoQuantidade.Value = Vend.QuantidadeProduto;
                CampoData.Text = Vend.Data;
                CampoValor.Value = Vend.ValorEntrada;
                CampoPrestacao.Value = DateTime.Parse(Vend.DataPrestacao);
                if (Vend.Produto != null)
                    foreach (Model.Produto p in ListaProdutos.Items)
                    {
                        if (p.Codigo == Vend.Produto.Codigo)
                        {
                            ListaProdutos.SelectedItem = p;
                            break;
                        }
                    }
                if (Vend.Cliente != null)
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

        private void ListaProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaProdutos.SelectedItem is Model.Produto && ListaProdutos.SelectedItem != null)
            {
                Vend.Produto = (Model.Produto)ListaProdutos.SelectedItem;
                Vend.CodigoProduto = Vend.Produto.Codigo;
                CampoQuantidade.Maximum = ((Model.Produto)ListaProdutos.SelectedItem).Quantidade;
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
                Data = DateTime.Today.ToShortDateString(),
                FormaPagamento = "Crédito"
            };
            CampoQuantidade.Value = 0;
            CampoMetodo.SelectedIndex = 0;
            CampoValor.Value = 0;
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
            BotaoSalvar.IsEnabled = vf;
            CampoValor.IsEnabled = vf;
        }

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (Vend.CPFCliente == null)
                {
                    var mensagem = MessageBox.Show("Não há um cliente selecionado, deseja cadastrar um?", "Aviso", MessageBoxButton.YesNo);
                    if (mensagem == MessageBoxResult.Yes)
                    {
                        var janela = new JanelaCliente();
                        janela.ShowDialog();
                        if (ClienteCadastrado != null)
                        {
                            Vend.CPFCliente = ClienteCadastrado;
                            if (await InsertUpdate())
                            {
                                Lista.ItemsSource = await Synchro.tbVenda.ReadAsync();
                                ListaClientes.ItemsSource = await Synchro.tbCliente.ReadAsync();
                                op = 0;
                                await ReduzirQuantidade();
                                AtivarCampos(false);
                            }
                        }
                    }
                }
                else
                {
                    if (await InsertUpdate())
                    {
                        Lista.ItemsSource = await Synchro.tbVenda.ReadAsync();
                        op = 0;
                        await ReduzirQuantidade();
                        AtivarCampos(false);
                    }
                }
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
            ClienteCadastrado = null;
        }


        public async Task<bool> InsertUpdate()
        {
            switch (op)
            {
                case 1: //Novo
                    await Control.VendaControl.Insert(Vend);
                    MessageBox.Show("Produto vendido com sucesso!");
                    return true;
            }
            return false;
        }

        public async Task ReduzirQuantidade()
        {
            var produto = Vend.Produto;
            produto.Quantidade -= Vend.QuantidadeProduto;
            await Synchro.tbProduto.UpdateAsync(produto);
        }

        public bool VerificarCamposVazios() => CampoQuantidade.Value != 0 && CampoValor.Value != 0
            && Vend.CodigoProduto != null;


        private void CampoValor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vend.ValorEntrada = CampoValor.Value.Value;
            CalcularValorTotal();
        }

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

        private void CampoPrestacao_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vend.DataPrestacao = CampoPrestacao.Value.Value.ToShortDateString();
        }
    }
}
