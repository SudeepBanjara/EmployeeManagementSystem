using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.DTOs;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static EmployeeManagementSystem.Responses.CustomResponses;

namespace EmployeeManagementSystem.Repos
{
    public class Account : IAccount
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration config;

        public Account(AppDbContext appDbContext, IConfiguration config)
        {
            this.appDbContext = appDbContext;
            this.config = config;
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser != null) return new LoginResponse(false, "User Alrady Exists");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser.Password)) 
                return new LoginResponse(false, "Email/Password did not Match");

            string jwtToken = GenerateToken(findUser);
            return new LoginResponse(true, "Login SuccessFull", jwtToken);
        }

        public async Task<RegistrationResponse> RegiserAsync(RegisterDTO model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser != null) return new RegistrationResponse(false, "User Alrady Exists");

            appDbContext.Users.Add(
                new ApplicationUser()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
                });

            await appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Success");
        }

        private string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),

            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"]!,
                audience: config["Jwt:Audience"]!,
                claims: userClaims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> GetUser(string email)
            => await appDbContext.Users.FirstOrDefaultAsync(e => e.Email == email);


    }
}
