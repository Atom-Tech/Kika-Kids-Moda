using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace KikaKidsModa
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MobileServiceClient banco = new MobileServiceClient("https://lojaroupaapi.azurewebsites.net");

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            /*if (e.Exception is MobileServiceConflictException)
            {
                MessageBox.Show("Um conflito aconteceu");
            }*/
            MessageBox.Show($"Erro {e.Exception.InnerException}: {e.Exception.Message}");
            e.Handled = true;
        }
    }
}
