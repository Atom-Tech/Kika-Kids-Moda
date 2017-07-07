using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class RetiradaControl
    {
        public static async Task Insert(Model.Retirada r)
        {
            await Synchro.tbRetirada.InsertAsync(r);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
        
        public static async Task Delete(Model.Retirada r)
        {
            await Synchro.tbRetirada.DeleteAsync(r);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }

        public static async Task Update(Model.Retirada r)
        {
            await Synchro.tbRetirada.UpdateAsync(r);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
    }
}
