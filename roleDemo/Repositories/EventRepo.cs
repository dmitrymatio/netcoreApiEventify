using roleDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace roleDemo.Repositories
{
    public class EventRepo
    {
        ApplicationDbContext db;

        public EventRepo(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<Event> GetAll()
        {
            var query = from e in db.Events
                        select new Event()
                        {
                            EventName = e.EventName,
                            Description = e.Description,
                            Date = e.Date,
                            Time = e.Time,
                            ID = e.ID
                        };


            return query;
        }

        public Event GetOne(int EventID)
        {
            var query = GetAll().Where(x => x.ID == EventID).FirstOrDefault();
            return query;
        }

        public bool Create(string EventName, string Desc, DateTime Date, string Time)
        {
            Event E = new Event()
            {
                EventName = EventName,
                Description = Desc,
                Date = Date,
                Time = Time
            };

            db.Events.Add(E);
            db.SaveChanges();

            return true;
        }

        public bool Delete(int EventID)
        {
            Event E = db.Events.Where(e => e.ID == EventID).FirstOrDefault();
            db.Remove(E);
            db.SaveChanges();
            return true;
        }

        public bool AttendEvent(int EventID, int AttendeeID)
        {
            EventAttendee AttendEvent = new EventAttendee { AttendeeID = AttendeeID, EventID = EventID };
            db.EventAttendees.Add(AttendEvent);
            db.SaveChanges();
            return true;
        }

        public bool CancelAttendance(int EventID, int AttendeeID)
        {
            EventAttendee CancelAttendance = db.EventAttendees.Where(x => x.AttendeeID == AttendeeID && x.EventID == EventID).FirstOrDefault();
            db.Remove(CancelAttendance);
            db.SaveChanges();
            return true;
        }
    }
}