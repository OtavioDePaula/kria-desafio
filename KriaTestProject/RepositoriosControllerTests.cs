using kria_desafio.Controllers;
using kria_desafio.Data;
using kria_desafio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace KriaTestProject
{
    [TestClass]
    public class RepositoriosControllerTests
    {
        private static ApplicationDbContext _context;
        private static RepositoriosController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            if (_context == null && _controller == null)
            {
                _context = new ApplicationDbContext(options);
                SeedData.Initialize(_context);
                _controller = new RepositoriosController(_context);
            }
        }

        //Index
        [TestMethod]
        public async Task Index_ReturnsViewWithRepositories_WhenSearchTermIsNull()
        {
            // Arrange
            var searchTerm = (string)null;

            // Act
            var result = await _controller.Index(searchTerm) as ViewResult;
            var model = result.Model as List<Repositorio>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, model.Count);
        }

        [TestMethod]
        public async Task Index_ReturnsViewWithFilteredRepositories_WhenSearchTermIsProvided()
        {
            // Arrange
            var searchTerm = "Repository";

            // Act
            var result = await _controller.Index(searchTerm) as ViewResult;
            var model = result.Model as List<Repositorio>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, model.Count);
        }

        [TestMethod]
        public async Task Index_ReturnsErrorMessage_WhenNoRepositoriesFound()
        {
            // Arrange
            var searchTerm = "NonExistent";

            // Act
            var result = await _controller.Index(searchTerm) as ViewResult;
            var model = result.Model as List<Repositorio>;
            var errorMessage = result.ViewData["ErrorMessage"];

            // Assert
            Assert.AreEqual("Nenhum resultado encontrado para 'NonExistent'.", errorMessage);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, model.Count);
        }

        //Details
        [TestMethod]
        public async Task Details_ReturnsViewWithRepository_WhenIdIsValid()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.Details(id) as ViewResult;
            var model = result.Model as Repositorio;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public async Task Details_ReturnsErrorView_WhenIdIsNull()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Details(id) as ViewResult;
            var errorMessage = result.ViewData["ErrorMessage"];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ID do repositório não especificado.", errorMessage);
        }

        [TestMethod]
        public async Task Details_ReturnsErrorView_WhenRepositoryNotFound()
        {
            // Arrange
            var id = 999;

            // Act
            var result = await _controller.Details(id) as ViewResult;
            var errorMessage = result.ViewData["ErrorMessage"];

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Repositório não encontrado.", errorMessage);
        }

        //Create

        [TestMethod]
        public void Create_Get_ReturnsViewWithSelectLists()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(null, result.ViewName);
            Assert.IsNotNull(result.ViewData["DonoId"]);
            Assert.IsNotNull(result.ViewData["LinguagemId"]);
        }

        [Fact]
        [TestMethod]
        public async Task Create_Post_ReturnsRedirectToIndexWithMessage_WhenModelStateIsValid()
        {
            // Arrange
            var mockTempData = new Mock<ITempDataDictionary>();
            _controller.TempData = mockTempData.Object; 

            var repositorio = new Repositorio
            {
                Nome = "Novo Repositório",
                DataUltimaAtualizacao = DateTime.Now,
                Descricao = "Descrição do novo repositório",
                DonoId = 1, 
                LinguagemId = 1 
            };

            // Act
            var result = await _controller.Create(repositorio, "Dono1", "C#") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName); 
            Assert.IsTrue(_context.Repositorio.Any(r => r.Nome == "Novo Repositório")); 

            mockTempData.VerifySet(td => td["SucessfullyMessage"] = "Repositório criado com sucesso!", Times.Once);
        }
    }

    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            var donos = new List<DonoRepositorio>
            {
                new DonoRepositorio { Nome = "Dono1" },
                new DonoRepositorio { Nome = "Dono2" },
                new DonoRepositorio { Nome = "Dono3" }
            };

            var linguagens = new List<Linguagem>
            {
                new Linguagem { Nome = "C#" },
                new Linguagem { Nome = "Java" },
                new Linguagem { Nome = "Python" }
            };

            var repositorios = new List<Repositorio>
            {
                new Repositorio { Id = 1, Nome = "Repository1", DataUltimaAtualizacao = DateTime.Now, Descricao = "Descrição do Repo1", DonoId = 1, LinguagemId = 1 },
                new Repositorio { Id = 2, Nome = "Repo2", DataUltimaAtualizacao = DateTime.Now, Descricao = "Descrição do Repo2", DonoId = 2, LinguagemId = 2 },
                new Repositorio { Id = 3, Nome = "Repository3", DataUltimaAtualizacao = DateTime.Now, Descricao = "Descrição do Repo3", DonoId = 3, LinguagemId = 3 }
            };

            context.DonoRepositorio.AddRange(donos);
            context.Linguagem.AddRange(linguagens);
            context.Repositorio.AddRange(repositorios);
            context.SaveChanges();
        }
    }
}
