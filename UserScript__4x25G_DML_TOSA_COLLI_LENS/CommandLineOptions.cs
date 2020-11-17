using CommandLine;

namespace UserScript
{
    public class Options
    {
        //[Option('h', "help", Required = false, HelpText = "打印帮助文件")]
        //public bool IsHelpTextRequired { get; set; }

        #region 耦合参数定义

        [Option('a', "pth-rascan", Required = false, HelpText = "盲扫退出的阈值功率，单位dBm")]
        public double PowerThreRectAreaScan { get; set; } = -25;

        [Option('b', "pth-fscan", Required = false, HelpText = "快速焦距扫描退出的阈值功率，单位dBm")]
        public double PowerThreFocusScan { get; set; } = 0;

        [Option('d', "fscan-step", Required = false, HelpText = "快速焦距扫描的步进")]
        public double FocusScanStep { get; set; } = 10;

        [Option('e', "fscan-range", Required = false, HelpText = "快速焦距扫描的最大移动范围")]
        public double FocusScanRange { get; set; } = 50;

        [Option('f', "fscan-final-step", Required = false, HelpText = "快速焦距扫描的最小步进，当扫描步进收敛到此值时终止扫描")]
        public double FocusScanFinalStep { get; set; } = 2;

        [Option('g', "pth-lscan-n", Required = false,
            HelpText = "慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanN { get; set; } = -0.1;

        [Option('i', "pth-lscan-p", Required = false,
            HelpText = "慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreLineScanP { get; set; } = 0.2;

        [Option('g', "pth-dual-lscan-n", Required = false,
            HelpText = "双边慢速扫描退出的阈值功率最小值，如果最后两次扫描功率之差大于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanN { get; set; } = -0.1;

        [Option('k', "pth-dual-lscan-p", Required = false,
            HelpText = "双边慢速扫描退出的阈值功率最大值，如果最后两次扫描功率之差小于此值时退出耦合，单位dBm")]
        public double PowerThreDualLineScanP { get; set; } = 0.2; 

        #endregion


        #region 耦合使用的配置文件名定义

        [Option('l', "profilename--focusscan-colli-lens", Required = false,
            HelpText = "焦距扫描Lens端扫描使用的配置文件")]
        public string ProfileNameFocusScanColliLens { get; set; } = "准直Lens_XY_0.2_20_Z_1_20";

        [Option('m', "profilename-focusscan-colli-recept", Required = false,
            HelpText = "焦距扫描Receptacle端扫描使用的配置文件")]
        public string ProfileNameFocusScanColliRecept { get; set; } = "准直Rept_XY_5_200";

        [Option('q', "profilename-dual-lscan-lens", Required = false,
            HelpText = "慢速扫描Lens端扫描使用的配置文件")]
        public string ProfileNameLineScanLens { get; set; } = "准直Lens_XY_0.2_10_Z_0.5_10";

        [Option('r', "profilename-dual-lscan-colli-recept", Required = false,
            HelpText = "双边慢速扫描Receptacle端扫描使用的配置文件")]
        public string ProfileNameDualLineScanColliRecept { get; set; } = "准直Rept_XY_5_200";

        [Option('s', "profilename-dual-lscan-colli-lens", Required = false,
            HelpText = "双边慢速扫描Lens端扫描使用的配置文件")]
        public string ProfileNameDualLineScanColliLens { get; set; } = "准直Lens_XY_0.2_10_Z_0.5_10";

        [Option('t', "profilename-hillclimb", Required = false,
            HelpText = "爬山扫描使用的配置文件")]
        public string ProfileNameHillClimb { get; set; } = "准直Lens_FineTune"; 

        #endregion
    }
}
