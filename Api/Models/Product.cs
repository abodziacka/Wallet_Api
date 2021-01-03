using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Amount { get; set; }

        public double Price { get; set; }

        public int CategoryId { get; set; }
        public int BillId { get; set; }


    }
}
