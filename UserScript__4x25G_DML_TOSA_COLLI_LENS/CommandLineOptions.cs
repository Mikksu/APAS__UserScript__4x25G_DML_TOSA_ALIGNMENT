using CommandLine;

namespace UserScript
{
    public class Options
    {
        #region 耦合参数定义

        [Option('a', "pth-rascan", Required = false, Default = -25,
            HelpText = "盲扫退出的阈值功率，单位dBm")]
        public double PowerThreRectAreaScan { get; set; }

        [Option('b', "pth-fscan", Required = false, Default = 0,
            HelpText = "快速焦距扫描退出的阈值功率，单位dBm")]
        public double PowerThreFocusScan { get; set; }

        [Option('d', "fscan-step", Required = false, Default = 10,
            HelpText = "快速焦距扫描的步进")]
        public double FocusScanStep { get; set; }

        [Option('e', "fscan-range", Required = false, Default = 50,
            HelpText = "快速焦距扫描的最大移动范围")]
        public double FocusScanRange { get; set; }

        [Option('f', "fscan-final-step", Required = false, Default = 2,
            HelpText = "快速焦距扫描的最小步进，当扫描步进收敛到此值时终止扫描")]
        public double FocusScanFinalStep { get; set; }

        [Option('g', "pth-lscan-n", Required = false, Default = -0.1,
            HelpText = "慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanN { get; set; }

        [Option('i', "pth-lscan-p", Required = false, Default = 0.2,
            HelpText = "慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanP { get; set; }

        [Option('g', "pth-dual-lscan-n", Required = false, Default = -0.1,
            HelpText = "双边慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanN { get; set; }

        [Option('k', "pth-dual-lscan-p", Required = false, Default = 0.2,
            HelpText = "双边慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanP { get; set; }

        #endregion

        #region 耦合使用的配置文件名定义

        [Option('l', "profilename--focusscan-colli-lens", Required = false, Default = "准直Lens_XY_0.2_20_Z_1_20",
            HelpText = "焦距扫描Lens端扫描使用的配置文件")]
        public string ProfileNameFocusScanColliLens { get; set; }

        [Option('m', "profilename-focusscan-colli-recept", Required = false, Default = "准直Rept_XY_5_200",
            HelpText = "焦距扫描Receptacle端扫描使用的配置文件")]
        public string ProfileNameFocusScanColliRecept { get; set; }

        [Option('q', "profilename-dual-lscan-lens", Required = false, Default = "准直Lens_XY_0.2_10_Z_0.5_10",
            HelpText = "慢速扫描Lens端扫描使用的配置文件")]
        public string ProfileNameLineScanLens { get; set; }

        [Option('r', "profilename-dual-lscan-colli-recept", Required = false, Default = "准直Rept_XY_5_200",
            HelpText = "双边慢速扫描Receptacle端扫描使用的配置文件")]
        public string ProfileNameDualLineScanColliRecept { get; set; }

        [Option('s', "profilename-dual-lscan-colli-lens", Required = false, Default = "准直Lens_XY_0.2_10_Z_0.5_10",
            HelpText = "双边慢速扫描Lens端扫描使用的配置文件")]
        public string ProfileNameDualLineScanColliLens { get; set; }

        [Option('t', "profilename-hillclimb", Required = false, Default = "准直Lens_FineTune",
            HelpText = "爬山扫描使用的配置文件")]
        public string ProfileNameHillClimb { get; set; }

        #endregion
    }
}