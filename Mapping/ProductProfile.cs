using AutoMapper;
using ProductInventoryAPI.DTO;
using ProductInventoryAPI.Models;

namespace ProductInventoryAPI.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();
            CreateMap<Product, ProductResponseDto>();

            CreateMap<Supplier, Supplierdto>();
        }
    }
}