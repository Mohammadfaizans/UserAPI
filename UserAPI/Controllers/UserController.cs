using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Repositories;
using System.Net;
using UserAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Web.WebPages.Html;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
   // [Authorize] // All endpoints except login must be authorized
    public class UserController : ControllerBase
    {
        private IRepository<User> _repository;

        public UserController(IRepository<User> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            
            var users = _repository.Get();
            return Ok(users);


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
                // Return a 400 Bad Request response with validation errors
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
                // Log the exception or handle it as needed
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
                    // Consider returning a more informative message or status code here if the deletion fails.
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


    }
}
