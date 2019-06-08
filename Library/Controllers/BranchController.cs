using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch branch;

        public BranchController(ILibraryBranch branch)
        {
            this.branch = branch;
        }
        
        public IActionResult Index()
        {
            var branches = this.branch.GetAll().Select(branch => new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen = this.branch.IsBranchOpen(branch.Id),
                NumberOfAssets = this.branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = this.branch.GetPatrons(branch.Id).Count()
            });

            var model = new BranchIndexModel()
            {
                Branches = branches
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branch = this.branch.Get(id);

            var model = new BranchDetailModel()
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Telephone = branch.Telephone,
                OpenDate = branch.OpenTime.ToString("yyyy-MM-dd"),
                NumberOfAssets = this.branch.GetAssets(id).Count(),
                NumberOfPatrons = this.branch.GetPatrons(id).Count(),
                TotalAssetValue = this.branch.GetAssets(id).Sum(a => a.Cost),
                ImageUrl = branch.ImageUrl,
                HoursOpen = this.branch.GetBranchHours(id)
            };

            return View(model);
        }
    }
}
