using System;

namespace todocompany.Common.Models
{
    public class Entry
    {
        public DateTime CreatedTime { get; set; }

        public string TaskDescription { get; set; }

        public bool IsCompleted { get; set; }
    }
}
