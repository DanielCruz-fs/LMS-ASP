using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryBranchService : ILibraryBranch
    {
        private readonly LibraryContext context;

        public LibraryBranchService(LibraryContext context)
        {
            this.context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            this.context.Add(newBranch);
            this.context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return this.GetAll().FirstOrDefault(b => b.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return this.context.LibraryBranches.Include(b => b.Patrons)
                                               .Include(b => b.LibraryAssets);
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return this.context.LibraryBranches.Include(b => b.LibraryAssets)
                                               .FirstOrDefault(b => b.Id == branchId).LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = this.context.BranchHours.Include(b => b.Branch).Where(b => b.Branch.Id == branchId);
            return DataHelpers.HumanizeBizHours(hours);
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return this.context.LibraryBranches.Include(b => b.Patrons)
                                               .FirstOrDefault(b => b.Id == branchId)
                                               .Patrons;
        }

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfTheWeek = (int)DateTime.Now.DayOfWeek + 1;
            var hours = this.context.BranchHours.Include(b => b.Branch).Where(b => b.Branch.Id == branchId);
            var daysHours = hours.FirstOrDefault(h => h.DayOfWeek == currentDayOfTheWeek);

            return currentTimeHour < daysHours.CloseTime && currentTimeHour > daysHours.OpenTime;
        }
    }
}
