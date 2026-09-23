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

CREATE TABLE Inmueble (
    IdInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Direccion VARCHAR(255) NOT NULL,
    Cupo INT NOT NULL,
    Latitud DOUBLE NOT NULL,
    Longitud DOUBLE NOT NULL,
    PrecioPorDia DECIMAL(18,2) NOT NULL,
    PorcentajeSeña DECIMAL(5,2) NOT NULL DEFAULT 0,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,
    ImagenPortada VARCHAR(255),
    IdTipoInmueble INT NOT NULL,
    IdPropietario INT NOT NULL,
    FOREIGN KEY (IdTipoInmueble) REFERENCES TipoInmueble(IdTipoInmueble),
    FOREIGN KEY (IdPropietario) REFERENCES Propietario(IdPropietario)
);

-- TABLA RESERVA
CREATE TABLE IF NOT EXISTS Reserva (
    IdReserva INT AUTO_INCREMENT PRIMARY KEY,
    IdInmueble INT NOT NULL,
    IdInquilino INT NOT NULL,
    FechaDesde DATE NOT NULL,
    FechaHasta DATE NOT NULL,
    MontoPorDia DECIMAL(18,2) NOT NULL,
    Multa DECIMAL(18,2) NULL,
    FechaEfectivaTerminacion DATETIME NULL,
    FechaRealTerminacion DATETIME NULL,
    Estado INT NOT NULL DEFAULT 1,
    IdUsuarioCreador INT NOT NULL, 
    IdUsuarioTerminador INT NULL,
    IdUsuarioTerminacion INT NULL,  
    FOREIGN KEY (IdInmueble) REFERENCES Inmueble(IdInmueble) ON DELETE RESTRICT,
    FOREIGN KEY (IdInquilino) REFERENCES Inquilino(IdInquilino) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioCreador) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioTerminador) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT,
    FOREIGN KEY (IdUsuarioTerminacion) REFERENCES Usuario(IdUsuario) ON DELETE RESTRICT
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
('Admin', 'Principal', 'admin@inmobiliaria.com', 'QRtmeYlpSnMaa/AAbPiCFCABqFvDw5iujZqs9xROCU4=', 'Administrador');

INSERT INTO TipoInmueble (Nombre) VALUES
('Casa'),
('Departamento'),
('Monoambiente');

INSERT INTO Inmueble (Direccion, Cupo, Latitud, Longitud, PrecioPorDia, PorcentajeSeña, Estado, ImagenPortada, IdTipoInmueble, IdPropietario) VALUES
('Calle Falsa 123', 3, 120, 120, 1000, 0.2, TRUE, 'casa.jpg', 1, 1),
('Avenida Falsa 456', 2, 80, 80, 6000, 0.15, TRUE, 'departamento.jpg', 2, 2),
('Calle Falsa 789', 1, 50, 50, 3000, 0.1, TRUE, 'monoambiente.jpg', 3, 3);