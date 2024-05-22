using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;
using static System.Net.WebRequestMethods;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {


        public MappingProfiles()
        {
           
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d=>d.Brand , o=> o.MapFrom(s=>s.Brand.Name))
                .ForMember(c=>c.Category , o=> o.MapFrom(s=>s.Category.Name))
                .ForMember(p=>p.PictureUrl , o => o.MapFrom<ProductPictureUrlReSolver>());

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<CustomerBasketDto, CustomerBasket>()
                .ForMember(d => d.ShippingPrice , o => o.MapFrom(s=>s.ShippingPrice))
                .ForMember(d => d.PaymentIntentId , o => o.MapFrom(s=>s.PaymentIntentId))
                .ForMember(d => d.ClientSecret , o => o.MapFrom(s=>s.ClientSecret)).ReverseMap()
                ;

            CreateMap<BasketItemDto, BasketItem>().ReverseMap();
         

            CreateMap<AddressDto, Adsress>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                    .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                    .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

            //CreateMap<OrderItem, OrderItemDto>()
            //.ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
            //.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
            //.ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))


            CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl))
            .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());

            //CreateMap<OrderItem, OrderItemDto>()
            //.ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
            //.ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
            //.ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl));


            CreateMap<OrderItem, OrderItemDto>();
   



        }
    }
}
