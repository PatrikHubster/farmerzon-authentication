using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapr.Client;
using Dapr.Client.Http;
using FarmerzonAuthenticationDataAccess;
using FarmerzonAuthenticationErrorHandling.CustomException;
using FarmerzonAuthenticationManager.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using DAO = FarmerzonAuthenticationDataAccessModel;
using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthenticationManager.Implementation
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private UserManager<IdentityUser> UserManager { get; set; }
        private SignInManager<IdentityUser> SignInManager { get; set; }
        private DaprClient DaprClient { get; set; }
        private IConfiguration Configuration { get; set; }
        private TokenValidationParameters ValidationParameters { get; set; }
        private FarmerzonAuthenticationContext Context { get; set; }

        public AuthenticationManager(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            DaprClient daprClient, IConfiguration configuration, TokenValidationParameters validationParameters, 
            FarmerzonAuthenticationContext context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            DaprClient = daprClient;
            Configuration = configuration;
            ValidationParameters = validationParameters;
            Context = context;
        }

        private async Task<DTO.TokenOutput> GenerateAuthenticationTokenAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.NormalizedUserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id), 
                    new Claim("username", user.UserName)
                }),
                
                Expires = DateTime.UtcNow.Add(new TimeSpan(0, 30, 0)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            var refreshToken = new DAO.RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMonths(6)
            };

            await Context.RefreshTokens.AddAsync(refreshToken);
            await Context.SaveChangesAsync();
            
            return new DTO.TokenOutput
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<DTO.TokenOutput> RegisterUserAsync(DTO.RegistrationInput registration)
        {
            var errors = new List<string>();
            var existingEmail = await UserManager.FindByEmailAsync(registration.Email);
            if (existingEmail != null)
            {
                errors.Add("This email already exists.");
            }
            
            var existingUserName = await UserManager.FindByNameAsync(registration.UserName);
            if (existingUserName != null)
            {
                errors.Add("This username already exists.");
            }

            if (errors.Count != 0)
            {
                throw new BadRequestException(errors);
            }

            var result = await UserManager.CreateAsync(new IdentityUser
            {
                UserName = registration.UserName,
                Email = registration.Email
            }, registration.Password);
            
            if (!result.Succeeded)
            {
                throw new BadRequestException(result.Errors.Select(error => error.Description).ToList());
            }

            var insertedUser = await UserManager.FindByNameAsync(registration.UserName);
            var tokenResult = await GenerateAuthenticationTokenAsync(insertedUser);
            
            var httpExtension = new HTTPExtension
            {
                ContentType = "application/json", 
                Verb = HTTPVerb.Post
            };
            httpExtension.Headers.Add("Authorization", $"Bearer {tokenResult.Token}");
            
            _ = DaprClient.InvokeMethodAsync<DTO.AddressInput, DTO.SuccessResponse<DTO.AddressOutput>>(
                "farmerzon-address", "address", registration.Address, httpExtension);

            return tokenResult;
        }
        
        public async Task<DTO.TokenOutput> LoginUserAsync(DTO.UserNameLoginInput userNameLogin)
        {
            var existingUser = await UserManager.FindByNameAsync(userNameLogin.UserName);
            if (existingUser == null)
            {
                throw new BadRequestException("An incorrect password or username was used.");
            }

            var validPassword = 
                await UserManager.CheckPasswordAsync(existingUser, userNameLogin.Password);
            if (!validPassword)
            {
                throw new BadRequestException("An incorrect password or username was used.");
            }

            return await GenerateAuthenticationTokenAsync(existingUser);
        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principle = tokenHandler.ValidateToken(token, ValidationParameters, out var validatedToken);
                return IsJwtWithValidSecurityAlgorithm(validatedToken) ? principle : null;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken securityToken)
        {
            return securityToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture);
        }
        
        public async Task<DTO.TokenOutput> RefreshTokenAsync(DTO.RefreshTokenInput refreshToken)
        {
            var validatedToken = GetClaimsPrincipalFromToken(refreshToken.Token);
            if (validatedToken == null)
            {
                throw new UnautherizedException("Invalid token.");
            }

            var expirationDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expirationDateTimeUtc =
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expirationDateUnix);

            if (expirationDateTimeUtc > DateTime.UtcNow)
            {
                throw new UnautherizedException("This token is still valid.");
            }
            
            var storedRefreshToken = Context.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken.RefreshToken);
            if (storedRefreshToken == null)
            {
                throw new UnautherizedException("This token does not exist.");
            }
            
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var errors = new List<string>();
            if (DateTime.UtcNow > storedRefreshToken.ExpirationDate)
            {
                errors.Add("This refresh token has expired.");
            }

            if (storedRefreshToken.Invalidated)
            {
                errors.Add("This refresh token has been invalidated.");
            }

            if (storedRefreshToken.Used)
            {
                errors.Add("This refresh token has been used.");
            }

            if (storedRefreshToken.JwtId != jti)
            {
                errors.Add("This refresh token does not match this jwt token.");
            }

            if (errors.Count != 0)
            {
                throw new UnautherizedException(errors);
            }

            storedRefreshToken.Used = true;
            Context.RefreshTokens.Update(storedRefreshToken);
            await Context.SaveChangesAsync();
            
            var user = await UserManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationTokenAsync(user);
        }
    }
}