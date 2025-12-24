using AutoMapper;
using Domain;
using Domain.Party;
using Web.ViewModels;
using Web.ViewModels.Admin;

namespace Web.Mappings
{
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile()
        {
            CreateMap<Actuality, ActualityVM>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartPublish.ToString("o")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndPublish.HasValue ? src.EndPublish.Value.ToString("o") : null));

            CreateMap<Actuality, ActualityAdminVM>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<Communication, CommunicationAdminVM>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.HasValue ? src.StartDate.Value.ToString("o") : null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString("o") : null));

            CreateMap<Report, ReportVM>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context.ToString()));

            CreateMap<Category, CategoryAdminVM>();

            CreateMap<Question, QuestionAdminVM>();

            CreateMap<Proposal, ProposalAdminVM>();
        }
    }
}
