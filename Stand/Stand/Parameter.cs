using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stand
{
    public enum DataType : int
    {
        DT_Single = 0,
        DT_Float_AB = 1,
        DT_Float_BA = 2,
    };
    public enum RegType : int
    {
        RT_Input = 0,
        RT_Holding = 1,
    };
    public class Parameter
    {
        public string name;
        public ushort RegisterAddress;
        public SortedList<string,float> UnitsOfMeasure = new SortedList<string, float>();
        private int SelectedIndex;
        private bool IsConnected;
        private RegType RegType;
        private DataType DataType;
        public List<float> MeasuredRegs = new List<float> { };

        public Parameter(string name)
        {
            this.name = name;
        }

        public void LoadXML(XElement el, ref TextBox RegTB, ref ComboBox UofMCB, ref ComboBox RegTypeCB, 
            ref ComboBox DTCB)
        {
            RegisterAddress = Convert.ToUInt16(el.Attribute("Register").Value);
            IsConnected = true;
            SelectedIndex = Convert.ToInt32(el.Attribute("UnitOfMeasure").Value);
            this.RegType = (RegType)Convert.ToInt32(el.Attribute("RegType").Value);
            this.DataType = (DataType)Convert.ToInt32(el.Attribute("DataType").Value);
            RegTB.Text = RegisterAddress.ToString();
            RegTypeCB.SelectedIndex = (int)this.RegType;
            DTCB.SelectedIndex = (int)this.DataType;
            UnitsOfMeasure.Clear();
            if (name.Contains("Pressure"))
            {
                using (StreamReader sr = new StreamReader($"{Application.StartupPath}/UoM/Pressure.dat"))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] par = sr.ReadLine().Split('$');
                        UnitsOfMeasure.Add(par[0], float.Parse(par[1]));

                    }
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader($"{Application.StartupPath}/UoM/{name}.dat"))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] par = sr.ReadLine().Split('$');
                        UnitsOfMeasure.Add(par[0], float.Parse(par[1]));

                    }
                }
            }
            UofMCB.Items.Clear();
            UofMCB.Items.AddRange(UnitsOfMeasure.Keys.ToArray());
            UofMCB.SelectedIndex = SelectedIndex;
        }
        public void LoadXML(XElement el, ref TextBox RegTB, ref ComboBox UofMCB, ref ComboBox RegTypeCB, 
            ref ComboBox DTCB, ref CheckBox IsConnectedChB)
        {
            RegisterAddress = Convert.ToUInt16(el.Attribute("Register").Value);
            IsConnected = Convert.ToBoolean(el.Attribute("IsConnected").Value);
            SelectedIndex = Convert.ToInt32(el.Attribute("UnitOfMeasure").Value);
            this.RegType = (RegType)Convert.ToInt32(el.Attribute("RegType").Value);
            this.DataType = (DataType)Convert.ToInt32(el.Attribute("DataType").Value);
            RegTB.Text = RegisterAddress.ToString();
            IsConnectedChB.Checked = IsConnected;
            RegTypeCB.SelectedIndex = (int)this.RegType;
            DTCB.SelectedIndex = (int)this.DataType;
            UnitsOfMeasure.Clear();
            if (name.Contains("Pressure"))
            {
                using (StreamReader sr = new StreamReader($"{Application.StartupPath}/UoM/Pressure.dat"))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] par = sr.ReadLine().Split('$');
                        UnitsOfMeasure.Add(par[0], float.Parse(par[1]));

                    }
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader($"{Application.StartupPath}/UoM/{name}.dat"))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] par = sr.ReadLine().Split('$');
                        UnitsOfMeasure.Add(par[0], float.Parse(par[1]));

                    }
                }
            }
            UofMCB.Items.Clear();
            UofMCB.Items.AddRange(UnitsOfMeasure.Keys.ToArray());
            UofMCB.SelectedIndex = SelectedIndex;
        }

        public void SaveXML(ref TextBox RegTB, ref ComboBox UofMCB, ref ComboBox RegTypeCB, ref ComboBox DTCB)
        {
            SelectedIndex = UofMCB.SelectedIndex;
            RegisterAddress = Convert.ToUInt16(RegTB.Text);
            this.RegType = (RegType)Convert.ToUInt16(RegTypeCB.SelectedIndex);
            this.DataType = (DataType)Convert.ToUInt16(DTCB.SelectedIndex);
            Form1.xSettings.Elements("parameter").Where(p => p.Attribute("Name").Value == name).Remove();
            XElement xParameter = new XElement("parameter");
            XAttribute xNameAttr = new XAttribute("Name", name);
            XAttribute xConnected = new XAttribute("IsConnected", IsConnected);
            XAttribute xRegister = new XAttribute("Register", RegisterAddress);
            XAttribute xRegType = new XAttribute("RegType", (int)this.RegType);
            XAttribute xDataType = new XAttribute("DataType", (int)this.DataType);
            XAttribute xUnitOfMeasure = new XAttribute("UnitOfMeasure", SelectedIndex);
            xParameter.Add(xNameAttr, xConnected, xRegister, xRegType, xDataType, xUnitOfMeasure);
            Form1.xSettings.Add(xParameter);
            if (name.Contains("Pressure"))
            {
                using (StreamWriter sw = new StreamWriter($"{Application.StartupPath}/UoM/Pressure.dat"))
                {
                    foreach (var UoM in UnitsOfMeasure)
                    {
                        sw.WriteLine(UoM.Key + "$" + UoM.Value);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter($"{Application.StartupPath}/UoM/{name}.dat"))
                {
                    foreach (var UoM in UnitsOfMeasure)
                    {
                        sw.WriteLine(UoM.Key + "$" + UoM.Value);
                    }
                }
            }
        }
        public void SaveXML(ref TextBox RegTB, ref ComboBox UofMCB, ref ComboBox RegTypeCB, 
            ref ComboBox DTCB, ref CheckBox IsConnectedChB)
        {
            IsConnected = IsConnectedChB.Checked;
            SelectedIndex = UofMCB.SelectedIndex;
            RegisterAddress = Convert.ToUInt16(RegTB.Text);
            this.RegType = (RegType)Convert.ToUInt16(RegTypeCB.SelectedIndex);
            this.DataType = (DataType)Convert.ToUInt16(DTCB.SelectedIndex);
            Form1.xSettings.Elements("parameter").Where(p => p.Attribute("Name").Value == name).Remove();
            XElement xParameter = new XElement("parameter");
            XAttribute xNameAttr = new XAttribute("Name", name);
            XAttribute xConnected = new XAttribute("IsConnected", IsConnected);
            XAttribute xRegister = new XAttribute("Register", RegisterAddress);
            XAttribute xRegType = new XAttribute("RegType", (int)this.RegType);
            XAttribute xDataType = new XAttribute("DataType", (int)this.DataType);
            XAttribute xUnitOfMeasure = new XAttribute("UnitOfMeasure", SelectedIndex);
            xParameter.Add(xNameAttr, xConnected, xRegister, xRegType, xDataType, xUnitOfMeasure);
            Form1.xSettings.Add(xParameter);
            if (name.Contains("Pressure"))
            {
                using (StreamWriter sw = new StreamWriter($"{Application.StartupPath}/UoM/Pressure.dat"))
                {
                    foreach (var UoM in UnitsOfMeasure)
                    {
                        sw.WriteLine(UoM.Key + "$" + UoM.Value);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter($"{Application.StartupPath}/UoM/{name}.dat"))
                {
                    foreach (var UoM in UnitsOfMeasure)
                    {
                        sw.WriteLine(UoM.Key + "$" + UoM.Value);
                    }
                }
            }
        }
        public string GetUoMstring()
        {
            return UnitsOfMeasure.ElementAt(SelectedIndex).Key;
        }
        public bool CheckIfToggled()
        {
            return IsConnected;
        }
        public RegType GetRegType()
        {
            return RegType;
        }
        public DataType GetDataType()
        {
            return DataType;
        }
        public void SetMeasuredRegs(float reg)
        {
            MeasuredRegs.Add(reg * UnitsOfMeasure.Values[SelectedIndex]);
        }
    }
}
