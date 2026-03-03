using AutoMapper;
using VentasApi2026.DTOs;
using VentasApi2026.Models;

namespace VentasApi2026.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {

            CreateMap<CreateProductDto, Product>();
            CreateMap<Product, ProductDto>();

            //Update
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Product, ProductDto>();


            //Order
            CreateMap<OrderDetail, OrderItemDto>();
            CreateMap<Order, OrderDto>()
                .ForMember(dest=>dest.Items,
                            opt => opt.MapFrom(src => src.Details));

        }   
    }
}
