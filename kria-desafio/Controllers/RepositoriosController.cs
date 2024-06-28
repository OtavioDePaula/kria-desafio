using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kria_desafio.Data;
using kria_desafio.Models;

namespace kria_desafio.Controllers
{
    public class RepositoriosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RepositoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Repositorios
        public async Task<IActionResult> Index(string? search)
        {
            try
            {
                var repositorios = from r in _context.Repositorio.Include(r => r.Linguagem).Include(r => r.Dono)
                                   select r;

                if (!string.IsNullOrEmpty(search))
                {
                    repositorios = repositorios.Where(r => r.Nome.Contains(search) || r.Linguagem.Nome.Contains(search));
                }

                repositorios = repositorios.OrderByDescending(r => r.DataUltimaAtualizacao);

                var listaRepositorios = await repositorios.ToListAsync();

                if (listaRepositorios.Count == 0 && !string.IsNullOrEmpty(search))
                {
                    ViewData["ErrorMessage"] = $"Nenhum resultado encontrado para '{search}'.";
                }
                else if (listaRepositorios.Count == 0)
                {
                    ViewData["ErrorMessage"] = "Nenhum repositório cadastrado.";
                }

                return View(listaRepositorios);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro ao carregar a lista de repositórios: {ex.Message}";
                return View(new List<Repositorio>());
            }
        }


        // GET: Repositorios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ViewData["ErrorMessage"] = "ID do repositório não especificado.";
                return View(new Repositorio());
            }

            try
            {
                var repositorio = await _context.Repositorio
                    .Include(r => r.Dono)
                    .Include(r => r.Linguagem)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (repositorio == null)
                {
                    ViewData["ErrorMessage"] = "Repositório não encontrado.";
                    return View(new Repositorio());
                }

                return View(repositorio);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro ao carregar os detalhes do repositório: {ex.Message}";
                return View(new Repositorio());
            }
        }

        // GET: Repositorios/Create
        public IActionResult Create()
        {
            try
            {
                CarregarSelectLists();
                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro ao carregar a página de cadastro: {ex.Message}";
                return View();
            }
        }

        // POST: Repositorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,DataUltimaAtualizacao,Descricao,DonoId,LinguagemId")] Repositorio repositorio, string nomeDono, string nomeLinguagem)
        {
            try
            {
                repositorio.DonoId = await VerificarDonoAsync(nomeDono);
                repositorio.LinguagemId = await VerificarLinguagemAsync(nomeLinguagem);

                _context.Add(repositorio);
                await _context.SaveChangesAsync();
                TempData["SucessfullyMessage"] = "Repositório criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ViewData["ErrorMessage"] = "Erro ao salvar o repositório.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro inesperado: {ex.Message}";
            }

            CarregarSelectLists(repositorio);
            return View(repositorio);
        }

        // GET: Repositorios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ViewData["ErrorMessage"] = "Nenhum repositório com esse id foi encontrado.";
                return View();
            }

            try
            {
                var repositorio = await _context.Repositorio
                    .Include(r => r.Dono)
                    .Include(r => r.Linguagem)
                    .FirstOrDefaultAsync(r => r.Id == id);
                if (repositorio == null)
                {
                    ViewData["ErrorMessage"] = "Nenhum repositório com esse id foi encontrado.";
                }

                CarregarSelectLists(repositorio);
                return View(repositorio);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro ao carregar a página de edição: {ex.Message}";
                return View();
            }
        }

        // POST: Repositorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,DataUltimaAtualizacao,Descricao,DonoId,LinguagemId")] Repositorio repositorio)
        {
            if (id != repositorio.Id) return NotFound();

            try
            {
                _context.Update(repositorio);
                await _context.SaveChangesAsync();
                TempData["SucessfullyMessage"] = "Repositório atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepositorioExists(repositorio.Id)) return NotFound();

                throw;
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro ao atualizar o repositório: {ex.Message}";
            }

            CarregarSelectLists(repositorio);
            return View(repositorio);
        }

        // POST: Repositorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC DeletarRepositorio @Id = {id}");
                TempData["SucessfullyMessage"] = "Repositório excluido com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao excluir o repositório: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Repositorios/DeleteAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllRepositorios()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC DeletarTodosRepositorios");
                TempData["SucessfullyMessage"] = "Repositórios excluídos com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao excluir todos os repositórios: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Repositorios/Favoritos
        public IActionResult Favoritos()
        {
            try
            {
                List<Repositorio> favoritos = _context.Favorito
                    .Include(f => f.Repositorio)
                        .ThenInclude(r => r.Dono)
                    .Include(f => f.Repositorio)
                        .ThenInclude(r => r.Linguagem)
                    .Select(f => f.Repositorio)
                    .ToList();

                if (favoritos.Count == 0)
                {
                    ViewData["ErrorMessage"] = "Nenhum repositório favoritado encontrado.";
                }
                return View(favoritos);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro ao carregar os repositórios favoritos: {ex.Message}";
                return View(new List<Repositorio>());
            }
        }

        // POST: Repositorios/FavoritarRepositorio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavoritarRepositorio(int id)
        {
            try
            {
                var repositorio = await _context.Repositorio
                    .Include(r => r.Dono)
                    .Include(r => r.Linguagem)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (repositorio != null)
                {
                    var favoritoExistente = await _context.Favorito
                        .FirstOrDefaultAsync(f => f.RepositorioId == id);

                    if (favoritoExistente == null)
                    {
                        var novoFavorito = new Favorito { RepositorioId = id };
                        _context.Favorito.Add(novoFavorito);
                        await _context.SaveChangesAsync();
                    }
                }
                TempData["SucessfullyMessage"] = "Repositório adicionado aos favoritos com sucesso!";
                return RedirectToAction(nameof(Favoritos));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao favoritar o repositório: {ex.Message}";
                return RedirectToAction(nameof(Favoritos));
            }
        }

        // POST: Repositorios/RemoverFavorito/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverFavorito(int id)
        {
            try
            {
                var favorito = await _context.Favorito
                    .FirstOrDefaultAsync(f => f.RepositorioId == id);

                if (favorito != null)
                {
                    _context.Favorito.Remove(favorito);
                    await _context.SaveChangesAsync();
                }

                TempData["SucessfullyMessage"] = "Repositório removido dos favoritos com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao remover o favorito: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RepositorioExists(int id)
        {
            return _context.Repositorio.Any(e => e.Id == id);
        }

        // Método para buscar donos para autocompletar
        [HttpPost]
        public IActionResult BuscarDonos(string term)
        {
            try
            {
                var donos = _context.DonoRepositorio
                    .Where(d => d.Nome.Contains(term))
                    .Select(d => new { id = d.Id, nome = d.Nome })
                    .ToList();

                return Json(donos);
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Erro ao buscar donos: {ex.Message}" });
            }
        }

        // Método para buscar linguagens para autocompletar
        [HttpPost]
        public IActionResult BuscarLinguagens(string term)
        {
            try
            {
                var linguagens = _context.Linguagem
                    .Where(l => l.Nome.Contains(term))
                    .Select(l => new { id = l.Id, nome = l.Nome })
                    .ToList();

                return Json(linguagens);
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Erro ao buscar linguagens: {ex.Message}" });
            }
        }
        private async Task<int> VerificarDonoAsync(string nomeDono)
        {
            var donoExistente = await _context.DonoRepositorio.FirstOrDefaultAsync(d => d.Nome == nomeDono);

            if (donoExistente == null)
            {
                var novoDono = new DonoRepositorio { Nome = nomeDono };
                _context.DonoRepositorio.Add(novoDono);
                await _context.SaveChangesAsync();

                return novoDono.Id;
            }

            return donoExistente.Id;
        }

        private async Task<int> VerificarLinguagemAsync(string nomeLinguagem)
        {
            var linguagemExistente = await _context.Linguagem.FirstOrDefaultAsync(l => l.Nome == nomeLinguagem);

            if (linguagemExistente == null)
            {
                var novaLinguagem = new Linguagem { Nome = nomeLinguagem };
                _context.Linguagem.Add(novaLinguagem);
                await _context.SaveChangesAsync();

                return novaLinguagem.Id;
            }

            return linguagemExistente.Id;
        }

        private void CarregarSelectLists(Repositorio repositorio = null)
        {
            ViewData["DonoId"] = new SelectList(_context.DonoRepositorio, "Id", "Nome", repositorio?.DonoId);
            ViewData["LinguagemId"] = new SelectList(_context.Linguagem, "Id", "Nome", repositorio?.LinguagemId);
        }
    }
}
