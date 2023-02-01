using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stand
{
    public partial class AdditionalUnitSettingsForm : Form
    {
        private Unit un;
        public AdditionalUnitSettingsForm(ref Unit un)
        {
            this.un = un;
            InitializeComponent();
            this.Text = un.GetName();
        }

        private void AdditionalUnitSettingsForm_Load(object sender, EventArgs e)
        {
            ParityComboBox.Items.AddRange(Enum.GetNames(typeof(Parity)));
            StopBitsComboBox.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            HandshakeComboBox.Items.AddRange(Enum.GetNames(typeof(Handshake)));

            string databits = "";
            string parity = "";
            string stopBits = "";
            string dtr = "";
            string rts = "";
            string readtout = "";
            string writetout = "";
            string handshake = "";
            un.GetParametersString(ref databits, ref parity, ref stopBits, ref dtr, ref rts, ref readtout, ref writetout,
                ref handshake);

            DataBitsTextBox.Text = databits;
            ParityComboBox.SelectedIndex = ParityComboBox.FindStringExact(parity);
            StopBitsComboBox.SelectedIndex = StopBitsComboBox.FindStringExact(stopBits);
            DtrComboBox.SelectedIndex = DtrComboBox.FindStringExact(dtr);
            RtsCcomboBox.SelectedIndex = RtsCcomboBox.FindStringExact(rts);
            ReadTimeotTextBox.Text = readtout;
            WriteTimeoutTextBox.Text = writetout;
            HandshakeComboBox.SelectedIndex = HandshakeComboBox.FindStringExact(handshake);
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                un.SetParameters(Convert.ToInt32(DataBitsTextBox.Text), 
                    (Parity)Enum.Parse(typeof(Parity), ParityComboBox.Text),
                    (StopBits)Enum.Parse(typeof(StopBits), StopBitsComboBox.Text), 
                    Convert.ToBoolean(DtrComboBox.Text),
                    Convert.ToBoolean(RtsCcomboBox.Text), 
                    Convert.ToInt32(ReadTimeotTextBox.Text),
                    Convert.ToInt32(WriteTimeoutTextBox.Text), 
                    (Handshake)Enum.Parse(typeof(Handshake), HandshakeComboBox.Text));
                this.Close();
            }
            catch(Exception)
            {
                MessageBox.Show("Неправильные параметры");
            }
            
            
        }
    }
}
