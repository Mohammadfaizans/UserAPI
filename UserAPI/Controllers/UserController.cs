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
            var users = _repository.Get(guid);
            return Ok(users);
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
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { Message = $"An error occurred during the update process: {ex.Message}" });
            }
        }

          

    }
}
