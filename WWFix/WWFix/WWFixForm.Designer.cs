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
            ((System.ComponentModel.ISupportInitialize)(this.drawBar16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.drawBar5)).BeginInit();
            this.SuspendLayout();
            // 
            // logText
            // 
            this.logText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logText.Location = new System.Drawing.Point(12, 12);
            this.logText.Name = "logText";
            this.logText.Size = new System.Drawing.Size(514, 392);
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
            // WWFixForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 423);
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
            this.Text = "WWFix 0.1 by Sebastian Mate";
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
    }
}