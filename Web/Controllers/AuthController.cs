using Application.Exceptions;
using Application.Services;
using AutoMapper;
using Domain.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web.Constants;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly string _key;

        public AuthController(UserService us, TokenService ts,  IMapper m, IOptions<AuthConfiguration> config)
        {
            _userService = us;
            _tokenService = ts;
            _mapper = m;
            _key = config.Value.Key;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginVM login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AuthUser userFinded;

            try
            {
                userFinded = await _userService.ConnectUser(login.Pseudo, login.Password);
            }
            catch(Exception ex)
            {
                return Unauthorized();
            }

            var userAgent = Request.Headers.UserAgent;
            var ipAddress = HttpContext.Connection.RemoteIpAddress;

            await _tokenService.RevokeSpecificTokens(userFinded.Id, userAgent.ToString());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userFinded.Id.ToString()),
                new Claim(ClaimTypes.Role, userFinded.Role.ToString()),
                new Claim(ClaimTypes.Name, userFinded.Pseudo.ToString()),
            };

            var token = await _tokenService.CreateTokenAsync(userFinded, claims, userAgent.ToString(), ipAddress);
            Response.AppendCookie(ApiConstants.AccessTokenCookieName, token.Item1.ToString(), TimeSpan.FromMinutes(5), httpOnly: false);
            Response.AppendCookie(ApiConstants.RefreshTokenCookieName, token.Item2.RefreshToken.ToString(), TimeSpan.FromDays(30));
            
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[ApiConstants.RefreshTokenCookieName];

            if(string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "The accesstoken is invalid or malformed",
                    Instance = HttpContext.TraceIdentifier
                });
            
            TokenInfo token;
            try
            {
                token = await _tokenService.GetByRefreshToken(refreshToken);
            }
            catch (EntityNotFoundException)
            {
                return Unauthorized();
            }

            if (token.IsRevoked ||
                token.IsRefreshExpires() ||
                token.RefreshToken != refreshToken )   
            {
                try
                {
                    await _tokenService.RevokeTokenById(token.Id);
                }
                catch(InvalidOperationException)
                {
                    // LOG
                }
                Response.DeleteCookie(ApiConstants.AccessTokenCookieName, httpOnly: false, secure: true);
                Response.DeleteCookie(ApiConstants.RefreshTokenCookieName);
                return Unauthorized();
            }

            await _tokenService.RevokeTokenById(token.Id);

            var user = await _userService.GetDetail(token.User.Id) as AuthUser;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Pseudo.ToString()),
            };

            var userAgent = Request.Headers.UserAgent;
            var ipAddress = HttpContext.Connection.RemoteIpAddress;

            var newToken = await _tokenService.CreateTokenAsync(token.User, claims, userAgent.ToString(), ipAddress);

            Response.AppendCookie(ApiConstants.AccessTokenCookieName, newToken.Item1.ToString(), TimeSpan.FromMinutes(5), httpOnly: false);
            Response.AppendCookie(ApiConstants.RefreshTokenCookieName, newToken.Item2.RefreshToken.ToString(), TimeSpan.FromDays(30));
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies[ApiConstants.AccessTokenCookieName];

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest();

            var claimPrincipal = GetPrincipalFromExpiredToken(token);

            if (claimPrincipal is null)
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "The accesstoken is invalid or malformed",
                    Instance = HttpContext.TraceIdentifier
                });

            if (!Guid.TryParse(claimPrincipal.FindFirstValue(JwtRegisteredClaimNames.Jti), out var tokenId))
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "The accesstoken is invalid or malformed",
                    Instance = HttpContext.TraceIdentifier
                });

            try
            {
                await _tokenService.RevokeTokenById(tokenId);
            }
            catch (InvalidOperationException)
            {
                // LOG
            }

            Response.DeleteCookie(ApiConstants.AccessTokenCookieName, httpOnly: false, secure: true);
            Response.DeleteCookie(ApiConstants.RefreshTokenCookieName);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> LogoutAllDevice()
        {
            var token = Request.Cookies[ApiConstants.AccessTokenCookieName];

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest();

            var claimPrincipal = GetPrincipalFromExpiredToken(token);

            if (claimPrincipal is null)
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "The accesstoken is invalid or malformed",
                    Instance = HttpContext.TraceIdentifier
                });

            if (!Guid.TryParse(claimPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "The accesstoken is invalid or malformed",
                    Instance = HttpContext.TraceIdentifier
                });

            await _tokenService.RevokeAllTokens(userId);

            Response.DeleteCookie(ApiConstants.AccessTokenCookieName, httpOnly: false, secure: true);
            Response.DeleteCookie(ApiConstants.RefreshTokenCookieName);

            return Ok();
        }


        [HttpGet]
        [Authorize(Policy =ApiConstants.AuthenticatedUserPolicy)]
        public async Task<IActionResult> Me()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user =await _userService.GetDetail(userId) as AuthUser;

            if(user is null)
            {
                return BadRequest("Utilisateur non trouvé");
            }

            return Ok(_mapper.Map<AuthUserDetailVM>(user));
        }
        #region Private Methods

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidation = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidation, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurity || !jwtSecurity.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }

    public class LoginVM
    {
        [Required]
        [MinLength(3)]
        public string Pseudo { get; set; }


        [Required]
        public string Password { get; set; }
    }
}
