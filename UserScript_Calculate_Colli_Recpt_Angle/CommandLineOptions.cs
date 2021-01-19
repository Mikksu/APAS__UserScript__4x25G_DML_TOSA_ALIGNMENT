using CommandLine;

namespace UserScript
{
    public interface IOptions
    {
        double Pitch { get; set; }

        double Coeff { get; set; }

        string PrefixVarRead { get; set; }

        string VarNameTheta { get; set; }
        
        string VarNamePosMaxDiff { get; set; }
    }


    [Verb("ry", HelpText = "根据准直Receptacle X轴偏差计算RY修正角度。")]
    public class CalRYOptions : IOptions
    {
        [Option('p', "pitch", Required = false, Default = 0.75,
            HelpText = "通道间间距，单位mm")]
        public double Pitch { get; set; }

        [Option('c', "coeff", Required = false, Default = 0.05,
            HelpText = "通道间间距换算系数")]
        public double Coeff { get; set; }

        [Option('r', "prefix-recept-x", Required = false, Default = "RECEPT_X_",
            HelpText = "Receptacle X轴坐标变量名")]
        public string PrefixVarRead { get; set; }
        
        [Option('w', "prefix-theta-y", Required = false, Default = "THETA_Y", 
            HelpText = "保存计算后的θy的变量名")]
        public string VarNameTheta { get; set; }

        [Option('m', "var-name-pos-max-diff", Required = false, Default = "X_POS_MAX_DIFF", 
            HelpText = "保存X轴坐标极差")]
        public string VarNamePosMaxDiff { get; set; }
    }

    [Verb("rx", HelpText = "根据准直Receptacle Y轴偏差计算RX修正角度。")]
    public class CalRXOptions : IOptions
    {
        [Option('p', "pitch", Required = false, Default = 1,
            HelpText = "通道间间距，单位mm")]
        public double Pitch { get; set; }

        [Option('c', "coeff", Required = false, Default = 0.045,
            HelpText = "通道间间距换算系数")]
        public double Coeff { get; set; }

        [Option('r', "prefix-recept-y", Required = false, Default = "RECEPT_Y_",
            HelpText = "Receptacle Y轴坐标变量名")]
        public string PrefixVarRead { get; set; }
        
        [Option('w', "prefix-theta-x", Required = false, Default = "THETA_X", 
            HelpText = "保存计算后的θx的变量名")]
        public string VarNameTheta { get; set; }
        
        [Option('m', "var-name-pos-max-diff", Required = false, Default = "Y_POS_MAX_DIFF", 
            HelpText = "保存Y轴坐标极差")]
        public string VarNamePosMaxDiff { get; set; }
    }
}