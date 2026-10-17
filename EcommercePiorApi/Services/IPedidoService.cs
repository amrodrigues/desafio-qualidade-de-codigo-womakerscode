using Ecommerce.DTO;
using Ecommerce.Model;

namespace Ecommerce.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CriarPedidoAsync(PedidoRequest request);
        Task<Pedido> ReprocessarPagamentoAsync(int pedidoId);
        Task<Pedido> ReenviarLogisticaAsync(int pedidoId);
    }
}