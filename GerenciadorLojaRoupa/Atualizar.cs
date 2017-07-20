using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace KikaKidsModa
{
    public class Atualizar
    {
        private Version v;
        private ProgressBar barraProgresso;
        private TextBlock porcentagem, versao;
        public bool PossuiAtualizacao { get; private set; }

        public Atualizar(ProgressBar b, TextBlock p, TextBlock vr)
        {
            v = new Version();
            barraProgresso = b;
            porcentagem = p;
            versao = vr;
            PossuiAtualizacao = VerificarAtualizacao();
        }

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            barraProgresso.Visibility = Visibility.Hidden;
            MessageBox.Show("Atualização Baixada com Sucesso!");
        }

        public void FazerDownload()
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var box = MessageBox.Show("Deseja atualizar o software Gerenciador Kika Kids para a versão " + v.Versao +
    "? \nA atualização contém:\n" + v.Historico, "Atualizar", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (box == MessageBoxResult.Yes)
            {
                string local = Path.GetTempPath() + "GKK\\";
                Directory.CreateDirectory(local);
                string nomeArquivo = "GerenciadorKikaKids.v" + v.Versao + ".msi";
                barraProgresso.Visibility = Visibility.Visible;
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(v.Link), local + nomeArquivo);
            }
        }
        
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            porcentagem.Text = e.ProgressPercentage + "%";
            barraProgresso.Value = e.ProgressPercentage;
        }

        public void Instalar()
        {
            string local = Path.GetTempPath() + "GKK\\";
            var pasta = new DirectoryInfo(local);
            var arquivo = pasta.GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            using (FileStream stream = File.OpenRead(local + arquivo.Name))
            {
                using (SHA1Managed sha = new SHA1Managed())
                {
                    byte[] checksum = sha.ComputeHash(stream);
                    string sendCheckSum = BitConverter.ToString(checksum)
                        .Replace("-", string.Empty).ToLower();
                    if (sendCheckSum == v.Checksum)
                    {
                        Process.Start(local + arquivo.Name);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show("Erro no download do arquivo. Por favor, tente novamente");
                        arquivo.Delete();
                    }
                }
            }
        }

        public bool ArquivoExiste()
        {
            string local = Path.GetTempPath() + "GKK\\";
            var pasta = new DirectoryInfo(local);
            if (!pasta.Exists) pasta.Create();
            if (pasta.GetFiles().Count() > 0)
            {
                var arquivo = pasta.GetFiles()
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();
                return arquivo.Name.Contains(v.Versao);
            }
            return false;
        }

        public bool VerificarAtualizacao()
        {
            XmlSerializer xml = new XmlSerializer(v.GetType());
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string data = Encoding.UTF8.GetString(client.DownloadData("https://pastebin.com/raw/Gj6EZDre"));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
            v = (Version)xml.Deserialize(stream);
            return v.Versao != versao.Text.Replace("Versão ", "");
        }
    }
}
