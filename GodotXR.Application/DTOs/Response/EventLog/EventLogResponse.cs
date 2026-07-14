using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Response.EventLog
{
    public class EventLogResponse
    {
        public int Id { get; set; }
        public int ResultId { get; set; }
        public int ChildId { get; set; }
        public string EventType { get; set; } = null!;
        public DateTime EventTime { get; set; }
        public string? Description { get; set; }
    }
}
