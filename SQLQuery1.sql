create database SistemaCaja
go
use SistemaCaja
go

-- Rol
CREATE TABLE Rol (
    IdRol  int primary key identity(1,1),
    Descripcion VARCHAR(100),
	FechaCreacion datetime default getdate(),

);

-- Usuarios
CREATE TABLE Usuarios (
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
CREATE TABLE Permisos (
    idPermiso int primary key identity(1,1),
	IdRol int references Rol(IdRol),
	NombreMenu varchar(100),
	FechaCreacion datetime default getdate(),

	);

-- Transacciones (con descripción)
CREATE TABLE Transacciones (
    transaccion_id int primary key identity(1,1),
    usuario_id int,
	cambio_id INT,
    tipo_transaccion varchar(50),--entrada o salida
    moneda varchar (50),-- dolar o corboda
    monto DECIMAL(10, 2),
    descripcion VARCHAR(255),
    fecha_hora datetime default getdate(),
	FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)

);

CREATE TABLE AperturasCaja (
    apertura_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT,
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_inicial DECIMAL(10, 2),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

CREATE TABLE CierresCaja (
    cierre_id INT PRIMARY KEY IDENTITY(1,1),
    usuario_id INT,
    fecha_hora DATETIME DEFAULT GETDATE(),
    monto_final DECIMAL(10, 2),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);


-- Cambio de Divisas (con descripción)
CREATE TABLE CambioDivisas (
    cambio_id INT PRIMARY KEY identity(1,1),
    usuario_id INT,
    tipo varchar (50),--VENTA O COMPRA--
    moneda varchar (50),--DOLAR O CORDOBA
    monto DECIMAL(10, 2),
    tasa_cambio DECIMAL(10, 4),
    descripcion VARCHAR(255),
    fecha_hora datetime default getdate(),
    FOREIGN KEY (usuario_id) REFERENCES Usuarios(usuario_id)
);

-- Historial de Transacciones
CREATE TABLE HistorialTransacciones (
    historial_id INT PRIMARY KEY,
    transaccion_id int,
    accion varchar(50),
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (transaccion_id) REFERENCES Transacciones(transaccion_id)
);
