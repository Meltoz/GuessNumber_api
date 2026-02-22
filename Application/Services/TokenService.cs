using Application.Exceptions;
using Application.Interfaces.Repository;
using Domain.User;
using Domain.ValueObjects;
using Microsoft.Win32.SafeHandles;
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
        private readonly static int accessExpires = 5;

        public async Task<Tuple<string, TokenInfo>> CreateTokenAsync(AuthUser user, IList<Claim> claims, string device, IPAddress ipAddress)
        {
            var expires = DateTime.UtcNow.AddDays(daysRefreshExpire);
            var expiresAccess = DateTime.UtcNow.AddMinutes(accessExpires);

            var id = Guid.NewGuid();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, id.ToString()));

            var accessToken = _jwtService.CreateAccessToken(claims, expiresAccess);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var token = new TokenInfo(id, refreshToken, expires, expiresAccess, user, device, ipAddress);
            var tokenInserted = await _tokenRepository.InsertAsync(token);

            return new (accessToken, tokenInserted);
            
        }

        public async Task RevokeAllTokens(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            await _tokenRepository.RevokeAllAsync(userId);
        }

        public async Task RevokeTokenById(Guid tokenId)
        {
            if(tokenId == Guid.Empty)
                throw new ArgumentNullException(nameof(tokenId));

            var exists = await _tokenRepository.GetByIdAsync(tokenId);

            if (exists is null)
                throw new EntityNotFoundException(tokenId);

            await _tokenRepository.RevokeAsync(tokenId);
        }

        public async Task RevokeSpecificTokens(Guid userId, string deviceName)
        {
            if(userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            if(string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException(nameof(deviceName));

            await _tokenRepository.RevokeAllAsync(userId, deviceName);
        }

        public async Task<TokenInfo> GetByIdAsync(Guid id)
        {
            var token  = await _tokenRepository.GetByIdWithUserAsync(id);

            if (token is null)
                throw new EntityNotFoundException(id);

            return token;
        }

        public async Task<TokenInfo> GetByRefreshToken(string refreshToken)
        {
            var refresh = Token.Create(refreshToken);
            var token = await _tokenRepository.GetByRefreshToken(refresh);

            if (token is null)
                throw new EntityNotFoundException("Token was not found");

            return token;
        }
    }
}
