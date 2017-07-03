using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class CaixaControl
    {
        public static async Task Insert(Model.Caixa c)
        {
            await Synchro.tbCaixa.InsertAsync(c);
            await App.banco.SyncContext.PushAsync();
        }

        public static async Task Delete()
        {
            foreach (var c in (await Synchro.tbCaixa.ReadAsync()))
            {
                await Synchro.tbCaixa.DeleteAsync(c);
            }
        }

        public static async Task Update(Model.Caixa c)
        {
            await Synchro.tbCaixa.UpdateAsync(c);
            await App.banco.SyncContext.PushAsync();
        }
    }
}
