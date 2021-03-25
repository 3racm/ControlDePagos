
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/25/2021 12:50:06
-- Generated from EDMX file: C:\Users\3R Server\source\repos\ControlDePagos\ControlPagosModel\ControlGastosModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ControlDeGastos];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Tb_ProyectosTb_Pagos]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tb_Pagos] DROP CONSTRAINT [FK_Tb_ProyectosTb_Pagos];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Tb_Pagos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tb_Pagos];
GO
IF OBJECT_ID(N'[dbo].[Tb_Proyectos]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tb_Proyectos];
GO
IF OBJECT_ID(N'[dbo].[Tb_Usuario]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tb_Usuario];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Tb_Proyectos'
CREATE TABLE [dbo].[Tb_Proyectos] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Num_Proyecto_Cuenta] nvarchar(max)  NOT NULL,
    [FechaInicio] datetime  NOT NULL,
    [MontoInicial] decimal(18,2)  NOT NULL,
    [Moneda] nvarchar(50)  NOT NULL,
    [MontoFinal] decimal(18,2)  NULL,
    [Retorno] decimal(18,2)  NULL,
    [Descripcion] nvarchar(max)  NOT NULL,
    [Estado] nvarchar(30)  NULL
);
GO

-- Creating table 'Tb_Pagos'
CREATE TABLE [dbo].[Tb_Pagos] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FechaPago] datetime  NOT NULL,
    [Monto] decimal(18,2)  NOT NULL,
    [Referencia] nvarchar(max)  NULL,
    [TipoPago] nvarchar(50)  NOT NULL,
    [Retorno] decimal(18,2)  NULL,
    [Monto2] decimal(18,2)  NULL,
    [TipoPago2] nvarchar(50)  NULL,
    [Monto3] decimal(18,2)  NULL,
    [TipoPago3] nvarchar(50)  NULL,
    [Notas] nvarchar(max)  NULL,
    [RegistradoPor] nvarchar(max)  NULL,
    [Tb_Proyectos_Id] int  NOT NULL
);
GO

-- Creating table 'Tb_Usuario'
CREATE TABLE [dbo].[Tb_Usuario] (
    [Id_usuario] int IDENTITY(1,1) NOT NULL,
    [Nombre] nvarchar(max)  NOT NULL,
    [CorreoElectronico] nvarchar(max)  NOT NULL,
    [Contraseña] nvarchar(50)  NOT NULL,
    [Permiso] nvarchar(50)  NOT NULL,
    [FechaRegistro] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Tb_Proyectos'
ALTER TABLE [dbo].[Tb_Proyectos]
ADD CONSTRAINT [PK_Tb_Proyectos]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tb_Pagos'
ALTER TABLE [dbo].[Tb_Pagos]
ADD CONSTRAINT [PK_Tb_Pagos]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id_usuario] in table 'Tb_Usuario'
ALTER TABLE [dbo].[Tb_Usuario]
ADD CONSTRAINT [PK_Tb_Usuario]
    PRIMARY KEY CLUSTERED ([Id_usuario] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Tb_Proyectos_Id] in table 'Tb_Pagos'
ALTER TABLE [dbo].[Tb_Pagos]
ADD CONSTRAINT [FK_Tb_ProyectosTb_Pagos]
    FOREIGN KEY ([Tb_Proyectos_Id])
    REFERENCES [dbo].[Tb_Proyectos]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tb_ProyectosTb_Pagos'
CREATE INDEX [IX_FK_Tb_ProyectosTb_Pagos]
ON [dbo].[Tb_Pagos]
    ([Tb_Proyectos_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------