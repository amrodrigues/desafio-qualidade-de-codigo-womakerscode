
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
        private readonly AppDbContext _db;

        public PedidosController(IPedidoService pedidoService, AppDbContext db)
        {
            _pedidoService = pedidoService;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPedidos() =>
            Ok(await _db.Pedidos.Include(p => p.Itens).ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPedido(int id)
        {
            var pedido = await _db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
            return pedido == null ? NotFound() : Ok(pedido);
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
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
        }
    }
}
