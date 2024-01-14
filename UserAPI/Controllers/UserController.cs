using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Repositories;
using System.Net;
using UserAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
   // [Authorize(Roles = "Admin")]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private IRepository<User> _repository;
        private AppDbContext _Dbcontext;

        public UserController(IRepository<User> repository ,AppDbContext appDbContext)
        {
            _repository = repository;
            _Dbcontext = appDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var isAdmin = _Dbcontext.Users.Where(u => u.IsAdmin).Any();

            if (isAdmin)
            {
                var users = _repository.Get();
                return Ok(users);
            }
            else
            {
                return Unauthorized();
            }


        }

        // GET api/<CustomersController>/5
        [HttpGet("{guid}")]
        public ActionResult Get(Guid guid)
        {
            var user = _repository.Get(guid);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult Post([FromForm] User newUser)
        {
            if (!ModelState.IsValid)
            {
                
                return BadRequest(ModelState);
            }
            try
            {
               
                     _repository.Add(newUser);
                    
                     return Created("Added user", newUser);
            
         
            }
            catch (Exception)
            {

                throw ;
            }
            

        }

        [HttpPut("{guid}")]
        public ActionResult Put(Guid guid, [FromForm] User user)
        {
            if (guid == Guid.Empty || user == null || user.Id != guid)
            {
                return BadRequest("Invalid request data");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool isUpdate = _repository.Update(user);

                if (user == null)
                {
                    return NotFound();
                }
                if (isUpdate)
                {
                    return Ok(new { Message = "User Updated successfully" });
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        new { Message = "Update failed" });
                }
            }
            catch (Exception ex)
            {
                
                return NotFound();
            }
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{guid}")]
        public ActionResult Delete(Guid guid)
        {
            try
            {
                bool isDeleted = _repository.Delete(guid);

                if (isDeleted)
                {
                    return Ok(new { Message = "User Deleted successfully" });
                }
                else
                {
                    
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


       
        [HttpPost("SearchUsers")]
        //[Authorize(Roles = "Admin")] // Access only for Admin Users
        public IActionResult SearchUsers([FromBody] UserSearchFilters searchFilters, int page = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            try
            {
                
                var users = _repository.Get();

                // Apply filters
                if (!string.IsNullOrEmpty(searchFilters.FieldName) && !string.IsNullOrEmpty(searchFilters.FieldValue))
                {
                    // Apply the filter condition based on FieldName and FieldValue
                    switch (searchFilters.FieldName.ToLower())
                    {
                        case "username":
                            users = users.Where(u => u.Username == searchFilters.FieldValue);
                            break;

                        case "id":
                            // Id is a Guid
                            if (Guid.TryParse(searchFilters.FieldValue, out var userId))
                            {
                                users = users.Where(u => u.Id == userId);
                            }
                            break;

                        case "isadmin":
                            if (bool.TryParse(searchFilters.FieldValue, out var isAdmin))
                            {
                                users = users.Where(u => u.IsAdmin == isAdmin);
                            }
                            break;

                        case "age":
                            if (int.TryParse(searchFilters.FieldValue, out var age))
                            {
                                users = users.Where(u => u.Age == age);
                            }
                            break;

                        case "hobbies":
                            users = users.Where(u => u.Hobbies.Contains(searchFilters.FieldValue));
                            break;

                        

                        default:
                            // Handle unknown field name, or you can ignore it
                            break;
                    }
                }

                // Apply sorting
                users = ApplySorting(users, sortBy, sortOrder);

                // Apply pagination
                var pagedUsers = ApplyPagination(users, page, pageSize);

                return Ok(pagedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = ex.Message });
            }
        }

        // Helper method to apply sorting
        private IEnumerable<User> ApplySorting(IEnumerable<User> users, string sortBy, string sortOrder)
        {
            // Implement sorting logic based on sortBy and sortOrder
             users = sortOrder == "asc" ? users.OrderBy(u => u.Username) : users.OrderByDescending(u => u.Username);

            return users;
        }

        // Helper method to apply pagination
        private IEnumerable<User> ApplyPagination(IEnumerable<User> users, int page, int pageSize)
        {
            // Calculate skip and take values based on page and pageSize
            var skip = (page - 1) * pageSize;
            var pagedUsers = users.Skip(skip).Take(pageSize).ToList();

            return pagedUsers;
        }

    }
}
