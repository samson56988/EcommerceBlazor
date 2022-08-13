using EcommerceBlazor.Server.Services.AuthService;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBlazor.Server.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly DataContext _dataContext;
        private readonly IAuthService _authService;
        public AddressService(DataContext context,IAuthService authService )
        {
            _dataContext = context;
            _authService = authService;
        }
        public async Task<ServiceResponse<Address>> AddOrUpdateAddress(Address address)
        {
            var response = new  ServiceResponse<Address>();
            var dbAddress = (await GetAddress()).Data;
            if(dbAddress == null)
            {
                address.UserId = _authService.GetUserID();
                _dataContext.Addresses.Add(address);
                response.Data = address;
            }
            else
            {
                dbAddress.FirstName =  address.FirstName;
                dbAddress.LastName = address.LastName;
                dbAddress.State = address.State;
                dbAddress.Country = address.Country;
                dbAddress.City = address.City;
                dbAddress.Zip = address.Zip;
                dbAddress.Street = address.Street;
                response.Data = dbAddress;
            }

            await _dataContext.SaveChangesAsync();

            return response;
        }

        public async Task<ServiceResponse<Address>> GetAddress()
        {
            int UserId = _authService.GetUserID();
            var address = await _dataContext.Addresses.FirstOrDefaultAsync(a => a.UserId == UserId);
            
            return new ServiceResponse<Address> { Data = address };
        }
    }
}
