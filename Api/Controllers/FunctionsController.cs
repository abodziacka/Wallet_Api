using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FunctionsController : ControllerBase
    {
        //AuthenticationContext authContext = new AuthenticationContext();

        private readonly AuthenticationContext _context;

        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private IHttpContextAccessor _httpContextAccessor;

        public FunctionsController(AuthenticationContext context, UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        //[HttpGet]
        //[Route("get-bill")]
        ////GET: /functions/get-bill
        //public Bill Details(int? id)
        //{
        //    //var query = from billQ in _context.Bills join productQ in _context.Products on billQ.Id equals productQ.BillId where billQ.Id == id select billQ;
        //    Bill bill = (Bill)_context.Bills.Find(id);
        //    List<Product> products = (List<Product>)_context.Products.Where(p => p.BillId == bill.Id).Select(p=>p).ToList();
        //    //if (bill == null)
        //    //{
        //    //    return BadRequest(new { message = "Username or password incorect." });
        //    //}
        //    return bill;
        //}

        [HttpGet]
        [Route("get-bill")]
        //GET: /functions/get-bill
        public IActionResult Details(int? id)
        {
            //var query = from billQ in _context.Bills join productQ in _context.Products on billQ.Id equals productQ.BillId where billQ.Id == id select billQ;
            Bill bill = (Bill)_context.Bills.Find(id);
            String userId = GetUserId();
            List<Product> products = (List<Product>)_context.Products.Where(p => p.BillId == bill.Id).Select(p => p).ToList();
            if (bill == null || bill.UserId != userId)
            {
                return NotFound(id);
            }
            return Ok(bill);
        }


        [HttpGet]
        [Route("get-bills")]
        //GET: /functions/get-bills
        public IEnumerable<Bill> GetAllAsync()
        {
            String userId = GetUserId();
            //var query = from billQ in _context.Bills join productQ in _context.Products on billQ.Id equals productQ.BillId where billQ.Id == id select billQ;
            IEnumerable<Bill> bills = _context.Bills.Where(p => p.UserId == userId).Select(p => p).ToList();

            return bills;
        }

        private String GetUserId()
        {
            string userId = "";
            int identiteisCount = User.Identities.Count();
            if (identiteisCount > 0)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identities.ToArray().GetValue(0);
                for (int i = 0; i < claimsIdentity.Claims.Count(); i++)
                {
                    Claim claim = (Claim)claimsIdentity.Claims.ToArray().GetValue(i);
                    if (claim.Type == "UserID")
                    {
                        userId = ((Claim)claimsIdentity.Claims.ToArray().GetValue(i)).Value;
                    }
                }
            }
            return userId;
        }

        [HttpPost]
        [Route("add-bill")]
        //POST: /functions/add-bill
        public void AddbBill(Bill bill)
        {
            String userId = GetUserId();
            _context.Bills.Add(new Bill()
            {
                Id=bill.Id,
                UserId=userId,
                Shop=bill.Shop,
                City=bill.City,
                Date=bill.Date,
                Products=bill.Products

            });
            //_context.Bills.Add(bill);
            _context.SaveChanges();
        }

        [HttpPost]
        [Route("add-category")]
        //POST: /functions/add-category
        public void AddCategory(Category category)
        {
            String userId = GetUserId();
            _context.Categories.Add(new Category()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                UserId = userId


            });
            //_context.Bills.Add(bill);
            _context.SaveChanges();
        }

        [HttpGet]
        [Route("get-category")]
        //GET: /functions/get-category
        public IEnumerable<Category> GetCategory()
        {
            String userId = GetUserId();

            IEnumerable<Category> categories = _context.Categories.Where(p => p.UserId == userId).Select(p => p).ToList();
            return categories;
        }

        [HttpPost]
        [Route("add-budget")]
        //POST: /functions/add-budget
        public void AddBudget(Budget budget)
        {
            String userId = GetUserId();
            _context.Budgets.Add(new Budget()
            {
                Id = budget.Id,
                Quantity = budget.Quantity,
                FromDate = budget.FromDate,
                ToDate = budget.ToDate,
                UserId = userId


            }); ;
            //_context.Bills.Add(bill);
            _context.SaveChanges();
        }

        [HttpGet]
        [Route("get-budgets")]
        //GET: /functions/get-category
        public IEnumerable<Budget> GetBudgets()
        {
            String userId = GetUserId();

            IEnumerable<Budget> budgets = _context.Budgets.Where(p => p.UserId == userId).Select(p => p).ToList();
            return budgets;
        }
        [HttpDelete]
        [Route("delete-budget-by-id")]
        public void DeleteBudgetById(int budgetId)
        {
            Budget budget = (Budget)_context.Budgets.Find(budgetId);
            String userId = GetUserId();

            if (budget.UserId == userId)
            {
                _context.Budgets.Remove(budget);
                _context.SaveChanges();
            }
        }

        [HttpPut]
        [Route("update-bill")]
        //PUT: /functions/update-bill
        public void UpdateBill(Bill bill)
        {
            Bill billFind = (Bill)_context.Bills.AsNoTracking().Where(p => p.Id == bill.Id).FirstOrDefault();
            String userId = GetUserId();
            if (billFind.UserId == userId && bill.UserId == userId) {
                List<Product> products = (List<Product>)_context.Products.AsNoTracking().Where(p => p.BillId == bill.Id).Select(p => p).ToList();
                for (int i = 0; i < products.Count; i++)
                {
                    bool existProduct = false;
                    for (int j = 0; j < bill.Products.Count; j++)
                    {
                        if (bill.Products[j].Id == products[i].Id)
                        {
                            existProduct = true;
                        }
                    }
                    if (!existProduct)
                    {
                        _context.Products.Remove(products[i]);
                        _context.SaveChanges();
                    }
                }
                _context.Bills.Update(new Bill()
                {
                    Id = bill.Id,
                    UserId = userId,
                    Shop = bill.Shop,
                    City = bill.City,
                    Date = bill.Date,
                    Products = bill.Products

                });
                _context.SaveChanges();
            }
        }


        [HttpDelete]
        [Route("delete-bill")]
        public void DeleteBill(Bill bill)
        {
            List<Product> products = (List<Product>)_context.Products.AsNoTracking().Where(p => p.BillId == bill.Id).Select(p => p).ToList();
            for (int i = 0; i < products.Count; i++)
            {
                    _context.Products.Remove(products[i]);
                    _context.SaveChanges();
            };
            _context.Bills.Remove(bill);
            _context.SaveChanges();
        }

        [HttpDelete]
        [Route("delete-bill-by-id")]
        public void DeleteBillById(int billId)
        {
            Bill bill = (Bill)_context.Bills.Find(billId);
            String userId = GetUserId();

            if (bill.UserId == userId)
            {
                List<Product> products = (List<Product>)_context.Products.AsNoTracking().Where(p => p.BillId == billId).Select(p => p).ToList();
                _context.Products.RemoveRange(products);
                _context.SaveChanges();

                _context.Bills.Remove(bill);
                _context.SaveChanges();
            }
        }

        [HttpGet]
        [Route("get-budgetStatistics")]
        //GET: /functions/get-bill
        public Object GetBudgetStatistics(DateTime dateFrom, DateTime dateTo)
        {
            String userId = GetUserId();

            var query2 = from bu in _context.Budgets
                         where bu.UserId == userId && bu.FromDate >= dateFrom && bu.FromDate <= dateTo
                         orderby bu.FromDate descending
                         select new
                         {
                             bu.Id,
                             bu.FromDate,
                             bu.ToDate,
                             bu.Quantity,
                             Price = (double?)
                             (from p in _context.Products
                              join b in _context.Bills on p.BillId equals b.Id
                              where
       b.Date >= bu.FromDate && b.Date <= bu.ToDate && b.UserId == userId
                              select new
                              {
                                  TotalPrice = p.Price * p.Amount
                              }).Sum(p => p.TotalPrice),
                             SaveMoney = (double?)(bu.Quantity -
                             (from p in _context.Products
                              join b in _context.Bills on p.BillId equals b.Id
                              where
      b.Date >= bu.FromDate && b.Date <= bu.ToDate && b.UserId == userId
                              select new
                              {
                                  TotalPrice = p.Price * p.Amount
                              }).Sum(p => p.TotalPrice))
                         };
            return query2;
                
        }

        [HttpGet]
        [Route("get-diagramDetails")]
        //GET: /functions/get-bill
        public Object GetDiagramDetails(int budgetId)
        {
            String userId = GetUserId();
            Budget budget = (Budget)_context.Budgets.Find(budgetId);
            if (budget != null && budget.UserId == userId)
            {
                var query2 = from p in _context.Products
                             join c in _context.Categories on new { Id = p.CategoryId } equals new { Id = c.Id } into c_join
                             from c in c_join.DefaultIfEmpty()
                             join b in _context.Bills on p.BillId equals b.Id
                             where
                               b.Date >= budget.FromDate && b.Date <= budget.ToDate && b.UserId == userId
                             group new { c, p } by new
                             {
                                 c.Id,
                                 c.Name
                             } into g
                             select new
                             {
                                 Id = g.Key.Id,
                                 Name = g.Key.Name,
                                 TotalPrice = (double?)g.Sum(p => p.p.Amount * p.p.Price)
                             };
                return query2;
            } else
            {
                return null;
            }

        }

        [HttpGet]
        [Route("get-diagramDetails-categorydetails")]
        //GET: /functions/get-diagramDetails-categorydetails
        public Object GetDiagramCategory(int budgetId, int categoryId)
        {
            String userId = GetUserId();
            Budget budget = (Budget)_context.Budgets.Find(budgetId);
            if (budget != null && budget.UserId == userId)
            {
                var query2 = from p in _context.Products
                             join b in _context.Bills on p.BillId equals b.Id
                             where
                               p.CategoryId == categoryId &&
                               b.Date >= budget.FromDate && b.Date <= budget.ToDate && 
                               b.UserId == userId
                             group p by new
                             {
                                 p.Name
                             } into g
                             select new
                             {
                                 g.Key.Name,
                                 Amount = (double?)g.Sum(p => p.Amount),
                                 TotalPrice = (double?)g.Sum(p => p.Amount * p.Price)
                             };
                return query2;
            }
            else
            {
                return null;
            }

        }

        [HttpGet]
        [Route("get-count-budget-in-period")]
        //GET: /functions/get-bill
        public int GetCountBudgetInPeriod(DateTime dateFrom, DateTime dateTo)
        {
            String userId = GetUserId();
            List<Budget> budgets = (List<Budget>)_context.Budgets.Where(p => (p.FromDate <= dateFrom && p.ToDate >= dateFrom && p.UserId == userId) || (p.FromDate <= dateTo && p.ToDate >= dateTo && p.UserId == userId)).Select(p => p).ToList();
            return budgets.Count();
        }


    }
}
