# EnergyCo Loyalty API

A .NET Web API for managing customer loyalty baskets, applying discount and points promotions, and producing a final checkout summary.
This project was prepared for a technical assessment. I kept the implementation focused on clean API design, readable business logic, testability, 
and a structure that can grow without making the solution unnecessarily complex.

---

## Contents

- [What this API does](#what-this-api-does)
- [Solution structure](#solution-structure)
- [Architecture decision](#architecture-decision)
- [Main design approach](#main-design-approach)
- [Key assumptions](#key-assumptions)
- [Prerequisites](#prerequisites)
- [Run with Docker](#run-with-docker)
- [Run locally without Docker](#run-locally-without-docker)
- [Testing](#testing)
- [Testing with Postman](#testing-with-postman)
- [API overview](#api-overview)
- [Error handling](#error-handling)
- [Security and production considerations](#security-and-production-considerations)
- [Current limitations](#current-limitations)
- [Future enhancements](#future-enhancements)
- [Final note](#final-note)

---

## What this API does

The API supports a simple loyalty checkout flow:

- Return the product catalogue
- Add products to a customer basket
- Retrieve the current customer basket
- Checkout an existing basket
- Apply discount promotions
- Apply loyalty points promotions
- Return a final checkout summary
- Perform an express checkout in one request (as in given sample data)

All requests and responses use JSON.

Dates use the following format:

```text
dd-MMM-yyyy
```

Example:

```text
03-Apr-2020
```

---

## Solution structure

```text
EnergyCo.Loyalty.Api
  ASP.NET Core controllers, middleware, API contracts and request/response mapping

EnergyCo.Loyalty.Application
  Use cases, CQRS-style command/query handlers, application services and abstractions

EnergyCo.Loyalty.Domain
  Domain entities, value objects and business rules

EnergyCo.Loyalty.Infrastructure
  In-memory repositories, seed data and infrastructure service registration

EnergyCo.Loyalty.UnitTests
  xUnit tests for the main business scenarios
```

The API layer is intentionally thin. Most of the business behaviour sits in the Application and Domain layers so the logic is easier to test and maintain.

---

## Architecture decision

I designed this solution as a **modular monolith** rather than microservices.

For this technical assessment, a modular monolith gives the best balance between clean separation of concerns and practical delivery. 
The application is split into clear layers and modules, but it avoids the extra operational complexity that microservices would introduce, such as distributed transactions, 
service discovery, network failure handling, separate deployments and cross-service observability.

The main reasons for this approach were:

- The domain is small enough to keep in one deployable application.
- The assessment is mainly about business rules, API design, maintainability and testability.
- Application, Domain, Infrastructure and API concerns are separated clearly.
- Repository abstractions allow the in-memory store to be replaced later with a real database.
- The command/query flow keeps use cases isolated and easier to test.
- The codebase is simple for reviewers to run locally with Docker or `dotnet run`.

A microservices approach would make sense later if the system had independently owned bounded contexts, separate release cycles, very different scaling needs, 
or multiple teams working on different parts of the platform. For the current scope, it would add more complexity than value.

---

## Main design approach

A few decisions I made intentionally:

- Followed Clean Architecture principles to separate API, Application, Domain, and Infrastructure concerns.
- Kept the API layer thin, with business rules handled in the Application and Domain layers.
- Used a CQRS-style dispatcher to keep command and query handling consistent.
- Used repository abstractions so the in-memory storage can be replaced later without changing the core business logic.
- Kept API contracts separate from application DTOs.
- Used value objects such as `Money` to model important domain concepts clearly.
- Added validation around product pricing, transaction dates, basket state, and checkout rules.
- Used `ProblemDetails` for consistent API error responses.
- Added Docker and Postman support so the project can be reviewed and tested easily.
- Kept authentication simple for the assessment, while leaving a clear path for production security.

---

## Key assumptions

The most important assumptions are:

- The product catalogue and promotion rules are treated as the source of truth, rather than relying on values submitted in the request eg: price.
- If sample output conflicts with the mathematical result of the rules, the rules win.
- The client must submit the exact catalogue unit price. A mismatched price is rejected rather than silently corrected.
- Loyalty points are calculated on the original line total before discounts are applied.
- Promotion start and end dates are inclusive.
- All monetary values are assumed to be in AUD.
- A customer can have only one active basket at a time.
- Adding the same product twice increases the quantity instead of creating duplicate basket lines.
- Express checkout is stateless and does not require or persist basket state.
- The transaction date is supplied by the client and must use the expected `dd-MMM-yyyy` format.
- Data is stored in memory for this assessment, so baskets reset when the application restarts.
- Promotion data is seeded at startup and cannot be changed at runtime.

---

## Prerequisites

| Tool | Version |
| --- | --- |
| .NET SDK | 10.0 or compatible with the project target framework |
| Docker Desktop | 4.x or later |
| Postman | Optional, but recommended for manual testing |

---

## Run with Docker

From the repository root:

```bash
docker compose up --build
```

Run in the background:

```bash
docker compose up --build -d
```

Stop and remove containers:

```bash
docker compose down
```

The API will be available at:

```text
http://localhost:5000
```

---

## Run locally without Docker

From the repository root:

```bash
dotnet run --project EnergyCo.Loyalty/EnergyCo.Loyalty.Api.csproj
```

Depending on the launch profile, the API should run on:

```text
http://localhost:5000
https://localhost:5001 or 7240
```

---

## Testing

Run all tests:

```bash
dotnet test
```

I focused the tests on the core business logic and important edge cases, including:

- Product price validation
- Basket item handling
- Discount calculation
- Points calculation
- Promotion date windows
- Express checkout scenarios
- Invalid or missing data scenarios

---

## Testing with Postman

The repository includes Postman files for manual and collection-based testing:

| File | Purpose |
| --- | --- |
| `EnergyCo.Loyalty.postman_collection.json` | API requests and automated Postman checks |
| `EnergyCo.Loyalty.postman_environment.json` | Environment variables such as base URL and customer ID |

### Import steps

1. Open Postman.
2. Click **Import**.
3. Import both the collection and environment files.
4. Select the imported environment, usually named **EnergyCo Loyalty – Docker**.
5. Start the API:

```bash
docker compose up --build -d
```

6. Run individual requests or use **Run collection**.

### Important Postman variables

| Variable | Default | Description |
| --- | --- | --- |
| `baseUrl` | `http://localhost:5000` | API base URL |
| `customerId` | sample UUID | Used by customer basket requests |

### Recommended manual flow

For normal basket checkout:

1. `GET /api/products`
2. `POST /api/customers/{customerId}/basket/items`
3. Add a few different products
4. `GET /api/customers/{customerId}/basket`
5. `POST /api/customers/{customerId}/basket/checkout`

For express checkout:

1. Use `POST /api/customers/{customerId}/basket/checkout/express`
2. Provide all items in the request body
3. No previous basket state is required

---

## API overview

Base URL:

```text
http://localhost:5000
```

### Products

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/products` | Returns the seeded product catalogue |

### Basket

All basket endpoints are scoped to a customer:

```text
/api/customers/{customerId}/basket
```

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/customers/{customerId}/basket` | Returns the current basket |
| POST | `/api/customers/{customerId}/basket/items` | Adds an item to the basket |
| POST | `/api/customers/{customerId}/basket/checkout` | Checks out the existing basket |
| POST | `/api/customers/{customerId}/basket/checkout/express` | Checks out a full basket in one request |

### Example add item request

```json
{
  "productId": "PRD01",
  "unitPrice": 1.20,
  "quantity": 2
}
```

### Example checkout request

```json
{
  "loyaltyCard": "CARD-ABC-001",
  "transactionDate": "03-Apr-2020"
}
```

### Example express checkout request

```json
{
  "loyaltyCard": "CARD-XYZ-999",
  "transactionDate": "03-Apr-2020",
  "items": [
    {
      "productId": "PRD01",
      "unitPrice": 1.20,
      "quantity": 10
    },
    {
      "productId": "PRD04",
      "unitPrice": 2.30,
      "quantity": 1
    }
  ]
}
```

---

## Error handling

The API returns standard `ProblemDetails` responses for errors.

Common statuses:

| Status | Meaning |
| --- | --- |
| 400 | Validation failure, bad date format or price mismatch |
| 404 | Basket or product not found |
| 422 | Basket exists but cannot be processed in its current state |
| 500 | Unexpected server error |

Example:

```json
{
  "title": "Bad Request",
  "status": 400,
  "detail": "TransactionDate must be in dd-MMM-yyyy format, e.g. 03-Apr-2020."
}
```

---

## Security and production considerations

For local development and assessment review, the API is kept simple so it can be run easily with Docker, Postman and automated tests.

In a production environment, I would strengthen the security model with:

- Token-based authentication, typically JWT bearer tokens, issued by an identity provider such as Microsoft Entra ID, Auth0, or another OIDC-compliant provider.
- Azure API Management in front of the API for centralised gateway control.
- APIM policies for JWT validation, rate limiting, IP filtering, subscription keys, request/response transformation and consistent error handling.
- Custom APIM policies where required for partner-specific headers, claims validation, correlation IDs or request enrichment.
- Role-based or policy-based authorization inside the API for protected operations.
- Secure secret management using Azure Key Vault or an equivalent secret store.
- HTTPS-only traffic, secure headers and environment-specific configuration.
- Structured logging, audit events and Application Insights/OpenTelemetry for observability.
- Persistent storage with backup, recovery and data retention policies.
- CI/CD security gates such as dependency scanning, static analysis and automated tests before deployment.

I intentionally did not overbuild all of these production concerns in the assessment code, but the current layering and abstractions are designed 
so they can be introduced without rewriting the core business logic.

---

## Current limitations

These are intentional for the assessment scope:

- Data is stored in memory and resets on restart.
- There is no database migration or persistent storage.
- There is no admin API to manage products or promotions.
- Authentication is not fully wired to a real identity provider.
- Promotion configuration is static seed data.
- Only the required checkout and basket flows are implemented.

---

## Future enhancements

If this solution were extended beyond the assessment, I would improve it in the following areas.

### 1. Microservices readiness

The current implementation uses a modular monolith to keep the solution simple to run and review, while still maintaining clear boundaries between capabilities. 
If the product grew in scale, the clear module boundaries would make it easier to extract selected capabilities into microservices.

Potential candidates for future service separation include:

- Product catalogue service
- Promotion service
- Basket service
- Checkout service
- Loyalty points service
- Reporting or audit service

I would only split these out when there is a real need for independent scaling, separate ownership, independent deployment, or integration with other enterprise systems.

### 2. Persistence

Replace the in-memory repositories with a database-backed implementation, likely SQL Server or PostgreSQL or CosmosDB, while keeping the existing repository abstractions.

### 3. Promotion management

Add an admin API or back-office UI to create, update, expire and audit promotions.

### 4. Stronger validation

Introduce FluentValidation for request validation and more consistent validation messages.

### 5. Security hardening

Integrate with Entra ID, Auth0 or another OIDC provider, enforce authorization policies and front the API with Azure API Management.

### 6. Observability

Add structured logs, tracing, metrics, dashboards and alerts using Application Insights and OpenTelemetry.

### 7. Audit history

Store checkout transactions, applied promotions and point calculations for support, reporting and audit purposes.

---

## Final note

I tried to keep the solution practical rather than over-engineered.

The main focus was to show:

- Clear API design
- Clean business logic
- Testable application services
- Sensible architecture decisions
- Docker and Postman support for easy review
- A production-aware structure without unnecessary complexity
