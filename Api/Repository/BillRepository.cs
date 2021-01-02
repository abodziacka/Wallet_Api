using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Repository
{
    public class BillRepository
    {
        private readonly AuthenticationContext _context;

        public BillRepository(AuthenticationContext context)
        {
            _context = context;
        }

        public void Add(Bill bill)
        {
            _context.Bills.Add(bill);
        }

        public Bill Get(int id)
                => _context.Bills.Single(x => x.Id == id);

        public IEnumerable<Bill> GetAll()
            => _context.Bills;

        public void Remove(int id)
        {
            var bill = Get(id);
            _context.Bills.Remove(bill);
        }

        public void Update(Bill bill)
        {
            throw new NotImplementedException();
        }
    }
}
