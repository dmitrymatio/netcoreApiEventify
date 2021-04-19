using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace roleDemo.ViewModels
{
    public class AttendeesVM
    {
        [Required]
        public string EventName { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string EventTime { get; set; }

        [Required]
        public List<EventAttendeeVM> AttendeeList { get; set; }

    }
}
