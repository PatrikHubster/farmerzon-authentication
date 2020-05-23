using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationDataAccess.Interface;
using AuthenticationErrorHandling.CustomException;
using AuthenticationManager.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using DAO = AuthenticationDataAccessModel;
using DTO = AuthenticationDataTransferModel;

namespace AuthenticationManager.Implementation
{
    public class AuthManager : IAuthManager
    {
        private UserManager<DAO.Account> UserManager { get; set; }
        private SignInManager<DAO.Account> SignInManager { get; set; }
        private IAccountRepository AccountRepository { get; set; }
        private IConfiguration Configuration { get; set; }
        private IAddressRepository AddressRepository { get; set; }
        private ICityRepository CityRepository { get; set; }
        private ICountryRepository CountryRepository { get; set; }
        private IStateRepository StateRepository { get; set; }

        public AuthManager(UserManager<DAO.Account> userManager, SignInManager<DAO.Account> signInManager, 
            IAccountRepository accountRepository, IConfiguration configuration, ICityRepository cityRepository,
            IAddressRepository addressRepository, ICountryRepository countryRepository, 
            IStateRepository stateRepository)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AccountRepository = accountRepository;
            Configuration = configuration;
            AddressRepository = addressRepository;
            CityRepository = cityRepository;
            CountryRepository = countryRepository;
            StateRepository = stateRepository;
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
                    new Claim("id", account.Id),
                    new Claim("username", account.UserName),
                    new Claim("street", account.Address.Street), 
                    new Claim("doorNumber", account.Address.DoorNumber),
                    new Claim("zipCode", account.Address.City.ZipCode),
                    new Claim("cityName", account.Address.City.Name),
                    new Claim("countryCode", account.Address.Country.Code),
                    new Claim("countryName", account.Address.Country.Name), 
                    new Claim("stateName", account.Address.State.Name)
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

        private async Task<DAO.Account> AddAccount(DTO.Registration registration)
        {
            var creationResult = await UserManager.CreateAsync(new DAO.Account
            {
                UserName = registration.UserName,
                Email = registration.Email
            }, registration.Password);
            
            if (!creationResult.Succeeded)
            {
                throw new BadRequestException(creationResult.Errors.Select(error => error.Description).ToList());
            }
            
            var managedCity = await CityRepository.FindOrInsertCityAsync(new DAO.City
            {
                Name = registration.Address.CityName,
                ZipCode = registration.Address.ZipCode
            });

            var managedCountry = await CountryRepository.FindOrInsertCountryAsync(new DAO.Country
            {
                Code = registration.Address.CountryCode,
                Name = registration.Address.CountryName
            });

            var managedState = await StateRepository.FindOrInsertStateAsync(new DAO.State
            {
                Name = registration.Address.StateName
            });

            var managedAddress = await AddressRepository.AddOrUpdateEntityAsync(new DAO.Address
            {
                City = managedCity,
                Country = managedCountry,
                State = managedState,
                DoorNumber = registration.Address.DoorNumber,
                Street = registration.Address.Street
            });

            var managedAccount = await UserManager.FindByEmailAsync(registration.Email);
            managedAccount.Address = managedAddress;
            return await AccountRepository.AddOrUpdateEntityAsync(managedAccount);
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

            var account = await AddAccount(registrationRequest);
            return GenerateAuthenticationToken(account);
        }
        
        public async Task<string> LoginAccountAsync(DTO.LoginByUserName loginByUserNameRequest)
        {
            var existingUser =
                await AccountRepository.FindAccountByUserNameAsync(loginByUserNameRequest.UserName);
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