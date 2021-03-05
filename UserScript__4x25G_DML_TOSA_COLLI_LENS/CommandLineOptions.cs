using CommandLine;

namespace UserScript
{
    public class Options
    {

        #region Coupling Behaviour Control

        [Option("ignore-blindsearch", Required = false,
           HelpText = "是否忽略盲扫")]
        public bool IgnoreBlindSearch { get; set; }

        [Option("ignore-fast-fouse-scan", Required = false,
          HelpText = "是否忽略快速焦距扫描")]
        public bool IgnoreFastFocusScan { get; set; }

        [Option("ignore-lens-profile-scan", Required = false,
          HelpText = "是否忽略Lens慢速扫描")]
        public bool IgnoreLensProfileScan { get; set; }

        [Option("ignore-dual-scan", Required = false,
          HelpText = "是否忽略Receptacle和Lens双边慢速扫描")]
        public bool IgnoreReceptLensDualScan { get; set; }

        [Option("ignore-final-finetune", Required = false,
          HelpText = "是否忽略爬山算法进行最终微调")]
        public bool IgnoreFinalFineTune { get; set; }

        [Option("use-profile-nd-in-dual-scan-for-recept", Required = false,
          HelpText = "是否使用Profile ND算法进行Receptacle-Lens双面扫描。默认使用Fast ND算法扫描Receptacle端")]
        public bool UseProfileNdInReceptLensDualScan { get; set; }

        [Option("use-hill-climb-in-lens-align", Required = false,
         HelpText = "是否使用HillClimb对Lens耦合。")]
        public bool UseHillClimbInLensAlign { get; set; }

        [Option("use-hill-climb-in-final-fine-tune", Required = false,
         HelpText = "是否使用HillClimb对Lens进行最终细调。")]
        public bool UseHillClimbInFinalFineTune { get; set; }

        #endregion

        #region 耦合参数定义

        [Option("pth-rascan", Required = false, Default = -25,
            HelpText = "盲扫退出的阈值功率，单位dBm")]
        public double PowerThreRectAreaScan { get; set; }

        [Option("pth-fscan", Required = false, Default = 0,
            HelpText = "快速焦距扫描退出的阈值功率，单位dBm")]
        public double PowerThreFocusScan { get; set; }

        [Option("fscan-step", Required = false, Default = 10,
            HelpText = "快速焦距扫描的步进")]
        public double FocusScanStep { get; set; }

        [Option( "fscan-range", Required = false, Default = 100,
            HelpText = "快速焦距扫描的最大移动范围")]
        public double FocusScanRange { get; set; }

        [Option("fscan-final-step", Required = false, Default = 2,
            HelpText = "快速焦距扫描的最小步进，当扫描步进收敛到此值时终止扫描")]
        public double FocusScanFinalStep { get; set; }

        [Option("pth-lscan-n", Required = false, Default = -0.2,
            HelpText = "慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanN { get; set; }

        [Option("pth-lscan-p", Required = false, Default = 0.5,
            HelpText = "慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanP { get; set; }

        [Option("pth-dual-lscan-n", Required = false, Default = -0.1,
            HelpText = "双边慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanN { get; set; }

        [Option("pth-dual-lscan-p", Required = false, Default = 0.3,
            HelpText = "双边慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanP { get; set; }

        [Option("pth-terminate", Required = false, Default = 7,
            HelpText = "耦合终止阈值功率，当超过此功率时结束耦合，单位dBm")]
        public double PowerThreTerminate { get; set; }

        [Option("powermeter", Required = false, Default = "PM1906A2",
            HelpText = "读取功率使用的功率计名称")]
        public string PowerMeterCaption { get; set; }

        #endregion

        #region 耦合使用的配置文件名定义

        [Option("profilename--blindsearch-lens", Required = false, Default = "LD_Lens_初始光",
           HelpText = "Lens找初始光使用的配置文件")]
        public string ProfileNameBlindSearch { get; set; }

        [Option("profilename--blindsearch-lens-s", Required = false, Default = "LD_Lens_初始光_s",
           HelpText = "Lens小步进找初始光使用的配置文件")]
        public string ProfileNameBlindSearchSmallStep { get; set; }

        [Option("profilename--focusscan-lens", Required = false, Default = "LD_Lens_XY_0.2_20_Z_1_20",
            HelpText = "焦距扫描Lens端扫描使用的配置文件")]
        public string ProfileNameFocusScanLens { get; set; }

        [Option("profilename-focusscan-recept", Required = false, Default = "准直Recept_XY_5_200",
            HelpText = "焦距扫描Receptacle端扫描使用的配置文件")]
        public string ProfileNameFocusScanRecept { get; set; }

        [Option("profilename-linescan-lens", Required = false, Default = "LD_Lens_XY_0.2_10_Z_0.5_10",
            HelpText = "慢速扫描Lens端扫描使用的配置文件")]
        public string ProfileNameLineScanLens { get; set; }

        [Option("profilename-dual-linescan-recept", Required = false, Default = "准直Recept_XY_5_200",
            HelpText = "双边慢速扫描Receptacle端扫描使用的配置文件")]
        public string ProfileNameDualLineScanRecept { get; set; }

        [Option("profilename-dual-linescan-lens", Required = false, Default = "LD_Lens_XY_0.2_10_Z_0.5_10",
            HelpText = "双边慢速扫描Lens端扫描使用的配置文件")]
        public string ProfileNameDualLineScanLens { get; set; }

        [Option("profilename-final-fine-tune", Required = false, Default = "LD_Lens_FineTune",
            HelpText = "最终细调时使用的配置文件")]
        public string ProfileNameFinalFineTune { get; set; }

        #endregion
    }
}