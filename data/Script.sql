CREATE DATABASE IF NOT EXISTS inmobiliariagrupo22;
USE inmobiliariagrupo22;

CREATE TABLE IF NOT EXISTS Propietario (
    IdPropietario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Dni VARCHAR(20) NOT NULL,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL,
    Clave VARCHAR(255),
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Inquilino (
    IdInquilino INT AUTO_INCREMENT PRIMARY KEY,
    Dni VARCHAR(20) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Telefono VARCHAR(20),
    Email VARCHAR(100) NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS TipoInmueble (
    IdTipoInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Descripcion VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS Inmueble (
    IdInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Direccion VARCHAR(150) NOT NULL,
    Cupo INT NOT NULL,
    IdTipoInmueble INT NOT NULL,
    Latitud DECIMAL(10,7),
    Longitud DECIMAL(10,7),
    PrecioPorDia DECIMAL(10,2) NOT NULL,
    PorcentajeReserva DECIMAL(5,2) NOT NULL DEFAULT 30.00,
    -- ImagenPortada VARCHAR(255),
    IdPropietario INT NOT NULL,
    Disponible BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (IdTipoInmueble) REFERENCES TipoInmueble(IdTipoInmueble),
    FOREIGN KEY (IdPropietario) REFERENCES Propietario(IdPropietario)
);

CREATE TABLE IF NOT EXISTS Reserva (
    IdReserva INT AUTO_INCREMENT PRIMARY KEY,
    IdInquilino INT NOT NULL,
    IdInmueble INT NOT NULL,
    FechaDesde DATE NOT NULL,
    FechaHasta DATE NOT NULL,
    FechaHastaOriginal DATE NOT NULL,
    FechaTerminacionAnticipada DATE NULL,
    Multa DECIMAL(10,2) NULL,
    FOREIGN KEY (IdInquilino) REFERENCES Inquilino(IdInquilino),
    FOREIGN KEY (IdInmueble) REFERENCES Inmueble(IdInmueble)
);

CREATE TABLE IF NOT EXISTS Usuario (
    IdUsuario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Clave VARCHAR(255) NOT NULL,
    Avatar VARCHAR(255) NULL,
    Rol INT NOT NULL DEFAULT 2 -- 1: Administrador, 2: Empleado
);

INSERT INTO Usuario (Nombre, Apellido, Email, Clave, Rol)
VALUES ('Admin', 'Principal', 'admin@inmobiliaria.com', 'admin123', 1)
ON DUPLICATE KEY UPDATE Email = Email;

CREATE TABLE IF NOT EXISTS Pago (
    IdPago INT AUTO_INCREMENT PRIMARY KEY,
    IdReserva INT NOT NULL,
    Concepto VARCHAR(255) NOT NULL,
    FechaPago DATETIME NOT NULL,
    Importe DECIMAL(10,2) NOT NULL,
    Estado TINYINT(1) NOT NULL DEFAULT 1,
    IdUsuarioCreador INT NOT NULL,
    IdUsuarioAnulador INT NULL,
    FOREIGN KEY (IdReserva) REFERENCES Reserva(IdReserva),
    FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (IdUsuarioAnulador) REFERENCES Usuario(IdUsuario)
);