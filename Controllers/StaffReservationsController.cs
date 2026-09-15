using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Constants;
using RestaurantSystem.Data;
using RestaurantSystem.ViewModels.Reservations;
using Microsoft.AspNetCore.Authorization;
namespace RestaurantSystem.Controllers
{
   // [Authorize(Roles = "Admin,Staff")]
    public class StaffReservationsController : Controller
    {
        private readonly RestaurantDbContext _context;

        public StaffReservationsController(RestaurantDbContext context)
        {
            _context = context;
        }

        // Display all reservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .AsNoTracking()
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .Select(r => new ReservationListViewModel
                {
                    ReservationId = r.ReservationId,

                    ReservationDate = r.ReservationDate,

                    ReservationTime = r.ReservationTime,

                    GuestCount = r.GuestCount,

                    Status = r.Status,

                    CustomerName = r.User != null
                        ? r.User.FullName
                        : "Unknown Customer",

                    CustomerEmail = r.User != null
                        ? r.User.Email
                        : null,

                    CustomerPhone = r.User != null
                        ? r.User.PhoneNumber
                        : null,

                    TableNumber = r.Table != null
                        ? r.Table.TableNumber
                        : 0,

                    TableCapacity = r.Table != null
                        ? r.Table.Capacity
                        : 0,

                    LocationZone = r.Table != null
                        ? r.Table.LocationZone
                        : "Unknown"
                })
                .ToListAsync();

            return View(reservations);
        }

        // Pending -> Confirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status != ReservationStatuses.Pending)
            {
                TempData["ErrorMessage"] =
                    "Only pending reservations can be confirmed.";

                return RedirectToAction(nameof(Index));
            }

            if (reservation.Table == null)
            {
                TempData["ErrorMessage"] =
                    "This reservation does not have a valid table.";

                return RedirectToAction(nameof(Index));
            }

            if (reservation.GuestCount > reservation.Table.Capacity)
            {
                TempData["ErrorMessage"] =
                    "The assigned table does not have enough capacity.";

                return RedirectToAction(nameof(Index));
            }

            // Prevent another active reservation for the same table
            // at exactly the same date and time.
            var conflictExists = await _context.Reservations
                .AnyAsync(r =>
                    r.ReservationId != reservation.ReservationId &&
                    r.TableId == reservation.TableId &&
                    r.ReservationDate.Date == reservation.ReservationDate.Date &&
                    r.ReservationTime == reservation.ReservationTime &&
                    r.Status != ReservationStatuses.Cancelled);

            if (conflictExists)
            {
                TempData["ErrorMessage"] =
                    "This table already has another reservation at the same time.";

                return RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatuses.Confirmed;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Reservation #{reservation.ReservationId} has been confirmed.";

            return RedirectToAction(nameof(Index));
        }

        // Confirmed -> Seated
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeatGuest(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status != ReservationStatuses.Confirmed)
            {
                TempData["ErrorMessage"] =
                    "Only confirmed reservations can be seated.";

                return RedirectToAction(nameof(Index));
            }

            if (reservation.Table == null)
            {
                TempData["ErrorMessage"] =
                    "The reservation does not have a valid table.";

                return RedirectToAction(nameof(Index));
            }

            if (reservation.Table.Status == TableStatuses.Occupied)
            {
                TempData["ErrorMessage"] =
                    "This table is currently occupied.";

                return RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatuses.Seated;

            reservation.Table.Status = TableStatuses.Occupied;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Guests for reservation #{reservation.ReservationId} are now seated.";

            return RedirectToAction(nameof(Index));
        }

        // Seated -> Completed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status != ReservationStatuses.Seated)
            {
                TempData["ErrorMessage"] =
                    "Only seated reservations can be completed.";

                return RedirectToAction(nameof(Index));
            }

            reservation.Status = ReservationStatuses.Completed;

            if (reservation.Table != null)
            {
                reservation.Table.Status = TableStatuses.Available;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Reservation #{reservation.ReservationId} has been completed.";

            return RedirectToAction(nameof(Index));
        }

        // Cancel active reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Status == ReservationStatuses.Completed ||
                reservation.Status == ReservationStatuses.Cancelled)
            {
                TempData["ErrorMessage"] =
                    "This reservation cannot be cancelled.";

                return RedirectToAction(nameof(Index));
            }

            // If guests were already seated, release the table.
            if (reservation.Status == ReservationStatuses.Seated &&
                reservation.Table != null)
            {
                reservation.Table.Status = TableStatuses.Available;
            }

            reservation.Status = ReservationStatuses.Cancelled;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Reservation #{reservation.ReservationId} has been cancelled.";

            return RedirectToAction(nameof(Index));
        }
    }
}