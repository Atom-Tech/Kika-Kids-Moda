using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class VendedorControl
    {
        public static async Task Insert(Model.Vendedor v)
        {
            await Synchro.tbVendedor.InsertAsync(v);
            await App.banco.SyncContext.PushAsync();
        }
        
        public static async Task Delete(Model.Vendedor v)
        {
            await Synchro.tbVendedor.DeleteAsync(v);
            await App.banco.SyncContext.PushAsync();
        }

        public static async Task Update(Model.Vendedor v)
        {
            await Synchro.tbVendedor.UpdateAsync(v);
            await App.banco.SyncContext.PushAsync();
        }
    }
}
