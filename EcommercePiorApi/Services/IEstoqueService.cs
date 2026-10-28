namespace Ecommerce.Services
{
    public interface IEstoqueService
    {
        Task<IEnumerable<object>> ListarEstoqueAsync();
        Task<bool> ReabastecerEstoqueAsync(int produtoId, int quantidade);
    }
}
