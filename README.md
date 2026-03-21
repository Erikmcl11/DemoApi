# DemoApi

REST API desarrollada con .NET 10, Entity Framework Core y Azure SQL Database. Permite gestionar contactos mediante operaciones CRUD completas.

## Arquitectura

```
DemoApi/
├── src/
│   ├── Program.cs              # Endpoints y configuración de la app
│   ├── DemoApi.csproj          # Dependencias del proyecto
│   ├── dockerfile              # Imagen Docker para producción
│   ├── Data/
│   │   └── DataContext.cs      # Contexto de Entity Framework
│   ├── Models/
│   │   └── Contactos.cs        # Modelo de la entidad Contacto
│   └── appsettings.json        # Configuración base
├── infra/
│   └── infraDeploy.sh          # Script de despliegue en Azure
└── README.md
```

### Stack tecnológico

| Capa | Tecnología |
|------|-----------|
| Framework | .NET 10 Minimal API |
| ORM | Entity Framework Core 10 |
| Base de datos | Azure SQL Database |
| Documentación | Swagger / OpenAPI 3.0 |
| Contenedor | Docker |
| Nube | Azure Container Apps |

---

## Endpoints

Base URL local: `http://localhost:5143`
Base URL producción: `https://demoapi.politeforest-08638a17.brazilsouth.azurecontainerapps.io`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/contactos` | Obtener todos los contactos |
| GET | `/contactos/{id}` | Obtener contacto por ID |
| POST | `/contactos` | Crear nuevo contacto |
| PUT | `/contactos/{id}` | Actualizar contacto existente |
| DELETE | `/contactos/{id}` | Eliminar contacto |

### Modelo Contacto

```json
{
  "id": 1,
  "name": "Erik",
  "lastname": "Martinez",
  "birthday": "1990-01-15",
  "phoneNumber": "55551234",
  "email": "erik@email.com"
}
```

---

## Correr en local

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server o Azure SQL Database
- [Git](https://git-scm.com/)

### Pasos

**1. Clonar el repositorio**
```bash
git clone https://github.com/Erikmcl11/DemoApi.git
cd DemoApi
```

**2. Configurar la connection string**

Edita `src/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<tu-servidor>.database.windows.net,1433;Initial Catalog=<tu-db>;User ID=<usuario>;Password=<contraseña>;Encrypt=True;"
  }
}
```

**3. Ejecutar la aplicación**
```bash
cd src
dotnet run
```

**4. Abrir Swagger**
```
http://localhost:5143/swagger
```

---

## Correr con Docker

### Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Pasos

**1. Construir la imagen**
```bash
cd src
docker build -t demoapi:latest -f dockerfile .
```

**2. Correr el contenedor**
```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=tcp:<servidor>.database.windows.net,1433;Initial Catalog=<db>;User ID=<usuario>;Password=<contraseña>;Encrypt=True;" \
  demoapi:latest
```

**3. Abrir Swagger**
```
http://localhost:8080/swagger
```

---

## Despliegue en Azure

### Requisitos

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Cuenta en [Docker Hub](https://hub.docker.com/)
- Suscripción de Azure activa

### Infraestructura en Azure

| Recurso | Nombre |
|---------|--------|
| Resource Group | rg-demo |
| SQL Server | demo-server-erik |
| SQL Database | DemoApiDB |
| Container Apps Environment | demo-environment |
| Container App | demoapi |

### Pasos de despliegue

**1. Login en Azure y Docker**
```bash
az login
docker login
```

**2. Instalar extensión de Container Apps**
```bash
az extension add --name containerapp --upgrade
```

**3. Crear entorno de Container Apps**
```bash
az containerapp env create \
  --name demo-environment \
  --resource-group rg-demo \
  --location brazilsouth
```

**4. Construir y publicar imagen Docker**
```bash
cd src
docker build -t erikmcl/demoapi:latest -f dockerfile .
docker push erikmcl/demoapi:latest
```

**5. Desplegar en Azure Container Apps**
```bash
az containerapp up \
  --name demoapi \
  --resource-group rg-demo \
  --environment demo-environment \
  --image erikmcl/demoapi:latest
```

**6. Habilitar acceso externo**
```bash
az containerapp ingress enable \
  -n demoapi -g rg-demo \
  --type external \
  --target-port 8080 \
  --transport auto
```

**7. Configurar connection string como secreto**
```bash
az containerapp secret set -n demoapi -g rg-demo \
  --secrets "connstring=<tu-connection-string>"

az containerapp update -n demoapi -g rg-demo \
  --set-env-vars "ConnectionStrings__DefaultConnection=secretref:connstring"
```

**8. Permitir acceso desde Azure al SQL Server**

En Azure Portal → `demo-server-erik` → Seguridad → Redes → activar:
> "Permitir que los servicios y recursos de Azure accedan a este servidor"

---

## Producción

La API está desplegada y disponible en:

- **Swagger:** https://demoapi.politeforest-08638a17.brazilsouth.azurecontainerapps.io/swagger
- **API:** https://demoapi.politeforest-08638a17.brazilsouth.azurecontainerapps.io/contactos
