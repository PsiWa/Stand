namespace Stand
{
    partial class UoMRedactor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UoMListBox = new System.Windows.Forms.ListBox();
            this.ParameterLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            this.MultiplierTextBox = new System.Windows.Forms.TextBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.UoMChangeButton = new System.Windows.Forms.Button();
            this.UomDeleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UoMListBox
            // 
            this.UoMListBox.FormattingEnabled = true;
            this.UoMListBox.Location = new System.Drawing.Point(12, 25);
            this.UoMListBox.Name = "UoMListBox";
            this.UoMListBox.Size = new System.Drawing.Size(422, 225);
            this.UoMListBox.TabIndex = 1;
            this.UoMListBox.SelectedIndexChanged += new System.EventHandler(this.UoMListBox_SelectedIndexChanged);
            // 
            // ParameterLabel
            // 
            this.ParameterLabel.AutoSize = true;
            this.ParameterLabel.Location = new System.Drawing.Point(9, 9);
            this.ParameterLabel.Name = "ParameterLabel";
            this.ParameterLabel.Size = new System.Drawing.Size(180, 13);
            this.ParameterLabel.TabIndex = 3;
            this.ParameterLabel.Text = "Единицы измерения и множители";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Ед. измерения";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(116, 253);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Множитель";
            // 
            // ValueTextBox
            // 
            this.ValueTextBox.Location = new System.Drawing.Point(12, 269);
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.Size = new System.Drawing.Size(101, 20);
            this.ValueTextBox.TabIndex = 6;
            this.ValueTextBox.TextChanged += new System.EventHandler(this.ValueTextBox_TextChanged);
            // 
            // MultiplierTextBox
            // 
            this.MultiplierTextBox.Location = new System.Drawing.Point(119, 269);
            this.MultiplierTextBox.Name = "MultiplierTextBox";
            this.MultiplierTextBox.Size = new System.Drawing.Size(101, 20);
            this.MultiplierTextBox.TabIndex = 7;
            this.MultiplierTextBox.TextChanged += new System.EventHandler(this.MultiplierTextBox_TextChanged);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(12, 295);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(208, 23);
            this.AddButton.TabIndex = 8;
            this.AddButton.Text = "Добавить новую";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // UoMChangeButton
            // 
            this.UoMChangeButton.Enabled = false;
            this.UoMChangeButton.Location = new System.Drawing.Point(226, 266);
            this.UoMChangeButton.Name = "UoMChangeButton";
            this.UoMChangeButton.Size = new System.Drawing.Size(101, 23);
            this.UoMChangeButton.TabIndex = 9;
            this.UoMChangeButton.Text = "Изменить";
            this.UoMChangeButton.UseVisualStyleBackColor = true;
            this.UoMChangeButton.Click += new System.EventHandler(this.UoMChangeButton_Click);
            // 
            // UomDeleteButton
            // 
            this.UomDeleteButton.Enabled = false;
            this.UomDeleteButton.Location = new System.Drawing.Point(333, 266);
            this.UomDeleteButton.Name = "UomDeleteButton";
            this.UomDeleteButton.Size = new System.Drawing.Size(101, 23);
            this.UomDeleteButton.TabIndex = 10;
            this.UomDeleteButton.Text = "Удалить";
            this.UomDeleteButton.UseVisualStyleBackColor = true;
            this.UomDeleteButton.Click += new System.EventHandler(this.UomDeleteButton_Click);
            // 
            // UoMRedactor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 327);
            this.Controls.Add(this.UomDeleteButton);
            this.Controls.Add(this.UoMChangeButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.MultiplierTextBox);
            this.Controls.Add(this.ValueTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ParameterLabel);
            this.Controls.Add(this.UoMListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UoMRedactor";
            this.Text = "UoMRedactor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UoMRedactor_FormClosed);
            this.Load += new System.EventHandler(this.UoMRedactor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox UoMListBox;
        private System.Windows.Forms.Label ParameterLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ValueTextBox;
        private System.Windows.Forms.TextBox MultiplierTextBox;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button UoMChangeButton;
        private System.Windows.Forms.Button UomDeleteButton;
    }
}