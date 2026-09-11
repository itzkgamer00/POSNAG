/* =====================================================================
   Sistema de Caja - Esquema de seguridad (Roles / Usuario / Permiso)
   Idempotente: se puede ejecutar varias veces sin error.
   Ejecutar con SQLCMD o SSMS conectado a la instancia de SQL Server.
   ===================================================================== */

IF DB_ID('SistemaCaja') IS NULL
    CREATE DATABASE SistemaCaja;
GO

USE SistemaCaja;
GO

/* ---------- Tabla Roles ---------- */
IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Roles
    (
        IdRol         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        Descripcion   NVARCHAR(50)      NOT NULL,
        estado        BIT              NOT NULL CONSTRAINT DF_Roles_estado DEFAULT (1),
        FechaCreacion DATETIME2(0)      NOT NULL CONSTRAINT DF_Roles_fecha  DEFAULT (SYSDATETIME()),
        CONSTRAINT UQ_Roles_Descripcion UNIQUE (Descripcion)
    );
END
GO

/* ---------- Tabla Usuario ---------- */
IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario
    (
        usuario_id     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuario PRIMARY KEY,
        NombreCompleto NVARCHAR(100)     NOT NULL,
        usuario        NVARCHAR(50)      NOT NULL,
        password_hash  VARCHAR(200)      NOT NULL,
        IdRol          INT               NULL,
        estado         BIT               NOT NULL CONSTRAINT DF_Usuario_estado DEFAULT (1),
        fechacreacion  DATETIME2(0)      NOT NULL CONSTRAINT DF_Usuario_fecha  DEFAULT (SYSDATETIME()),
        CONSTRAINT UQ_Usuario_usuario UNIQUE (usuario)
    );
END
GO

/* ---------- Ajustes para bases ya existentes (agrega lo que falte) ---------- */
IF COL_LENGTH('dbo.Usuario', 'IdRol') IS NULL
    ALTER TABLE dbo.Usuario ADD IdRol INT NULL;
GO
IF COL_LENGTH('dbo.Usuario', 'estado') IS NULL
    ALTER TABLE dbo.Usuario ADD estado BIT NOT NULL CONSTRAINT DF_Usuario_estado DEFAULT (1);
GO
IF COL_LENGTH('dbo.Usuario', 'fechacreacion') IS NULL
    ALTER TABLE dbo.Usuario ADD fechacreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Usuario_fecha DEFAULT (SYSDATETIME());
GO

/* FK Usuario -> Roles (solo si no existe ya alguna FK sobre Usuario.IdRol) */
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys fk
    JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
    JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
    WHERE fk.parent_object_id = OBJECT_ID('dbo.Usuario') AND c.name = 'IdRol'
)
    ALTER TABLE dbo.Usuario
        ADD CONSTRAINT FK_Usuario_Roles FOREIGN KEY (IdRol) REFERENCES dbo.Roles (IdRol);
GO

/* ---------- Tabla Permiso ---------- */
IF OBJECT_ID('dbo.Permiso', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Permiso
    (
        idPermiso     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Permiso PRIMARY KEY,
        IdRol         INT               NOT NULL,
        codigo        NVARCHAR(50)       NOT NULL,
        FechaCreacion DATETIME2(0)       NOT NULL CONSTRAINT DF_Permiso_fecha DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_Permiso_Roles FOREIGN KEY (IdRol) REFERENCES dbo.Roles (IdRol),
        CONSTRAINT UQ_Permiso UNIQUE (IdRol, codigo)
    );
END
GO
