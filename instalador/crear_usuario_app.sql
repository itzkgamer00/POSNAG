/* =====================================================================
   Sistema de Caja - Usuario SQL de la aplicacion
   Crea (o actualiza la contraseña de) el login que usan las cajas para
   conectarse a SistemaCaja, con permisos solo de lectura/escritura de datos.
   Lo ejecuta el instalador en el equipo principal con:
     sqlcmd -S <servidor> -E -b -i crear_usuario_app.sql -v UsuarioApp="..." ClaveApp="..."
   Idempotente.
   ===================================================================== */

USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'$(UsuarioApp)')
    CREATE LOGIN [$(UsuarioApp)] WITH PASSWORD = N'$(ClaveApp)', CHECK_POLICY = OFF, DEFAULT_DATABASE = SistemaCaja;
ELSE
    ALTER LOGIN [$(UsuarioApp)] WITH PASSWORD = N'$(ClaveApp)';
GO

ALTER LOGIN [$(UsuarioApp)] ENABLE;
GO

USE SistemaCaja;
GO

IF USER_ID(N'$(UsuarioApp)') IS NULL
    CREATE USER [$(UsuarioApp)] FOR LOGIN [$(UsuarioApp)];
GO

ALTER ROLE db_datareader ADD MEMBER [$(UsuarioApp)];
ALTER ROLE db_datawriter ADD MEMBER [$(UsuarioApp)];
GO
