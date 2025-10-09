using Coin_up.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coin_up.Data
{
    public class CoinUpDbContext : DbContext
    {
        public CoinUpDbContext(DbContextOptions<CoinUpDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Conta> Contas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configurações da Entidade Usuario ---
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.FirebaseUid).IsUnique();

                entity.HasMany(u => u.Contas)
              .WithOne(c => c.Usuario)
              .HasForeignKey(c => c.UserId)
              .OnDelete(DeleteBehavior.Cascade);

                // Relação: UM Usuário tem MUITAS Quests
                entity.HasMany(u => u.Quests)
                      .WithOne(q => q.Usuario)
                      .HasForeignKey(q => q.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relação: UM Usuário tem MUITAS Transações
                entity.HasMany(u => u.Transacoes)
                      .WithOne(t => t.Usuario)
                      .HasForeignKey(t => t.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Configurações da Entidade Conta ---
            modelBuilder.Entity<Conta>(entity =>
            {
                // Relação: UMA Conta tem MUITAS Transações
                entity.HasMany(c => c.Transacoes)
                      .WithOne(t => t.Conta)
                      .HasForeignKey(t => t.ContaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
