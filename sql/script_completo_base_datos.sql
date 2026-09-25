/* ============================================================
   SCRIPT COMPLETO DE BASE DE DATOS - Sistema de Caja (PosNg3)
   Generado automaticamente concatenando todos los scripts de la
   carpeta /sql, en el orden en que deben ejecutarse.
   ============================================================ */

/* Requerido por los indices filtrados; sqlcmd lo deja OFF por defecto.
   Aplica a toda la sesion, asi que cubre todo el script. */
SET QUOTED_IDENTIFIER ON;
GO


/* ============================================================
   ARCHIVO: 01_esquema_seguridad.sql
   ============================================================ */

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


/* ============================================================
   ARCHIVO: 02_seed_admin.sql
   ============================================================ */

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


/* ============================================================
   ARCHIVO: 04_esquema_caja.sql
   ============================================================ */

/* =====================================================================
   Sistema de Caja - Esquema de apertura/cierre de caja (multi-moneda)
   Idempotente: se puede ejecutar varias veces sin error.
   Ejecutar DESPUES de 01_esquema_seguridad.sql (requiere dbo.Usuario).

   Este script documenta en el repositorio el esquema que ya existe en
   la base de datos real (fue creado directamente en SSMS). Refleja
   fielmente columnas, defaults, checks, indices y foreign keys.
   ===================================================================== */

USE SistemaCaja;
GO

/* ---------- Tabla Caja (cajas registradoras/puntos de cobro) ---------- */
IF OBJECT_ID('dbo.Caja', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Caja
    (
        caja_id       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Caja PRIMARY KEY,
        nombre        NVARCHAR(200)     NOT NULL,
        estado        BIT               NOT NULL CONSTRAINT DF_Caja_estado DEFAULT (1),
        FechaCreacion DATETIME          NOT NULL CONSTRAINT DF_Caja_fecha  DEFAULT (GETDATE()),
        CONSTRAINT UQ_Caja_Nombre UNIQUE (nombre)
    );
END
GO

/* ---------- Tabla Moneda ---------- */
IF OBJECT_ID('dbo.Moneda', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Moneda
    (
        moneda_id     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Moneda PRIMARY KEY,
        nombre        NVARCHAR(100)     NOT NULL,
        codigo        VARCHAR(10)       NOT NULL,
        simbolo       VARCHAR(10)       NULL,
        estado        BIT               NOT NULL CONSTRAINT DF_Moneda_estado DEFAULT (1),
        FechaCreacion DATETIME          NOT NULL CONSTRAINT DF_Moneda_fecha  DEFAULT (GETDATE()),
        CONSTRAINT UQ_Moneda_Codigo UNIQUE (codigo)
    );
END
GO

/* ---------- Tabla FormaPago ---------- */
IF OBJECT_ID('dbo.FormaPago', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.FormaPago
    (
        forma_pago_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FormaPago PRIMARY KEY,
        nombre        NVARCHAR(100)     NOT NULL,
        estado        BIT               NOT NULL CONSTRAINT DF_FormaPago_estado DEFAULT (1),
        CONSTRAINT UQ_FormaPago_Nombre UNIQUE (nombre)
    );
END
GO

/* ---------- Tabla Concepto (motivos de ingreso/egreso de efectivo) ---------- */
IF OBJECT_ID('dbo.Concepto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Concepto
    (
        concepto_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Concepto PRIMARY KEY,
        operacion   VARCHAR(30)       NOT NULL,
        nombre      NVARCHAR(200)     NOT NULL,
        tipo        VARCHAR(10)       NOT NULL,
        estado      BIT               NOT NULL CONSTRAINT DF_Concepto_estado DEFAULT (1),
        CONSTRAINT UQ_Concepto_Operacion UNIQUE (operacion),
        CONSTRAINT CK_Concepto_Tipo CHECK (tipo IN ('INGRESO', 'EGRESO'))
    );
END
GO

/* ---------- Tabla AperturaCaja ---------- */
IF OBJECT_ID('dbo.AperturaCaja', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AperturaCaja
    (
        apertura_id   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AperturaCaja PRIMARY KEY,
        caja_id       INT               NOT NULL,
        usuario_id    INT               NOT NULL,
        fecha_hora    DATETIME          NOT NULL CONSTRAINT DF_AperturaCaja_fecha  DEFAULT (GETDATE()),
        estado        VARCHAR(20)       NOT NULL CONSTRAINT DF_AperturaCaja_estado DEFAULT ('ABIERTA'),
        observaciones NVARCHAR(255)     NULL,

        CONSTRAINT FK_AperturaCaja_Caja
            FOREIGN KEY (caja_id) REFERENCES dbo.Caja (caja_id),
        CONSTRAINT FK_AperturaCaja_Usuario
            FOREIGN KEY (usuario_id) REFERENCES dbo.Usuario (usuario_id),
        CONSTRAINT CK_AperturaCaja_Estado
            CHECK (estado IN ('ABIERTA', 'CERRADA')),

        /* Permite que Transacciones valide con FK compuesta (apertura_id, caja_id). */
        CONSTRAINT UQ_AperturaCaja_AperturaCaja UNIQUE (apertura_id, caja_id)
    );
END
GO

/* Solo puede haber una apertura ABIERTA por caja a la vez. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_UnaAperturaAbiertaPorCaja')
    CREATE UNIQUE INDEX UX_UnaAperturaAbiertaPorCaja
        ON dbo.AperturaCaja (caja_id)
        WHERE estado = 'ABIERTA';
GO

/* ---------- Tabla AperturaCajaMoneda (monto inicial por cada moneda) ---------- */
IF OBJECT_ID('dbo.AperturaCajaMoneda', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AperturaCajaMoneda
    (
        apertura_moneda_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AperturaCajaMoneda PRIMARY KEY,
        apertura_id        INT               NOT NULL,
        moneda_id          INT               NOT NULL,
        monto_inicial      DECIMAL(18,2)     NOT NULL CONSTRAINT DF_AperturaCajaMoneda_monto DEFAULT (0),

        CONSTRAINT FK_AperturaCajaMoneda_Apertura
            FOREIGN KEY (apertura_id) REFERENCES dbo.AperturaCaja (apertura_id),
        CONSTRAINT FK_AperturaCajaMoneda_Moneda
            FOREIGN KEY (moneda_id) REFERENCES dbo.Moneda (moneda_id),
        CONSTRAINT CK_AperturaCajaMoneda_Monto
            CHECK (monto_inicial >= 0),
        CONSTRAINT UQ_AperturaCajaMoneda UNIQUE (apertura_id, moneda_id)
    );
END
GO

/* ---------- Tabla CierreCaja ---------- */
IF OBJECT_ID('dbo.CierreCaja', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CierreCaja
    (
        cierre_id     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CierreCaja PRIMARY KEY,
        apertura_id   INT               NOT NULL,
        usuario_id    INT               NOT NULL,
        fecha_hora    DATETIME          NOT NULL CONSTRAINT DF_CierreCaja_fecha DEFAULT (GETDATE()),
        observaciones NVARCHAR(255)     NULL,

        CONSTRAINT FK_CierreCaja_Apertura
            FOREIGN KEY (apertura_id) REFERENCES dbo.AperturaCaja (apertura_id),
        CONSTRAINT FK_CierreCaja_Usuario
            FOREIGN KEY (usuario_id) REFERENCES dbo.Usuario (usuario_id),

        /* Una apertura solamente puede tener un cierre. */
        CONSTRAINT UQ_CierreCaja_Apertura UNIQUE (apertura_id)
    );
END
GO

/* ---------- Tabla CierreCajaMoneda (conteo final por cada moneda) ---------- */
IF OBJECT_ID('dbo.CierreCajaMoneda', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CierreCajaMoneda
    (
        cierre_moneda_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CierreCajaMoneda PRIMARY KEY,
        cierre_id        INT               NOT NULL,
        moneda_id        INT               NOT NULL,
        monto_sistema    DECIMAL(18,2)     NOT NULL,
        monto_final      DECIMAL(18,2)     NOT NULL,

        /* Diferencia = dinero fisico - dinero esperado */
        diferencia AS (monto_final - monto_sistema),

        CONSTRAINT FK_CierreCajaMoneda_Cierre
            FOREIGN KEY (cierre_id) REFERENCES dbo.CierreCaja (cierre_id),
        CONSTRAINT FK_CierreCajaMoneda_Moneda
            FOREIGN KEY (moneda_id) REFERENCES dbo.Moneda (moneda_id),
        CONSTRAINT UQ_CierreCajaMoneda UNIQUE (cierre_id, moneda_id),
        CONSTRAINT CK_CierreCajaMoneda_MontoSistema CHECK (monto_sistema >= 0),
        CONSTRAINT CK_CierreCajaMoneda_MontoFinal   CHECK (monto_final >= 0)
    );
END
GO

/* ---------- Tabla Transacciones (ingresos/egresos de efectivo durante una apertura) ---------- */
IF OBJECT_ID('dbo.Transacciones', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transacciones
    (
        transaccion_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Transacciones PRIMARY KEY,
        apertura_id    INT               NOT NULL,
        caja_id        INT               NOT NULL,
        usuario_id     INT               NOT NULL,
        concepto_id    INT               NOT NULL,
        moneda_id      INT               NOT NULL,
        forma_pago_id  INT               NULL,
        tipo           VARCHAR(10)       NOT NULL,
        monto          DECIMAL(18,2)     NOT NULL,
        descripcion    NVARCHAR(255)     NULL,
        fecha_hora     DATETIME          NOT NULL CONSTRAINT DF_Transacciones_fecha  DEFAULT (GETDATE()),
        estado         BIT               NOT NULL CONSTRAINT DF_Transacciones_estado DEFAULT (1),

        /* Coincide con UQ_AperturaCaja_AperturaCaja: valida que la caja de la
           transaccion sea realmente la caja de esa apertura. */
        CONSTRAINT FK_Transacciones_AperturaCaja
            FOREIGN KEY (apertura_id, caja_id) REFERENCES dbo.AperturaCaja (apertura_id, caja_id),
        CONSTRAINT FK_Transacciones_Usuario
            FOREIGN KEY (usuario_id) REFERENCES dbo.Usuario (usuario_id),
        CONSTRAINT FK_Transacciones_Concepto
            FOREIGN KEY (concepto_id) REFERENCES dbo.Concepto (concepto_id),
        CONSTRAINT FK_Transacciones_Moneda
            FOREIGN KEY (moneda_id) REFERENCES dbo.Moneda (moneda_id),
        CONSTRAINT FK_Transacciones_FormaPago
            FOREIGN KEY (forma_pago_id) REFERENCES dbo.FormaPago (forma_pago_id),

        CONSTRAINT CK_Transacciones_Tipo  CHECK (tipo IN ('INGRESO', 'EGRESO')),
        CONSTRAINT CK_Transacciones_Monto CHECK (monto > 0)
    );

    CREATE INDEX IX_Transacciones_Apertura     ON dbo.Transacciones (apertura_id);
    CREATE INDEX IX_Transacciones_Caja_Moneda  ON dbo.Transacciones (caja_id, moneda_id);
    CREATE INDEX IX_Transacciones_Concepto     ON dbo.Transacciones (concepto_id);
    CREATE INDEX IX_Transacciones_Fecha        ON dbo.Transacciones (fecha_hora);
END
GO


/* ============================================================
   ARCHIVO: 05_seed_caja.sql
   ============================================================ */

/* =====================================================================
   Sistema de Caja - Datos maestros para apertura/cierre de caja
   Idempotente. Ejecutar DESPUES de 04_esquema_caja.sql.

   Solo siembra tablas de catalogo (Moneda, Caja, FormaPago, Concepto).
   Las tablas transaccionales (AperturaCaja, AperturaCajaMoneda, CierreCaja,
   CierreCajaMoneda, Transacciones) NO se siembran: representan eventos
   reales que debe generar la aplicacion, no datos de referencia.
   ===================================================================== */

USE SistemaCaja;
GO

/* ---------- Moneda ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Moneda WHERE codigo = 'NIO')
    INSERT dbo.Moneda (nombre, codigo, simbolo) VALUES (N'Cordoba Nicaraguense', 'NIO', 'C$');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Moneda WHERE codigo = 'USD')
    INSERT dbo.Moneda (nombre, codigo, simbolo) VALUES (N'Dolar Estadounidense', 'USD', '$');
GO

/* ---------- Caja ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Caja WHERE nombre = N'Caja Principal')
    INSERT dbo.Caja (nombre) VALUES (N'Caja Principal');
GO

/* ---------- FormaPago ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.FormaPago WHERE nombre = N'Efectivo')
    INSERT dbo.FormaPago (nombre) VALUES (N'Efectivo');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.FormaPago WHERE nombre = N'Cheque')
    INSERT dbo.FormaPago (nombre) VALUES (N'Cheque');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.FormaPago WHERE nombre = N'Transferencia')
    INSERT dbo.FormaPago (nombre) VALUES (N'Transferencia');
GO

/* ---------- Concepto ----------
   Coinciden con las opciones ya presentes en los combos de FrmIngreso/FrmSalida. */
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'DOTACION')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('DOTACION', N'Dotacion', 'INGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'VENTA')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('VENTA', N'Venta', 'INGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'OTRO_INGRESO')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('OTRO_INGRESO', N'Otro ingreso', 'INGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'DEPOSITO_BANPRO')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('DEPOSITO_BANPRO', N'Deposito BANPRO', 'EGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'DEPOSITO_BAC')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('DEPOSITO_BAC', N'Deposito BAC', 'EGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'PAGALO_TODO')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('PAGALO_TODO', N'Pagalo Todo', 'EGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'OTRO_EGRESO')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('OTRO_EGRESO', N'Otro egreso', 'EGRESO');
GO


/* ============================================================
   ARCHIVO: 06_seed_cambio_divisas.sql
   ============================================================ */

/* =====================================================================
   Sistema de Caja - Conceptos para operaciones de Mesa de Cambio
   Idempotente. Ejecutar DESPUES de 04_esquema_caja.sql y 05_seed_caja.sql.

   Una operacion de cambio de divisa se registra en Transacciones como un
   par: un INGRESO (moneda que el cliente entrega y la caja recibe) y un
   EGRESO (moneda que la caja entrega al cliente). Estos son los conceptos
   que identifican ese par.
   ===================================================================== */

USE SistemaCaja;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'CAMBIO_DIVISA_RECIBIDO')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('CAMBIO_DIVISA_RECIBIDO', N'Cambio de Divisa (Recibido)', 'INGRESO');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Concepto WHERE operacion = 'CAMBIO_DIVISA_ENTREGADO')
    INSERT dbo.Concepto (operacion, nombre, tipo) VALUES ('CAMBIO_DIVISA_ENTREGADO', N'Cambio de Divisa (Entregado)', 'EGRESO');
GO


/* ============================================================
   ARCHIVO: 07_esquema_tasa_cambio.sql
   ============================================================ */

/* =====================================================================
   Sistema de Caja - Tasas de cambio para Mesa de Cambio
   Idempotente. Ejecutar DESPUES de 04_esquema_caja.sql.

   Una tasa vigente por (moneda, tipo de operacion). "moneda" es siempre
   la divisa extranjera (p.ej. USD); el valor se expresa en NIO por cada
   unidad de esa divisa. COMPRA = tasa a la que la casa compra la divisa
   del cliente; VENTA = tasa a la que la casa la vende.
   ===================================================================== */

USE SistemaCaja;
GO

SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.TasaCambio', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TasaCambio
    (
        tasa_id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TasaCambio PRIMARY KEY,
        moneda_id      INT               NOT NULL,
        tipo_operacion VARCHAR(10)       NOT NULL,
        valor          DECIMAL(18,4)     NOT NULL,
        fecha_hora     DATETIME          NOT NULL CONSTRAINT DF_TasaCambio_fecha  DEFAULT (GETDATE()),
        estado         BIT               NOT NULL CONSTRAINT DF_TasaCambio_estado DEFAULT (1),

        CONSTRAINT FK_TasaCambio_Moneda FOREIGN KEY (moneda_id) REFERENCES dbo.Moneda (moneda_id),
        CONSTRAINT CK_TasaCambio_Tipo CHECK (tipo_operacion IN ('COMPRA', 'VENTA')),
        CONSTRAINT CK_TasaCambio_Valor CHECK (valor > 0)
    );
END
GO

/* Solo puede haber una tasa vigente por moneda y tipo de operacion. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_TasaCambio_MonedaTipoVigente')
    CREATE UNIQUE INDEX UX_TasaCambio_MonedaTipoVigente
        ON dbo.TasaCambio (moneda_id, tipo_operacion)
        WHERE estado = 1;
GO

/* ---------- Seed: tasa inicial para USD (ajustar al valor real del dia) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.TasaCambio t JOIN dbo.Moneda m ON m.moneda_id = t.moneda_id WHERE m.codigo = 'USD' AND t.tipo_operacion = 'COMPRA')
    INSERT dbo.TasaCambio (moneda_id, tipo_operacion, valor)
    SELECT moneda_id, 'COMPRA', 36.6000 FROM dbo.Moneda WHERE codigo = 'USD';
GO
IF NOT EXISTS (SELECT 1 FROM dbo.TasaCambio t JOIN dbo.Moneda m ON m.moneda_id = t.moneda_id WHERE m.codigo = 'USD' AND t.tipo_operacion = 'VENTA')
    INSERT dbo.TasaCambio (moneda_id, tipo_operacion, valor)
    SELECT moneda_id, 'VENTA', 36.9000 FROM dbo.Moneda WHERE codigo = 'USD';
GO


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


/* =====================================================================
   Sistema de Caja - Auditoria de anulacion de transacciones
   Registra que Administrador autorizo la anulacion y cuando.
   ===================================================================== */

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


/* ============================================================
   NOTA: 03_migrar_contrasenas.md es una guia de pasos manuales
   (no es un script SQL ejecutable). Su contenido:
   ============================================================ */

-- # Migración de contraseñas en texto plano
-- 
-- Antes de estos cambios, la columna `Usuario.password_hash` guardaba la contraseña
-- **en texto plano** y el login la comparaba directamente. Ahora se guarda un hash
-- PBKDF2 y el texto plano ya no sirve para iniciar sesión.
-- 
-- No es posible convertir un texto plano a este hash con solo SQL (hace falta PBKDF2).
-- Por eso, para las cuentas que ya existían hay que **re-establecer** la contraseña.
-- 
-- ## Opción recomendada (rápida, para pocas cuentas)
-- 
-- 1. Ejecutá `01_esquema_seguridad.sql` y luego `02_seed_admin.sql`.
-- 2. Iniciá sesión con `admin` / `Admin123*`.
-- 3. Volvé a crear cada usuario desde la aplicación (pantalla de administración de
--    usuarios), o generá su hash y actualizá la fila:
-- 
-- ```sql
-- -- Reemplazá <HASH> por el valor que produce PasswordHasher.Hash("<clave nueva>")
-- UPDATE dbo.Usuario
-- SET    password_hash = '<HASH>',
--        estado        = 1
-- WHERE  usuario = N'<nombre_de_usuario>';
-- ```
-- 
-- ## Mientras tanto: desactivar las cuentas viejas
-- 
-- Para que nadie entre con una contraseña en texto plano que quedó guardada:
-- 
-- ```sql
-- USE SistemaCaja;
-- 
-- UPDATE dbo.Usuario
-- SET    estado = 0
-- WHERE  usuario <> N'admin'
--   AND  password_hash NOT LIKE 'PBKDF2.SHA256.%';
-- ```
-- 
-- El login rechaza cualquier `password_hash` que no tenga el formato
-- `PBKDF2.SHA256....`, así que esas cuentas ya no pueden entrar aunque estén activas;
-- este `UPDATE` solo lo deja explícito.
-- 
-- ## Generar un hash suelto
-- 
-- Cualquier `string` que devuelva `CapaNegocio.Seguridad.PasswordHasher.Hash(clave)`
-- es un valor válido para la columna. Se puede obtener con un pequeño programa de
-- consola que referencie `CapaNegocio`, o con PowerShell replicando los parámetros
-- (PBKDF2-HMAC-SHA256, 100000 iteraciones, sal de 16 bytes, clave derivada de 32 bytes).
