# Hospital Management System API

A RESTful API built with ASP.NET Core 8 for managing hospital operations including patients, doctors, and appointments.

## Tech Stack

- **Framework:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core 8
- **Database:** SQL Server
- **Authentication:** Custom JWT with role-based access control
- **Logging:** Serilog (Console + File)
- **Documentation:** Swagger / OpenAPI

## Features

- JWT Authentication with role-based authorization (Admin, Doctor, Receptionist)
- Patient management (CRUD)
- Doctor management (CRUD)
- Appointment scheduling and management
- Global exception handling middleware
- Structured logging with Serilog
- Database seeding with default Admin account

## Roles

| Role | Permissions |
|---|---|
| Admin | Full access — manage users, patients, doctors, appointments |
| Doctor | View and manage appointments, view patients |
| Receptionist | Manage appointments, create patients |

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server

### Setup

1. Clone the repository
```bash
git clone https://github.com/jankovicdanilo/HospitalManagement.git
```

2. Update connection string in `appsettings.json`

3. Run the application
```bash
dotnet run
```

4. Navigate to Swagger UI
https://localhost:7265/swagger

### Default Admin Credentials
- **Username:** admin
- **Password:** Admin123!

## Project Structure
HospitalManagement/
├── Controllers/        # API endpoints
├── Services/           # Business logic
│   └── Interfaces/
├── Repositories/       # Data access
│   └── Interfaces/
├── Models/
│   ├── Domain/         # Database entities
│   └── DTOs/           # Data transfer objects
├── Data/               # DbContext and seed data
├── Middleware/         # Custom middleware
├── Settings/           # Configuration classes
└── Common/             # Shared utilities (Result<T>)

## Architecture

Layered architecture with clear separation of concerns:
- **Controllers** — handle HTTP requests and responses
- **Services** — business logic and validation
- **Repositories** — data access and database operations
- **Middleware** — cross cutting concerns (exception handling, logging)
