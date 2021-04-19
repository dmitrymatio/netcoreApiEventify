using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using roleDemo.Data;
using roleDemo.ViewModels;


namespace roleDemo.Repositories
{
    public class EventAttendeeRepo
    {
        ApplicationDbContext db;
        public EventAttendeeRepo(ApplicationDbContext context)
        {
            db = context;
        }

        public IQueryable<EventAttendeeVM> GetAll()
        {
            var query1 = from ea in db.EventAttendees
                        from a in db.Attendees
                        where ea.AttendeeID == a.ID
                        select new
                        {
                            attendeeID = a.ID,
                            firstName = a.firstName,
                            lastName = a.lastName,
                            eventID = ea.EventID
                        };


            var query2 = from e in db.Events
                         from q in query1
                         where e.ID == q.eventID
                         select new EventAttendeeVM()
                         {
                             FirstName = q.firstName,
                             LastName = q.lastName,
                             AttendeeID = q.attendeeID,
                             EventID = e.ID,
                             Date = e.Date,
                             Time = e.Time,
                             Description = e.Description,
                             EventName = e.EventName
                         };


            return query2;

        }


        public IQueryable<EventAttendeeVM> GetCurrentAttandingEvents(int AttendeeID)
        {
            var query1 = GetAll().Where(x => x.AttendeeID == AttendeeID);
            return query1;
        }


        public AttendeesVM ViewAttendees(int EventID)
        {
            var query1 = GetAll()
                .AsEnumerable()
                .GroupBy(x => new { x.EventName, x.EventID, x.Date, x.Time })
                .Select(x => new AttendeesVM()
                {
                    EventName = x.Key.EventName,
                    EventID = x.Key.EventID,
                    EventDate = x.Key.Date,
                    EventTime = x.Key.Time,
                    AttendeeList = x.ToList()
                }
                );
            var filter = query1.Where(x => x.EventID == EventID).FirstOrDefault();
            return filter;
        }


    }
}