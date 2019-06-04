using Library.Models.Catalog;
using Library.Models.CheckoutModels;
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
        private readonly ICheckout checkouts;

        public CatalogController(ILibraryAsset assets, ICheckout checkouts) 
        {
            this.assets = assets;
            this.checkouts = checkouts;
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

        public IActionResult Detail(int id)
        {
            var asset = this.assets.GetById(id);
            var currentHolds = this.checkouts.GetCurrentHolds(id)
                                             .Select(ch => new AssetHoldModel
                                             {
                                                 HoldPlaced = this.checkouts.GetCurrentHoldPlaced(ch.Id),
                                                 PatronName = this.checkouts.GetCurrentHoldPatronName(ch.Id)
                                             });
            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = asset.Title,
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = this.assets.GetAuthorOrDirector(id),
                CurrentLocation = this.assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = this.assets.GetDeweyIndex(id),
                ISBN = this.assets.GetIsbn(id),
                CheckoutHistory = this.checkouts.GetCheckOutHistory(id),
                LatestCheckout = this.checkouts.GetLatestCheckout(id),
                PatronName = this.checkouts.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds
            };

            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = this.assets.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = this.checkouts.IsCheckedOut(id)
            };
            return View(model);
        }

        public IActionResult CheckIn(int id)
        {
            this.checkouts.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }

        public IActionResult Hold(int id)
        {
            var asset = this.assets.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = this.checkouts.IsCheckedOut(id),
                HoldCount = this.checkouts.GetCurrentHolds(id).Count()
            };
            return View(model);
        }

        public IActionResult MarkLost(int assetId)
        {
            this.checkouts.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkFound(int assetId)
        {
            this.checkouts.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlacedCheckout(int assetId, int libraryCardId)
        {
            this.checkouts.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            this.checkouts.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}
