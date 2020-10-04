using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleCore.Models;
using SampleCore.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SampleCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly AppDbContext appDbContext;

        public LoginController(AppDbContext context)
        {
            this.appDbContext = context;
        }

      
        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        public class PostResponse
        {
           
            public int Id { get; set; }
        }

        // POST api/<LoginController>
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] User loginUser)
        {
            var ps = new PostResponse();

            var dbUser = appDbContext.Users.Where(u => u.Email == loginUser.Email).FirstOrDefault();
            if (dbUser != null && PasswordHash.VerifyHashedPassword(dbUser.Password, loginUser.Password))
            {
                await HttpContext.Session.LoadAsync();
                HttpContext.Session.SetInt32("LoginId", dbUser.Id);
                ps.Id = dbUser.Id;
                await HttpContext.Session.CommitAsync();
            }
           
            return ps;
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
