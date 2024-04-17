using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;
using static System.Net.WebRequestMethods;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        private readonly IConfiguration _configuration;

        public MappingProfiles(IConfiguration configuration)
        {
            _configuration = configuration;
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d=>d.Brand , o=> o.MapFrom(s=>s.Brand.Name))
                .ForMember(c=>c.Category , o=> o.MapFrom(s=>s.Category.Name))
                .ForMember(p=>p.PictureUrl , o => o.MapFrom<ProductPictureUrlReSolver>())
                ;
           
        }
    }
}
