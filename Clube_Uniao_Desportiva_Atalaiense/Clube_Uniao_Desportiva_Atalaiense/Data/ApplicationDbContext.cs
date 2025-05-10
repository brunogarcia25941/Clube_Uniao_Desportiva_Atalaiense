using Microsoft.EntityFrameworkCore;

namespace Clube_Uniao_Desportiva_Atalaiense.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Equipa> Equipas { get; set; }
        public DbSet<Jogador> Jogadores { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<JogadorJogo> JogadoresJogos { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Loja> Loja { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorito>()
                .HasKey(f => new { f.UtilizadorUsername, f.EquipaId });

            // Chave composta
            modelBuilder.Entity<JogadorJogo>()
                .HasKey(jj => new { jj.JogadorId, jj.JogoId });

            // Relação Jogador → JogadorJogo (cascade delete)
            modelBuilder.Entity<JogadorJogo>()
                .HasOne(jj => jj.Jogador)
                .WithMany(j => j.JogadoresJogos)
                .HasForeignKey(jj => jj.JogadorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação Jogo → JogadorJogo (Restrict)
            modelBuilder.Entity<JogadorJogo>()
                .HasOne(jj => jj.Jogo)
                .WithMany(j => j.JogadoresJogos)
                .HasForeignKey(jj => jj.JogoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do Preco em Loja
            modelBuilder.Entity<Loja>()
                .Property(l => l.Preco)
                .HasColumnType("decimal(18,2)");
        }
    }
}
