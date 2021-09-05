using System;

namespace todocompany.Common.Models
{
    public class Entry
    {
        public DateTime Date { get; set; }

        public string EmployedId { get; set; }

        public bool IsConsolidated { get; set; }

        public int Type { get; set; }


    }
}
