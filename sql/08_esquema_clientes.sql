/* =====================================================================
   Sistema de Caja - Clientes de Mesa de Cambio
   Idempotente. Ejecutar DESPUES de 04_esquema_caja.sql.
   ===================================================================== */

USE SistemaCaja;
GO

SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.Clientes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        cliente_id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY,
        tipo_identificacion   NVARCHAR(50)       NOT NULL,
        numero_identificacion NVARCHAR(50)       NOT NULL,
        nombres               NVARCHAR(100)      NOT NULL,
        apellidos             NVARCHAR(100)      NULL,
        telefono              NVARCHAR(50)       NULL,
        direccion             NVARCHAR(200)      NULL,
        correo                NVARCHAR(150)      NULL,
        fecha_registro        DATETIME           NOT NULL CONSTRAINT DF_Clientes_fecha DEFAULT (GETDATE()),

        CONSTRAINT UQ_Clientes_Identificacion UNIQUE (tipo_identificacion, numero_identificacion)
    );
END
GO

/* Instalaciones que ya habian creado la tabla con una sola columna "nombre": la renombramos. */
IF COL_LENGTH('dbo.Clientes', 'nombre') IS NOT NULL AND COL_LENGTH('dbo.Clientes', 'nombres') IS NULL
    EXEC sp_rename 'dbo.Clientes.nombre', 'nombres', 'COLUMN';
GO

IF COL_LENGTH('dbo.Clientes', 'apellidos') IS NULL
    ALTER TABLE dbo.Clientes ADD apellidos NVARCHAR(100) NULL;
GO

IF COL_LENGTH('dbo.Clientes', 'correo') IS NULL
    ALTER TABLE dbo.Clientes ADD correo NVARCHAR(150) NULL;
GO
