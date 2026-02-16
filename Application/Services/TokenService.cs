using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain.User;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Application.Services
{
    public class TokenService(JwtService jwts, ITokenRepository tr)
    {
        private readonly JwtService _jwtService = jwts;
        private readonly ITokenRepository _tokenRepository = tr;

        private readonly static int daysRefreshExpire = 30;
        private readonly static int accessExpires = 30;

        public async Task<TokenInfo> CreateTokenAsync(AuthUser user, IList<Claim> claims, string device, IPAddress ipAddress)
        {
            var expires = DateTime.UtcNow.AddDays(daysRefreshExpire);
            var expiresAccess = DateTime.UtcNow.AddMinutes(accessExpires);

            var id = Guid.NewGuid();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, id.ToString()));

            var accessToken = _jwtService.CreateAccessToken(claims, expiresAccess);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var token = new TokenInfo(id, accessToken, refreshToken, expires, expiresAccess, user, device, ipAddress); 

            return await _tokenRepository.InsertAsync(token);
        }

        public async Task RevokeAllTokens(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            var tokens = await _tokenRepository.GetTokens(userId);

            foreach (var token in tokens)
            {
                token.RevokeToken();
                await _tokenRepository.UpdateWithOutSaveAsync(token);
            }

            await _tokenRepository.SaveAsync();
        }

        public async Task RevokeTokenById(Guid tokenId)
        {
            if(tokenId == Guid.Empty)
                throw new ArgumentNullException(nameof(tokenId));

            var token = await _tokenRepository.GetByIdAsync(tokenId);

            if (token is null)
                throw new EntityNotFoundException(tokenId);

            token.RevokeToken();
            await _tokenRepository.UpdateAsync(token);
        }

        public async Task RevokeSpecificTokens(Guid userId, string deviceName)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            if(string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException(nameof(deviceName));


            var tokens = await _tokenRepository.GetTokens(userId, deviceName);

            foreach(var token in tokens)
            {
                token.RevokeToken();
                await _tokenRepository.UpdateWithOutSaveAsync(token);
            }

            await _tokenRepository.SaveAsync();
        }
    }
}
