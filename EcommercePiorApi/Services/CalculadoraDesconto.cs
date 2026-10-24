namespace Ecommerce.Services
{
    public class CalculadoraDesconto : ICalculadoraDesconto
    {
        public decimal Aplicar(decimal total, string clienteEmail)
        {
            if (total > 500m)
            {
                return total * 0.90m;
            }

            if (clienteEmail.Contains("vip", StringComparison.OrdinalIgnoreCase))
            {
                return total * 0.80m;
            }

            return total;
        }
    }
}
