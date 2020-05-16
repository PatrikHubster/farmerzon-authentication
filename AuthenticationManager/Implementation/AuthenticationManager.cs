using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationErrorHandling.CustomException;
using AuthenticationManager.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using DAO = AuthenticationDataAccessModel;
using DTO = AuthenticationDataTransferModel;

namespace AuthenticationManager.Implementation
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private UserManager<DAO.Account> UserManager { get; set; }
        private SignInManager<DAO.Account> SignInManager { get; set; }
        private IConfiguration Configuration { get; set; }

        public AuthenticationManager(UserManager<DAO.Account> userManager, SignInManager<DAO.Account> signInManager,
            IConfiguration configuration)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Configuration = configuration;
        }

        private string GenerateAuthenticationToken(DAO.Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, account.NormalizedUserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, account.Email),
                    new Claim(JwtRegisteredClaimNames.GivenName, account.UserName),
                    new Claim("id", account.Id),
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

        public async Task<string> RegisterAccountAsync(DTO.Registration registrationRequest)
        {
            var errors = new List<string>();
            var existingEmail = await UserManager.FindByEmailAsync(registrationRequest.Email);
            if (existingEmail != null)
            {
                errors.Add("This email already exists.");
            }
            
            var existingUserName = await UserManager.FindByNameAsync(registrationRequest.UserName);

            if (existingUserName != null)
            {
                errors.Add("This username already exists.");
            }

            if (errors.Count != 0)
            {
                throw new BadRequestException(errors);
            }
            
            var account = new DAO.Account
            {
                UserName = registrationRequest.UserName,
                Email = registrationRequest.Email
            };

            var createdUser = await UserManager.CreateAsync(account, 
                registrationRequest.Password);
            if (!createdUser.Succeeded)
            {
                throw new BadRequestException(createdUser.Errors.Select(error => error.Description).ToList());
            }

            return GenerateAuthenticationToken(account);
        }
        
        public async Task<string> LoginAccountAsync(DTO.LoginByUserName loginByUserNameRequest)
        {
            var existingUser = await UserManager.FindByNameAsync(loginByUserNameRequest.UserName);
            if (existingUser == null)
            {
                throw new BadRequestException("An incorrect password or username was used.");
            }

            var validPassword = 
                await UserManager.CheckPasswordAsync(existingUser, loginByUserNameRequest.Password);
            if (!validPassword)
            {
                throw new BadRequestException("An incorrect password or username was used.");
            }

            return GenerateAuthenticationToken(existingUser);
        }
    }
}