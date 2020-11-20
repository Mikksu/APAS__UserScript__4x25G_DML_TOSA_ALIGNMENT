using CommandLine;
using CommandLine.Text;

namespace UserScript
{

    [Verb("on", HelpText = "打开指定通道的IBias")]
    public class TurnOnOptions
    {
        [Option('c',"channel", Required = true,
            HelpText = "4x25G DML TOSA 通道。")]
        public int Channel { get; set; }
        
        [Option('i',"ibias", Required = true,
            HelpText = "IBias值，单位mA")]
       public double IBias { get; set; }
    }

    [Verb("off", HelpText = "关闭指定通道或所有通道的的IBias")]
    public class TurnOffOptions
    {
        [Option('c',"channel", Required = true,
            HelpText = "4x25G DML TOSA 通道，0表示关闭所有通道。")]
        public int Channel { get; set; }
    }
}