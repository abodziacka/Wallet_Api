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


        [HttpGet]
        [Route("get-category")]
        //GET: /functions/get-category
        public IEnumerable<Category> GetCategoryl()
        {
            //var query = from billQ in _context.Bills join productQ in _context.Products on billQ.Id equals productQ.BillId where billQ.Id == id select billQ;
            IEnumerable<Category> categories = _context.Categories;
            //List<Product> products = (List<Product>)_context.Products.ToList();
            return categories;
        }

        [HttpPut]
        [Route("update-bill")]
        //PUT: /functions/update-bill
        public void UpdateBill(Bill bill)
        {
            Bill billFind = (Bill)_context.Bills.Find(bill.Id);
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
    }
}
