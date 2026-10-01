# 🏡 Inmobiliaria: Pereyra, Elías, Peláez & Lagos
> **TP integral ULP 2026** | 📞 Contacto: 2664884044

Sistema integral de gestión de alquileres temporales desarrollado en ASP.NET Core MVC. Permite la administración de propietarios, inmuebles, inquilinos, reservas y pagos, con un estricto control de roles y disponibilidad.
LO MAS IMPORTANTE QUE ME GUSTARIA DESTACRA ES LO SIGUIENTE:
- Un inmueble se crea solo desde el propietario, y desde ver inmuebles en el index de propietario. Llegando desde este lugar se pueden listar informes de unicamente ESE propietario.
- Las reservas se pueden eliminar por un Admin pero estas quedan registradas pero sin interactivilidad
- Los usuarios solo se pueden crear DESDE GESTION USUARIOS, siendo solo accesible por el ADMIN.
- El comando dotnet watch se crashea cuando se sube una imagen y no supe como solucionarlo
- Los detalles de las entidades Reserva y Pago permanecen ocultos para cualquier empleado, al igual que el boton eliminar en todas las entidades...
- Los demas detalles de las demas entidades supuse que son demasiado importantes para el empleado poder verlas, asique los deje a su disposicion.
---

<details>
  <summary><strong>🚀 Características Principales</strong></summary>
  <br>

  * **Gestión de Usuarios y Roles:** Autenticación por cookies. Roles de *Administrador* (acceso total, eliminación, anulación) y *Empleado* (gestión operativa).
  * **Administración de Entidades (CRUD):**
    * **Propietarios:** Dueños de los inmuebles.
    * **Inquilinos:** Clientes que alquilan.
    * **Tipos de Inmuebles:** Categorización (Casa, Depto, etc.).
    * **Inmuebles:** Capacidad, coordenadas, precio por día y estado.
  * **Sistema de Reservas:**
    * Asignación por rangos de fechas.
    * Validación automática de solapamiento para evitar doble reserva.
    * Auditoría del empleado que generó el contrato.
  * **Control de Pagos:** Registro de transacciones y baja lógica (Baja) exclusiva para administradores, y anulacion normal disponible para Usuarios.
  * **Interfaz Gráfica:** Bootstrap 5, soporte Claro/Oscuro.

</details>

<details>
  <summary><strong>🛠️ Tecnologías Utilizadas</strong></summary>
  <br>

  * **Backend:** C# con .NET 10.0 (ASP.NET Core MVC).
  * **Base de Datos:** MySQL / MariaDB (ADO.NET puro vía MySqlConnector).
  * **Patrón de Diseño:** Modelo-Vista-Controlador (MVC) y Patrón Repositorio.
  * **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 5, Bootstrap Icons.

</details>

<details>
  <summary><strong>🔐 Acceso al Sistema (LO MAS IMPORTANTE)</strong></summary>
  <br>
  
  **El script SQL genera un usuario administrador por defecto para probar el sistema inmediatamente:**
  * Email: admin@inmobiliaria.com
  * Clave: admin123
  * Rol: Administrador
</details>

👥 Equipo de Desarrollo
Pereyra

Elías

Peláez Víctor

Lagos Maximiliano

<img width="5198" height="6050" alt="Inmueble Herencia Ecosystem-2026-08-20-202152" src="https://github.com/user-attachments/assets/e0a514e3-ca61-4f2a-b644-1cb861286802" />

