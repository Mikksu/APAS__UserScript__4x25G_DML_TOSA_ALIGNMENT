using CommandLine;

namespace UserScript
{
    public class Option
    {
        [Option( "filename", Required = false, Default = "4x25G_DML_TOSA_Alignment_Data",
            HelpText = "文件名前缀，完整的文件名包含当天日期信息。")]
        public string Filename { get; set; }

        [Option("lens-type", Required = true,
            HelpText = "当前耦合的Lens类型，请填写LD Lens或Fiber Lens。")]
        public string LensType { get; set; }
    }
}