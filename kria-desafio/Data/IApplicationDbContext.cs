using kria_desafio.Models;
using Microsoft.EntityFrameworkCore;

namespace kria_desafio.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Repositorio> Repositorio { get; set; }
        DbSet<Linguagem> Linguagem { get; set; }
        DbSet<DonoRepositorio> DonoRepositorio { get; set; }
        DbSet<Favorito> Favorito { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
