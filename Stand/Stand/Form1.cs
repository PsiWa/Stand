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

namespace Stand
{
    public partial class Form1 : Form
    {
        private static ManualResetEvent _stopper = new ManualResetEvent(false);
        private bool stopthread= false;
        public Form1()
        {
            InitializeComponent();
        }

        private XDocument xDefaultDoc = new XDocument();
        public static XElement xSettings = new XElement("settings");
        internal bool IsRegistersAdditionalToggled = false;

        #region Инициализация устройств и параметров
        static public Unit FlowMeter = new Unit("Flowmeter");
        static public Unit FrequencyChanger = new Unit("FrequencyChanger");
        static public Unit PressureGauges = new Unit("PressureGauges");
        static public Unit Valve = new Unit("Valve");
        static public Unit Vibration = new Unit("Vibration");

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

        static private Parameter Flow = new Parameter("Flow");
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
            {Flow.name, Flow},
            {Current.name, Current},
            {CurrentFrequency.name, CurrentFrequency },
            {Voltage.name, Voltage},
            {Torque.name, Torque},
            {Power.name, Power},
            {RPM.name, RPM},
            {EngineLoad.name, EngineLoad},
            {XAmplitude.name, XAmplitude},
            {YAmplitude.name, YAmplitude},
            {Pressure1.name, Pressure1},
            {Pressure2.name, Pressure2},
            {Pressure3.name, Pressure3},
            {Pressure4.name, Pressure4},
            {Pressure5.name, Pressure5},
            {Pressure6.name, Pressure6},
            {Pressure7.name, Pressure7},
            {Pressure8.name, Pressure8}
        };

        #endregion
        #region Utils
        internal void GetComPorts()
        {
            FlowComComboBox.Items.Clear();
            FrequencyComComboBox.Items.Clear();
            PressureComComboBox.Items.Clear();
            ValveComComboBox.Items.Clear();
            VibrationComComboBox.Items.Clear();

            FlowComComboBox.Items.AddRange(SerialPort.GetPortNames());
            FrequencyComComboBox.Items.AddRange(SerialPort.GetPortNames());
            PressureComComboBox.Items.AddRange(SerialPort.GetPortNames());
            ValveComComboBox.Items.AddRange(SerialPort.GetPortNames());
            VibrationComComboBox.Items.AddRange(SerialPort.GetPortNames());
        }
        public void GetUnitsOfMeasure()
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
        }
        internal void ParametersLoadXML()
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
        }
        internal void ParametersSaveXML()
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
        }
        internal void UnitLoadXML()
        {
            xSettings = xDefaultDoc.Element("settings");

            foreach (XElement el in xSettings.Elements("unit"))
            {
                if (el.Attribute("Name").Value == "Flowmeter")
                    FlowMeter.LoadSettingsXML(el, ref FlowComComboBox, ref FlowSpeedTextBox, ref FlowAddressTextBox);
                if (el.Attribute("Name").Value == "FrequencyChanger")
                    FrequencyChanger.LoadSettingsXML(el, ref FrequencyComComboBox, ref FrequencySpeedTextBox, ref FrequencyAddressTextBox);
                if (el.Attribute("Name").Value == "PressureGauges")
                    PressureGauges.LoadSettingsXML(el, ref PressureComComboBox, ref PressureSpeedTextBox, ref PressureAddressTextBox);
                if (el.Attribute("Name").Value == "Valve")
                    Valve.LoadSettingsXML(el, ref ValveComComboBox, ref ValveSpeedTextBox, ref ValveAddressTextBox);
                if (el.Attribute("Name").Value == "Vibration")
                    Vibration.LoadSettingsXML(el, ref VibrationComComboBox, ref VibrationSpeedTextBox, ref VibrationAddresTextBox);
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
            GetUnitsOfMeasure();

            xDefaultDoc = XDocument.Load("Defaults.xml");
            try
            {
                UnitLoadXML();
                ParametersLoadXML();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка в файле настроек");
            }
            }
        #region Расходомер
        private void FlowSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                FlowMeter.SetParameters(FlowComComboBox.Text, Convert.ToInt32(FlowSpeedTextBox.Text),
                    Convert.ToInt32(FlowAddressTextBox.Text));
                FlowMeter.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
        }

        private void FlowAdditionalButton_Click(object sender, EventArgs e)
        {
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref FlowMeter);
            addsett.Show();
        }

        private void FlowConnectButton_Click(object sender, EventArgs e)
        {
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
            }
        }

        #endregion
        #region Частотник
        private void FrequencyAdditionalSettings_Click(object sender, EventArgs e)
        {
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref FrequencyChanger);
            addsett.Show();
        }

        private void FrequencySaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                FrequencyChanger.SetParameters(FrequencyComComboBox.Text, Convert.ToInt32(FrequencySpeedTextBox.Text),
                    Convert.ToInt32(FrequencyAddressTextBox.Text));
                FrequencyChanger.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
        }

        private void FrequencyConnectButton_Click(object sender, EventArgs e)
        {
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
            }
        }
        #endregion
        #region Датчик давления
        private void PressureAdditionalButton_Click(object sender, EventArgs e)
        {
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref PressureGauges);
            addsett.Show();
        }

        private void PressureConnectButton_Click(object sender, EventArgs e)
        {
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
            }
        }

        private void PressureSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                PressureGauges.SetParameters(PressureComComboBox.Text, Convert.ToInt32(PressureSpeedTextBox.Text),
                    Convert.ToInt32(PressureAddressTextBox.Text));
                PressureGauges.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
        }

        #endregion
        #region Задвижка
        private void ValveAdditionalButton_Click(object sender, EventArgs e)
        {
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref Valve);
            addsett.Show();
        }
        private void ValveConnectButton_Click(object sender, EventArgs e)
        {
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
            }
        }
        private void ValveSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Valve.SetParameters(ValveComComboBox.Text, Convert.ToInt32(ValveSpeedTextBox.Text),
                    Convert.ToInt32(ValveAddressTextBox.Text));
                Valve.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
        }
        #endregion
        #region Датчик вибрации
        private void VibrationAdditionalSettings_Click(object sender, EventArgs e)
        {
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref Vibration);
            addsett.Show();
        }

        private void VibrationConnectButton_Click(object sender, EventArgs e)
        {
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
            }
        }

        private void VibrationSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Vibration.SetParameters(VibrationComComboBox.Text, Convert.ToInt32(VibrationSpeedTextBox.Text),
                    Convert.ToInt32(VibrationAddresTextBox.Text));
                Vibration.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
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
                float fRegs = un.ComRead(ref par, 0);
                Action read = () => VarTimeChart.Series[series+1].Points.AddXY((DateTime.Now - start).Seconds, fRegs);
                Action add = () => VarTimeChart.Series[series].Points.AddXY((DateTime.Now - start).Seconds, fRegs);
                Action clear = () => VarTimeChart.Series[series+1].Points.Clear();
                if (InvokeRequired)
                {
                    Invoke(clear);
                    Invoke(read);
                }
                else
                {
                    clear();
                    read();
                }
                if (_stopper.WaitOne(wait, false))
                {
                    if (stopthread)
                        break;
                    par.SetMeasuredRegs(fRegs);
                    if (InvokeRequired)
                        Invoke(add);
                    else
                        add();
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
                ParametersSaveXML();
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
            }*/
            if (PressureGauges.isConnected)
            {
                //ushort address = 49161;
                //int offset = 0;
                //bool isReversed = false;
                Pressure1.MeasuredRegs.Clear();
                VarTimeChart.Series[2].Points.Clear();
                string UoM = Pressure1.GetUoMstring();
                VarTimeChart.Series[2].Name = UoM;
                VarTimeChart.Series[3].Name = UoM + " мгновенный";
                var threadParameters = new System.Threading.ThreadStart(delegate
                { TestChart(PressureGauges, ref Pressure1, 2); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stopthread = true;
            _stopper.Set();
        }

        private void UoMRedactorButton_Click(object sender, EventArgs e)
        {
            UoMRedactor UomRed = new UoMRedactor(ref ParameterDictionary,this);
            UomRed.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
        internal void TEST()
        {
            while (true)
            {
                float flow = FlowMeter.ComRead(ref Flow, 0);
                float p1 = PressureGauges.ComRead(ref Pressure1, 0);
                float p8 = PressureGauges.ComRead(ref Pressure8, 0);
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
        }
        private void button4_Click(object sender, EventArgs e)
        {
            _stopper.Reset();
            if (true || (FlowMeter.isConnected && PressureGauges.isConnected && FrequencyChanger.isConnected))//поменять
            {
                stopthread = false;
                var threadParameters = new System.Threading.ThreadStart(delegate
                { TEST(); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            stopthread = true;
            _stopper.Set();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
    }
}