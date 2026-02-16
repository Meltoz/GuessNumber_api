using Application.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Converters
{
    public class TokenValueConverter : ValueConverter<string, string>
    {
        public TokenValueConverter(AesEncryptionService encryptionService, ConverterMappingHints? mappingHints) : base(
            plainText => encryptionService.Encrypt(plainText),
            cipherText => encryptionService.Decrypt(cipherText),
            mappingHints)
        { }
    }
}
