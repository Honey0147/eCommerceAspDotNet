using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleCore.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //[ForeignKey("Address")]
        //public int AddressId { get; set; }
        //public Address Address { get; set; }

        public List<Address> Addresses { get; set; }
    }
}
