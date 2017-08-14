using KikaKidsModa.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for Relatorios.xaml
    /// </summary>
    public partial class Relatorios : Page
    {
        Func<DateTime, int> groupbySemana = d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Sunday);

        public Relatorios()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCaixa();
        }

        public DateTime Data(string data) => DateTime.Parse(data);

        private async void ComboRel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PCV != null) Dispatcher.Invoke(() => PCV.Visibility = Visibility.Visible);
            switch (ComboRel.SelectedIndex)
            {
                case 0:
                    await LoadCaixa();
                    if (PCV != null) Dispatcher.Invoke(() => PCV.Visibility = Visibility.Collapsed);
                    break;
                case 1:
                    await LoadRetirada();
                    break;
                case 2:
                    await LoadVenda();
                    break;
            }
        }

        public async Task LoadCaixa()
        {
            var listas = (await Synchro.tbCaixa.ReadAsync())
                .GroupBy(c => groupbySemana(Data(c.DataCaixa))).ToList();
            var lista = new List<RelatorioCaixa>();
            foreach (var l in listas)
            {
                lista.Add(new RelatorioCaixa(l));
            }
            ListaRelatorios.ItemsSource = lista;
        }

        public async Task LoadRetirada()
        {
            var listas = (await Synchro.tbRetirada.ReadAsync())
                .GroupBy(c => groupbySemana(Data(c.Data))).ToList();
            var lista = new List<RelatorioRetirada>();
            foreach (var l in listas)
            {
                lista.Add(new RelatorioRetirada(l));
            }
            ListaRelatorios.ItemsSource = lista;
        }

        public async Task LoadVenda()
        {
            var listas = (await Synchro.tbVenda.ReadAsync())
                .GroupBy(c => groupbySemana(Data(c.Data))).ToList();
            var lista = new List<RelatorioVenda>();
            foreach (var l in listas)
            {
                lista.Add(new RelatorioVenda(l));
            }
            ListaRelatorios.ItemsSource = lista;
        }

        private void ListaRelatorios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListaRelatorios.SelectedIndex != -1)
            {
                var row = (Relatorio)ListaRelatorios.SelectedItem;
                if (row is RelatorioCaixa)
                    ViewRelatorio.ItemsSource = (row as RelatorioCaixa).Caixas;
                if (row is RelatorioRetirada)
                    ViewRelatorio.ItemsSource = (row as RelatorioRetirada).Produtos;
                if (row is RelatorioVenda)
                    ViewRelatorio.ItemsSource = (row as RelatorioVenda).Produtos;
            }
        }

        private void ListaRelatorios_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            if (propertyDescriptor.DisplayName == "Caixas" || propertyDescriptor.DisplayName == "Produtos")
            {
                e.Cancel = true;
            }
        }

        private void ViewRelatorio_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor coluna = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = coluna.DisplayName;
            List<string> blackList = new List<string>()
            {
                "Version", "Id", "Produto", "Cliente", "Vendedor"
            };
            if (blackList.Contains(coluna.DisplayName))
            {
                e.Cancel = true;
            }
            switch (ComboRel.SelectedIndex)
            {
                case 0:
                    if (Model.Caixa.Colunas.ContainsKey(coluna.DisplayName) && coluna.DisplayName != "ValorAcumulado")
                        e.Column.Header = Model.Caixa.Colunas[coluna.DisplayName];
                    break;
                case 1:
                    if (Model.Retirada.Colunas.ContainsKey(coluna.DisplayName))
                        e.Column.Header = Model.Retirada.Colunas[coluna.DisplayName];
                    break;
                case 2:
                    if (Model.Venda.Colunas.ContainsKey(coluna.DisplayName))
                        e.Column.Header = Model.Venda.Colunas[coluna.DisplayName];
                    break;
            }
        }

        private async void ViewRelatorio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewRelatorio.SelectedIndex != -1)
            {
                var row = ViewRelatorio.SelectedItem;
                if (row is Model.Caixa) return;
                if (row is Model.Venda)
                {
                    var venda = row as Model.Venda;
                    await venda.Load();
                    if (venda.Cliente != null)
                        View2.ItemsSource = new List<Model.Cliente>() { venda.Cliente };
                    else View2.ItemsSource = null;
                    var items = (await Synchro.tbItem.ReadAsync()).Where(i => i.CodigoVenda == venda.Id).ToList();
                    var lista = new List<dynamic>();
                    foreach (var item in items)
                    {
                        await item.Load();
                        lista.Add(new { Codigo = item.Produto.Codigo, Nome = item.Produto.Nome, Quantidade = item.QuantidadeProduto, Valor = item.ValorProduto, Data = item.Venda.Data, DataPrestacao = item.Venda.DataPrestacao });
                    }
                    View1.ItemsSource = lista;
                }
                if (row is Model.Retirada)
                {
                    var retirada = row as Model.Retirada;
                    await retirada.Load();
                    if (retirada.Produto != null)
                        View1.ItemsSource = new List<Model.Produto>() { retirada.Produto };
                    else View1.ItemsSource = null;
                    if (retirada.Vendedor != null)
                        View2.ItemsSource = new List<Model.Vendedor>() { retirada.Vendedor };
                    else View2.ItemsSource = null;
                }
            }
        }

        private void View_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor coluna = (PropertyDescriptor)e.PropertyDescriptor;
            switch (coluna.DisplayName)
            {
                case "CodigoProduto":
                    e.Column.Header = "Código do Produto";
                    break;
                case "QuantidadeProduto":
                    e.Column.Header = "Quantidade do Produto";
                    break;
                case "ValorProduto":
                    e.Column.Header = "Valor Total do Produto";
                    break;
                default:
                    e.Column.Header = coluna.DisplayName;
                    break;
            }
            List<string> blackList = new List<string>()
            {
                "Version", "Id", "CodigoVenda", "Venda", "Produto"
            };
            if (blackList.Contains(coluna.DisplayName))
            {
                e.Cancel = true;
            }
        }
        
    }
}
