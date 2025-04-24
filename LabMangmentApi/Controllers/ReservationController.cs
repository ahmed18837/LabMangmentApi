using LabMangmentApi.Data;
using LabMangmentApi.Models.Dtos.Reservation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Physics.DAL.Models.DTO;

namespace physics.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {

            var reservations = await _context.Reservations
                .Include(r => r.Device)
                .Include(r => r.User)
                .ToListAsync();
            if (reservations.Count == 0)
            {

                return NotFound(new
                {
                    message = "No reservation found."
                });
            }


            var result = reservations.Select(r => new GetReservationDto
            {
                Id = r.Id,
                DeviceName = r.Device.Name,
                Date = r.Date,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                UserName = r.User.FullName,
                LabName = r.Device.LocationName,
                Purpose = r.Purpose
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Device)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound(new
                {
                    message = $"No reservation found with ID {id}."
                });
            }

            var result = new GetReservationDto
            {
                Id = reservation.Id,
                DeviceName = reservation.Device.Name,
                Date = reservation.Date,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                UserName = reservation.User.FullName,
                LabName = reservation.Device.LocationName,
                Purpose = reservation.Purpose
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound(new
                {
                    message = $"Reservation with ID {id} is not found."
                });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Reservation deleted successfully."
            });
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetReservationsByDate([FromQuery] DateRequestDTO dto)
        {
            //var requestedDate =DateOnly.Parse(date);

            var reservations = await _context.Reservations
                .Include(r => r.Device)
                .Include(r => r.User)
                .Where(r => r.Date == dto.Date)
                .ToListAsync();

            var result = reservations.Select(r => new GetReservationDto
            {
                Id = r.Id,
                DeviceName = r.Device.Name,
                UserName = r.User.FullName,
                Date = r.Date,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                LabName = r.Device.LocationName,
                Purpose = r.Purpose

            }).ToList();

            return Ok(result);

        }
    }

}
