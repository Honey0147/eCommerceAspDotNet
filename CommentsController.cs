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
    public class CommentsController : ControllerBase
    {

        //Access to the database
        private readonly AppDbContext appDbContext;

        public CommentsController(AppDbContext appDbContext)
        {
            
            this.appDbContext = appDbContext;


        }

        
        [HttpGet]
        public async Task<IEnumerable<Comments>> Get()
        {
            IEnumerable<Comments> comments = Array.Empty<Comments>();
            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("LoginId");

            if ((userId ?? 0) > 0)

                comments = await appDbContext.Comments
                                         .Select(p => new Comments { Id = p.Id, Product = p.Product, User = p.User, rating = p.rating, text=p.text })
                                         .ToListAsync();

            return comments;

        }

        

        //Response object creation
        public class PostResponse
        {
            public int Id { get; set; }
        }

        // POST api/Register
        
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] Comments newComments)
        {
            var ps = new PostResponse();

            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("LoginId");
            var pId = HttpContext.Session.GetInt32("ProductId");

            var prodId = await appDbContext.Products.Where(p => p.Id == pId).FirstOrDefaultAsync();
            var uId = await appDbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();


           

            if (userId > 0)
            {

                

                newComments.User = uId;
                newComments.Product = prodId;

                appDbContext.Comments.Add(newComments);
                await appDbContext.SaveChangesAsync();
                
                ps.Id = newComments.Id;
                
            }

            return ps;
        }

        
        public class PutResponse
        {
            public bool Success { get; set; }
            public Comments comments { get; set; }
        }



        // DELETE api/<RegisterController>/5
        [HttpDelete("{id}")]
        public async Task<PostResponse> Delete(int id)
        {
            var pr = new PostResponse();
            //Finding the product to be deleted
            var comments = await appDbContext.Comments.FirstOrDefaultAsync(p => p.Id == id);

            if (comments != null)
            {
                //remove the user from the list
                appDbContext.Comments.Remove(comments);
                //save the updated list
                await appDbContext.SaveChangesAsync();
                pr.Id = comments.Id;
            }
            return pr;
        }
    }
}
