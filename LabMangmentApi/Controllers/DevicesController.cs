using LabMangmentApi.Data;
using LabMangmentApi.Models.Dtos.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMangmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetAllDevices")]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await _context.Devices
                .Select(d => new DeviceListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Status = d.Status,
                    SerialNumber = d.SerialNumber
                })
                .ToListAsync();

            return Ok(devices);
        }




        [HttpGet("deviceById/{id}")]
        public async Task<IActionResult> GetDeviceById(int id)
        {
            var device = await _context.Devices
                .Include(d => d.MaintenanceReports)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound("الجهاز غير موجود");

            var result = new DeviceDetailsDto
            {
                Id = device.Id,
                SerialNumber = device.SerialNumber,
                Name = device.Name,
                Status = device.Status,
                Category = device.CategoryName,
                Location = device.LocationName,
                PurchaseDate = device.PurchaseDate,
                Lifespan = device.Lifespan,
                LastMaintenance = device.LastMaintenanceDate,
                Notes = device.Notes,
                MaintenanceHistory = device.MaintenanceReports
                    .Select(m => new MaintenanceDto
                    {
                        Date = m.SchedulingAt,
                        Description = m.Type,
                        Status = m.Status
                    }).ToList()
            };

            return Ok(result);
        }




        [HttpPost("AddDevice")]
        public async Task<IActionResult> AddDevice([FromBody] CreateDeviceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var device = new Device
            {
                Name = dto.Name,
                SerialNumber = dto.SerialNumber,
                Status = dto.Status,
                CategoryName = dto.Category,
                LocationName = dto.Location,
                PurchaseDate = dto.PurchaseDate,
                Lifespan = dto.Lifespan,
                Notes = dto.Notes,
                CurrentHour = 0,
                MaximumHour = 100, // تقدر تعدلها حسب السيستم بتاعك
                LastMaintenanceDate = null,
                NextMaintenanceDate = null,
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم اضافه الجهاز بنجاح" });
        }


        [HttpPut("updateDeviceById/{id}")]
        public async Task<IActionResult> UpdateDevice(int id, [FromBody] UpdateDeviceDto dto)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return NotFound("الجهاز غير موجود");

            device.Name = dto.Name;
            device.SerialNumber = dto.SerialNumber;
            device.Status = dto.Status;
            device.CategoryName = dto.Category;
            device.LocationName = dto.Location;
            device.PurchaseDate = dto.PurchaseDate;
            device.Lifespan = dto.Lifespan;
            device.Notes = dto.Notes;

            _context.Devices.Update(device);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تعديل بيانات الجهاز بنجاح" });
        }

        [HttpDelete("deleteDeviceById/{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
            if (device == null)
                return NotFound("الجهاز غير موجود");

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return Ok("تم حذف الجهاز بنجاح");
        }



    }
}
