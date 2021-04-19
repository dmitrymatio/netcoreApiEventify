using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roleDemo.Data;
using roleDemo.ViewModels;

namespace roleDemo.Repositories
{
    public class AttendeeRepo
    {
        ApplicationDbContext db;

        public AttendeeRepo(ApplicationDbContext context)
        {
            db = context;
        }
        public Attendee GetOneByEmail(string email)
        {
            var user = db.Attendees.Where(x => x.email == email).FirstOrDefault();

            return user;
        }
        public bool isExist(string email)
        {
            var isRegister = GetOneByEmail(email);

            if (isRegister != null)
                return true;
            return false;
        }


        public bool Create(string LastName, string FirstName, string Email)
        {
            Attendee attendee = new Attendee()
            {
                lastName = LastName,
                firstName = FirstName,
                email = Email
            };

            db.Attendees.Add(attendee);
            db.SaveChanges();

            return true;
        }


        public bool Update(int AttendeeID, string LastName, string FirstName)
        {
            Attendee attendee = db.Attendees.Where(x => x.ID == AttendeeID).FirstOrDefault();
            attendee.lastName = LastName;
            attendee.firstName = FirstName;

            db.SaveChanges();

            return true;
        }
    }
}
