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
    /// Interaction logic for Produto.xaml
    /// </summary>
    public partial class Produto : Page
    {
        int op = 0;
        Model.Produto Prod = new Model.Produto();

        public Produto()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
            AtivarCampos(false);
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
                switch (Prod.Tamanho)
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
            Prod = new Model.Produto();
            CampoCodigo.Text = "";
            CampoNome.Text = "";
            CampoDescricao.Text = "";
            CampoQuantidade.Value = 0;
            CampoTamanho.SelectedIndex = 1;
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
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (Prod.Id != null)
            {
                op = 2;
                AtivarCampos(true);
            }
        }

        private async void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (Prod.Id != null)
            {
                var message = MessageBox.Show("Tem certeza que deseja deletar? Não será possivel recuperar depois", "Aviso", MessageBoxButton.YesNo);
                if (message == MessageBoxResult.Yes)
                {
                    await Control.ProdutoControl.Delete(Prod);
                    Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
                }
            }
        }

        private void CampoNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            Prod.Nome = CampoNome.Text;
        }


        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await InsertUpdate())
                    Lista.ItemsSource = await Synchro.tbProduto.ReadAsync();
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
                    if (await SemUnique())
                        await Control.ProdutoControl.Insert(Prod);
                    else
                    {
                        MessageBox.Show("Não pode haver produtos com o mesmo código");
                        return false;
                    }
                    return true;
                case 2: //Alterar
                    if (await SemUnique())
                        await Control.ProdutoControl.Update(Prod);
                    else
                    {
                        MessageBox.Show("Não pode haver produtos com o mesmo código");
                        return false;
                    }
                    return true;
            }
            return false;
        }

        public async Task<bool> SemUnique()
        {
            return (await Synchro.tbProduto.ReadAsync()).Where(p => p.Codigo == Prod.Codigo).Count() == 0;
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoCodigo.Text != "" && CampoTamanho.Text != ""
                && CampoValor.Value.HasValue && CampoQuantidade.Value.HasValue;

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
            if (CampoTamanho.SelectedIndex == 0) Prod.Tamanho = "P";
            if (CampoTamanho.SelectedIndex == 1) Prod.Tamanho = "M";
            if (CampoTamanho.SelectedIndex == 2) Prod.Tamanho = "G";
            if (CampoTamanho.SelectedIndex == 3) Prod.Tamanho = "GG";
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
    }
}
