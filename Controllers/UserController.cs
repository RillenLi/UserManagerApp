using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using UserManagerApp.Models.EntityModels;
using UserManagerApp.Models.ViewModels;
using UserManagerApp.Repository;
using UserManagerApp.Services;
using UserManagerApp.Helpers;

namespace UserManagerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizeService _authorizeService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IAuthorizeService authorizeService)
        {
            _userRepository = userRepository;
            _authorizeService = authorizeService;
            _logger = logger;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IResult> LoginUser(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid) return Results.BadRequest("Ошибка валидации");
                var response = await _authorizeService.Authorization(model.Login, model.Password);
                _logger.LogWarning($"Авторизация пользователя {User.Identity.Name}");
                return Results.Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка авторизации {@ex}", ex);
                return Results.BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("getUsers")]
        public IResult GetAllUsers()
        {
            try
            {
                var result = _userRepository.GetAll().Select(x => new UserView
                {
                    Id = x.Id,
                    FirstName = String.IsNullOrEmpty(x.FirstName) ? "" : x.FirstName,
                    Patronymic = String.IsNullOrEmpty(x.Patronymic) ? "" : x.Patronymic,
                    Surname = String.IsNullOrEmpty(x.Surname) ? "" : x.Surname,
                    Login = String.IsNullOrEmpty(x.Login) ? "" : x.Login,
                    Email = String.IsNullOrEmpty(x.Email) ? "" : x.Email,
                    BirthDate = x.BirthDate.HasValue ? x.BirthDate.Value.ToShortDateString() : "",
                }).OrderBy(x => x.FullName).ToList();
                _logger.LogWarning($"Получение таблицы пользователем {User.Identity.Name}");
                return Results.Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка получения данных {@ex}", ex);
                return Results.BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getUser")]
        public async Task<IResult> GetUser([FromQuery] string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdASync(userId);

                UserDtoModel result = new UserDtoModel
                {
                    Id = user.Id,
                    Login = user.Login,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Patronymic = user.Patronymic,
                    Surname = user.Surname,
                    Password = String.Empty,
                    BirthDate = user.BirthDate.HasValue ? user.BirthDate.Value.ToString("yyyy-MM-dd") : ""
                };
                _logger.LogWarning($"Получение данных пользователя {result.Login} пользователем {User.Identity.Name}");
                return Results.Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка получения данных {@ex}", ex);
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("saveUser")]
        [Authorize]
        public async Task<IResult> SaveUser(UserDtoModel user)
        {
            var us = User;
            if (String.IsNullOrEmpty(user.Login) || String.IsNullOrEmpty(user.Password))
            {
                _logger.LogInformation("Model not valid");
                return Results.BadRequest("Логин и пароль обязательны для заполнения");
            }
            if (String.IsNullOrEmpty(user.Id) && await _userRepository.CheckUserExist(user.Login))
            {
                return Results.BadRequest("Пользователь с таким логином уже существует");
            }

            try
            {

                var userModel = new UserModel
                {
                    Id = user.Id,
                    Login = user.Login,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Patronymic = user.Patronymic,
                    Surname = user.Surname,
                    Password = MD5Helper.GetMD5Hash(user.Password),
                    BirthDate = DateTime.TryParse(user.BirthDate, out DateTime bdate) ? bdate : null,
                };

                string? res = string.Empty;

                if (String.IsNullOrEmpty(userModel.Id))
                {
                    res = await _userRepository.AddUserAsync(userModel);
                }
                else
                {
                    res = await _userRepository.UpdateAsync(userModel);
                }
                _logger.LogWarning($"Сохранение данных пользователя {userModel.Login} пользователем {User.Identity.Name}");
                return Results.Json(res);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка сохранения данных {@ex}", ex);
                return Results.BadRequest(ex.Message);
            }
        }



        [Authorize]
        [HttpGet]
        [Route("deleteUser")]
        public async Task<IResult> DeleteUser([FromQuery] string userId)
        {
            try
            {
                var deleteLogin = (await _userRepository.GetByIdASync(userId)).Login;
                if (User.Identity.Name == deleteLogin)
                {
                    return Results.BadRequest("Пользователь с таким логином сейчас активен");
                }
                var result = await _userRepository.DeleteAsync(userId);
                _logger.LogWarning($"Удаление данных пользователя {deleteLogin} пользователем {User.Identity.Name}");
                return Results.Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка удаления данных {@ex}", ex);
                return Results.BadRequest(ex.Message);
            }
        }
    }
}
