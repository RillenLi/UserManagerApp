namespace UserManagerApp.Models.EntityModels
{
    public class UserModel : BaseMongoModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
