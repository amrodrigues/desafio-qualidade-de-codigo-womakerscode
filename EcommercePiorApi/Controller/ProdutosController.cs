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
        public async Task<IActionResult> ListarProdutos()
        {
            try
            {
                var produtos = await _db.Produtos.ToListAsync();
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                Utils.Log($"Erro técnico ao listar produtos: {ex.Message}");
                return StatusCode(500, "Não foi possível carregar os produtos no momento.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarProduto(int id)
        {
            try
            {
                var produto = await _db.Produtos.FindAsync(id);
                return produto == null ? NotFound("Produto não encontrado.") : Ok(produto);
            }
            catch (Exception ex)
            {
                Utils.Log($"Erro técnico ao buscar produto {id}: {ex.Message}");
                return StatusCode(500, "Não foi possível carregar as informações do produto.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome)) return BadRequest("Nome inválido.");
            if (produto.Preco <= 0) return BadRequest("Preço inválido.");
            try
            {
                _db.Produtos.Add(produto);
                await _db.SaveChangesAsync();
                Utils.Log($"Produto criado: {produto.Nome}");
                return Ok(produto);
            }
            catch (Exception ex)
            {
                Utils.Log($"Erro técnico ao criar produto: {ex.Message}");
                return StatusCode(500, "Não foi possível criar o produto no momento.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome)) return BadRequest("Nome inválido.");
            if (produto.Preco <= 0) return BadRequest("Preço inválido.");

            try
            {

                var existente = await _db.Produtos.FindAsync(id);
                if (existente == null) return NotFound();

                existente.Nome = produto.Nome;
                existente.Descricao = produto.Descricao;
                existente.Preco = produto.Preco;
                existente.QuantidadeEstoque = produto.QuantidadeEstoque;
                existente.Ativo = produto.Ativo;

                await _db.SaveChangesAsync();
                Utils.Log($"Produto atualizado: ID {id} - {existente.Nome}");
                return Ok(existente);
            }
            catch (Exception ex)
            {
                Utils.Log($"Erro técnico ao atualizar produto {id} - {ex.Message}");
                return StatusCode(500, "Não foi possível atualizar o produto no momento.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarProduto(int id)
        {
            try
            {
                var existente = await _db.Produtos.FindAsync(id);
                if (existente == null) return NotFound("Produto não encontrado.");

                _db.Produtos.Remove(existente);
                await _db.SaveChangesAsync();
                Utils.Log($"Produto removido: ID {id} - {existente.Nome}");
                return Ok();
            }
            catch (Exception ex)
            {

                Utils.Log($"Erro técnico ao deletar produto {id}: {ex.Message}");
                return StatusCode(500, "Não foi possível remover o produto no momento.");
            }
        }
    }
}