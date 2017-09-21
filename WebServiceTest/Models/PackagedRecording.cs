using MHCC.Common.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDemo.Models
{
    public class PackagedRecording
    {
        public DateTime Timestamp { get; set; }
        public byte Type { get; set; }
        public Alarmstatus Alarmstatus { get; set; }
        public float BiasVoltage { get; set; }
        public float Delta { get; set; }
        public float Offset { get; set; }
        public float Overall { get; set; }
        public float RPM { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
    }
}
