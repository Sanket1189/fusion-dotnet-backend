# Fusion ASP.NET Core Web API

The complete backend built throughout the .NET learning path in Sanket Keluskar's portfolio.
It is a beginner-focused ASP.NET Core Web API using controllers, a service layer, Entity Framework
Core database-first, MySQL, JWT authentication, role authorization, Swagger, global error handling,
and validated local image uploads.

## What is included

- Thin API controllers and service-layer business logic
- MySQL database-first Entity Framework Core model
- CRUD endpoints with search, sorting, and pagination
- DTO validation and consistent error responses
- Registration, password hashing, login, and JWT authentication
- User and Admin role authorization
- JPG, PNG, and WebP uploads with size and file-signature validation
- Swagger/OpenAPI documentation
- SQL setup scripts under `Database/`

## Requirements

- .NET 10 SDK
- MySQL Server 8
- Visual Studio 2026 or another .NET-capable editor

## 1. Create the database

Open MySQL Workbench and run:

```text
Database/Fusion.sql
```

`Fusion.sql` creates the database and the complete `users` table, including the `PasswordHash` and
`Role` columns used by the authentication lessons. The smaller upgrade scripts remain available for
learners who created the table during an earlier lesson.

## 2. Configure local secrets

From the backend project directory, run:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=127.0.0.1;Port=3306;Database=fusion;User ID=root;Password=YOUR_MYSQL_PASSWORD;SslMode=None;"
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_LONG_RANDOM_DEVELOPMENT_KEY"
```

`SslMode=None` is intended only for the local MySQL server used by this tutorial. Use properly
configured TLS for a remote or production database. Secrets are deliberately excluded from the
repository; Git is excellent version control and a terrible password manager.

## 3. Restore and run

```powershell
dotnet restore
dotnet run --launch-profile https
```

Swagger opens at:

```text
https://localhost:7205/swagger
```

The HTTP profile is available at `http://localhost:5008`.

## Main endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a user and return a JWT |
| POST | `/api/auth/login` | Log in and return a JWT |
| GET | `/api/auth/me` | Read the authenticated identity |
| GET | `/api/auth/admin-only` | Test the Admin policy |
| GET | `/api/users` | Search, sort, and page through users |
| GET | `/api/users/{id}` | Read one user |
| POST | `/api/users` | Create one user |
| PUT | `/api/users/{id}` | Update one user |
| DELETE | `/api/users/{id}` | Delete one user as an Admin |
| POST | `/api/files/images` | Upload a validated image |

## Related learning guide

The step-by-step lessons live in the portfolio repository:
[Sanket Keluskar Portfolio](https://github.com/Sanket1189/sanket-keluskar).

