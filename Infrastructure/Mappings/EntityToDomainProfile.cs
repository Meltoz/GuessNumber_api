using AutoMapper;
using Domain;
using Domain.Party;
using Infrastructure.Entities;

namespace Infrastructure.Mappings
{
    public class EntityToDomainProfile: Profile
    {
        public EntityToDomainProfile()
        {
            CreateMap<ActualityEntity, Actuality>();

            CreateMap<CommunicationEntity, Communication>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Start))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<ReportEntity, Report>();

            CreateMap<CategoryEntity, Category>();
        }
    }
}
