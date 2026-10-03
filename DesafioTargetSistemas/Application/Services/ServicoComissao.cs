using System;
 
using static DesafioTargetSistemas.Domain.ClassesQuestao1;

namespace DesafioTargetSistemas.Application.Services
{
    public class ServicoComissao
    {
        public decimal ObterTaxaComissao(decimal valorVenda)
        {
            if (valorVenda < 100.00m) return 0.0m;
            if (valorVenda < 500.00m) return 0.01m;
            return 0.05m;
        }

        public decimal CalcularComissaoVenda(decimal valorVenda)
        {
            return valorVenda * ObterTaxaComissao(valorVenda);
        }

        public List<ResumoComissaoVendedor> GerarRelatorioComissoes(IEnumerable<Venda> vendas)
        {
            return vendas
                .GroupBy(v => v.Vendedor)
                .Select(g => new ResumoComissaoVendedor
                {
                    Vendedor = g.Key,
                    QuantidadeVendas = g.Count(),
                    TotalVendas = g.Sum(v => v.Valor),
                    TotalComissao = g.Sum(v => CalcularComissaoVenda(v.Valor))
                })
                .OrderByDescending(r => r.TotalVendas)
                .ToList();
        }
    }
}
