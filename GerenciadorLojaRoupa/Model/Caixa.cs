using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KikaKidsModa.Model
{
    [JsonObject(Title ="tbCaixa")]
    public class Caixa
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "dtCaixa")]
        public string DataCaixa { get; set; }
        [JsonProperty(PropertyName = "hrAbertura")]
        public string HoraAbertura { get; set; }
        [JsonProperty(PropertyName = "hrFechamento")]
        public string HoraFechamento { get; set; }
        [JsonProperty(PropertyName = "vlAbertura")]
        public double ValorAbertura { get; set; }
        [JsonProperty(PropertyName = "vlFechamento")]
        public double ValorFechamento { get; set; }
        [JsonProperty(PropertyName = "vlSangria")]
        public double ValorSangria { get; set; }
        [Version]
        public byte[] Version { get; set; }
        public static Dictionary<string, string> Colunas { get; } = new Dictionary<string, string>()
        {
            { "DataCaixa", "Data" },
            { "HoraAbertura", "Horário de Abertura" },
            { "HoraFechamento", "Horário de Fechamento" },
            { "ValorAbertura", "Valor de Abertura" },
            { "ValorFechamento", "Valor de Fechamento" },
            { "ValorSangria", "Valor de Sangria" }
        };
    }
}
