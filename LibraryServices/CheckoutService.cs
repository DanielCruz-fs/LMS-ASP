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

        public Checkout GetLatestCheckouts(int id)
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
            this.context.Update(item);

            //remove any existing checkouts on the item
            //close any existing checkout history
            //look for existing holds on the item
            //if there are holds, checkout the item to the librarycard with the earliest hold
            //otherwise, update the item status to available
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentHoldPatronName(int id)
        {
            throw new NotImplementedException();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }
    }
}
