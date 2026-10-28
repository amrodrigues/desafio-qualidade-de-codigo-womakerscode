using Ecommerce.Data;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiorEcommerceDoMundo;

namespace Ecommerce.Controllers
{
   
        [ApiController]
        [Route("api/estoque")]
        public class EstoqueController : ControllerBase
        {
            private readonly IEstoqueService _estoqueService;

            public EstoqueController(IEstoqueService estoqueService)
            {
                _estoqueService = estoqueService;
            }

            [HttpGet]
            public async Task<IActionResult> ListarEstoque()
            {
                try
                {
                    var produtos = await _estoqueService.ListarEstoqueAsync();
                    return Ok(produtos);
                }
                catch (Exception ex)
                {
                    PiorEcommerceDoMundo.Utils.Log($"Erro técnico ao listar estoque: {ex.Message}");
                    return StatusCode(500, "Não foi possível carregar as informações de estoque no momento.");
                }
            }

            [HttpPost("reabastecer/{produtoId}/{quantidade}")]
            public async Task<IActionResult> ReabastecerEstoque(int produtoId, int quantidade)
            {
                if (quantidade <= 0) return BadRequest("A quantidade deve ser maior que zero.");

                try
                {
                    var sucesso = await _estoqueService.ReabastecerEstoqueAsync(produtoId, quantidade);
                    if (!sucesso) return NotFound("Produto não encontrado.");

                    return Ok("Estoque reabastecido com sucesso.");
                }
                catch (Exception ex)
                {
                    PiorEcommerceDoMundo.Utils.Log($"Erro técnico ao reabastecer estoque: {ex.Message}");
                    return StatusCode(500, "Não foi possível concluir a operação de reabastecimento.");
                }
            }
        }
    }
