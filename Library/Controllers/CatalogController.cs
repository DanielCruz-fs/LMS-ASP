using Library.Models.Catalog;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset assets;

        public CatalogController(ILibraryAsset assets)
        {
            this.assets = assets;
        }

        public IActionResult Index()
        {
            var assetModels = this.assets.GetAll();
            var listingResult = assetModels.Select(result => new AssetIndexListingModel
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                AuthorOrDirector = this.assets.GetAuthorOrDirector(result.Id),
                DeweyCallNumber = this.assets.GetDeweyIndex(result.Id),
                Title = result.Title,
                Type = this.assets.GetType(result.Id)
            });
            var model = new AssetIndexModel
            {
                Assets = listingResult
            };

            return View(model);
        }
    }
}
