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
    public static class UsuarioControl
    {
        public static async Task Insert(Model.Usuario c)
        {
            MobileServicePreconditionFailedException<Model.Usuario> exception = null;
            try
            {
                await Synchro.tbUsuario.InsertAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Usuario> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        public static async Task Update(Model.Usuario c)
        {
            MobileServicePreconditionFailedException<Model.Usuario> exception = null;
            try
            {
                await Synchro.tbUsuario.UpdateAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Usuario> ex)
            {
                exception = ex;
            }
            if (exception != null)
            {
                await ResolveConflict(c, exception.Item);
            }
        }

        private static async Task ResolveConflict(Model.Usuario localItem, Model.Usuario serverItem)
        {
            localItem.Version = serverItem.Version;
            await Update(localItem);
        }

        public static async Task Delete(Model.Usuario c)
        {
            MobileServicePreconditionFailedException<Model.Usuario> exception = null;
            try
            {
                await Synchro.tbUsuario.DeleteAsync(c);
                if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
            }
            catch (MobileServicePreconditionFailedException<Model.Usuario> ex)
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
