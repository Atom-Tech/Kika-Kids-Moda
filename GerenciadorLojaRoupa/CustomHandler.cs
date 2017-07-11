using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.MobileServices;
using KikaKidsModa.Model;
using System.Diagnostics;
using System.Net;

namespace KikaKidsModa
{
    public class CustomHandler : MobileServiceSyncHandler
    {

        /*public async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            JObject result = null;
            MobileServicePreconditionFailedException<Caixa> caixaConflict = null;
            MobileServicePreconditionFailedException<Cliente> clienteConflict = null;
            MobileServicePreconditionFailedException<Produto> produtoConflict = null;
            MobileServicePreconditionFailedException<Retirada> retiradaConflict = null;
            MobileServicePreconditionFailedException<Usuario> UsuarioConflict = null;
            MobileServicePreconditionFailedException<Venda> vendaConflict = null;
            MobileServicePreconditionFailedException<Vendedor> vendedorConflict = null;

            try
            {
                operation.AbortPush();
                result = await operation.ExecuteAsync();
            }
            catch (MobileServicePreconditionFailedException<Dairy> ex)
            {
                // Azure's version of the Dairy - Strongly typed
                var serverDairyItem = ex.Item;

                // App's version of the Dairy - in conflict with what is in Azure
                var localDairyItem = operation.Item.ToObject<Dairy>();


                // Do something to handle the dairy conflict

            }
            catch (MobileServicePreconditionFailedException<Cheese> ex)
            {
                // Azure's version of the Cheese - strongly typed
                var serverCheeseItem = ex.Item;

                // App's version of the Cheese - in conflict with what is in Azure
                var localCheeseItem = operation.Item.ToObject<Cheese>();

                // Do something to handle the cheese conflict
            }

            return result;
        }*/

        public async override Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            try
            {
                return await base.ExecuteTableOperationAsync(operation);
            }
            catch (MobileServiceConflictException cex)
            {
                Debug.WriteLine(cex.Message);
                throw;
            }
        }

        public override Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Status == HttpStatusCode.Conflict)
                {
                    error.CancelAndUpdateItemAsync(error.Result);
                    error.Handled = true;
                }
            }
            return base.OnPushCompleteAsync(result);
        }
        
    }
}
