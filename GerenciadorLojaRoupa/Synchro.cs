using KikaKidsModa;
using KikaKidsModa.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa
{
    public static class Synchro
    {
        public static IMobileServiceSyncTable<Caixa> tbCaixa { get; } = App.banco.GetSyncTable<Caixa>();
        public static IMobileServiceSyncTable<Usuario> tbUsuario { get; } = App.banco.GetSyncTable<Usuario>();
        public static IMobileServiceSyncTable<Produto> tbProduto { get; } = App.banco.GetSyncTable<Produto>();
        public static IMobileServiceSyncTable<Vendedor> tbVendedor { get; } = App.banco.GetSyncTable<Vendedor>();
        public static IMobileServiceSyncTable<Retirada> tbRetirada { get; } = App.banco.GetSyncTable<Retirada>();
        public static IMobileServiceSyncTable<Cliente> tbCliente { get; } = App.banco.GetSyncTable<Cliente>();
        public static IMobileServiceSyncTable<Venda> tbVenda { get; } = App.banco.GetSyncTable<Venda>();

        public static async Task InitLocalStoreAsync()
        {
            if (!App.banco.SyncContext.IsInitialized)
            {
                string caminho = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bancokikakids.db");
                var store = new MobileServiceSQLiteStore(caminho);
                store.DefineTable<Caixa>();
                store.DefineTable<Usuario>();
                store.DefineTable<Produto>();
                store.DefineTable<Vendedor>();
                store.DefineTable<Retirada>();
                store.DefineTable<Cliente>();
                store.DefineTable<Venda>();
                await App.banco.SyncContext.InitializeAsync(store);
            }
            await SyncAsync();
        }

        public static async Task SyncAsync()
        {
            await App.banco.SyncContext.PushAsync();
            await tbCaixa.PullAsync("tbCaixa", tbCaixa.CreateQuery());
            await tbUsuario.PullAsync("tbUsuario", tbUsuario.CreateQuery());
            await tbProduto.PullAsync("tbProduto", tbProduto.CreateQuery());
            await tbVendedor.PullAsync("tbVendedor", tbVendedor.CreateQuery());
            await tbRetirada.PullAsync("tbRetirada", tbRetirada.CreateQuery());
            await tbCliente.PullAsync("tbCliente", tbCliente.CreateQuery());
            await tbVenda.PullAsync("tbVenda", tbVenda.CreateQuery());
        }
    }
}
