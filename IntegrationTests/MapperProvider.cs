using AutoMapper;
using Infrastructure.Mappings;
using Microsoft.Extensions.Logging;

namespace IntegrationTests
{
    public static class MapperProvider
    {
        public static IMapper SetupMapper()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntityToDomainProfile>();
                cfg.AddProfile<DomainToEntityProfile>();
            }, loggerFactory);

            return configuration.CreateMapper();
        }
    }
}
