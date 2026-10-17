using Ecommerce.Data;
using Ecommerce.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProdutosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> ListarProdutos() => Ok(await _db.Produtos.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarProduto(int id)
        {
            var produto = await _db.Produtos.FindAsync(id);
            return produto == null ? NotFound() : Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome)) return BadRequest("Nome inválido.");
            if (produto.Preco <= 0) return BadRequest("Preço inválido.");

            _db.Produtos.Add(produto);
            await _db.SaveChangesAsync();
            Utils.Log($"Produto criado: {produto.Nome}");
            return Ok(produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
        {
            var existente = await _db.Produtos.FindAsync(id);
            if (existente == null) return NotFound();

            existente.Nome = produto.Nome;
            existente.Descricao = produto.Descricao;
            existente.Preco = produto.Preco;
            existente.QuantidadeEstoque = produto.QuantidadeEstoque;
            existente.Ativo = produto.Ativo;

            await _db.SaveChangesAsync();
            return Ok(existente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarProduto(int id)
        {
            var existente = await _db.Produtos.FindAsync(id);
            if (existente == null) return NotFound();

            _db.Produtos.Remove(existente);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}