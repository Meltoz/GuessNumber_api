# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GuessNumber API is a .NET 9.0 web API for a number guessing game platform. The project follows Clean Architecture principles with strict separation between Domain, Application, Infrastructure, and Web layers.

## Architecture

### Layer Structure

- **Domain**: Pure business logic with rich domain models. Uses private setters and validation in domain entities. No external dependencies.
  - Domain models use value objects (Libelle, Author, Unit) for type safety
  - Enums for TypeQuestion and VisibilityQuestion (using [Flags] attribute)
  - Business rules enforced in constructors and Change* methods

- **Application**: Business use cases and service orchestration. Depends only on Domain and Shared.
  - Services registered in `Application/DependencyInjection.cs`
  - Repository interfaces defined here (in `Application/Interfaces/Repository/`)

- **Infrastructure**: Data persistence and external concerns
  - Entity Framework Core with PostgreSQL (production) and SQLite (testing)
  - AutoMapper for Domain ↔ Entity mapping (configured in `Mappings/`)
  - Repository pattern implementations inheriting from `BaseRepository<TDomain, TEntity>`
  - All entities inherit from `BaseEntity` (Id, Created, Updated)
  - DbContext automatically handles UTC timestamps via `UpdateTimestamps()` override

- **Web**: ASP.NET Core API controllers and HTTP concerns
  - Controllers in `Web/Controllers/Admins/` namespace
  - AutoMapper for ViewModel ↔ Domain mapping (configured in `Mappings/`)
  - CORS configured for http://localhost:3000
  - Global exception handling via `ExceptionHandlingMiddleware`
  - Automatic migrations on startup (except in Testing environment)

- **Shared**: Cross-cutting utilities
  - `PagedResult<T>` for pagination
  - `SortOption` and `SortOptionFactory` for dynamic sorting
  - Configuration models
  - Shared enums and filters

### Key Architectural Patterns

1. **Domain-Driven Design**: Rich domain models with encapsulated business logic. All mutations go through Change* methods that validate business rules.

2. **Repository Pattern**: Generic `IRepository<T>` interface with standard CRUD operations. Specific repositories extend with domain-specific queries (e.g., `IActualityRepository.Search`).

3. **Dual Mapping Strategy**:
   - Domain ↔ Entity (Infrastructure layer, for persistence)
   - ViewModel ↔ Domain (Web layer, for API contracts)

4. **Testing Environments**:
   - Production/Development: PostgreSQL via `DefaultConnection` connection string
   - Testing: SQLite in-memory database via `DbContextProvider.SetupContext()`
   - Environment detection via `ASPNETCORE_ENVIRONMENT` (set to "Testing" in tests)

5. **Timestamp Management**: DbContext automatically sets Created/Updated timestamps in `SaveChanges()` override. Can be disabled via constructor parameter for testing.

## Development Commands

### Build and Run

```bash
# Build the solution
dotnet build

# Run the Web API (development mode with Swagger)
dotnet run --project Web

# Run with specific environment
dotnet run --project Web --environment Development
```

### Testing

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test UnitTests/UnitTests.csproj

# Run integration tests only
dotnet test IntegrationTests/IntegrationTests.csproj

# Run a specific test
dotnet test --filter "FullyQualifiedName~QuestionServiceTests.AddQuestion_ShouldReturnQuestion"

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

### Database Migrations

```bash
# Create a new migration (run from repository root)
dotnet ef migrations add MigrationName --project Infrastructure --startup-project Web

# Update database to latest migration
dotnet ef database update --project Infrastructure --startup-project Web

# Rollback to specific migration
dotnet ef database update MigrationName --project Infrastructure --startup-project Web

# Remove last migration (if not applied)
dotnet ef migrations remove --project Infrastructure --startup-project Web

# View migration SQL without applying
dotnet ef migrations script --project Infrastructure --startup-project Web
```

### Project Dependencies

```bash
# Restore NuGet packages
dotnet restore

# Add package to a project
dotnet add Application/Application.csproj package PackageName

# Update packages
dotnet list package --outdated
dotnet add package PackageName --version x.x.x
```

## Testing Guidelines

### Unit Tests (UnitTests/)
- Use Moq for mocking repository dependencies
- Test Application services in isolation
- Test Domain model validation logic
- No database required

### Integration Tests (IntegrationTests/)
- Use `CustomWebApplicationFactory<Program>` for API testing
- Use `DbContextProvider.SetupContext()` for repository testing with SQLite in-memory
- Tests run with ASPNETCORE_ENVIRONMENT="Testing"
- Each test should use a fresh in-memory database instance

Example integration test setup:
```csharp
var context = DbContextProvider.SetupContext();
var mapper = MapperProvider.CreateMapper();
var repository = new SomeRepository(context, mapper);
```

Example API integration test setup:
```csharp
var context = DbContextProvider.SetupContext();
var factory = new CustomWebApplicationFactory(context);
var client = factory.CreateClient();
```

## Important Implementation Notes

### Adding a New Entity

1. Create Domain model in `Domain/` with private setters and validation
2. Create Entity in `Infrastructure/Entities/` inheriting from `BaseEntity`
3. Add DbSet to `GuessNumberContext`
4. Create mapping profiles in `Infrastructure/Mappings/` (Domain ↔ Entity)
5. Create repository interface in `Application/Interfaces/Repository/`
6. Implement repository in `Infrastructure/Repositories/` inheriting from `BaseRepository`
7. Register repository in `Infrastructure/DependencyInjection.cs`
8. Create service in `Application/Services/`
9. Register service in `Application/DependencyInjection.cs`
10. Create ViewModel in `Web/ViewModels/`
11. Create mapping profiles in `Web/Mappings/` (ViewModel ↔ Domain)
12. Create controller in `Web/Controllers/Admins/`
13. Create migration: `dotnet ef migrations add AddEntityName --project Infrastructure --startup-project Web`

### Business Rules Validation

- Domain validation belongs in Domain models (via Change* methods or constructors)
- Infrastructure validation (like foreign key constraints) configured in `GuessNumberContext.OnModelCreating`
- Example: Question visibility and type validation enforced in `SetVisibilityAndType()` method

### Connection String Configuration

- Development: Set in `Web/appsettings.Development.json` under `ConnectionStrings:DefaultConnection`
- Production: Should be provided via environment variables or configuration
- Testing: Uses SQLite in-memory, no connection string needed

### AutoMapper Configuration

- Infrastructure mappings: Registered in `Infrastructure/DependencyInjection.cs` with `ShouldUseConstructor = ci => ci.IsPrivate` to support domain models with private constructors
- Web mappings: Registered in `Web/Program.cs` with default settings
