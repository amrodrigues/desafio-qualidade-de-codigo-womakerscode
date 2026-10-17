using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/estoque")]
    public class EstoqueController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EstoqueController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> ListarEstoque()
        {
            var produtos = await _db.Produtos
                .Select(p => new { produto = p.Nome, quantidade = p.QuantidadeEstoque })
                .ToListAsync();

            return Ok(produtos);
        }

        [HttpPost("reabastecer/{produtoId}/{quantidade}")]
        public async Task<IActionResult> ReabastecerEstoque(int produtoId, int quantidade)
        {
            var produto = await _db.Produtos.FindAsync(produtoId);
            if (produto == null) return NotFound();

            produto.QuantidadeEstoque += quantidade;
            await _db.SaveChangesAsync();
            Utils.Log($"Estoque reabastecido produto {produtoId}");
            return Ok(produto);
        }
    }
}