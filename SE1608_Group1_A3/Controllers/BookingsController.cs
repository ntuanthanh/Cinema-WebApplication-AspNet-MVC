using Microsoft.AspNetCore.Mvc;
using SE1608_Group1_A3.Models;
using System.Collections.Generic;
using System.Linq;

namespace SE1608_Group1_A3.Controllers
{
    public class BookingsController : Controller
    {
        private readonly CinemaContext _context;

        public BookingsController(CinemaContext context)
        {
            _context = context;
        }
        // Show all Bookings of ShowId
        public char[] AllBookedMatrix(int showId)
        {
            List<Booking> bookings = _context.Bookings.Where(x => x.ShowId == showId).ToList();
            char[] BookedMatrix = new char[100];
            for (int j = 0; j < BookedMatrix.Length; j++)
            {
                BookedMatrix[j] = '0';
            }

            foreach (Booking booking in bookings)
            {
                for (int i = 0; i < 100; i++)
                {
                    if (booking.SeatStatus[i] == '1') BookedMatrix[i] = '1';
                }
            }
            return BookedMatrix;
        }

        public IActionResult Index(int Id)
        {
            char[] BookedMatrix = AllBookedMatrix(Id);
            ViewData["BookedMatrix"] = BookedMatrix;
            ViewData["ShowId"] = Id;
            // Gửi Bookings 
            List<Booking> bookings = _context.Bookings.Where(x => x.ShowId == Id).ToList();
            ViewData["bookings"] = bookings;
            return View();
        }
        // Create new Booking of Show
        public IActionResult Create(int Id)
        {
            char[] BookedMatrix = AllBookedMatrix(Id);
            ViewData["BookedMatrix"] = BookedMatrix;
            ViewData["ShowId"] = Id;
            Show show = _context.Shows.FirstOrDefault(x => x.ShowId == Id);
            return View(show);
        }

        [HttpPost]
        [ActionName("Create")]
        public IActionResult DoCreate(int []cbBook, string Name, decimal Amount, int ShowId)
        {
            string temp = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            char[] set = temp.ToCharArray();
            for(int i = 0; i < cbBook.Length; i++)
            {
                set[cbBook[i]] = '1';
            }
            string setStatus = new string(set);
            Booking booking = new Booking();
            booking.Name = Name;
            booking.SeatStatus = setStatus;
            booking.Amount = Amount;
            booking.ShowId = ShowId;
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", new { Id = ShowId });
        }
        // Edit of Booking 
        public IActionResult Edit(int Id)
        {
            // Model 
            Booking booking = _context.Bookings.Find(Id);
            // Show 
            Show show = _context.Shows.FirstOrDefault(s => s.ShowId == booking.ShowId);
            booking.Show = show;
            // Checkbox của mình
            char[] BookedMatrix = AllBookedMatrixByBookingId(Id);
            ViewData["BookedMatrix"] = BookedMatrix;
            // Checkbox của người khác
            char[] BookedMatrix_other = AllBookedMatrix(show.ShowId);
            ViewData["BookedMatrix_other"] = BookedMatrix_other;
            
            return View(booking);
        }
        [HttpPost]
        [ActionName("Edit")]
        public IActionResult doEdit(int[] cbBook, string Name, decimal Amount, int BookingId, int ShowId)
        {
            string temp = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            char[] set = temp.ToCharArray();
            for (int i = 0; i < cbBook.Length; i++)
            {
                set[cbBook[i]] = '1';
            }
            string setStatus = new string(set);
            Booking booking = new Booking();
            booking.BookingId = BookingId;
            booking.Name = Name;
            booking.SeatStatus = setStatus;
            booking.Amount = Amount;
            booking.ShowId = ShowId;
            _context.Bookings.Update(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", new { Id = ShowId });
        }
        // Details of Booking
        public char[] AllBookedMatrixByBookingId(int bookingID)
        {
            Booking booking = _context.Bookings.Find(bookingID);
            return booking.SeatStatus.ToCharArray();
        }
        public IActionResult Details(int Id)
        {
            char[] BookedMatrix = AllBookedMatrixByBookingId(Id);
            ViewData["BookedMatrix"] = BookedMatrix;
            // Model 
            Booking booking = _context.Bookings.Find(Id);
            return View(booking);
        }
        // Delete
        public IActionResult Delete(int Id)
        {
            char[] BookedMatrix = AllBookedMatrixByBookingId(Id);
            ViewData["BookedMatrix"] = BookedMatrix;
            // Model 
            Booking booking = _context.Bookings.Find(Id);
            return View(booking);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DoDelete(int Id)
        {
            var booking = _context.Bookings.Find(Id);
            int ShowId = booking.ShowId;
            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            return RedirectToAction("Index", new { Id = ShowId });
        }
    }
}
