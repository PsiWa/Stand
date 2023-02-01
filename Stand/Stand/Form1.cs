﻿using System;
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

namespace Stand
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private XDocument xDefaultDoc = new XDocument();
        private XElement xSettings = new XElement("settings");
        public Unit FlowMeter = new Unit("flowmeter");



        private void Form1_Load(object sender, EventArgs e)
        {
            FlowComComboBox.Items.AddRange(SerialPort.GetPortNames());
            try
            {
                xDefaultDoc = XDocument.Load("Defaults.xml");
                xSettings = xDefaultDoc.Element("settings");
                foreach (XElement el in xSettings.Elements("unit"))
                {
                    if (el.Attribute("Name").Value == "flowmeter")
                    {
                        string COM = el.Attribute("COM").Value;
                        int address = Convert.ToInt32(el.Attribute("Address").Value);
                        int speed = Convert.ToInt32(el.Attribute("BaudRate").Value);
                        int databits = Convert.ToInt32(el.Attribute("DataBits").Value);
                        Parity parity = (Parity)Enum.Parse(typeof(Parity), el.Attribute("Parity").Value);
                        StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), el.Attribute("StopBits").Value);
                        bool dtr = Convert.ToBoolean(el.Attribute("DtrEnable").Value);
                        bool rts = Convert.ToBoolean(el.Attribute("RtsEnable").Value);
                        int readtout = Convert.ToInt32(el.Attribute("ReadTimeout").Value);
                        int writetout = Convert.ToInt32(el.Attribute("WriteTimeout").Value);
                        Handshake handshake = (Handshake)Enum.Parse(typeof(Handshake), el.Attribute("Handshake").Value);

                        FlowMeter.SetParameters(COM, speed, address, databits, parity, stopbits, dtr, 
                            rts, readtout, writetout, handshake);

                        FlowComComboBox.SelectedIndex = FlowComComboBox.FindStringExact(COM);
                        FlowSpeedTextBox.Text = speed.ToString();
                        FlowAddressTextBox.Text = address.ToString();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка в файле настроек");
            }
        }

        private void FlowSaveButton_Click(object sender, EventArgs e)
        {
            FlowMeter.SaveSettingsXML(ref xSettings);
        }

        private void FlowAdditionalButton_Click(object sender, EventArgs e)
        {
            AdditionalUnitSettingsForm addsett = new AdditionalUnitSettingsForm(ref FlowMeter) ;
            addsett.Show();
        }

        //НАСТРОЙКИ
        //Расходомер

    }
}
