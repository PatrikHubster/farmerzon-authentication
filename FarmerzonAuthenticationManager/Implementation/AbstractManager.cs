using AutoMapper;

namespace FarmerzonAuthenticationManager.Implementation
{
    public abstract class AbstractManager
    {
        protected IMapper Mapper { get; set; }
        
        protected AbstractManager(IMapper mapper)
        {
            Mapper = mapper;
        } 
    }
}