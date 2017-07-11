using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    public class RelatorioVenda : Relatorio
    {        
        public RelatorioVenda(IGrouping<int, Venda> l)
        {
            foreach (var i in l)
            {
                Produtos.Add(i);
            }
            Datas += $"{Produtos.First().Data} - {Produtos.Last().Data}";
            Tipo = "Venda";
        }

        public List<Venda> Produtos { get; set; } = new List<Venda>();
        

    }
}
