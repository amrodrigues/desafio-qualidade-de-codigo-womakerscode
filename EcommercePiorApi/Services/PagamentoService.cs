namespace Ecommerce.Services
{
    public class PagamentoService : IPagamentoService
    {
        public async Task<bool> ProcessarPagamentoAsync(decimal valor, string formaPagamento)
        {
            Console.WriteLine($"Processando pagamento de {valor} via {formaPagamento} de forma assíncrona...");
            
            //CORREÇÃO: Libera a thread do servidor, aguardando os 500ms
            await Task.Delay(500); 

            var random = new Random();
            var sorteio = random.Next(0, 100);

            if (formaPagamento == "cartao" && sorteio < 10)
            {
                return false;
            }
            return true;
        }
    }
}
