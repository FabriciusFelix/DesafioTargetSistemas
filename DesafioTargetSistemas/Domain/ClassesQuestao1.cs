using System.Text.Json.Serialization;

namespace DesafioTargetSistemas.Domain
{
    public class ClassesQuestao1
    {
        public class Venda
        {
            [JsonPropertyName("vendedor")]
            public string Vendedor { get; set; } = string.Empty;

            [JsonPropertyName("valor")]
            public decimal Valor { get; set; }
        }

        public class BaseVendasJson
        {
            [JsonPropertyName("vendas")]
            public List<Venda> Vendas { get; set; } = new();
        }

        public class ResumoComissaoVendedor
        {
            public string Vendedor { get; set; } = string.Empty;
            public int QuantidadeVendas { get; set; }
            public decimal TotalVendas { get; set; }
            public decimal TotalComissao { get; set; }
        }
    }
}
