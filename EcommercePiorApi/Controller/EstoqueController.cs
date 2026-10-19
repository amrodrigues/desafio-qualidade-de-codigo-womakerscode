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
            try
            {
            var produtos = await _db.Produtos
                .Select(p => new { produto = p.Nome, quantidade = p.QuantidadeEstoque })
                .ToListAsync();

            return Ok(produtos);
            }
            catch (Exception ex)
            {
                Utils.Log($"Erro técnico ao listar estoque: {ex.Message}");
                return StatusCode(500, "Não foi possível carregar as informações de estoque no momento.");
            }
        }

        [HttpPost("reabastecer/{produtoId}/{quantidade}")]
        public async Task<IActionResult> ReabastecerEstoque(int produtoId, int quantidade)
        {
            if (quantidade <= 0) return BadRequest("A quantidade deve ser maior que zero.");
            try
            {
            var produto = await _db.Produtos.FindAsync(produtoId);
            if (produto == null) return NotFound();

            produto.QuantidadeEstoque += quantidade;
            await _db.SaveChangesAsync();
            Utils.Log($"Estoque reabastecido: {quantidade} unidades adicionadas ao produto {produtoId}");
            return Ok(produto);
            }
        catch (Exception ex)
        {
        Utils.Log($"Erro técnico ao reabastecer estoque: {ex.Message}");
        
        return StatusCode(500, "Não foi possível concluir a operação de reabastecimento.");
        }
    }
}

}