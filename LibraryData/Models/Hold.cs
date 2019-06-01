using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData.Models
{
    public class Hold
    {
        public int Id { get; set; }

        public DateTime HoldPlace { get; set; }

        //fkeys
        public virtual LibraryAsset LibraryAsset { get; set; }

        public virtual LibraryCard LibraryCard { get; set; }
    }
}
