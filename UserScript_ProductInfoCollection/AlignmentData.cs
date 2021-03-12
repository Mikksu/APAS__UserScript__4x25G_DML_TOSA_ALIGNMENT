using System;
using CsvHelper.Configuration.Attributes;

namespace UserScript
{
    public class AlignmentData
    {
        public string Sn { get; set; }

        public string Pn { get; set; }

        public string Traveler { get; set; }

        public string WorkOrder { get; set; }

        public string Op { get; set; }

        public double CollimatorX { get; set; }

        public double CollimatorY { get; set; }

        public double LDLensPowerBeforeUV { get; set; }
        
        public double LDLensPowerAfterUV { get; set; }

        public double  LDLensGap { get; set; }

        public double FiberLensPowerBeforeUV { get; set; }

        public double FiberLensPowerAfterUV { get; set; }

        public double FiberLensGap { get; set; }

        public DateTime Time { get; set; }
    }
}
