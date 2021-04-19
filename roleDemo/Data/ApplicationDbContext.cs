using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace roleDemo.Data
{
    public class Attendee
    {
        [Key]
        [Display(Name = "Attendee ID")]
        public int ID { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        [Required]
        public string email { get; set; }

        [Display(Name = "First Name")]
        [Required]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string lastName { get; set; }



        public virtual ICollection<EventAttendee> EventAttendee { get; set; }
    }


    public class Event
    {
        [Key]
        [Display(Name = "Event ID")]
        public int ID { get; set; }


        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }


        [Required]
        [RegularExpression(@"^(2[0-3]|[01]?[0-9]):([0-5][0-9])$", ErrorMessage = "Please input your time in 24 hr format (xx:xx)")]
        public string Time { get; set; }


        [Required]
        public string EventName { get; set; }


        [Required]
        public string Description { get; set; }


        public virtual ICollection<EventAttendee> EventAttendee { get; set; }


    }


    public class EventAttendee
    {
        [Key, Column(Order = 0)]
        [Required]
        public int AttendeeID { get; set; }


        [Key, Column(Order = 1)]
        [Required]
        public int EventID { get; set; }


        public virtual Attendee Attendee { get; set; }
        public virtual Event Event { get; set; }
    }


    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventAttendee> EventAttendees { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<EventAttendee>()
                .HasKey(x => new { x.AttendeeID, x.EventID });


            builder.Entity<EventAttendee>()
               .HasOne(x => x.Attendee)
               .WithMany(x => x.EventAttendee)
               .HasForeignKey(fk => new { fk.AttendeeID })
               .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<EventAttendee>()
               .HasOne(x => x.Event)
               .WithMany(x => x.EventAttendee)
               .HasForeignKey(fk => new { fk.EventID })
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
