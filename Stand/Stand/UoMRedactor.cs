using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Stand
{
    public partial class UoMRedactor : Form
    {
        private Parameter Parameter;
        private Form1 mainform;
        private string UnitName;
        private bool IsModified = false;
        public UoMRedactor(ref Parameter Parameter, string UnitName,
            Form1 mainform)
        {
            this.mainform = mainform;
            this.Parameter = Parameter;
            this.UnitName = UnitName;
            InitializeComponent();
        }
        private void RefreshUoMListbox()
        {
            UoMListBox.Items.Clear();
            foreach (var uom in Parameter.GetUoMs())
            {
                UoMListBox.Items.Add($"{uom.Key} : {uom.Value}");
            }
        }
        private void UoMRedactor_Load(object sender, EventArgs e)
        {
            this.Text = $"Редактор ед. изм. {UnitName}({Parameter.GetName()})";
            RefreshUoMListbox();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {   
            IsModified = true;
            Parameter.UnitsOfMeasure
                .Add($"новая ед.{UoMListBox.Items.Count}", 1);
            RefreshUoMListbox();
            UoMListBox.SelectedIndex = UoMListBox.Items.Count - 1;
        }

        private void UoMListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UoMListBox.SelectedIndex != -1)
            {
                ValueTextBox.Text = Parameter.UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Key;
                MultiplierTextBox.Text = Parameter.UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Value.ToString();
            }
        }

        private void UoMChangeButton_Click(object sender, EventArgs e)
        {
            IsModified = true;
            string oldkey = Parameter.UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Key;
            try
            {
                string newkey = ValueTextBox.Text;
                float newvalue = float.Parse(MultiplierTextBox.Text);
                Parameter.UnitsOfMeasure.Remove(oldkey);
                Parameter.UnitsOfMeasure.Add(newkey, newvalue);
                RefreshUoMListbox();
                UoMListBox.SelectedIndex = UoMListBox.Items.Count - 1;
            }
            catch (Exception)
            {
                MessageBox.Show("Неправильный формат");
            }
        }

        private void MultiplierTextBox_TextChanged(object sender, EventArgs e)
        {
            if (MultiplierTextBox.Text != "" && ValueTextBox.Text != "")
            {
                UoMChangeButton.Enabled = true;
                UomDeleteButton.Enabled = true;
            }
            else
            {
                UoMChangeButton.Enabled = false;
                UomDeleteButton.Enabled = false;
            }
        }

        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (MultiplierTextBox.Text != "" && ValueTextBox.Text != "")
            {
                UoMChangeButton.Enabled = true;
                UomDeleteButton.Enabled = true;
            }
            else
            {
                UoMChangeButton.Enabled = false;
                UomDeleteButton.Enabled = false;
            }
        }

        private void UomDeleteButton_Click(object sender, EventArgs e)
        {
            IsModified = true;
            string oldkey = Parameter.UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Key;
            Parameter.UnitsOfMeasure.Remove(oldkey);
            RefreshUoMListbox();
            UoMListBox.SelectedIndex = UoMListBox.Items.Count - 1;
        }

        private void UoMRedactor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsModified)
            {
                Parameter.SaveXML();
                mainform.UpdateSelectedParameter();
            }
        }
    }
}
