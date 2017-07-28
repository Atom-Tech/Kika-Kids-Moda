using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {

        public Home()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var vendasHoje = (await Synchro.tbVenda.ReadAsync()).Where(c => c.Data == DateTime.Today.ToShortDateString());
            Welcome.Text = $"Bem Vindo, {Main.usuarioLogado.Login}!";
            if (Main.Caixa != null) Caixa.Text = $"O dinheiro no caixa atualmente é R$" +
                $"{Main.Caixa.ValorAbertura + Main.Caixa.ValorAcumulado - Main.Caixa.ValorSangria + vendasHoje.Sum(v => v.Valor)}";
            ComboProduto.ItemsSource = await Synchro.tbProduto.ReadAsync();
            await CarregarNotificacoes();
        }

        public async Task CarregarNotificacoes()
        {
            ListaPrest.ItemsSource = (await Synchro.tbVenda.ReadAsync())
                .Where(c => (c.DataPrestacao.ToDay() >= DateTime.Today.AddDays(-3)));
        }

        private async void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            if (QuantidadeEstoque.Value > 0)
            {
                if (ComboProduto.SelectedIndex != -1)
                {
                    var produto = (Model.Produto)ComboProduto.SelectedItem;
                    produto.Quantidade += QuantidadeEstoque.Value.Value;
                    await Control.ProdutoControl.Update(produto);
                    MessageBox.Show("Estoque adicionado com sucesso!");
                }
                else
                {
                    MessageBox.Show("Não há produto selecionado");
                }
            }
            else
            {
                MessageBox.Show("A quantidade deve ser maior que 0");
            }
        }

        private async void BotaoPagar_Click(object sender, RoutedEventArgs e)
        {
            if (ListaPrest.SelectedIndex != -1)
            {
                var message = MessageBox.Show("Deseja marcar essa venda como paga?", "Aviso", MessageBoxButton.YesNo);
                if (message == MessageBoxResult.Yes)
                {
                    var venda = (Model.Venda)ListaPrest.SelectedItem;
                    venda.Pago = true;
                    await Control.VendaControl.Update(venda);
                    Main.Caixa.ValorAcumulado += (venda.Valor - venda.ValorEntrada) / venda.Parcelas;
                    await Control.CaixaControl.Update(Main.Caixa);
                    ListaPrest.ItemsSource = null;
                    await CarregarNotificacoes();
                }
            }
        }

        private void ListaPrest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaPrest.SelectedIndex != -1)
            {
                var venda = (Model.Venda)ListaPrest.SelectedItem;
                BotaoPagar.IsEnabled = !venda.Pago;
            }
        }

        private void GerarNotaPromissoria_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Nota.Caminho))
            {
                string caminho = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Nota Promissória.pdf");
                File.Copy(Nota.Caminho, caminho, true);
                Process.Start(caminho);
            }
            else MessageBox.Show("Erro na hora de gerar PDF", "Aviso");
        }
    }

    public class BackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().ToLower() == "true") return "true";
            if (value.ToString().ToLower() == "false") return "false";
            if (value.ToString().ToDay() < DateTime.Today) return "Antes";
            if (value.ToString().ToDay() == DateTime.Today) return "Agora";
            return "Depois";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
