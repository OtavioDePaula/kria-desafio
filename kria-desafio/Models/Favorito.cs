namespace kria_desafio.Models
{
    public class Favorito
    {
        public int Id { get; set; }
        public int RepositorioId { get; set; }
        public Repositorio Repositorio { get; set; }
    }
}
