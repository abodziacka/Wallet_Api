using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Bill
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Shop { get; set; }

        public string City { get; set; }

        public DateTime Date { get; set; }

        public List<Product> Products { get; set; }

        


    }
}
