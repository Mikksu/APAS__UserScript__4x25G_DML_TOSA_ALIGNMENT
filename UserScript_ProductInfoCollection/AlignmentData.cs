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

        public string Operator { get; set; }

        public double CollimatorX_CH3 { get; set; }

        public double CollimatorY_CH3 { get; set; }

        public double CollimatorX_CH0 { get; set; }

        public double CollimatorY_CH0 { get; set; }

        public double LDLensGap_CH3 { get; set; }

        public double LDLensGap_CH0 { get; set; }

        public double LDLensPowerAfterAlignment { get; set; }

        public double LDLensPowerBeforeUVCuring { get; set; }

        public double LDLensPowerAfterUVCuring { get; set; }

        public double FiberLensGap { get; set; }

        public double FiberLensPowerAfterAlignment { get; set; }

        public double FiberLensPowerBeforeUV { get; set; }

        public double FiberLensPowerAfterUV { get; set; }


        public DateTime Time { get; set; }
    }
}
