using AutoMapper;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d=>d.Brand , o=> o.MapFrom(s=>s.Brand.Name))
                .ForMember(c=>c.Category , o=> o.MapFrom(s=>s.Category.Name))
                ;
        }
    }
}
