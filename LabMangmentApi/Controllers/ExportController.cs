using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LabMangmentApi.Data;
using LabMangmentApi.Models.Dtos.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Globalization;


[Route("api/export")]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExportController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("excel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] string? device, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var data = await GetFilteredUsage(device, startDate, endDate);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Device Usage");

        // Headers
        var props = typeof(DeviceUsageReportDto).GetProperties();
        for (int i = 0; i < props.Length; i++)
            worksheet.Cells[1, i + 1].Value = props[i].Name;

        // Data
        for (int row = 0; row < data.Count; row++)
        {
            for (int col = 0; col < props.Length; col++)
            {
                worksheet.Cells[row + 2, col + 1].Value = props[col].GetValue(data[row]);
            }
        }

        var excelBytes = package.GetAsByteArray();
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "device-usage.xlsx");
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> ExportToPdf([FromQuery] string? device, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var data = await GetFilteredUsage(device, startDate, endDate);

        using var ms = new MemoryStream();
        var doc = new Document(PageSize.A4);
        var writer = PdfWriter.GetInstance(doc, ms);
        doc.Open();

        // 🔹 تحميل خط يدعم العربية
        string fontPath = "C:\\Windows\\Fonts\\arial.ttf"; // تأكد من وجود الخط
        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        Font arabicFont = new Font(baseFont, 12, Font.NORMAL);
        Font titleFont = new Font(baseFont, 16, Font.BOLD);

        // 🔹 إضافة العنوان في جدول RTL
        PdfPTable titleTable = new PdfPTable(1)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100
        };

        PdfPCell titleCell = new PdfPCell(new Phrase("سجل استخدام الأجهزة", titleFont))
        {
            Border = Rectangle.NO_BORDER,
            HorizontalAlignment = Element.ALIGN_CENTER
        };

        titleTable.AddCell(titleCell);
        doc.Add(titleTable);
        doc.Add(new Chunk("\n"));

        // 🔹 إنشاء الجدول
        var props = typeof(DeviceUsageReportDto).GetProperties();
        var table = new PdfPTable(props.Length)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100
        };

        // 🔸 رؤوس الأعمدة
        foreach (var prop in props)
        {
            var cell = new PdfPCell(new Phrase(prop.Name, arabicFont))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            table.AddCell(cell);
        }

        // 🔸 بيانات الجدول
        foreach (var item in data)
        {
            foreach (var prop in props)
            {
                string value = prop.GetValue(item)?.ToString() ?? string.Empty;
                var cell = new PdfPCell(new Phrase(value, arabicFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };
                table.AddCell(cell);
            }
        }

        doc.Add(table);
        doc.Close();

        return File(ms.ToArray(), "application/pdf", "device-usage.pdf");
    }

    private async Task<List<DeviceUsageReportDto>> GetFilteredUsage(string? device, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Reservations
            .Include(r => r.Device)
            .Include(r => r.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(device))
            query = query.Where(r => r.Device.Name.Contains(device));

        if (startDate.HasValue)
            query = query.Where(r => r.Date >= DateOnly.FromDateTime(startDate.Value));

        if (endDate.HasValue)
            query = query.Where(r => r.Date <= DateOnly.FromDateTime(endDate.Value));

        return await query.Select(r => new DeviceUsageReportDto
        {
            Device = r.Device.Name,
            DeviceId = r.Device.SerialNumber.ToString(),
            User = r.User.FullName,
            Date = r.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            StartTime = r.StartTime.ToString(@"hh\:mm"),
            EndTime = r.EndTime.ToString(@"hh\:mm"),
            Hours = (r.EndTime - r.StartTime).TotalHours
        }).ToListAsync();
    }
}
