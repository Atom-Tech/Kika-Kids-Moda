using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class ClienteControl
    {
        public static async Task Insert(Model.Cliente c)
        {
            await Synchro.tbCliente.InsertAsync(c);
            await App.banco.SyncContext.PushAsync();
        }
        
        public static async Task Delete(Model.Cliente c)
        {
            await Synchro.tbCliente.DeleteAsync(c);
            await App.banco.SyncContext.PushAsync();
        }

        public static async Task Update(Model.Cliente c)
        {
            await Synchro.tbCliente.UpdateAsync(c);
            await App.banco.SyncContext.PushAsync();
        }
    }
}
