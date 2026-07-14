using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotXR.Application.DTOs.Request.Result
{
    public class SubmitEventLogRequest
    {
        public string EventType { get; set; } = null!;   
        public DateTime EventTime { get; set; }           
        public string? Description { get; set; }
    }
}
