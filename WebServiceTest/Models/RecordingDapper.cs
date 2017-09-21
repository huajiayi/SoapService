using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceDemo.Models
{
    public class RecordingDapper
    {
        public DateTime Timestamp { get; set; }
        public string MeasurementType { get; set; }
        public int AlarmStatus { get; set; }
        public double BiasVoltage { get; set; }
        public double Delta { get; set; }
        public double Offset { get; set; }
        public double Overall { get; set; }
        public double RPM { get; set; }
        public string XUnit { get; set; }
        public int YCount { get; set; }
        public string YUnit { get; set; }
        public int Type { get; set; }
    }
}