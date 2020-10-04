using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleCore.Models
{
    public class Cart
    {

        public int Id { get; set; }
        public Product Product { get; set; }
        public User User { get; set; }
        public int Quantities { get; set; }
        
            
    }
}
