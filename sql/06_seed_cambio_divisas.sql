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
