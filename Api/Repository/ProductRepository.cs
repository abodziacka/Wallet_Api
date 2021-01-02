using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Repository
{
    public class ProductRepository
    {

        private readonly AuthenticationContext _context;

        public ProductRepository(AuthenticationContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public Product Get(int id)
                => _context.Products.Single(x => x.Id == id);

        public IEnumerable<Product> GetAll()
            => _context.Products;

        public void Remove(int id)
        {
            var product = Get(id);
            _context.Products.Remove(product);
        }

        public void Update(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
