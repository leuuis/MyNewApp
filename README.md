# MyNewApp API

API RESTful construida en .NET 8 con autenticación JWT (RSA), roles de usuario y operaciones CRUD sobre tareas (ToDos).

## 🚀 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html) (opcional si usas el archivo `.db` embebido)
- [DB Browser for SQLite (opcional)](https://sqlitebrowser.org/)

---

## ⚙️ Instalación

1. **Clona el repositorio:**

```bash
git clone https://github.com/tu-usuario/MyNewApp.git
cd MyNewApp
```

2. **Instala las dependencias:**

```bash
dotnet restore
```

3. **Configura las variables de entorno:**

Crea un archivo `.env` (no se incluye por defecto) o configura las siguientes en tu entorno:

```
JWT__Issuer=https://mynewapp.local
JWT__Audience=mynewapp-client
JWT__TokenLifetimeMinutes=60
JWT__PrivateKeyPath=./Keys/private_key.pem
JWT__PublicKeyPath=./Keys/public_key.pem
```

> 🔐 Asegúrate de que las claves RSA existan en la ruta `./Keys`. Puedes generarlas con:

```bash
openssl genpkey -algorithm RSA -out private_key.pem -pkeyopt rsa_keygen_bits:2048
openssl rsa -pubout -in private_key.pem -out public_key.pem
```

---

## 🛠️ Migraciones y Seeders

1. **Ejecuta migraciones y siembra de datos:**

```bash
dotnet run -- --seed
```

> Esto aplicará las migraciones pendientes y creará usuarios por defecto (uno con rol `admin` y otro con rol `user`).

---

## ▶️ Ejecución del Proyecto

```bash
dotnet run
```

La API estará disponible en: `https://localhost:5001` o `http://localhost:5000`

---

## 🧪 Probar en Swagger

1. Accede a Swagger en:

```
https://localhost:5001/swagger
```

2. Usa el endpoint `/api/Auth/token` con credenciales válidas para obtener un token JWT:

```json
{
  "username": "admin",
  "password": "admin123"
}
```

3. Copia el token y haz clic en el botón de candado 🔒 en Swagger.
   - Elige `Bearer`, pega el token como:
     ```
     Bearer eyJhbGciOiJSUzI1NiIsInR...
     ```

---

## 🔐 Roles

- `admin`: acceso completo incluyendo `DELETE`
- `user`: acceso limitado (sin delete)

Puedes proteger rutas con `[Authorize(Roles = "admin")]`.

---

## 🧹 Limpiar y reconstruir

```bash
dotnet clean
dotnet build
```

---

## 📂 Estructura de carpetas

```
MyNewApp/
├── Controllers/
├── Models/
├── Services/
├── Data/
├── Keys/
│   ├── private_key.pem
│   └── public_key.pem
├── appsettings.json
├── Program.cs
└── MyNewApp.csproj
MyNewApp.Tests/
├── Services/
```

---

## ✨ Créditos

Proyecto creado con ❤️ para fines educativos y de autenticación robusta con JWT + Roles.

---

---

## 🧪 Cómo ejecutar pruebas unitarias

Este proyecto usa `xUnit` para las pruebas. Para ejecutarlas:

1. Asegúrate de estar en la raíz del proyecto.
2. Ejecuta el siguiente comando en la terminal:

```bash
dotnet test
```

Esto compilará los proyectos de prueba y ejecutará todos los tests definidos.

> 💡 Si necesitas generar un reporte más detallado, puedes usar:
>
> ```bash
> dotnet test --logger "console;verbosity=detailed"
> ```
