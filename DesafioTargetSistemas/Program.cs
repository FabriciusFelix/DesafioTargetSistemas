using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Desafio;

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

public enum TipoMovimentacao
{
    Entrada = 1,
    Saida = 2
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

public class BaseEstoqueJson
{
    [JsonPropertyName("estoque")]
    public List<Produto> Produtos { get; set; } = new();
}

public class RegistroMovimentacao
{
    public int Id { get; set; }
    public int CodigoProduto { get; set; }
    public string DescricaoProduto { get; set; } = string.Empty;
    public TipoMovimentacao Tipo { get; set; }
    public string DescricaoOperacao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int EstoqueFinal { get; set; }
    public DateTime DataHora { get; set; }
}

public class ServicoEstoque
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<RegistroMovimentacao> _historico = new();
    private int _sequencialId = 1;

    public ServicoEstoque(IEnumerable<Produto> produtosIniciais)
    {
        _produtos = produtosIniciais.ToDictionary(p => p.Codigo);
    }

    public RegistroMovimentacao RegistrarEntrada(int codigoProduto, int quantidade, string descricao)
    {
        return Movimentar(codigoProduto, quantidade, TipoMovimentacao.Entrada, descricao);
    }

    public RegistroMovimentacao RegistrarSaida(int codigoProduto, int quantidade, string descricao)
    {
        return Movimentar(codigoProduto, quantidade, TipoMovimentacao.Saida, descricao);
    }

    private RegistroMovimentacao Movimentar(int codigoProduto, int quantidade, TipoMovimentacao tipo, string descricao)
    {
        if (!_produtos.TryGetValue(codigoProduto, out var produto))
            throw new KeyNotFoundException($"Produto {codigoProduto} não encontrado.");

        if (tipo == TipoMovimentacao.Entrada)
            produto.CreditarEstoque(quantidade);
        else
            produto.DebitarEstoque(quantidade);

        var registro = new RegistroMovimentacao
        {
            Id = _sequencialId++,
            CodigoProduto = produto.Codigo,
            DescricaoProduto = produto.Descricao,
            Tipo = tipo,
            DescricaoOperacao = descricao,
            Quantidade = quantidade,
            EstoqueFinal = produto.QuantidadeEstoque,
            DataHora = DateTime.Now
        };

        _historico.Add(registro);
        return registro;
    }

    public IReadOnlyList<RegistroMovimentacao> ObterHistorico() => _historico.AsReadOnly();
}

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

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var cultura = new CultureInfo("pt-BR");

        // 1 Calculo de comissoes
        const string jsonVendas = """
        {
          "vendas": [
            { "vendedor": "João Silva", "valor": 1200.50 },
            { "vendedor": "João Silva", "valor": 950.75 },
            { "vendedor": "João Silva", "valor": 1800.00 },
            { "vendedor": "João Silva", "valor": 1400.30 },
            { "vendedor": "João Silva", "valor": 1100.90 },
            { "vendedor": "João Silva", "valor": 1550.00 },
            { "vendedor": "João Silva", "valor": 1700.80 },
            { "vendedor": "João Silva", "valor": 250.30 },
            { "vendedor": "João Silva", "valor": 480.75 },
            { "vendedor": "João Silva", "valor": 320.40 },
            { "vendedor": "Maria Souza", "valor": 2100.40 },
            { "vendedor": "Maria Souza", "valor": 1350.60 },
            { "vendedor": "Maria Souza", "valor": 950.20 },
            { "vendedor": "Maria Souza", "valor": 1600.75 },
            { "vendedor": "Maria Souza", "valor": 1750.00 },
            { "vendedor": "Maria Souza", "valor": 1450.90 },
            { "vendedor": "Maria Souza", "valor": 400.50 },
            { "vendedor": "Maria Souza", "valor": 180.20 },
            { "vendedor": "Maria Souza", "valor": 90.75 },
            { "vendedor": "Carlos Oliveira", "valor": 800.50 },
            { "vendedor": "Carlos Oliveira", "valor": 1200.00 },
            { "vendedor": "Carlos Oliveira", "valor": 1950.30 },
            { "vendedor": "Carlos Oliveira", "valor": 1750.80 },
            { "vendedor": "Carlos Oliveira", "valor": 1300.60 },
            { "vendedor": "Carlos Oliveira", "valor": 300.40 },
            { "vendedor": "Carlos Oliveira", "valor": 500.00 },
            { "vendedor": "Carlos Oliveira", "valor": 125.75 },
            { "vendedor": "Ana Lima", "valor": 1000.00 },
            { "vendedor": "Ana Lima", "valor": 1100.50 },
            { "vendedor": "Ana Lima", "valor": 1250.75 },
            { "vendedor": "Ana Lima", "valor": 1400.20 },
            { "vendedor": "Ana Lima", "valor": 1550.90 },
            { "vendedor": "Ana Lima", "valor": 1650.00 },
            { "vendedor": "Ana Lima", "valor": 75.30 },
            { "vendedor": "Ana Lima", "valor": 420.90 },
            { "vendedor": "Ana Lima", "valor": 315.40 }
          ]
        }
        """;

        Console.WriteLine("QUestao 1 > Relatorio de comissoes:");
        var baseVendas = JsonSerializer.Deserialize<BaseVendasJson>(jsonVendas)!;
        var servicoComissao = new ServicoComissao();
        var relatorio = servicoComissao.GerarRelatorioComissoes(baseVendas.Vendas);

        foreach (var item in relatorio)
        {
            Console.WriteLine($"Vendedor: {item.Vendedor,-16} | Vendas: {item.QuantidadeVendas,2} | " +
                              $"Total: {item.TotalVendas.ToString("C", cultura),12} | " +
                              $"Comissao: {item.TotalComissao.ToString("C", cultura),10}");
        }

        // 2  Movimentacoes de estoque
        const string jsonEstoque = """
        {
          "estoque": [
            { "codigoProduto": 101, "descricaoProduto": "Caneta Azul", "estoque": 150 },
            { "codigoProduto": 102, "descricaoProduto": "Caderno Universitário", "estoque": 75 },
            { "codigoProduto": 103, "descricaoProduto": "Borracha Branca", "estoque": 200 },
            { "codigoProduto": 104, "descricaoProduto": "Lápis Preto HB", "estoque": 320 },
            { "codigoProduto": 105, "descricaoProduto": "Marcador de Texto Amarelo", "estoque": 90 }
          ]
        }
        """;

        Console.WriteLine("\n QUestao 2 > Movimentacoes de estoque:");
        var baseEstoque = JsonSerializer.Deserialize<BaseEstoqueJson>(jsonEstoque)!;
        var servicoEstoque = new ServicoEstoque(baseEstoque.Produtos);

        var m1 = servicoEstoque.RegistrarEntrada(101, 50, "Entrada NF 4920");
        Console.WriteLine($"[ID: {m1.Id}] {m1.Tipo,-7} | Produto: {m1.DescricaoProduto,-25} | Qtd: {m1.Quantidade,3} | Saldo final: {m1.EstoqueFinal,3} | Motivo: {m1.DescricaoOperacao}");

        var m2 = servicoEstoque.RegistrarSaida(102, 20, "Pedido 8831");
        Console.WriteLine($"[ID: {m2.Id}] {m2.Tipo,-7} | Produto: {m2.DescricaoProduto,-25} | Qtd: {m2.Quantidade,3} | Saldo final: {m2.EstoqueFinal,3} | Motivo: {m2.DescricaoOperacao}");

        var m3 = servicoEstoque.RegistrarSaida(104, 50, "Transferencia filial");
        Console.WriteLine($"[ID: {m3.Id}] {m3.Tipo,-7} | Produto: {m3.DescricaoProduto,-25} | Qtd: {m3.Quantidade,3} | Saldo final: {m3.EstoqueFinal,3} | Motivo: {m3.DescricaoOperacao}");

        var m4 = servicoEstoque.RegistrarEntrada(103, 15, "Devolçao cliente");
        Console.WriteLine($"[ID: {m4.Id}] {m4.Tipo,-7} | Produto: {m4.DescricaoProduto,-25} | Qtd: {m4.Quantidade,3} | Saldo final: {m4.EstoqueFinal,3} | Motivo: {m4.DescricaoOperacao}");

        // 3  Calculo de juros
        Console.WriteLine("\nQUestao 3  > Calculo de juros:");
        var servicoCobranca = new ServicoCalculoCobranca();

        var b1 = servicoCobranca.Calcular(1000.00m, DateTime.Today.AddDays(-10));
        Console.WriteLine($"Valor: {b1.ValorOriginal.ToString("C", cultura)} | Vencimento: {b1.DataVencimento:dd/MM/yyyy} | Atraso: {b1.DiasAtraso,2} dias | Juros: {b1.ValorJuros.ToString("C", cultura)} | Total: {b1.ValorTotalFinal.ToString("C", cultura)}");

        var b2 = servicoCobranca.Calcular(2500.50m, DateTime.Today);
        Console.WriteLine($"Valor: {b2.ValorOriginal.ToString("C", cultura)} | Vencimento: {b2.DataVencimento:dd/MM/yyyy} | Em dia   | Juros: {b2.ValorJuros.ToString("C", cultura)} | Total: {b2.ValorTotalFinal.ToString("C", cultura)}");
    }
}