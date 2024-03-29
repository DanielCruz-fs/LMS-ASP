﻿using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class PatronService : IPatron 
    {
        private readonly LibraryContext context;

        public PatronService(LibraryContext context)
        {
            this.context = context;
        }

        public void Add(Patron newPatron)
        {
            this.context.Add(newPatron);
            this.context.SaveChanges();
        }

        public Patron Get(int id)
        {
            return this.GetAll().FirstOrDefault(patron => patron.Id == id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return this.context.Patrons.Include(patron => patron.LibraryCard)
                                       .Include(patron => patron.LibraryBranch);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = this.Get(patronId).LibraryCard.Id;

            return this.context.CheckoutHistories.Include(co => co.LibraryCard)
                                                 .Include(co => co.LibraryAsset)
                                                 .Where(co => co.LibraryCard.Id == cardId)
                                                 .OrderByDescending(co => co.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            var cardId = this.Get(patronId).LibraryCard.Id;

            return this.context.Checkouts.Include(co => co.LibraryCard)
                                         .Include(co => co.LibraryAsset)
                                         .Where(co => co.LibraryCard.Id == cardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            var cardId = this.Get(patronId).LibraryCard.Id;

            return this.context.Holds.Include(h => h.LibraryCard)
                                     .Include(h => h.LibraryAsset)
                                     .Where(h => h.LibraryCard.Id == cardId)
                                     .OrderByDescending(h => h.HoldPlace);
        }
    }
}
