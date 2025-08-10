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
            var allowedroles = new[] { "admin", "seller", "buyer" };
            var allowedDivisions = new[]{"dhaka", "mymensingh", "khulna", "barisal", "chittagong", "rajshahi", "rangpur", "sylhet"};

            if (string.IsNullOrEmpty(user.PhoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^(?:\+8801|01)[3-9]\d{8}$"))
            {
                return BadRequest(new { message = "Invalide Phone number format" });
            }
            if (string.IsNullOrEmpty(user.Role) || !allowedroles.Contains(user.Role.ToLower()))
            {
                return BadRequest(new { message = "Role must be admin, seller, or buyer." });
            }
            if (string.IsNullOrEmpty(user.Address) || !allowedDivisions.Contains(user.Address.ToLower()))
            {
                return BadRequest(new { message = "Address must be a valid division of Bangladesh." });
            }
            if (string.IsNullOrEmpty(user.Password) || !System.Text.RegularExpressions.Regex.IsMatch(user.Password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$"))
            {
                return BadRequest(new
                {
                    message = "Password must contain at least one letter, one number, one special character, and be at least 6 characters long."
                });
            }

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

        [HttpPatch("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto newUser)
        {
            // 1. Find the existing user
            var user = db.users.Find(id);
            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    StatusCode = 404,
                    message = "User not found"
                });
            }
            if (!string.IsNullOrEmpty(newUser.PhoneNumber) &&
                !System.Text.RegularExpressions.Regex.IsMatch(newUser.PhoneNumber, @"^(?:\+8801|01)[3-9]\d{8}$"))
            {
                return BadRequest(new { message = "Invalid phone number format. Must provide valid Bangladeshi phone number " });
            }

            if (!string.IsNullOrEmpty(newUser.Role))
            {
                var allowedRoles = new[] { "admin", "seller", "buyer" };
                if (!allowedRoles.Contains(newUser.Role.ToLower()))
                    return BadRequest(new { message = "Role must be admin, seller, or buyer." });
            }

            if (!string.IsNullOrEmpty(newUser.Address))
            {
                var allowedDivisions = new[]
                {
            "dhaka", "mymensingh", "khulna", "barisal",
            "chittagong", "rajshahi", "rangpur", "sylhet"
        };
                if (!allowedDivisions.Contains(newUser.Address.ToLower()))
                    return BadRequest(new { message = "Address must be a valid division of Bangladesh." });
            }

            if (!string.IsNullOrEmpty(newUser.Password) &&
                !System.Text.RegularExpressions.Regex.IsMatch(newUser.Password, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$"))
            {
                return BadRequest(new
                {
                    message = "Password must contain at least one letter, one number, one special character, and be at least 6 characters long."
                });
            }

            if (newUser.Name != null)
                user.Name = newUser.Name;

            if (newUser.PhoneNumber != null)
                user.PhoneNumber = newUser.PhoneNumber;

            if (newUser.Role != null)
                user.Role = newUser.Role;

            if (newUser.Address != null)
                user.Address = newUser.Address;

            if (newUser.Password != null)
                user.Password = newUser.Password;

            db.SaveChanges();

            return Ok(new
            {
                success = true,
                StatusCode = 200,
                message = "User updated successfully",
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
