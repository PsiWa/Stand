using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
        public bool isConnected;
        private SerialPort COMport = new SerialPort();

        public Unit(string name)
        {
            this.name = name;
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

        public void SetParameters(string COM, int BaudRate, int address,int DataBits, Parity Parity, StopBits StopBits, bool DtrEnable,
            bool RtsEnable, int ReadTimeout, int WriteTimeout, Handshake Handshake)
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

        public void SaveSettingsXML(ref XElement xSettings)
        {
            xSettings.Elements("unit").Where(p => p.Attribute("Name").Value == this.name).Remove();
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
            xUnit.Add(xNameAttr,xCOMAttr,xSpeedAttr,xAddrAttr, xDataBitsAttr,xParityAttr, xStopBitsAttr,xDtrEnableAttr,xRtsEnableAttr,
                xReadTimeoutAttr,xWriteTimeoutAttr,xHandshakeAttr);
            xSettings.Add(xUnit);
            xSettings.Save("Defaults.xml");
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
    }
}
