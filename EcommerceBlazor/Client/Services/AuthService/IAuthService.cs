namespace EcommerceBlazor.Client.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegister request);

        Task<ServiceResponse<string>> Login(UserLoginModel request);

        Task<ServiceResponse<bool>> ChangePassword(UserChangePassword request);
    }
}
