using Desafio;
using System.Text.Json.Serialization;

namespace DesafioTargetSistemas.Domain
{
     public class ClassesQuestao3
    {
        public class ResultadoCalculoJuros
        {
            public decimal ValorOriginal { get; set; }
            public DateTime DataVencimento { get; set; }
            public DateTime DataCalculo { get; set; }
            public int DiasAtraso { get; set; }
            public decimal TaxaDiariaPercentual { get; set; }
            public decimal ValorJuros { get; set; }
            public decimal ValorTotalFinal { get; set; }
            public bool EstaEmAtraso => DiasAtraso > 0;
        }
    }
}
