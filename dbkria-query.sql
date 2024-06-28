CREATE DATABASE dbKriaRepositorios
COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

USE dbKriaRepositorios;

CREATE TABLE DonoRepositorio (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome VARCHAR(100) NOT NULL
);

CREATE TABLE Linguagem (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome VARCHAR(20) NOT NULL
);

CREATE TABLE Repositorio (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome VARCHAR(100) NOT NULL,
    DataUltimaAtualizacao DATETIME NOT NULL,
    Descricao VARCHAR(MAX) NOT NULL,
    DonoId INT NOT NULL,
    LinguagemId INT NOT NULL,
    FOREIGN KEY (DonoId) REFERENCES DonoRepositorio(Id),
    FOREIGN KEY (LinguagemId) REFERENCES Linguagem(Id)
);

CREATE TABLE Favorito (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RepositorioId INT NOT NULL,
    FOREIGN KEY (RepositorioId) REFERENCES Repositorio(Id)
);

GO

CREATE PROCEDURE DeletarRepositorio
    @Id INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Favorito WHERE RepositorioId = @Id)
        DELETE FROM Favorito WHERE RepositorioId = @Id;
        
    DELETE FROM Repositorio WHERE Id = @Id;
END;
GO

CREATE PROCEDURE DeletarTodosRepositorios
AS
BEGIN
    TRUNCATE TABLE Favorito;
    DELETE FROM Repositorio;
END;
GO
/*
INSERT INTO DonoRepositorio (Nome) VALUES
    ('Jota Silva'),
    ('Maria Santos'),
    ('Pedro Almeida'),
    ('Ana Souza'),
    ('Carlos Oliveira'),
    ('Mariana Costa'),
    ('Rafael Pereira'),
    ('Juliana Martins'),
    ('Fernando Rodrigues'),
    ('Camila Lima'),
    ('Gustavo Cardoso'),
    ('Patricia Vieira'),
    ('Lucas Ferreira'),
    ('Carolina Mendes'),
    ('Roberto Castro'),
    ('Amanda Pereira'),
    ('Daniel Oliveira'),
    ('Vanessa Santos'),
    ('Paulo Fernandes'),
    ('Cristina Alves');

INSERT INTO Linguagem (Nome) VALUES
    ('C#'),
    ('Java'),
    ('Python'),
    ('JavaScript'),
    ('C++'),
    ('Ruby'),
    ('Swift'),
    ('PHP'),
    ('Go'),
    ('Rust'),
    ('TypeScript'),
    ('Kotlin'),
    ('Perl'),
    ('Scala'),
    ('Haskell'),
    ('Objective-C'),
    ('Lua'),
    ('VB.NET'),
    ('Shell'),
    ('Delphi');
    
*/
