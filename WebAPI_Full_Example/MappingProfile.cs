using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace WebAPI_Full_Example
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAdress,
                    opt => opt.MapFrom(src => $"{src.Address}, {src.Country}"));
        }
    }
}
