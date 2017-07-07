using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class ProdutoControl
    {
        public static async Task Insert(Model.Produto p)
        {
            await Synchro.tbProduto.InsertAsync(p);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
        
        public static async Task Delete(Model.Produto p)
        {
            await Synchro.tbProduto.DeleteAsync(p);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }

        public static async Task Update(Model.Produto p)
        {
            await Synchro.tbProduto.UpdateAsync(p);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
    }
}
