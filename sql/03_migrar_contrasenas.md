# Migración de contraseñas en texto plano

Antes de estos cambios, la columna `Usuario.password_hash` guardaba la contraseña
**en texto plano** y el login la comparaba directamente. Ahora se guarda un hash
PBKDF2 y el texto plano ya no sirve para iniciar sesión.

No es posible convertir un texto plano a este hash con solo SQL (hace falta PBKDF2).
Por eso, para las cuentas que ya existían hay que **re-establecer** la contraseña.

## Opción recomendada (rápida, para pocas cuentas)

1. Ejecutá `01_esquema_seguridad.sql` y luego `02_seed_admin.sql`.
2. Iniciá sesión con `admin` / `Admin123*`.
3. Volvé a crear cada usuario desde la aplicación (pantalla de administración de
   usuarios), o generá su hash y actualizá la fila:

```sql
-- Reemplazá <HASH> por el valor que produce PasswordHasher.Hash("<clave nueva>")
UPDATE dbo.Usuario
SET    password_hash = '<HASH>',
       estado        = 1
WHERE  usuario = N'<nombre_de_usuario>';
```

## Mientras tanto: desactivar las cuentas viejas

Para que nadie entre con una contraseña en texto plano que quedó guardada:

```sql
USE SistemaCaja;

UPDATE dbo.Usuario
SET    estado = 0
WHERE  usuario <> N'admin'
  AND  password_hash NOT LIKE 'PBKDF2.SHA256.%';
```

El login rechaza cualquier `password_hash` que no tenga el formato
`PBKDF2.SHA256....`, así que esas cuentas ya no pueden entrar aunque estén activas;
este `UPDATE` solo lo deja explícito.

## Generar un hash suelto

Cualquier `string` que devuelva `CapaNegocio.Seguridad.PasswordHasher.Hash(clave)`
es un valor válido para la columna. Se puede obtener con un pequeño programa de
consola que referencie `CapaNegocio`, o con PowerShell replicando los parámetros
(PBKDF2-HMAC-SHA256, 100000 iteraciones, sal de 16 bytes, clave derivada de 32 bytes).
