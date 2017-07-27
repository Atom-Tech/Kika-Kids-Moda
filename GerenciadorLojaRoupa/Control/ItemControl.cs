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
    public static class ItemControl
    {
        public static async Task Insert(Model.Item c)
        {
            MobileServicePreconditionFailedException<Model.Item> exception = null;
            try
            {
                await Synchro.tbItem.InsertAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Item> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Update(Model.Item c)
        {
            MobileServicePreconditionFailedException<Model.Item> exception = null;
            try
            {
                await Synchro.tbItem.UpdateAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Item> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        private static async Task ResolveConflict(Model.Item localItem, Model.Item serverItem)
        {
            localItem.Version = serverItem.Version;
            await Update(localItem);
        }

        public static async Task Delete(Model.Item c)
        {
            MobileServicePreconditionFailedException<Model.Item> exception = null;
            try
            {
                await Synchro.tbItem.DeleteAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Item> ex)
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
