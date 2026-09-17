# Inmobiliaria-Pereyra-Elias-Pelaez-Victor-Lagos-Maximiliano
TP integral ULP 2026.

[!NOTE]
NUMERO DE CONTACTO PARA EL INFORME DE REVISION CRUZADA: 2664884044

🚀 Características Principales
Gestión de Usuarios y Roles: Autenticación basada en cookies. Dos roles definidos (Administrador y Empleado) con restricciones de acceso a las distintas vistas y acciones (por ejemplo, solo el administrador puede eliminar registros o anular pagos).

Administración de Entidades Básicas (ABM/CRUD):

Propietarios: Registro y gestión de los dueños de los inmuebles.

Inquilinos: Registro de los clientes que alquilan las propiedades.

Tipos de Inmuebles: Categorización de propiedades (Casa, Departamento, Monoambiente, etc.).

Gestión de Inmuebles: Registro de propiedades asociadas a un dueño y un tipo, incluyendo capacidad (cupo), coordenadas (latitud/longitud), precio por día y estado de disponibilidad.

Sistema de Reservas Temporales:

Asignación de un inquilino a un inmueble por un rango de fechas.

Cálculo automático de la disponibilidad (validación contra solapamiento de fechas para evitar doble reserva).

Registro del empleado que generó la reserva.

Control de Pagos:

Registro de transacciones económicas asociadas a cada reserva.

Sistema de anulación (baja lógica) exclusivo para administradores, guardando auditoría de quién anuló el pago.

Interfaz Gráfica: Diseño responsivo utilizando Bootstrap 5, con soporte para modo Claro/Oscuro (Dark Mode).

🛠️ Tecnologías Utilizadas
Framework: .NET 10.0 (ASP.NET Core MVC)

Lenguaje: C#

Base de Datos: MySQL / MariaDB

Acceso a Datos: ADO.NET puro a través de MySqlConnector (Patrón Repositorio)

Frontend: HTML5, CSS3, JavaScript, Bootstrap 5, Bootstrap Icons

Arquitectura: Modelo-Vista-Controlador (MVC)

⚙️ Requisitos Previos
.NET SDK 10.0 o superior.

Servidor MySQL o MariaDB corriendo localmente o en un servidor remoto.

Visual Studio 2022, VS Code, o Rider.

🔧 Instalación y Configuración
1. Clonar el repositorio

Bash
git clone <URL_DEL_REPOSITORIO>
cd Inmobiliaria_Pereyra_Elias_Pelaez_Victor_Lagos_Maximiliano
2. Configurar la Base de Datos

Abre tu gestor de base de datos preferido (phpMyAdmin, DBeaver, MySQL Workbench, etc.).

Ejecuta el script SQL incluido en el proyecto (inmobiliaria.sql) para crear la base de datos inmobiliaria_db, sus tablas y cargar los datos de prueba iniciales.

3. Configurar la cadena de conexión

Abre el archivo appsettings.json.

Verifica que los credenciales de DefaultConnection coincidan con la configuración de tu servidor MySQL local (usuario, contraseña y puerto):

JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost; port=3306; Database=inmobiliaria_db; User ID=root; Password=;"
}
4. Ejecutar el proyecto

Desde la terminal, en la raíz del proyecto, ejecuta:

Bash
dotnet build
dotnet run
El sistema estará disponible típicamente en http://localhost:5000 o https://localhost:5001.

🔐 Acceso al Sistema
El script SQL genera un usuario administrador por defecto. Sin embargo, como el sistema utiliza hashing para las contraseñas, se recomienda registrar el primer usuario desde la pantalla de "Registrarse" en el inicio de la aplicación para que la contraseña se encripte correctamente en la base de datos.

Una vez creado tu primer usuario (que por defecto se creará con el rol Empleado), puedes cambiarle el rol a Administrador directamente en la base de datos mediante la siguiente consulta SQL para tener acceso total:

SQL
UPDATE Usuario SET Rol = 'Administrador' WHERE Email = 'tu_correo@ejemplo.com';
👥 Equipo de Desarrollo
Pereyra

Elías

Peláez Víctor

Lagos Maximiliano

<img width="5198" height="6050" alt="Inmueble Herencia Ecosystem-2026-08-20-202152" src="https://github.com/user-attachments/assets/e0a514e3-ca61-4f2a-b644-1cb861286802" />
