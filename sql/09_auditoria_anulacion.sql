/* =====================================================================
   Sistema de Caja - Auditoria de anulacion de transacciones
   Registra que Administrador autorizo la anulacion y cuando.
   Idempotente. Ejecutar DESPUES de 04_esquema_caja.sql.
   ===================================================================== */

USE SistemaCaja;
GO

SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.Transacciones', 'anulado_por') IS NULL
    ALTER TABLE dbo.Transacciones ADD anulado_por INT NULL;
GO

IF COL_LENGTH('dbo.Transacciones', 'fecha_anulacion') IS NULL
    ALTER TABLE dbo.Transacciones ADD fecha_anulacion DATETIME NULL;
GO

IF OBJECT_ID('dbo.FK_Transacciones_AnuladoPor', 'F') IS NULL
    ALTER TABLE dbo.Transacciones ADD CONSTRAINT FK_Transacciones_AnuladoPor
        FOREIGN KEY (anulado_por) REFERENCES dbo.Usuario (usuario_id);
GO
