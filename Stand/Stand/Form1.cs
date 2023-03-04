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

namespace Stand
{
    public partial class Form1 : Form
    {
        private static ManualResetEvent _stopper = new ManualResetEvent(false);
        public Form1()
        {
            InitializeComponent();
        }

        private XDocument xDefaultDoc = new XDocument();
        public static XElement xSettings = new XElement("settings");
        internal bool IsRegistersAdditionalToggled = false;

        #region Инициализация устройств и параметров
        public Unit FlowMeter = new Unit("Flowmeter");
        public Unit FrequencyChanger = new Unit("FrequencyChanger");
        public Unit PressureGauges = new Unit("PressureGauges");
        public Unit Valve = new Unit("Valve");
        public Unit Vibration = new Unit("Vibration");

        private Parameter Flow = new Parameter("Flow", new string[] { "л/ч", "л/сут", "м3/час", "м3/сут" });
        private Parameter Current = new Parameter("Current", new string[] { "А" });
        private Parameter CurrentFrequency = new Parameter("CurrentFrequency", new string[] { "Гц" });
        private Parameter Voltage = new Parameter("Voltage", new string[] { "В" });
        private Parameter Torque = new Parameter("Torque", new string[] { "Н*м" });
        private Parameter Power = new Parameter("Power", new string[] { "Вт","кВт" });
        private Parameter RPM = new Parameter("RPM", new string[] { "об/мин" });
        private Parameter EngineLoad = new Parameter("EngineLoad", new string[] { "%" });
        private Parameter XAmplitude = new Parameter("XAmplitude", new string[] { "мм/с" });
        private Parameter YAmplitude = new Parameter("YAmplitude", new string[] { "мм/с" });
        private Parameter Pressure1 = new Parameter("Pressure1", new string[] { "Па","кПа","МПа","атм" });
        private Parameter Pressure2 = new Parameter("Pressure2", new string[] { "Па", "кПа", "МПа", "атм" });
        private Parameter Pressure3 = new Parameter("Pressure3", new string[] { "Па", "кПа", "МПа", "атм" });
        private Parameter Pressure4 = new Parameter("Pressure4", new string[] { "Па", "кПа", "МПа", "атм" });
        private Parameter Pressure5 = new Parameter("Pressure5", new string[] { "Па", "кПа", "МПа", "атм" });
        private Parameter Pressure6 = new Parameter("Pressure6", new string[] { "Па", "кПа", "МПа", "атм" });
        private Parameter Pressure7 = new Parameter("Pressure7", new string[] { "Па", "кПа", "МПа", "атм" });
        private Parameter Pressure8 = new Parameter("Pressure8", new string[] { "Па", "кПа", "МПа", "атм" });
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
        internal void GetUnitsOfMeasure()
        {
            PFlowСomboBox.Items.AddRange(Flow.UnitsOfMeasure);
            PCurrentСomboBox.Items.AddRange(Current.UnitsOfMeasure);
            PCurrentFreqСomboBox.Items.AddRange(CurrentFrequency.UnitsOfMeasure);
            PVoltageСomboBox.Items.AddRange(Voltage.UnitsOfMeasure);
            PTorqueСomboBox.Items.AddRange(Torque.UnitsOfMeasure);
            PPowerСomboBox.Items.AddRange(Power.UnitsOfMeasure);
            PRPMСomboBox.Items.AddRange(RPM.UnitsOfMeasure);
            PLoadСomboBox.Items.AddRange(EngineLoad.UnitsOfMeasure);
            PXAmplitudeСomboBox.Items.AddRange(XAmplitude.UnitsOfMeasure);
            PYAmplitydeСomboBox.Items.AddRange(YAmplitude.UnitsOfMeasure);
            PPressureСomboBox.Items.AddRange(Pressure1.UnitsOfMeasure);
        }
        internal void ParametersLoadXML()
        {
            xSettings = xDefaultDoc.Element("settings");
            foreach (XElement el in xSettings.Elements("parameter"))
            {
                if (el.Attribute("Name").Value == "Flow")
                {
                    Flow.LoadXML(el, ref PFlowRegisterTextBox, ref PFlowСomboBox);
                }
                if (el.Attribute("Name").Value == "Current")
                {
                    Current.LoadXML(el, ref PCurrentTextBox, ref PCurrentСomboBox);
                }
                if (el.Attribute("Name").Value == "CurrentFrequency")
                {
                    CurrentFrequency.LoadXML(el, ref PCurrentFreqTextBox, ref PCurrentFreqСomboBox);
                }
                if (el.Attribute("Name").Value == "Voltage")
                {
                    Voltage.LoadXML(el, ref PVoltageTextBox, ref PVoltageСomboBox);
                }
                if (el.Attribute("Name").Value == "Torque")
                {
                    Torque.LoadXML(el, ref PTorqueTextBox, ref PTorqueСomboBox);
                }
                if (el.Attribute("Name").Value == "Power")
                {
                    Power.LoadXML(el, ref PPowerTextBox, ref PPowerСomboBox);
                }
                if (el.Attribute("Name").Value == "RPM")
                {
                    RPM.LoadXML(el, ref PRPMTextBox, ref PRPMСomboBox);
                }
                if (el.Attribute("Name").Value == "EngineLoad")
                {
                    EngineLoad.LoadXML(el, ref PLoadTextBox, ref PLoadСomboBox);
                }
                if (el.Attribute("Name").Value == "XAmplitude")
                {
                    XAmplitude.LoadXML(el, ref PXAmplitudeTextBox, ref PXAmplitudeСomboBox);
                }
                if (el.Attribute("Name").Value == "YAmplitude")
                {
                    YAmplitude.LoadXML(el, ref PYAmplitudeTextBox, ref PYAmplitydeСomboBox);
                }
                if (el.Attribute("Name").Value == "Pressure1")
                {
                    Pressure1.LoadXML(el, ref PPressure1TextBox, ref PPressureСomboBox, ref PPressure1CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure2")
                {
                    Pressure2.LoadXML(el, ref PPressure2TextBox, ref PPressureСomboBox, ref PPressure2CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure3")
                {
                    Pressure3.LoadXML(el, ref PPressure3TextBox, ref PPressureСomboBox, ref PPressure3CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure4")
                {
                    Pressure4.LoadXML(el, ref PPressure4TextBox, ref PPressureСomboBox, ref PPressure4CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure5")
                {
                    Pressure5.LoadXML(el, ref PPressure5TextBox, ref PPressureСomboBox, ref PPressure5CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure6")
                {
                    Pressure6.LoadXML(el, ref PPressure6TextBox, ref PPressureСomboBox, ref PPressure6CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure7")
                {
                    Pressure7.LoadXML(el, ref PPressure7TextBox, ref PPressureСomboBox, ref PPressure7CheckBox);
                }
                if (el.Attribute("Name").Value == "Pressure8")
                {
                    Pressure8.LoadXML(el, ref PPressure8TextBox, ref PPressureСomboBox, ref PPressure8CheckBox);
                }
            }
        }
        internal void ParametersSaveXML()
        {
            Flow.SaveXML(ref PFlowRegisterTextBox, ref PFlowСomboBox);
            Current.SaveXML(ref PCurrentTextBox, ref PCurrentСomboBox);
            CurrentFrequency.SaveXML(ref PCurrentFreqTextBox, ref PCurrentFreqСomboBox);
            Voltage.SaveXML(ref PVoltageTextBox, ref PVoltageСomboBox);
            Torque.SaveXML(ref PTorqueTextBox, ref PTorqueСomboBox);
            Power.SaveXML(ref PPowerTextBox, ref PPowerСomboBox);
            RPM.SaveXML(ref PRPMTextBox, ref PRPMСomboBox);
            EngineLoad.SaveXML(ref PLoadTextBox, ref PLoadСomboBox);
            XAmplitude.SaveXML(ref PXAmplitudeTextBox, ref PXAmplitudeСomboBox);
            YAmplitude.SaveXML(ref PYAmplitudeTextBox, ref PYAmplitydeСomboBox);
            Pressure1.SaveXML(ref PPressure1TextBox, ref PPressureСomboBox, ref PPressure1CheckBox);
            Pressure2.SaveXML(ref PPressure2TextBox, ref PPressureСomboBox, ref PPressure2CheckBox);
            Pressure3.SaveXML(ref PPressure3TextBox, ref PPressureСomboBox, ref PPressure3CheckBox);
            Pressure4.SaveXML(ref PPressure4TextBox, ref PPressureСomboBox, ref PPressure4CheckBox);
            Pressure5.SaveXML(ref PPressure5TextBox, ref PPressureСomboBox, ref PPressure5CheckBox);
            Pressure6.SaveXML(ref PPressure6TextBox, ref PPressureСomboBox, ref PPressure6CheckBox);
            Pressure7.SaveXML(ref PPressure7TextBox, ref PPressureСomboBox, ref PPressure7CheckBox);
            Pressure8.SaveXML(ref PPressure8TextBox, ref PPressureСomboBox, ref PPressure8CheckBox);

            xSettings.Save("Defaults.xml");
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
                            MessageBox.Show("Подключено новое устройство");
                        }
                        break;
                    case 0x8004:
                        //DBT_DEVICEREMOVECOMPLETE = 0x8004
                        dbh = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbh.dbch_devicetype == 0x00000003)
                        {
                            GetComPorts();
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
            xSettings = xDefaultDoc.Element("settings");
            try
            {
                ParametersLoadXML();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка в файле настроек (параметры)");
            }

            foreach (XElement el in xSettings.Elements("unit"))
            {
                if (el.Attribute("Name").Value == "Flowmeter")
                    FlowMeter.LoadSettingsXML(el,ref FlowComComboBox, ref FlowSpeedTextBox, ref FlowAddressTextBox);
                if (el.Attribute("Name").Value == "FrequencyChanger")
                    FrequencyChanger.LoadSettingsXML(el, ref FrequencyComComboBox, ref FrequencySpeedTextBox,ref FrequencyAddressTextBox);
                if (el.Attribute("Name").Value == "PressureGauges")
                    PressureGauges.LoadSettingsXML(el, ref PressureComComboBox, ref PressureSpeedTextBox, ref PressureAddressTextBox);
                if (el.Attribute("Name").Value == "Valve")
                    Valve.LoadSettingsXML(el,ref ValveComComboBox, ref ValveSpeedTextBox,ref ValveAddressTextBox);
                if (el.Attribute("Name").Value == "Vibration")
                    Vibration.LoadSettingsXML(el,ref VibrationComComboBox,ref VibrationSpeedTextBox, ref VibrationAddresTextBox);
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            if (FlowMeter.isConnected)
            {
                //ushort address = 49161;
                int offset = 0;
                bool isReversed = false;

                var threadParameters = new System.Threading.ThreadStart(delegate 
                { TestReadInput(FlowMeter, listBox1, ref Flow, isReversed, offset); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
            if (PressureGauges.isConnected)
            {

                //ushort address = 4; // регистры 4,10,16,22,28,34,40,46
                int offset = 0;
                bool isReversed = false;

                if (Pressure1.CheckIfToggled())
                {
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { TestReadInput(PressureGauges, listBox6, ref Pressure1, isReversed, offset); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
                if (Pressure8.CheckIfToggled())
                {
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { TestReadInput(PressureGauges, listBox7, ref Pressure8, isReversed, offset); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
            }
            if (FrequencyChanger.isConnected)
            {
                //ushort address = 16479; // N меню * 10 - 1
                int offset = 1;
                bool isReversed = true;
                {
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { TestReadHolding(FrequencyChanger, listBox2, ref RPM, isReversed, offset); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
                {
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { TestReadHolding(FrequencyChanger, listBox3, ref Voltage, isReversed, offset); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
            }
            if (Vibration.isConnected)
            {
                //ushort address = 16479;
                int offset = 0;
                bool isReversed = false;

                {
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { TestReadInput(Vibration, listBox4, ref XAmplitude, isReversed, offset); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
                {
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { TestReadInput(Vibration, listBox5, ref YAmplitude, isReversed, offset); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
    }
}