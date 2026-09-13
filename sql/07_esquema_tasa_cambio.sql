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
