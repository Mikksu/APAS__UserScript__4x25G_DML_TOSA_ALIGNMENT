using System;
using CsvHelper.Configuration.Attributes;

namespace UserScript
{
    public class AlignmentData
    {
        [Index(0)]
        public string Sn { get; set; }

        [Index(1)]
        public string Pn { get; set; }

        [Index(2)]
        public string Traveler { get; set; }

        [Index(3)]
        public string WorkOrder { get; set; }

        [Index(4)]
        public string Op { get; set; }

        [Index(5)]
        public double PowerBeforeUV { get; set; }
        
        [Index(6)]
        public double PowerAfterUV { get; set; }

        [Index(7)]
        public double  LensGap { get; set; }

        [Index(8)]
        public string LensType { get; set; }
        
        [Index(9)]
        public DateTime Time { get; set; }
    }
}
