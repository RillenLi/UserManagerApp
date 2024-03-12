using MongoDB.Bson;
using MongoDB.Driver;
using UserManagerApp.Controllers;
using UserManagerApp.Models.EntityModels;

namespace UserManagerApp.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly ILogger<UserRepository> _logger;
        private readonly string usersCollection = "Users";
        public UserRepository(IMongoDatabase mongoDatabase, ILogger<UserRepository> logger) 
        {
            _mongoDatabase = mongoDatabase;
            _logger = logger;
        }

        public List<UserModel> GetAll()
        {
            try
            {
                return _mongoDatabase.GetCollection<UserModel>(usersCollection).Find(new BsonDocument()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Ошибка получения данных");
            }
        }

        public async Task<string?> AddUserAsync(UserModel model)
        {
            try
            {
                await _mongoDatabase.GetCollection<UserModel>(usersCollection).InsertOneAsync(model);
                return "Сохранено успешно";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Ошибка сохранения данных");
            }

        }

        public async Task<UserModel> GetByIdASync(string id)
        {
            try
            {
                return await _mongoDatabase.GetCollection<UserModel>(usersCollection).Find(x=>x.Id == id).FirstOrDefaultAsync(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Ошибка получения данных");
            }
        }

        public async Task<UserModel> GetByLoginAndPass(string login, string password)
        {
            try
            {
                return await _mongoDatabase.GetCollection<UserModel>(usersCollection).Find(x => x.Login == login && x.Password == password).FirstOrDefaultAsync(); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Ошибка получения данных");
            }
        }

        public async Task<string?> UpdateAsync(UserModel model)
        {
            try
            {
                var update = await _mongoDatabase.GetCollection<UserModel>(usersCollection).FindOneAndReplaceAsync(x => x.Id == model.Id, model, new() {ReturnDocument = ReturnDocument.After});
                if(update != null)
                {
                    return "Сохранено успешно";
                }
                else
                {
                    throw new Exception("Ошибка сохранения данных");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Ошибка сохранения данных");
            }
        }

        public async Task<string?> DeleteAsync(string id)
        {
            try
            {
                var delete = await _mongoDatabase.GetCollection<UserModel>(usersCollection).FindOneAndDeleteAsync(x => x.Id == id);
                if (delete != null)
                {
                    return "Сохранено успешно";
                }
                else
                {
                    throw new Exception("Ошибка сохранения данных");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Ошибка удаления данных");
            }
        }

        public async Task<bool> CheckUserExist(string login) 
        {   
            var user = await _mongoDatabase.GetCollection<UserModel>(usersCollection).Find(x=>x.Login == login).FirstOrDefaultAsync();
            if (user != null) return true;
            else return false;
        }
    }
}
