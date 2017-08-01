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
    /// Interaction logic for Produto.xaml
    /// </summary>
    public partial class Produto : Page
    {
        int op = 0;
        Model.Produto Prod = new Model.Produto();
        string codigoAntigo = "";
        bool tamanhoNumero = false;

        public Produto()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
            AtivarCampos(false);
            Main.CodigoEscaneado += Main_CodigoEscaneado;
        }

        private void Main_CodigoEscaneado(object sender, ScannerEventArgs e)
        {
            if (op == 1 || op == 2)
            {
                CampoCodigo.Text = e.Codigo;
            }
            else
            {
                foreach (Model.Produto p in Lista.Items)
                {
                    if (p.Codigo == e.Codigo)
                    {
                        Lista.SelectedItem = p;
                        break;
                    }
                }
            }
        }

        private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Produto && Lista.SelectedItem != null)
            {
                Prod = (Model.Produto)Lista.SelectedItem;
                CampoCodigo.Text = Prod.Codigo;
                CampoNome.Text = Prod.Nome;
                CampoDescricao.Text = Prod.Descricao;
                CampoValor.Value = Prod.Valor;
                codigoAntigo = Prod.Codigo;
                switch (Prod.SiglaTamanho)
                {
                    case "P":
                        CampoTamanho.SelectedIndex = 0;
                        break;
                    case "M":
                        CampoTamanho.SelectedIndex = 1;
                        break;
                    case "G":
                        CampoTamanho.SelectedIndex = 2;
                        break;
                    case "GG":
                        CampoTamanho.SelectedIndex = 3;
                        break;
                    case "Único":
                        CampoTamanho.SelectedIndex = 4;
                        break;
                    case null:
                        break;
                }
                TamanhoNumero.Value = Prod.NumeroTamanho.HasValue ? Prod.NumeroTamanho.Value : 0;
                if (Prod.NumeroTamanho.HasValue && Prod.NumeroTamanho.Value != 0)
                {
                    tamanhoNumero = true;
                    RadioNumero.IsChecked = true;
                }
                else
                {
                    tamanhoNumero = false;
                    RadioSigla.IsChecked = true;
                }
                CampoQuantidade.Value = Prod.Quantidade;
            }
            AtivarCampos(false);
        }

        private async void CampoBuscaNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBuscaNome.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
            }
            else
            {
                Lista.ItemsSource = (await Synchro.tbProduto.ReadAsync()).Where(c => c.Nome.Contains(CampoBuscaNome.Text));
            }
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            Prod = new Model.Produto()
            {
                SiglaTamanho = "P"
            };
            CampoCodigo.Text = "";
            CampoNome.Text = "";
            CampoDescricao.Text = "";
            CampoQuantidade.Value = 0;
            CampoTamanho.SelectedIndex = 0;
            CampoValor.Value = 0;
            AtivarCampos(true);
        }

        public void AtivarCampos(bool vf)
        {
            CampoCodigo.IsEnabled = vf;
            CampoNome.IsEnabled = vf;
            CampoDescricao.IsEnabled = vf;
            CampoQuantidade.IsEnabled = vf;
            CampoTamanho.IsEnabled = vf;
            CampoValor.IsEnabled = vf;
            TamanhoNumero.IsEnabled = vf;
            BotaoSalvar.IsEnabled = vf;
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (Prod.Id != null)
            {
                op = 2;
                AtivarCampos(true);
            }
        }

        public async Task<bool> HasForeign()
        {
            return (await Synchro.tbRetirada.ReadAsync()).Where(v => v.CodigoProduto == Prod.Codigo).Count() > 0
            || (await Synchro.tbItem.ReadAsync()).Where(v => v.CodigoProduto == Prod.Codigo).Count() > 0;
        }


        private async void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (!await HasForeign())
            {
                if (Prod.Id != null)
                {
                    var message = MessageBox.Show("Tem certeza que deseja deletar? Não será possivel recuperar depois", "Aviso", MessageBoxButton.YesNo);
                    if (message == MessageBoxResult.Yes)
                    {
                        await Control.ProdutoControl.Delete(Prod);
                        Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
                        MessageBox.Show("Produto deletado com sucesso!");
                    }
                }
            }
            else
                MessageBox.Show("Esse produto já foi retirado ou vendido antes");
        }

        private void CampoNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            Prod.Nome = CampoNome.Text;
        }


        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await SemUnique())
                {
                    if (await InsertUpdate())
                    {
                        Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
                        op = 0;
                        AtivarCampos(false);
                    }
                }
                else
                {
                    MessageBox.Show("Não pode haver produtos com o mesmo código");
                }
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
                    await Control.ProdutoControl.Insert(Prod);
                    MessageBox.Show("Produto inserido com sucesso!");
                    return true;
                case 2: //Alterar
                    await Control.ProdutoControl.Update(Prod);
                    await UpdateCodigo();
                    MessageBox.Show("Produto alterado com sucesso!");
                    return true;
            }
            return false;
        }

        public async Task UpdateCodigo()
        {
            var vendas = await Synchro.tbVenda.ReadAsync();
            var retiradas = await Synchro.tbRetirada.ReadAsync();
            foreach (var retirada in retiradas)
            {
                if (retirada.CodigoProduto == codigoAntigo)
                {
                    retirada.CodigoProduto = Prod.Codigo;
                    await Control.RetiradaControl.Update(retirada);
                }
            }
            foreach (var venda in vendas)
            {/*
                if (venda.CodigoProduto == codigoAntigo)
                {
                    venda.CodigoProduto = Prod.Codigo;
                    await Control.VendaControl.Update(venda);
                }
*/            }
        }

        public async Task<bool> SemUnique()
        {
            return (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == Prod.Codigo && p.Id != Prod?.Id).Count() == 0;
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoCodigo.Text != "" && CheckTamanho()
                && CampoValor.Value.HasValue && CampoQuantidade.Value.HasValue;

        public bool CheckTamanho()
        {
            if (tamanhoNumero)
            {
                Prod.SiglaTamanho = null;
                return TamanhoNumero.Value.Value != 0 && TamanhoNumero.Value.HasValue;
            }
            else
            {
                Prod.NumeroTamanho = null;
                return CampoTamanho.SelectedIndex != -1;
            }
        }

        private void CampoCodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            Prod.Codigo = CampoCodigo.Text;
        }

        private void CampoDescricao_TextChanged(object sender, TextChangedEventArgs e)
        {
            Prod.Descricao = CampoDescricao.Text;
        }

        private void CampoTamanho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CampoTamanho.SelectedIndex == 0) Prod.SiglaTamanho = "P";
            if (CampoTamanho.SelectedIndex == 1) Prod.SiglaTamanho = "M";
            if (CampoTamanho.SelectedIndex == 2) Prod.SiglaTamanho = "G";
            if (CampoTamanho.SelectedIndex == 3) Prod.SiglaTamanho = "GG";
            if (CampoTamanho.SelectedIndex == 4) Prod.SiglaTamanho = "Único";
        }

        private void CampoQuantidade_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Prod.Quantidade = CampoQuantidade.Value.Value;
        }

        private void CampoValor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Prod.Valor = CampoValor.Value.Value;
        }

        private async void CampoBuscaCodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBuscaCodigo.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
            }
            else
            {
                Lista.ItemsSource = (await Synchro.tbProduto.ReadAsync()).Where(c => c.Codigo.Contains(CampoBuscaCodigo.Text));
            }
        }

        private void RadioSigla_Checked(object sender, RoutedEventArgs e)
        {
            tamanhoNumero = false;
        }

        private void RadioNumero_Checked(object sender, RoutedEventArgs e)
        {
            tamanhoNumero = true;
        }

        private void TamanhoNumero_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Prod.NumeroTamanho = TamanhoNumero.Value.HasValue ? TamanhoNumero.Value.Value : 0;
        }
    }
}
