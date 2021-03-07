using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserDAO userDao;

        public UserController(IUserDAO _userDao)
        {
            userDao = _userDao;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAllUsers()
        {
            User currentUser = new User()
            {
                UserId = Convert.ToInt32(User.FindFirst("sub").Value),
                Username = User.FindFirst("name").Value,
                PasswordHash = null,
                Salt = null,
                Email = null
            };

            List<User> users = userDao.GetUsers();

            if (users != null)
            {
                users.Remove(currentUser);

                return Ok(users);
            }

            return NotFound();
        }
    }
}
