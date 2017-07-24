using KikaKidsModa;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Control
{
    public static class ItemControl
    {
        public static async Task Insert(Model.Item i)
        {
            await Synchro.tbItem.InsertAsync(i);
            if (Main.HasInternet) await App.banco.SyncContext.PushAsync();
        }
        
    }
}
