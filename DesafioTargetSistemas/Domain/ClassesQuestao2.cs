using Desafio;
using DesafioTargetSistemas.Domain.Enums;
using System.Text.Json.Serialization;

namespace DesafioTargetSistemas.Domain
{
    public class ClassesQuestao2
    {
        public class BaseEstoqueJson
        {
            [JsonPropertyName("estoque")]
            public List<Produto> Produtos { get; set; } = new();
        }
        public class Produto
        {
            [JsonPropertyName("codigoProduto")]
            public int Codigo { get; set; }

            [JsonPropertyName("descricaoProduto")]
            public string Descricao { get; set; } = string.Empty;

            [JsonPropertyName("estoque")]
            public int QuantidadeEstoque { get; set; }

            public void CreditarEstoque(int quantidade)
            {
                if (quantidade <= 0)
                    throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade de entrada deve ser maior que zero.");

                QuantidadeEstoque += quantidade;
            }

            public void DebitarEstoque(int quantidade)
            {
                if (quantidade <= 0)
                    throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade de saída deve ser maior que zero.");

                if (QuantidadeEstoque < quantidade)
                    throw new InvalidOperationException($"Saldo insuficiente para {Descricao}. Disponível: {QuantidadeEstoque}, Solicitado: {quantidade}.");

                QuantidadeEstoque -= quantidade;
            }
        }
        public class RegistroMovimentacao
        {
            public int Id { get; set; }
            public int CodigoProduto { get; set; }
            public string DescricaoProduto { get; set; } = string.Empty;
            public TipoMovimentacaoEnum Tipo { get; set; }
            public string DescricaoOperacao { get; set; } = string.Empty;
            public int Quantidade { get; set; }
            public int EstoqueFinal { get; set; }
            public DateTime DataHora { get; set; }
        }

    }
}
