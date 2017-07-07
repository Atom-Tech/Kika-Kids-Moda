using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class VendaControl
    {
        public static async Task Insert(Model.Venda v)
        {
            await Synchro.tbVenda.InsertAsync(v);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
        
        public static async Task Delete(Model.Venda v)
        {
            await Synchro.tbVenda.DeleteAsync(v);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }

        public static async Task Update(Model.Venda v)
        {
            await Synchro.tbVenda.UpdateAsync(v);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
    }
}
