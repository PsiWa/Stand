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
        private int address;
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

        public void SetParameters(string COM, int BaudRate, int address)
        {
            COMport.PortName = COM;
            COMport.BaudRate = BaudRate;
            this.address = address;
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

        public void SetParameters(string COM, int BaudRate, int address, int DataBits, Parity Parity, 
            StopBits StopBits, bool DtrEnable, bool RtsEnable, int ReadTimeout, int WriteTimeout, Handshake Handshake)
        {
            COMport.PortName = COM;
            COMport.BaudRate= BaudRate;
            this.address = address;
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
            XAttribute xAddrAttr = new XAttribute("Address", this.address);
            XAttribute xDataBitsAttr = new XAttribute("DataBits", COMport.DataBits);
            XAttribute xParityAttr = new XAttribute("Parity",COMport.Parity);
            XAttribute xStopBitsAttr = new XAttribute("StopBits", COMport.StopBits);
            XAttribute xDtrEnableAttr = new XAttribute("DtrEnable", COMport.DtrEnable);
            XAttribute xRtsEnableAttr = new XAttribute("RtsEnable", COMport.RtsEnable);
            XAttribute xReadTimeoutAttr = new XAttribute("ReadTimeout", COMport.ReadTimeout);
            XAttribute xWriteTimeoutAttr = new XAttribute("WriteTimeout", COMport.WriteTimeout);
            XAttribute xHandshakeAttr = new XAttribute("Handshake", COMport.Handshake);
            xUnit.Add(xNameAttr, xCOMAttr, xSpeedAttr, xAddrAttr, xDataBitsAttr, xParityAttr, xStopBitsAttr, 
                xDtrEnableAttr, xRtsEnableAttr, xReadTimeoutAttr, xWriteTimeoutAttr, xHandshakeAttr);
            Form1.xSettings.Add(xUnit);
            Form1.xSettings.Save("Defaults.xml");
        }

        public void LoadSettingsXML(XElement el, ref ComboBox ComCB, ref TextBox BaudRateTB, ref TextBox AddressTB)
        {
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
                address = Convert.ToInt32(el.Attribute("Address").Value);
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
            ComCB.SelectedIndex = ComCB.FindStringExact(COMport.PortName);
            BaudRateTB.Text = this.COMport.BaudRate.ToString();
            AddressTB.Text = this.address.ToString();
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
            address = this.address.ToString();
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

        public float ComReadHolding(ref Parameter par, bool reversFlag, int offset)
        {
            float fRegs;
            byte _slaveStationAddr = Convert.ToByte(this.address);
            ushort[] usRegs = null;
            try
            {
                mreScan.Reset();
                usRegs = masterCOM.ReadHoldingRegisters(_slaveStationAddr, par.RegisterAddress, 2);
                mreScan.Set();
            }
            catch (Exception)
            {
                //MessageBox.Show("Ошибка чтения регистра");
                return 0;
            }
            if (offset == 0)
            {
                if (reversFlag)
                    fRegs = GetFloatFromRegs(usRegs[1], usRegs[0]);
                else
                    fRegs = GetFloatFromRegs(usRegs[0], usRegs[1]);
            }
            else
                fRegs = Convert.ToInt32(usRegs[offset]);
            par.MeasuredRegs.Add(fRegs);
            return par.MeasuredRegs.Last();
        }

        public float ComReadInput(ref Parameter par, bool reversFlag, int offset)
        {
            float fRegs;
            byte _slaveStationAddr = Convert.ToByte(this.address);
            ushort[] usRegs = null;
            try
            {
                mreScan.Reset();
                usRegs = masterCOM.ReadInputRegisters(_slaveStationAddr, par.RegisterAddress, 2);
                mreScan.Set();
            }
            catch (Exception)
            {
                //MessageBox.Show("Ошибка чтения регистра");
                return 0;
            }
            if (offset == 0)
            {
                if (reversFlag)
                    fRegs = GetFloatFromRegs(usRegs[1], usRegs[0]);
                else
                    fRegs = GetFloatFromRegs(usRegs[0], usRegs[1]);
            }
            else
                fRegs = Convert.ToInt32(usRegs[offset]);
            par.MeasuredRegs.Add(fRegs);
            return par.MeasuredRegs.Last();
        }

        static float test = 0;
        public float ComRead(ref Parameter par, ushort offset)
        {
            float fRegs = 0;
            byte _slaveStationAddr = Convert.ToByte(this.address);
            ushort[] usRegs = null;
            if (isConnected) //убрать
            {
                if (par.GetDataType() == DataType.DT_Single)
                {
                    if (par.GetRegType() == RegType.RT_Input)
                    {
                        mreScan.Reset();
                        usRegs = masterCOM.ReadInputRegisters(_slaveStationAddr,
                            (ushort)(par.RegisterAddress + offset), 1);
                        mreScan.Set();
                    }
                    else if (par.GetRegType() == RegType.RT_Holding)
                    {
                        mreScan.Reset();
                        usRegs = masterCOM.ReadHoldingRegisters(_slaveStationAddr,
                            (ushort)(par.RegisterAddress + offset), 1);
                        mreScan.Set();
                    }
                    fRegs = Convert.ToInt32(usRegs[0]);
                }
                else
                {
                    if (par.GetRegType() == RegType.RT_Input)
                    {
                        mreScan.Reset();
                        usRegs = masterCOM.ReadInputRegisters(_slaveStationAddr,
                            (ushort)(par.RegisterAddress + offset), 2);
                        mreScan.Set();
                    }
                    else if (par.GetRegType() == RegType.RT_Holding)
                    {
                        mreScan.Reset();
                        usRegs = masterCOM.ReadHoldingRegisters(_slaveStationAddr,
                            (ushort)(par.RegisterAddress + offset), 2);
                        mreScan.Set();
                    }
                    if (par.GetDataType() == DataType.DT_Float_AB)
                        fRegs = GetFloatFromRegs(usRegs[0], usRegs[1]);
                    if (par.GetDataType() == DataType.DT_Float_BA)
                        fRegs = GetFloatFromRegs(usRegs[0], usRegs[1]);
                }
            }
            else
            {
                test += (float)0.1;
                fRegs = test;
            }
            //par.SetMeasuredRegs(fRegs);
            return fRegs;//par.MeasuredRegs.Last();
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
