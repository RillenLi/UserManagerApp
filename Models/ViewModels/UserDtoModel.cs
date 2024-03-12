using System.ComponentModel.DataAnnotations;

namespace UserManagerApp.Models.ViewModels
{
    public class UserDtoModel
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? BirthDate { get; set; }
    }
}
