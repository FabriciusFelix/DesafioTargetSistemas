using Desafio;
using DesafioTargetSistemas.Domain.Enums;
using static DesafioTargetSistemas.Domain.ClassesQuestao2;

namespace DesafioTargetSistemas.Application.Services
{
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
            return Movimentar(codigoProduto, quantidade, TipoMovimentacaoEnum.Entrada, descricao);
        }

        public RegistroMovimentacao RegistrarSaida(int codigoProduto, int quantidade, string descricao)
        {
            return Movimentar(codigoProduto, quantidade, TipoMovimentacaoEnum.Saida, descricao);
        }

        private RegistroMovimentacao Movimentar(int codigoProduto, int quantidade, TipoMovimentacaoEnum tipo, string descricao)
        {
            if (!_produtos.TryGetValue(codigoProduto, out var produto))
                throw new KeyNotFoundException($"Produto {codigoProduto} não encontrado.");

            if (tipo == TipoMovimentacaoEnum.Entrada)
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

}
