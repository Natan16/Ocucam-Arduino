namespace VMR9Playback
{
    partial class CaptureSelection
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
            this.leftEyeDevice = new System.Windows.Forms.ComboBox();
            this.rightEyeDevice = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxDisplay = new System.Windows.Forms.ComboBox();
            this.checkBoxFullScreen = new System.Windows.Forms.CheckBox();
            this.RiftDetectedText = new System.Windows.Forms.Label();
            this.detectedLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.XLabel = new System.Windows.Forms.Label();
            this.YLabel = new System.Windows.Forms.Label();
            this.ZLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.resolutionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left eye device:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right eye device:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // leftEyeDevice
            // 
            this.leftEyeDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.leftEyeDevice.FormattingEnabled = true;
            this.leftEyeDevice.Location = new System.Drawing.Point(101, 6);
            this.leftEyeDevice.Name = "leftEyeDevice";
            this.leftEyeDevice.Size = new System.Drawing.Size(410, 21);
            this.leftEyeDevice.TabIndex = 2;
            // 
            // rightEyeDevice
            // 
            this.rightEyeDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rightEyeDevice.FormattingEnabled = true;
            this.rightEyeDevice.Location = new System.Drawing.Point(101, 33);
            this.rightEyeDevice.Name = "rightEyeDevice";
            this.rightEyeDevice.Size = new System.Drawing.Size(410, 21);
            this.rightEyeDevice.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(355, 86);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(436, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Exit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Display:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxDisplay
            // 
            this.comboBoxDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplay.FormattingEnabled = true;
            this.comboBoxDisplay.Location = new System.Drawing.Point(101, 59);
            this.comboBoxDisplay.Name = "comboBoxDisplay";
            this.comboBoxDisplay.Size = new System.Drawing.Size(410, 21);
            this.comboBoxDisplay.TabIndex = 7;
            // 
            // checkBoxFullScreen
            // 
            this.checkBoxFullScreen.AutoSize = true;
            this.checkBoxFullScreen.Location = new System.Drawing.Point(101, 86);
            this.checkBoxFullScreen.Name = "checkBoxFullScreen";
            this.checkBoxFullScreen.Size = new System.Drawing.Size(77, 17);
            this.checkBoxFullScreen.TabIndex = 8;
            this.checkBoxFullScreen.Text = "Full screen";
            this.checkBoxFullScreen.UseVisualStyleBackColor = true;
            // 
            // RiftDetectedText
            // 
            this.RiftDetectedText.AutoSize = true;
            this.RiftDetectedText.Location = new System.Drawing.Point(8, 152);
            this.RiftDetectedText.Name = "RiftDetectedText";
            this.RiftDetectedText.Size = new System.Drawing.Size(76, 13);
            this.RiftDetectedText.TabIndex = 9;
            this.RiftDetectedText.Text = "Rift Detected: ";
            // 
            // detectedLabel
            // 
            this.detectedLabel.AutoSize = true;
            this.detectedLabel.Location = new System.Drawing.Point(93, 152);
            this.detectedLabel.Name = "detectedLabel";
            this.detectedLabel.Size = new System.Drawing.Size(53, 13);
            this.detectedLabel.TabIndex = 10;
            this.detectedLabel.Text = "<starting>";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "X: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Y: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 221);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Z: ";
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Location = new System.Drawing.Point(93, 175);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(87, 13);
            this.XLabel.TabIndex = 14;
            this.XLabel.Text = "No Rift Detected";
            // 
            // YLabel
            // 
            this.YLabel.AutoSize = true;
            this.YLabel.Location = new System.Drawing.Point(93, 198);
            this.YLabel.Name = "YLabel";
            this.YLabel.Size = new System.Drawing.Size(87, 13);
            this.YLabel.TabIndex = 15;
            this.YLabel.Text = "No Rift Detected";
            // 
            // ZLabel
            // 
            this.ZLabel.AutoSize = true;
            this.ZLabel.Location = new System.Drawing.Point(93, 221);
            this.ZLabel.Name = "ZLabel";
            this.ZLabel.Size = new System.Drawing.Size(87, 13);
            this.ZLabel.TabIndex = 16;
            this.ZLabel.Text = "No Rift Detected";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 243);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Resolution: ";
            // 
            // resolutionLabel
            // 
            this.resolutionLabel.AutoSize = true;
            this.resolutionLabel.Location = new System.Drawing.Point(93, 243);
            this.resolutionLabel.Name = "resolutionLabel";
            this.resolutionLabel.Size = new System.Drawing.Size(87, 13);
            this.resolutionLabel.TabIndex = 18;
            this.resolutionLabel.Text = "No Rift Detected";
            // 
            // CaptureSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 291);
            this.Controls.Add(this.resolutionLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ZLabel);
            this.Controls.Add(this.YLabel);
            this.Controls.Add(this.XLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.detectedLabel);
            this.Controls.Add(this.RiftDetectedText);
            this.Controls.Add(this.checkBoxFullScreen);
            this.Controls.Add(this.comboBoxDisplay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.rightEyeDevice);
            this.Controls.Add(this.leftEyeDevice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CaptureSelection";
            this.Text = "Ocucam - Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox leftEyeDevice;
        private System.Windows.Forms.ComboBox rightEyeDevice;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxDisplay;
        private System.Windows.Forms.CheckBox checkBoxFullScreen;
        private System.Windows.Forms.Label RiftDetectedText;
        private System.Windows.Forms.Label detectedLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Label YLabel;
        private System.Windows.Forms.Label ZLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label resolutionLabel;
    }
}