using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceDemo.Models
{
    public class MeasurementPath
    {
        public string MachineName { get; set; }
        public string SensorName { get; set; }
        public string MeasurementName { get; set; }
        public Guid MeasurementID { get; set; }
    }
}