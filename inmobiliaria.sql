-- CREACION BASE DE DATOS
CREATE DATABASE IF NOT EXISTS inmobiliaria_db;
USE inmobiliaria_db;

-- TABLA PROPIETARIO
CREATE TABLE IF NOT EXISTS Propietario (
    IdPropietario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(100) NOT NULL UNIQUE
);

-- TABLA INQUILINO
CREATE TABLE IF NOT EXISTS Inquilino (
    IdInquilino INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Dni VARCHAR(20) NOT NULL UNIQUE,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(100) NOT NULL UNIQUE
);

-- TABLA USUARIO
CREATE TABLE IF NOT EXISTS Usuario (
    IdUsuario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Clave VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL,
    Avatar VARCHAR(255) NULL 
);

-- TABLA TIPO INMUEBLE 
CREATE TABLE IF NOT EXISTS TipoInmueble (
    IdTipoInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL
);

-- TABLA INMUEBLE 
CREATE TABLE IF NOT EXISTS Inmueble (
    IdInmueble INT AUTO_INCREMENT PRIMARY KEY,
    IdPropietario INT NOT NULL,
    IdTipoInmueble INT NOT NULL,
    Direccion VARCHAR(255) NOT NULL,
    Cupo INT NOT NULL,
    Coordenadas VARCHAR(100) NULL,
    PrecioPorDia DECIMAL(10,2) NOT NULL,
    PorcentajeReserva DECIMAL(5,2) NOT NULL,
    Estado VARCHAR(50) DEFAULT 'Disponible',
    ImagenPortada VARCHAR(255) NULL,
    FOREIGN KEY (IdPropietario) REFERENCES Propietario(IdPropietario) ON DELETE RESTRICT,
    FOREIGN KEY (IdTipoInmueble) REFERENCES TipoInmueble(IdTipoInmueble) ON DELETE RESTRICT
);

-- TABLA RESERVA
CREATE TABLE IF NOT EXISTS Reserva (
    IdReserva INT AUTO_INCREMENT PRIMARY KEY,
    IdInmueble INT NOT NULL,
    IdInquilino INT NOT NULL,
    FechaDesde DATE NOT NULL,
    FechaHasta DATE NOT NULL,
    MontoPorDia DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(50) DEFAULT 'Vigente',
    FechaEfectivaTerminacion DATE NULL,
    IdUsuarioCreador INT NOT NULL, 
    IdUsuarioTerminador INT NULL,  
    FOREIGN KEY (IdInmueble) REFERENCES Inmueble(IdInmueble) ON DELETE RESTRICT,
    FOREIGN KEY (IdInquilino) REFERENCES Inquilino(IdInquilino) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioTerminador) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT
);

-- TABLA PAGO 
CREATE TABLE IF NOT EXISTS Pago (
    IdPago INT AUTO_INCREMENT PRIMARY KEY,
    IdReserva INT NOT NULL,
    Concepto VARCHAR(255) NOT NULL,
    FechaPago DATE NOT NULL,
    Importe DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(50) DEFAULT 'Activo', 
    IdUsuarioCreador INT NOT NULL,       
    IdUsuarioAnulador INT NULL,         
    FOREIGN KEY (IdReserva) REFERENCES Reserva(IdReserva) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioAnulador) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT
);

-- DATOS DE PRUEBA
INSERT INTO Propietario (Nombre, Apellido, Dni, Telefono, Email) VALUES
('Juan', 'Perez', '11222333', '2664111111', 'juan.perez@email.com'),
('Maria', 'Gomez', '44555666', '2664222222', 'maria.gomez@email.com'),
('Carlos', 'Lopez', '77888999', '2664333333', 'carlos.lopez@email.com');

INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono, Email) VALUES
('Ana', 'Martinez', '12345678', '2664444444', 'ana.martinez@email.com'),
('Pedro', 'Rodriguez', '87654321', '2664555555', 'pedro.rodriguez@email.com');

-- Usuario administrador
INSERT INTO Usuario (Nombre, Apellido, Email, Clave, Rol) VALUES
('Admin', 'Principal', 'admin@inmobiliaria.com', 'admin123', 'Administrador');

-- Tipos de inmueble iniciales
INSERT INTO TipoInmueble (Nombre) VALUES
('Casa'),
('Departamento'),
('Monoambiente'),