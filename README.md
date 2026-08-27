<div align="center">

# 🏢 Innova - Sistema de Gestión Inmobiliaria

<p align="center">
  Sistema de informatización para la gestión integral de alquileres temporarios de propiedades inmuebles realizados por una agencia inmobiliaria.
</p>

</div>

---

## 👥 Integrantes del Equipo (Grupo 22)

| Nombre y Apellido | Correo Electrónico | GitHub | Discord |
| :--- | :--- | :--- | :--- |
| 🧑‍💻 **Lucas Zarate** | `maxolucas@gmail.com` | [@Maxorgz](https://github.com/Maxorgz) | `maxy_1604` |
| 🧑‍💻 **Juan Genero** | `juanignaciogenero@gmail.com` | [@Juani-ds](https://github.com/Juani-ds) | `juanistratos` |
| 🧑‍💻 **Daniel Rodriguez** | `danielrodriguez45b@gmail.com` | [@Danirodriguez45B](https://github.com/Danirodriguez45B) | `dani45gsr` |

---

## 🛠️ Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de contar con el siguiente software instalado:

* 🔹 **[.NET SDK](https://dotnet.microsoft.com/download)** `v8.0` o superior.
* 🔹 **[DBeaver](https://dbeaver.io/)** (para la administración visual de la base de datos).

---

## 🗄️ Configuración e Instalación de la Base de Datos

1. **Iniciar el servicio de Base de Datos:**
   * Asegúrate de tener tu servidor MySQL o MariaDB corriendo de forma local (o el servicio correspondiente activo en tu equipo).

2. **Ejecutar el Script SQL en DBeaver:**
   * Abre **DBeaver** y conéctate a tu servidor local de MySQL.
   * Abre o importa el archivo `.sql` ubicado en la carpeta del proyecto: `./data/Script.sql`.
   * Ejecuta el script completo para crear la base de datos y sus tablas de forma automática.

3. **Solución de problemas de conexión (Permisos de usuario):**
   * Si llegase a haber algún problema con la conexión de la BD respecto a los permisos del servidor, vistas o el nombre de tu ordenador, puedes ejecutar el siguiente script en una consola SQL de DBeaver:
     ```sql
     GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' IDENTIFIED BY 'TU_CLAVE_AQUI';
     FLUSH PRIVILEGES;
     ```

---

## 🚀 Guía de Instalación y Ejecución

1. **Clonar el proyecto:**
   * Desde la terminal utilizando Git:
     ```bash
     git clone [https://github.com/Maxorgz/inmobiliariaGrupo22ULP.git](https://github.com/Maxorgz/inmobiliariaGrupo22ULP.git)
     ```
   * O bien utilizando GitHub Desktop.

2. **Instalar dependencias necesarias:**
   * Ejecuta el siguiente comando en la terminal dentro de la carpeta del proyecto para habilitar el conector de SQL:
     ```bash
     dotnet add package MySqlConnector
     ```

3. **Iniciar la aplicación:**
   * Asegurate de tener la base de datos lista y activa.
   * Ejecuta el comando de compilación y ejecución:
     ```bash
     dotnet run
     ```
   * En la consola aparecera la linea `Now listening on: ...`. Manten presionado `Ctrl` y haz clic en el enlace (ej: `http://localhost:...`) para abrir la aplicación en tu navegador.

---

## 📐 Modelado de Datos

A continuación se presenta el Diagrama Entidad-Relación (DER) / Diagrama de Clases correspondiente a la aplicación:

![Diagrama del Proyecto](./diagrama/inmobiliariagrupo22.png)