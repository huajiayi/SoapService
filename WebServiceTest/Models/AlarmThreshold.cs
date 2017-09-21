using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceDemo.Models
{
    public class AlarmThreshold
    {
        public int AlarmIfLessThan { get; set; }
        public int Level { get; set; }
        public double Value { get; set; }
    }
}