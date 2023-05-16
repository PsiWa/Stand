using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Contexts;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;
using Application = System.Windows.Forms.Application;
using Spire.Xls;

namespace Stand
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static ManualResetEvent _stopper = new ManualResetEvent(false);
        private bool stopthread = false;

        private XDocument xDefaultDoc = new XDocument();
        public static XElement xSettings = new XElement("settings");
        private bool IsRegistersAdditionalToggled = false;

        #region Инициализация устройств и параметров
        public List<Unit> UnitsList = new List<Unit>();
        public List<Scheme> SchemasList = new List<Scheme>();
        public Scheme SelectedScheme;

        public double SiPressureCoefficient = 1;
        public double SiFlowCoefficient = 1;
        public double SiPowerCoefficient = 1;

        //static public Unit FlowMeter = new Unit("Flowmeter");
        //static public Unit FrequencyChanger = new Unit("FrequencyChanger");
        //static public Unit PressureGauges = new Unit("PressureGauges");
        //static public Unit Valve = new Unit("Valve");
        //static public Unit Vibration = new Unit("Vibration");


        /*static private Parameter Flow = new Parameter("Flow", new string[] { "л/ч", "л/сут", "м3/час", "м3/сут" });
        static private Parameter Current = new Parameter("Current", new string[] { "А" });
        static private Parameter CurrentFrequency = new Parameter("CurrentFrequency", new string[] { "Гц" });
        static private Parameter Voltage = new Parameter("Voltage", new string[] { "В" });
        static private Parameter Torque = new Parameter("Torque", new string[] { "Н*м" });
        static private Parameter Power = new Parameter("Power", new string[] { "Вт", "кВт" });
        static private Parameter RPM = new Parameter("RPM", new string[] { "об/мин" });
        static private Parameter EngineLoad = new Parameter("EngineLoad", new string[] { "%" });
        static private Parameter XAmplitude = new Parameter("XAmplitude", new string[] { "мм/с" });
        static private Parameter YAmplitude = new Parameter("YAmplitude", new string[] { "мм/с" });
        static private Parameter Pressure1 = new Parameter("Pressure1", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure2 = new Parameter("Pressure2", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure3 = new Parameter("Pressure3", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure4 = new Parameter("Pressure4", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure5 = new Parameter("Pressure5", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure6 = new Parameter("Pressure6", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure7 = new Parameter("Pressure7", new string[] { "Па", "кПа", "МПа", "атм" });
        static private Parameter Pressure8 = new Parameter("Pressure8", new string[] { "Па", "кПа", "МПа", "атм" });*/

        /*static private Parameter Flow = new Parameter("Flow");
        static private Parameter Current = new Parameter("Current");
        static private Parameter CurrentFrequency = new Parameter("CurrentFrequency");
        static private Parameter Voltage = new Parameter("Voltage");
        static private Parameter Torque = new Parameter("Torque");
        static private Parameter Power = new Parameter("Power");
        static private Parameter RPM = new Parameter("RPM");
        static private Parameter EngineLoad = new Parameter("EngineLoad");
        static private Parameter XAmplitude = new Parameter("XAmplitude");
        static private Parameter YAmplitude = new Parameter("YAmplitude");
        static private Parameter Pressure1 = new Parameter("Pressure1");
        static private Parameter Pressure2 = new Parameter("Pressure2");
        static private Parameter Pressure3 = new Parameter("Pressure3");
        static private Parameter Pressure4 = new Parameter("Pressure4");
        static private Parameter Pressure5 = new Parameter("Pressure5");
        static private Parameter Pressure6 = new Parameter("Pressure6");
        static private Parameter Pressure7 = new Parameter("Pressure7");
        static private Parameter Pressure8 = new Parameter("Pressure8");

        private SortedList<string, Parameter> ParameterDictionary = new SortedList<string, Parameter>()
        {
            {Flow.GetName(), Flow},
            {Current.GetName(), Current},
            {CurrentFrequency.GetName(), CurrentFrequency },
            {Voltage.GetName(), Voltage},
            {Torque.GetName(), Torque},
            {Power.GetName(), Power},
            {RPM.GetName(), RPM},
            {EngineLoad.GetName(), EngineLoad},
            {XAmplitude.GetName(), XAmplitude},
            {YAmplitude.GetName(), YAmplitude},
            {Pressure1.GetName(), Pressure1},
            {Pressure2.GetName(), Pressure2},
            {Pressure3.GetName(), Pressure3},
            {Pressure4.GetName(), Pressure4},
            {Pressure5.GetName(), Pressure5},
            {Pressure6.GetName(), Pressure6},
            {Pressure7.GetName(), Pressure7},
            {Pressure8.GetName(), Pressure8}
        };*/

        #endregion
        #region Utils
        private void UpdateUnitsList(int selected)
        {
            UnitsListBox.Items.Clear();
            foreach (Unit un in UnitsList)
            {
                if (un.isConnected)
                    UnitsListBox.Items.Add($"{un.GetName()} (подключено)");
                else
                    UnitsListBox.Items.Add($"{un.GetName()} (отключено)");
            }
            UnitsListBox.SelectedIndex = selected;
            UpdateSelectedUnit();
        }
        private void UpdateSelectedUnit()
        {
            Unit un = UnitsList[UnitsListBox.SelectedIndex];
            UnitNameTextBox.Text = un.GetName();
            UnitComComboBox.SelectedIndex = UnitComComboBox.FindString(un.GetComPortName());
            UnitBaudRateTextBox.Text = un.GetBaud().ToString();
            UnitAddressTextBox.Text = un.GetAddress().ToString();
            if (un.isConnected)
            {
                UnitStatusTextBox.BackColor = Color.Green;
                UnitConnectButton.Text = "Отключиться";
            }
            else
            {
                UnitStatusTextBox.BackColor = Color.Red;
                UnitConnectButton.Text = "Подключиться";
            }
            UIDLabel.Text = "-";
            UIDLabel.Text = un.id.ToString();

            ParametersListBox.Items.Clear();
            ParameterNameTextBox.Clear();
            ParameterUoMComboBox.Items.Clear();
            ParameterRegAddressTextBox.Clear();
            ParameterReadableCheckBox.Checked = false;
            PIDLabel.Text = "----";
            foreach (Parameter par in un.GetParametersList())
            {
                ParametersListBox.Items.Add(par.GetName());
                ParametersListBox.SelectedIndex = 0;
            }
        }
        public void UpdateSelectedParameter()
        {
            Unit un = UnitsList[UnitsListBox.SelectedIndex];
            Parameter par = un.GetParametersList()[ParametersListBox.SelectedIndex];
            ParameterNameTextBox.Text = par.GetName();
            ParameterRegAddressTextBox.Text = par.GetRegisterAddress().ToString();
            ParameterRegTypeComboBox.SelectedIndex = (int)par.GetRegType();
            ParameterTypeComboBox.SelectedIndex = (int)par.GetDataType();
            ParameterReadableCheckBox.Checked = par.CheckIfToggled();
            ParameterUoMComboBox.Items.Clear();
            ParameterUoMComboBox.Items.AddRange(par.GetUoMs().Keys.ToArray());
            if (ParameterUoMComboBox.Items.Count >0)
                ParameterUoMComboBox.SelectedIndex = par.GetSelectedUoM();
            PIDLabel.Text = par.id.ToString();
        }
        public void UpdateSchemeOptions()
        {
            PressureUnitСomboBox.Items.Clear();
            PressureUnitСomboBox.Items.Add("---");
            PressureParComboBox1.Items.Clear();
            PressureParComboBox1.Items.Add("---");
            PressureParComboBox2.Items.Clear();
            PressureParComboBox2.Items.Add("---");
            PressureParComboBox3.Items.Clear();
            PressureParComboBox3.Items.Add("---");
            PressureParComboBox4.Items.Clear();
            PressureParComboBox4.Items.Add("---");
            PressureParComboBox5.Items.Clear();
            PressureParComboBox5.Items.Add("---");

            ValveUnitComboBox.Items.Clear();
            ValveUnitComboBox.Items.Add("---");
            ValveParComboBox.Items.Clear();
            ValveParComboBox.Items.Add("---");

            FCUnitComboBox.Items.Clear();
            FCUnitComboBox.Items.Add("---");
            FCParComboBox.Items.Clear();
            FCParComboBox.Items.Add("---");

            FlowmeterUnitComboBox.Items.Clear();
            FlowmeterUnitComboBox.Items.Add("---");
            FlowmeterParComboBox.Items.Clear();
            FlowmeterParComboBox.Items.Add("---");

            foreach (var un in UnitsList)
            {
                PressureUnitСomboBox.Items.Add(un.GetName());
                ValveUnitComboBox.Items.Add(un.GetName());
                FCUnitComboBox.Items.Add(un.GetName());
                FlowmeterUnitComboBox.Items.Add(un.GetName());
            }
        }
        public void LoadScheme()
        {
            SchemeComboBox.Items.Clear();
            foreach (XElement sc in xSettings.Elements("scheme"))
            {
                SchemasList.Add(new Scheme(sc));
                SchemeComboBox.Items.Add(SchemasList.Last().name);
            }
            SchemasList.OrderBy(x => x.SchemeN);
            SchemeComboBox.SelectedIndex = 0;
        }
        public void SaveScheme()
        {
            if (SchemeComboBox.SelectedIndex != -1)
            {
                SchemasList[SchemeComboBox.SelectedIndex].SaveXML(ref UnitsList, SchemeNameTextBox, 
                    SchemeComboBox, PressureUnitСomboBox, PressureParComboBox1, PressureParComboBox2, 
                    PressureParComboBox3, PressureParComboBox4, PressureParComboBox5, FlowmeterUnitComboBox, 
                    FlowmeterParComboBox, FCUnitComboBox, FCParComboBox, FCParPowerComboBox, ValveUnitComboBox, ValveParComboBox, 
                    DensityTextBox, L1TextBox, L2TextBox, L3TextBox, L4TextBox, DiameterTextBox);
                SchemeNotSavedLabel.Visible = false;
            }
            SchemeComboBox.Items.Clear();
            foreach (var sc in SchemasList)
                SchemeComboBox.Items.Add(sc.name);
        }
        private void GetComPorts()
        {
            FlowComComboBox.Items.Clear();
            FrequencyComComboBox.Items.Clear();
            PressureComComboBox.Items.Clear();
            ValveComComboBox.Items.Clear();
            VibrationComComboBox.Items.Clear();
            UnitComComboBox.Items.Clear();

            FlowComComboBox.Items.AddRange(SerialPort.GetPortNames());
            FrequencyComComboBox.Items.AddRange(SerialPort.GetPortNames());
            PressureComComboBox.Items.AddRange(SerialPort.GetPortNames());
            ValveComComboBox.Items.AddRange(SerialPort.GetPortNames());
            VibrationComComboBox.Items.AddRange(SerialPort.GetPortNames());
            UnitComComboBox.Items.AddRange(SerialPort.GetPortNames());
        }
        /*public void GetUnitsOfMeasure()
        {
            PFlowСomboBox.Items.Clear();
            PCurrentСomboBox.Items.Clear();
            PCurrentFreqСomboBox.Items.Clear();
            PVoltageСomboBox.Items.Clear();
            PTorqueСomboBox.Items.Clear();
            PPowerСomboBox.Items.Clear();
            PRPMСomboBox.Items.Clear();
            PLoadСomboBox.Items.Clear();
            PXAmplitudeСomboBox.Items.Clear();
            PYAmplitydeСomboBox.Items.Clear();
            PPressureСomboBox.Items.Clear();

            PFlowСomboBox.Items.AddRange(Flow.UnitsOfMeasure.Keys.ToArray());
            PCurrentСomboBox.Items.AddRange(Current.UnitsOfMeasure.Keys.ToArray());
            PCurrentFreqСomboBox.Items.AddRange(CurrentFrequency.UnitsOfMeasure.Keys.ToArray());
            PVoltageСomboBox.Items.AddRange(Voltage.UnitsOfMeasure.Keys.ToArray());
            PTorqueСomboBox.Items.AddRange(Torque.UnitsOfMeasure.Keys.ToArray());
            PPowerСomboBox.Items.AddRange(Power.UnitsOfMeasure.Keys.ToArray());
            PRPMСomboBox.Items.AddRange(RPM.UnitsOfMeasure.Keys.ToArray());
            PLoadСomboBox.Items.AddRange(EngineLoad.UnitsOfMeasure.Keys.ToArray());
            PXAmplitudeСomboBox.Items.AddRange(XAmplitude.UnitsOfMeasure.Keys.ToArray());
            PYAmplitydeСomboBox.Items.AddRange(YAmplitude.UnitsOfMeasure.Keys.ToArray());
            PPressureСomboBox.Items.AddRange(Pressure1.UnitsOfMeasure.Keys.ToArray());
        }*/
        /*internal void ParametersLoadXML()
        {
            xSettings = xDefaultDoc.Element("settings");
            foreach (XElement el in xSettings.Elements("parameter"))
            {
                if (el.Attribute("Name").Value == "Flow")
                {
                    Flow.LoadXML(el, ref PFlowRegisterTextBox, ref PFlowСomboBox, ref PFlowRegisterComboBox, ref PFlowTypeComboBox);
                }
                if (el.Attribute("Name").Value == "Current")
                {
                    Current.LoadXML(el, ref PCurrentTextBox, ref PCurrentСomboBox, ref PCurrentRegisterComboBox, ref PCurrentTypeComboBox);
                }
                if (el.Attribute("Name").Value == "CurrentFrequency")
                {
                    CurrentFrequency.LoadXML(el, ref PCurrentFreqTextBox, ref PCurrentFreqСomboBox, ref PFreqRegisterComboBox, ref PFreqTypeComboBox);
                }
                if (el.Attribute("Name").Value == "Voltage")
                {
                    Voltage.LoadXML(el, ref PVoltageTextBox, ref PVoltageСomboBox, ref PVoltageRegisterComboBox, ref PVoltageTypeComboBox);
                }
                if (el.Attribute("Name").Value == "Torque")
                {
                    Torque.LoadXML(el, ref PTorqueTextBox, ref PTorqueСomboBox, ref PTorqueRegisterComboBox, ref PTorqueTypeComboBox);
                }
                if (el.Attribute("Name").Value == "Power")
                {
                    Power.LoadXML(el, ref PPowerTextBox, ref PPowerСomboBox,ref PPowerRegisterComboBox, ref PPowerTypeComboBox);
                }
                if (el.Attribute("Name").Value == "RPM")
                {
                    RPM.LoadXML(el, ref PRPMTextBox, ref PRPMСomboBox, ref PRPMRegisterComboBox, ref PRPMTypeComboBox);
                }
                if (el.Attribute("Name").Value == "EngineLoad")
                {
                    EngineLoad.LoadXML(el, ref PLoadTextBox, ref PLoadСomboBox, ref PLoadRegisterComboBox, ref PLoadTypeComboBox);
                }
                if (el.Attribute("Name").Value == "XAmplitude")
                {
                    XAmplitude.LoadXML(el, ref PXAmplitudeTextBox, ref PXAmplitudeСomboBox, ref PXAmplitudeRegisterComboBox, ref PXAmplitudeTypeComboBox);
                }
                if (el.Attribute("Name").Value == "YAmplitude")
                {
                    YAmplitude.LoadXML(el, ref PYAmplitudeTextBox, ref PYAmplitydeСomboBox, ref PYAmplitudeRegisterComboBox, ref PYAmplitudeTypeComboBox);
                }
                if (el.Attribute("Name").Value == "Pressure1")
                {
                    Pressure1.LoadXML(el, ref PPressure1TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure1CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure2")
                {
                    Pressure2.LoadXML(el, ref PPressure2TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure2CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure3")
                {
                    Pressure3.LoadXML(el, ref PPressure3TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure3CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure4")
                {
                    Pressure4.LoadXML(el, ref PPressure4TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure4CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure5")
                {
                    Pressure5.LoadXML(el, ref PPressure5TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure5CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure6")
                {
                    Pressure6.LoadXML(el, ref PPressure6TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure6CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure7")
                {
                    Pressure7.LoadXML(el, ref PPressure7TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure7CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure8")
                {
                    Pressure8.LoadXML(el, ref PPressure8TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure8CheckBox);
                }
            }
        }*/
        /*internal void ParametersSaveXML()
        {
            Flow.SaveXML(ref PFlowRegisterTextBox, ref PFlowСomboBox, ref PFlowRegisterComboBox, ref PFlowTypeComboBox);
            Current.SaveXML(ref PCurrentTextBox, ref PCurrentСomboBox,ref PCurrentRegisterComboBox, ref PCurrentTypeComboBox);
            CurrentFrequency.SaveXML(ref PCurrentFreqTextBox, ref PCurrentFreqСomboBox, ref PFreqRegisterComboBox, ref PFreqTypeComboBox);
            Voltage.SaveXML(ref PVoltageTextBox, ref PVoltageСomboBox, ref PVoltageRegisterComboBox, ref PVoltageTypeComboBox);
            Torque.SaveXML(ref PTorqueTextBox, ref PTorqueСomboBox, ref PTorqueRegisterComboBox, ref PTorqueTypeComboBox);
            Power.SaveXML(ref PPowerTextBox, ref PPowerСomboBox,ref PPowerRegisterComboBox, ref PPowerTypeComboBox);
            RPM.SaveXML(ref PRPMTextBox, ref PRPMСomboBox, ref PRPMRegisterComboBox, ref PRPMTypeComboBox);
            EngineLoad.SaveXML(ref PLoadTextBox, ref PLoadСomboBox, ref PLoadRegisterComboBox, ref PLoadTypeComboBox);
            XAmplitude.SaveXML(ref PXAmplitudeTextBox, ref PXAmplitudeСomboBox, ref PXAmplitudeRegisterComboBox
                , ref PXAmplitudeTypeComboBox);
            YAmplitude.SaveXML(ref PYAmplitudeTextBox, ref PYAmplitydeСomboBox, ref PYAmplitudeRegisterComboBox
                , ref PYAmplitudeTypeComboBox);
            Pressure1.SaveXML(ref PPressure1TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure1CheckBox);
            Pressure2.SaveXML(ref PPressure2TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure2CheckBox);
            Pressure3.SaveXML(ref PPressure3TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure3CheckBox);
            Pressure4.SaveXML(ref PPressure4TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure4CheckBox);
            Pressure5.SaveXML(ref PPressure5TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure5CheckBox);
            Pressure6.SaveXML(ref PPressure6TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure6CheckBox);
            Pressure7.SaveXML(ref PPressure7TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure7CheckBox);
            Pressure8.SaveXML(ref PPressure8TextBox, ref PPressureСomboBox, ref PPressureRegisterComboBox, ref PPressureTypeComboBox, ref PPressure8CheckBox);

            xSettings.Save("Defaults.xml");
        }*/
        internal void UnitLoadXML()
        {
            xSettings = xDefaultDoc.Element("settings");
            IEnumerable<XElement> pars = xSettings.Elements("parameter");
            foreach (XElement un in xSettings.Elements("unit"))
            {
                UnitsList.Add(new Unit(un, pars));
                /*if (un.Attribute("Name").Value == "Flowmeter")
                    FlowMeter.LoadSettingsXML(un, ref FlowComComboBox, ref FlowSpeedTextBox, ref FlowAddressTextBox);
                if (un.Attribute("Name").Value == "FrequencyChanger")
                    FrequencyChanger.LoadSettingsXML(un, ref FrequencyComComboBox, ref FrequencySpeedTextBox, ref FrequencyAddressTextBox);
                if (un.Attribute("Name").Value == "PressureGauges")
                    PressureGauges.LoadSettingsXML(un, ref PressureComComboBox, ref PressureSpeedTextBox, ref PressureAddressTextBox);
                if (un.Attribute("Name").Value == "Valve")
                    Valve.LoadSettingsXML(un, ref ValveComComboBox, ref ValveSpeedTextBox, ref ValveAddressTextBox);
                if (un.Attribute("Name").Value == "Vibration")
                    Vibration.LoadSettingsXML(un, ref VibrationComComboBox, ref VibrationSpeedTextBox, ref VibrationAddresTextBox);*/
            }
        }
        #endregion
        #region COM Con/Discon Event
        private struct DEV_BROADCAST_HDR
        {
#pragma warning disable 0649
            internal UInt32 dbch_size;
            internal UInt32 dbch_devicetype;
            internal UInt32 dbch_reserved;
#pragma warning restore 0649
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x0219)
            {
                //WM_DEVICECHANGE = 0x0219
                DEV_BROADCAST_HDR dbh;
                switch ((int)m.WParam)
                {
                    case 0x8000:
                        //DBT_DEVICEARRIVAL = 0x8000
                        dbh = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbh.dbch_devicetype == 0x00000003)
                        {
                            GetComPorts();
                            UnitLoadXML();
                            MessageBox.Show("Подключено новое устройство");
                        }
                        break;
                    case 0x8004:
                        //DBT_DEVICEREMOVECOMPLETE = 0x8004
                        dbh = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbh.dbch_devicetype == 0x00000003)
                        {
                            GetComPorts();
                            UnitLoadXML();
                            MessageBox.Show("Устройство отключено");
                        }
                        break;
                }
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            GetComPorts();
            //GetUnitsOfMeasure();
            if (!File.Exists($"{Application.StartupPath}/Defaults.xml"))
                File.Create($"{Application.StartupPath}/Defaults.xml");
            try
            {
                xDefaultDoc = XDocument.Load("Defaults.xml");
                UnitLoadXML();
                //ParametersLoadXML();
                UpdateUnitsList(0);
                UpdateSchemeOptions();
                LoadScheme();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка в файле настроек или файл настроек отсутствует");
            }
        }
        
        #region Расходомер
        private void FlowSaveButton_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                FlowMeter.SetComParameters(FlowComComboBox.Text, Convert.ToInt32(FlowSpeedTextBox.Text),
                    Convert.ToInt32(FlowAddressTextBox.Text));
                FlowMeter.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
            */
        }

        private void FlowAdditionalButton_Click(object sender, EventArgs e)
        {
            /*
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref FlowMeter);
            addsett.Show();*/
        }

        private void FlowConnectButton_Click(object sender, EventArgs e)
        {
            /*
            if (FlowMeter.isConnected)
            {
                FlowMeter.Disconnect();
                FlowConnectButton.Text = "Подключиться";
                FlowStatusTextBox.BackColor = Color.Red;
            }
            else
            {
                if (FlowMeter.TryToConnect())
                {
                    FlowConnectButton.Text = "Отключиться";
                    FlowStatusTextBox.BackColor = Color.Green;
                }
            }*/
        }

        #endregion
        #region Частотник
        private void FrequencyAdditionalSettings_Click(object sender, EventArgs e)
        {
            /*
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref FrequencyChanger);
            addsett.Show();*/
        }

        private void FrequencySaveButton_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                FrequencyChanger.SetComParameters(FrequencyComComboBox.Text, Convert.ToInt32(FrequencySpeedTextBox.Text),
                    Convert.ToInt32(FrequencyAddressTextBox.Text));
                FrequencyChanger.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }*/
        }

        private void FrequencyConnectButton_Click(object sender, EventArgs e)
        {
            /*
            if (FrequencyChanger.isConnected)
            {
                FrequencyChanger.Disconnect();
                FrequencyConnectButton.Text = "Подключиться";
                FrequencyStatusTextBox.BackColor = Color.Red;
            }
            else
            {
                if (FrequencyChanger.TryToConnect())
                {
                    FrequencyConnectButton.Text = "Отключиться";
                    FrequencyStatusTextBox.BackColor = Color.Green;
                }
            }*/
        }
        #endregion
        #region Датчик давления
        private void PressureAdditionalButton_Click(object sender, EventArgs e)
        {
            /*
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref PressureGauges);
            addsett.Show();*/
        }

        private void PressureConnectButton_Click(object sender, EventArgs e)
        {
            /*
            if (PressureGauges.isConnected)
            {
                PressureGauges.Disconnect();
                PressureConnectButton.Text = "Подключиться";
                PressureStatusTextBox.BackColor = Color.Red;
            }
            else
            {
                if (PressureGauges.TryToConnect())
                {
                    PressureConnectButton.Text = "Отключиться";
                    PressureStatusTextBox.BackColor = Color.Green;
                }
            }*/
        }

        private void PressureSaveButton_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                PressureGauges.SetComParameters(PressureComComboBox.Text, Convert.ToInt32(PressureSpeedTextBox.Text),
                    Convert.ToInt32(PressureAddressTextBox.Text));
                PressureGauges.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }*/
        }

        #endregion
        #region Задвижка
        private void ValveAdditionalButton_Click(object sender, EventArgs e)
        {
            //AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref Valve);
            //addsett.Show();
        }
        private void ValveConnectButton_Click(object sender, EventArgs e)
        {
            /*
            if (Valve.isConnected)
            {
                Valve.Disconnect();
                ValveConnectButton.Text = "Подключиться";
                ValveStatusTextBox.BackColor = Color.Red;
            }
            else
            {
                if (Valve.TryToConnect())
                {
                    ValveConnectButton.Text = "Отключиться";
                    ValveStatusTextBox.BackColor = Color.Green;
                }
            }*/
        }
        private void ValveSaveButton_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                Valve.SetComParameters(ValveComComboBox.Text, Convert.ToInt32(ValveSpeedTextBox.Text),
                    Convert.ToInt32(ValveAddressTextBox.Text));
                Valve.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }*/
        }
        #endregion
        #region Датчик вибрации
        private void VibrationAdditionalSettings_Click(object sender, EventArgs e)
        {
            //AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref Vibration);
            //addsett.Show();
        }

        private void VibrationConnectButton_Click(object sender, EventArgs e)
        {
            /*
            if (Vibration.isConnected)
            {
                Vibration.Disconnect();
                VibrationConnectButton.Text = "Подключиться";
                VibrationStatusTextBox.BackColor = Color.Red;
            }
            else
            {
                if (Vibration.TryToConnect())
                {
                    VibrationConnectButton.Text = "Отключиться";
                    VibrationStatusTextBox.BackColor = Color.Green;
                }
            }*/
        }

        private void VibrationSaveButton_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                Vibration.SetComParameters(VibrationComComboBox.Text, Convert.ToInt32(VibrationSpeedTextBox.Text),
                    Convert.ToInt32(VibrationAddresTextBox.Text));
                Vibration.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }*/
        }
        #endregion
        /// OLD/NEW
        #region Настройки соединения
        private void UnitAddButton_Click(object sender, EventArgs e)
        {
            Unit un = new Unit();
            UnitsList.Add(un);
            un.SaveSettingsXML();

            UpdateUnitsList(UnitsList.Count - 1);
        }


        private void UnitsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedUnit();
        }

        private void UnitSaveButton_Click(object sender, EventArgs e)
        {
            int selected = UnitsListBox.SelectedIndex;
            Unit un = UnitsList[selected];
            try
            {
                un.SetComParameters(UnitNameTextBox.Text, UnitComComboBox.Text,
                    int.Parse(UnitBaudRateTextBox.Text), int.Parse(UnitAddressTextBox.Text));
                un.SaveSettingsXML();
                UpdateUnitsList(selected);
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Введены некорректные параметры");
            }
        }

        private void UnitConnectButton_Click(object sender, EventArgs e)
        {
            int selected = UnitsListBox.SelectedIndex;
            Unit un = UnitsList[selected];
            if (un.isConnected)
                un.Disconnect();
            else
                un.TryToConnect();

            UpdateUnitsList(selected);
            UpdateSelectedUnit();
        }

        private void UnitDeleteButton_Click(object sender, EventArgs e)
        {
            int selected = UnitsListBox.SelectedIndex;
            string Name = UnitsList[selected].GetName();
            DialogResult result = MessageBox.Show($"Вы точно хотите удалить устройство {Name}",
                "Удаление устройства", MessageBoxButtons.YesNo);
            switch (result)
            {
                case DialogResult.Yes:
                    UnitsList.ElementAt(selected).Dispose();
                    UnitsList.RemoveAt(selected);
                    UpdateUnitsList(0);
                    break;
                default:
                    break;
            }
        }

        private void UnitAdditionalSettingsButton_Click(object sender, EventArgs e)
        {
            Unit un = UnitsList[UnitsListBox.SelectedIndex];
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref un);
            addsett.Show();
        }

        private void ParametersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedParameter();
        }

        private void RedactorUoMButton1_Click(object sender, EventArgs e)
        {
            if (UnitsListBox.SelectedIndex >= 0 && ParametersListBox.SelectedIndex >= 0)
            {
                Parameter par = UnitsList[UnitsListBox.SelectedIndex]
                .GetParametersList()[ParametersListBox.SelectedIndex];
                UoMRedactor UoMred = new UoMRedactor(ref par,
                    UnitsList[UnitsListBox.SelectedIndex].GetName(), this);
                UoMred.Show();
            }
        }

        private void ParameterAddButton_Click(object sender, EventArgs e)
        {
            Unit un = UnitsList[UnitsListBox.SelectedIndex];
            un.AddParameter();
            un.SaveSettingsXML();
            UpdateSelectedUnit();
        }

        private void ParameterDeleteButton_Click(object sender, EventArgs e)
        {
            int selected = ParametersListBox.SelectedIndex;
            string Name = ParametersListBox.SelectedItem.ToString();
            DialogResult result = MessageBox.Show($"Вы точно хотите удалить параметр {Name}",
                "Удаление устройства", MessageBoxButtons.YesNo);
            switch (result)
            {
                case DialogResult.Yes:
                    UnitsList[UnitsListBox.SelectedIndex].DeleteSelectedParameter(selected);
                    try
                    {
                        File.Delete($"{Application.StartupPath}/UoM/" +
                            $"{UnitsList[UnitsListBox.SelectedIndex].GetName()}/{Name}.dat");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Параметр успешно удален, но не удалось удалить файл с настройками единиц измерения. Попробуйте удалить его в ручную");
                    }
                    UpdateUnitsList(UnitsListBox.SelectedIndex);
                    break;
                default:
                    break;
            }
        }

        private void ParameterSaveButton_Click(object sender, EventArgs e)
        {
            int selected = UnitsListBox.SelectedIndex;
            Unit un = UnitsList[selected];
            try
            {
                Parameter par = un.GetParametersList()[ParametersListBox.SelectedIndex];
                par.SetParameters(ParameterNameTextBox.Text, Convert.ToUInt16(ParameterRegAddressTextBox.Text),
                    ParameterUoMComboBox.SelectedIndex, ParameterReadableCheckBox.Checked,
                    (RegType)ParameterRegTypeComboBox.SelectedIndex, (DataType)ParameterTypeComboBox.SelectedIndex);
                par.SaveXML();
                xSettings.Save("Defaults.xml");
                UpdateUnitsList(selected);
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Введены некорректные параметры");
            }
        }
        #endregion
        #region Схема
        private void SchemaAddButton_Click(object sender, EventArgs e)
        {
            SchemasList.Add(new Scheme());
            SchemeComboBox.Items.Add(SchemasList.Last().name);
            SchemeComboBox.SelectedIndex = SchemasList.Count - 1;
        }

        private void SchemaSaveButton_Click(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
            {
                try
                {
                    SaveScheme();
                    MessageBox.Show("Настройки сохранены");
                }
                catch (Exception)
                {
                    MessageBox.Show("Введены некорректные параметры");
                }
            }
        }


        private void PressureUnitСomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
            if (PressureUnitСomboBox.SelectedIndex > 0)
            {
                PressureParComboBox1.Items.Clear();
                PressureParComboBox1.Items.Add("---");
                PressureParComboBox2.Items.Clear();
                PressureParComboBox2.Items.Add("---");
                PressureParComboBox3.Items.Clear();
                PressureParComboBox3.Items.Add("---");
                PressureParComboBox4.Items.Clear();
                PressureParComboBox4.Items.Add("---");
                PressureParComboBox5.Items.Clear();
                PressureParComboBox5.Items.Add("---");
                var parlist = UnitsList.Find(un => un.GetName() == PressureUnitСomboBox.Text).GetParametersList();
                parlist.ForEach(par => PressureParComboBox1.Items.Add(par.GetName()));
                parlist.ForEach(par => PressureParComboBox2.Items.Add(par.GetName()));
                parlist.ForEach(par => PressureParComboBox3.Items.Add(par.GetName()));
                parlist.ForEach(par => PressureParComboBox4.Items.Add(par.GetName()));
                parlist.ForEach(par => PressureParComboBox5.Items.Add(par.GetName()));
            }
            //UpdateSchemeParametersForUnits();
        }

        private void FCUnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
            if (FCUnitComboBox.SelectedIndex > 0)
            {
                FCParComboBox.Items.Clear();
                FCParComboBox.Items.Add("---");
                var parlist = UnitsList.Find(un => un.GetName() == FCUnitComboBox.Text).GetParametersList();
                parlist.ForEach(par => FCParComboBox.Items.Add(par.GetName()));

                FCParPowerComboBox.Items.Clear();
                FCParPowerComboBox.Items.Add("---");
                parlist.ForEach(par => FCParPowerComboBox.Items.Add(par.GetName()));
            }
        }

        private void ValveUnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
            if (ValveUnitComboBox.SelectedIndex > 0)
            {
                ValveParComboBox.Items.Clear();
                ValveParComboBox.Items.Add("---");
                var parlist = UnitsList.Find(un => un.GetName() == ValveUnitComboBox.Text).GetParametersList();
                parlist.ForEach(par => ValveParComboBox.Items.Add(par.GetName()));
            }
        }

        private void FlowmeterUnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
            if (FlowmeterUnitComboBox.SelectedIndex > 0)
            {
                FlowmeterParComboBox.Items.Clear();
                FlowmeterParComboBox.Items.Add("---");
                var parlist = UnitsList.Find(un => un.GetName() == FlowmeterUnitComboBox.Text).GetParametersList();
                parlist.ForEach(par => FlowmeterParComboBox.Items.Add(par.GetName()));
            }
        }

        private void SchemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedScheme = SchemasList.Find(sc => sc.SchemeN == SchemeComboBox.SelectedIndex);
            if (SelectedScheme != null)
            {
                SchemeNameTextBox.Text = SelectedScheme.name;
                DensityTextBox.Text = SelectedScheme.Density.ToString();
                L1TextBox.Text = SelectedScheme.dL[0].ToString();
                L2TextBox.Text = SelectedScheme.dL[1].ToString();
                L3TextBox.Text = SelectedScheme.dL[2].ToString();
                L4TextBox.Text = SelectedScheme.dL[3].ToString();

                if (SelectedScheme.PressureUID != -1)
                {
                    var unit = UnitsList.Find(un => un.id == SelectedScheme.PressureUID);
                    PressureUnitСomboBox.SelectedIndex = PressureUnitСomboBox.Items
                        .IndexOf(unit.GetName());
                    if (SelectedScheme.PressureP1ID != -1)
                        PressureParComboBox1.SelectedIndex = PressureParComboBox1.Items
                            .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.PressureP1ID).GetName());
                    else
                        PressureParComboBox1.SelectedIndex = 0;
                    if (SelectedScheme.PressureP2ID != -1)
                        PressureParComboBox2.SelectedIndex = PressureParComboBox1.Items
                            .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.PressureP2ID).GetName());
                    else
                        PressureParComboBox2.SelectedIndex = 0;
                    if (SelectedScheme.PressureP3ID != -1)
                        PressureParComboBox3.SelectedIndex = PressureParComboBox1.Items
                            .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.PressureP3ID).GetName());
                    else
                        PressureParComboBox3.SelectedIndex = 0;
                    if (SelectedScheme.PressureP4ID != -1)
                        PressureParComboBox4.SelectedIndex = PressureParComboBox1.Items
                            .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.PressureP4ID).GetName());
                    else
                        PressureParComboBox4.SelectedIndex = 0;
                    if (SelectedScheme.PressureP5ID != -1)
                        PressureParComboBox5.SelectedIndex = PressureParComboBox1.Items
                            .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.PressureP5ID).GetName());
                    else
                        PressureParComboBox5.SelectedIndex = 0;
                }
                else
                {
                    PressureUnitСomboBox.SelectedIndex = 0;
                    PressureParComboBox1.SelectedIndex = 0;
                    PressureParComboBox2.SelectedIndex = 0;
                    PressureParComboBox3.SelectedIndex = 0;
                    PressureParComboBox4.SelectedIndex = 0;
                    PressureParComboBox5.SelectedIndex = 0;
                }
                if (SelectedScheme.FCUID != -1)
                {
                    var unit = UnitsList.Find(un => un.id == SelectedScheme.FCUID);
                    FCUnitComboBox.SelectedIndex = FCUnitComboBox.Items
                        .IndexOf(unit.GetName());
                    FCParComboBox.SelectedIndex = FCParComboBox.Items
                        .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.FCPID).GetName());
                    FCParPowerComboBox.SelectedIndex = FCParPowerComboBox.Items
                        .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.FCPPID).GetName());
                }
                else
                {
                    FCUnitComboBox.SelectedIndex = 0;
                    FCParComboBox.SelectedIndex = 0;
                    FCParPowerComboBox.SelectedIndex= 0;
                }
                if (SelectedScheme.FlowUID != -1)
                {
                    var unit = UnitsList.Find(un => un.id == SelectedScheme.FlowUID);
                    FlowmeterUnitComboBox.SelectedIndex = FlowmeterUnitComboBox.Items
                        .IndexOf(unit.GetName());
                    FlowmeterParComboBox.SelectedIndex = FlowmeterParComboBox.Items
                        .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.FlowPID).GetName());
                }
                else
                {
                    FlowmeterUnitComboBox.SelectedIndex = 0;
                    FlowmeterParComboBox.SelectedIndex = 0;
                }
                if (SelectedScheme.ValveUID != -1)
                {
                    var unit = UnitsList.Find(un => un.id == SelectedScheme.ValveUID);
                    ValveUnitComboBox.SelectedIndex = ValveUnitComboBox.Items
                        .IndexOf(unit.GetName());
                    ValveParComboBox.SelectedIndex = ValveParComboBox.Items
                        .IndexOf(unit.GetParametersList().Find(par => par.id == SelectedScheme.ValvePID).GetName());
                }
                else
                {

                    ValveUnitComboBox.SelectedIndex = 0;
                    ValveParComboBox.SelectedIndex = 0;
                }

                SchemeNotSavedLabel.Visible = false;
            }
        }
        private void FCParPowerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true; // лобавить для остальных
            if (FCParPowerComboBox.SelectedIndex != 0)
            {
                PowerCustomLabel.Text = UnitsList.Find(u => u.GetName() == FCUnitComboBox.Text).GetParametersList()
                    .Find(p => p.GetName() == FCParPowerComboBox.Text).GetUoMstring();
            }
        }

        private void FlowmeterParComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
            if (FlowmeterParComboBox.SelectedIndex != 0)
            {
                FlowCustomLabel.Text = UnitsList.Find(u => u.GetName() == FlowmeterUnitComboBox.Text).GetParametersList()
                    .Find(p => p.GetName() == FlowmeterParComboBox.Text).GetUoMstring();
            }
        }

        private void PressureParComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
            if (PressureParComboBox1.SelectedIndex != 0)
            {
                PressureCustomLabel.Text = UnitsList.Find(u => u.GetName() == PressureUnitСomboBox.Text).GetParametersList()
                    .Find(p => p.GetName() == PressureParComboBox1.Text).GetUoMstring();
            }
        }
        #endregion
        #region РНХ
        ManualResetEvent StartReadingEvent = new ManualResetEvent(false);
        ManualResetEvent AddEvent = new ManualResetEvent(false);
        private void UnitParametersRead(Unit un, ManualResetEvent finished)
        {
            while (true)
            {
                StartReadingEvent.WaitOne();
                if (stopthread)
                    break;
                un.ReadAllParams();
                finished.Set();
                StartReadingEvent.Reset();
            }
        }
        
        private void PerformanceParametersReadout(ManualResetEvent[] FinishedReadingEventList)
        {
            bool IsStartPossible = true;
            
            List<Parameter> PressureParametersList = new List<Parameter>();
            Parameter FCPowerParameter =null;
            float PowerLastReg = 0;
            Parameter FlowParameter = null;
            float FlowLastReg = 0;
            if (SelectedScheme.PressureP1ID != -1 && SelectedScheme.PressureP2ID != -1)
            {
                PressureParametersList.Add(UnitsList.Find(u => u.id == SelectedScheme.PressureUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.PressureP1ID));
                PressureParametersList.Add(UnitsList.Find(u => u.id == SelectedScheme.PressureUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.PressureP2ID));
            }
            else
            {
                MessageBox.Show("Не заданы контролируемые параметры давления");
                stopthread = true;
                IsStartPossible = false;
            }
            if (SelectedScheme.PressureP3ID != -1)
            {
                PressureParametersList.Add(UnitsList.Find(u => u.id == SelectedScheme.PressureUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.PressureP3ID));
            }
            if (SelectedScheme.PressureP4ID != -1)
            {
                PressureParametersList.Add(UnitsList.Find(u => u.id == SelectedScheme.PressureUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.PressureP4ID));
            }
            if (SelectedScheme.PressureP5ID != -1)
            {
                PressureParametersList.Add(UnitsList.Find(u => u.id == SelectedScheme.PressureUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.PressureP5ID));
            }
            if (SelectedScheme.FCPPID != -1)
            {
                FCPowerParameter = UnitsList.Find(u => u.id == SelectedScheme.FCUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.FCPPID);
            }
            else
            {
                MessageBox.Show("Не задан контролируемый параметр мощности");
                stopthread = true;
                IsStartPossible = false;
            }
            if (SelectedScheme.FlowPID != -1)
            {
                FlowParameter = UnitsList.Find(u => u.id == SelectedScheme.FlowUID).GetParametersList()
                    .Find(par => par.id == SelectedScheme.FlowPID);
            }
            else
            {
                MessageBox.Show("Не задан контролируемый параметр расхода");
                stopthread = true;
                IsStartPossible = false;
            }
            float[] H = new float[PressureParametersList.Count() - 1];
            float[] ECE = new float[PressureParametersList.Count() - 1];
            StartReadingEvent.Set();
            //Thread.Sleep(1000);
            while (IsStartPossible)
            {
                WaitHandle.WaitAll(FinishedReadingEventList);
                foreach (var threadfinished in FinishedReadingEventList)
                    threadfinished.Reset();

                PowerLastReg = FCPowerParameter.GetLastMeasuredRegs();
                FlowLastReg = FlowParameter.GetLastMeasuredRegs();
                for (int i = 0; i < H.Length; i++)
                {
                    H[i] = SelectedScheme.dL[i] + (PressureParametersList[i+1].GetLastMeasuredRegs() - 
                        PressureParametersList[i].GetLastMeasuredRegs()) / (SelectedScheme.Density * Scheme.g_constant);
                    ECE[i] = SelectedScheme.Density * Scheme.g_constant * H[i] * FlowLastReg / PowerLastReg;
                }

                Action<float> lb1 = (float f) => listBox1.Items.Add(f);
                Action<float> lb2 = (float f) => listBox2.Items.Add(f); 
                Action<float> lb3 = (float f) => listBox3.Items.Add(f);

                Action<int> readH = (int i) => chart1.Series[1].Points.AddXY(FlowLastReg, H[i]);
                Action readN = () => chart1.Series[3].Points.AddXY(FlowLastReg, PowerLastReg);

                Action clearH = () => chart1.Series[1].Points.Clear();
                Action clearN = () => chart1.Series[3].Points.Clear();

                Action addH = () => chart1.Series[0].Points.AddXY(FlowLastReg, H[0]);
                Action addN = () => chart1.Series[2].Points.AddXY(FlowLastReg, PowerLastReg);
                if (InvokeRequired)
                {
                    Invoke(lb1, PowerLastReg);
                    Invoke(lb2, FlowLastReg);
                    Invoke(lb3, PressureParametersList[0].GetLastMeasuredRegs());
                    Invoke(clearH);
                    Invoke(clearN);
                    Invoke(readH, 0);
                    Invoke(readN);
                }
                else
                {
                    lb1(PowerLastReg);
                    lb2(FlowLastReg);
                    lb3(PressureParametersList[0].GetLastMeasuredRegs());
                    readH(0);
                    readN();
                    clearH();
                    clearN();
                }
                StartReadingEvent.Set();
                if (_stopper.WaitOne(1000, false))
                {
                    if (stopthread)
                        break;
                    if (InvokeRequired)
                    {
                        Invoke(addH);
                        Invoke(addN);
                    }
                    else
                    {
                        addH();
                        addN();
                    }
                    _stopper.Reset();
                }
            }
        }
        private void PerformanceTestStartButton_Click(object sender, EventArgs e)
        {
            try
            {
                SiPressureCoefficient = Convert.ToSingle(PressureSITextBox.Text) /
                    Convert.ToSingle(PressureCustomTextBox.Text);
                SiFlowCoefficient = Convert.ToSingle(FlowSITextBox.Text) /
                    Convert.ToSingle(FlowCustomTextBox.Text);
                SiPowerCoefficient = Convert.ToSingle(PowerSITextBox.Text) /
                    Convert.ToSingle(PowerCustomTextBox.Text);
                //System.FormatException

                stopthread = false;
                _stopper.Reset();
                ManualResetEvent[] FinishedReadingEventList = new ManualResetEvent[UnitsList.Count()];
                int UnCount = 0;
                foreach (Unit un in UnitsList)
                {
                    un.ClearMeasuredRegs();
                    ManualResetEvent _IsFinished = new ManualResetEvent(false);
                    FinishedReadingEventList[UnCount] = _IsFinished;
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { UnitParametersRead(un, _IsFinished); });
                    UnCount++;
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
                var threadParameters1 = new System.Threading.ThreadStart(delegate
                { PerformanceParametersReadout(FinishedReadingEventList); });
                var thread1 = new System.Threading.Thread(threadParameters1);
                thread1.Start();
                /*for (int i = 0; i < UnCount; i++)
                {
                    UnitsList[i].ClearMeasuredRegs();
                    ManualResetEvent _IsFinished = new ManualResetEvent(false);
                    FinishedReadingEventList[i] = _IsFinished;
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { UnitParametersRead(UnitsList[i], FinishedReadingEventList[i]); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }*/
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Ошибка в переводе параметров к СИ");
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FCParPowerComboBox.SelectedIndex != 0)
            {
                PowerCustomLabel.Text = UnitsList.Find(u => u.GetName() == FCUnitComboBox.Text).GetParametersList()
                    .Find(p => p.GetName() == FCParPowerComboBox.Text).GetUoMstring();
            }
            if (FlowmeterParComboBox.SelectedIndex != 0)
            {
                FlowCustomLabel.Text = UnitsList.Find(u => u.GetName() == FlowmeterUnitComboBox.Text).GetParametersList()
                    .Find(p => p.GetName() == FlowmeterParComboBox.Text).GetUoMstring();
            }
            if (PressureParComboBox1.SelectedIndex != 0)
            {
                PressureCustomLabel.Text = UnitsList.Find(u => u.GetName() == PressureUnitСomboBox.Text).GetParametersList()
                    .Find(p => p.GetName() == PressureParComboBox1.Text).GetUoMstring();
            }
        }
        #endregion
        internal void TestReadHolding(Unit un, ListBox l, ref Parameter par, bool isReversed, int offset)
        {
            int wait = 1000;
            while (true)
            {
                string UoM = par.GetUoMstring();
                float fRegs = un.ComReadHolding(ref par, isReversed, offset);
                Action read = () => l.Items.Add($"{DateTime.Now.Second}c {fRegs} [{UoM}]");
                if (InvokeRequired)
                    Invoke(read);
                else
                    read();
                if (_stopper.WaitOne(wait, false))
                {
                    break;
                }
            }
        }
        internal void TestReadInput(Unit un, ListBox l, ref Parameter par, bool isReversed, int offset)
        {
            int wait = 1000;
            while (true)
            {
                string UoM = par.GetUoMstring();
                float fRegs = un.ComReadInput(ref par, isReversed, offset);
                Action read = () => l.Items.Add($"{DateTime.Now.Second}c {fRegs} [{UoM}]");
                if (InvokeRequired)
                    Invoke(read);
                else
                    read();
                if (_stopper.WaitOne(wait, false))
                {
                    break;
                }
            }
        }

        internal void TestChart(Unit un, ref Parameter par, int series)//, Unit un2, ref Parameter par2, 
            //Unit un3, ref Parameter par3)
        {
            int wait = 1000;
            var start = DateTime.Now;
            while (true)
            {
                //float fRegs = un.ComRead(par, 0);
                //Action read = () => VarTimeChart.Series[series+1].Points.AddXY((DateTime.Now - start).Seconds, fRegs);
                //Action add = () => VarTimeChart.Series[series].Points.AddXY((DateTime.Now - start).Seconds, fRegs);
                Action clear = () => VarTimeChart.Series[series+1].Points.Clear();
                if (InvokeRequired)
                {
                    //Invoke(clear);
                    //Invoke(read);
                }
                else
                {
                    clear();
                    //read();
                }
                if (_stopper.WaitOne(wait, false))
                {
                    if (stopthread)
                        break;
                    //par.SetMeasuredRegs(fRegs);
                    //if (InvokeRequired)
                        //Invoke(add);
                    //else
                        //add();
                    _stopper.Reset();
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopthread = true;
            _stopper.Set();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
        private void RgistersAdditionalButton_Click(object sender, EventArgs e)
        {
            IsRegistersAdditionalToggled = !IsRegistersAdditionalToggled;
            RgisterLabel.Visible = IsRegistersAdditionalToggled;
            PLabel1.Visible = IsRegistersAdditionalToggled;
            PLabel2.Visible = IsRegistersAdditionalToggled;
            PLabel3.Visible = IsRegistersAdditionalToggled;
            PLabel4.Visible = IsRegistersAdditionalToggled;
            PLabel5.Visible = IsRegistersAdditionalToggled;
            PLabel6.Visible = IsRegistersAdditionalToggled;
            PLabel7.Visible = IsRegistersAdditionalToggled;
            PLabel8.Visible = IsRegistersAdditionalToggled;
            PPressure1CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure2CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure3CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure4CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure5CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure6CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure7CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure8CheckBox.Visible = IsRegistersAdditionalToggled;
            PPressure1TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure1TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure2TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure3TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure4TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure5TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure6TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure7TextBox.Visible = IsRegistersAdditionalToggled;
            PPressure8TextBox.Visible = IsRegistersAdditionalToggled;
            PFlowRegisterTextBox.Visible = IsRegistersAdditionalToggled;
            PCurrentTextBox.Visible = IsRegistersAdditionalToggled;
            PCurrentFreqTextBox.Visible = IsRegistersAdditionalToggled;
            PVoltageTextBox.Visible = IsRegistersAdditionalToggled;
            PTorqueTextBox.Visible = IsRegistersAdditionalToggled;
            PPowerTextBox.Visible = IsRegistersAdditionalToggled;
            PRPMTextBox.Visible = IsRegistersAdditionalToggled;
            PLoadTextBox.Visible = IsRegistersAdditionalToggled;
            PXAmplitudeTextBox.Visible = IsRegistersAdditionalToggled;
            PYAmplitudeTextBox.Visible = IsRegistersAdditionalToggled;

            RegTypeLabel.Visible = IsRegistersAdditionalToggled;
            PFlowRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PCurrentRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PFreqRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PVoltageRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PTorqueRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PPowerRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PRPMRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PLoadRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PXAmplitudeRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PYAmplitudeRegisterComboBox.Visible = IsRegistersAdditionalToggled;
            PPressureRegisterComboBox.Visible = IsRegistersAdditionalToggled;

            RegDataTypeLable.Visible = IsRegistersAdditionalToggled;
            PFlowTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PCurrentTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PVoltageTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PFreqTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PTorqueTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PPowerTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PRPMTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PLoadTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PXAmplitudeTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PYAmplitudeTypeComboBox.Visible = IsRegistersAdditionalToggled;
            PPressureTypeComboBox.Visible = IsRegistersAdditionalToggled;

            UoMRedactorButton.Visible = IsRegistersAdditionalToggled;
        }

        private void SaveParametersButton_Click(object sender, EventArgs e)
        {
            try
            {
                //ParametersSaveXML();
                MessageBox.Show("Настройки параметров успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки параметров");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            _stopper.Reset();
            stopthread = false;
            /*if (FlowMeter.isConnected)
            {
                //ushort address = 49161;
                int offset = 0;
                bool isReversed = false;

                var threadParameters = new System.Threading.ThreadStart(delegate 
                { TestReadInput(FlowMeter, listBox1, ref Flow, isReversed, offset); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }*/
            /*if (FlowMeter.isConnected)
            {
                //ushort address = 49161;
                //int offset = 0;
                //bool isReversed = false;
                Flow.MeasuredRegs.Clear();
                VarTimeChart.Series[0].Points.Clear();
                string UoM = Flow.GetUoMstring();
                VarTimeChart.Series[0].Name = UoM;
                VarTimeChart.Series[1].Name = UoM+" мгновенный";
                var threadParameters = new System.Threading.ThreadStart(delegate
                { TestChart(FlowMeter,ref Flow, 0); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
            if (PressureGauges.isConnected)
            {
                //ushort address = 49161;
                //int offset = 0;
                //bool isReversed = false;
                Pressure1.ClearMeasuredRegs();
                VarTimeChart.Series[2].Points.Clear();
                string UoM = Pressure1.GetUoMstring();
                VarTimeChart.Series[2].Name = UoM;
                VarTimeChart.Series[3].Name = UoM + " мгновенный";
                var threadParameters = new System.Threading.ThreadStart(delegate
                { TestChart(PressureGauges, ref Pressure1, 2); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopthread = true;
            _stopper.Set();
        }

        private void UoMRedactorButton_Click(object sender, EventArgs e)
        {
            //UoMRedactor UomRed = new UoMRedactor(ref ParameterDictionary,this);
            //UomRed.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
        /*
        private void TEST()
        {
            while (true)
            {
                float flow = FlowMeter.ComRead(Flow, 0);
                float p1 = PressureGauges.ComRead(Pressure1, 0);
                float p8 = PressureGauges.ComRead(Pressure8, 0);
                float N = flow * (p8 - p1);
                Action readdp = () => chart1.Series[1].Points.AddXY(flow, (p8 - p1));
                Action readN = () => chart1.Series[3].Points.AddXY(flow, N);

                Action cleardp = () => chart1.Series[1].Points.Clear();
                Action clearN = () => chart1.Series[3].Points.Clear();

                Action adddp = () => chart1.Series[0].Points.AddXY(flow, (p8 - p1));
                Action addN = () => chart1.Series[2].Points.AddXY(flow, N);
                if (InvokeRequired)
                {
                    Invoke(cleardp);
                    Invoke(clearN);
                    Invoke(readdp);
                    Invoke(readN);
                }
                else
                {
                    readdp();
                    readN();
                    cleardp();
                    clearN();
                }
                if (_stopper.WaitOne(1000, false))
                {
                    if (stopthread)
                        break;
                    Flow.SetMeasuredRegs(flow);
                    Pressure1.SetMeasuredRegs(p1);
                    Pressure1.SetMeasuredRegs(p8);
                    if (InvokeRequired)
                    {
                        Invoke(adddp);
                        Invoke(addN);
                    }
                    else
                    {
                        adddp();
                        addN();
                    }
                    _stopper.Reset();
                }
            }
        }*/
        private void TEST2(Unit un)
        {
            while (!stopthread)
            {
                StartReadingEvent.WaitOne();
                un.ReadAllParams();
                StartReadingEvent.Reset();
            }
        }
        private void TEST3()
        {
            StartReadingEvent.Set();
            Thread.Sleep(1000);
            while (true)
            {
                float pressure1 = UnitsList.Find(un => un.GetName() == "ОВЕН").GetParametersList()
                .Find(par => par.GetName() == "Давление 1").GetLastMeasuredRegs();
                float pressure8 = UnitsList.Find(un => un.GetName() == "ОВЕН").GetParametersList()
                    .Find(par => par.GetName() == "Давление 8").GetLastMeasuredRegs();
                float flow = UnitsList.Find(un => un.GetName() == "Расходомер").GetParametersList()
                    .Find(par => par.GetName() == "Расход").GetLastMeasuredRegs();

                Action readp = () => chart1.Series[1].Points.AddXY(flow, pressure1);
                Action readN = () => chart1.Series[3].Points.AddXY(flow, pressure8);

                Action cleardp = () => chart1.Series[1].Points.Clear();
                Action clearN = () => chart1.Series[3].Points.Clear();

                Action adddp = () => chart1.Series[0].Points.AddXY(flow, pressure1);
                Action addN = () => chart1.Series[2].Points.AddXY(flow, pressure8);
                if (InvokeRequired)
                {
                    Invoke(cleardp);
                    Invoke(clearN);
                    Invoke(readp);
                    Invoke(readN);
                }
                else
                {
                    readp();
                    readN();
                    cleardp();
                    clearN();
                }
                StartReadingEvent.Set();
                if (_stopper.WaitOne(1000, false))
                {
                    if (stopthread)
                        break;
                    if (InvokeRequired)
                    {
                        Invoke(adddp);
                        Invoke(addN);
                    }
                    else
                    {
                        adddp();
                        addN();
                    }
                    _stopper.Reset();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            stopthread = false;
            _stopper.Reset();
            foreach (Unit un in UnitsList)
            {
                var threadParameters = new System.Threading.ThreadStart(delegate
                { TEST2(un); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
            var threadParameters1 = new System.Threading.ThreadStart(delegate
            { TEST3(); });
            var thread1 = new System.Threading.Thread(threadParameters1);
            thread1.Start();
            /*if (true || (FlowMeter.isConnected && PressureGauges.isConnected && FrequencyChanger.isConnected))//поменять
            {
                stopthread = false;
                var threadParameters = new System.Threading.ThreadStart(delegate
                { TEST(); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }*/
        }

        private void button5_Click(object sender, EventArgs e)
        {
            stopthread = true;
            _stopper.Set();
            StartReadingEvent.Set();
            chart1.Series[0].ChartType = SeriesChartType.Spline;
            chart1.Series[2].Sort(PointSortOrder.Ascending, "X");
            chart1.Series[2].ChartType = SeriesChartType.Spline;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
        // изменение масштаба
        private void button7_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Maximum = Convert.ToInt32(XSizeTextBox.Text);
            chart1.ChartAreas[0].AxisX2.Maximum = Convert.ToInt32(XSizeTextBox.Text);
            chart1.ChartAreas[0].AxisY.Maximum = Convert.ToInt32(YSizeTextBox.Text);
            chart1.ChartAreas[0].AxisY2.Maximum = Convert.ToInt32(XSizeTextBox.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            
            workbook.Worksheets.Clear();
            Worksheet worksheet = workbook.Worksheets.Add("TEST");

            
            int j = 1;
            foreach(var un in UnitsList)
            {
                int i = 1;
                worksheet.Range[i++, j].Value = un.GetName();
                foreach (var par in un.GetParametersList())
                {
                    int istart = i;
                    worksheet.Range[i++, j].Value = par.GetName();
                    foreach (var reg in par.GetAllMeasuredRegs())
                        worksheet.Range[i++, j].Value = reg.ToString();
                    j++;
                    i = istart;
                }
            }
            worksheet.AllocatedRange.AutoFitColumns();
            workbook.SaveToFile("TEST.xlsx", ExcelVersion.Version2016);
        }

       
    }
}