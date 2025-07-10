create database SistemaCaja
go
use SistemaCaja
go

-- Tabla de Roles
CREATE TABLE Roles (
    IdRol INT PRIMARY KEY IDENTITY(1,1),
    Descripcion VARCHAR(100),
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Tabla de Usuarios
CREATE TABLE Usuario (
    usuario_id INT PRIMARY KEY IDENTITY(1,1),
    NombreCompleto VARCHAR(100) NOT NULL,
    Correo VARCHAR(100),
    usuario VARCHAR(50) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    IdRol INT NOT NULL FOREIGN KEY REFERENCES Roles(IdRol),
    estado BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Tabla de Menús
CREATE TABLE Menu (
    MenuId INT PRIMARY KEY IDENTITY(1,1),
    NombreMenu VARCHAR(100) NOT NULL,
    Ruta VARCHAR(255),
    Icono VARCHAR(100),
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Tabla de Permisos
CREATE TABLE Permiso (
    idPermiso INT PRIMARY KEY IDENTITY(1,1),
    IdRol INT FOREIGN KEY REFERENCES Roles(IdRol),
    MenuId INT FOREIGN KEY REFERENCES Menu(MenuId),
    Accion VARCHAR(50) DEFAULT 'Acceso',
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Tabla de Monedas
CREATE TABLE Moneda (
    moneda_id INT PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(50) NOT NULL,
    simbolo VARCHAR(10),
    estado BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE()
);

-- Tabla de Cambio de Divisas
CREATE TABLE CambioDivisa (
    cambio_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT FOREIGN KEY REFERENCES Usuario(usuario_id),
    tipo VARCHAR(50) CHECK (tipo IN ('VENTA', 'COMPRA')),
    moneda_id INT FOREIGN KEY REFERENCES Moneda(moneda_id),
    monto DECIMAL(10,2),
    tasa_cambio DECIMAL(10,4),
    descripcion VARCHAR(255),
    estado BIT DEFAULT 1,
    fecha_hora DATETIME DEFAULT GETDATE()
);

-- Tabla de Transacciones 
CREATE TABLE Transacciones (
    transaccion_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT FOREIGN KEY REFERENCES Usuario(usuario_id),
    moneda_id INT FOREIGN KEY REFERENCES Moneda(moneda_id),
    monto_entrada DECIMAL(10,2),
    monto_cobrado DECIMAL(10,2),
    monto_salida DECIMAL(10,2), 
    descripcion VARCHAR(255),
    referencia VARCHAR(100),
    fecha_hora DATETIME DEFAULT GETDATE(),
    estado BIT DEFAULT 1,
    usuario_modifico INT NULL FOREIGN KEY REFERENCES Usuario(usuario_id),
    fecha_modificacion DATETIME NULL
);

-- Tabla de Apertura de Caja
CREATE TABLE AperturaCaja (
    apertura_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT FOREIGN KEY REFERENCES Usuario(usuario_id),
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_inicial DECIMAL(10,2),
    observaciones VARCHAR(255)
);

-- Tabla de Cierre de Caja
CREATE TABLE CierreCaja (
    cierre_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT FOREIGN KEY REFERENCES Usuario(usuario_id),
    apertura_id INT FOREIGN KEY REFERENCES AperturaCaja(apertura_id),
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_final DECIMAL(10,2),
    observaciones VARCHAR(255)
);


INSERT INTO Usuario (NombreCompleto, correo, usuario, password_hash, IdRol, estado)
VALUES ('Administrador','kenrrichg@gmail.com', 'admin', '12345', 1, 1);

INSERT INTO Roles (Descripcion)
VALUES ('Administrador');

INSERT INTO Roles (Descripcion)
VALUES ('Cajero');

INSERT INTO Roles (Descripcion)
VALUES ('Supervisor');


select * from Usuario;
DELETE FROM Usuario;




