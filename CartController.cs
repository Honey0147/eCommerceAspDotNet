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
    public class CartController : ControllerBase
    {

        private readonly AppDbContext appDbContext;

        public CartController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;

            
        }

       
        [HttpGet]
        public async Task<IEnumerable<Cart>> Get()
        {
            IEnumerable<Cart> carts = Array.Empty<Cart>();
            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("LoginId");

            if ((userId ?? 0) > 0)

                carts = await appDbContext.Cart
                                         .Select(p => new Cart { Id = p.Id, Product = p.Product, User = p.User, Quantities = p.Quantities })
                                         .ToListAsync();

            return carts;

        }

        
        public class PostResponse
        {
            public int Id { get; set; }
        }
       
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] Cart newCart)
        {
            var ps = new PostResponse();

            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("LoginId");
            var pId = HttpContext.Session.GetInt32("ProductId");

            var prodId = await appDbContext.Products.Where(p => p.Id == pId ).FirstOrDefaultAsync();
            var uId = await appDbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();



          

            if (userId > 0)
            {

                newCart.User = uId;
                newCart.Product = prodId;
              
                appDbContext.Cart.Add(newCart);
                await appDbContext.SaveChangesAsync();
              
                ps.Id = newCart.Id;
               
            }

            return ps;
        }

        
        public class PutResponse
        {
            public bool Success { get; set; }
            public Cart Cart { get; set; }
        }

        [HttpDelete("{id}")]
        public async Task<PostResponse> Delete(int id)
        {
            var pr = new PostResponse();
           
            var cart = await appDbContext.Cart.FirstOrDefaultAsync(p => p.Id == id);

            if (cart != null)
            {
               
                appDbContext.Cart.Remove(cart);
              
                await appDbContext.SaveChangesAsync();
                pr.Id = cart.Id;
            }
            return pr;
        }
    }
}
