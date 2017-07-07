using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class UsuarioControl
    {
        public static async Task Insert(Model.Usuario u)
        {
            await Synchro.tbUsuario.InsertAsync(u);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
        
        public static async Task Delete(Model.Usuario u)
        {
            await Synchro.tbUsuario.DeleteAsync(u);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }

        public static async Task Update(Model.Usuario u)
        {
            await Synchro.tbUsuario.UpdateAsync(u);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
    }
}
