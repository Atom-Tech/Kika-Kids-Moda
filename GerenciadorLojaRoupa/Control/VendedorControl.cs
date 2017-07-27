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
    public static class VendedorControl
    {
        public static async Task Insert(Model.Vendedor c)
        {
            MobileServicePreconditionFailedException<Model.Vendedor> exception = null;
            try
            {
                await Synchro.tbVendedor.InsertAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Vendedor> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Update(Model.Vendedor c)
        {
            MobileServicePreconditionFailedException<Model.Vendedor> exception = null;
            try
            {
                await Synchro.tbVendedor.UpdateAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Vendedor> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        private static async Task ResolveConflict(Model.Vendedor localItem, Model.Vendedor serverItem)
        {
            localItem.Version = serverItem.Version;
            await Update(localItem);
        }

        public static async Task Delete(Model.Vendedor c)
        {
            MobileServicePreconditionFailedException<Model.Vendedor> exception = null;
            try
            {
                await Synchro.tbVendedor.DeleteAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Vendedor> ex)
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
