# ProductInventoryAPI

A .NET 8 Web API for managing products, categories, and suppliers with authentication, authorization, validation, caching, rate limiting, bulk operations, CSV export, and product search.

## Project Overview

ProductInventoryAPI is a RESTful Web API built using ASP.NET Core Web API and Entity Framework Core.

The project provides:

- Product CRUD operations
- Category and product relationships
- Supplier management
- Many-to-many Product-Supplier relationship
- JWT-based authentication
- Refresh token authentication
- Claims-based authorization
- FluentValidation
- In-memory caching
- ETag-based conditional requests
- Rate limiting
- Bulk product creation
- CSV product export
- Product search
- Postman API testing
- Swagger/OpenAPI documentation

## Technologies Used

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- C#
- JWT Authentication
- BCrypt.Net
- FluentValidation
- AutoMapper
- Swagger / OpenAPI
- Postman
- Newman
- In-Memory Caching

## Architecture

```text
                         +----------------------+
                         |       Client         |
                         |   Postman / Client   |
                         +----------+-----------+
                                    |
                                    v
                         +----------------------+
                         |    ASP.NET Core      |
                         |      Web API         |
                         +----------+-----------+
                                    |
              +---------------------+---------------------+
              |                     |                     |
              v                     v                     v
     +----------------+    +----------------+    +----------------+
     | Authentication |    | Rate Limiting  |    |   Validation   |
     | & Authorization|    |   Middleware   |    | FluentValidation|
     +----------------+    +----------------+    +----------------+
              |
              v
     +----------------------+
     |     Controllers      |
     | Products / Auth      |
     +----------+-----------+
                |
                v
     +----------------------+
     |     Repository       |
     |       Layer          |
     +----------+-----------+
                |
                v
     +----------------------+
     |       EF Core        |
     |      DbContext       |
     +----------+-----------+
                |
                v
     +----------------------+
     |      SQL Server      |
     | Product / Category   |
     | Supplier / Users     |
     | RefreshTokens        |
     +----------------------+
```

## Project Structure

```text
ProductInventoryAPI
|
+-- Controllers
|   +-- AuthController.cs
|   +-- ProductsController.cs
|
+-- Data
|   +-- AppDbContext.cs
|
+-- DTO
|   +-- CreateProductDTO.cs
|   +-- UpdateProductDTO.cs
|   +-- ProductResponseDto.cs
|   +-- SupplierDTO.cs
|   +-- RegisterProductInventoryUserDTO.cs
|   +-- LoginProductInventoryUserDTO.cs
|   +-- RefreshTokenDTO.cs
|
+-- Models
|   +-- Product.cs
|   +-- Category.cs
|   +-- Supplier.cs
|   +-- ProductInventoryUser.cs
|   +-- RefreshToken.cs
|
+-- Repositories
|   +-- IProductRepository.cs
|   +-- ProductRepository.cs
|   +-- ISupplierRepository.cs
|   +-- SupplierRepository.cs
|
+-- Validators
|   +-- CreateProductValidator.cs
|   +-- UpdateProductValidator.cs
|
+-- Mapping
|   +-- ProductProfile.cs
|
+-- Middleware
|   +-- RateLimitingMiddleware.cs
|
+-- Migrations
|
+-- Program.cs
+-- appsettings.json
+-- README.md
```

## Database Design

The application uses SQL Server with Entity Framework Core.

### Main Entities

#### Product

Stores product information such as:

- Id
- Name
- Description
- Price
- Quantity
- CategoryId
- CreatedDate

#### Category

Stores product categories.

- Id
- Name

#### Supplier

Stores supplier information.

- Id
- Name
- ContactEmail
- Phone

#### Product-Supplier Relationship

A many-to-many relationship exists between Product and Supplier.

```text
Product
   |
   | Many
   |
   +----------------+
                    |
                    | Many-to-Many
                    |
                    +----------------+
                                     |
                                     |
                                  Supplier
```

#### ProductInventoryUser

Stores application users.

- Id
- Username
- PasswordHash
- Role

#### RefreshToken

Stores refresh tokens associated with users.

- Id
- Token
- ProductInventoryUserId
- ExpiresAt
- IsRevoked

## Authentication

The API uses JWT-based authentication.

### Login Flow

```text
Client
   |
   | Username + Password
   v
POST /api/auth/login
   |
   v
Validate User
   |
   v
Generate JWT + Refresh Token
   |
   v
Return Tokens
```

The JWT contains user identity and role information.

Example:

```text
Authorization: Bearer <JWT_TOKEN>
```

### Refresh Token

When the access token expires, the client can request a new access token using:

```http
POST /api/auth/refresh
```

Request body:

```json
{
    "refreshToken": "<refresh-token>"
}
```

## Claims-Based Authorization

The application uses the `CanManageInventory` claim to protect inventory-management operations.

The claim is added to users who are allowed to manage inventory.

Example:

```text
CanManageInventory = true
```

The authorization policy is configured as:

```csharp
options.AddPolicy("CanManageInventory", policy =>
    policy.RequireClaim("CanManageInventory", "true"));
```

Product creation, update, delete, and bulk creation require this policy.

## Validation

FluentValidation is used for business validation.

### Product Validation Rules

- Product name is required.
- Quantity cannot be negative.
- Price must be greater than zero when quantity is greater than zero.
- Supplier IDs must exist when creating a product.

Example:

```csharp
RuleFor(x => x.Price)
    .GreaterThan(0)
    .When(x => x.Quantity > 0)
    .WithMessage("Price must be greater than 0 when quantity is greater than 0");
```

## Caching

The product list uses ASP.NET Core `IMemoryCache`.

The product list is cached for 5 minutes.

```text
GET /api/products
       |
       v
Check Cache
   /       \
 Hit       Miss
 |          |
Return     Database
             |
             v
           Cache
             |
             v
           Return
```

The cache is invalidated after successful:

- POST
- PUT
- DELETE
- Bulk POST

operations.

## ETag Support

The product list endpoint generates an ETag based on the response content.

Example:

```http
ETag: "A1B2C3..."
```

A client can send:

```http
If-None-Match: "A1B2C3..."
```

If the resource has not changed, the API returns:

```http
304 Not Modified
```

This avoids sending the same response data repeatedly.

## Rate Limiting

The API implements custom rate limiting middleware.

The configured limit is:

```text
100 requests per minute per user
```

When the limit is exceeded, the API returns:

```http
429 Too Many Requests
```

with:

```http
Retry-After: 60
```

The current implementation stores request information in application memory. For a multi-instance production deployment, a shared store such as Redis would be required.

## Bulk Product Creation

Multiple products can be created using:

```http
POST /api/products/bulk
```

Example request:

```json
[
    {
        "name": "Laptop",
        "description": "Business laptop",
        "price": 50000,
        "quantity": 5,
        "categoryId": 1,
        "supplierIds": [1]
    },
    {
        "name": "Keyboard",
        "description": "Mechanical keyboard",
        "price": 3000,
        "quantity": 10,
        "categoryId": 1,
        "supplierIds": [2]
    }
]
```

All products are validated before they are inserted.

## CSV Export

Products can be exported as CSV using:

```http
GET /api/products/export?format=csv
```

The API returns a downloadable CSV response containing:

```text
Id,Name,Description,Price,Quantity,CategoryId
```

Only CSV format is currently supported.

## Product Search

Products can be searched using:

```http
GET /api/products/search?q=laptop
```

The search checks:

- Product Name
- Product Description
- Category Name

Example:

```http
GET /api/products/search?q=laptop
```

An empty search query returns:

```http
400 Bad Request
```

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT + refresh token |
| POST | `/api/auth/refresh` | Generate a new JWT using refresh token |

### Products

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a product |
| POST | `/api/products/bulk` | Create multiple products |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |
| GET | `/api/products/search?q={query}` | Search products |
| GET | `/api/products/export?format=csv` | Export products as CSV |

## Setup Instructions

### Prerequisites

Install the following:

- .NET 8 SDK
- SQL Server
- SQL Server Management Studio or Azure Data Studio
- Visual Studio or Visual Studio Code
- Postman
- Newman

### Clone the Repository

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
cd ProductInventoryAPI
```

### Configure Database Connection

Configure the SQL Server connection string using .NET User Secrets.

Example:

```bash
dotnet user-secrets init
```

Then:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<YOUR_CONNECTION_STRING>"
```

Do not commit passwords, connection strings, JWT secrets, or other sensitive values to source control.

### Configure JWT Secret

Store the JWT key using User Secrets:

```bash
dotnet user-secrets set "Jwt:Key" "<YOUR_JWT_SECRET>"
```

The JWT configuration should contain the issuer and audience:

```json
"Jwt": {
    "Issuer": "ProductInventoryAPI",
    "Audience": "ProductInventoryClient"
}
```

### Apply EF Core Migrations

Open Package Manager Console and run:

```powershell
Update-Database
```

Or using the .NET CLI:

```bash
dotnet ef database update
```

### Run the Application

```bash
dotnet run
```

The API will be available at the configured HTTPS URL.

Example:

```text
https://localhost:<PORT>
```

## Swagger / OpenAPI

When enabled, Swagger provides interactive API documentation.

The Swagger UI can be accessed at:

```text
https://localhost:<PORT>/swagger
```

Swagger can be used to:

- View available endpoints
- View request and response models
- Test API endpoints
- Inspect API documentation

## Postman Testing

A Postman collection is used to test the complete API.

Suggested collection structure:

```text
ProductInventoryAPI - Assignment 4
|
+-- 01. Authentication
|   +-- Register
|   +-- Login
|   +-- Refresh Token
|
+-- 02. Products
    +-- Get All Products
    +-- Get Product By ID
    +-- Create Product
    +-- Create Product - Validation Error
    +-- Create Product - Invalid Supplier
    +-- Bulk Create Products
    +-- Export Products CSV
    +-- Export Products - Invalid Format
    +-- Search Products
    +-- Search Products - Empty Query
    +-- Update Product
    +-- Delete Product
```

The Login request stores the JWT token and refresh token in Postman environment variables:

```text
token
refreshToken
```

Subsequent authenticated requests use:

```text
Authorization: Bearer {{token}}
```

## Postman Automated Tests

Postman tests validate:

- HTTP status codes
- Response structure
- Required fields
- Response time
- JWT token generation
- Refresh token generation
- Validation errors
- Authentication failures
- Authorization failures
- CSV response
- Search responses
- ETag behavior

Example:

```javascript
pm.test("Login successful", function () {
    pm.response.to.have.status(200);
});
```

## Newman Testing

Newman can be used to execute the Postman collection from the command line.

Example:

```bash
newman run "ProductInventoryAPI - Assignment 4.postman_collection.json" \
    -e "ProductInventoryAPI.postman_environment.json"
```

A Newman report can be generated as part of the final test execution.

## Security Considerations

The application implements:

- JWT authentication
- Password hashing using BCrypt
- Refresh tokens
- Claims-based authorization
- Role-based access
- FluentValidation
- Parameterized database operations through EF Core
- Rate limiting
- Secret management using User Secrets
- Authentication and authorization middleware

Sensitive information such as:

- Database passwords
- Connection strings
- JWT secrets
- User passwords

should not be committed to GitHub.

