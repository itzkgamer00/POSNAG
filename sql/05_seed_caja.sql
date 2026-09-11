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
