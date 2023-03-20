using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stand
{
    public partial class UoMRedactor : Form
    {
        private SortedList<string, Parameter> ParameterDictionary;
        private Form1 mainform;
        private bool IsModified = false;
        public UoMRedactor(ref SortedList<string, Parameter> ParameterDictionary, Form1 mainform)
        {
            this.mainform = mainform;
            this.ParameterDictionary = ParameterDictionary;
            InitializeComponent();
        }
        private void RefreshUoMListbox()
        {
            UoMListBox.Items.Clear();
            var dict = ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.UnitsOfMeasure;
            foreach (var uom in dict)
            {
                UoMListBox.Items.Add($"{uom.Key} : {uom.Value}");
            }
        }
        private void UoMRedactor_Load(object sender, EventArgs e)
        {
            ParametersListBox.Items.Clear();
            ParametersListBox.Items.AddRange(ParameterDictionary.Keys.ToArray());
            ParametersListBox.SelectedIndex = 0;
        }

        private void ParametersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUoMListbox();
            ValueTextBox.Clear();
            MultiplierTextBox.Clear();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {   
            IsModified = true;
            if (ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Key.Contains("Pressure"))
            {
                foreach (var par in ParameterDictionary)
                {
                    if (par.Key.Contains("Pressure"))
                        par.Value.UnitsOfMeasure.Add($"новая ед.{UoMListBox.Items.Count}", 1);
                }
            }
            else
            {
                ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.UnitsOfMeasure
                    .Add($"новая ед.{UoMListBox.Items.Count}", 1);
            }
            RefreshUoMListbox();
            UoMListBox.SelectedIndex = UoMListBox.Items.Count - 1;
        }

        private void UoMListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValueTextBox.Text = ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Key;
            MultiplierTextBox.Text = ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Value.ToString();
        }

        private void UoMChangeButton_Click(object sender, EventArgs e)
        {
            IsModified = true;
            string oldkey = ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Key;
            try
            {
                string newkey = ValueTextBox.Text;
                float newvalue = float.Parse(MultiplierTextBox.Text);
                if (ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Key.Contains("Pressure"))
                {
                    foreach (var par in ParameterDictionary)
                    {
                        if (par.Key.Contains("Pressure"))
                        {
                            par.Value.UnitsOfMeasure.Remove(oldkey);
                            par.Value.UnitsOfMeasure.Add(newkey, newvalue);
                        }
                    }
                }
                else
                {
                    ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                    UnitsOfMeasure.Remove(oldkey);
                    ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                        UnitsOfMeasure.Add(newkey, newvalue);
                }
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
            string oldkey = ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                UnitsOfMeasure.ElementAt(UoMListBox.SelectedIndex).Key;
            if (ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Key.Contains("Pressure"))
            {
                foreach (var par in ParameterDictionary)
                {
                    if (par.Key.Contains("Pressure"))
                    {
                        par.Value.UnitsOfMeasure.Remove(oldkey);
                    }
                }
            }
            else
            {
                ParameterDictionary.ElementAt(ParametersListBox.SelectedIndex).Value.
                    UnitsOfMeasure.Remove(oldkey);
            }
            RefreshUoMListbox();
            UoMListBox.SelectedIndex = UoMListBox.Items.Count - 1;
        }

        private void UoMRedactor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(IsModified)
                mainform.GetUnitsOfMeasure();
        }
    }
}
