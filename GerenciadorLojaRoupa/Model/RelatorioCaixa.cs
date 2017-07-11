using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    public class RelatorioCaixa : Relatorio
    {        
        public RelatorioCaixa(IGrouping<int, Caixa> l)
        {
            foreach (var i in l)
            {
                Caixas.Add(i);
            }
            Datas += $"{Caixas.First().DataCaixa} - {Caixas.Last().DataCaixa}";
            Tipo = "Caixa";
        }

        public List<Caixa> Caixas { get; set; } = new List<Caixa>();
        

    }
}
