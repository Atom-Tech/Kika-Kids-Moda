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
    /// Interaction logic for JanelaCliente.xaml
    /// </summary>
    public partial class JanelaCliente : Window
    {
        bool cpfValido = false;
        Model.Cliente Cli = new Model.Cliente();

        public JanelaCliente()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void CampoCPF_TextChanged(object sender, TextChangedEventArgs e)
        {
            cpfValido = Validar.CPF(CampoCPF.Text);
            CampoCPF.BorderBrush = cpfValido ? Brushes.Green : Brushes.Red;
            if (cpfValido) Cli.CPF = CampoCPF.Text;
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
                        Venda.ClienteCadastrado = Cli.CPF;
                        Close();
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
            await Control.ClienteControl.Insert(Cli);
            return true;
        }

        public bool VerificarCamposVazios() => CampoNome.Text != "" && CampoTel.IsMaskCompleted && CampoEnd.Text != ""
                && cpfValido && CampoRG.IsMaskCompleted;

        private void BotaoCancelar_Click(object sender, RoutedEventArgs e)
        {
            Venda.ClienteCadastrado = null;
            Close();
        }

        double width, height;

        private void minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        
        private void maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                maximize.Visibility = Visibility.Collapsed;
                restore.Visibility = Visibility.Visible;
                height = Height;
                width = Width;
                WindowState = WindowState.Maximized;
            }
            else
            {
                maximize.Visibility = Visibility.Visible;
                restore.Visibility = Visibility.Collapsed;
                Height = height;
                Width = width;
                WindowState = WindowState.Normal;
            }
        }

        private bool _isMouseDown = false;

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && this.WindowState == WindowState.Maximized)
            {
                _isMouseDown = false;
                this.WindowState = WindowState.Normal;
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            this.DragMove();
        }
        private void TitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }
    }
}
