using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDemo.Models
{
    public class Header
    {
        public Header()
        {
            Measurements = new List<PackagedMeasurement>();
        }

        public byte Version { get; set; }
        public Guid SiteID { get; set; }
        public string SiteName { get; set; }
        public Guid MachineID { get; set; }
        public string MachineName { get; set; }
        public Guid SensorID { get; set; }
        public string SensorName { get; set; }
        public byte MeasurementCount { get; set; }
        public int RecordingCount { get; set; }
        public int DataBytesCount { get; set; }

        public IList<PackagedMeasurement> Measurements { get; set; }
    }
}
