namespace Ecommerce.DTO;

// requests da API misturados no mesmo arquivo dos models, sem pasta de DTOs
public class PedidoRequest
{
    public string ClienteNome { get; set; }
    public string ClienteEmail { get; set; }
    public string FormaPagamento { get; set; }
    public List<ItemRequest> Itens { get; set; }
}
