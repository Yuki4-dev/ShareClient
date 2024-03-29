﻿using ShareClientForm.Controls;
using SharedClientForm;
using SharedClientForm.Controls;
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
            this.ClientRadioBtn = new System.Windows.Forms.RadioButton();
            this.ServerRadioBtn = new System.Windows.Forms.RadioButton();
            this.PictureArea = new PictureArea();
            this.ClientHostTextBox = new System.Windows.Forms.TextBox();
            this.ClientPortTextBox = new System.Windows.Forms.TextBox();
            this.ServerPortTextBox = new System.Windows.Forms.TextBox();
            this.StartBtn = new System.Windows.Forms.Button();
            this.StopBtn = new System.Windows.Forms.Button();
            this.SpeedBtn = new DropButton();
            this.SpeedMeter = new SpeedMeter();
            this.SettingBtn = new DropButton();
            this.SuspendLayout();
            // 
            // ClientRadioBtn
            // 
            this.ClientRadioBtn.AutoSize = true;
            this.ClientRadioBtn.Location = new System.Drawing.Point(13, 23);
            this.ClientRadioBtn.Name = "ClientRadioBtn";
            this.ClientRadioBtn.Size = new System.Drawing.Size(74, 17);
            this.ClientRadioBtn.TabIndex = 0;
            this.ClientRadioBtn.Text = "クライアント";
            this.ClientRadioBtn.UseVisualStyleBackColor = true;
            this.ClientRadioBtn.CheckedChanged += new System.EventHandler(this.RudioBtn_CheckedChanged);
            // 
            // ServerRadioBtn
            // 
            this.ServerRadioBtn.AutoSize = true;
            this.ServerRadioBtn.Checked = true;
            this.ServerRadioBtn.Location = new System.Drawing.Point(13, 71);
            this.ServerRadioBtn.Name = "ServerRadioBtn";
            this.ServerRadioBtn.Size = new System.Drawing.Size(57, 17);
            this.ServerRadioBtn.TabIndex = 1;
            this.ServerRadioBtn.TabStop = true;
            this.ServerRadioBtn.Text = "サーバー";
            this.ServerRadioBtn.UseVisualStyleBackColor = true;
            this.ServerRadioBtn.CheckedChanged += new System.EventHandler(this.RudioBtn_CheckedChanged);
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
            this.ClientHostTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ClientHostTextBox.Location = new System.Drawing.Point(109, 14);
            this.ClientHostTextBox.Name = "ClientHostTextBox";
            this.ClientHostTextBox.Size = new System.Drawing.Size(212, 29);
            this.ClientHostTextBox.TabIndex = 3;
            // 
            // ClientPortTextBox
            // 
            this.ClientPortTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ClientPortTextBox.Location = new System.Drawing.Point(327, 14);
            this.ClientPortTextBox.Name = "ClientPortTextBox";
            this.ClientPortTextBox.Size = new System.Drawing.Size(100, 29);
            this.ClientPortTextBox.TabIndex = 4;
            // 
            // ServerPortTextBox
            // 
            this.ServerPortTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ServerPortTextBox.Location = new System.Drawing.Point(327, 62);
            this.ServerPortTextBox.Name = "ServerPortTextBox";
            this.ServerPortTextBox.Size = new System.Drawing.Size(100, 29);
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
            // SpeedBtn
            // 
            this.SpeedBtn.FlatAppearance.BorderSize = 0;
            this.SpeedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SpeedBtn.ForeColor = System.Drawing.Color.Gray;
            this.SpeedBtn.IsDrop = false;
            this.SpeedBtn.Location = new System.Drawing.Point(584, 73);
            this.SpeedBtn.Name = "SpeedBtn";
            this.SpeedBtn.Size = new System.Drawing.Size(55, 32);
            this.SpeedBtn.TabIndex = 7;
            this.SpeedBtn.Text = "<";
            this.SpeedBtn.UseVisualStyleBackColor = true;
            this.SpeedBtn.IsDropChanged += new System.EventHandler(this.SpeedBtn_IsDropChanged);
            // 
            // SpeedMeter
            // 
            this.SpeedMeter.Location = new System.Drawing.Point(489, 123);
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
            this.SettingBtn.IsDropChanged += new System.EventHandler(this.SettingBtn_IsDropChanged);
            // 
            // SharedClientMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(751, 455);
            this.Controls.Add(this.SettingBtn);
            this.Controls.Add(this.SpeedMeter);
            this.Controls.Add(this.SpeedBtn);
            this.Controls.Add(this.StopBtn);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.ServerPortTextBox);
            this.Controls.Add(this.ClientPortTextBox);
            this.Controls.Add(this.ClientHostTextBox);
            this.Controls.Add(this.PictureArea);
            this.Controls.Add(this.ServerRadioBtn);
            this.Controls.Add(this.ClientRadioBtn);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "SharedClientMainForm";
            this.Text = "SharedDisplay";
            this.Activated += new System.EventHandler(this.SharedClientMainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SharedClientMainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton ClientRadioBtn;
        private System.Windows.Forms.RadioButton ServerRadioBtn;
        private System.Windows.Forms.TextBox ClientPortTextBox;
        private System.Windows.Forms.TextBox ServerPortTextBox;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.TextBox ClientHostTextBox;
        private PictureArea PictureArea;
        private DropButton SpeedBtn;
        private SpeedMeter SpeedMeter;
        private DropButton SettingBtn;
    }
}

