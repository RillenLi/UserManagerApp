using UserManagerApp.Models.EntityModels;

namespace UserManagerApp.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Получение списка пользователей
        /// </summary>
        /// <returns>Список сущностей БД</returns>
        List<UserModel> GetAll();
        /// <summary>
        /// Добавление пользователя в БД
        /// </summary>
        /// <param name="user">Модель сущности</param>
        /// <returns>Успех операции</returns>
        Task<string?> AddUserAsync(UserModel user);
        /// <summary>
        /// Получение сущности пользователя по логину - паролю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Пользователь</returns>
        Task<UserModel> GetByLoginAndPass(string login, string password);
        /// <summary>
        /// Получение сущности пользователя по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns>Пользователь</returns>
        Task<UserModel> GetByIdASync(string id);
        /// <summary>
        /// Обновление данных в БД
        /// </summary>
        /// <param name="model">Сущность пользователя</param>
        /// <returns>Успех операции</returns>
        Task<string?> UpdateAsync(UserModel model);
        /// <summary>
        /// Удаление сущности пользователя из БД
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns>Успех операции</returns>
        Task<string?> DeleteAsync(string id);
        /// <summary>
        /// Проверка существования пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns></returns>
        Task<bool> CheckUserExist(string login);
    }
}
