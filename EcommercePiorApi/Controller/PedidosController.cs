
using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPedidos()
        {
            try
            {
                var pedidos = await _pedidoService.ListarPedidosAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                PiorEcommerceDoMundo.Utils.Log($"Erro técnico ao listar pedidos: {ex.Message}");
                return StatusCode(500, "Não foi possível carregar os pedidos no momento.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPedido(int id)
        {
            try
            {
                var pedido = await _pedidoService.BuscarPedidoPorIdAsync(id);
                return pedido == null ? NotFound() : Ok(pedido);
            }
            catch (Exception ex)
            {
                PiorEcommerceDoMundo.Utils.Log($"Erro técnico ao buscar pedido {id}: {ex.Message}");
                return StatusCode(500, "Não foi possível carregar as informações do pedido.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoRequest request)
        {
            try
            {
                var pedido = await _pedidoService.CriarPedidoAsync(request);
                return Ok(pedido);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                PiorEcommerceDoMundo.Utils.Log($"Erro inesperado ao criar pedido: {ex.Message}");
                return StatusCode(500, "Ocorreu um erro interno. Tente novamente mais tarde.");
            }
        }

        [HttpPost("pagamentos/reprocessar/{pedidoId}")]
        public async Task<IActionResult> ReprocessarPagamento(int pedidoId)
        {
            try
            {
                var pedido = await _pedidoService.ReprocessarPagamentoAsync(pedidoId);
                return Ok(pedido);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                PiorEcommerceDoMundo.Utils.Log($"Erro técnico ao reprocessar pagamento do pedido {pedidoId}: {ex.Message}");
                return StatusCode(500, "Não foi possível reprocessar o pagamento no momento.");
            }
        }

        [HttpPost("logistica/reenviar/{pedidoId}")]
        public async Task<IActionResult> ReenviarLogistica(int pedidoId)
        {
            try
            {
                var pedido = await _pedidoService.ReenviarLogisticaAsync(pedidoId);
                return Ok(pedido);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                PiorEcommerceDoMundo.Utils.Log($"Erro técnico ao reenviar logística do pedido {pedidoId}: {ex.Message}");
                return StatusCode(500, "Não foi possível reenviar para a logística no momento.");
            }
        }
    }
}