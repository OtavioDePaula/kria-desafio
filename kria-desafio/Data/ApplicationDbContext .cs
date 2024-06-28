using kria_desafio.Models;
using Microsoft.EntityFrameworkCore;

namespace kria_desafio.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Repositorio> Repositorio { get; set; }
        public DbSet<Linguagem> Linguagem { get; set; }
        public DbSet<DonoRepositorio> DonoRepositorio { get; set; }
        public DbSet<Favorito> Favorito { get; set; }
    }

}
