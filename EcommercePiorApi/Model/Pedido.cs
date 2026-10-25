using Ecommerce.Enum;
using System.ComponentModel.DataAnnotations.Schema;

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

    [NotMapped]
    public decimal ValorSubtotal =>
        Itens?.Sum(item => item.PrecoUnitario * item.Quantidade) ?? 0m;

    [NotMapped]
    public decimal ValorDesconto => Math.Max(0m, ValorSubtotal - ValorTotal);
}
