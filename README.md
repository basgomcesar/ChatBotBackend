# ChatBotBackend

¡Bienvenido a ChatBotBackend! Este proyecto está desarrollado completamente en C# y contiene un backend que se encarga de gestionar un chatbot. A continuación, encontrarás las instrucciones para configurar y levantar el proyecto, así como los pasos necesarios para realizar migraciones hacia la base de datos.

## Prerrequisitos

Antes de comenzar, asegúrate de tener instalados los siguientes programas:
- [.NET SDK](https://dotnet.microsoft.com/download) (versión compatible con el proyecto)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) o cualquier sistema de base de datos compatible con Entity Framework Core
- [Redis](https://redis.io/docs/getting-started/) para la gestión de caché.
- Un editor de texto o IDE, como [Visual Studio](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)

## Instalación

1. **Clona el repositorio**:
   ```bash
   git clone https://github.com/basgomcesar/ChatBotBackend.git
   cd ChatBotBackend
   ```

2. **Restaura las dependencias del proyecto**:
   Ejecuta el siguiente comando para restaurar las dependencias y los paquetes NuGet:
   ```bash
   dotnet restore
   ```

3. **Configura la base de datos**:
   Edita el archivo `appsettings.json` y configura la cadena de conexión a tu base de datos:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR;Database=ChatBotDb;User Id=TU_USUARIO;Password=TU_CONTRASEÑA;"
   }
   ```

4. **Configura la conexión a Redis**:
   Redis se utiliza en este proyecto para la gestión de caché y otras funcionalidades que dependen del almacenamiento en memoria. La configuración básica se encuentra en el archivo `appsettings.json`:
   ```json
   "ConnectionStrings": {
       "Redis": "localhost:6379"
   }
   ```
   Asegúrate de que tu servidor Redis esté corriendo en la dirección y puerto especificados (`localhost:6379` por defecto). Si es necesario, actualiza la configuración con las credenciales adecuadas.

## Levantar el Proyecto

1. **Compila el proyecto**:
   ```bash
   dotnet build
   ```

2. **Ejecuta el proyecto**:
   ```bash
   dotnet run
   ```

3. El servidor estará disponible en la dirección mostrada en la consola (por defecto, `https://localhost:5001` o `http://localhost:5000`).

## Migraciones hacia la Base de Datos

Entity Framework Core se utiliza para gestionar las migraciones de la base de datos. Sigue estos pasos para crear y aplicar migraciones:

1. **Generar una nueva migración**:
   Ejecuta el comando sustituyendo `NombreDeLaMigracion` por un nombre descriptivo para la migración:
   ```bash
   dotnet ef migrations add NombreDeLaMigracion
   ```

2. **Aplicar las migraciones a la base de datos**:
   ```bash
   dotnet ef database update
   ```

3. **Verificar el estado de las migraciones**:
   Si quieres consultar el historial de las migraciones aplicadas:
   ```bash
   dotnet ef migrations list
   ```

## Estructura del Proyecto

Una descripción general de las carpetas principales:
- **Controllers/**: Contiene los controladores de la API.
- **Models/**: Define las entidades y modelos de datos.
- **Data/**: Gestión de la base de datos, contextos y migraciones.
- **Services/**: Contiene la lógica de negocio.

## Contribuciones

Si deseas contribuir a este proyecto, por favor sigue estos pasos:
1. Crea un fork del repositorio.
2. Crea una rama (`git checkout -b feature/nueva-funcionalidad`).
3. Realiza tus cambios y haz commits descriptivos.
4. Envía un Pull Request.

## Soporte

Si encuentras un problema o tienes preguntas, por favor abre un [issue](https://github.com/basgomcesar/ChatBotBackend/issues).

---

¡Gracias por usar ChatBotBackend!
