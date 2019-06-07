using Library.Models.Patron;
using LibraryData;
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
    }
}
