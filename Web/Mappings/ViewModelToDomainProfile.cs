using AutoMapper;
using Domain;
using System.Globalization;
using Web.Converters;
using Web.ViewModels;

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
        }

    }
}
