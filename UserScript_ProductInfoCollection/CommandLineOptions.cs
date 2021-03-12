using CommandLine;

namespace UserScript
{
    public class Option
    {
        [Option( "filename-prefix", Required = false, Default = "4x25G_DML_TOSA_Alignment_Data_",
            HelpText = "文件名前缀，完整的文件名包含当天日期信息。")]
        public string FilenamePrefix { get; set; }
    }
}