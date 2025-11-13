using AutoMapper;
using Domain;
using Domain.Party;
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

            CreateMap<Category, CategoryEntity>();

            CreateMap<Question, QuestionEntity>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id));
        }

    }
}
