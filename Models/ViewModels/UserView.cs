namespace UserManagerApp.Models.ViewModels
{
    public class UserView
    {
        public string Id { get;set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string BirthDate { get;set; }
        public string FullName
        {
            get
            {
                string fname = String.IsNullOrEmpty(FirstName) ? "" : $"{FirstName[0]}. ";
                string pname = String.IsNullOrEmpty(Patronymic) ? "" : $"{Patronymic[0]}. ";
                string sname = String.IsNullOrEmpty(Surname) ? "" : $"{Surname} ";
                return $"{sname}{fname}{pname}";
            }
        }
    }
}
