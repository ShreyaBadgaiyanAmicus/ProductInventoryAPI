using Microsoft.AspNetCore.Mvc;
using ProductInventoryAPI.DTO;
using ProductInventoryAPI.Models;
using FluentValidation;
using ProductInventoryAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ProductInventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IValidator<CreateProductDTO> _createValidator;
        private readonly IValidator<UpdateProductDTO> _updateValidator;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ProductsController(
            IProductRepository repository,
            ISupplierRepository supplierRepository,
            IValidator<CreateProductDTO> createValidator,
            IValidator<UpdateProductDTO> updateValidator,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            _repository = repository;
            _supplierRepository = supplierRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            const string cacheKey = "products";

            List<ProductResponseDto> products;

            if (_memoryCache.TryGetValue(
                cacheKey,
                out List<ProductResponseDto>? cachedProducts))
            {
                products = cachedProducts;
            }
            else
            {
                var dbProducts = await _repository.GetAll();

                products = _mapper.Map<List<ProductResponseDto>>(dbProducts);

                _memoryCache.Set(
                    cacheKey,
                    products,
                    TimeSpan.FromMinutes(5)
                );
            }

            var etag = GenerateETag(products);

            Response.Headers.ETag = etag;

            if (Request.Headers.IfNoneMatch == etag)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }

            return Ok(products);
        }

        private string GenerateETag(List<ProductResponseDto> products)
        {
            var json = JsonSerializer.Serialize(products);

            var hash = SHA256.HashData(
                Encoding.UTF8.GetBytes(json)
            );

            return $"\"{Convert.ToHexString(hash)}\"";
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ProductResponseDto>(product);

            return Ok(response);
        }

        [Authorize(Policy = "CanManageInventory")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDTO dto)
        {
            var validationResult = _createValidator.Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!await _supplierRepository.AllExist(dto.SupplierIds))
            {
                return BadRequest("One or more supplier IDs do not exist.");
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                CreatedDate = DateTime.Now
            };

            var suppliers = await _supplierRepository.GetByIds(dto.SupplierIds);

            foreach (var supplier in suppliers)
            {
                product.Suppliers.Add(supplier);
            }

            await _repository.Add(product);

            _memoryCache.Remove("products");

            var response = _mapper.Map<ProductResponseDto>(product);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                response
            );
        }


        [Authorize(Policy = "CanManageInventory")]
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateProductsBulk(
        List<CreateProductDTO> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest("At least one product is required.");
            }

            foreach (var dto in dtos)
            {
                var validationResult = _createValidator.Validate(dto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                if (!await _supplierRepository.AllExist(dto.SupplierIds))
                {
                    return BadRequest(
                        "One or more supplier IDs do not exist."
                    );
                }
            }

            var products = new List<Product>();

            foreach (var dto in dtos)
            {
                var product = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    CategoryId = dto.CategoryId,
                    CreatedDate = DateTime.Now
                };

                var suppliers = await _supplierRepository
                    .GetByIds(dto.SupplierIds);

                foreach (var supplier in suppliers)
                {
                    product.Suppliers.Add(supplier);
                }

                products.Add(product);
            }
            await _repository.AddRange(products);

            _memoryCache.Remove("products");

            return Ok("Products created successfully.");
        }
        
        [Authorize(Policy = "CanManageInventory")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            UpdateProductDTO dto)
        {
            var validationResult = _updateValidator.Validate(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!await _supplierRepository.AllExist(dto.SupplierIds))
            {
                return BadRequest("One or more supplier IDs do not exist.");
            }

            var product = await _repository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.CategoryId = dto.CategoryId;

            product.Suppliers.Clear();

            var suppliers = await _supplierRepository.GetByIds(dto.SupplierIds);

            foreach (var supplier in suppliers)
            {
                product.Suppliers.Add(supplier);
            }

            await _repository.Update(product);
            _memoryCache.Remove("products");
            return NoContent();
        }

        [Authorize(Policy = "CanManageInventory")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            await _repository.Delete(product);
            _memoryCache.Remove("products");
            return NoContent();
        }

        [Authorize]
        [HttpGet("export")]
        public async Task ExportProducts(string format)
        {
            if (format.ToLower() != "csv")
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("Only CSV format is supported.");
                return;
            }

            var products = await _repository.GetAll();

            Response.ContentType = "text/csv";
            Response.Headers.ContentDisposition =
                "attachment; filename=products.csv";

            await Response.WriteAsync(
                "Id,Name,Description,Price,Quantity,CategoryId\r\n"
            );

            foreach (var product in products)
            {
                await Response.WriteAsync(
                    $"{product.Id}," +
                    $"\"{product.Name}\"," +
                    $"\"{product.Description}\"," +
                    $"{product.Price}," +
                    $"{product.Quantity}," +
                    $"{product.CategoryId}\r\n"
                );
            }
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Search query is required.");
            }

            var products = await _repository.Search(q);

            var response = _mapper.Map<List<ProductResponseDto>>(products);

            return Ok(response);
        }
    }
}