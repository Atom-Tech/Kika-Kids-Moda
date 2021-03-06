﻿using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Cliente.xaml
    /// </summary>
    public partial class Cliente : Page
    {
        int op = 0;
        bool cpfValido = false;
        Model.Cliente Cli = new Model.Cliente();
        string cpfAntigo = "";

        public Cliente()
        {
            InitializeComponent();
            DataContext = this;
        }

        public async Task UpdateList()
        {
            Lista.ItemsSource = null;
            Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateList();
            AtivarCampos(false);
        }

        private async void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lista.SelectedItem is Model.Cliente && Lista.SelectedItem != null)
            {
                Cli = (Model.Cliente)Lista.SelectedItem;
                CampoNome.Text = Cli.Nome;
                CampoEnd.Text = Cli.Endereco;
                CampoCPF.Text = Cli.CPF;
                CampoEmail.Text = Cli.Email;
                CampoRG.Text = Cli.RG;
                CampoTel.Text = Cli.Telefone;
                cpfAntigo = Cli.CPF;
                await LoadCompras();
            }
            AtivarCampos(false);
        }

        public async Task LoadCompras()
        {
            var listas = (await Synchro.tbVenda.ReadAsync()).Where(v => v.CPFCliente == Cli.CPF);
            var lista = new List<dynamic>();
            foreach (var l in listas)
            {
                var list = (await Synchro.tbItem.ReadAsync()).Where(i => i.CodigoVenda == l.Id);
                foreach (var item in list){
                    await item.Load();
                    lista.Add(new { Codigo = item.Produto.Codigo, Nome = item.Produto.Nome, Quantidade = item.QuantidadeProduto,
                        Valor = item.ValorProduto, Data = item.Venda.Data, DataPrestacao = item.Venda.DataPrestacao,
                        Parcelas = item.Venda.Parcelas, Desconto = item.Venda.PorcentagemDesconto, ValorTotalComDesconto = item.Venda.ValorTotalDesconto,
                        Pago = item.Venda.Pago});
                }
            }
            TabelaCompras.ItemsSource = lista;
        }

        private async void CampoBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBusca.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
            }
            else
            {
                Lista.ItemsSource = (await Synchro.tbCliente.ReadAsync()).Where(c => c.Nome.Contains(CampoBusca.Text));
            }
        }

        private void CampoCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            cpfValido = Validar.CPF(CampoCPF.Text);
            CampoCPF.BorderBrush = cpfValido ? Brushes.Green : Brushes.Red;
            if (cpfValido) Cli.CPF = CampoCPF.Text;
        }

        private void Novo_Click(object sender, RoutedEventArgs e)
        {
            op = 1;
            Cli = new Model.Cliente();
            CampoNome.Text = "";
            CampoEnd.Text = "";
            CampoCPF.Text = "";
            CampoEmail.Text = "";
            CampoRG.Text = "";
            CampoTel.Text = "";
            AtivarCampos(true);
        }

        public void AtivarCampos(bool vf)
        {
            CampoRG.IsEnabled = vf;
            CampoNome.IsEnabled = vf;
            CampoEnd.IsEnabled = vf;
            CampoCPF.IsEnabled = vf;
            CampoTel.IsEnabled = vf;
            CampoEmail.IsEnabled = vf;
            BotaoSalvar.IsEnabled = vf;
        }

        private void Alterar_Click(object sender, RoutedEventArgs e)
        {
            if (Cli.Id != null)
            {
                op = 2;
                AtivarCampos(true);
            }
        }

        public async Task<bool> HasForeign()
        {
            return (await Synchro.tbVenda.ReadAsync()).Where(c => c.CPFCliente == Cli.CPF).Count() > 0;
        }

        private async void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (!await HasForeign())
            {
                if (Cli.Id != null)
                {
                    var message = MessageBox.Show("Tem certeza que deseja deletar? Não será possivel recuperar depois", "Aviso", MessageBoxButton.YesNo);
                    if (message == MessageBoxResult.Yes)
                    {
                        await Control.ClienteControl.Delete(Cli);
                        Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
                        MessageBox.Show("Cliente deletado com sucesso!");
                    }
                }
            }
            else
                MessageBox.Show("Há vendas com esse cliente");
        }

        private void CampoRG_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.RG = CampoRG.Text;
        }

        private void CampoNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Nome = CampoNome.Text;
        }

        private void CampoEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Endereco = CampoEnd.Text;
        }

        private void CampoTel_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Telefone = CampoTel.Text;
        }

        private void CampoEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            Cli.Email = CampoEmail.Text;
        }

        private async void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarCamposVazios())
            {
                if (await SemUnique())
                {
                    if (await InsertUpdate())
                    {
                        Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
                        op = 0;
                        AtivarCampos(false);
                    }
                }
                else
                {
                    MessageBox.Show("Não pode haver Clientes com o mesmo email e/ou CPF");
                }
            }
            else
            {
                MessageBox.Show("Há campos vazios", "Aviso");
            }
        }


        public async Task<bool> SemUnique()
        {
            return (await Synchro.tbCliente.ReadAsync()).Where(v => v.Email == Cli.Email && v.Id != Cli?.Id).Count() == 0
                && (await Synchro.tbCliente.ReadAsync()).Where(v => v.CPF == Cli.CPF && v.Id != Cli?.Id).Count() == 0;
        }


        public async Task<bool> InsertUpdate()
        {
            switch (op)
            {
                case 1: //Novo
                    await Control.ClienteControl.Insert(Cli);
                    MessageBox.Show("Cliente inserido com sucesso!");
                    return true;
                case 2: //Alterar
                    await Control.ClienteControl.Update(Cli);
                    await UpdateCPF();
                    MessageBox.Show("Cliente alterado com sucesso!");
                    return true;
            }
            return false;
        }

        public async Task UpdateCPF()
        {
            var vendas = await Synchro.tbVenda.ReadAsync();
            foreach (var venda in vendas)
            {
                if (venda.CPFCliente == cpfAntigo)
                {
                    venda.CPFCliente = Cli.CPF;
                    await Control.VendaControl.Update(venda);
                }
            }
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoTel.IsMaskCompleted && CampoEnd.Text != ""
                && cpfValido && CampoRG.IsMaskCompleted;

        private async void CampoBuscaCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            Lista.ItemsSource = null;
            if (CampoBuscaCPF.Text == "")
            {
                Lista.ItemsSource = await Synchro.tbCliente.ReadAsync();
            }
            else
            {
                string t = CampoBuscaCPF.Text;
                string cpf = "";
                for (int i = 0; i < t.Length; i++)
                {
                    cpf += t.Substring(i, 1);
                    if (i == 2 || i == 5) cpf += ".";
                    if (i == 8) cpf = "-";
                }
                Lista.ItemsSource = (await Synchro.tbCliente.ReadAsync()).Where(c => c.CPF.Contains(cpf));
            }
        }

        private void TabelaCompras_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dynamic item = e.Row.Item;
            if (item != null)
            {
                if (item.Pago)
                    e.Row.Background = new SolidColorBrush(Colors.LightGreen);
                else if (!item.Pago && DateTime.Parse(item.DataPrestacao) >= DateTime.Today)
                    e.Row.Background = new SolidColorBrush(Colors.LightYellow);
                else
                    e.Row.Background = new SolidColorBrush(Colors.LightPink);
            }
        }
    }
}
