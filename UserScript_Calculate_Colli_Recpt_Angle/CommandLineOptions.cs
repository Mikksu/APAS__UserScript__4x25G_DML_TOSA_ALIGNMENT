using CommandLine;

namespace UserScript
{
    public interface IOptions
    {
        double Pitch { get; set; }

        double Coeff { get; set; }

        string PrefixVarRead { get; }

        string PrefixVarWrite { get; }
    }


    [Verb("ry", HelpText = "根据准直Receptacle X轴偏差计算RY修正角度。")]
    public class CalRYOptions : IOptions
    {
        [Option('p', "pitch", Required = false, Default = 0.75,
            HelpText = "通道间间距，单位mm")]
        public double Pitch { get; set; }

        [Option('c', "coeff", Required = true, HelpText = "通道间间距换算系数")]
        public double Coeff { get; set; }

        public string PrefixVarRead { get; } = "RECEPT_X_";

        public string PrefixVarWrite { get; } = "RY";
    }

    [Verb("rx", HelpText = "根据准直Receptacle Y轴偏差计算RX修正角度。")]
    public class CalRXOptions : IOptions
    {
        [Option('p', "pitch", Required = false, Default = 1,
            HelpText = "通道间间距，单位mm")]
        public double Pitch { get; set; }

        [Option('c', "coeff", Required = true, HelpText = "通道间间距换算系数")]
        public double Coeff { get; set; }


        public string PrefixVarRead { get; } = "RECEPT_Y_";

        public string PrefixVarWrite { get; } = "RX";
    }
}