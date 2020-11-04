using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//使用DllImport需要这个头文件


namespace GY7501
{
    public class GY7501
    {
        #region Properties

        public const byte ISNK_TX0_MSB_ADDR = 0x05;
        public const byte ISNK_TX0_LSB_ADDR = 0x06;
        public const byte VEQ_TX0_ADDR = 0x01;
        public const byte VCPA_TX0_ADDR = 0x00;
        public const byte MOD_TX0_MSB_ADDR = 0x03;
        public const byte MOD_TX0_LSB_ADDR = 0x04;
        public const byte LDD_TX0_ADDR = 0x07;
        public const byte CONTROL_TX0_ADDR = 0x09;

        public const byte ISNK_TX1_MSB_ADDR = 0x15;
        public const byte ISNK_TX1_LSB_ADDR = 0x16;
        public const byte VEQ_TX1_ADDR = 0x11;
        public const byte VCPA_TX1_ADDR = 0x10;
        public const byte MOD_TX1_MSB_ADDR = 0x13;
        public const byte MOD_TX1_LSB_ADDR = 0x14;
        public const byte LDD_TX1_ADDR = 0x17;

        public const byte CONTROL_TX1_ADDR = 0x19;

        public const byte ISNK_TX2_MSB_ADDR = 0x25;
        public const byte ISNK_TX2_LSB_ADDR = 0x26;
        public const byte VEQ_TX2_ADDR = 0x21;
        public const byte VCPA_TX2_ADDR = 0x20;
        public const byte MOD_TX2_MSB_ADDR = 0x23;
        public const byte MOD_TX2_LSB_ADDR = 0x24;
        public const byte LDD_TX2_ADDR = 0x27;
        public const byte CONTROL_TX2_ADDR = 0x29;

        public const byte ISNK_TX3_MSB_ADDR = 0x35;
        public const byte ISNK_TX3_LSB_ADDR = 0x36;
        public const byte VEQ_TX3_ADDR = 0x31;
        public const byte VCPA_TX3_ADDR = 0x30;
        public const byte MOD_TX3_MSB_ADDR = 0x33;
        public const byte MOD_TX3_LSB_ADDR = 0x34;
        public const byte LDD_TX3_ADDR = 0x37;
        public const byte CONTROL_TX3_ADDR = 0x39;

        public const int DEV_GY7501A = 1; //1ch-I2C
        public const int DEV_GY7512 = 2; //2ch-I2C
        public const int DEV_GY7514 = 3; //4ch-I2C
        public const int DEV_GY7518 = 4; //8ch-I2C
        public const int DEV_GY7503 = 5; //1ch-I2C
        public const int DEV_GY7506 = 6; //1ch-I2C,module/
        public const int DEV_GY7601 = 7; //1ch-I2C
        public const int DEV_GY7602 = 8; //2ch-I2C
        public const int DEV_GY7604 = 9; //4ch-I2C
        public const int DEV_GY7608 = 10;//8ch-I2C

        public const int SLAVE_ADDR = 0xA6;

        [StructLayout(LayoutKind.Sequential)]
        public struct GYI2C_DATA_INFO
        {
            public byte SlaveAddr;//设备物理地址，bit7-1 有效
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 520)]//声明数组大小为520
            public byte[] Databuffer;//Data 报文的数据
            public UInt32 WriteNum;//需要写入的地址（字节）的总个数
            public UInt32 ReadNum;//需要读的字节个数
            public byte IoSel;//1 表示被选择，将被读/写，bit3－0 分别表示4 个IO 口
            public byte IoData;//IO 口状态，bit3－0 分别表示4 个IO 口
                               //只有与IoSel 中为1 的位相同的位值有效
            public UInt32 DlyMsRead; //I2C 读操作时，PC 发出读命令后，延时多少ms 请求读到的数据。
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]//声明数组大小为4
            public byte[] Reserved;
        }

        GYI2C_DATA_INFO ReadBuff;
        GYI2C_DATA_INFO SendBuff;

        byte control_Addr = 0;
        #endregion

        #region Constructor

        public GY7501()
        {
            ReadBuff = new GY7501.GYI2C_DATA_INFO();
            SendBuff = new GY7501.GYI2C_DATA_INFO();

            var ret = GYI2C_Open(DEV_GY7501A, 0, 0);
            ret = GYI2C_SetMode(DEV_GY7501A, 0, 0);
            ret = GYI2C_SetClk(DEV_GY7501A, 0, 100);
        }
       
        #endregion

        #region Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Devtype"></param>
        /// <param name="Devindex"></param>
        /// <param name="Reserved">只有串口转I2C 适配器才会用到该参数。表示串口波特率，值为9600，19200，57600，115200 等</param>
        /// <returns></returns>
        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_Open")]
        public static extern int GYI2C_Open(uint Devtype, uint Devindex, uint Reserved);

        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_Close")]
        public static extern int GYI2C_Close(uint Devtype, uint Devindex, uint Reserved);

        /// <summary>
        /// Set the colock frequency
        /// </summary>
        /// <param name="Devtype"></param>
        /// <param name="Devindex"></param>
        /// <param name="ClkValue">设置当前通道的I2C 时钟频率，单位khz</param>
        /// <returns>1 表示成功，0 表示失败，-1 表示设备未打开</returns>
        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_SetClk")]
        public static extern int GYI2C_SetClk(uint Devtype, uint Devindex, uint ClkValue);

        /// <summary>
        /// set current work mode
        /// </summary>
        /// <param name="Devtype">设备类型，例如DEV_GY7501A</param>
        /// <param name="Devindex">如果为串口转I2C 模块，则输入0 表示串口1，输入1 表示串口2</param>
        /// <param name="ModeValue">0 表示Easy I2C 模式，1 表示Timing I2C 模式。</param>
        /// <returns>1 表示成功，0 表示失败，-1 表示设备未打开</returns>
        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_SetMode")]
        public static extern int GYI2C_SetMode(uint Devtype, uint Devindex, byte ModeValue);

        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_Read")]
        public static extern int GYI2C_Read(uint Devtype, uint Devindex,ref GYI2C_DATA_INFO pDataInfo);

        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_Read2")]
        public static extern int GYI2C_Read2(uint Devtype, uint Devindex, ref byte[] pDataInfo);

        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_Write")]
        public static extern int GYI2C_Write(uint Devtype, uint Devindex,ref GYI2C_DATA_INFO pDataInfo);

        [DllImport("VCI_GYI2C.dll", EntryPoint = "GYI2C_Write2")]
        public static extern int GYI2C_Write2(uint Devtype, uint Devindex, ref byte[] pDataInfo);

        #endregion

        public void DisableTx(int Channel)
        {
            switch (Channel)
            {
                case 1:
                    control_Addr = CONTROL_TX0_ADDR;
                    break;
                case 2:
                    control_Addr = CONTROL_TX1_ADDR;
                    break;
                case 3:
                    control_Addr = CONTROL_TX2_ADDR;
                    break;
                case 4:
                    control_Addr = CONTROL_TX3_ADDR;
                    break;
            }
            List<byte> dataBuff = new List<byte>();
            var valueRead = ReadValue(control_Addr);

            //从寄存器读值，最后一位设为1，其余位保持现有值.
            var valueToWrite = valueRead[0] | 1;
            dataBuff.Add(control_Addr);
            dataBuff.Add(Convert.ToByte(valueToWrite));
            SetValue(dataBuff.ToArray());
        }

        public void EnableTx(int Channel)
        {
            switch (Channel)
            {
                case 1:
                    control_Addr = CONTROL_TX0_ADDR;
                    break;
                case 2:
                    control_Addr = CONTROL_TX1_ADDR;
                    break;
                case 3:
                    control_Addr = CONTROL_TX2_ADDR;
                    break;
                case 4:
                    control_Addr = CONTROL_TX3_ADDR;
                    break;
            }
            List<byte> dataBuff = new List<byte>();
            var valueRead = ReadValue(control_Addr);

            //从寄存器读值，最后一位设为0，其余位保持现有值.
            var valueToWrite = valueRead[0] & ~1;
            dataBuff.Add(control_Addr);
            dataBuff.Add(Convert.ToByte(valueToWrite));
            SetValue(dataBuff.ToArray());
        }

        public void SetIbias(int Channel, double ibias)
        {
            switch (Channel)
            {
                case 1:
                    control_Addr = ISNK_TX0_MSB_ADDR;
                    break;
                case 2:
                    control_Addr = ISNK_TX1_MSB_ADDR;
                    break;
                case 3:
                    control_Addr = ISNK_TX2_MSB_ADDR;
                    break;
                case 4:
                    control_Addr = ISNK_TX3_MSB_ADDR;
                    break;
            }
            List<byte> dataBuff = new List<byte>();

            var Isnk = Math.Round(ibias / 0.07, 0);
            byte[] byteArray = BitConverter.GetBytes((ushort)(Isnk));
            dataBuff.Add(control_Addr);
            dataBuff.Add(byteArray[1]);
            dataBuff.Add(byteArray[0]);
            SetValue(dataBuff.ToArray());
        }

        #region private Methods

        /// <summary>
        /// 向寄存器写入数据
        /// </summary>
        /// <param name="regAddr">寄存器地址</param>
        /// <param name="Value">要写入的值</param>
        /// <param name="writeNum">要写入的字节数，要将寄存器地址所占自己也计算在内</param>
        private void SetValue(byte[] dataBuff)
        {
            SendBuff.SlaveAddr = SLAVE_ADDR;
            SendBuff.Databuffer = new byte[520];

            for (int i = 0; i < dataBuff.Length; i++)
            {
                SendBuff.Databuffer[i] = dataBuff[i];
            }
             SendBuff.WriteNum =(uint) dataBuff.Length;

            SendBuff.ReadNum = 0;
            if (GYI2C_Write(DEV_GY7501A, 0, ref SendBuff) != 1)
            {
                throw new Exception("GYI2C write error!");
            }
        }

        /// <summary>
        /// 从寄存器读取数值
        /// </summary>
        /// <param name="regAddr">寄存器地址</param>
        /// <param name="readByteNum">需要读取的数据的字节数</param>
        /// <returns>读取到的数据组</returns>
        private byte[] ReadValue(byte regAddr,uint readByteNum=1)
        {
            ReadBuff.SlaveAddr = SLAVE_ADDR; ;
            ReadBuff.Databuffer = new byte[520];
            ReadBuff.Databuffer[0] = regAddr;
            ReadBuff.WriteNum = 1;
            ReadBuff.ReadNum = readByteNum;
            ReadBuff.DlyMsRead = 1;

            if (GY7501.GYI2C_Read(GY7501.DEV_GY7501A, 0, ref ReadBuff) == 0)
            {
                throw new Exception("读取数据出错！");
            }
            return ReadBuff.Databuffer;
        }

        #endregion
    }
}