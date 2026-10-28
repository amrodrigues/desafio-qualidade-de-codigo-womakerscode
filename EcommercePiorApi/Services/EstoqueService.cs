using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly AppDbContext _db;

        public EstoqueService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<object>> ListarEstoqueAsync()
        {
            return await _db.Produtos
                .Select(p => new { produto = p.Nome, quantidade = p.QuantidadeEstoque })
                .ToListAsync();
        }

        public async Task<bool> ReabastecerEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await _db.Produtos.FindAsync(produtoId);
            if (produto == null) return false;

            produto.QuantidadeEstoque += quantidade;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}