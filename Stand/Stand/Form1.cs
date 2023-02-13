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

        #region Инициализация устройств
        public Unit FlowMeter = new Unit("Flowmeter");
        public Unit FrequencyChanger = new Unit("FrequencyChanger");
        public Unit PressureGauges = new Unit("PressureGauges");
        public Unit Valve = new Unit("Valve");
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            FlowComComboBox.Items.AddRange(SerialPort.GetPortNames());
            FrequencyComComboBox.Items.AddRange(SerialPort.GetPortNames());
            PressureComComboBox.Items.AddRange(SerialPort.GetPortNames());
            ValveComComboBox.Items.AddRange(SerialPort.GetPortNames());

            FlowAddressOffsetComboBox.SelectedIndex = 1;
            FrequencyAddressOffsetComboBox.SelectedIndex = 1;
            PressureAddressOffsetComboBox.SelectedIndex = 1;
            ValveAddressOffsetComboBox.SelectedIndex = 1;

            xDefaultDoc = XDocument.Load("Defaults.xml");
            xSettings = xDefaultDoc.Element("settings");
            foreach (XElement el in xSettings.Elements("unit"))
            {
                if (el.Attribute("Name").Value == "Flowmeter")
                {
                    FlowMeter.LoadSettingsXML(el);
                    string name = "";
                    string baudrate = "";
                    string address = "";
                    string offset = "";
                    FlowMeter.GetParametersString(ref name, ref baudrate, ref address, ref offset);
                    FlowComComboBox.SelectedIndex = FlowComComboBox.FindStringExact(name);
                    FlowSpeedTextBox.Text = baudrate;
                    FlowAddressTextBox.Text = address;
                    FlowAddressOffsetComboBox.SelectedIndex = FlowAddressOffsetComboBox.FindStringExact(offset);
                }
                if (el.Attribute("Name").Value == "FrequencyChanger")
                {
                    FrequencyChanger.LoadSettingsXML(el);
                    string name = "";
                    string baudrate = "";
                    string address = "";
                    string offset = "";
                    FrequencyChanger.GetParametersString(ref name, ref baudrate, ref address, ref offset);
                    FrequencyComComboBox.SelectedIndex = FlowComComboBox.FindStringExact(name);
                    FrequencySpeedTextBox.Text = baudrate;
                    FrequencyAddressTextBox.Text = address;
                    FrequencyAddressOffsetComboBox.SelectedIndex = FrequencyAddressOffsetComboBox.FindStringExact(offset);
                }
                if (el.Attribute("Name").Value == "PressureGauges")
                {
                    PressureGauges.LoadSettingsXML(el);
                    string name = "";
                    string baudrate = "";
                    string address = "";
                    string offset = "";
                    PressureGauges.GetParametersString(ref name, ref baudrate, ref address, ref offset);
                    PressureComComboBox.SelectedIndex = PressureComComboBox.FindStringExact(name);
                    PressureSpeedTextBox.Text = baudrate;
                    PressureAddressTextBox.Text = address;
                    PressureAddressOffsetComboBox.SelectedIndex = PressureAddressOffsetComboBox.FindStringExact(offset);
                }
                if (el.Attribute("Name").Value == "Valve")
                {
                    Valve.LoadSettingsXML(el);
                    string name = "";
                    string baudrate = "";
                    string address = "";
                    string offset = "";
                    Valve.GetParametersString(ref name, ref baudrate, ref address, ref offset);
                    ValveComComboBox.SelectedIndex = ValveComComboBox.FindStringExact(name);
                    ValveSpeedTextBox.Text = baudrate;
                    ValveAddressTextBox.Text = address;
                    ValveAddressOffsetComboBox.SelectedIndex = ValveAddressOffsetComboBox.FindStringExact(offset);
                }
            }

        }
        /// Расходомер
        #region Расходомер
        private void FlowSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                FlowMeter.SetParameters(FlowComComboBox.Text, Convert.ToInt32(FlowSpeedTextBox.Text),
                    Convert.ToInt32(FlowAddressTextBox.Text), Convert.ToInt32(FlowAddressOffsetComboBox.Text));
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
        /// Частотник
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
                    Convert.ToInt32(FrequencyAddressTextBox.Text), Convert.ToInt32(FrequencyAddressOffsetComboBox.Text));
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
        /// Датчик давления
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
                    Convert.ToInt32(PressureAddressTextBox.Text), Convert.ToInt32(PressureAddressOffsetComboBox.Text));
                PressureGauges.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
        }

        #endregion
        /// Задвижка
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
                    Convert.ToInt32(ValveAddressTextBox.Text), Convert.ToInt32(ValveAddressOffsetComboBox.Text));
                Valve.SaveSettingsXML();
                MessageBox.Show("Настройки успешно сохранены");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось сохранить настройки");
            }
        }
        #endregion


        public int i = 0;
        internal void TestReadHolding(Unit un, ListBox l,ushort address, ushort nofpoints, bool isReversed)
        {
            int wait = 1000;
            while (true)
            {
                float[] fRegs = un.ComReadHolding(address, nofpoints, isReversed);
                if (nofpoints == 1)
                {
                    Action read = () => l.Items.Add($"{fRegs[0]} {DateTime.Now}");
                    if (InvokeRequired)
                        Invoke(read);
                    else
                        read();
                }
                if (nofpoints == 2)
                {
                    Action read = () => l.Items.Add($"{fRegs[0]} {fRegs[1]} {DateTime.Now}");
                    if (InvokeRequired)
                        Invoke(read);
                    else
                        read();
                }
                if (_stopper.WaitOne(wait, false))  
                {                                  
                    break;
                }
            }
        }
        internal void TestReadInput(Unit un, ListBox l, ushort address, ushort nofpoints, bool isReversed)
        {
            int wait = 1000;
            while (true)
            {
                float[] fRegs = un.ComReadHolding(address, nofpoints, isReversed);
                if (nofpoints == 1)
                {
                    Action read = () => l.Items.Add($"{fRegs[0]} {DateTime.Now}");
                    if (InvokeRequired)
                        Invoke(read);
                    else
                        read();
                }
                if (nofpoints == 2)
                {
                    Action read = () => l.Items.Add($"{fRegs[0]} {fRegs[1]} {DateTime.Now}");
                    if (InvokeRequired)
                        Invoke(read);
                    else
                        read();
                }
                if (_stopper.WaitOne(wait, false)) 
                {                             
                    break;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (FlowMeter.isConnected)
            {
                ushort address = 49161;
                ushort nofpoints = 2;
                bool isReversed = false;

                var threadParameters = new System.Threading.ThreadStart(delegate { TestReadInput(FlowMeter, listBox1,address,nofpoints,isReversed); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
            if (PressureGauges.isConnected)
            {

                ushort address = 49161;
                ushort nofpoints = 2;
                bool isReversed = false;

                var threadParameters = new System.Threading.ThreadStart(delegate { TestReadInput(PressureGauges, listBox2, address, nofpoints, isReversed); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
            if (FrequencyChanger.isConnected)
            {
                ushort address = 49161; // посмотреть формулу расчета
                ushort nofpoints = 2;
                bool isReversed = false;

                var threadParameters = new System.Threading.ThreadStart(delegate { TestReadInput(FrequencyChanger, listBox3, address, nofpoints, isReversed); });
                var thread = new System.Threading.Thread(threadParameters);
                thread.Start();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
    }
}
