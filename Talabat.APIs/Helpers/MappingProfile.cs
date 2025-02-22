using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order;

namespace Talabat.APIs.Helpers
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(D => D.Brand, O => O.MapFrom(S => S.Brand.Name))
                .ForMember(D => D.Category, O => O.MapFrom(S => S.Category.Name))
                //.ForMember(D => D.PictureUrl, O => O.MapFrom(S => $"{_configuration["BaseUrl"]}/{S.PictureUrl}"));
                .ForMember(D => D.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());
            CreateMap<AddressDTO, Address>();
            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(D => D.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod!.ShortName))
                .ForMember(D => D.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod!.Cost))
                .ForMember(D => D.OrderItem, O => O.MapFrom(S => S.OrderItem));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(D => D.ProductId, O => O.MapFrom(S => S.Product.Id))
                .ForMember(D => D.ProductName, O => O.MapFrom(S => S.Product.Name))
                .ForMember(D => D.PictureUrl, O => O.MapFrom<ProductInOrderPictureUrlResolver>());
            CreateMap<UserAddress, AddressDTO>().ReverseMap();
        }
    }
}
