using System.Drawing;

namespace SharedClientForm
{
    partial class DisplaySelectForm
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
            this.WindowTextList = new System.Windows.Forms.ListBox();
            this.DisplayArea = new PictureArea();
            this.SelectBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WindowTextList
            // 
            this.WindowTextList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.WindowTextList.FormattingEnabled = true;
            this.WindowTextList.HorizontalScrollbar = true;
            this.WindowTextList.IntegralHeight = false;
            this.WindowTextList.ItemHeight = 15;
            this.WindowTextList.Location = new System.Drawing.Point(7, 4);
            this.WindowTextList.Name = "WindowTextList";
            this.WindowTextList.Size = new System.Drawing.Size(250, 439);
            this.WindowTextList.TabIndex = 0;
            this.WindowTextList.SelectedIndexChanged += new System.EventHandler(this.WindowTextList_SelectedIndexChanged);
            this.WindowTextList.DoubleClick += new System.EventHandler(this.WindowTextList_DoubleClick);
            // 
            // DisplayArea
            // 
            this.DisplayArea.DefaultPicture = null;
            this.DisplayArea.Location = new System.Drawing.Point(266, 9);
            this.DisplayArea.Name = "DisplayArea";
            this.DisplayArea.Size = new System.Drawing.Size(522, 398);
            this.DisplayArea.TabIndex = 1;
            this.DisplayArea.Text = "pictureArea1";
            // 
            // SelectBtn
            // 
            this.SelectBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectBtn.Location = new System.Drawing.Point(699, 413);
            this.SelectBtn.Name = "SelectBtn";
            this.SelectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelectBtn.Size = new System.Drawing.Size(89, 30);
            this.SelectBtn.TabIndex = 2;
            this.SelectBtn.Text = "選択";
            this.SelectBtn.UseVisualStyleBackColor = true;
            this.SelectBtn.Click += new System.EventHandler(this.SelectBtn_Click);
            // 
            // SheardDisplaySelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.BackColor = Color.White;
            this.Controls.Add(this.SelectBtn);
            this.Controls.Add(this.WindowTextList);
            this.Controls.Add(this.DisplayArea);
            this.Name = "SheardDisplaySelectForm";
            this.Text = "SheardDisplaySelectForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox WindowTextList;
        private PictureArea DisplayArea;
        private System.Windows.Forms.Button SelectBtn;
    }
}