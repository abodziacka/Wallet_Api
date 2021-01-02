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

        public int Amount { get; set; }

        public int Price { get; set; }

        public int CategoryId { get; set; }
        public int BillId { get; set; }


    }
}
