using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KikaKidsModa.Control
{
    public static class ClienteControl
    {
        public static async Task Insert(Model.Cliente c)
        {
            MobileServicePreconditionFailedException<Model.Cliente> exception = null;
            try
            {
                await Synchro.tbCliente.InsertAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Cliente> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Update(Model.Cliente c)
        {
            MobileServicePreconditionFailedException<Model.Cliente> exception = null;
            try
            {
                await Synchro.tbCliente.UpdateAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Cliente> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        private static async Task ResolveConflict(Model.Cliente localItem, Model.Cliente serverItem)
        {
            localItem.Version = serverItem.Version;
            await Update(localItem);
        }

        public static async Task Delete(Model.Cliente c)
        {
            MobileServicePreconditionFailedException<Model.Cliente> exception = null;
            try
            {
                await Synchro.tbCliente.DeleteAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Cliente> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }
    }
}
