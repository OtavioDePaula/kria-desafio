namespace kria_desafio.Models
{
    public class Repositorio
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
        public string Descricao { get; set; }
        public int DonoId { get; set; }
        public DonoRepositorio Dono { get; set; }
        public int LinguagemId { get; set; }
        public Linguagem Linguagem { get; set; }
    }
}
