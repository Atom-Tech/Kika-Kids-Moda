using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title = "tbVenda")]
    public class Venda
    {
       public string Id { get; set; }
       private string idProduto { get; set; }
       private string cpfCliente { get; set; }

       public Produto Produto { get; set; }
       public Cliente Cliente { get; set; }
       public string qtProduto { get; set; }
       public string vlVenda { get; set; }
       public string dtVenda { get; set; }
       public string dsFormaPagamento { get; set; }
    }
}
