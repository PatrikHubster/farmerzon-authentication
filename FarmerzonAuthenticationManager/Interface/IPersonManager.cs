using System.Collections.Generic;
using System.Threading.Tasks;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthenticationManager.Interface
{
    public interface IPersonManager
    {
        public Task<IEnumerable<DTO.PersonOutput>> GetEntitiesAsync(string userName = null,
            string normalizedUserName = null);
    }
}