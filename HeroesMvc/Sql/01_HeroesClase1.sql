-- Base HeroesClase1: tablas de la clase 2, Clase 02 (Ciudades/Villanos) y ejercicio (Cuarteles/Vehiculos).
-- Se ejecuta una sola vez. No se usan migraciones.
IF DB_ID(N'HeroesClase1') IS NULL CREATE DATABASE HeroesClase1;
GO
USE HeroesClase1;
GO

/* ===== Clase 2: Equipo, Heroe, Ficha, Superpoder, HeroeSuperpoder ===== */
CREATE TABLE dbo.Equipos (
    Id     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Equipos_Nombre UNIQUE (Nombre)
);
GO
CREATE TABLE dbo.Heroes (
    Id       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre   NVARCHAR(100) NOT NULL,
    Ciudad   NVARCHAR(100) NOT NULL,
    EquipoId INT NULL,
    CONSTRAINT FK_Heroes_Equipos FOREIGN KEY (EquipoId) REFERENCES dbo.Equipos(Id) ON DELETE SET NULL
);
GO
CREATE TABLE dbo.Fichas (
    HeroeId          INT NOT NULL PRIMARY KEY,
    IdentidadSecreta NVARCHAR(100) NULL,
    Debilidad        NVARCHAR(150) NULL,
    CONSTRAINT FK_Fichas_Heroes FOREIGN KEY (HeroeId) REFERENCES dbo.Heroes(Id) ON DELETE CASCADE
);
GO
CREATE TABLE dbo.Superpoderes (
    Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL
);
GO
CREATE TABLE dbo.HeroeSuperpoder (
    HeroeId       INT NOT NULL,
    SuperpoderId  INT NOT NULL,
    CONSTRAINT PK_HeroeSuperpoder PRIMARY KEY (HeroeId, SuperpoderId),
    CONSTRAINT FK_HeroeSuperpoder_Heroes       FOREIGN KEY (HeroeId)      REFERENCES dbo.Heroes(Id)       ON DELETE CASCADE,
    CONSTRAINT FK_HeroeSuperpoder_Superpoderes FOREIGN KEY (SuperpoderId) REFERENCES dbo.Superpoderes(Id) ON DELETE CASCADE
);
GO

/* ===== Clase 02: Ciudades y Villanos ===== */
CREATE TABLE dbo.Ciudades (
    Id     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Pais   NVARCHAR(60)  NOT NULL,
    CONSTRAINT UQ_Ciudades_Nombre UNIQUE (Nombre)
);
GO
CREATE TABLE dbo.Villanos (
    Id             INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Alias          NVARCHAR(100) NOT NULL,
    Nivel_Amenaza  INT NOT NULL,
    Recompensa     DECIMAL(10,2) NOT NULL,
    FechaRegistro  DATETIME2 NOT NULL CONSTRAINT DF_Villanos_FechaRegistro DEFAULT (GETDATE()),
    CiudadId       INT NOT NULL,
    CONSTRAINT CK_Villanos_NivelAmenaza CHECK (Nivel_Amenaza BETWEEN 1 AND 5),
    CONSTRAINT FK_Villanos_Ciudades FOREIGN KEY (CiudadId) REFERENCES dbo.Ciudades(Id) ON DELETE NO ACTION
);
GO

/* ===== Ejercicio: Cuarteles y Vehiculos ===== */
CREATE TABLE dbo.Cuarteles (
    Id        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre    NVARCHAR(80)  NOT NULL,
    Direccion NVARCHAR(150) NOT NULL,
    Capacidad INT NOT NULL,
    CONSTRAINT UQ_Cuarteles_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Cuarteles_Capacidad CHECK (Capacidad > 0)
);
GO
CREATE TABLE dbo.Vehiculos (
    Id                 INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nro_Placa          NVARCHAR(10) NOT NULL,
    Modelo             NVARCHAR(60) NOT NULL,
    CostoMantenimiento DECIMAL(12,2) NOT NULL,
    FechaAlta          DATETIME2 NOT NULL CONSTRAINT DF_Vehiculos_FechaAlta DEFAULT (GETDATE()),
    CuartelId          INT NULL,
    CONSTRAINT UQ_Vehiculos_Placa UNIQUE (Nro_Placa),
    CONSTRAINT FK_Vehiculos_Cuarteles FOREIGN KEY (CuartelId) REFERENCES dbo.Cuarteles(Id) ON DELETE SET NULL
);
GO

/* ===== Datos de ejemplo ===== */
INSERT INTO dbo.Equipos (Nombre) VALUES (N'Liga de la Justicia');
INSERT INTO dbo.Heroes (Nombre, Ciudad, EquipoId) VALUES
    (N'Superman', N'Metrópolis', 1), (N'Batman', N'Gotham', 1);
INSERT INTO dbo.Fichas (HeroeId, IdentidadSecreta, Debilidad) VALUES
    (1, N'Clark Kent', N'Kryptonita'), (2, N'Bruce Wayne', N'Sin superpoderes');
INSERT INTO dbo.Superpoderes (Nombre, Descripcion) VALUES
    (N'Volar', N'Puede desplazarse por el aire.'),
    (N'Superfuerza', N'Tiene una fuerza extraordinaria.'),
    (N'Inteligencia estratégica', N'Planifica y analiza situaciones complejas.');
INSERT INTO dbo.HeroeSuperpoder (HeroeId, SuperpoderId) VALUES (1,1),(1,2),(2,3);

INSERT INTO dbo.Ciudades (Nombre, Pais) VALUES (N'La Paz', N'Bolivia'), (N'Santa Cruz', N'Bolivia');
DECLARE @LaPaz INT = (SELECT Id FROM dbo.Ciudades WHERE Nombre = N'La Paz');
INSERT INTO dbo.Villanos (Alias, Nivel_Amenaza, Recompensa, CiudadId) VALUES
    (N'Sombra Roja', 4, 15000.50, @LaPaz),
    (N'Doctor Niebla', 3, 8200.00, @LaPaz);

INSERT INTO dbo.Cuarteles (Nombre, Direccion, Capacidad) VALUES
    (N'Base Illimani', N'Av. Arce 123, La Paz', 20),
    (N'Base Oriente', N'Calle Sucre 45, Santa Cruz', 12);
DECLARE @Illimani INT = (SELECT Id FROM dbo.Cuarteles WHERE Nombre = N'Base Illimani');
INSERT INTO dbo.Vehiculos (Nro_Placa, Modelo, CostoMantenimiento, CuartelId) VALUES
    (N'2345-ABC', N'Halcón X1', 8500.00, @Illimani),
    (N'9876-XYZ', N'Rayo T2', 4200.75, @Illimani),
    (N'1111-DEF', N'Tortuga M3', 1500.00, NULL);
GO
