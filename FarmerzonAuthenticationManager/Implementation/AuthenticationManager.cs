using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapr.Client;
using Dapr.Client.Http;
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
        private UserManager<DAO.Account> UserManager { get; set; }
        private SignInManager<DAO.Account> SignInManager { get; set; }
        private DaprClient DaprClient { get; set; }
        private IConfiguration Configuration { get; set; }

        public AuthenticationManager(UserManager<DAO.Account> userManager, SignInManager<DAO.Account> signInManager, 
            DaprClient daprClient, IConfiguration configuration)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            DaprClient = daprClient;
            Configuration = configuration;
        }

        private string GenerateAuthenticationToken(string normalizedUserName, string userName, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, normalizedUserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim("username", userName)
                }),
                
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> RegisterAccountAsync(DTO.RegistrationInput registration)
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

            var result = await UserManager.CreateAsync(new DAO.Account
            {
                UserName = registration.UserName,
                Email = registration.Email
            }, registration.Password);
            
            if (!result.Succeeded)
            {
                throw new BadRequestException(result.Errors.Select(error => error.Description).ToList());
            }

            var insertedUser = await UserManager.FindByNameAsync(registration.UserName);
            var token = GenerateAuthenticationToken(insertedUser.NormalizedUserName, insertedUser.UserName,
                insertedUser.Email);
            
            var httpExtension = new HTTPExtension
            {
                ContentType = "application/json", 
                Verb = HTTPVerb.Post
            };
            httpExtension.Headers.Add("Authorization", $"Bearer {token}");

            _ = DaprClient.InvokeMethodAsync<DTO.AddressInput, DTO.AddressOutput>(
                "farmerzon-address", "address", registration.Address, httpExtension);

            return token;
        }
        
        public async Task<string> LoginAccountAsync(DTO.UserNameLoginInput userNameLogin)
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

            return GenerateAuthenticationToken(existingUser.NormalizedUserName, existingUser.UserName,
                existingUser.Email);
        }
    }
}