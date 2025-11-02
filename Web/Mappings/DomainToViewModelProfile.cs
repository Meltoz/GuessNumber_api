using AutoMapper;
using Domain;
using Web.ViewModels;

namespace Web.Mappings
{
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile()
        {
            CreateMap<Actuality, ActualityAdminVM>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartPublish.ToString("o")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndPublish.Value.ToString("o")));

            CreateMap<Communication, CommunicationAdminVM>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.Value.ToString("o")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.Value.ToString("o")));

            CreateMap<Report, ReportVM>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context.ToString()));
        }
    }
}
