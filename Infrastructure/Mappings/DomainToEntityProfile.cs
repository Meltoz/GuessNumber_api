using AutoMapper;
using Domain;
using Infrastructure.Entities;

namespace Infrastructure.Mappings
{
    public class DomainToEntityProfile : Profile
    {
        public DomainToEntityProfile() 
        {
            CreateMap<Actuality, ActualityEntity>();

            CreateMap<Communication, CommunicationEntity>()
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<Report, ReportEntity>();
        }

    }
}
