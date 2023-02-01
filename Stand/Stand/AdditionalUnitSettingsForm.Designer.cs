namespace Stand
{
    partial class AdditionalUnitSettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DataBitsTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ParityComboBox = new System.Windows.Forms.ComboBox();
            this.StopBitsComboBox = new System.Windows.Forms.ComboBox();
            this.DtrComboBox = new System.Windows.Forms.ComboBox();
            this.RtsCcomboBox = new System.Windows.Forms.ComboBox();
            this.ReadTimeotTextBox = new System.Windows.Forms.TextBox();
            this.WriteTimeoutTextBox = new System.Windows.Forms.TextBox();
            this.HandshakeComboBox = new System.Windows.Forms.ComboBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DataBits";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Parity";
            // 
            // DataBitsTextBox
            // 
            this.DataBitsTextBox.Location = new System.Drawing.Point(129, 6);
            this.DataBitsTextBox.Name = "DataBitsTextBox";
            this.DataBitsTextBox.Size = new System.Drawing.Size(100, 20);
            this.DataBitsTextBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "StopBits";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "DtrEnable";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "RtsEnable";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "ReadTimeout";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 169);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "WriteTimeout";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 195);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Handshake";
            // 
            // ParityComboBox
            // 
            this.ParityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ParityComboBox.FormattingEnabled = true;
            this.ParityComboBox.Location = new System.Drawing.Point(129, 32);
            this.ParityComboBox.Name = "ParityComboBox";
            this.ParityComboBox.Size = new System.Drawing.Size(100, 21);
            this.ParityComboBox.TabIndex = 9;
            // 
            // StopBitsComboBox
            // 
            this.StopBitsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StopBitsComboBox.FormattingEnabled = true;
            this.StopBitsComboBox.Location = new System.Drawing.Point(129, 59);
            this.StopBitsComboBox.Name = "StopBitsComboBox";
            this.StopBitsComboBox.Size = new System.Drawing.Size(100, 21);
            this.StopBitsComboBox.TabIndex = 10;
            // 
            // DtrComboBox
            // 
            this.DtrComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DtrComboBox.FormattingEnabled = true;
            this.DtrComboBox.Items.AddRange(new object[] {
            "true",
            "false"});
            this.DtrComboBox.Location = new System.Drawing.Point(129, 86);
            this.DtrComboBox.Name = "DtrComboBox";
            this.DtrComboBox.Size = new System.Drawing.Size(100, 21);
            this.DtrComboBox.TabIndex = 11;
            // 
            // RtsCcomboBox
            // 
            this.RtsCcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RtsCcomboBox.FormattingEnabled = true;
            this.RtsCcomboBox.Items.AddRange(new object[] {
            "true",
            "false"});
            this.RtsCcomboBox.Location = new System.Drawing.Point(129, 113);
            this.RtsCcomboBox.Name = "RtsCcomboBox";
            this.RtsCcomboBox.Size = new System.Drawing.Size(100, 21);
            this.RtsCcomboBox.TabIndex = 12;
            // 
            // ReadTimeotTextBox
            // 
            this.ReadTimeotTextBox.Location = new System.Drawing.Point(129, 140);
            this.ReadTimeotTextBox.Name = "ReadTimeotTextBox";
            this.ReadTimeotTextBox.Size = new System.Drawing.Size(100, 20);
            this.ReadTimeotTextBox.TabIndex = 13;
            // 
            // WriteTimeoutTextBox
            // 
            this.WriteTimeoutTextBox.Location = new System.Drawing.Point(129, 166);
            this.WriteTimeoutTextBox.Name = "WriteTimeoutTextBox";
            this.WriteTimeoutTextBox.Size = new System.Drawing.Size(100, 20);
            this.WriteTimeoutTextBox.TabIndex = 14;
            // 
            // HandshakeComboBox
            // 
            this.HandshakeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HandshakeComboBox.FormattingEnabled = true;
            this.HandshakeComboBox.Location = new System.Drawing.Point(129, 192);
            this.HandshakeComboBox.Name = "HandshakeComboBox";
            this.HandshakeComboBox.Size = new System.Drawing.Size(100, 21);
            this.HandshakeComboBox.TabIndex = 15;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(129, 228);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(96, 23);
            this.SaveButton.TabIndex = 16;
            this.SaveButton.Text = "Сохранить";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // AdditionalUnitSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 263);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.HandshakeComboBox);
            this.Controls.Add(this.WriteTimeoutTextBox);
            this.Controls.Add(this.ReadTimeotTextBox);
            this.Controls.Add(this.RtsCcomboBox);
            this.Controls.Add(this.DtrComboBox);
            this.Controls.Add(this.StopBitsComboBox);
            this.Controls.Add(this.ParityComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DataBitsTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AdditionalUnitSettingsForm";
            this.Load += new System.EventHandler(this.AdditionalUnitSettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DataBitsTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox ParityComboBox;
        private System.Windows.Forms.ComboBox StopBitsComboBox;
        private System.Windows.Forms.ComboBox DtrComboBox;
        private System.Windows.Forms.ComboBox RtsCcomboBox;
        private System.Windows.Forms.TextBox ReadTimeotTextBox;
        private System.Windows.Forms.TextBox WriteTimeoutTextBox;
        private System.Windows.Forms.ComboBox HandshakeComboBox;
        private System.Windows.Forms.Button SaveButton;
    }
}