using DesafioTargetSistemas.Application.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static DesafioTargetSistemas.Domain.ClassesQuestao1;
using static DesafioTargetSistemas.Domain.ClassesQuestao2;

namespace Desafio;
 
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var cultura = new CultureInfo("pt-BR"); //Forcei a cultura para pt-BR para exibir corretamente os valores monetários por garantia.

        #region Questão 1
        //  1 Calculo de comissoes
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
        #endregion

        #region Questão 2
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

        var baseEstoque = JsonSerializer.Deserialize<BaseEstoqueJson>(jsonEstoque)!;
        var servicoEstoque = new ServicoEstoque(baseEstoque.Produtos);
        #endregion

        #region Questão 3
        // 3  Calculo de juros
        var servicoCobranca = new ServicoCalculoCobranca();
        #endregion

        while (true)
        {
            Console.WriteLine("\nEscolha uma opcao:");
            Console.WriteLine("1 - QUestao 1");
            Console.WriteLine("2 - QUestao 2");
            Console.WriteLine("3 - QUestao 3");
            Console.WriteLine("0 - Sair");
            Console.Write("Opcao: ");
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
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

                    Console.WriteLine("\nPressione qualquer tecla para voltar para as 3 opcoes...");
                    if (Console.IsInputRedirected)
                        Console.ReadLine();
                    else
                        Console.ReadKey();
                    break;

                case "2":
                    while (true)
                    {
                        Console.WriteLine("\n QUestao 2 > Movimentacoes de estoque:");
                        foreach (var item in baseEstoque.Produtos)
                        {
                            Console.WriteLine($"Codigo: {item.Codigo} | Produto: {item.Descricao,-25} | Estoque: {item.QuantidadeEstoque,3}");
                        }

                        Console.Write("\nDigite o codigo do produto (ou 'voltar' para voltar para as 3 opcoes): ");
                        var entradaCodigo = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(entradaCodigo) || string.Equals(entradaCodigo, "voltar", StringComparison.OrdinalIgnoreCase) || entradaCodigo == "0" || string.Equals(entradaCodigo, "v", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (!int.TryParse(entradaCodigo, out int codigoProduto))
                        {
                            Console.WriteLine("Codigo invalido.");
                            continue;
                        }
                        var existe = baseEstoque.Produtos.FirstOrDefault(p => p.Codigo == codigoProduto);

                        if (existe == null)
                        {
                            Console.WriteLine("Produto nã0o encontrado.");
                            continue;
                        }

                        Console.Write("Tipo da movimentacao (1 - Entrada, 2 - Saida): ");
                        var tipo = Console.ReadLine();
                        if (string.Equals(tipo, "voltar", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        Console.Write("Quantidade: ");
                        var entradaQtd = Console.ReadLine();
                        if (string.Equals(entradaQtd, "voltar", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (!int.TryParse(entradaQtd, out int quantidade))
                        {
                            Console.WriteLine("Quantidade invalida.");
                            continue;
                        }

                        Console.Write("Descricao da operacao: ");
                        var descricao = Console.ReadLine();
                        if (string.Equals(descricao, "voltar", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        try
                        {
                            RegistroMovimentacao m;
                            if (tipo == "1" || string.Equals(tipo, "entrada", StringComparison.OrdinalIgnoreCase))
                            {
                                m = servicoEstoque.RegistrarEntrada(codigoProduto, quantidade, descricao ?? "");
                            }
                            else if (tipo == "2" || string.Equals(tipo, "saida", StringComparison.OrdinalIgnoreCase))
                            {
                                m = servicoEstoque.RegistrarSaida(codigoProduto, quantidade, descricao ?? "");
                            }
                            else
                            {
                                Console.WriteLine("Tipo invalido.");
                                continue;
                            }

                            Console.WriteLine($"[ID: {m.Id}] {m.Tipo,-7} | Produto: {m.DescricaoProduto,-25} | Qtd: {m.Quantidade,3} | Saldo final: {m.EstoqueFinal,3} | Motivo: {m.DescricaoOperacao}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;

                case "3":
                    while (true)
                    {
                        Console.WriteLine("\nQUestao 3  > Calculo de juros:");
                        Console.Write("Digite o valor (ou 'voltar' para voltar para as 3 opcoes): ");
                        var entradaValor = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(entradaValor) || string.Equals(entradaValor, "voltar", StringComparison.OrdinalIgnoreCase) || entradaValor == "0" || string.Equals(entradaValor, "v", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (!decimal.TryParse(entradaValor.Replace(',', '.'), CultureInfo.InvariantCulture, out decimal valor))
                        {
                            Console.WriteLine("Valor invalido.");
                            continue;
                        }

                        Console.Write("Digite a data de vencimento (dd/MM/yyyy): ");
                        var entradaData = Console.ReadLine();
                        if (string.Equals(entradaData, "voltar", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (!DateTime.TryParse(entradaData, cultura, DateTimeStyles.None, out DateTime dataVencimento))
                        {
                            Console.WriteLine("Data invalida.");
                            continue;
                        }

                        try
                        {
                            var res = servicoCobranca.Calcular(valor, dataVencimento);
                            var statusAtraso = res.EstaEmAtraso ? $"Atraso: {res.DiasAtraso,2} dias" : "Em dia  ";
                            Console.WriteLine($"Valor: {res.ValorOriginal.ToString("C", cultura)} | Vencimento: {res.DataVencimento:dd/MM/yyyy} | {statusAtraso} | Juros: {res.ValorJuros.ToString("C", cultura)} | Total: {res.ValorTotalFinal.ToString("C", cultura)}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opcao invalida.");
                    break;
            }
        }
    }
}