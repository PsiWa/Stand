using Modbus.Device;
using Modbus.Extensions.Enron;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stand
{
    [Serializable]
    public class Unit
    {
        private string name;
        public int[] address = new int[1];
        public bool isConnected=false;
        private SerialPort COMport = new SerialPort();
        private ModbusSerialMaster masterCOM;
        static private ManualResetEvent mreScan= new ManualResetEvent(false);

        public Unit(string name)
        {
            this.name = name;
            this.SetDeffaultAdditionalSettings();
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetParameters(string COM, int BaudRate, int address, int offset)
        {
            COMport.PortName = COM;
            COMport.BaudRate = BaudRate;
            Array.Resize(ref this.address, offset);
            this.address[0] = address;
            for (int i = 1; i < offset; i++)
                this.address[i] = this.address[0] + i;
        }

        public void SetParameters(int DataBits, Parity Parity, StopBits StopBits, bool DtrEnable, 
            bool RtsEnable, int ReadTimeout, int WriteTimeout, Handshake Handshake)
        {
            COMport.DataBits = DataBits;
            COMport.Parity = Parity;
            COMport.StopBits = StopBits;
            COMport.DtrEnable = DtrEnable;
            COMport.RtsEnable = RtsEnable;
            COMport.ReadTimeout = ReadTimeout;
            COMport.WriteTimeout = WriteTimeout;
            COMport.Handshake = Handshake;
        }

        public void SetParameters(string COM, int BaudRate, int address, int offset, int DataBits, 
            Parity Parity, StopBits StopBits, bool DtrEnable, bool RtsEnable, int ReadTimeout, int WriteTimeout, Handshake Handshake)
        {
            COMport.PortName = COM;
            COMport.BaudRate= BaudRate;
            Array.Resize(ref this.address, offset);
            this.address[0] = address;
            for (int i = 1; i < offset; i++)
                this.address[i] = this.address[0] + i;
            COMport.DataBits = DataBits;
            COMport.Parity = Parity;
            COMport.StopBits = StopBits;
            COMport.DtrEnable = DtrEnable;
            COMport.RtsEnable = RtsEnable;
            COMport.ReadTimeout = ReadTimeout;
            COMport.WriteTimeout = WriteTimeout;
            COMport.Handshake = Handshake;
        }

        public void SaveSettingsXML()
        {
            Form1.xSettings.Elements("unit").Where(p => p.Attribute("Name").Value == this.name).Remove();
            XElement xUnit = new XElement("unit");
            XAttribute xNameAttr = new XAttribute("Name", name);
            XAttribute xCOMAttr = new XAttribute("COM", COMport.PortName);
            XAttribute xSpeedAttr = new XAttribute("BaudRate", COMport.BaudRate);
            XAttribute xAddrAttr = new XAttribute("Address", this.address[0]);
            XAttribute xOffsetAttr = new XAttribute("Offset", this.address.Length);
            XAttribute xDataBitsAttr = new XAttribute("DataBits", COMport.DataBits);
            XAttribute xParityAttr = new XAttribute("Parity",COMport.Parity);
            XAttribute xStopBitsAttr = new XAttribute("StopBits", COMport.StopBits);
            XAttribute xDtrEnableAttr = new XAttribute("DtrEnable", COMport.DtrEnable);
            XAttribute xRtsEnableAttr = new XAttribute("RtsEnable", COMport.RtsEnable);
            XAttribute xReadTimeoutAttr = new XAttribute("ReadTimeout", COMport.ReadTimeout);
            XAttribute xWriteTimeoutAttr = new XAttribute("WriteTimeout", COMport.WriteTimeout);
            XAttribute xHandshakeAttr = new XAttribute("Handshake", COMport.Handshake);
            xUnit.Add(xNameAttr, xCOMAttr, xSpeedAttr, xAddrAttr, xOffsetAttr, xDataBitsAttr, xParityAttr, xStopBitsAttr, 
                xDtrEnableAttr, xRtsEnableAttr, xReadTimeoutAttr, xWriteTimeoutAttr, xHandshakeAttr);
            Form1.xSettings.Add(xUnit);
            Form1.xSettings.Save("Defaults.xml");
        }

        public void LoadSettingsXML(XElement el)
        {
            int offset = 0;
            try
            {
                COMport.PortName = el.Attribute("COM").Value;
            }
            catch (Exception)
            {
                MessageBox.Show($"Устройству {this.name} не назначен COM порт, установлено значение по умолчанию");
            }

            try
            {
                address[0] = Convert.ToInt32(el.Attribute("Address").Value);
                offset = Convert.ToInt32(el.Attribute("Offset").Value);
                Array.Resize(ref this.address, offset);
                for (int i = 1; i < offset; i++)
                    this.address[i] = this.address[0] + i;
                COMport.BaudRate = Convert.ToInt32(el.Attribute("BaudRate").Value);
            }
            catch (Exception)
            {
                MessageBox.Show($"Устройству {this.name} не назначена скорость или адресс, установлены значения по умолчанию");
            }
            try
            {
                COMport.DataBits = Convert.ToInt32(el.Attribute("DataBits").Value);
                COMport.Parity = (Parity)Enum.Parse(typeof(Parity), el.Attribute("Parity").Value);
                COMport.StopBits = (StopBits)Enum.Parse(typeof(StopBits), el.Attribute("StopBits").Value);
                COMport.DtrEnable = Convert.ToBoolean(el.Attribute("DtrEnable").Value);
                COMport.RtsEnable = Convert.ToBoolean(el.Attribute("RtsEnable").Value);
                COMport.ReadTimeout = Convert.ToInt32(el.Attribute("ReadTimeout").Value);
                COMport.WriteTimeout = Convert.ToInt32(el.Attribute("WriteTimeout").Value);
                COMport.Handshake = (Handshake)Enum.Parse(typeof(Handshake), el.Attribute("Handshake").Value);
            }
            catch (Exception)
            {
                this.SetDeffaultAdditionalSettings();
            }
            }

        public void SetDeffaultAdditionalSettings()
        {
            COMport.DataBits = 8;
            COMport.Parity = Parity.Odd;
            COMport.StopBits = StopBits.One;
            COMport.DtrEnable = true;
            COMport.RtsEnable = true;
            COMport.ReadTimeout = 2000;
            COMport.WriteTimeout = 2000;
            COMport.Handshake = Handshake.None;
        }

        public void GetParametersString(ref string COMname, ref string BaudRate, ref string address, ref string offset)
        {
            COMname = COMport.PortName;
            BaudRate = COMport.BaudRate.ToString();
            address = this.address[0].ToString();
            offset = this.address.Length.ToString();
        }
        public void GetParametersString(ref string DataBits,ref string Parity,ref string StopBits,ref string DtrEnable,
             ref string RtsEnable,ref string ReadTimeout,ref string WriteTimeout,ref string Handshake)
        {
            DataBits = COMport.DataBits.ToString();
            Parity = COMport.Parity.ToString();
            StopBits = COMport.StopBits.ToString();
            DtrEnable = COMport.DtrEnable.ToString();
            RtsEnable = COMport.RtsEnable.ToString();
            ReadTimeout = COMport.ReadTimeout.ToString();
            WriteTimeout = COMport.WriteTimeout.ToString();
            Handshake = COMport.Handshake.ToString();
        }

        public bool TryToConnect()
        {
            try
            {
                COMport.Open();
                masterCOM = ModbusSerialMaster.CreateRtu(COMport);
                isConnected = true;
                return isConnected;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка подключения");
                COMport.Close();
                isConnected= false;
                return isConnected;
            }
        }

        public void Disconnect()
        {
            COMport.Close();
            isConnected = false;
        }

        public float[] ComReadHolding(ushort startAddress, ushort nOfPoints, bool reversFlag)
        {
            float[] fRegs = new float[this.address.Length];
            for (int i = 0; i<address.Length;i++)
            {
                byte _slaveStationAddr = Convert.ToByte(this.address[i]);
                ushort[] usRegs = null;
                try
                {
                    mreScan.Reset();
                    usRegs = masterCOM.ReadHoldingRegisters(_slaveStationAddr, startAddress, nOfPoints);
                    mreScan.Set();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка чтения регистра");
                    fRegs[i] = 0;
                    break;
                }
                if (usRegs.Length == 1)
                    fRegs[i] = usRegs[0];
                else if (usRegs.Length == 2)
                {
                    if (reversFlag)
                        fRegs[i] = GetFloatFromRegs(usRegs[1], usRegs[0]);
                    else
                        fRegs[i] = GetFloatFromRegs(usRegs[0], usRegs[1]);
                }
                else
                {
                    MessageBox.Show("Пока читаем максимум по 2"); // это надо исправить
                    fRegs[i] = 0;
                }
            }
            return fRegs;
        }

        public float[] ComReadInput(ushort startAddress, ushort nOfPoints, bool reversFlag)
        {
            float[] fRegs = new float[this.address.Length];
            for (int i = 0; i < address.Length; i++)
            {
                byte _slaveStationAddr = Convert.ToByte(this.address[i]);
                ushort[] usRegs = null;
                try
                {
                    mreScan.Reset();
                    usRegs = masterCOM.ReadInputRegisters(_slaveStationAddr, startAddress, nOfPoints);
                    mreScan.Set();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка чтения регистра");
                    fRegs[i] = 0;
                    break;
                }
                if (usRegs.Length == 1)
                    fRegs[i] = usRegs[0];
                else if (usRegs.Length == 2)
                {
                    if (reversFlag)
                        fRegs[0] = GetFloatFromRegs(usRegs[1], usRegs[0]);
                    else
                        fRegs[i] = GetFloatFromRegs(usRegs[0], usRegs[1]);
                }
                else
                {
                    MessageBox.Show("Пока читаем максимум по 2"); // это надо исправить
                    fRegs[i] = 0;
                }
            }
            return fRegs;
        }

        static private Single GetFloatFromRegs(ushort _reg1, ushort _reg0)
        {
            try
            {
                List<byte> lstFloat = new List<byte>();
                byte[] bytes0 = BitConverter.GetBytes(_reg0);
                byte[] bytes1 = BitConverter.GetBytes(_reg1);
                lstFloat.AddRange(bytes0);
                lstFloat.AddRange(bytes1);
                Single _float = BitConverter.ToSingle(lstFloat.ToArray(), 0);
                return _float;
            }
            catch (Exception)
            {
                return Single.NaN;
            }
        }
        /// Test Methods

        public string TestComRead()
        {
            byte _slaveStationAddr = Convert.ToByte(this.address);
            ushort[] usRegs = null;
            mreScan.Reset();
            usRegs = masterCOM.ReadHoldingRegisters(_slaveStationAddr, 16299, 2);
            mreScan.Set();
            return (usRegs[0]).ToString();//GetFloatFromRegs(usRegs[1], usRegs[0]).ToString();

        }

        public int GetBaud()
        {
            return COMport.BaudRate;
        }
    }
}
