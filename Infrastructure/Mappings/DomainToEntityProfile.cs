using AutoMapper;
using Domain;
using Domain.Party;
using Domain.User;
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

            CreateMap<Proposal, ProposalEntity>();

            CreateMap<AuthUser, AuthUserEntity>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Mail.ToString()))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password.ToString()))
                .ForMember(dest => dest.Pseudo, opt => opt.MapFrom(src => src.Pseudo.ToString()));

            CreateMap<GuestUser, UserEntity>()
                .ForMember(dest => dest.Pseudo, opt => opt.MapFrom(src => src.Pseudo.ToString()));
        }

    }
}
