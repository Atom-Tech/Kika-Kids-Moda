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
            var caixa = (await Synchro.tbCaixa.ReadAsync()).Where(c => c.DataCaixa == DateTime.Today.ToShortDateString()).First();
            Welcome.Text = $"Bem Vindo, {Main.usuarioLogado.Login}!";
            Caixa.Text = $"O dinheiro no caixa atualmente é R${caixa.ValorAbertura-caixa.ValorSangria}";
            ComboProduto.ItemsSource = await Synchro.tbProduto.ReadAsync();
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
    }
}
