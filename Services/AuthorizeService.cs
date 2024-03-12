using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserManagerApp.Helpers;
using UserManagerApp.Models.ViewModels;
using UserManagerApp.Repository;

namespace UserManagerApp.Services
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IUserRepository _userRepository;
        public AuthorizeService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<AuthorizeResult> Authorization(string login, string password)
        {
            try
            {
                var passHash = MD5Helper.GetMD5Hash(password);

                var user = await _userRepository.GetByLoginAndPass(login, passHash);

                if (user == null) 
                {
                    throw new Exception("Пользователя с заданным логином и паролем не существует");
                }

                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Login)
                };

                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                var resToken = new JwtSecurityTokenHandler().WriteToken(jwt);

                return new AuthorizeResult(resToken, user.Login);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
