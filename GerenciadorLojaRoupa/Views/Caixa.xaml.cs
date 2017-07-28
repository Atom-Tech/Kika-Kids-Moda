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
    /// Interaction logic for Caixa.xaml
    /// </summary>
    public partial class Caixa : Page
    {
        Model.Caixa caixa;
        bool abrir;

        public Caixa(bool abrir = true)
        {
            InitializeComponent();
            if (Main.usuarioLogado.NivelAcesso == "Administrador")
            {
                CampoAbertura.IsEnabled = abrir;
                BotaoAbrir.IsEnabled = abrir;
                BotaoFechar.IsEnabled = !abrir;
                MudarValorAbertura.Visibility = Visibility.Visible;
            }
            else
            {
                CampoAbertura.IsEnabled = false;
                BotaoAbrir.IsEnabled = false;
                BotaoFechar.IsEnabled = false;
                MudarValorAbertura.Visibility = Visibility.Collapsed;
            }
            this.abrir = abrir;
        }


        private async void BotaoAbrir_Click(object sender, RoutedEventArgs e)
        {
            if (CampoAbertura.Value.HasValue)
            {
                caixa.ValorAbertura = CampoAbertura.Value.Value;
                await Control.CaixaControl.Update(caixa);
                Main.Caixa = caixa;
                Main.HM.Visibility = Visibility.Visible;
                Main.HM.Content[0].IsSelected = true;
                Main.MainFrame.Navigate(new Home());
            }
            else
            {
                MessageBox.Show("Não há valor inserido", "Aviso");
            }
        }

        private async void BotaoFechar_Click(object sender, RoutedEventArgs e)
        {
            if (CampoFechamento.Value.HasValue)
            {
                caixa.ValorFechamento = CampoFechamento.Value.Value;
                await Control.CaixaControl.Update(caixa);
                Environment.Exit(0);
            }
            else
            {
                MessageBox.Show("Não há valor inserido", "Aviso");
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var vendasHoje = (await Synchro.tbVenda.ReadAsync()).Where(c => c.Data == DateTime.Today.ToShortDateString());
            var itemsHoje = new List<Model.Item>();
            if ((await Synchro.tbCaixa.ReadAsync())
                .Where(c => c.DataCaixa == DateTime.Today.ToShortDateString()).Count() == 0)
            {
                var c = new Model.Caixa()
                {
                    DataCaixa = DateTime.Today.ToShortDateString(),
                    HoraAbertura = DateTime.Now.ToShortTimeString()
                };
                await Control.CaixaControl.Insert(c);
                caixa = c;
                CampoAbertura.Value = 0;
            }
            else
            {
                caixa = (await Synchro.tbCaixa.ReadAsync()).Where(c => c.DataCaixa == DateTime.Today.ToShortDateString()).First();
                CampoAbertura.Value = caixa.ValorAbertura;
                ValorSangria.Value = caixa.ValorSangria;
                var valor = vendasHoje.Sum(v => v.Valor);
                CampoFechamento.Value = valor + caixa.ValorAbertura - caixa.ValorSangria + caixa.ValorAcumulado;
                if (caixa.ValorAbertura != 0 && abrir)
                {
                    Main.HM.Visibility = Visibility.Visible;
                    Main.HM.Content[0].IsSelected = true;
                    Main.Caixa = caixa;
                    Main.MainFrame.Navigate(new Home());
                }
            }
            if (vendasHoje.Count() > 0)
            {
                string vendas = "";
                foreach (var v in vendasHoje)
                {
                    var items = (await Synchro.tbItem.ReadAsync()).Where(i => i.CodigoVenda == v.Id);
                    if (items.Count() > 0)
                    {
                        foreach (var item in items)
                        {
                            itemsHoje.Add(item);
                        }
                    }
                }
                foreach (var item in itemsHoje)
                {
                    vendas += $"\n{item.QuantidadeProduto} unidades de " +
                        $"{await Model.Produto.GetNome(item.CodigoProduto)} foi vendido por R${item.ValorProduto}";
                }
                Vendas.Text = vendas;
            }
            else
            {
                Vendas.Text = "Sem Vendas";
            }
        }

        private void MudarValorAbertura_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CampoAbertura.IsEnabled = true;
            BotaoAbrir.IsEnabled = true;
            CampoFechamento.IsEnabled = false;
            BotaoFechar.IsEnabled = false;
        }

        private void Sangria_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!abrir)
            {
                if (GroupSangria.Visibility == Visibility.Visible)
                    GroupSangria.Visibility = Visibility.Collapsed;
                else
                    GroupSangria.Visibility = Visibility.Visible;
            }
        }

        private async void RealizarSangria_Click(object sender, RoutedEventArgs e)
        {
            if (ValorSangria.Value.HasValue)
            {
                caixa = (await Synchro.tbCaixa.ReadAsync()).Where(c => c.DataCaixa == DateTime.Today.ToShortDateString()).First();
                if (ValorSangria.Value < caixa.ValorAbertura)
                {
                    caixa.ValorSangria = ValorSangria.Value.Value;
                    await Control.CaixaControl.Update(caixa);
                    var vendasHoje = (await Synchro.tbVenda.ReadAsync()).Where(c => c.Data == DateTime.Today.ToShortDateString());
                    var valor = vendasHoje.Sum(v => v.Valor);
                    CampoFechamento.Value = valor + caixa.ValorAbertura - caixa.ValorSangria;
                    GroupSangria.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Sangria realizada com sucesso!");
                }
                else
                {
                    MessageBox.Show("Sangria não pode ser maior que o valor de abertura");
                }
            }
        }
    }
}
