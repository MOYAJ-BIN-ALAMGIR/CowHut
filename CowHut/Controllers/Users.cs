using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CowHut.Models;
using Microsoft.EntityFrameworkCore;
namespace CowHut.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Users : ControllerBase
    {
        private readonly ApplicationDbContext db;
        public Users(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpPost("signup")]
        public IActionResult CreateUser(User user)
        {
            db.users.Add(user);
            db.SaveChanges();
            return Ok(new
            {
                success = true,
                StatusCode = 200,
                message = "user created successfully",
                data = user
            });
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var user = db.users.ToList();
            return Ok(new
            {
                success = true,
                StatusCode = 200,
                messag = "Users retrived successfull",
                data = user 
            });
        }

        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var user = db.users.Find(id);
            if(user == null)
            {
                return NotFound(new
                {
                    success = false,
                    statuscode = 404,
                    message = "user not found"
                });
            }

            db.users.Remove(user);
            db.SaveChanges();
            return Ok(new
            {
                success = true,
                StatusCode = 200,
                message = "user deleted successfully",
                data = user
            });
        }

        [HttpPatch]
        public IActionResult UpdateUser(int id, User newuser)
        {
            var user = db.users.Find(id);
            if(user == null)
            {
                return NotFound(new
                {
                    success = false,
                    StatusCode = 404,
                    message = "User not found"
                });
            }

            user.PhoneNumber = newuser.PhoneNumber ?? user.PhoneNumber;
            user.Role = newuser.Role ?? user.Role;
            user.Password = newuser.Password ?? user.Password;
            user.Name = newuser.Name ?? user.Name;
            user.Address = newuser.Address ?? user.Address;

            db.SaveChanges();

            return Ok(new
            {
                success = true,
                StatusCode = 200,
                message = "User Updated Successfully",
                data = user
            });
        }

        [HttpGet("search")]
        public IActionResult SearchUser(
            string ? phonenumber,
            string ? name,
            string ? address,
            string ? role)
        {
            var query = db.users.AsQueryable();

            if(!string.IsNullOrEmpty(phonenumber))
            {
                query = query.Where(u => u.PhoneNumber == phonenumber);
            }
            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name == name);
            }
            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(u => u.Address == address);
            }
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }
            var result = query.ToList();
            return Ok(new
            {
                success = true,
                StatusCode = 200,
                total = result.Count,
                data = result
            });
        }
    }
}
