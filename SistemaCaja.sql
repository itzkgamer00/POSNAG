
IF DB_ID(N'SistemaCaja') IS NOT NULL
BEGIN
    ALTER DATABASE SistemaCaja SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaCaja;
END
GO

CREATE DATABASE SistemaCaja;
GO

USE SistemaCaja;
GO


-- SEGURIDAD Y PERMISOS


CREATE TABLE Roles (
    IdRol           INT PRIMARY KEY IDENTITY(1,1),
    Descripcion     VARCHAR(100) NOT NULL,
    Estado          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Usuario (
    usuario_id      INT PRIMARY KEY IDENTITY(1,1),
    NombreCompleto  VARCHAR(100) NOT NULL,
    Correo          VARCHAR(100) NULL,
    usuario         VARCHAR(50) NOT NULL,
    password_hash   VARCHAR(255) NOT NULL,
    IdRol           INT NOT NULL,
    estado          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Usuario_Rol
        FOREIGN KEY (IdRol) REFERENCES Roles(IdRol),
    CONSTRAINT UQ_Usuario_Login
        UNIQUE (usuario)
);

-- Permisos por rol 
CREATE TABLE Permiso (
    idPermiso       INT PRIMARY KEY IDENTITY(1,1),
    IdRol           INT NOT NULL,
    codigo          VARCHAR(50) NOT NULL,   -- DASHBOARD, INGRESOS, USUARIOS, etc.
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Permiso_Rol
        FOREIGN KEY (IdRol) REFERENCES Roles(IdRol),
    CONSTRAINT UQ_Permiso_Rol_Codigo
        UNIQUE (IdRol, codigo)
);


-- 3. CATÁLOGOS

CREATE TABLE Moneda (
    moneda_id       INT PRIMARY KEY IDENTITY(1,1),
    nombre          VARCHAR(50) NOT NULL,
    codigo          VARCHAR(3) NOT NULL,        -- ISO: USD, MXN, PEN, etc.
    simbolo         VARCHAR(10) NULL,
    es_base         BIT NOT NULL DEFAULT 0,     -- Moneda local de referencia
    estado          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT UQ_Moneda_Codigo
        UNIQUE (codigo)
);

CREATE TABLE CategoriaMovimiento (
    categoria_id    INT PRIMARY KEY IDENTITY(1,1),
    nombre          VARCHAR(100) NOT NULL,
    tipo            VARCHAR(10) NOT NULL,
    estado          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_CategoriaMovimiento_Tipo
        CHECK (tipo IN ('INGRESO', 'EGRESO'))
);

-- Clientes del sistema (módulo Clientes)
CREATE TABLE Cliente (
    cliente_id      INT PRIMARY KEY IDENTITY(1,1),
    nombre          VARCHAR(150) NOT NULL,
    documento       VARCHAR(30) NULL,
    telefono        VARCHAR(20) NULL,
    correo          VARCHAR(100) NULL,
    direccion       VARCHAR(255) NULL,
    estado          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE UNIQUE INDEX UX_Cliente_Documento
    ON Cliente(documento)
    WHERE documento IS NOT NULL;

-- Procesos / referencias para ComboBox (módulo Procesos)
-- tipo_catalogo: OPERACION = referencia del movimiento (Depósito BAC, etc.)
--                  FORMA_PAGO = tipo de pago (Efectivo, Cheque, etc.)
CREATE TABLE Proceso (
    proceso_id      INT PRIMARY KEY IDENTITY(1,1),
    nombre          VARCHAR(100) NOT NULL,
    tipo_catalogo   VARCHAR(20) NOT NULL,
    tipo_movimiento VARCHAR(10) NOT NULL DEFAULT 'AMBOS',
    descripcion     VARCHAR(255) NULL,
    orden           INT NOT NULL DEFAULT 0,
    estado          BIT NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Proceso_TipoCatalogo
        CHECK (tipo_catalogo IN ('OPERACION', 'FORMA_PAGO')),
    CONSTRAINT CK_Proceso_TipoMovimiento
        CHECK (tipo_movimiento IN ('INGRESO', 'EGRESO', 'AMBOS')),
    CONSTRAINT UQ_Proceso_Nombre_Catalogo
        UNIQUE (nombre, tipo_catalogo)
);

CREATE INDEX IX_Proceso_Catalogo
    ON Proceso(tipo_catalogo, tipo_movimiento, estado, orden);


-- 4. APERTURA Y CIERRE DE CAJA (MULTIMONEDA)
 

CREATE TABLE AperturaCaja (
    apertura_id     INT PRIMARY KEY IDENTITY(1,1),
    usuario_id      INT NOT NULL,
    fecha_hora      DATETIME NOT NULL DEFAULT GETDATE(),
    estado          VARCHAR(20) NOT NULL DEFAULT 'ABIERTA',
    observaciones   VARCHAR(255) NULL,

    CONSTRAINT FK_AperturaCaja_Usuario
        FOREIGN KEY (usuario_id) REFERENCES Usuario(usuario_id),
    CONSTRAINT CK_AperturaCaja_Estado
        CHECK (estado IN ('ABIERTA', 'CERRADA'))
);

-- Saldo inicial por moneda al abrir caja
CREATE TABLE AperturaCajaDetalle (
    detalle_id      INT PRIMARY KEY IDENTITY(1,1),
    apertura_id     INT NOT NULL,
    moneda_id       INT NOT NULL,
    monto_inicial   DECIMAL(18,2) NOT NULL,

    CONSTRAINT FK_AperturaDetalle_Apertura
        FOREIGN KEY (apertura_id) REFERENCES AperturaCaja(apertura_id),
    CONSTRAINT FK_AperturaDetalle_Moneda
        FOREIGN KEY (moneda_id) REFERENCES Moneda(moneda_id),
    CONSTRAINT CK_AperturaDetalle_Monto
        CHECK (monto_inicial >= 0),
    CONSTRAINT UQ_AperturaDetalle_Apertura_Moneda
        UNIQUE (apertura_id, moneda_id)
);

CREATE TABLE CierreCaja (
    cierre_id       INT PRIMARY KEY IDENTITY(1,1),
    apertura_id     INT NOT NULL,
    usuario_id      INT NOT NULL,
    fecha_hora      DATETIME NOT NULL DEFAULT GETDATE(),
    observaciones   VARCHAR(255) NULL,

    CONSTRAINT FK_CierreCaja_Apertura
        FOREIGN KEY (apertura_id) REFERENCES AperturaCaja(apertura_id),
    CONSTRAINT FK_CierreCaja_Usuario
        FOREIGN KEY (usuario_id) REFERENCES Usuario(usuario_id),
    CONSTRAINT UQ_CierreCaja_Apertura
        UNIQUE (apertura_id)   -- Una sola vez por apertura
);

-- Arqueo por moneda al cerrar caja
CREATE TABLE CierreCajaDetalle (
    detalle_id      INT PRIMARY KEY IDENTITY(1,1),
    cierre_id       INT NOT NULL,
    moneda_id       INT NOT NULL,
    monto_sistema   DECIMAL(18,2) NOT NULL,     -- Calculado según movimientos
    monto_contado   DECIMAL(18,2) NOT NULL,     -- Contado físicamente
    diferencia      AS (monto_contado - monto_sistema) PERSISTED,

    CONSTRAINT FK_CierreDetalle_Cierre
        FOREIGN KEY (cierre_id) REFERENCES CierreCaja(cierre_id),
    CONSTRAINT FK_CierreDetalle_Moneda
        FOREIGN KEY (moneda_id) REFERENCES Moneda(moneda_id),
    CONSTRAINT UQ_CierreDetalle_Cierre_Moneda
        UNIQUE (cierre_id, moneda_id)
);

-- Solo una caja abierta por usuario
CREATE UNIQUE INDEX UX_AperturaCaja_Usuario_Abierta
    ON AperturaCaja(usuario_id)
    WHERE estado = 'ABIERTA';

-- ============================================================
-- 5. MOVIMIENTOS


CREATE TABLE CambioDivisa (
    cambio_id           INT PRIMARY KEY IDENTITY(1,1),
    apertura_id         INT NOT NULL,
    usuario_id          INT NOT NULL,
    tipo                VARCHAR(10) NOT NULL,   -- COMPRA | VENTA
    moneda_origen_id    INT NOT NULL,
    monto_origen        DECIMAL(18,2) NOT NULL,
    moneda_destino_id   INT NOT NULL,
    monto_destino       DECIMAL(18,2) NOT NULL,
    tasa_cambio         DECIMAL(18,6) NOT NULL,
    descripcion         VARCHAR(255) NULL,
    estado              BIT NOT NULL DEFAULT 1,
    fecha_hora          DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_CambioDivisa_Apertura
        FOREIGN KEY (apertura_id) REFERENCES AperturaCaja(apertura_id),
    CONSTRAINT FK_CambioDivisa_Usuario
        FOREIGN KEY (usuario_id) REFERENCES Usuario(usuario_id),
    CONSTRAINT FK_CambioDivisa_MonedaOrigen
        FOREIGN KEY (moneda_origen_id) REFERENCES Moneda(moneda_id),
    CONSTRAINT FK_CambioDivisa_MonedaDestino
        FOREIGN KEY (moneda_destino_id) REFERENCES Moneda(moneda_id),
    CONSTRAINT CK_CambioDivisa_Tipo
        CHECK (tipo IN ('COMPRA', 'VENTA')),
    CONSTRAINT CK_CambioDivisa_Montos
        CHECK (monto_origen > 0 AND monto_destino > 0),
    CONSTRAINT CK_CambioDivisa_MonedasDistintas
        CHECK (moneda_origen_id <> moneda_destino_id)
);

CREATE TABLE Transacciones (
    transaccion_id      INT PRIMARY KEY IDENTITY(1,1),
    apertura_id         INT NOT NULL,
    usuario_id          INT NOT NULL,
    cliente_id          INT NULL,
    moneda_id           INT NOT NULL,
    categoria_id        INT NULL,
    proceso_id          INT NULL,               -- Operación / referencia (ComboBox operación)
    forma_pago_id       INT NULL,               -- Forma de pago (ComboBox tipo de pago)
    cambio_id           INT NULL,
    tipo                VARCHAR(10) NOT NULL,   -- INGRESO | EGRESO
    monto               DECIMAL(18,2) NOT NULL,
    descripcion         VARCHAR(255) NULL,
    referencia          VARCHAR(100) NULL,      -- Nº comprobante, folio, etc.
    fecha_hora          DATETIME NOT NULL DEFAULT GETDATE(),
    estado              BIT NOT NULL DEFAULT 1,
    usuario_modifico    INT NULL,
    fecha_modificacion  DATETIME NULL,

    CONSTRAINT FK_Transacciones_Apertura
        FOREIGN KEY (apertura_id) REFERENCES AperturaCaja(apertura_id),
    CONSTRAINT FK_Transacciones_Usuario
        FOREIGN KEY (usuario_id) REFERENCES Usuario(usuario_id),
    CONSTRAINT FK_Transacciones_Cliente
        FOREIGN KEY (cliente_id) REFERENCES Cliente(cliente_id),
    CONSTRAINT FK_Transacciones_Moneda
        FOREIGN KEY (moneda_id) REFERENCES Moneda(moneda_id),
    CONSTRAINT FK_Transacciones_Categoria
        FOREIGN KEY (categoria_id) REFERENCES CategoriaMovimiento(categoria_id),
    CONSTRAINT FK_Transacciones_Proceso
        FOREIGN KEY (proceso_id) REFERENCES Proceso(proceso_id),
    CONSTRAINT FK_Transacciones_FormaPago
        FOREIGN KEY (forma_pago_id) REFERENCES Proceso(proceso_id),
    CONSTRAINT FK_Transacciones_CambioDivisa
        FOREIGN KEY (cambio_id) REFERENCES CambioDivisa(cambio_id),
    CONSTRAINT FK_Transacciones_UsuarioModifico
        FOREIGN KEY (usuario_modifico) REFERENCES Usuario(usuario_id),
    CONSTRAINT CK_Transacciones_Tipo
        CHECK (tipo IN ('INGRESO', 'EGRESO')),
    CONSTRAINT CK_Transacciones_Monto
        CHECK (monto > 0)
);


-- 6. ÍNDICES PARA CONSULTAS Y REPORTES


CREATE INDEX IX_Transacciones_Fecha
    ON Transacciones(fecha_hora DESC);

CREATE INDEX IX_Transacciones_Apertura
    ON Transacciones(apertura_id, estado);

CREATE INDEX IX_Transacciones_Usuario
    ON Transacciones(usuario_id, fecha_hora DESC);

CREATE INDEX IX_Transacciones_Cliente
    ON Transacciones(cliente_id);

CREATE INDEX IX_Transacciones_Proceso
    ON Transacciones(proceso_id);

CREATE INDEX IX_Cliente_Nombre
    ON Cliente(nombre);

CREATE INDEX IX_CambioDivisa_Apertura
    ON CambioDivisa(apertura_id, estado);

CREATE INDEX IX_CambioDivisa_Fecha
    ON CambioDivisa(fecha_hora DESC);

CREATE INDEX IX_AperturaCaja_Usuario
    ON AperturaCaja(usuario_id, fecha_hora DESC);


-- 7. VISTAS ÚTILES


GO
CREATE VIEW vw_SaldoPorMonedaApertura AS
SELECT
    a.apertura_id,
    a.usuario_id,
    a.estado AS estado_caja,
    m.moneda_id,
    m.codigo AS moneda,
    m.simbolo,
    ISNULL(d.monto_inicial, 0) AS monto_inicial,
    ISNULL(SUM(CASE WHEN t.tipo = 'INGRESO' AND t.estado = 1 THEN t.monto ELSE 0 END), 0) AS total_ingresos,
    ISNULL(SUM(CASE WHEN t.tipo = 'EGRESO'  AND t.estado = 1 THEN t.monto ELSE 0 END), 0) AS total_egresos,
    ISNULL(d.monto_inicial, 0)
        + ISNULL(SUM(CASE WHEN t.tipo = 'INGRESO' AND t.estado = 1 THEN t.monto ELSE 0 END), 0)
        - ISNULL(SUM(CASE WHEN t.tipo = 'EGRESO'  AND t.estado = 1 THEN t.monto ELSE 0 END), 0) AS saldo_sistema
FROM AperturaCaja a
CROSS JOIN Moneda m
LEFT JOIN AperturaCajaDetalle d
    ON d.apertura_id = a.apertura_id AND d.moneda_id = m.moneda_id
LEFT JOIN Transacciones t
    ON t.apertura_id = a.apertura_id AND t.moneda_id = m.moneda_id
WHERE m.estado = 1
GROUP BY
    a.apertura_id, a.usuario_id, a.estado,
    m.moneda_id, m.codigo, m.simbolo, d.monto_inicial;
GO

-- Operaciones para ComboBox de ingresos/egresos
CREATE VIEW vw_ProcesosOperacion AS
SELECT
    proceso_id,
    nombre,
    tipo_movimiento,
    orden
FROM Proceso
WHERE tipo_catalogo = 'OPERACION'
  AND estado = 1;
GO

-- Formas de pago para ComboBox
CREATE VIEW vw_FormasPago AS
SELECT
    proceso_id AS forma_pago_id,
    nombre,
    orden
FROM Proceso
WHERE tipo_catalogo = 'FORMA_PAGO'
  AND estado = 1;
GO

-- Clientes activos para ComboBox o búsqueda
CREATE VIEW vw_ClientesActivos AS
SELECT
    cliente_id,
    nombre,
    documento,
    telefono,
    correo
FROM Cliente
WHERE estado = 1;
GO

-- ============================================================
-- 8. DATOS INICIALES (orden correcto de dependencia

-- Roles
INSERT INTO Roles (Descripcion) VALUES ('Administrador');
INSERT INTO Roles (Descripcion) VALUES ('Cajero');
INSERT INTO Roles (Descripcion) VALUES ('Supervisor');

-- Usuario admin (reemplazar password_hash por hash real desde la aplicación)
INSERT INTO Usuario (NombreCompleto, Correo, usuario, password_hash, IdRol, estado)
VALUES ('Administrador', 'kenrrichg@gmail.com', 'admin', 'admin', 1, 1);

-- Permisos por rol (usar codigo en WinForms para mostrar/ocultar botones)
INSERT INTO Permiso (IdRol, codigo) VALUES
(1, 'DASHBOARD'), (1, 'APERTURA_CAJA'), (1, 'CIERRE_CAJA'),
(1, 'INGRESOS'), (1, 'EGRESOS'), (1, 'CAMBIO_DIVISAS'),
(1, 'TRANSACCIONES'), (1, 'USUARIOS'), (1, 'CLIENTES'), (1, 'PROCESOS');

INSERT INTO Permiso (IdRol, codigo) VALUES
(2, 'DASHBOARD'), (2, 'APERTURA_CAJA'), (2, 'CIERRE_CAJA'),
(2, 'INGRESOS'), (2, 'EGRESOS'), (2, 'CAMBIO_DIVISAS'),
(2, 'TRANSACCIONES'), (2, 'CLIENTES');

INSERT INTO Permiso (IdRol, codigo) VALUES
(3, 'DASHBOARD'), (3, 'APERTURA_CAJA'), (3, 'CIERRE_CAJA'),
(3, 'INGRESOS'), (3, 'EGRESOS'), (3, 'CAMBIO_DIVISAS'),
(3, 'TRANSACCIONES'), (3, 'USUARIOS'), (3, 'CLIENTES'), (3, 'PROCESOS');

-- Monedas
INSERT INTO Moneda (nombre, codigo, simbolo, es_base) VALUES ('Sol Peruano',  'PEN', 'S/', 1);
INSERT INTO Moneda (nombre, codigo, simbolo, es_base) VALUES ('Dólar USA',    'USD', '$',  0);
INSERT INTO Moneda (nombre, codigo, simbolo, es_base) VALUES ('Euro',         'EUR', '€',  0);

-- Categorías de movimiento
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Venta contado',        'INGRESO');
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Cobro de servicio',    'INGRESO');
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Ingreso extraordinario','INGRESO');
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Compra insumos',       'EGRESO');
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Pago proveedor',       'EGRESO');
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Gasto operativo',      'EGRESO');
INSERT INTO CategoriaMovimiento (nombre, tipo) VALUES ('Retiro de caja',       'EGRESO');

-- Clientes de ejemplo
INSERT INTO Cliente (nombre, documento, telefono, correo) VALUES
('Cliente General', NULL, NULL, NULL),
('Juan Pérez', '001-250789-0001A', '8888-1234', 'juan.perez@correo.com'),
('María López', '001-120456-0002B', '8888-5678', 'maria.lopez@correo.com');

-- Procesos / operaciones (ComboBox operación en Ingresos y Egresos)
INSERT INTO Proceso (nombre, tipo_catalogo, tipo_movimiento, orden) VALUES
('Depósito Banpro',       'OPERACION', 'INGRESO', 1),
('Depósito BAC',          'OPERACION', 'INGRESO', 2),
('Depósito a cuenta',     'OPERACION', 'INGRESO', 3),
('Págalo Todo',           'OPERACION', 'INGRESO', 4),
('Dotación',              'OPERACION', 'EGRESO',  5),
('Retiro de efectivo',    'OPERACION', 'EGRESO',  6),
('Pago a proveedor',      'OPERACION', 'EGRESO',  7);

-- Formas de pago (ComboBox tipo de pago)
INSERT INTO Proceso (nombre, tipo_catalogo, tipo_movimiento, orden) VALUES
('Efectivo ventanilla',   'FORMA_PAGO', 'AMBOS', 1),
('Cheque',                'FORMA_PAGO', 'AMBOS', 2),
('Transferencia',         'FORMA_PAGO', 'AMBOS', 3);

-- ============================================================
-- 9. PROCEDIMIENTO: REGISTRAR CAMBIO DE DIVISA
--     Genera automáticamente los 2 movimientos en Transacciones
-- ============================================================

GO
CREATE OR ALTER PROCEDURE sp_RegistrarCambioDivisa
    @apertura_id        INT,
    @usuario_id         INT,
    @tipo               VARCHAR(10),      -- COMPRA | VENTA
    @moneda_origen_id   INT,
    @monto_origen       DECIMAL(18,2),
    @moneda_destino_id  INT,
    @monto_destino      DECIMAL(18,2),
    @tasa_cambio        DECIMAL(18,6),
    @descripcion        VARCHAR(255) = NULL,
    @cambio_id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1 FROM AperturaCaja
            WHERE apertura_id = @apertura_id AND estado = 'ABIERTA'
        )
            THROW 50001, 'No existe una caja abierta para esta apertura.', 1;

        INSERT INTO CambioDivisa (
            apertura_id, usuario_id, tipo,
            moneda_origen_id, monto_origen,
            moneda_destino_id, monto_destino,
            tasa_cambio, descripcion
        )
        VALUES (
            @apertura_id, @usuario_id, @tipo,
            @moneda_origen_id, @monto_origen,
            @moneda_destino_id, @monto_destino,
            @tasa_cambio, @descripcion
        );

        SET @cambio_id = SCOPE_IDENTITY();

        -- Egreso: sale la moneda que entrega el cliente
        INSERT INTO Transacciones (
            apertura_id, usuario_id, moneda_id, cambio_id,
            tipo, monto, descripcion, referencia
        )
        VALUES (
            @apertura_id, @usuario_id, @moneda_origen_id, @cambio_id,
            'EGRESO', @monto_origen,
            ISNULL(@descripcion, 'Cambio de divisa - egreso'),
            CONCAT('CAMBIO-', @cambio_id)
        );

        -- Ingreso: entra la moneda que recibe el cliente
        INSERT INTO Transacciones (
            apertura_id, usuario_id, moneda_id, cambio_id,
            tipo, monto, descripcion, referencia
        )
        VALUES (
            @apertura_id, @usuario_id, @moneda_destino_id, @cambio_id,
            'INGRESO', @monto_destino,
            ISNULL(@descripcion, 'Cambio de divisa - ingreso'),
            CONCAT('CAMBIO-', @cambio_id)
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================================
-- 10. PROCEDIMIENTO: CERRAR CAJA
-- ============================================================

GO
CREATE OR ALTER PROCEDURE sp_CerrarCaja
    @apertura_id    INT,
    @usuario_id     INT,
    @observaciones  VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1 FROM AperturaCaja
            WHERE apertura_id = @apertura_id AND estado = 'ABIERTA'
        )
            THROW 50002, 'La caja no está abierta o no existe.', 1;

        IF EXISTS (SELECT 1 FROM CierreCaja WHERE apertura_id = @apertura_id)
            THROW 50003, 'Esta apertura ya fue cerrada.', 1;

        DECLARE @cierre_id INT;

        INSERT INTO CierreCaja (apertura_id, usuario_id, observaciones)
        VALUES (@apertura_id, @usuario_id, @observaciones);

        SET @cierre_id = SCOPE_IDENTITY();

        INSERT INTO CierreCajaDetalle (cierre_id, moneda_id, monto_sistema, monto_contado)
        SELECT
            @cierre_id,
            v.moneda_id,
            v.saldo_sistema,
            v.saldo_sistema   -- Por defecto igual; la app puede actualizar monto_contado
        FROM vw_SaldoPorMonedaApertura v
        WHERE v.apertura_id = @apertura_id
          AND (v.monto_inicial > 0 OR v.total_ingresos > 0 OR v.total_egresos > 0);

        UPDATE AperturaCaja
        SET estado = 'CERRADA'
        WHERE apertura_id = @apertura_id;

        COMMIT TRANSACTION;

        SELECT @cierre_id AS cierre_id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================================
-- 11. EJEMPLO DE USO (opcional, comentar en producción)
-- ============================================================

/*
DECLARE @apertura_id INT = 1;
DECLARE @cambio_id   INT;

-- 1) Abrir caja
INSERT INTO AperturaCaja (usuario_id, observaciones)
VALUES (1, 'Apertura turno mañana');
SET @apertura_id = SCOPE_IDENTITY();

INSERT INTO AperturaCajaDetalle (apertura_id, moneda_id, monto_inicial)
VALUES (@apertura_id, 1, 500.00),   -- PEN
       (@apertura_id, 2, 100.00);   -- USD

-- 2) Ingreso con cliente, operación y forma de pago
INSERT INTO Transacciones (
    apertura_id, usuario_id, cliente_id, moneda_id,
    proceso_id, forma_pago_id, categoria_id, tipo, monto, descripcion, referencia
)
VALUES (
    @apertura_id, 1, 2, 1,
    1, 1, 1, 'INGRESO', 150.00,
    'Depósito Banpro cliente Juan Pérez', 'REC-001'
);

-- 3) Cambio: cliente vende USD y recibe PEN
EXEC sp_RegistrarCambioDivisa
    @apertura_id       = @apertura_id,
    @usuario_id        = 1,
    @tipo              = 'COMPRA',
    @moneda_origen_id  = 2,      -- USD entrega el cliente
    @monto_origen      = 50.00,
    @moneda_destino_id = 1,      -- PEN recibe el cliente
    @monto_destino     = 185.00,
    @tasa_cambio       = 3.7000,
    @descripcion       = 'Compra de dólares',
    @cambio_id         = @cambio_id OUTPUT;

-- 4) Consultar saldos
SELECT * FROM vw_SaldoPorMonedaApertura WHERE apertura_id = @apertura_id;

-- 5) Cerrar caja
EXEC sp_CerrarCaja @apertura_id = @apertura_id, @usuario_id = 1;
*/

PRINT 'Base de datos SistemaCaja creada correctamente.';
GO
