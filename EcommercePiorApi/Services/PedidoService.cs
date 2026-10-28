using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Enum;
using Ecommerce.Model;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo;

namespace Ecommerce.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _db;

        //CORREÇÃO: Injetando a chamada de pagamento
        private readonly IPagamentoService _pagamentoService;
        private readonly ICalculadoraDesconto _calculadoraDesconto;

        public PedidoService(
            AppDbContext db,
            IPagamentoService pagamentoService,
            ICalculadoraDesconto calculadoraDesconto)
        {
            _db = db;
            _pagamentoService = pagamentoService;
            _calculadoraDesconto = calculadoraDesconto;
        }

        public async Task<Pedido> CriarPedidoAsync(PedidoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ClienteNome))
                throw new ArgumentException("Informe o nome do cliente.");
            if (!Utils.ValidarEmail(request.ClienteEmail))
                throw new ArgumentException("E-mail inválido.");
            if (request.Itens == null || request.Itens.Count == 0)
                throw new ArgumentException("O pedido deve conter ao menos um item.");

            // Utiliza transação para garantir atomicidade (evita o bug de baixar estoque e perder se falhar)
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var pedido = new Pedido
                {
                    ClienteNome = request.ClienteNome,
                    ClienteEmail = request.ClienteEmail,
                    DataPedido = DateTime.UtcNow,
                    FormaPagamento = request.FormaPagamento,
                    Status = StatusPedido.Pendente
                };

                decimal total = 0;

                foreach (var itemReq in request.Itens)
                {
                    var produto = await _db.Produtos.FindAsync(itemReq.ProdutoId);
                    if (produto == null)
                        throw new InvalidOperationException($"Produto {itemReq.ProdutoId} não encontrado.");
                    if (!produto.Ativo)
                        throw new InvalidOperationException($"Produto {produto.Nome} está inativo.");
                    if (produto.QuantidadeEstoque < itemReq.Quantidade)
                        throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.Nome}.");

                    // Baixa o estoque
                    produto.QuantidadeEstoque -= itemReq.Quantidade;

                    pedido.Itens.Add(new ItemPedido
                    {
                        ProdutoId = produto.Id,
                        Quantidade = itemReq.Quantidade,
                        PrecoUnitario = produto.Preco
                    });

                    total += produto.Preco * itemReq.Quantidade;
                }

                pedido.ValorTotal = _calculadoraDesconto.Aplicar(total, request.ClienteEmail);

                _db.Pedidos.Add(pedido);
                await _db.SaveChangesAsync();

                //CORREÇÃO: Chamada usando a interface de pagamento 
                bool pagamentoOk = await _pagamentoService.ProcessarPagamentoAsync(pedido.ValorTotal, request.FormaPagamento);
                if (!pagamentoOk)
                {
                    pedido.Status = StatusPedido.Falhou;
                    await _db.SaveChangesAsync();
                    await transaction.RollbackAsync();
                    PiorEcommerceDoMundo.Utils.Log($"Pagamento falhou para o pedido {pedido.Id}. Estoque estornado.");
                    throw new InvalidOperationException("Pagamento recusado.");
                }

                pedido.Status = StatusPedido.Pago;
                await _db.SaveChangesAsync();

                // Logística
                var codigoRastreio = await LogisticaMock.EnviarAsync(pedido);
                pedido.CodigoRastreio = codigoRastreio;
                pedido.Status = StatusPedido.Enviado;
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                Utils.Log($"Pedido {pedido.Id} criado e processado com sucesso.");

                return pedido;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Pedido> ReprocessarPagamentoAsync(int pedidoId)
        {
            var pedido = await _db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == pedidoId);
            if (pedido == null) throw new KeyNotFoundException("Pedido não encontrado.");

            //CORREÇÂO: Processamento assíncrono
            bool ok = await _pagamentoService.ProcessarPagamentoAsync(pedido.ValorTotal, pedido.FormaPagamento);
            if (!ok)
            {
                pedido.Status = StatusPedido.Falhou;
                await _db.SaveChangesAsync();
                throw new InvalidOperationException("Pagamento recusado novamente.");
            }
    
            pedido.Status = StatusPedido.Pago;
            await _db.SaveChangesAsync();
            return pedido;
        }

        public async Task<Pedido> ReenviarLogisticaAsync(int pedidoId)
        {
            var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);
            if (pedido == null) throw new KeyNotFoundException("Pedido não encontrado.");

            var codigo = await LogisticaMock.EnviarAsync(pedido);
            pedido.CodigoRastreio = codigo;
            pedido.Status = StatusPedido.Enviado;
            await _db.SaveChangesAsync();
            return pedido;
        }

        public async Task<IEnumerable<Pedido>> ListarPedidosAsync()
        {
            return await _db.Pedidos.Include(p => p.Itens).ToListAsync();
        }

        public async Task<Pedido?> BuscarPedidoPorIdAsync(int id)
        {
            return await _db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}