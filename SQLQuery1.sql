create database SistemaCaja
go
use SistemaCaja
go

-- Rol
CREATE TABLE Roles (
    IdRol  int primary key identity(1,1),
    Descripcion VARCHAR(100),
	FechaCreacion datetime default getdate(),

);

-- Usuarios
CREATE TABLE Usuario (
    usuario_id int primary key identity(1,1),
    NombreCompleto varchar(100),
    Correo varchar(100),
    usuario VARCHAR(50),
    contraseña varchar(100),
	IdRol int references Rol(IdRol),
	estado bit,
	FechaCreacion datetime default getdate(),
);


-- Permisos
CREATE TABLE Permiso (
    idPermiso int primary key identity(1,1),
	IdRol int references Rol(IdRol),
	NombreMenu varchar(100),
	FechaCreacion datetime default getdate(),

	);
CREATE TABLE CambioDivisa (
    cambio_id INT primary key IDENTITY(1,1),
    usuario_id INT,
    tipo varchar (50), -- VENTA O COMPRA
    moneda varchar (50), -- DÓLAR O CÓRDOBA
    monto DECIMAL(10, 2),
    tasa_cambio DECIMAL(10, 4),
    descripcion VARCHAR(255),
    fecha_hora datetime DEFAULT GETDATE(),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

CREATE TABLE Transaccione (
    transaccion_id int PRIMARY KEY IDENTITY(1,1),
    usuario_id int,
    cambio_id INT REFERENCES CambioDivisa(cambio_id), -- Puede ser NULL si la transacción no es un cambio de divisas
    tipo_transaccion varchar(50), -- entrada o salida
    moneda varchar (50), -- dólar o córdoba
    monto DECIMAL(10, 2),
    descripcion VARCHAR(255),
    fecha_hora datetime DEFAULT GETDATE(),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id),
    
); 

CREATE TABLE AperturaCaja (
    apertura_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT,
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_inicial DECIMAL(10, 2),
    FOREIGN KEY (usuario_id) REFERENCES Usuario(usuario_id)
);

CREATE TABLE CierreCaja (
    cierre_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT,
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_final DECIMAL(10, 2),
    FOREIGN KEY (usuario_id) REFERENCES Usuario(usuario_id)
);


-- Historial de Transacciones
CREATE TABLE HistorialTransaccione (
    historial_id INT PRIMARY KEY,
    transaccion_id int,
    accion varchar(50),
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (transaccion_id) REFERENCES Transacciones(transaccion_id)
);
