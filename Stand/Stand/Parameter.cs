using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stand
{
    public class Parameter
    {
        private string name;
        public ushort RegisterAddress;
        public string[] UnitsOfMeasure;
        private int SelectedIndex;
        private bool IsConnected;
        public List<float> MeasuredRegs = new List<float> { };

        public Parameter(string name, string[] unitOfMeasure)
        {
            this.name = name;
            UnitsOfMeasure = unitOfMeasure;
        }

        public void LoadXML(XElement el, ref TextBox RegTB, ref ComboBox UofMCB)
        {
            RegisterAddress = Convert.ToUInt16(el.Attribute("Register").Value);
            IsConnected = true;
            SelectedIndex = Convert.ToInt32(el.Attribute("UnitOfMeasure").Value);
            RegTB.Text = RegisterAddress.ToString();
            UofMCB.SelectedIndex = SelectedIndex;
        }
        public void LoadXML(XElement el, ref TextBox RegTB, ref ComboBox UofMCB, ref CheckBox IsConnectedChB)
        {
            RegisterAddress = Convert.ToUInt16(el.Attribute("Register").Value);
            IsConnected = Convert.ToBoolean(el.Attribute("IsConnected").Value);
            SelectedIndex = Convert.ToInt32(el.Attribute("UnitOfMeasure").Value);
            RegTB.Text = RegisterAddress.ToString();
            UofMCB.SelectedIndex = SelectedIndex;
            IsConnectedChB.Checked = IsConnected;
        }

        public void SaveXML(ref TextBox RegTB, ref ComboBox UofMCB)
        {
            SelectedIndex = UofMCB.SelectedIndex;
            RegisterAddress = Convert.ToUInt16(RegTB.Text);
            Form1.xSettings.Elements("parameter").Where(p => p.Attribute("Name").Value == name).Remove();
            XElement xParameter = new XElement("parameter");
            XAttribute xNameAttr = new XAttribute("Name", name);
            XAttribute xConnected = new XAttribute("IsConnected", IsConnected);
            XAttribute xRegister = new XAttribute("Register", RegisterAddress);
            XAttribute xUnitOfMeasure = new XAttribute("UnitOfMeasure", SelectedIndex);
            xParameter.Add(xNameAttr, xConnected, xRegister, xUnitOfMeasure);
            Form1.xSettings.Add(xParameter);
        }
        public void SaveXML(ref TextBox RegTB, ref ComboBox UofMCB, ref CheckBox IsConnectedChB)
        {
            IsConnected = IsConnectedChB.Checked;
            SelectedIndex = UofMCB.SelectedIndex;
            RegisterAddress = Convert.ToUInt16(RegTB.Text);
            Form1.xSettings.Elements("parameter").Where(p => p.Attribute("Name").Value == name).Remove();
            XElement xParameter = new XElement("parameter");
            XAttribute xNameAttr = new XAttribute("Name", name);
            XAttribute xConnected = new XAttribute("IsConnected", IsConnected);
            XAttribute xRegister = new XAttribute("Register", RegisterAddress);
            XAttribute xUnitOfMeasure = new XAttribute("UnitOfMeasure", SelectedIndex);
            xParameter.Add(xNameAttr, xConnected, xRegister, xUnitOfMeasure);
            Form1.xSettings.Add(xParameter);
        }
        public string GetUoMstring()
        {
            return UnitsOfMeasure[SelectedIndex];
        }
        public bool CheckIfToggled()
        {
            return IsConnected;
        }
    }
}
