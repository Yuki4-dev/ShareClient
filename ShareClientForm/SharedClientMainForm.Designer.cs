using SharedClientForm;
using System.Drawing;

namespace SharedDisplayForm
{
    public partial class SharedClientMainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SharedClientMainForm));
            this.ClientRudioBtn = new System.Windows.Forms.RadioButton();
            this.ServerRudioBtn = new System.Windows.Forms.RadioButton();
            this.PictureArea = new SharedClientForm.PictureArea();
            this.ClientHostTextBox = new System.Windows.Forms.TextBox();
            this.ClientPortTextBox = new System.Windows.Forms.TextBox();
            this.ServerPortTextBox = new System.Windows.Forms.TextBox();
            this.StartBtn = new System.Windows.Forms.Button();
            this.StopBtn = new System.Windows.Forms.Button();
            this.ImageCmb = new System.Windows.Forms.ComboBox();
            this.SpeedBtn = new SharedClientForm.DropButton();
            this.SpeedMeter = new SharedClientForm.SpeedMeter();
            this.SettingBtn = new SharedClientForm.DropButton();
            this.SuspendLayout();
            // 
            // ClientRudioBtn
            // 
            this.ClientRudioBtn.AutoSize = true;
            this.ClientRudioBtn.Location = new System.Drawing.Point(13, 23);
            this.ClientRudioBtn.Name = "ClientRudioBtn";
            this.ClientRudioBtn.Size = new System.Drawing.Size(92, 23);
            this.ClientRudioBtn.TabIndex = 0;
            this.ClientRudioBtn.Text = "クライアント";
            this.ClientRudioBtn.UseVisualStyleBackColor = true;
            this.ClientRudioBtn.CheckedChanged += new System.EventHandler(this.ClientRudioBtn_CheckedChanged);
            // 
            // ServerRudioBtn
            // 
            this.ServerRudioBtn.AutoSize = true;
            this.ServerRudioBtn.Checked = true;
            this.ServerRudioBtn.Location = new System.Drawing.Point(13, 71);
            this.ServerRudioBtn.Name = "ServerRudioBtn";
            this.ServerRudioBtn.Size = new System.Drawing.Size(70, 23);
            this.ServerRudioBtn.TabIndex = 1;
            this.ServerRudioBtn.TabStop = true;
            this.ServerRudioBtn.Text = "サーバー";
            this.ServerRudioBtn.UseVisualStyleBackColor = true;
            this.ServerRudioBtn.CheckedChanged += new System.EventHandler(this.ServerRudioBtn_CheckedChanged);
            // 
            // PictureArea
            // 
            this.PictureArea.DefaultPicture = null;
            this.PictureArea.Location = new System.Drawing.Point(13, 110);
            this.PictureArea.Name = "PictureArea";
            this.PictureArea.Size = new System.Drawing.Size(726, 333);
            this.PictureArea.TabIndex = 2;
            this.PictureArea.Text = "PictureArea";
            // 
            // ClientHostTextBox
            // 
            this.ClientHostTextBox.Location = new System.Drawing.Point(109, 23);
            this.ClientHostTextBox.Name = "ClientHostTextBox";
            this.ClientHostTextBox.Size = new System.Drawing.Size(212, 25);
            this.ClientHostTextBox.TabIndex = 3;
            // 
            // ClientPortTextBox
            // 
            this.ClientPortTextBox.Location = new System.Drawing.Point(327, 23);
            this.ClientPortTextBox.Name = "ClientPortTextBox";
            this.ClientPortTextBox.Size = new System.Drawing.Size(100, 25);
            this.ClientPortTextBox.TabIndex = 4;
            // 
            // ServerPortTextBox
            // 
            this.ServerPortTextBox.Location = new System.Drawing.Point(327, 71);
            this.ServerPortTextBox.Name = "ServerPortTextBox";
            this.ServerPortTextBox.Size = new System.Drawing.Size(100, 25);
            this.ServerPortTextBox.TabIndex = 4;
            // 
            // StartBtn
            // 
            this.StartBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartBtn.Location = new System.Drawing.Point(442, 13);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(75, 43);
            this.StartBtn.TabIndex = 5;
            this.StartBtn.Text = "開始";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // StopBtn
            // 
            this.StopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopBtn.Location = new System.Drawing.Point(442, 62);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(75, 41);
            this.StopBtn.TabIndex = 5;
            this.StopBtn.Text = "停止";
            this.StopBtn.UseVisualStyleBackColor = true;
            this.StopBtn.Click += new System.EventHandler(this.StopBtn_Click);
            // 
            // ImageCmb
            // 
            this.ImageCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ImageCmb.FormattingEnabled = true;
            this.ImageCmb.Location = new System.Drawing.Point(523, 13);
            this.ImageCmb.Name = "ImageCmb";
            this.ImageCmb.Size = new System.Drawing.Size(71, 25);
            this.ImageCmb.TabIndex = 6;
            // 
            // SpeedBtn
            // 
            this.SpeedBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpeedBtn.FlatAppearance.BorderSize = 0;
            this.SpeedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SpeedBtn.ForeColor = System.Drawing.Color.Gray;
            this.SpeedBtn.IsDrop = false;
            this.SpeedBtn.Location = new System.Drawing.Point(684, 74);
            this.SpeedBtn.Name = "SpeedBtn";
            this.SpeedBtn.Size = new System.Drawing.Size(55, 32);
            this.SpeedBtn.TabIndex = 7;
            this.SpeedBtn.Text = "<";
            this.SpeedBtn.UseVisualStyleBackColor = true;
            this.SpeedBtn.IsDropChanged += SpeedBtn_IsDropChanged;
            // 
            // SpeedMeter
            // 
            this.SpeedMeter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpeedMeter.Location = new System.Drawing.Point(489, 110);
            this.SpeedMeter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SpeedMeter.Name = "SpeedMeter";
            this.SpeedMeter.Size = new System.Drawing.Size(250, 147);
            this.SpeedMeter.TabIndex = 8;
            this.SpeedMeter.Visible = false;
            // 
            // SettingBtn
            // 
            this.SettingBtn.FlatAppearance.BorderSize = 0;
            this.SettingBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingBtn.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SettingBtn.ForeColor = System.Drawing.Color.Gray;
            this.SettingBtn.IsDrop = false;
            this.SettingBtn.Location = new System.Drawing.Point(523, 71);
            this.SettingBtn.Name = "SettingBtn";
            this.SettingBtn.Size = new System.Drawing.Size(55, 32);
            this.SettingBtn.TabIndex = 0;
            this.SettingBtn.Text = "...";
            this.SettingBtn.IsDropChanged += SettingBtn_IsDropChanged;
            // 
            // SharedClientMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(751, 455);
            this.Controls.Add(this.SettingBtn);
            this.Controls.Add(this.SpeedMeter);
            this.Controls.Add(this.SpeedBtn);
            this.Controls.Add(this.ImageCmb);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.ServerPortTextBox);
            this.Controls.Add(this.ClientPortTextBox);
            this.Controls.Add(this.ClientHostTextBox);
            this.Controls.Add(this.PictureArea);
            this.Controls.Add(this.ServerRudioBtn);
            this.Controls.Add(this.ClientRudioBtn);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "SharedClientMainForm";
            this.Text = "SharedDisplay";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton ClientRudioBtn;
        private System.Windows.Forms.RadioButton ServerRudioBtn;
        private System.Windows.Forms.TextBox ClientPortTextBox;
        private System.Windows.Forms.TextBox ServerPortTextBox;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.TextBox ClientHostTextBox;
        private System.Windows.Forms.ComboBox ImageCmb;
        private PictureArea PictureArea;
        private DropButton SpeedBtn;
        private SpeedMeter SpeedMeter;
        private DropButton SettingBtn;
    }
}

