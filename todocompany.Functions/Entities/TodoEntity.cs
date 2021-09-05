using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace todocompany.Functions.Entities
{
    public class TodoEntity : TableEntity
    {
        public DateTime Date { get; set; }

        public string EmployedId { get; set; }

        public bool IsConsolidated { get; set; }

        public int Type { get; set; }
    }
}
