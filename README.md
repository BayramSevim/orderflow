# OrderFlow

[![CI](https://github.com/BayramSevim/orderflow/actions/workflows/ci.yml/badge.svg)](https://github.com/BayramSevim/orderflow/actions/workflows/ci.yml)

A B2B order and shipment API for manufacturers and their dealers. Dealers place orders, admins approve, ship and deliver them, and stock is reserved when an order is approved.

Built with .NET 8, Clean Architecture, CQRS, EF Core and PostgreSQL.

## Tech stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core 8 with PostgreSQL 17 (Npgsql)
- JWT authentication, role-based authorization, BCrypt password hashing
- FluentValidation, Scrutor
- xUnit
- Docker Compose, GitHub Actions

## Architecture

```
src/
  OrderFlow.Domain          Entities and business rules. No dependencies.
  OrderFlow.Application     Use cases (command / query handlers) and interfaces.
  OrderFlow.Infrastructure  EF Core, repositories, migrations, JWT, BCrypt.
  OrderFlow.Api             Controllers, validation, error handling, auth setup.
tests/
  OrderFlow.Domain.Tests
  OrderFlow.Application.Tests
```

Dependencies point inward: `Api → Application → Domain`, and `Infrastructure` implements the interfaces defined in `Application`. The domain does not know about the database or HTTP.

## Design decisions

**Rich domain model.** Entities protect their own rules. Properties have private setters and all changes go through methods (`order.Approve()`, `product.DecreaseStock()`), so an invalid object cannot be created or reached.

**Order aggregate.** `Order` is the aggregate root. Items can only be added through `Order.AddItem`, and only while the order is pending. There is no `DbSet<OrderItem>`; items are loaded and saved through the order.

**Price snapshot.** When an item is added, the product's current name and price are copied into the order item. Later price changes never alter existing orders.

**Order state machine.** Only valid transitions are allowed:

```
Pending ──Approve──► Approved ──Ship──► Shipped ──Deliver──► Delivered
   │                    │
   └──────Cancel────────┴──► Cancelled
```

**Stock reservation.** Stock is decreased when an order is approved and restored if an approved order is cancelled. The order and all product stocks are saved in a single `SaveChanges` call, so either everything is saved or nothing is.

**Optimistic concurrency.** Products use PostgreSQL's `xmin` system column as a row version. If two admins approve orders for the same product at the same time, the second save fails with `409 Conflict` instead of overselling stock.

**CQRS without MediatR.** Each use case is a small handler implementing `ICommandHandler<TCommand, TResult>` or `IQueryHandler<TQuery, TResult>`. Handlers only take the dependencies they need, and adding a feature means adding a file instead of editing a growing service class. Handlers are registered automatically with Scrutor, and controllers receive them per action with `[FromServices]`.

**Centralized error handling.** Business errors are thrown as domain exceptions and mapped to HTTP responses in a single `IExceptionHandler`, using `ProblemDetails`:

| Exception | Status |
|---|---|
| `NotFoundException` | 404 |
| `ConflictException`, concurrency conflict | 409 |
| `UnauthorizedException` | 401 |
| `ForbiddenException` | 403 |
| `DomainException` | 400 |
| anything else | 500 (logged, details hidden) |

**Validation in two layers.** FluentValidation checks the shape of each request and returns all errors at once. The domain still enforces its own rules, because objects can be created from places other than HTTP requests.

**Authentication and ownership.** Users log in with email and password and receive a JWT. Admins manage dealers and products and move orders through their lifecycle. Dealer users can only create, see and cancel their own dealer's orders. Accessing another dealer's order returns `404` rather than `403`, so the API does not reveal which orders exist.

## Running locally

Requirements: .NET SDK (see `global.json`), Docker.

```bash
docker compose up -d
dotnet ef database update --project src/OrderFlow.Infrastructure --startup-project src/OrderFlow.Api
dotnet run --project src/OrderFlow.Api
```

Open http://localhost:5079/swagger.

On first start an admin user is created from `appsettings.Development.json`:

- Email: `admin@orderflow.local`
- Password: `Admin12345`

Call `POST /api/auth/login`, copy the token and paste it into Swagger's **Authorize** button.

> The connection string, JWT key and seed credentials in `appsettings.Development.json` are local development values. In production they come from environment variables or a secret store.

## API overview

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/auth/login` | Anyone |
| POST | `/api/auth/register` | Admin |
| GET | `/api/dealers`, `/api/dealers/{id}` | Admin |
| POST | `/api/dealers` | Admin |
| GET | `/api/products`, `/api/products/{id}` | Authenticated |
| POST | `/api/products` | Admin |
| POST | `/api/orders` | Admin, or dealer for its own dealer |
| GET | `/api/orders`, `/api/orders/{id}` | Admin sees all, dealer sees its own |
| POST | `/api/orders/{id}/approve` | Admin |
| POST | `/api/orders/{id}/ship` | Admin |
| POST | `/api/orders/{id}/deliver` | Admin |
| POST | `/api/orders/{id}/cancel` | Admin, or dealer for its own order |

## Tests

```bash
dotnet test
```

- **Domain tests** cover entity rules: order state transitions, totals, stock changes, user role rules.
- **Application tests** run the handlers against in-memory fakes of the repositories, unit of work and current user. They cover stock reservation and restoration, price snapshots, ownership checks, and that nothing is saved when a rule fails.

Tests run on every push with GitHub Actions.

## Roadmap

- Integration tests with Testcontainers and `WebApplicationFactory`
- Order events published to RabbitMQ through a transactional outbox
