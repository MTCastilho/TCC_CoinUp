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
                // Garante que não haverá dois usuários com o mesmo Firebase Uid
                entity.HasIndex(u => u.FirebaseUid).IsUnique();

                // Relação: UM Usuário tem MUITAS Contas
                entity.HasMany(u => u.Contas)
                      .WithOne(c => c.Usuario)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Apagar usuário apaga suas contas

                // Relação: UM Usuário tem MUITAS Quests
                entity.HasMany(u => u.Quests)
                      .WithOne(q => q.Usuario)
                      .HasForeignKey(q => q.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Apagar usuário apaga suas quests

                // Relação: UM Usuário tem MUITAS Transações
                entity.HasMany(u => u.Transacoes)
                      .WithOne(t => t.Usuario)
                      .HasForeignKey(t => t.UsuarioId)
                      // CORREÇÃO CRÍTICA: Alterado para Restrict para evitar conflito de cascade path
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Configurações da Entidade Conta ---
            modelBuilder.Entity<Conta>(entity =>
            {
                // MELHORIA: Define a precisão para campos monetários
                entity.Property(c => c.SaldoAtual)
                      .HasColumnType("decimal(18, 2)");

                // Relação: UMA Conta tem MUITAS Transações
                entity.HasMany(c => c.Transacoes)
                      .WithOne(t => t.Conta)
                      .HasForeignKey(t => t.ContaId)
                      // CORRETO: Impede que uma conta com histórico seja apagada
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Configurações da Entidade Transacao ---
            modelBuilder.Entity<Transacao>(entity =>
            {
                // MELHORIA: Define a precisão para campos monetários
                entity.Property(t => t.Valor)
                      .HasColumnType("decimal(18, 2)");
            });

            // --- Configurações da Entidade Quest ---
            modelBuilder.Entity<Quest>(entity =>
            {
                // MELHORIA: Define a precisão para campos monetários
                entity.Property(q => q.ValorAlvo)
                      .HasColumnType("decimal(18, 2)");
            });
        }
    }
}
