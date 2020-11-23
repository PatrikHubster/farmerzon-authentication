using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FarmerzonAuthenticationDataAccess;
using FarmerzonAuthenticationManager.Interface;
using Microsoft.EntityFrameworkCore;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthenticationManager.Implementation
{
    public class PersonManager : AbstractManager, IPersonManager
    {
        private FarmerzonAuthenticationContext Context { get; set; }

        public PersonManager(IMapper mapper, FarmerzonAuthenticationContext context) : base(mapper)
        {
            Context = context;
        }

        public async Task<IEnumerable<DTO.PersonOutput>> GetEntitiesAsync(string userName = null, string 
            normalizedUserName = null)
        {
            var people = await Context.Users
                .Where(p => (userName == null || p.UserName == userName) && 
                            (normalizedUserName == null || p.NormalizedUserName == normalizedUserName))
                .ToListAsync();

            return Mapper.Map<IEnumerable<DTO.PersonOutput>>(people);
        }
    }
}