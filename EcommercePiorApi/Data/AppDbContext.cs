using Ecommerce.Model; // Ajuste para o namespace correto do seu projeto
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
    public class AppDbContext : DbContext
    {
        // 1. Adicione este construtor para aceitar as opções injetadas pelo Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 2. Mantenha o construtor sem parâmetros (se necessário para instâncias manuais antigas)
        public AppDbContext()
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Se o optionsBuilder já foi configurado externamente, não precisa forçar aqui,
            // mas se não foi, você pode manter uma fallback ou deixar vazio se o Program.cs já configura.
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=ecommerce.db");
            }
        }
    }
}