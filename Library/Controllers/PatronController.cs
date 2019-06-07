using Library.Models.Patron;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IPatron patron;

        public PatronController(IPatron patron)
        {
            this.patron = patron;
        }

        public IActionResult Index()
        {
            var allPatrons = this.patron.GetAll();

            var patronModels = allPatrons.Select(p => new PatronDetailModel
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                LibraryCardId = p.LibraryCard.Id,
                OverdueFees = p.LibraryCard.Fees,
                HomeLibraryBranch = p.LibraryBranch.Name
            }).ToList();

            var model = new PatronIndexModel { Patrons = patronModels };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var patron = this.patron.Get(id);

            var model = new PatronDetailModel
            {
                LastName = patron.LastName,
                FirstName = patron.FirstName,
                Address = patron.Address,
                HomeLibraryBranch = patron.LibraryBranch.Name,
                MemberSince = patron.LibraryCard.Created,
                OverdueFees = patron.LibraryCard.Fees,
                LibraryCardId = patron.LibraryCard.Id,
                Telephone = patron.TelephoneNumber,
                AssetsCheckedouts = this.patron.GetCheckouts(id).ToList() ?? new List<Checkout>(),
                CheckoutHistory = this.patron.GetCheckoutHistory(id),
                Holds = this.patron.GetHolds(id)
            };

            return View(model);
        }
    }
}
