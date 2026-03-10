# Linkr

Linkr is a lightweight URL shortening service implemented as an ASP.NET Core Web API with a PostgreSQL backend. It supports user authentication, click tracking, and provides a simple REST API for creating and resolving short URLs.

---

## 🚀 Features

- Create and manage shortened URLs
- Track click statistics (click count + metadata)
- Authentication via JWT
- Clean separation of API, domain, and infrastructure layers
- Docker support for local development and deployment

---

## 🧱 Architecture

This solution is structured as a multi-project .NET solution:

- `Linkr.Api` – ASP.NET Core Web API project (hosts controllers, middleware, and DI setup)
- `Linkr.Domain` – Core domain models, business logic, and DTOs
- `Linkr.Infrastructure` – Database access, repositories, and persistence

---

## ✅ Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, but recommended for local development)
- PostgreSQL (if not using Docker)

---

## 🛠️ Configuration

Configuration is managed via `appsettings.json` + environment variables. The following values must be set before running the app:

### Required settings

- `AuthenticationSettings:Secret` – JWT signing secret (set via `appsettings.json` or `AuthenticationSettings__Secret` env var)
- `ConnectionStrings:DefaultConnection` – PostgreSQL connection string (set via `appsettings.Development.json` or `ConnectionStrings__DefaultConnection` env var)

### Docker-specific settings

- `POSTGRES_PASSWORD` – Password for the built-in PostgreSQL container (default is `changeme`)
- `WebsiteConfiguration:Url` – Base URL used for creating full short URLs (update in `appsettings.Production.json` for production deployments)

---

## 🏃‍♂️ Run locally (without Docker)

1. Restore dependencies:

```bash
cd Linkr.Api
dotnet restore
```

2. Apply EF Core migrations (if needed):

```bash
dotnet ef database update --project ../Linkr.Infrastructure/Linkr.Infrastructure.csproj --startup-project Linkr.Api
```

3. Run the API:

```bash
dotnet run --project Linkr.Api
```

The API will start on the configured port (default: `https://localhost:5001`).

---

## 🐳 Run with Docker

Start the full stack (API + PostgreSQL) via Docker Compose:

```bash
docker-compose up --build
```

To shut it down:

```bash
docker-compose down
```

---

## 🧪 Running Tests

> (If there are tests in this repo add instructions here; otherwise remove this section.)

---

## 🧩 API Endpoints

> Update this section with your actual endpoints and request/response samples.

- `POST /api/users/register` – Register a new user
- `POST /api/users/login` – Log in and receive a JWT
- `POST /api/urls` – Create a new shortened URL (authenticated)
- `GET /{shortCode}` – Redirect to original URL
- `GET /api/urls/{id}` – Get URL details (authenticated)

---

## 🧭 Deployment Notes

- Verify `WebsiteConfiguration:Url` is set correctly for your deployed domain.
- Ensure the database is accessible from the deployed environment.
- Set environment variables for secrets and connection strings instead of committing them.

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
