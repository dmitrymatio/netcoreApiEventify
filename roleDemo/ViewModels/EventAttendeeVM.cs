using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace roleDemo.ViewModels
{
    public class EventAttendeeVM
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int AttendeeID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string Description { get; set; }

    }
}
