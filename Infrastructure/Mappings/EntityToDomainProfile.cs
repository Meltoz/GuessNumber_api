using AutoMapper;
using Domain;
using Domain.Party;
using Domain.User;
using Domain.ValueObjects;
using Infrastructure.Entities;
using System.Net;

namespace Infrastructure.Mappings
{
    public class EntityToDomainProfile: Profile
    {
        public EntityToDomainProfile()
        {
            // Value Object converters
            CreateMap<string, Mail>().ConvertUsing(s => string.IsNullOrEmpty(s) ? null : Mail.Create(s));
            CreateMap<string, Password>().ConvertUsing(s => string.IsNullOrEmpty(s) ? null : Password.FromHash(s));
            CreateMap<string, Pseudo>().ConvertUsing(s => string.IsNullOrEmpty(s) ? null : Pseudo.Create(s));
            CreateMap<string, Token>().ConvertUsing(s => string.IsNullOrEmpty(s) ? null : Token.Create(s));

            CreateMap<ActualityEntity, Actuality>();

            CreateMap<CommunicationEntity, Communication>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Start))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.End));

            CreateMap<ReportEntity, Report>();

            CreateMap<CategoryEntity, Category>();

            CreateMap<QuestionEntity, Question>();

            CreateMap<ProposalEntity, Proposal>();

            CreateMap<UserEntity, GuestUser>();

            CreateMap<AuthUserEntity, AuthUser>()
                .ForMember(dest => dest.Mail, opt => opt.MapFrom(src => src.Email));

            CreateMap<TokenEntity, TokenInfo>()
                .ForMember(dest => dest.IpAdress, opt => opt.MapFrom(src => IPAddress.Parse(src.IpAddress)));
        }
    }
}
                                                            