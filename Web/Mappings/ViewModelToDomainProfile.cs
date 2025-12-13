using AutoMapper;
using Domain;
using Domain.Party;
using System.Globalization;
using Web.Converters;
using Web.ViewModels.Admin;

namespace Web.Mappings
{
    public class ViewModelToDomainProfile : Profile
    {
        public ViewModelToDomainProfile()
        {
            CreateMap<string, DateTime?>().ConvertUsing<DateConverter>();

            CreateMap<ActualityAdminVM, Actuality>()
                .ForMember(dest => dest.StartPublish,opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndPublish, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<CommunicationAdminVM, Communication>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<CategoryAdminVM, Category>();

            CreateMap<QuestionAdminVM, Question>();
        }

    }
}
