namespace UserManagerApp.Models.ViewModels
{
    public class AuthorizeResult
    {
        public AuthorizeResult(string token, string userName) 
        {
            Token = token;
            UserName = userName;
        }
        public string Token { get; set; }
        public string UserName { get; set; }    
    }
}
