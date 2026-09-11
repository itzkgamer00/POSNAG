/* =====================================================================
   Sistema de Caja - Datos iniciales: rol Administrador + usuario 'admin'
   Idempotente. Ejecutar DESPUES de 01_esquema_seguridad.sql.

   Usuario:     admin
   Contrasena:  Admin123*
   >>> Cambie esta contrasena inmediatamente despues del primer ingreso. <<<

   El hash es PBKDF2-HMAC-SHA256, 100000 iteraciones, sal de 16 bytes,
   en el formato "PBKDF2.SHA256.{iteraciones}.{salBase64}.{hashBase64}"
   que entiende CapaNegocio.Seguridad.PasswordHasher.
   ===================================================================== */

USE SistemaCaja;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Descripcion = N'Administrador')
    INSERT dbo.Roles (Descripcion) VALUES (N'Administrador');
GO

DECLARE @idRolAdmin INT = (SELECT IdRol FROM dbo.Roles WHERE Descripcion = N'Administrador');

IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE usuario = N'admin')
BEGIN
    INSERT dbo.Usuario (NombreCompleto, usuario, password_hash, IdRol, estado)
    VALUES (N'Administrador del sistema',
            N'admin',
            'PBKDF2.SHA256.100000.EUOc/YlHKiXhXSzgV30FlQ==.8UEONMgoDlhH83n+SoZ/tbB+inzMp2aQFdUPE8sHOEg=',
            @idRolAdmin,
            1);
END
ELSE
BEGIN
    /* Si ya existe, se asegura el rol pero NO se pisa la contrasena. */
    UPDATE dbo.Usuario SET IdRol = @idRolAdmin WHERE usuario = N'admin' AND IdRol IS NULL;
END
GO
