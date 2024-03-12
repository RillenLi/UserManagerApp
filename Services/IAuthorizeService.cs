using UserManagerApp.Models.ViewModels;

namespace UserManagerApp.Services
{
    public interface IAuthorizeService
    {
        Task<AuthorizeResult> Authorization(string username, string password);
    }
}
