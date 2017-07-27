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
    public static class CaixaControl
    {
        public static async Task Insert(Model.Caixa c)
        {
            MobileServicePreconditionFailedException<Model.Caixa> exception = null;
            try
            {
                await Synchro.tbCaixa.InsertAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Caixa> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Update(Model.Caixa c)
        {
            MobileServicePreconditionFailedException<Model.Caixa> exception = null;
            try
            {
                await Synchro.tbCaixa.UpdateAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Caixa> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Delete(Model.Caixa c)
        {
            MobileServicePreconditionFailedException<Model.Caixa> exception = null;
            try
            {
                await Synchro.tbCaixa.DeleteAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Caixa> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        private static async Task ResolveConflict(Model.Caixa localItem, Model.Caixa serverItem)
        {
            localItem.Version = serverItem.Version;
            await Update(localItem);
        }
    }
}
