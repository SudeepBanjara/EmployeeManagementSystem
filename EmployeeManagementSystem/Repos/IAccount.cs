using EmployeeManagementSystem.DTOs;
using static EmployeeManagementSystem.Responses.CustomResponses;

namespace EmployeeManagementSystem.Repos
{
    public interface IAccount
    {
        Task<RegistrationResponse> RegiserAsync(RegisterDTO model);
        Task<LoginResponse> LoginAsync(LoginDTO model);
    }
}
