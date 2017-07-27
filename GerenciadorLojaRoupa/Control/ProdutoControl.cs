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
    public static class ProdutoControl
    {
        public static async Task Insert(Model.Produto c)
        {
            MobileServicePreconditionFailedException<Model.Produto> exception = null;
            try
            {
                await Synchro.tbProduto.InsertAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Produto> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Update(Model.Produto c)
        {
            MobileServicePreconditionFailedException<Model.Produto> exception = null;
            try
            {
                await Synchro.tbProduto.UpdateAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Produto> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        private static async Task ResolveConflict(Model.Produto localItem, Model.Produto serverItem)
        {
            localItem.Version = serverItem.Version;
            await Update(localItem);
        }

        public static async Task Delete(Model.Produto c)
        {
            MobileServicePreconditionFailedException<Model.Produto> exception = null;
            try
            {
                await Synchro.tbProduto.DeleteAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Produto> ex)
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
