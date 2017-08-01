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
using System.Windows.Threading;

namespace KikaKidsModa.Views
{
    /// <summary>
    /// Interaction logic for Venda.xaml
    /// </summary>
    public partial class Venda : Page
    {
        public static string ClienteCadastrado = null;
        Model.Venda Vend;
        List<Model.Produto> Produtos = new List<Model.Produto>();
        List<Model.Item> Items = new List<Model.Item>();

        public Venda()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += Page_Loaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListaProdutos.ItemsSource = await Synchro.tbProduto.ReadAsync();
            ListaClientes.ItemsSource = await Synchro.tbCliente.ReadAsync();
            Vend = new Model.Venda()
            {
                Data = DateTime.Today.ToShortDateString(),
                FormaPagamento = "Crédito"
            };
            CampoParcelas.Value = 1;
            CampoMetodo.SelectedIndex = 0;
            CampoValor.Value = 0;
            CampoPorcentagem.Value = 0;
            CampoPrestacao.Value = DateTime.Today;
            CampoPrestacao.Minimum = DateTime.Today;
            ListaProdutos.SelectedIndex = -1;
            ListaClientes.SelectedIndex = -1;
            Main.CodigoEscaneado += Main_CodigoEscaneado;
        }

        private void Main_CodigoEscaneado(object sender, ScannerEventArgs e)
        {
            for (int i = 0; i < ListaProdutos.Items.Count; i++)
            {
                var p = (Model.Produto)ListaProdutos.Items[i];
                if (p.Codigo == e.Codigo)
                {
                    if (ListaProdutos.SelectedItems.Contains(p))
                        ListaProdutos.SelectedItems.Remove(p);
                    else
                        ListaProdutos.SelectedItems.Add(p);
                }
            }
        }

        private void ListaProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaProdutos.SelectedItems.Count == 0)
            {
                Lista.ItemsSource = null;
            }
            if (ListaProdutos.SelectedItem is Model.Produto && ListaProdutos.SelectedItem != null)
            {
                Lista.ItemsSource = null;
                Produtos = new List<Model.Produto>();
                Items = new List<Model.Item>();
                foreach (var i in ListaProdutos.SelectedItems)
                {
                    Produtos.Add((Model.Produto)i);
                    Items.Add(new Model.Item()
                    {
                        QuantidadeProduto = 0,
                        ValorProduto = 0,
                        CodigoProduto = ((Model.Produto)i).Codigo
                    });
                }
                Lista.ItemsSource = Produtos;
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

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (Vend.CPFCliente == null)
                {
                    var mensagem = MessageBox.Show("Não há um cliente selecionado, deseja cadastrar um?", "Aviso", MessageBoxButton.YesNoCancel);
                    if (mensagem == MessageBoxResult.Yes)
                    {
                        var janela = new JanelaCliente();
                        janela.ShowDialog();
                        if (ClienteCadastrado != null)
                        {
                            Vend.CPFCliente = ClienteCadastrado;
                            if (await InsertUpdate())
                            {
                                ListaClientes.ItemsSource = await Synchro.tbCliente.ReadAsync();
                                await ReduzirQuantidade();
                                Retornar();
                            }
                        }
                    }
                    if (mensagem == MessageBoxResult.No)
                    {
                        if (await InsertUpdate())
                        {
                            await ReduzirQuantidade();
                            Retornar();
                        }
                    }
                }
                else
                {
                    if (await InsertUpdate())
                    {
                        await ReduzirQuantidade();
                        Retornar();
                    }
                }
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
            ClienteCadastrado = null;
        }

        public void Retornar()
        {
            var message = MessageBox.Show("Deseja cadastrar mais uma venda?", "Aviso", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes)
            {
                Main.HM.Content[0].IsSelected = true;
                Main.HM.Content[8].IsSelected = true;
            }
            else
                Main.HM.Content[0].IsSelected = true;
        }


        public async Task<bool> InsertUpdate()
        {
            await Control.VendaControl.Insert(Vend);
            MessageBox.Show("Produto vendido com sucesso!");
            var venda = (await Synchro.tbVenda.ReadAsync()).LastOrDefault();
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].CodigoVenda = venda.Id;
                await Control.ItemControl.Insert(Items[i]);
            }
            return true;
        }

        public async Task ReduzirQuantidade()
        {
            foreach (var item in Items)
            {
                var produto = (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == item.CodigoProduto).FirstOrDefault();
                produto.Quantidade -= item.QuantidadeProduto;
                await Synchro.tbProduto.UpdateAsync(produto);
            }
        }

        public bool VerificarCamposVazios()
        {
            bool ver = Items.Count > 0;
            foreach (var i in Items)
            {
                if (i.QuantidadeProduto == 0 || i.ValorProduto == 0) ver = false;
            }
            return ver;
        }


        private void CampoValor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vend.ValorEntrada = CampoValor.Value.Value;
            CalcularParcela();
            CalcularValorTotal();
        }

        public void CalcularValorTotal()
        {
            Vend.Valor = Items.Sum(i => i.ValorProduto);
            CampoValorTotal.Value = Vend.Valor;
            CalcularParcela();
            CalcularPorcentagem();
        }

        private void CampoMetodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vend.FormaPagamento = CampoMetodo.SelectedIndex.FromIndex();
            if (CampoMetodo.SelectedIndex == 0)
            {
                CampoPorcentagem.Value = 0;
                CampoPorcentagem.IsEnabled = false;
            }
            else
            {
                CampoPorcentagem.IsEnabled = true;
            }
        }

        private void CampoPrestacao_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vend.DataPrestacao = CampoPrestacao.Value.Value.ToShortDateString();
        }

        private void CampoParcelas_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vend.Parcelas = CampoParcelas.Value.Value;
            CalcularParcela();
        }

        public void CalcularParcela()
        {
            CampoParcelado.Value = (Vend.Valor - Vend.ValorEntrada) / Vend.Parcelas;
        }

        private void CampoPorcentagem_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Vend.PorcentagemDesconto = CampoPorcentagem.Value.Value;
            CalcularPorcentagem();
        }

        public void CalcularPorcentagem()
        {
            CampoValorDesconto.Value = Vend.Valor - Vend.Valor * Vend.PorcentagemDesconto / 100;
            Vend.ValorTotalDesconto = CampoValorDesconto.Value.Value;
        }

        private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.Items.Count > 0 && Lista.SelectedIndex != -1)
            {
                var item = Items[Lista.SelectedIndex];
                var produto = (Model.Produto)Lista.SelectedItem;
                CampoQuantidadeProduto.Maximum = produto.Quantidade;
                CampoQuantidadeProduto.Value = item.QuantidadeProduto;
            }
        }

        private void CampoQuantidadeProduto_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Lista.Items.Count > 0 && Lista.SelectedIndex != -1 && CampoQuantidadeProduto.Value.HasValue)
            {
                Items[Lista.SelectedIndex].QuantidadeProduto = CampoQuantidadeProduto.Value.Value;
                CampoValorProduto.Value = Items[Lista.SelectedIndex].QuantidadeProduto
                    * ((Model.Produto)Lista.Items[Lista.SelectedIndex]).Valor;
                Items[Lista.SelectedIndex].ValorProduto = CampoValorProduto.Value.Value;
                CalcularValorTotal();
            }
        }
    }
}
