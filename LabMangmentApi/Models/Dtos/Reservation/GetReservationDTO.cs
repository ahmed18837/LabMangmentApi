using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physics.DAL.Models.DTO
{
    public class GetReservationDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string UserName { get; set; }
        public string LabName { get; set; }
        public String Purpose { get; set; }


    }
}

