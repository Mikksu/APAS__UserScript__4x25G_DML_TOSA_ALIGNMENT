using CommandLine;

namespace UserScript
{
    public class Options
    {

        #region Coupling Behaviour Control

        [Option("enable-blindsearch", Required = false, Default = true,
           HelpText = "是否执行盲扫")]
        public bool EnableBlindSearch { get; set; }

        [Option("enable-fast-fouse-scan", Required = false, Default = true,
          HelpText = "是否执行快速焦距扫描")]
        public bool EnableFastFocusScan { get; set; }

        [Option("enable-lens-profile-scan", Required = false, Default = true,
          HelpText = "是否执行Lens慢速扫描")]
        public bool EnableLensProfileScan { get; set; }

        [Option("enable-dual-profile-scan", Required = false, Default = true,
          HelpText = "是否执行Receptacle和Lens双边慢速扫描")]
        public bool EnableReceptLensDualScan { get; set; }

        [Option("enable-final-finetune", Required = false, Default = true,
          HelpText = "是否执行爬山算法进行最终微调")]
        public bool EnableFinalFineTune { get; set; }

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

        [Option( "fscan-range", Required = false, Default = 50,
            HelpText = "快速焦距扫描的最大移动范围")]
        public double FocusScanRange { get; set; }

        [Option("fscan-final-step", Required = false, Default = 2,
            HelpText = "快速焦距扫描的最小步进，当扫描步进收敛到此值时终止扫描")]
        public double FocusScanFinalStep { get; set; }

        [Option("pth-lscan-n", Required = false, Default = -0.1,
            HelpText = "慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanN { get; set; }

        [Option("pth-lscan-p", Required = false, Default = 0.2,
            HelpText = "慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanP { get; set; }

        [Option("pth-dual-lscan-n", Required = false, Default = -0.1,
            HelpText = "双边慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanN { get; set; }

        [Option("pth-dual-lscan-p", Required = false, Default = 0.2,
            HelpText = "双边慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanP { get; set; }

        [Option("powermeter", Required = false, Default = "PM1906A2",
            HelpText = "读取功率使用的功率计名称")]
        public string PowerMeterCaption { get; set; }

        #endregion

        #region 耦合使用的配置文件名定义

        [Option("profilename--blindsearch-lens", Required = false, Default = "LD_Lens_初始光",
           HelpText = "Lens找初始光使用的配置文件")]
        public string ProfileNameBlindSearch { get; set; }

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

        [Option("profilename-hillclimb", Required = false, Default = "LD_Lens_FineTune",
            HelpText = "爬山扫描使用的配置文件")]
        public string ProfileNameHillClimb { get; set; }

        #endregion
    }
}