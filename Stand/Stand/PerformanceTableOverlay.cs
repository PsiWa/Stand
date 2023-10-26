using Spire.Xls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Stand
{
    public partial class PerformanceTableOverlay : Form
    {
        public PerformanceTableOverlay(ref DataTable PerformanceDataTable, string name)
        {
            InitializeComponent();
            try
            {
                dataGridView1.DataSource = PerformanceDataTable;
                this.Text = name;
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        private void PerformanceTableOverlay_FormClosing(object sender, FormClosingEventArgs e) // Это надо убрать
        {
            var result = MessageBox.Show("Вы уверены, что хотите закрыть форму?", "Закрытие",
                MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {

            }
        }

        public void MakeRowBold()
        {
            DataGridViewCellStyle boldStyle = new DataGridViewCellStyle();
            boldStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.dataGridView1.Rows[0].DefaultCellStyle = boldStyle;
        }
    }
}
