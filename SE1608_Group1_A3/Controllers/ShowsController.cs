using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1608_Group1_A3.Models;

namespace SE1608_Group1_A3.Controllers
{
    public class ShowsController : Controller
    {
        private readonly CinemaContext _context;

        public ShowsController(CinemaContext context)
        {
            _context = context;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            var cinemaContext = _context.Shows
                .Include(s => s.Film)
                .Include(s => s.Room);

            ViewData["shows"] = await cinemaContext.ToListAsync();
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name");
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title");
            
            Show show = new Show();
            show.ShowDate = DateTime.Now;
            return View(show);
        }
        [HttpPost]
        public async Task<IActionResult> Index(Show show)
        {
            var cinemaContext = _context.Shows
                .Include(s => s.Film)
                .Include(s => s.Room);

            ViewData["shows"] = await cinemaContext
                .Where(s => s.ShowDate == show.ShowDate
                && s.RoomId == show.RoomId
                && s.FilmId == show.FilmId)
                .ToListAsync();

            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name", show.RoomId);
            ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "Title", show.FilmId);

            return View(show);
        }

        // GET: Shows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // GET: Shows/Create
        public IActionResult Create(int id)
        {
            // RoomId
            int roomId = id; 
            // Date hiện tại to add new Show
            DateTime dateShow = DateTime.Now;
            // Slot available 
            bool[] slots = new bool[9];
            List<int> slotAvailable = new List<int>();
            List<Show> shows = _context.Shows
                .Where(s => s.ShowDate == DateTime.Now.Date
                && s.RoomId == roomId)
                .ToList<Show>();
            foreach (Show s in shows)
                slots[(int)s.Slot - 1] = true;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] == false) slotAvailable.Add(i+1);
            // All Film
            List<Film> films = _context.Films.ToList();
            // Đẩy data qua view
            ViewData["roomId"] = roomId;
            ViewData["dateShow"] = dateShow;
            ViewData["slotAvailable"] = slotAvailable;
            ViewData["films"] = films;
            return View();
        }

        // POST: Shows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] 
        [ActionName("Create")]
        public IActionResult Create(decimal Price,Show show)
        {
            show.Price = Price;
            // Add show 
            _context.Add(show);
            _context.SaveChanges();
            //ViewData["FilmId"] = new SelectList(_context.Films, "FilmId", "CountryCode", show.FilmId);
            //ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", show.RoomId);
            return RedirectToAction("Index");
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Show show = _context.Shows.FirstOrDefault(s => s.ShowId == id);
            // Slot available 
            bool[] slots = new bool[9];
            List<int> slotAvailable = new List<int>();
            List<Show> shows = _context.Shows
                .Where(s => s.ShowDate == show.ShowDate
                && s.RoomId == show.RoomId
                && s.ShowId != show.ShowId)
                .ToList<Show>();
            foreach (Show s in shows)
                slots[(int)s.Slot - 1] = true;
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] == false) slotAvailable.Add(i + 1);
            // All Film
            List<Film> films = _context.Films.ToList();
            // Đẩy data qua view
            ViewData["slotAvailable"] = slotAvailable;
            ViewData["films"] = films;
            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Edit(decimal Price, Show show)
        {
            show.Price = Price;
            _context.Update(show);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Shows
                .Include(s => s.Film)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.ShowId == id);
            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Xóa bên booking trước
            List<Booking> bookings = await _context.Bookings.Where(b => b.ShowId == id).ToListAsync();
            if(bookings != null)
            {
                foreach(Booking booking in bookings) { 
                    _context.Bookings.Remove(booking);
                    await _context.SaveChangesAsync();
                }
            }
            var show = await _context.Shows.FindAsync(id);
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowExists(int id)
        {
            return _context.Shows.Any(e => e.ShowId == id);
        }
    }
}
