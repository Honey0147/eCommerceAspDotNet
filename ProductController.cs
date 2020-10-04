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
    public class ProductController : ControllerBase
    {
       
 private readonly AppDbContext appDbContext;

        //constructor
        public ProductController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;

            //Condition if we do not have produces in db
            if (!appDbContext.Products.Any())
            {
                //create products
                var products = new List<Product>
                {
                    new Product() { Discription = "Redmi1",  Image ="img1.jpg", Price = "280", Shipping_Cost="30" },
                    new Product() { Discription = "S20 Ultra",  Image ="img2.jpg", Price = "500", Shipping_Cost="80" },
                    new Product() { Discription = "Iphone 11 pro Max",  Image ="img3.jpg", Price = "2000", Shipping_Cost="20" },
                };

                //add products to db
                appDbContext.Products.AddRange(products);
                //save
                appDbContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            IEnumerable<Product> products = Array.Empty<Product>();
            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("LoginId");

            if ((userId ?? 0) > 0)
            
                products = await appDbContext.Products
                                         .Select(p => new Product { Id = p.Id, Discription = p.Discription, Image = p.Image , Price = p.Price, Shipping_Cost = p.Shipping_Cost })
                                         .ToListAsync();

            return products;
            
        }

        //Response object
        public class PostResponse
        {
            public int Id { get; set; }
        }    
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] Product newProduct)
        {
            var ps = new PostResponse();

            await HttpContext.Session.LoadAsync();
            var userId = HttpContext.Session.GetInt32("LoginId");

            if (userId > 0)
            {
                
                appDbContext.Products.Add(newProduct);
                await appDbContext.SaveChangesAsync();                
                ps.Id = newProduct.Id;                
            }

            return ps;
        }

        //Response
        public class PutResponse
        {
            public bool Success { get; set; }
            public Product Product { get; set; }
        }

    
        [HttpPut("{id}")]
        public async Task<PutResponse> Put(int id, [FromBody] Product updateProduct)
        {
            var ps = new PutResponse();
            //Product to update
            var product = await appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            //condition if product found
            if (product != null)
            {                
                product.Discription = updateProduct.Discription;
                product.Image = updateProduct.Image;
                product.Price = updateProduct.Price;
                product.Shipping_Cost = updateProduct.Shipping_Cost;
                
                await appDbContext.SaveChangesAsync();
                ps.Success = true;             
                ps.Product = product;
            }
            return ps;
        }

    //Delete
        [HttpDelete("{id}")]
        public async Task<PostResponse> Delete(int id)
        {
            var pr = new PostResponse();
            //Product to delete
            var product = await appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                //Delete product from collection
                appDbContext.Products.Remove(product);
                //save
                await appDbContext.SaveChangesAsync();
                pr.Id = product.Id;
            }
            return pr;
        }
    }
}