using Desafio;
using static DesafioTargetSistemas.Domain.ClassesQuestao3;

namespace DesafioTargetSistemas.Application.Services
{
    public class ServicoCalculoCobranca
    {
        private const decimal TaxaMultaDiaria = 0.025m;

        public ResultadoCalculoJuros Calcular(decimal valor, DateTime dataVencimento, DateTime? dataReferencia = null)
        {
            if (valor < 0)
                throw new ArgumentOutOfRangeException(nameof(valor), "O valor não pode ser negativo.");

            DateTime hoje = (dataReferencia ?? DateTime.Today).Date;
            DateTime vencimento = dataVencimento.Date;

            int diasAtraso = 0;
            decimal valorJuros = 0m;

            if (hoje > vencimento)
            {
                diasAtraso = (hoje - vencimento).Days;
                valorJuros = valor * (TaxaMultaDiaria * diasAtraso);
            }

            return new ResultadoCalculoJuros
            {
                ValorOriginal = valor,
                DataVencimento = vencimento,
                DataCalculo = hoje,
                DiasAtraso = diasAtraso,
                TaxaDiariaPercentual = TaxaMultaDiaria * 100m,
                ValorJuros = valorJuros,
                ValorTotalFinal = valor + valorJuros
            };
        }
    }
}
