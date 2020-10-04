using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SampleCore.Models;
using SampleCore.Utils;
using System;

namespace SampleCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        
        
    private readonly AppDbContext appDbContext;

        //constructor
        public RegisterController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;

            //Condition for checking users exists
            if (!appDbContext.Users.Any())
            {
                //Users
                var users = new List<User>      
                {
                    new User() { Email = "a@b.co", Password = PasswordHash.HashPassword("asdefqwer") },
                    new User() { Email = "b@b.co", Password = PasswordHash.HashPassword("asdefqwer") },
                    new User() { Email = "c@b.co", Password = PasswordHash.HashPassword("asdefqwer") },
                };
                //Add user to database
                appDbContext.Users.AddRange(users);
                //save
                appDbContext.SaveChanges();
            }
        }

        // GET for register
    
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            IEnumerable<User> users = Array.Empty<User>();
            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("UserId");

            if ((userId ?? 0) > 0)
                users = await appDbContext.Users
                                          .Select(u => new User { Id = u.Id, Email = u.Email, Addresses = u.Addresses })
                                          .ToListAsync();

            return users;
        }

  
        
        //Response object
        public class PostResponse
        {
            public int Id { get; set; }
        }
        // POST for register
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] User newUser)
        {
            var ps = new PostResponse();

            //Condition for checking email exists
            if (!appDbContext.Users.Any(u => u.Email == newUser.Email))
            {
                newUser.Password = PasswordHash.HashPassword(newUser.Password);
                appDbContext.Users.Add(newUser);
                await appDbContext.SaveChangesAsync();               
                ps.Id = newUser.Id;
                await HttpContext.Session.LoadAsync();
                HttpContext.Session.SetInt32("UserId", newUser.Id);
                await HttpContext.Session.CommitAsync();
            }
            return ps;
        }
    //Response
        public class PutResponse
        {
            public bool Success { get; set; }
            public User User { get; set; }
        }

    
        [HttpPut("{id}")]
        public async Task<PutResponse> Put(int id, [FromBody] User updatedUser)
        {
            var ps = new PutResponse();
            //find the user to update
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            //if user is found
            if (user != null)
            {
                user.Email = updatedUser.Email;
                user.Password = updatedUser.Password;
                await appDbContext.SaveChangesAsync();
                ps.Success = true;
                ps.User = user;
            }

            return ps;
        }

        //Delete
        [HttpDelete("{id}")]
        public async Task<PostResponse> Delete(int id)
        {
            var pr = new PostResponse();
            //find the user to delete
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                //Delete user from collection
                appDbContext.Users.Remove(user);
                //save
                await appDbContext.SaveChangesAsync();
                pr.Id = user.Id;
            }

            return pr;
        }
    }
}
