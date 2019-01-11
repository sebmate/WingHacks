namespace WWFix
{
    partial class WWFixForm
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
            this.logText = new System.Windows.Forms.RichTextBox();
            this.volumeLabel = new System.Windows.Forms.Label();
            this.accRunningLabel = new System.Windows.Forms.Label();
            this.userPlayingLabel = new System.Windows.Forms.Label();
            this.drawBar16 = new System.Windows.Forms.TrackBar();
            this.drawBar5 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Off16 = new System.Windows.Forms.Button();
            this.Off5 = new System.Windows.Forms.Button();
            this.Full5 = new System.Windows.Forms.Button();
            this.Full16 = new System.Windows.Forms.Button();
            this.logImportant = new System.Windows.Forms.CheckBox();
            this.logMIDI = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.drawBar16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.drawBar5)).BeginInit();
            this.SuspendLayout();
            // 
            // logText
            // 
            this.logText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logText.Location = new System.Drawing.Point(12, 12);
            this.logText.Name = "logText";
            this.logText.Size = new System.Drawing.Size(514, 376);
            this.logText.TabIndex = 0;
            this.logText.Text = "";
            // 
            // volumeLabel
            // 
            this.volumeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.volumeLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.volumeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeLabel.Location = new System.Drawing.Point(542, 12);
            this.volumeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.volumeLabel.Name = "volumeLabel";
            this.volumeLabel.Size = new System.Drawing.Size(176, 23);
            this.volumeLabel.TabIndex = 1;
            this.volumeLabel.Text = "Master Volume: 0";
            this.volumeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.volumeLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // accRunningLabel
            // 
            this.accRunningLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.accRunningLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accRunningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accRunningLabel.Location = new System.Drawing.Point(542, 41);
            this.accRunningLabel.Margin = new System.Windows.Forms.Padding(3);
            this.accRunningLabel.Name = "accRunningLabel";
            this.accRunningLabel.Size = new System.Drawing.Size(176, 23);
            this.accRunningLabel.TabIndex = 2;
            this.accRunningLabel.Text = "Acc Not Running";
            this.accRunningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.accRunningLabel.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // userPlayingLabel
            // 
            this.userPlayingLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.userPlayingLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.userPlayingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userPlayingLabel.Location = new System.Drawing.Point(542, 70);
            this.userPlayingLabel.Margin = new System.Windows.Forms.Padding(3);
            this.userPlayingLabel.Name = "userPlayingLabel";
            this.userPlayingLabel.Size = new System.Drawing.Size(176, 23);
            this.userPlayingLabel.TabIndex = 3;
            this.userPlayingLabel.Text = "User Not Playing";
            this.userPlayingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // drawBar16
            // 
            this.drawBar16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(100)))), ((int)(((byte)(0)))));
            this.drawBar16.Location = new System.Drawing.Point(588, 141);
            this.drawBar16.Maximum = 127;
            this.drawBar16.Name = "drawBar16";
            this.drawBar16.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.drawBar16.Size = new System.Drawing.Size(42, 209);
            this.drawBar16.SmallChange = 32;
            this.drawBar16.TabIndex = 4;
            this.drawBar16.TickFrequency = 16;
            this.drawBar16.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.drawBar16.Value = 127;
            this.drawBar16.Scroll += new System.EventHandler(this.drawBar16_Scroll);
            this.drawBar16.ValueChanged += new System.EventHandler(this.drawBar16_ValueChanged);
            this.drawBar16.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawBar16_MouseUp);
            // 
            // drawBar5
            // 
            this.drawBar5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(100)))), ((int)(((byte)(0)))));
            this.drawBar5.Location = new System.Drawing.Point(636, 141);
            this.drawBar5.Maximum = 127;
            this.drawBar5.Name = "drawBar5";
            this.drawBar5.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.drawBar5.Size = new System.Drawing.Size(42, 209);
            this.drawBar5.SmallChange = 32;
            this.drawBar5.TabIndex = 5;
            this.drawBar5.TickFrequency = 16;
            this.drawBar5.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.drawBar5.Value = 127;
            this.drawBar5.Scroll += new System.EventHandler(this.drawBar5_Scroll);
            this.drawBar5.ValueChanged += new System.EventHandler(this.drawBar5_ValueChanged);
            this.drawBar5.MouseUp += new System.Windows.Forms.MouseEventHandler(this.drawBar5_MouseUp);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(588, 353);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "16";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(636, 353);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "5";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(655, 353);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "1/3";
            // 
            // Off16
            // 
            this.Off16.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Off16.Location = new System.Drawing.Point(588, 110);
            this.Off16.Name = "Off16";
            this.Off16.Size = new System.Drawing.Size(42, 25);
            this.Off16.TabIndex = 9;
            this.Off16.Text = "OFF";
            this.Off16.UseVisualStyleBackColor = true;
            this.Off16.Click += new System.EventHandler(this.Off16_Click);
            // 
            // Off5
            // 
            this.Off5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Off5.Location = new System.Drawing.Point(636, 110);
            this.Off5.Name = "Off5";
            this.Off5.Size = new System.Drawing.Size(42, 25);
            this.Off5.TabIndex = 10;
            this.Off5.Text = "OFF";
            this.Off5.UseVisualStyleBackColor = true;
            this.Off5.Click += new System.EventHandler(this.Off5_Click);
            // 
            // Full5
            // 
            this.Full5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Full5.Location = new System.Drawing.Point(636, 379);
            this.Full5.Name = "Full5";
            this.Full5.Size = new System.Drawing.Size(42, 25);
            this.Full5.TabIndex = 12;
            this.Full5.Text = "FULL";
            this.Full5.UseVisualStyleBackColor = true;
            this.Full5.Click += new System.EventHandler(this.Full5_Click);
            // 
            // Full16
            // 
            this.Full16.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Full16.Location = new System.Drawing.Point(588, 379);
            this.Full16.Name = "Full16";
            this.Full16.Size = new System.Drawing.Size(42, 25);
            this.Full16.TabIndex = 11;
            this.Full16.Text = "FULL";
            this.Full16.UseVisualStyleBackColor = true;
            this.Full16.Click += new System.EventHandler(this.Full16_Click);
            // 
            // logImportant
            // 
            this.logImportant.AutoSize = true;
            this.logImportant.Checked = true;
            this.logImportant.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logImportant.Location = new System.Drawing.Point(12, 394);
            this.logImportant.Name = "logImportant";
            this.logImportant.Size = new System.Drawing.Size(140, 17);
            this.logImportant.TabIndex = 13;
            this.logImportant.Text = "Log important messages";
            this.logImportant.UseVisualStyleBackColor = true;
            this.logImportant.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.logImportant.Click += new System.EventHandler(this.logImportant_Click);
            // 
            // logMIDI
            // 
            this.logMIDI.AutoSize = true;
            this.logMIDI.Location = new System.Drawing.Point(158, 394);
            this.logMIDI.Name = "logMIDI";
            this.logMIDI.Size = new System.Drawing.Size(120, 17);
            this.logMIDI.TabIndex = 14;
            this.logMIDI.Text = "Log MIDI messages";
            this.logMIDI.UseVisualStyleBackColor = true;
            this.logMIDI.Click += new System.EventHandler(this.logMIDI_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(684, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(684, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(684, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(684, 217);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(684, 240);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "4";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(684, 263);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "5";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(684, 286);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "6";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(684, 309);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "7";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(684, 330);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "8";
            // 
            // WWFixForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 423);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.logMIDI);
            this.Controls.Add(this.logImportant);
            this.Controls.Add(this.Full5);
            this.Controls.Add(this.Full16);
            this.Controls.Add(this.Off5);
            this.Controls.Add(this.Off16);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.drawBar5);
            this.Controls.Add(this.drawBar16);
            this.Controls.Add(this.userPlayingLabel);
            this.Controls.Add(this.accRunningLabel);
            this.Controls.Add(this.volumeLabel);
            this.Controls.Add(this.logText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WWFixForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WWFix 0.2 by Sebastian Mate";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WWFixForm_FormClosed);
            this.Load += new System.EventHandler(this.WWFixForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.drawBar16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.drawBar5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logText;
        private System.Windows.Forms.Label volumeLabel;
        private System.Windows.Forms.Label accRunningLabel;
        private System.Windows.Forms.Label userPlayingLabel;
        private System.Windows.Forms.TrackBar drawBar16;
        private System.Windows.Forms.TrackBar drawBar5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Off16;
        private System.Windows.Forms.Button Off5;
        private System.Windows.Forms.Button Full5;
        private System.Windows.Forms.Button Full16;
        private System.Windows.Forms.CheckBox logImportant;
        private System.Windows.Forms.CheckBox logMIDI;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
    }
}