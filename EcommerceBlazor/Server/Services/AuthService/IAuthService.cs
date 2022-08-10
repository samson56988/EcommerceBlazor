﻿namespace EcommerceBlazor.Server.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(User user, string password);

        Task<bool> UserExist(string email);

        Task<ServiceResponse<string>> Login(string email, string password);

        Task<ServiceResponse<bool>> ChangePassword(int UserId, string Password);
    }
}