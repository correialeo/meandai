-- =============================================
-- MeandAI Database Schema
-- =============================================
-- Script de criação do banco de dados para o projeto MeandAI
-- GS - DevOps Tools & Cloud Computing

-- Criar banco de dados (se não existir)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'meandai_db')
BEGIN
    CREATE DATABASE meandai_db;
END
GO

USE meandai_db;
GO

-- =============================================
-- TABELAS PRINCIPAIS
-- =============================================

-- Tabela de Usuários
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    CurrentRole NVARCHAR(100) NOT NULL,
    DesiredArea NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Tabela de Habilidades
CREATE TABLE Skills (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Tabela de Trilhas de Aprendizado
CREATE TABLE LearningPaths (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    TargetArea NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- Tabela de Passos das Trilhas de Aprendizado
CREATE TABLE LearningPathSteps (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    LearningPathId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    ExternalUrl NVARCHAR(500) NULL,
    [Order] INT NOT NULL,
    EstimatedHours INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (LearningPathId) REFERENCES LearningPaths(Id) ON DELETE CASCADE
);
GO

-- Tabela de Habilidades dos Usuários (Relacionamento N:N)
CREATE TABLE UserSkills (
    UserId UNIQUEIDENTIFIER NOT NULL,
    SkillId UNIQUEIDENTIFIER NOT NULL,
    Level INT NOT NULL CHECK (Level BETWEEN 1 AND 10),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY (UserId, SkillId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (SkillId) REFERENCES Skills(Id) ON DELETE CASCADE
);
GO

-- Tabela de Trilhas dos Usuários (Relacionamento N:N)
CREATE TABLE UserLearningPaths (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    LearningPathId UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'NotStarted' CHECK (Status IN ('NotStarted', 'InProgress', 'Completed', 'Paused')),
    CurrentStep INT NOT NULL DEFAULT 0,
    StartedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (LearningPathId) REFERENCES LearningPaths(Id) ON DELETE CASCADE,
    UNIQUE (UserId, LearningPathId)
);
GO

-- =============================================
-- ÍNDICES PARA MELHORAR PERFORMANCE
-- =============================================

CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Skills_Category ON Skills(Category);
CREATE INDEX IX_LearningPaths_TargetArea ON LearningPaths(TargetArea);
CREATE INDEX IX_LearningPathSteps_LearningPathId_Order ON LearningPathSteps(LearningPathId, [Order]);
CREATE INDEX IX_UserSkills_SkillId_Level ON UserSkills(SkillId, Level);
CREATE INDEX IX_UserLearningPaths_Status ON UserLearningPaths(Status);
GO

-- =============================================
-- TRIGGERS PARA ATUALIZAR UpdatedAt
-- =============================================

CREATE TRIGGER TR_Users_UpdatedAt
ON Users
AFTER UPDATE
AS
BEGIN
    UPDATE Users
    SET UpdatedAt = GETUTCDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

CREATE TRIGGER TR_Skills_UpdatedAt
ON Skills
AFTER UPDATE
AS
BEGIN
    UPDATE Skills
    SET UpdatedAt = GETUTCDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

CREATE TRIGGER TR_LearningPaths_UpdatedAt
ON LearningPaths
AFTER UPDATE
AS
BEGIN
    UPDATE LearningPaths
    SET UpdatedAt = GETUTCDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

CREATE TRIGGER TR_LearningPathSteps_UpdatedAt
ON LearningPathSteps
AFTER UPDATE
AS
BEGIN
    UPDATE LearningPathSteps
    SET UpdatedAt = GETUTCDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

CREATE TRIGGER TR_UserLearningPaths_UpdatedAt
ON UserLearningPaths
AFTER UPDATE
AS
BEGIN
    UPDATE UserLearningPaths
    SET UpdatedAt = GETUTCDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
GO

-- =============================================
-- DADOS INICIAIS (SEED DATA)
-- =============================================

-- Inserir habilidades básicas
INSERT INTO Skills (Name, Category, Description) VALUES
('C#', 'Programming', 'Linguagem C# para desenvolvimento .NET'),
('JavaScript', 'Programming', 'Linguagem JavaScript para desenvolvimento web'),
('SQL Server', 'Database', 'Banco de dados Microsoft SQL Server'),
('Docker', 'DevOps', 'Containerização com Docker'),
('Azure', 'Cloud', 'Plataforma de nuvem Microsoft Azure'),
('React', 'Frontend', 'Biblioteca JavaScript para interfaces'),
('Python', 'Programming', 'Linguagem Python para Data Science'),
('Git', 'DevOps', 'Controle de versão com Git'),
('API REST', 'Architecture', 'Desenvolvimento de APIs RESTful'),
('Machine Learning', 'Data Science', 'Algoritmos de Machine Learning');
GO

-- Inserir trilhas de aprendizado exemplo
INSERT INTO LearningPaths (Name, TargetArea, Description) VALUES
('Desenvolvedor .NET Full Stack', 'Desenvolvimento', 'Trilha completa para desenvolvedor .NET'),
('Engenheiro de DevOps', 'DevOps', 'Especialização em práticas DevOps'),
('Cientista de Dados', 'Data Science', 'Carreira em Data Science e Machine Learning');
GO

-- Inserir passos para as trilhas
DECLARE @dotnetId UNIQUEIDENTIFIER, @devopsId UNIQUEIDENTIFIER, @datascienceId UNIQUEIDENTIFIER;
SELECT @dotnetId = Id FROM LearningPaths WHERE Name = 'Desenvolvedor .NET Full Stack';
SELECT @devopsId = Id FROM LearningPaths WHERE Name = 'Engenheiro de DevOps';
SELECT @datascienceId = Id FROM LearningPaths WHERE Name = 'Cientista de Dados';

-- Trilha .NET
INSERT INTO LearningPathSteps (LearningPathId, Title, Description, ExternalUrl, [Order], EstimatedHours) VALUES
(@dotnetId, 'Fundamentos C#', 'Aprenda os conceitos básicos da linguagem C#', NULL, 1, 40),
(@dotnetId, 'ASP.NET Core', 'Desenvolvimento web com ASP.NET Core', NULL, 2, 60),
(@dotnetId, 'Entity Framework Core', 'Trabalhando com bancos de dados usando EF Core', NULL, 3, 30),
(@dotnetId, 'React Frontend', 'Desenvolvendo interfaces com React', NULL, 4, 50),
(@dotnetId, 'Azure Deployment', 'Deploy de aplicações .NET no Azure', NULL, 5, 20);

-- Trilha DevOps
INSERT INTO LearningPathSteps (LearningPathId, Title, Description, ExternalUrl, [Order], EstimatedHours) VALUES
(@devopsId, 'Git e Controle de Versão', 'Dominando Git para controle de versão', NULL, 1, 20),
(@devopsId, 'Docker Fundamentos', 'Containerização com Docker', NULL, 2, 30),
(@devopsId, 'CI/CD Pipelines', 'Criando pipelines de integração e deploy', NULL, 3, 40),
(@devopsId, 'Azure DevOps', 'Ferramentas DevOps no Azure', NULL, 4, 35),
(@devopsId, 'Infraestrutura como Código', 'IaC com ARM Templates e Bicep', NULL, 5, 45);

-- Trilha Data Science
INSERT INTO LearningPathSteps (LearningPathId, Title, Description, ExternalUrl, [Order], EstimatedHours) VALUES
(@datascienceId, 'Python para Data Science', 'Fundamentos de Python para análise de dados', NULL, 1, 50),
(@datascienceId, 'Estatística Básica', 'Conceitos estatísticos para Data Science', NULL, 2, 40),
(@datascienceId, 'Machine Learning', 'Algoritmos de ML com scikit-learn', NULL, 3, 60),
(@datascienceId, 'Deep Learning', 'Redes neurais com TensorFlow/Keras', NULL, 4, 70),
(@datascienceId, 'ML em Produção', 'Deploy de modelos de ML em produção', NULL, 5, 30);
GO

-- =============================================
-- VIEWS PARA CONSULTAS ÚTEIS
-- =============================================

-- View de usuários com suas habilidades
CREATE VIEW VW_UserSkills AS
SELECT 
    u.Id as UserId,
    u.Name as UserName,
    u.Email,
    s.Name as SkillName,
    s.Category,
    us.Level,
    us.UpdatedAt
FROM Users u
INNER JOIN UserSkills us ON u.Id = us.UserId
INNER JOIN Skills s ON us.SkillId = s.Id;
GO

-- View de progresso das trilhas
CREATE VIEW VW_UserLearningPathProgress AS
SELECT 
    u.Id as UserId,
    u.Name as UserName,
    u.Email,
    lp.Name as LearningPathName,
    lp.TargetArea,
    ulp.Status,
    ulp.CurrentStep,
    COUNT(lps.Id) as TotalSteps,
    ulp.StartedAt,
    ulp.CompletedAt
FROM Users u
INNER JOIN UserLearningPaths ulp ON u.Id = ulp.UserId
INNER JOIN LearningPaths lp ON ulp.LearningPathId = lp.Id
LEFT JOIN LearningPathSteps lps ON lp.Id = lps.LearningPathId
GROUP BY u.Id, u.Name, u.Email, lp.Name, lp.TargetArea, ulp.Status, ulp.CurrentStep, ulp.StartedAt, ulp.CompletedAt;
GO

-- =============================================
-- STORED PROCEDURES ÚTEIS
-- =============================================

-- Procedure para buscar usuários por área desejada
CREATE PROCEDURE SP_GetUsersByDesiredArea
    @DesiredArea NVARCHAR(100)
AS
BEGIN
    SELECT Id, Name, Email, CurrentRole, DesiredArea, CreatedAt
    FROM Users
    WHERE DesiredArea = @DesiredArea
    ORDER BY CreatedAt DESC;
END;
GO

-- Procedure para buscar habilidades por categoria
CREATE PROCEDURE SP_GetSkillsByCategory
    @Category NVARCHAR(50)
AS
BEGIN
    SELECT Id, Name, Category, Description, CreatedAt
    FROM Skills
    WHERE Category = @Category
    ORDER BY Name;
END;
GO

-- Procedure para atualizar progresso do usuário
CREATE PROCEDURE SP_UpdateUserLearningPathProgress
    @UserId UNIQUEIDENTIFIER,
    @LearningPathId UNIQUEIDENTIFIER,
    @NewStatus NVARCHAR(20),
    @CurrentStep INT = NULL
AS
BEGIN
    UPDATE UserLearningPaths
    SET 
        Status = @NewStatus,
        CurrentStep = ISNULL(@CurrentStep, CurrentStep),
        UpdatedAt = GETUTCDATE(),
        StartedAt = CASE WHEN @NewStatus = 'InProgress' AND StartedAt IS NULL THEN GETUTCDATE() ELSE StartedAt END,
        CompletedAt = CASE WHEN @NewStatus = 'Completed' AND CompletedAt IS NULL THEN GETUTCDATE() ELSE CompletedAt END
    WHERE UserId = @UserId AND LearningPathId = @LearningPathId;
END;
GO

PRINT '=============================================';
PRINT 'MeandAI Database Schema criado com sucesso!';
PRINT '=============================================';
PRINT 'Tabelas criadas: 6';
PRINT 'Índices criados: 6';
PRINT 'Triggers criados: 5';
PRINT 'Views criadas: 2';
PRINT 'Stored Procedures criadas: 3';
PRINT 'Dados de exemplo inseridos: Habilidades, Trilhas e Passos';
PRINT '=============================================';
