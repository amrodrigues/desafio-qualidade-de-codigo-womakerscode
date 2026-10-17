using Ecommerce.Enum;

namespace Ecommerce.Model;

public class Pedido
{
    public int Id { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime DataPedido { get; set; }
    public StatusPedido Status { get; set; }
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; }
    public string CodigoRastreio { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
}
