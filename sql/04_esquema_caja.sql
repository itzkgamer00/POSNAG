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
