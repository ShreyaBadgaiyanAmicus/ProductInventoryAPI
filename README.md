## Assignment 1

### 1. Create ASP.NET Core Web API project: ProductInventoryAPI [Use .NET 8, Web API template, verify project runs]

![Alt Text](Screenshots/Assignment1/task1_img1.png)

![Alt Text](Screenshots/Assignment1/task1_img2.png)

### 2. Create models: Product & Category entities [Product (Id, Name, Description, Price, Quantity, Category, CreatedDate), Category (Id, Name)]

![Alt Text](Screenshots/Assignment1/task2_code1.png)

![Alt Text](Screenshots/Assignment1/task2_code2.png)

### 3. Set up EF Core with SQL Server (Code-First) [DbContext, connection string in appsettings.json, create & run migrations]

1. Install EF Core Packages

![Alt Text](Screenshots/Assignment1/task3_code1.png)

2. Add the Connection String

![Alt Text](Screenshots/Assignment1/task3_code2.png)

3. Create AppDbContext

![Alt Text](Screenshots/Assignment1/task3_code3.png)

4. Configure AppDbContext in Program.cs

![Alt Text](Screenshots/Assignment1/task3_code4.png)

5. Create Migrations

![Alt Text](Screenshots/Assignment1/task3_code5.png)


### 4. Create DTOs & AutoMapper profiles [CreateProductDto, UpdateProductDto, ProductResponseDto + mapping profiles]

1. Install Automapper

![Alt Text](Screenshots/Assignment1/task4_img.png)

2. Create CreateProductDto

![Alt Text](Screenshots/Assignment1/task4_code1.png)

3. Create UpdateProductDto

![Alt Text](Screenshots/Assignment1/task4_code2.png)

4. Create ProductResponseDto

![Alt Text](Screenshots/Assignment1/task4_code3.png)

5. Create ProductProfile.cs

![Alt Text](Screenshots/Assignment1/task4_code5.png)

6. Register Automapper in Program.cs

![Alt Text](Screenshots/Assignment1/task4_code6.png)

### 5. Build full CRUD endpoints with attribute routing [GET, GET/{id}, POST, PUT/{id}, DELETE/{id} — return 200, 201, 204, 400, 404]
Create ProductController in Controllers folder

1. GET

![Alt Text](Screenshots/Assignment1/task5_code1.png)

2. GET{id}

![Alt Text](Screenshots/Assignment1/task5_code2.png)

3. POST

![Alt Text](Screenshots/Assignment1/task5_code3.png)

4. PUT

![Alt Text](Screenshots/Assignment1/task5_code4.png)

5. DELETE

![Alt Text](Screenshots/Assignment1/task5_code5.png)


### 6. Add Data Annotations on models  [[Required], [StringLength], [Range] on Product entity properties]

![Alt Text](Screenshots/Assignment1/task6_code.png)

### 7. Add FluentValidation for DTOs [CreateProductValidator, UpdateProductValidator — Name required, Price > 0, Quantity ≥ 0]

1. Install Package FluentValidation and FluentValidation.DependencyInjectionExtensions

![Alt Text](Screenshots/Assignment1/task7_code1.png)

2. Create Validator folder and files

![Alt Text](Screenshots/Assignment1/task7_code2.png)

3. Create CreateProductValidator

![Alt Text](Screenshots/Assignment1/task7_code3.png)

4. Create UpdateProductValidator

![Alt Text](Screenshots/Assignment1/task7_code4.png)

5. Update ProductsController

![Alt Text](Screenshots/Assignment1/task7_code5.png)

6. Register in Program.cs

![Alt Text](Screenshots/Assignment1/task7_code6.png)


### 8. Implement Repository pattern with basic DI [IProductRepository + ProductRepository. Register with services.AddScoped() — basic DI setup covered with EF Core DbContext registration.]

1. Create Repository folder with files

![Alt Text](Screenshots/Assignment1/task8_code1.png)

2. Create IProductRepository.cs

![Alt Text](Screenshots/Assignment1/task8_code2.png)

3. Create ProductRepository.cs

![Alt Text](Screenshots/Assignment1/task8_code3.png)

4. Register in program.cs

![Alt Text](Screenshots/Assignment1/task8_code4.png)

5. Update ProductController

![Alt Text](Screenshots/Assignment1/task8_code5.png)


### 9. Use async/await with EF Core methods [ Use ToListAsync, FindAsync, SaveChangesAsync in repository — this is standard EF Core usage, not advanced async patterns.]

1. Update IProductRepository

![Alt Text](Screenshots/Assignment1/task9_code1.png)

2. Update ProductRepository

![Alt Text](Screenshots/Assignment1/task9_code2.png)

3. Update ProductsController

![Alt Text](Screenshots/Assignment1/task9_code3.png)


### 10. Test all endpoints using Postman Collection [ Create a Postman collection with folders (Products CRUD). Add environment variables (baseUrl). Verify all status codes (200, 201, 204, 400, 404) and validation error responses.]

1. Postman Collection

![Alt Text](Screenshots/Assignment1/task10_code1.png)

2. Adding baseurl

![Alt Text](Screenshots/Assignment1/task10_code2.png)

3. 200 OK

![Alt Text](Screenshots/Assignment1/task10_code3.png)

4. 404 Not Found

![Alt Text](Screenshots/Assignment1/task10_code4.png)

5. 201 Created

![Alt Text](Screenshots/Assignment1/task10_code5.png)

6. 400 Bad Request

![Alt Text](Screenshots/Assignment1/task10_code6.png)

7. 204 No Content

![Alt Text](Screenshots/Assignment1/task10_code7.png)


## Assignment 2

### 1. Extend data model — Add Supplier entity & relationships [ Supplier (Id, Name, ContactEmail, Phone), Product-Supplier many-to-many, new migration + seed data]

1. Create Supplier.cs in models

![Alt Text](Screenshots/Assignment2/task1_code1.png)

2. Create many-many relationship in product.cs

![Alt Text](Screenshots/Assignment2/task1_code2.png)

3. Create many-many relationship in supplier.cs

![Alt Text](Screenshots/Assignment2/task1_code3.png)

4. Update appdbcontext.cs

![Alt Text](Screenshots/Assignment2/task1_code4.png)

5. Run add-migration

![Alt Text](Screenshots/Assignment2/task1_code5.png)

6. Add seed data to appdbcontext

![Alt Text](Screenshots/Assignment2/task1_code6.png)

7. Add-migration

![Alt Text](Screenshots/Assignment2/task1_code7.png)


### 2. Extend validation — Cross-field & business rules [ Custom FluentValidation: Price > 0 when Quantity > 0. Supplier must exist when creating product.]

1. Update CreateProductValidator

![Alt Text](Screenshots/Assignment2/task2_code1.png)

2. Update UpdateProductValidator

![Alt Text](Screenshots/Assignment2/task2_code2.png)

3. Supplier Repository

![Alt Text](Screenshots/Assignment2/task2_code3.png)

4. Update productcontroller

![Alt Text](Screenshots/Assignment2/task2_code4.png)



### 3. Extend auth — Refresh tokens & claims-based access [ POST /api/auth/refresh, claims-based authorization ("CanManageInventory" claim)]


1. Create refreshToken.cs

![Alt Text](Screenshots/Assignment2/task3_code1.png)

2. Add to appdbcontext

![Alt Text](Screenshots/Assignment2/task3_code2.png)


3. Update authcontroller to generate refresh token as well.

![Alt Text](Screenshots/Assignment2/task3_code3.png)

4. Create refreshtokendto

![Alt Text](Screenshots/Assignment2/task3_code4.png)

5. Add refresh endpoint in authcontroller

![Alt Text](Screenshots/Assignment2/task3_code5.png)

6. Test in postman

![Alt Text](Screenshots/Assignment2/task3_code6.png)

26. Add policy in program.cs

![Alt Text](Screenshots/Assignment2/task3_code7.png)

27. Update auth controller

![Alt Text](Screenshots/Assignment2/task3_code8.png)



### 4. Extend caching — Cache invalidation & ETags[ Invalidate cache on POST/PUT/DELETE, add ETag support for conditional GET requests]

1. Add to program.cs

![Alt Text](Screenshots/Assignment2/task4_code1.png)

2. Inject in productcontroller

![Alt Text](Screenshots/Assignment2/task4_code2.png)

3. Update http endpoints

![Alt Text](Screenshots/Assignment2/task4_code3.png)

4. Etag code update in productcontroller.cs

![Alt Text](Screenshots/Assignment2/task4_code4.png)



### 5. Add rate limiting middleware [ Max 100 requests/minute per user, return 429 Too Many Requests with Retry-After header]

1. Create RateLimitingMiddleware

![Alt Text](Screenshots/Assignment2/task5_code1.png)

2. Register middleware

![Alt Text](Screenshots/Assignment2/task5_code2.png)


### 6. Add bulk operations & export [ POST /api/products/bulk (batch create), GET /api/products/export?format=csv (async streaming)]

![Alt Text](Screenshots/Assignment2/task6_code1.png)

![Alt Text](Screenshots/Assignment2/task6_code2.png)


### 7. Add full-text search endpoint [ GET /api/products/search?q=laptop — search across Name, Description, Category]

![Alt Text](Screenshots/Assignment2/task7_code1.png)

![Alt Text](Screenshots/Assignment2/task7_code2.png)


### 8. Complete Postman collection + API docs + README [ Full Postman collection with test scripts & auto-generated API documentation. Newman CLI test run. README with setup instructions and architecture diagram.]

1. Register User

![Alt Text](Screenshots/Assignment2/task8_code1.png)

2. Register Admin

![Alt Text](Screenshots/Assignment2/task8_code2.png)

3. Login User

![Alt Text](Screenshots/Assignment2/task8_code3.png)

4. Login Admin

![Alt Text](Screenshots/Assignment2/task8_code4.png)

5. Refresh Token

![Alt Text](Screenshots/Assignment2/task8_code5.png)

6. Get All Products

![Alt Text](Screenshots/Assignment2/task8_code6.png)

7. ETag 

![Alt Text](Screenshots/Assignment2/task8_code7.png)

8. Get Product by id

![Alt Text](Screenshots/Assignment2/task8_code8.png)

9. Create Product [POST]

![Alt Text](Screenshots/Assignment2/task8_code9.png)

10. Update Product [PUT]

![Alt Text](Screenshots/Assignment2/task8_code10.png)

11. Delete Product [DELETE]

![Alt Text](Screenshots/Assignment2/task8_code11.png)

12. Bulk Create Product

![Alt Text](Screenshots/Assignment2/task8_code12.png)

13. Export Products as CSV

![Alt Text](Screenshots/Assignment2/task8_code13.png)

14. Search Products
	
![Alt Text](Screenshots/Assignment2/task8_code14.png)