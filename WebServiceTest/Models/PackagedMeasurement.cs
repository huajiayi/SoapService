using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceDemo.Models
{
    public class PackagedMeasurement
    {
        public PackagedMeasurement()
        {
            Recordings = new List<PackagedRecording>();
        }

        public Guid MeasurementID { get; set; }
        public string MeasurementName { get; set; }
        public string MeasurementType { get; set; }
        public int Count { get; set; }
        public string XUnit { get; set; }
        public string YUnit { get; set; }
        public int RecordingsCount { get; set; }

        public IList<PackagedRecording> Recordings { get; set; }
    }
}
