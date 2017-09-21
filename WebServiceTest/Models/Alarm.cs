using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServiceDemo.Models
{
    public class Alarm
    {
        public string Name { get; set; }
        public int AlarmType { get; set; }
        public double? FrequenceStart { get; set; }
        public double? FrequenceEnd { get; set; }
        public double? ThresholdWarning { get; set; }
        public double? ThresholdAlert { get; set; }
        public double? ThresholdDanger { get; set; }
        public double? ThresholdMinusWarning { get; set; }
        public double? ThresholdMinusAlert { get; set; }
        public double? ThresholdMinusDanger { get; set; }
        public double CalculatedValue { get; set; }
        public int AlarmStatus { get; set; }
    }
}
