using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    public class RelatorioRetirada : Relatorio
    {        
        public RelatorioRetirada(IGrouping<int, Retirada> l)
        {
            foreach (var i in l)
            {
                Produtos.Add(i);
            }
            Datas += $"{Produtos.First().Data} - {Produtos.Last().Data}";
            Tipo = "Retirada";
        }

        public List<Retirada> Produtos { get; set; } = new List<Retirada>();
        

    }
}
