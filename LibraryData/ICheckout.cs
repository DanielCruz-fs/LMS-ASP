using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        IEnumerable<Hold> GetCurrentHolds(int id);
        IEnumerable<CheckoutHistory> GetCheckOutHistory(int id);

        Checkout GetByID(int checkoutId);
        Checkout GetLatestCheckout(int assetId);
        string GetCurrentCheckoutPatron(int assetId);
        string GetCurrentHoldPatronName(int holdId);
        DateTime GetCurrentHoldPlaced(int holdId);

        void MarkLost(int assetId);
        void MarkFound(int assetId);
        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int libraryCardId);
        void PlaceHold(int assetId, int libraryCardId);


    }
}
