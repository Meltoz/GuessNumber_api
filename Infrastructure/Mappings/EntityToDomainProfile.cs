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

            CreateMap<UserEntity, Domain.User.User>()
                .ConstructUsing((src, ctx) => ctx.Mapper.Map<GuestUser>(src));

            CreateMap<PlayerEntity, Player>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Pseudo, opt => opt.MapFrom(src => src.Pseudo))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar));


            CreateMap<GameEntity, Game>()
                .ConstructUsing((src, ctx) => new Game(src.Id, src.Code, src.Status, src.Type, src.TotalQuestion, src.MaxPlayers))
                .ForMember(dest => dest.Players, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest => dest.Settings, opt => opt.Ignore())
                .AfterMap((src, dest, ctx) =>
                {
                    if (src.Players != null)
                        dest.InitializePlayers(ctx.Mapper.Map<List<Player>>(src.Players));

                    if (src.Categories != null)
                    {
                        dest.Settings.InitializeCategories(src.Categories.Select(gc => gc.CategoryId));

                        var resolved = src.Categories
                            .Where(gc => gc.Category != null)
                            .Select(gc => ctx.Mapper.Map<Category>(gc.Category));
                        dest.InitializeResolvedCategories(resolved);
                    }
                });
        }
    }
}
                                                            