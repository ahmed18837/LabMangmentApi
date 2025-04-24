using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LabMangmentApi.Data;
using LabMangmentApi.Models.Entities;
using LabMangmentApi.Models.Dtos.Maintenance;

namespace YourNamespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetAllMaintenance()
        {
            try
            {
                var allMaintenance = await _context.Maintenances
                    .Include(m => m.Device)
                    .Include(m => m.user)
                    .AsNoTracking()
                    .Select(m => new MaintenanceResponseDto
                    {
                        Id = m.Id,
                        DeviceName = m.Device.Name,
                        MaintenanceType = m.Type,
                        ScheduledDate = m.SchedulingAt.ToString("yyyy-MM-dd"),
                        Responsible = m.user.FullName,
                        Priority = m.Priority,
                        Status = m.Status,
                        StartAt = m.StartAt,
                        EndAt = m.EndAt,
                        Notes = m.Notes,
                        Cost = m.Cost,
                        Reason = m.Reason
                    })
                    .ToListAsync();

                return Ok(allMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }

        [HttpGet("MaintenanceById/{id}")]
        public async Task<ActionResult<MaintenanceResponseDto>> GetMaintenanceById(int id)
        {
            if (id <= 0)
                return BadRequest("معرّف الصيانة غير صحيح");

            try
            {
                var maintenance = await _context.Maintenances
                    .Include(m => m.Device)
                    .Include(m => m.user)
                    .AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new MaintenanceResponseDto
                    {
                        Id = m.Id,
                        DeviceName = m.Device.Name,
                        MaintenanceType = m.Type,
                        ScheduledDate = m.SchedulingAt.ToString("yyyy-MM-dd"),
                        Responsible = m.user.FullName,
                        Priority = m.Priority,
                        Status = m.Status,
                        StartAt = m.StartAt,
                        EndAt = m.EndAt,
                        Notes = m.Notes,
                        Cost = m.Cost,
                        Reason = m.Reason
                    })
                    .FirstOrDefaultAsync();

                if (maintenance == null)
                    return NotFound($"لم يتم العثور على صيانة برقم التعريف {id}");

                return Ok(maintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }

   

        [HttpPut("startTime/{id}")]
        public async Task<IActionResult> UpdateStartTime(int id)
        {
           

            try
            {
                var maintenance = await _context.Maintenances.FirstOrDefaultAsync(m => m.Id == id);

                if (maintenance == null)
                    return NotFound($"لم يتم العثور على صيانة برقم التعريف {id}");

                maintenance.StartAt = DateTime.Now;
                maintenance.Status = "قيد التنفيذ";

                await _context.SaveChangesAsync();

                return Ok(new { message = "تم تحديث وقت البداية بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }

   

        [HttpPut("endTime/{id}")]
        public async Task<IActionResult> UpdateEndTimeint (int id, [FromBody] EndTimeDto model)
        {
            if (model == null || id <= 0)
                return BadRequest("بيانات غير صالحة");


            try
            {
                var maintenance = await _context.Maintenances.FindAsync(id);
                if (maintenance == null)
                    return NotFound($"لم يتم العثور على صيانة برقم التعريف {id}");

                // Get the device to update its status
                var device = await _context.Devices.FindAsync(maintenance.DeviceId);
                if (device == null)
                    return NotFound($"لم يتم العثور على الجهاز المرتبط بالصيانة");

                // Update maintenance information
                maintenance.EndAt = DateTime.Now;
                maintenance.Status = "مكتملة";
                maintenance.Cost = model.Cost;
                maintenance.Notes = model.Notes;

                // Update device status to available and reset current hours
                device.Status = "متاح";
                device.CurrentHour = 0;

                await _context.SaveChangesAsync();

                return Ok(new { message = "تم تحديث وقت الانتهاء بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }


        [HttpPut("update/{id}")]
        public async Task<ActionResult<MaintenanceResponseDto>> UpdateMaintenance(int id, [FromBody] UpdateMaintenanceFullDto model)
        {
            if (id <= 0 || model == null)
                return BadRequest("بيانات غير صالحة");

            try
            {
                var maintenance = await _context.Maintenances
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (maintenance == null)
                    return NotFound($"لم يتم العثور على صيانة برقم التعريف {id}");

                // تحديث بيانات الجهاز إذا تم تغيير الاسم
                if (!string.IsNullOrEmpty(model.DeviceName))
                {
                    var device = await _context.Devices
                        .FirstOrDefaultAsync(d => d.Name == model.DeviceName);

                    if (device != null)
                    {
                        maintenance.DeviceId = device.Id;
                    }
                    else
                    {
                        return BadRequest("الجهاز غير موجود");
                    }
                }

                // تحديث بيانات المسؤول إذا تم تغيير الاسم
                if (!string.IsNullOrEmpty(model.Responsible))
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.FullName == model.Responsible);

                    if (user != null)
                    {
                        maintenance.UserId = user.Id;
                    }
                    else
                    {
                        return BadRequest("المسؤول غير موجود");
                    }
                }

                // تحديث باقي البيانات
                if (!string.IsNullOrEmpty(model.Type))
                    maintenance.Type = model.Type;

                if (!string.IsNullOrEmpty(model.ScheduledDate))
                {
                    if (DateTime.TryParse(model.ScheduledDate, out DateTime scheduledDate))
                    {
                        maintenance.SchedulingAt = scheduledDate;
                    }
                    else
                    {
                        return BadRequest("صيغة التاريخ غير صحيحة");
                    }
                }

                if (!string.IsNullOrEmpty(model.Status))
                    maintenance.Status = model.Status;

                if (!string.IsNullOrEmpty(model.Priority))
                    maintenance.Priority = model.Priority;

                if (!string.IsNullOrEmpty(model.Notes))
                    maintenance.Notes = model.Notes;

                if (!string.IsNullOrEmpty(model.Reason))
                    maintenance.Reason = model.Reason;

                if (model.StartAt.HasValue)
                    maintenance.StartAt = model.StartAt.Value;

                if (model.EndAt.HasValue)
                    maintenance.EndAt = model.EndAt.Value;

                if (model.Cost.HasValue)
                    maintenance.Cost = model.Cost.Value; // ملاحظة: يجب تصحيح الاسم إلى Cost في نموذج البيانات

                await _context.SaveChangesAsync();

                // إعادة قراءة البيانات بعد التحديث
                var updatedMaintenance = await _context.Maintenances
                    .Include(m => m.Device)
                    .Include(m => m.user)
                    .AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new MaintenanceResponseDto
                    {
                        Id = m.Id,
                        DeviceName = m.Device.Name,
                        MaintenanceType = m.Type,
                        ScheduledDate = m.SchedulingAt.ToString("yyyy-MM-dd"),
                        Responsible = m.user.FullName,
                        Priority = m.Priority,
                        Status = m.Status,
                        Notes = m.Notes,
                        Reason = m.Reason,
                        StartAt = m.StartAt,
                        EndAt = m.EndAt,
                        Cost = m.Cost
                    })
                    .FirstOrDefaultAsync();

                return Ok(updatedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }

        [HttpPut("cancel/{id}")]
        public async Task<ActionResult<MaintenanceResponseDto>> CancelMaintenance(int id)
        {
            if (id <= 0)
                return BadRequest("معرّف الصيانة غير صحيح");

            try
            {
                var maintenance = await _context.Maintenances.FindAsync(id);

                if (maintenance == null)
                    return NotFound($"لم يتم العثور على صيانة برقم التعريف {id}");

                // تغيير الحالة إلى ملغاة
                maintenance.Status = "ملغاة";

                await _context.SaveChangesAsync();

                // إعادة قراءة البيانات بعد التحديث
                var updatedMaintenance = await _context.Maintenances
                    .Include(m => m.Device)
                    .Include(m => m.user)
                    .AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new MaintenanceResponseDto
                    {
                        Id = m.Id,
                        DeviceName = m.Device.Name,
                        MaintenanceType = m.Type,
                        ScheduledDate = m.SchedulingAt.ToString("yyyy-MM-dd"),
                        Responsible = m.user.FullName,
                        Priority = m.Priority,
                        Status = m.Status,
                        Notes = m.Notes
                    })
                    .FirstOrDefaultAsync();

                return Ok(updatedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }

        [HttpGet("MaintenanceHistory")]
        public async Task<ActionResult<IEnumerable<MaintenanceResponseDto>>> GetMaintenanceHistory()
        {
            try
            {
                var completedMaintenance = await _context.Maintenances
                    .Include(m => m.Device)
                    .Include(m => m.user)
                    .AsNoTracking()
                    .Where(m => m.Status == "مكتملة")
                    .OrderByDescending(m => m.EndAt)
                    .Select(m => new MaintenanceResponseDto
                    {
                        Id = m.Id,
                        DeviceName = m.Device.Name,
                        MaintenanceType = m.Type,
                        ScheduledDate = m.StartAt.HasValue ? m.StartAt.Value.ToString("yyyy-MM-dd") : null,
                        Responsible = m.user.FullName,
                        Cost = m.Cost,
                        Priority = m.Priority,
                        Notes = m.Notes,
                        Reason = m.Reason,
                        StartAt = m.StartAt,
                        EndAt = m.EndAt,
                        
                    })
                    .ToListAsync();

                return Ok(completedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }



        [HttpPost("addScheduledMaintenance")]
        public async Task<ActionResult<MaintenanceResponseDto>> AddScheduledMaintenance([FromBody] AddScheduledMaintenanceDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.DeviceName) ||
                string.IsNullOrEmpty(model.Responsible) || string.IsNullOrEmpty(model.Type))
            {
                return BadRequest("البيانات المطلوبة غير مكتملة");
            }

            try
            {
                // Find the device by name
                var device = await _context.Devices.FirstOrDefaultAsync(d => d.Name == model.DeviceName);
                if (device == null)
                {
                    return BadRequest("الجهاز غير موجود");
                }

                // Find the user by name
                var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == model.Responsible);
                if (user == null)
                {
                    return BadRequest("المسؤول غير موجود");
                }

                var maintenance = new Maintenance
                {
                    DeviceId = device.Id,
                    UserId = user.Id,
                    Type = model.Type,
                    SchedulingAt = DateTime.Now,
                    Status = "مجدولة", // تصحيح الإملاء من "مجدوله" إلى "مجدولة"
                    Reason = model.Reason,
                    Priority = null,
                    Notes = null,
                    Cost = 0,
                    StartAt = null,
                    EndAt = null
                };
                device.Status = "قيد الصيانة";
                _context.Maintenances.Add(maintenance);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطأ في الخادم: {ex.Message}");
            }
        }
    }
}