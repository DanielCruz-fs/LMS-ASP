using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private readonly LibraryContext context;

        public CheckoutService(LibraryContext context)
        {
            this.context = context;
        }
        public void Add(Checkout newCheckout)
        {
            this.context.Add(newCheckout);
            this.context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return this.context.Checkouts;
        }

        public Checkout GetByID(int checkoutId)
        {
            return this.GetAll().FirstOrDefault(checkout => checkout.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckOutHistory(int id)
        {
            return this.context.CheckoutHistories.Include(h => h.LibraryAsset)
                                                 .Include(h => h.LibraryCard)
                                                 .Where(h => h.LibraryAsset.Id == id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return this.context.Holds.Include(h => h.LibraryAsset)
                                     .Where(h => h.LibraryAsset.Id == id);
        }

        public Checkout GetLatestCheckout(int id)
        {
            return this.context.Checkouts.Where(c => c.LibraryAsset.Id == id)
                                         .OrderByDescending(c => c.Since)
                                         .FirstOrDefault();
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;

            this.UpdateAssetStatus(assetId, "Available");
            this.RemoveExistingCheckouts(assetId);
            this.CloseExistingCheckoutHistory(assetId, now);

            this.context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string v)
        {
            var item = this.context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);
            this.context.Update(item);
            item.Status = this.context.Status.FirstOrDefault(s => s.Name == v);
        }

        private void CloseExistingCheckoutHistory(int assetId, DateTime now)
        {
            //close any existing checkout history
            var history = this.context.CheckoutHistories.FirstOrDefault(h => h.LibraryAsset.Id == assetId
                                                                        && h.CheckedIn == null);
            if (history != null)
            {
                this.context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            //remove any existing checkouts on the item
            var checkout = this.context.Checkouts.FirstOrDefault(co => co.LibraryAsset.Id == assetId);
            if (checkout != null)
            {
                this.context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            this.UpdateAssetStatus(assetId, "Lost");
            this.context.SaveChanges();
        }

        public void CheckInItem(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;
            var item = this.context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            //remove any existing checkouts on the item
            this.RemoveExistingCheckouts(assetId);
            //close any existing checkout history
            this.CloseExistingCheckoutHistory(assetId, now);
            //look for existing holds on the item
            var currentHolds = this.context.Holds
                                           .Include(h => h.LibraryAsset)
                                           .Include(h => h.LibraryCard)
                                           .Where(h => h.LibraryAsset.Id == assetId);
            //if there are holds, checkout the item to the librarycard with the earliest hold
            if (currentHolds.Any())
            {
                this.CheckoutToEarliestHold(assetId, currentHolds); 
            }
            //otherwise, update the item status to available
            this.UpdateAssetStatus(assetId, "Available");

            this.context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds.OrderBy(holds => holds.HoldPlace).FirstOrDefault();
            var card = earliestHold.LibraryCard;

            this.context.Remove(earliestHold);
            this.context.SaveChanges();
            this.CheckOutItem(assetId, card.Id);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (this.IsCheckOut(assetId))
            {
                return;
            }

            var item = this.context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);
            this.UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = this.context.LibraryCards.Include(card => card.Checkouts)
                                                       .FirstOrDefault(card => card.Id == libraryCardId);
            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = this.GetDefaultCheckoutTime(now)
            };

            this.context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            this.context.Add(checkoutHistory);
            this.context.SaveChanges();

        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        private bool IsCheckOut(int assetId)
        {
            return this.context.Checkouts.Where(co => co.LibraryAsset.Id == assetId).Any();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;
            var asset = this.context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);
            var card = this.context.LibraryCards.FirstOrDefault(c => c.Id == libraryCardId);

            if (asset.Status.Name == "Available")
            {
                this.UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlace = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            this.context.Add(hold);
            this.context.SaveChanges();
        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = this.context.Holds.Include(h => h.LibraryAsset)
                                         .Include(h => h.LibraryCard)
                                         .FirstOrDefault(h => h.Id == holdId);
            var cardId = hold?.LibraryCard.Id;
            var patron = this.context.Patrons.Include(p => p.LibraryCard)
                                             .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return this.context.Holds.Include(h => h.LibraryAsset)
                                     .Include(h => h.LibraryCard)
                                     .FirstOrDefault(h => h.Id == holdId).HoldPlace;
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = this.GetCheckoutByAssetId(assetId);
            
            if (checkout == null)
            {
                return "";
            }

            var cardId = checkout.LibraryCard.Id;
            var patron = this.context.Patrons.Include(p => p.LibraryCard)
                                             .FirstOrDefault(p => p.LibraryCard.Id == assetId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return this.context.Checkouts.Include(co => co.LibraryAsset)
                                         .Include(co => co.LibraryCard)
                                         .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }
    }
}
