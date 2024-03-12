using MongoDB.Bson;
using MongoDB.Driver;
using UserManagerApp.Helpers;
using UserManagerApp.Models.EntityModels;

namespace UserManagerApp.DB
{
    public static class Initialize
    {
        public static async Task InitMonogoDB(IMongoDatabase _mongoDatabase)
        {
            try
            {
                string usersCollection = "Users";
                if (_mongoDatabase.GetCollection<UserModel>(usersCollection).Find(new BsonDocument()).ToList().Count == 0)
                {
                    await _mongoDatabase.GetCollection<UserModel>(usersCollection).InsertOneAsync(new UserModel
                    {
                        Login = "admin",
                        Password = MD5Helper.GetMD5Hash("admin"),
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка инициализации БД {ex.Message}");
            }
        }

    }
}
