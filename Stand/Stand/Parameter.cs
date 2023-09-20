using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;

namespace Stand
{
    public enum DataType : int
    {
        DT_Int = 0,
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
        private static int ParameterNum=1;
        public int id;
        private string name;
        private string ParentUnitName;
        private ushort RegisterAddress;
        private ushort RegisterOffset;
        public SortedList<string,float> UnitsOfMeasure = new SortedList<string, float>();
        private int SelectedIndex;
        private bool IsConnected;
        private RegType RegType;
        private DataType DataType;
        private List<float> MeasuredRegs = new List<float>() { };
        public List<bool> IsMeasureSetList = new List<bool>() { };

        #region Constructors
        public Parameter(XElement el, int ParentID)
        {
            LoadXML(el, ParentID);
        }
        public Parameter(string ParentUnitName, int ParentID)
        {
            this.id = ParameterNum + 1000*ParentID;
            this.ParentUnitName= ParentUnitName;
            this.name = $"Параметр {ParameterNum++}";
            UnitsOfMeasure.Add("ед. изм.", 1);
            SelectedIndex = 0;
        }
        #endregion
        #region Save/Load
        //рудимент
        public void LoadXML(XElement el, ref TextBox RegTB, ref ComboBox UofMCB, ref ComboBox RegTypeCB, 
            ref ComboBox DTCB)
        {
            name = el.Attribute("Name").Value;
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
        //рудимент
        public void LoadXML(XElement el, ref TextBox RegTB, ref ComboBox UofMCB, ref ComboBox RegTypeCB, 
            ref ComboBox DTCB, ref CheckBox IsConnectedChB)
        {
            name = el.Attribute("Name").Value;
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
            if (name.Contains("Pressure") || name.Contains("Давление"))
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

        public void LoadXML(XElement el, int ParentID=1)
        {
            try
            {
                this.id = Convert.ToInt32(el.Attribute("Id").Value);
                ParameterNum++;
            }
            catch (Exception)
            {
                this.id = ParentID*1000+ParameterNum++;
            }
            name = el.Attribute("Name").Value;
            RegisterAddress = Convert.ToUInt16(el.Attribute("Register").Value);
            RegisterOffset = Convert.ToUInt16(el.Attribute("Offset").Value);
            IsConnected = Convert.ToBoolean(el.Attribute("IsConnected").Value);
            SelectedIndex = Convert.ToInt32(el.Attribute("UnitOfMeasure").Value);
            this.RegType = (RegType)Convert.ToInt32(el.Attribute("RegType").Value);
            this.DataType = (DataType)Convert.ToInt32(el.Attribute("DataType").Value);
            ParentUnitName = el.Attribute("UnitName").Value;
            UnitsOfMeasure.Clear();
            try
            {
                using (StreamReader sr = new StreamReader($"{Application.StartupPath}/UoM/{ParentUnitName}/{name}.dat"))
                {
                    while (!sr.EndOfStream)
                    {
                        string[] par = sr.ReadLine().Split('$');
                        UnitsOfMeasure.Add(par[0], float.Parse(par[1]));
                    }
                }
            }
            catch (Exception)
            {
                UnitsOfMeasure.Add("ед. изм.", 1);
                SelectedIndex = 0;
            }
        }

        //рудимент
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
        //рудимент
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

        public void SaveXML()
        {
            Form1.xSettings.Elements("parameter").Where(p => p.Attribute("Name").Value == name).Remove();
            XAttribute xIdAttr = new XAttribute("Id", id);
            XElement xParameter = new XElement("parameter");
            XAttribute xNameAttr = new XAttribute("Name", name);
            XAttribute xConnected = new XAttribute("IsConnected", IsConnected);
            XAttribute xRegister = new XAttribute("Register", RegisterAddress);
            XAttribute xOffset = new XAttribute("Offset", RegisterOffset);
            XAttribute xRegType = new XAttribute("RegType", (int)this.RegType);
            XAttribute xDataType = new XAttribute("DataType", (int)this.DataType);
            XAttribute xUnitOfMeasure = new XAttribute("UnitOfMeasure", SelectedIndex);
            XAttribute xUnitName = new XAttribute("UnitName", ParentUnitName);
            xParameter.Add(xIdAttr ,xNameAttr, xConnected, xRegister, xOffset, xRegType, xDataType, xUnitOfMeasure, xUnitName);
            Form1.xSettings.Add(xParameter);
            bool exists = Directory.Exists($"{Application.StartupPath}/UoM/{ParentUnitName}");
            if (!exists)
                System.IO.Directory.CreateDirectory($"{Application.StartupPath}/UoM/{ParentUnitName}");
            using (StreamWriter sw = new StreamWriter($"{Application.StartupPath}/UoM/{ParentUnitName}/{name}.dat"))
            {
                foreach (var UoM in UnitsOfMeasure)
                {
                    sw.WriteLine(UoM.Key + "$" + UoM.Value);
                }
            }
        }
        #endregion
        #region Get/Set params
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
        public string GetName()
        {
            return name;
        }
        public ushort GetRegisterAddress()
        {
            return RegisterAddress;
        }
        public ushort GetOffset()
        {
            return RegisterOffset;
        }
        public SortedList<string, float> GetUoMs()
        {
            return UnitsOfMeasure;
        }
        public int GetSelectedUoM()
        {
            return SelectedIndex;
        }
        public void SetParameters(string name, ushort RegisterAddress, ushort RegisterOffset, int SelectedIndex, 
            bool IsConnected, RegType RegType, DataType DataType)
        {
            if (name != this.name)
            {
                Form1.xSettings.Elements("parameter").Where(p => p.Attribute("Name").Value == this.name).Remove();
                if (File.Exists($"{Application.StartupPath}/UoM/{ParentUnitName}/{this.name}.dat"))
                    File.Delete($"{Application.StartupPath}/UoM/{ParentUnitName}/{this.name}.dat");
            }
            this.name = name;
            this.RegisterAddress = RegisterAddress;
            this.RegisterOffset = RegisterOffset;
            this.SelectedIndex = SelectedIndex;
            this.IsConnected = IsConnected;
            this.RegType = RegType;
            this.DataType = DataType;
        }
        public void SetParentName(string ParentName)
        {
            ParentUnitName = ParentName;
        }
        public void ClearMeasuredRegs()
        {
            MeasuredRegs.Clear();
        }
        public float GetLastMeasuredRegs()
        {
            return MeasuredRegs.Last();
        }
        public List<float> GetAllMeasuredRegs()
        {
            return MeasuredRegs;
        }
    }
        #endregion
}

