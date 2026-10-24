namespace Ecommerce.Services
{
    public interface ICalculadoraDesconto
    {
        decimal Aplicar(decimal total, string clienteEmail);
    }
}
