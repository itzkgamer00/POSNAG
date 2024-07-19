create database Sistemacaja;
go

use Sistemacaja;
go

-- Crear tabla Usuarios
CREATE TABLE Usuarios (
    usuario_id INT PRIMARY KEY IDENTITY,
    nombre VARCHAR(100),
    usuario VARCHAR(50) UNIQUE,
    contraseña VARCHAR(100),
    rol NVARCHAR(50) CHECK (rol IN ('ADMINISTRADOR', 'CAJERO'))
);

-- Crear tabla Permisos
CREATE TABLE Permisos (
    permiso_id INT PRIMARY KEY IDENTITY,
    nombre VARCHAR(100) UNIQUE
);

-- Crear tabla Relación entre Usuarios y Permisos
CREATE TABLE UsuarioPermisos (
    usuario_id INT,
    permiso_id INT,
    PRIMARY KEY (usuario_id, permiso_id),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id),
    FOREIGN KEY (permiso_id) REFERENCES Permisos(permiso_id)
);

-- Crear tabla Transacciones
CREATE TABLE Transacciones (
    transaccion_id INT PRIMARY KEY IDENTITY,
    usuario_id INT,
    tipo_transaccion NVARCHAR(50) CHECK (tipo_transaccion IN ('ENTRADA', 'SALIDA')),
    moneda NVARCHAR(50) CHECK (moneda IN ('DOLARES', 'CORDOBAS')),
    monto DECIMAL(10, 2),
    descripcion NVARCHAR(255),
    fecha_hora DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

-- Crear tabla Apertura y Cierre de Caja
CREATE TABLE AperturaCierreCaja (
    caja_id INT PRIMARY KEY IDENTITY,
    usuario_id INT,
    tipo NVARCHAR(50) CHECK (tipo IN ('APERTURA', 'CIERRE')),
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_inicial DECIMAL(10, 2),
    monto_final DECIMAL(10, 2),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

-- Crear tabla Cambio de Divisas
CREATE TABLE CambioDivisas (
    cambio_id INT PRIMARY KEY IDENTITY,
    usuario_id INT,
    tipo NVARCHAR(50) CHECK (tipo IN ('COMPRA', 'VENTA')),
    moneda NVARCHAR(50) CHECK (moneda IN ('DOLARES', 'CORDOBAS')),
    monto DECIMAL(10, 2),
    tasa_cambio DECIMAL(10, 4),
    descripcion NVARCHAR(255),
    fecha_hora DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

-- Crear tabla Historial de Transacciones
CREATE TABLE HistorialTransacciones (
    historial_id INT PRIMARY KEY IDENTITY,
    transaccion_id INT,
    accion NVARCHAR(50) CHECK (accion IN ('CREAR', 'ANULAR')),
    fecha_hora DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (transaccion_id) REFERENCES Transacciones(transaccion_id)
);
