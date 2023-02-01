namespace Stand
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Settings = new System.Windows.Forms.TabPage();
            this.FlowSaveButton = new System.Windows.Forms.Button();
            this.FlowConnectButton = new System.Windows.Forms.Button();
            this.FlowStatusTextBox = new System.Windows.Forms.TextBox();
            this.FlowStatusLabel = new System.Windows.Forms.Label();
            this.FlowAddressTextBox = new System.Windows.Forms.TextBox();
            this.FlowAddressLabel = new System.Windows.Forms.Label();
            this.FlowSpeedTextBox = new System.Windows.Forms.TextBox();
            this.FlowSpeedLabel = new System.Windows.Forms.Label();
            this.FlowComLabel = new System.Windows.Forms.Label();
            this.FlowComComboBox = new System.Windows.Forms.ComboBox();
            this.FlowmeterLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.FlowAdditionalButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.Settings.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Settings);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(913, 607);
            this.tabControl1.TabIndex = 0;
            // 
            // Settings
            // 
            this.Settings.Controls.Add(this.FlowAdditionalButton);
            this.Settings.Controls.Add(this.FlowSaveButton);
            this.Settings.Controls.Add(this.FlowConnectButton);
            this.Settings.Controls.Add(this.FlowStatusTextBox);
            this.Settings.Controls.Add(this.FlowStatusLabel);
            this.Settings.Controls.Add(this.FlowAddressTextBox);
            this.Settings.Controls.Add(this.FlowAddressLabel);
            this.Settings.Controls.Add(this.FlowSpeedTextBox);
            this.Settings.Controls.Add(this.FlowSpeedLabel);
            this.Settings.Controls.Add(this.FlowComLabel);
            this.Settings.Controls.Add(this.FlowComComboBox);
            this.Settings.Controls.Add(this.FlowmeterLabel);
            this.Settings.Location = new System.Drawing.Point(4, 22);
            this.Settings.Name = "Settings";
            this.Settings.Padding = new System.Windows.Forms.Padding(3);
            this.Settings.Size = new System.Drawing.Size(905, 581);
            this.Settings.TabIndex = 0;
            this.Settings.Text = "Настройки";
            this.Settings.UseVisualStyleBackColor = true;
            // 
            // FlowSaveButton
            // 
            this.FlowSaveButton.Location = new System.Drawing.Point(25, 208);
            this.FlowSaveButton.Name = "FlowSaveButton";
            this.FlowSaveButton.Size = new System.Drawing.Size(264, 34);
            this.FlowSaveButton.TabIndex = 10;
            this.FlowSaveButton.Text = "Сохранить настройки";
            this.FlowSaveButton.UseVisualStyleBackColor = true;
            this.FlowSaveButton.Click += new System.EventHandler(this.FlowSaveButton_Click);
            // 
            // FlowConnectButton
            // 
            this.FlowConnectButton.Location = new System.Drawing.Point(25, 168);
            this.FlowConnectButton.Name = "FlowConnectButton";
            this.FlowConnectButton.Size = new System.Drawing.Size(264, 34);
            this.FlowConnectButton.TabIndex = 9;
            this.FlowConnectButton.Text = "Подключиться";
            this.FlowConnectButton.UseVisualStyleBackColor = true;
            // 
            // FlowStatusTextBox
            // 
            this.FlowStatusTextBox.BackColor = System.Drawing.Color.Red;
            this.FlowStatusTextBox.Enabled = false;
            this.FlowStatusTextBox.Location = new System.Drawing.Point(168, 142);
            this.FlowStatusTextBox.Name = "FlowStatusTextBox";
            this.FlowStatusTextBox.Size = new System.Drawing.Size(121, 20);
            this.FlowStatusTextBox.TabIndex = 8;
            // 
            // FlowStatusLabel
            // 
            this.FlowStatusLabel.AutoSize = true;
            this.FlowStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FlowStatusLabel.Location = new System.Drawing.Point(21, 140);
            this.FlowStatusLabel.Name = "FlowStatusLabel";
            this.FlowStatusLabel.Size = new System.Drawing.Size(62, 20);
            this.FlowStatusLabel.TabIndex = 7;
            this.FlowStatusLabel.Text = "Статус";
            // 
            // FlowAddressTextBox
            // 
            this.FlowAddressTextBox.Location = new System.Drawing.Point(168, 116);
            this.FlowAddressTextBox.Name = "FlowAddressTextBox";
            this.FlowAddressTextBox.Size = new System.Drawing.Size(121, 20);
            this.FlowAddressTextBox.TabIndex = 6;
            // 
            // FlowAddressLabel
            // 
            this.FlowAddressLabel.AutoSize = true;
            this.FlowAddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FlowAddressLabel.Location = new System.Drawing.Point(21, 114);
            this.FlowAddressLabel.Name = "FlowAddressLabel";
            this.FlowAddressLabel.Size = new System.Drawing.Size(147, 20);
            this.FlowAddressLabel.TabIndex = 5;
            this.FlowAddressLabel.Text = "Адрес устройства";
            // 
            // FlowSpeedTextBox
            // 
            this.FlowSpeedTextBox.Location = new System.Drawing.Point(168, 90);
            this.FlowSpeedTextBox.Name = "FlowSpeedTextBox";
            this.FlowSpeedTextBox.Size = new System.Drawing.Size(121, 20);
            this.FlowSpeedTextBox.TabIndex = 4;
            // 
            // FlowSpeedLabel
            // 
            this.FlowSpeedLabel.AutoSize = true;
            this.FlowSpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FlowSpeedLabel.Location = new System.Drawing.Point(21, 88);
            this.FlowSpeedLabel.Name = "FlowSpeedLabel";
            this.FlowSpeedLabel.Size = new System.Drawing.Size(141, 20);
            this.FlowSpeedLabel.TabIndex = 3;
            this.FlowSpeedLabel.Text = "Скорость обмена";
            // 
            // FlowComLabel
            // 
            this.FlowComLabel.AutoSize = true;
            this.FlowComLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FlowComLabel.Location = new System.Drawing.Point(21, 61);
            this.FlowComLabel.Name = "FlowComLabel";
            this.FlowComLabel.Size = new System.Drawing.Size(85, 20);
            this.FlowComLabel.TabIndex = 2;
            this.FlowComLabel.Text = "COM порт";
            // 
            // FlowComComboBox
            // 
            this.FlowComComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.FlowComComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.FlowComComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlowComComboBox.FormattingEnabled = true;
            this.FlowComComboBox.Location = new System.Drawing.Point(168, 63);
            this.FlowComComboBox.Name = "FlowComComboBox";
            this.FlowComComboBox.Size = new System.Drawing.Size(121, 21);
            this.FlowComComboBox.Sorted = true;
            this.FlowComComboBox.TabIndex = 1;
            // 
            // FlowmeterLabel
            // 
            this.FlowmeterLabel.AutoSize = true;
            this.FlowmeterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FlowmeterLabel.Location = new System.Drawing.Point(20, 17);
            this.FlowmeterLabel.Name = "FlowmeterLabel";
            this.FlowmeterLabel.Size = new System.Drawing.Size(135, 25);
            this.FlowmeterLabel.TabIndex = 0;
            this.FlowmeterLabel.Text = "Расходомер";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(905, 581);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FlowAdditionalButton
            // 
            this.FlowAdditionalButton.Location = new System.Drawing.Point(168, 21);
            this.FlowAdditionalButton.Name = "FlowAdditionalButton";
            this.FlowAdditionalButton.Size = new System.Drawing.Size(121, 23);
            this.FlowAdditionalButton.TabIndex = 11;
            this.FlowAdditionalButton.Text = "Доп. настройки";
            this.FlowAdditionalButton.UseVisualStyleBackColor = true;
            this.FlowAdditionalButton.Click += new System.EventHandler(this.FlowAdditionalButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 607);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.Settings.ResumeLayout(false);
            this.Settings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Settings;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label FlowmeterLabel;
        private System.Windows.Forms.ComboBox FlowComComboBox;
        private System.Windows.Forms.Button FlowSaveButton;
        private System.Windows.Forms.Button FlowConnectButton;
        private System.Windows.Forms.TextBox FlowStatusTextBox;
        private System.Windows.Forms.Label FlowStatusLabel;
        private System.Windows.Forms.TextBox FlowAddressTextBox;
        private System.Windows.Forms.Label FlowAddressLabel;
        private System.Windows.Forms.TextBox FlowSpeedTextBox;
        private System.Windows.Forms.Label FlowSpeedLabel;
        private System.Windows.Forms.Label FlowComLabel;
        private System.Windows.Forms.Button FlowAdditionalButton;
    }
}

