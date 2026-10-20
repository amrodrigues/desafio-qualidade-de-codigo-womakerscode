namespace Ecommerce.Services
{
    public interface IPagamentoService
    {
        Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento);
    }
}
