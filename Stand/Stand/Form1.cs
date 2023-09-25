using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using Application = System.Windows.Forms.Application;
using ClosedXML.Excel;
using Color = System.Drawing.Color;

namespace Stand
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static ManualResetEvent _stopper = new ManualResetEvent(false);
        private static ManualResetEvent _waiter = new ManualResetEvent(false);
        private bool stopthread = false;

        public XDocument xDefaultDoc = new XDocument();
        public static XElement xSettings = new XElement("settings");
        private bool IsRegistersAdditionalToggled = false;
        private bool IsExtended = false;
        private PerformanceTableOverlay Overlay;
        private DataTable PerformanceDataTable;
        private List<float> TimestampList = new List<float>();

        #region Инициализация устройств и параметров
        public List<Unit> UnitsList = new List<Unit>();
        public List<Scheme> SchemasList = new List<Scheme>();
        public Scheme SelectedScheme;
        PostgreSQLib postgreSQL = new PostgreSQLib();

        public double SiPressureCoefficient = 1;
        public double SiFlowCoefficient = 1;
        public double SiPowerCoefficient = 1;
        private string ExperimentName;

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
            RegTestListBox.Items.Clear();

            foreach (Parameter par in un.GetParametersList())
            {
                if (par.CheckIfToggled())
                    ParametersListBox.Items.Add(par.GetName() + " (считывается)");
                else
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
            OffsetTextBox.Text = par.GetOffset().ToString();
            ParameterRegTypeComboBox.SelectedIndex = (int)par.GetRegType();
            ParameterTypeComboBox.SelectedIndex = (int)par.GetDataType();
            ParameterReadableCheckBox.Checked = par.CheckIfToggled();
            ParameterUoMComboBox.Items.Clear();
            ParameterUoMComboBox.Items.AddRange(par.GetUoMs().Keys.ToArray());
            if (ParameterUoMComboBox.Items.Count > 0)
                ParameterUoMComboBox.SelectedIndex = par.GetSelectedUoM();
            PIDLabel.Text = par.id.ToString();
            ParameterChangesNotSavedLabel.Visible = false;
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
                    DensityTextBox, L1TextBox, L2TextBox, L3TextBox, L4TextBox, DiameterTextBox, FCStepTextBox, ValveStepTextBox);
                SchemeNotSavedLabel.Visible = false;
            }
            SchemeComboBox.Items.Clear();
            foreach (var sc in SchemasList)
                SchemeComboBox.Items.Add(sc.name);
        }
        private void GetComPorts()
        {
            UnitComComboBox.Items.Clear();
            UnitComComboBox.Items.AddRange(SerialPort.GetPortNames());
        }
        internal void UnitLoadXML()
        {
            UnitsList.Clear();
            xSettings = xDefaultDoc.Element("settings");
            IEnumerable<XElement> pars = xSettings.Elements("parameter");
            foreach (XElement un in xSettings.Elements("unit"))
            {
                UnitsList.Add(new Unit(un, pars));
            }
            UnitsList.Sort((x, y) => x.id.CompareTo(y.id));
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
            if (m.Msg == 0x219)
            {
                //WM_DEVICECHANGE = 0x219
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

        private void SaveUnitSettings()
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
        private void UnitSaveButton_Click(object sender, EventArgs e)
        {
            SaveUnitSettings();
        }

        private void UnitConnectButton_Click(object sender, EventArgs e)
        {
            SaveUnitSettings();
            int selected = UnitsListBox.SelectedIndex;
            Unit un = UnitsList[selected];
            if (un.isConnected)
                un.Disconnect();
            else
                un.TryToConnect();

            UpdateUnitsList(selected);
            UpdateSelectedUnit();
        }

        private void UnitConnectAllButton_Click(object sender, EventArgs e)
        {
            int selected = UnitsListBox.SelectedIndex;
            foreach (Unit un in UnitsList)
            {
                if (un.isConnected)
                    un.Disconnect();
                else
                    un.TryToConnect();
            }
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
                    Convert.ToUInt16(OffsetTextBox.Text), ParameterUoMComboBox.SelectedIndex, ParameterReadableCheckBox.Checked,
                    (RegType)ParameterRegTypeComboBox.SelectedIndex, (DataType)ParameterTypeComboBox.SelectedIndex);
                par.SaveXML();
                xSettings.Save("Defaults.xml");
                UpdateUnitsList(selected);
                MessageBox.Show("Настройки успешно сохранены");
                ParameterChangesNotSavedLabel.Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Введены некорректные параметры");
            }
        }
        private void ParameterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ParameterChangesNotSavedLabel.Visible = true;
        }
        private void ParameterRegAddressTextBox_TextChanged(object sender, EventArgs e)
        {
            ParameterChangesNotSavedLabel.Visible = true;
        }
        private void ParameterReadableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ParameterChangesNotSavedLabel.Visible = true;
        }
        private void OffsetTextBox_TextChanged(object sender, EventArgs e)
        {
            ParameterChangesNotSavedLabel.Visible = true;
        }
        private void OffsetTextBox_TextChanged_1(object sender, EventArgs e)
        {
            ParameterChangesNotSavedLabel.Visible = true;
        }
        /// <summary>
        /// 
        /// </summary>

        private void RegTest(Unit un, Parameter par)
        {
            while (!stopthread)
            {
                un.ComRead(par, 0);
                Action add = () => RegTestListBox.Items.Add(par.GetLastMeasuredRegs());
                if (InvokeRequired)
                    Invoke(add);
                else
                    add();
            }
        }

        private void RegTestToggleButton_Click(object sender, EventArgs e)
        {
            Unit un = UnitsList[UnitsListBox.SelectedIndex];
            Parameter par = un.GetParametersList()[ParametersListBox.SelectedIndex];
            if (!un.isConnected)
                MessageBox.Show("Не установлено соединение с устройством");
            else
            {
                if (RegTestToggleButton.Text == "Начать")
                {
                    RegTestListBox.Items.Clear();
                    RegTestToggleButton.Text = "Остановить";
                    stopthread = false;
                    var threadParameters = new System.Threading.ThreadStart(delegate
                    { RegTest(un, par); });
                    var thread = new System.Threading.Thread(threadParameters);
                    thread.Start();
                    return;
                }
            }
            if (RegTestToggleButton.Text == "Остановить")
            {
                RegTestToggleButton.Text = "Начать";
                stopthread = true;
            }
            return;
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

        private void SchemeDeleteButton_Click(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
            {
                SchemasList[SchemeComboBox.SelectedIndex].DeleteXML();
                SchemasList.RemoveAt(SchemeComboBox.SelectedIndex);
            }
            SchemeComboBox.Items.Clear();
            foreach (var sc in SchemasList)
                SchemeComboBox.Items.Add(sc.name);
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
            try
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
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ValveUnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void FlowmeterUnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void SchemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedScheme = SchemasList.Find(sc => sc.SchemeN == SchemeComboBox.SelectedIndex);
                if (SelectedScheme != null)
                {
                    SchemeNameTextBox.Text = SelectedScheme.name;
                    DensityTextBox.Text = SelectedScheme.Density.ToString();
                    L1TextBox.Text = SelectedScheme.L[1].ToString();
                    L2TextBox.Text = SelectedScheme.L[2].ToString();
                    L3TextBox.Text = SelectedScheme.L[3].ToString();
                    L4TextBox.Text = SelectedScheme.L[4].ToString();
                    FCStepTextBox.Text = SelectedScheme.FCStep.ToString();
                    ValveStepTextBox.Text = SelectedScheme.ValveStep.ToString();
                    FrequencyPlusButton.Text = $"+{SelectedScheme.FCStep}";
                    FrequencyMinusButton.Text = $"-{SelectedScheme.FCStep}";
                    ValvePosPlusButton.Text = $"+{SelectedScheme.ValveStep}";
                    ValvePosMinusButton.Text = $"-{SelectedScheme.ValveStep}";
                    DiameterTextBox.Text = SelectedScheme.D.ToString();

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
                        FCParPowerComboBox.SelectedIndex = 0;
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
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void FCParPowerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (SchemeComboBox.SelectedIndex != -1)
                    SchemeNotSavedLabel.Visible = true; // лобавить для остальных
                if (FCParPowerComboBox.SelectedIndex != 0)
                {
                    PowerCustomLabel.Text = UnitsList.Find(u => u.GetName() == FCUnitComboBox.Text).GetParametersList()
                        .Find(p => p.GetName() == FCParPowerComboBox.Text).GetUoMstring();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void FlowmeterParComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (SchemeComboBox.SelectedIndex != -1)
                    SchemeNotSavedLabel.Visible = true;
                if (FlowmeterParComboBox.SelectedIndex != 0)
                {
                    FlowCustomLabel.Text = UnitsList.Find(u => u.GetName() == FlowmeterUnitComboBox.Text).GetParametersList()
                        .Find(p => p.GetName() == FlowmeterParComboBox.Text).GetUoMstring();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void PressureParComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (SchemeComboBox.SelectedIndex != -1)
                    SchemeNotSavedLabel.Visible = true;
                if (PressureParComboBox1.SelectedIndex != 0)
                {
                    PressureCustomLabel.Text = UnitsList.Find(u => u.GetName() == PressureUnitСomboBox.Text).GetParametersList()
                        .Find(p => p.GetName() == PressureParComboBox1.Text).GetUoMstring();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void PressureParComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SchemeComboBox.SelectedIndex != -1)
                SchemeNotSavedLabel.Visible = true;
        }
        private void ValveParComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (SchemeComboBox.SelectedIndex != -1)
                    SchemeNotSavedLabel.Visible = true;

            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void L4TextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SchemeComboBox.SelectedIndex != -1)
                    SchemeNotSavedLabel.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }
        private void FCStepTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (SchemeComboBox.SelectedIndex != -1)
                    SchemeNotSavedLabel.Visible = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }

        #endregion
        #region РНХ
        ManualResetEvent StartReadingEvent = new ManualResetEvent(false);
        ManualResetEvent AddEvent = new ManualResetEvent(false);
        bool IsFCPlus = false;
        bool IsFCMinus = false;
        bool IsValvePlus = false;
        bool IsValveMinus = false;
        private void FrequencyPlusButton_Click(object sender, EventArgs e)
        {
            IsFCPlus = true;
            IsFCMinus = false;
        }
        private void FrequencyMinusButton_Click(object sender, EventArgs e)
        {
            IsFCMinus = true;
            IsFCPlus = false;
        }
        private void ValvePosMinusButton_Click(object sender, EventArgs e)
        {
            IsValveMinus = true;
            IsValvePlus = false;
        }
        private void ValvePosPlusButton_Click(object sender, EventArgs e)
        {
            IsValvePlus = true;
            IsValveMinus = false;
        }

        private void UnitParametersRead(Unit un, ManualResetEvent _finished)
        {
            while (true)
            {
                StartReadingEvent.WaitOne();
                Thread.Sleep(500);
                if (stopthread)
                    break;
                un.ReadAllParams();
                _finished.Set();
                StartReadingEvent.Reset();
            }
        }

        private bool MakePressureParametersList(ref List<Parameter> PressureParametersList)
        {
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
                return false;
                //IsStartPossible = false;
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
            return true;
        }

        private void MakeARow(ref DataRow row, float timespan, float Q, float N, float[] H, float[] ECE)
        {
            row[0] = 0;
            row[1] = timespan;
            row[2] = Q;
            row[3] = N;
            for (int i = 0; i < H.Length; i++)
                row[i + 4] = H[i];
            for (int i = 0; i < H.Length; i++)
                row[i + 4 + H.Length] = ECE[i];
            if (IsExtended)
            {
                int i = 0;
                foreach (var unit in UnitsList)
                {
                    foreach (var parameter in unit.GetParametersList())
                    {
                        if (parameter.CheckIfToggled())
                        {
                            row[3 + H.Length + ECE.Length + i] = parameter.GetLastMeasuredRegs();
                            i++;
                        }
                    }
                }
            }
        }

        private void PerformanceParametersReadout(ManualResetEvent[] FinishedReadingEventList,
            ref List<Parameter> PressureParametersList, ref DataTable PerformanceDataTable)
        {
            DataRow row = PerformanceDataTable.NewRow();
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();

            bool IsStartPossible = true;
            //List<Parameter> PressureParametersList = new List<Parameter>();
            Parameter FCPowerParameter = null;
            float PowerLastReg = 0;
            Parameter FlowParameter = null;
            float FlowLastReg = 0;

            Unit FC = UnitsList.Find(u => u.id == SelectedScheme.FCUID);
            Parameter FCChangableParameter = null;
            Unit Valve = UnitsList.Find(u => u.id == SelectedScheme.ValveUID);
            Parameter ValveChangableParameter = null;
            //IsStartPossible = MakePressureParametersList(ref PressureParametersList);
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
            if (SelectedScheme.FCPPID != -1)
            {
                FCChangableParameter = FC.GetParametersList().Find(par => par.id == SelectedScheme.FCPPID);
            }
            if (SelectedScheme.ValvePID != -1)
            {
                ValveChangableParameter = Valve.GetParametersList().Find(par => par.id == SelectedScheme.ValvePID);
            }

            float[] H = new float[PressureParametersList.Count() - 1];
            float[] ECE = new float[PressureParametersList.Count() - 1];
            StartReadingEvent.Set();

            // InitDataTable
            DataColumn checkcol = new DataColumn();
            checkcol.ColumnName = $"!";
            PerformanceDataTable.Columns.Add(checkcol);

            DataColumn Timecol = new DataColumn();
            Timecol.ColumnName = $"T [сек]";
            PerformanceDataTable.Columns.Add(Timecol);

            DataColumn Flowcol = new DataColumn();
            Flowcol.ColumnName = $"Q [{FlowParameter.GetUoMstring()}]";
            PerformanceDataTable.Columns.Add(Flowcol);

            DataColumn Ncol = new DataColumn();
            Ncol.ColumnName = $"N [{FCPowerParameter.GetUoMstring()}]";
            PerformanceDataTable.Columns.Add(Ncol);

            DataColumn[] Hcol = new DataColumn[H.Length];
            for (int i = 0; i < Hcol.Length; i++)
            {
                Hcol[i] = new DataColumn();
                Hcol[i].ColumnName = $"H {i + 1} [м]";
            }
            PerformanceDataTable.Columns.AddRange(Hcol);

            DataColumn[] ECEcol = new DataColumn[ECE.Length];
            for (int i = 0; i < ECEcol.Length; i++)
            {
                ECEcol[i] = new DataColumn();
                ECEcol[i].ColumnName = $"η {i + 1} [%]";
            }
            PerformanceDataTable.Columns.AddRange(ECEcol);

            if (IsExtended)
            {
                foreach (var unit in UnitsList)
                {
                    foreach (var parameter in unit.GetParametersList())
                    {
                        if (parameter.CheckIfToggled())
                        {
                            DataColumn col = new DataColumn();
                            col.ColumnName = $"&{parameter.GetName()} [{parameter.GetUoMstring()}]";
                            PerformanceDataTable.Columns.Add(col);
                        }
                    }
                }
            }
            _waiter.Set();

            //InitDataTable

            TimestampList.Clear();
            startTime = DateTime.Now;
            while (IsStartPossible)
            {
                WaitHandle.WaitAll(FinishedReadingEventList);
                endTime = DateTime.Now;
                foreach (var threadfinished in FinishedReadingEventList)
                    threadfinished.Reset();

                PowerLastReg = FCPowerParameter.GetLastMeasuredRegs();
                FlowLastReg = FlowParameter.GetLastMeasuredRegs();
                for (int i = 0; i < H.Length; i++)
                {
                    H[i] = (SelectedScheme.L[i] - SelectedScheme.L[i + 1] + (PressureParametersList[i + 1].GetLastMeasuredRegs() -
                        PressureParametersList[i].GetLastMeasuredRegs()) * (float)SiPressureCoefficient) / (SelectedScheme.Density * Scheme.g_constant);
                    ECE[i] = SelectedScheme.Density * Scheme.g_constant * H[i] * FlowLastReg * (float)SiFlowCoefficient
                        / (PowerLastReg * (float)SiPowerCoefficient);
                    if (ECE[i] == float.NaN)
                        ECE[i] = -1;
                    if (H[i] == float.NaN)
                        H[i] = -1;
                }

                int ECEoffset = H.Length * 2;

                Action<DataTable, DataRow> RowInsert = (DataTable t, DataRow r) => t.Rows.InsertAt(r, 0);

                Action<int> readH = (int i) =>
                    chart1.Series[i].Points.AddXY(FlowLastReg, H[i]); //H[i]
                Action<int> clearH = (int i) =>
                    chart1.Series[i].Points.Clear();
                Action<int> addH = (int i) =>
                    chart1.Series[i + H.Length].Points.AddXY(FlowLastReg, H[i]);

                Action<int> readECE = (int i) =>
                    chart1.Series[i + ECEoffset].Points.AddXY(FlowLastReg, ECE[i]); //H[i]
                Action<int> clearECE = (int i) =>
                    chart1.Series[i + ECEoffset].Points.Clear();
                Action<int> addECE = (int i) =>
                    chart1.Series[i + ECEoffset + ECE.Length].Points.AddXY(FlowLastReg, ECE[i]);

                Action readFreq = () => CurrentFrequencyTextBox.Text
                    = FCChangableParameter.GetLastMeasuredRegs().ToString();
                Action readValvePos = () => CurrentValvePosTextBox.Text
                    = ValveChangableParameter.GetLastMeasuredRegs().ToString();
                string Prstr = "";
                string Hstr = "";
                string ECEstr = "";
                for (int i = 0; i < PressureParametersList.Count(); i++)
                    Prstr = Prstr + PressureParametersList[i].GetLastMeasuredRegs().ToString() + " ; ";
                for (int i = 0; i < H.Count(); i++)
                    Hstr = Hstr + H[0].ToString() + " ; ";
                for (int i = 0; i < ECE.Count(); i++)
                    ECEstr = ECEstr + ECE[0].ToString() + " ; ";

                ////////////////
                row = PerformanceDataTable.NewRow();
                float span = (float)(endTime - startTime).TotalSeconds;
                TimestampList.Add(span);
                MakeARow(ref row, span, FlowLastReg, PowerLastReg, H, ECE);
                //PerformanceDataTable.Rows.Add(row);
                ////////////////

                if (InvokeRequired)
                {
                    Invoke(RowInsert, PerformanceDataTable, row);

                    for (int i = 0; i < H.Count(); i++)
                    {
                        Invoke(clearH, i);
                        Invoke(clearECE, i);
                    }
                    for (int i = 0; i < H.Count(); i++)
                    {
                        Invoke(readH, i);
                        Invoke(readECE, i);
                    }

                    if (FCChangableParameter != null)
                        Invoke(readFreq);
                    if (ValveChangableParameter != null)
                        Invoke(readValvePos);
                }
                else
                {
                    RowInsert(PerformanceDataTable, row);

                    for (int i = 0; i < H.Count(); i++)
                    {
                        clearH(i);
                        clearECE(i);
                    }
                    for (int i = 0; i < H.Count(); i++)
                    {
                        readH(i);
                        readECE(i);
                    }

                    if (FCChangableParameter != null)
                        readFreq();
                    if (ValveChangableParameter != null)
                        readValvePos();
                }
                StartReadingEvent.Set();
                if (_stopper.WaitOne(1000, false))
                {
                    if (stopthread)
                        break;
                    ///////
                    foreach (var un in UnitsList)
                    {
                        foreach (var par in un.GetParametersList())
                        {
                            par.IsMeasureSetList.Add(true);
                        }
                    }
                    Overlay.MakeRowBold();
                    PerformanceDataTable.Rows[0][0] = 1;
                    ///////
                    if (InvokeRequired)
                    {
                        for (int i = 0; i < H.Count(); i++)
                        {
                            Invoke(addH, i);
                            Invoke(addECE, i);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < H.Count(); i++)
                        {
                            addH(i);
                            addECE(i);
                        }
                    }
                    _stopper.Reset();
                }
                ////// 
                foreach (var un in UnitsList)
                {
                    foreach (var par in un.GetParametersList())
                    {
                        par.IsMeasureSetList.Add(false);
                    }
                }
                //////
                if (IsFCPlus)
                    FC.ComWrire(FCChangableParameter,
                        (ushort)(FCChangableParameter.GetLastMeasuredRegs() + SelectedScheme.FCStep));
                if (IsFCMinus)
                    FC.ComWrire(FCChangableParameter,
                        (ushort)(FCChangableParameter.GetLastMeasuredRegs() - SelectedScheme.FCStep));
                if (IsValvePlus)
                    FC.ComWrire(FCChangableParameter,
                        (ushort)(FCChangableParameter.GetLastMeasuredRegs() + SelectedScheme.ValveStep));
                if (IsValveMinus)
                    FC.ComWrire(FCChangableParameter,
                        (ushort)(FCChangableParameter.GetLastMeasuredRegs() - SelectedScheme.ValveStep));
                IsValvePlus = false;
                IsValveMinus = false;
                IsFCPlus = false;
                IsFCMinus = false;
            }
        }
        private void PerformanceTestStartButton_Click(object sender, EventArgs e)
        {
            ExperimentName = $"РНХ {DateTime.Now.ToString()}";
            if (Overlay != null)
            {
                var result = MessageBox.Show("Вы действительно хотите начать новый эксперимент? " +
                    "Несохраненные результаты будут удалены", "Новый эксперимент", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                    Overlay.Close();
                else return;
            }
            if (ExtendedParametersCheckBox.Checked)
                IsExtended = true;
            else
                IsExtended = false;
            try
            {
                if (SelectedScheme.FCPPID != -1)
                {
                    FrequencyPlusButton.Enabled = true;
                    FrequencyMinusButton.Enabled = true;
                }
                else
                {
                    FrequencyPlusButton.Enabled = false;
                    FrequencyMinusButton.Enabled = false;
                }
                if (SelectedScheme.ValvePID != -1)
                {
                    ValvePosPlusButton.Enabled = true;
                    ValvePosMinusButton.Enabled = true;
                }
                else
                {
                    ValvePosPlusButton.Enabled = false;
                    ValvePosMinusButton.Enabled = false;
                }
                SiPressureCoefficient = Convert.ToSingle(PressureSITextBox.Text) /
                    Convert.ToSingle(PressureCustomTextBox.Text);
                SiFlowCoefficient = Convert.ToSingle(FlowSITextBox.Text) /
                    Convert.ToSingle(FlowCustomTextBox.Text);
                SiPowerCoefficient = Convert.ToSingle(PowerSITextBox.Text) /
                    Convert.ToSingle(PowerCustomTextBox.Text);
                //System.FormatException

                chart1.ChartAreas[0].AxisX.Title = $"Расход [{FlowCustomLabel.Text}]";
                chart1.ChartAreas[0].AxisY.Title = $"Напор [м]";
                chart1.ChartAreas[0].AxisY2.Title = $"КПД [%]";
                chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
                chart1.Series.Clear();

                List<Parameter> PressureParameterList = new List<Parameter>();
                if (MakePressureParametersList(ref PressureParameterList))
                {
                    for (int i = 0; i < PressureParameterList.Count() - 1; i++)
                    {
                        chart1.Series.Add($"H {i + 1} мгновенный");
                        chart1.Series.Last().YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Primary;
                        chart1.Series.Last().ChartType = SeriesChartType.Point;
                        chart1.Series.Last().MarkerStyle = MarkerStyle.Diamond;
                        chart1.Series.Last().MarkerSize = 8;
                    }
                    for (int i = 0; i < PressureParameterList.Count() - 1; i++)
                    {
                        chart1.Series.Add($"H {i + 1}");
                        chart1.Series.Last().YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Primary;
                        chart1.Series.Last().ChartType = SeriesChartType.Point;
                        chart1.Series.Last().MarkerStyle = MarkerStyle.Cross;
                        chart1.Series.Last().MarkerSize = 8;
                    }
                    for (int i = 0; i < PressureParameterList.Count() - 1; i++)
                    {
                        chart1.Series.Add($"η {i + 1} мгновенный");
                        chart1.Series.Last().YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                        chart1.Series.Last().ChartType = SeriesChartType.Point;
                        chart1.Series.Last().MarkerStyle = MarkerStyle.Diamond;
                        chart1.Series.Last().MarkerSize = 8;
                    }
                    for (int i = 0; i < PressureParameterList.Count() - 1; i++)
                    {
                        chart1.Series.Add($"η {i + 1}");
                        chart1.Series.Last().YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                        chart1.Series.Last().ChartType = SeriesChartType.Point;
                        chart1.Series.Last().MarkerStyle = MarkerStyle.Cross;
                        chart1.Series.Last().MarkerSize = 8;
                    }
                }
                else
                {
                    return;
                }
                /*for (int i = 0; i < chart1.Series.Count; i++)
                {
                    chart1.Series[i].Points.Clear();
                    chart1.Series[i].ChartType = SeriesChartType.Point;
                }*/
                PerformanceDataTable = new DataTable("РНХ");
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
                {
                    PerformanceParametersReadout(FinishedReadingEventList,
                    ref PressureParameterList, ref PerformanceDataTable);
                });
                var thread1 = new System.Threading.Thread(threadParameters1);
                thread1.Start();

                _waiter.WaitOne();
                Overlay = new PerformanceTableOverlay(ref PerformanceDataTable, ExperimentName);
                try
                {
                    Overlay.Show();
                    foreach (DataGridViewColumn col in Overlay.dataGridView1.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
                catch (System.ObjectDisposedException)
                {
                    MessageBox.Show("Произошла непредвиденная ошибка");
                }
                _waiter.Reset();

            }
            catch (System.FormatException)
            {
                MessageBox.Show("Ошибка в переводе параметров к СИ");
            }
        }
        private void PerformanceTestStopButton_Click(object sender, EventArgs e)
        {
            stopthread = true;
            _stopper.Set();
            StartReadingEvent.Set();
            for (int i = chart1.Series.Count / 4; i < chart1.Series.Count / 2; i++)
            {
                chart1.Series[i].Sort(PointSortOrder.Ascending, "X");
                chart1.Series[i].ChartType = SeriesChartType.Spline;
            }
            for (int i = (chart1.Series.Count / 4 + chart1.Series.Count / 2); i < chart1.Series.Count; i++)
            {
                chart1.Series[i].Sort(PointSortOrder.Ascending, "X");
                chart1.Series[i].ChartType = SeriesChartType.Spline;
            }
        }
        private void PerformanceTestToExcelButton_Click(object sender, EventArgs e)
        {
            /*Workbook workbook = new Workbook();

            workbook.Worksheets.Clear();
            Worksheet worksheetAll = workbook.Worksheets.Add("Все параметры");
            Worksheet worksheetPerformance = workbook.Worksheets.Add("РНХ");

            int j = 1;
            foreach (var un in UnitsList)
            {
                int i = 1;
                worksheetAll.Range[i++, j].Value = un.GetName();
                foreach (var par in un.GetParametersList())
                {
                    int istart = i;
                    worksheetAll.Range[i++, j].Value = par.GetName();
                    foreach (var reg in par.GetAllMeasuredRegs())
                        worksheetAll.Range[i++, j].Value = reg.ToString();
                    j++;
                    i = istart;
                }
            }
            worksheetAll.AllocatedRange.AutoFitColumns();

            j = 1;
            worksheetPerformance.Range[1, j].Value = "Расход";
            worksheetPerformance.Range[1, j+1].Value = chart1.Series[0].Name;
            for (int i = 0; i < chart1.Series[0].Points.Count; i++)
            {
                worksheetPerformance.Range[i+2, j].Value = chart1.Series[0].Points[i].XValue.ToString();
                worksheetPerformance.Range[i + 2, j+1].Value = chart1.Series[0].Points[i].YValues[0].ToString();
            }
            j = 3;
            for (int k = 0; k < 4; k++)
            {
                worksheetPerformance.Range[1, j+k].Value = chart1.Series[k+2].Name;
                for (int i = 0; i < chart1.Series[k + 2].Points.Count; i++)
                {
                    worksheetPerformance.Range[i + 2, j+k].Value = chart1.Series[k + 2].Points[i].YValues[0].ToString();                    worksheetPerformance.Range[i + 2, j + 1].Value = chart1.Series[0].Points[i].YValues[0].ToString();
                }
            }

            workbook.SaveToFile("TEST.xlsx", ExcelVersion.Version2016);*/
        }
        /// <summary>
        /// 
        /// </summary>
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
            if (SchemeComboBox.SelectedIndex != -1)
            {
                PerformanceTestLabel.Text = $"Расходно-напорная характеристика насоса {SchemeComboBox.Text}";
            }
            if (tabControl1.SelectedIndex == 3)
            {
                if (postgreSQL.NpgsqlConnect())
                {
                    ExperimentsListBox.Items.Clear();
                    string[] exps = (string[])postgreSQL.GetExperimentNames().Result.ToArray();
                    ExperimentsListBox.Items.AddRange(exps);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopthread = true;
            _stopper.Set();
            StartReadingEvent.Set();

            foreach (var process in Process.GetProcessesByName("Stand"))
            {
                process.Kill();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _stopper.Set();
        }
        private void RgistersAdditionalButton_Click(object sender, EventArgs e)
        {
            IsRegistersAdditionalToggled = !IsRegistersAdditionalToggled;
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

        private void PerformanceTestAddButton_Click(object sender, EventArgs e)
        {
            if (postgreSQL.NpgsqlConnect())
            {
                if (postgreSQL.SaveMeasurements(ref UnitsList, ExperimentName, ref TimestampList, ref PerformanceDataTable))
                    MessageBox.Show("Сохранение успешно завершено");
                else
                    MessageBox.Show("Error");
            }
        }

        private void DeleteResultButton_Click(object sender, EventArgs e)
        {
            if (postgreSQL.NpgsqlConnect())
            {
                string[] parts = { "0" };
                if (ExperimentsListBox.SelectedIndex >= 0)
                    parts = (ExperimentsListBox.SelectedItem.ToString().Split('@'));
                if (postgreSQL.DeleteExperiment(int.Parse(parts[0])))
                {
                    MessageBox.Show("OK");

                    RefreshExperimentsListBox();
                }
                ExperimentGridView.Refresh();
            }
        }

        private void RefreshExperimentsListBox()
        {
            if (postgreSQL.NpgsqlConnect())
            {
                ExperimentsListBox.Items.Clear();
                string[] exps = (string[])postgreSQL.GetExperimentNames().Result.ToArray();
                ExperimentsListBox.Items.AddRange(exps);
            }
            if (ExperimentsListBox.Items.Count > 0)
                ExperimentsListBox.SelectedIndex = 0;
            else
            {
                ExperimentGridView.DataSource = null;
                TableNameTextBox.Text = "";
            }
        }
        private void ExperimentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (postgreSQL.NpgsqlConnect())
            {
                DataTable results = new DataTable();
                DataTable performance = new DataTable();
                string[] parts = { "0" };
                if (ExperimentsListBox.SelectedIndex >= 0)
                {
                    parts = (ExperimentsListBox.SelectedItem.ToString().Split('@'));
                    results.TableName = parts[1];
                    TableNameTextBox.Text = results.TableName;
                }
                if (postgreSQL.GetExperimentResults(int.Parse(parts[0]), ref results, ref performance))
                {
                    ExperimentGridView.DataSource = results;
                    PerformanceGridView.DataSource = performance;
                }
                ExperimentGridView.Refresh();
            }
        }

        private void ChangeExperimentNameButton_Click(object sender, EventArgs e)
        {
            if (postgreSQL.NpgsqlConnect())
            {
                string[] parts = { "0" };
                if (ExperimentsListBox.SelectedIndex >= 0)
                {
                    parts = (ExperimentsListBox.SelectedItem.ToString().Split('@'));
                    if (TableNameTextBox.Text != "")
                    {
                        if (postgreSQL.ChangeExperimentName(int.Parse(parts[0]), TableNameTextBox.Text))
                        {
                            MessageBox.Show("OK");
                            RefreshExperimentsListBox();
                        }
                    }
                }
                ExperimentGridView.Refresh();
            }
        }

        private void PerformanceTestToExcelButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                XLWorkbook wb = new XLWorkbook();
                DataTable alldt = new DataTable();
                DataTable perfdt = new DataTable();
                foreach (DataGridViewColumn col in ExperimentGridView.Columns)
                {
                    alldt.Columns.Add(col.Name);
                }
                foreach (DataGridViewRow row in ExperimentGridView.Rows)
                {
                    DataRow dRow = alldt.NewRow();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dRow[cell.ColumnIndex] = cell.Value;
                    }
                    alldt.Rows.Add(dRow);
                }
                wb.Worksheets.Add(alldt, "All");

                foreach (DataGridViewColumn col in PerformanceGridView.Columns)
                {
                    perfdt.Columns.Add(col.Name);
                }
                foreach (DataGridViewRow row in PerformanceGridView.Rows)
                {
                    DataRow dRow = perfdt.NewRow();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        dRow[cell.ColumnIndex] = cell.Value;
                    }
                    perfdt.Rows.Add(dRow);
                }
                wb.Worksheets.Add(perfdt, "Performance");
                var savestr = (string)($"Saves/{TableNameTextBox.Text}").Replace(" ", "_").Replace(".", "").Replace(":", "");
                wb.SaveAs(savestr + ".xlsx");
                MessageBox.Show("Успешно");
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
    #endregion
}