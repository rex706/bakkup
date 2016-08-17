namespace bakkup
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listBoxBakkups = new System.Windows.Forms.ListBox();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.buttonNewBackup = new System.Windows.Forms.Button();
            this.ButtonImageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.checkBoxAutoRun = new System.Windows.Forms.CheckBox();
            this.buttonRetrieve = new System.Windows.Forms.Button();
            this.buttonBackup = new System.Windows.Forms.Button();
            this.linkLabelVersion = new System.Windows.Forms.LinkLabel();
            this.buttonRemove = new System.Windows.Forms.Button();
<<<<<<< HEAD
            this.ButtonImageList = new System.Windows.Forms.ImageList(this.components);
            this.SeparatorLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.WriteTimeListBox = new System.Windows.Forms.ListBox();
=======
            this.SeparatorLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.serviceLabel = new System.Windows.Forms.Label();
            this.ServiceImageList = new System.Windows.Forms.ImageList(this.components);
            this.listBoxWriteTime = new System.Windows.Forms.ListBox();
>>>>>>> origin/master
            this.SuspendLayout();
            // 
            // listBoxBakkups
            // 
            this.listBoxBakkups.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.listBoxBakkups.FormattingEnabled = true;
            this.listBoxBakkups.ItemHeight = 16;
            this.listBoxBakkups.Location = new System.Drawing.Point(12, 34);
            this.listBoxBakkups.Name = "listBoxBakkups";
<<<<<<< HEAD
            this.listBoxBakkups.Size = new System.Drawing.Size(174, 148);
=======
            this.listBoxBakkups.Size = new System.Drawing.Size(308, 148);
>>>>>>> origin/master
            this.listBoxBakkups.TabIndex = 0;
            this.listBoxBakkups.SelectedIndexChanged += new System.EventHandler(this.listBoxBakkups_SelectedIndexChanged);
            // 
            // InfoLabel
            // 
<<<<<<< HEAD
            this.label1.Font = new System.Drawing.Font("Calibri Light", 10F);
            this.label1.Location = new System.Drawing.Point(60, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "# bakkups found";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(102, 263);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(75, 46);
=======
            this.InfoLabel.Font = new System.Drawing.Font("Calibri Light", 10F);
            this.InfoLabel.Location = new System.Drawing.Point(85, 9);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(157, 16);
            this.InfoLabel.TabIndex = 1;
            this.InfoLabel.Text = "# bakkups found";
            this.InfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.InfoLabel.Visible = false;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Location = new System.Drawing.Point(127, 267);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(75, 36);
>>>>>>> origin/master
            this.buttonSelect.TabIndex = 4;
            this.buttonSelect.Text = "Select";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // buttonNewBackup
            // 
            this.buttonNewBackup.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.buttonNewBackup.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonNewBackup.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonNewBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNewBackup.ImageIndex = 1;
            this.buttonNewBackup.ImageList = this.ButtonImageList;
<<<<<<< HEAD
            this.buttonNewBackup.Location = new System.Drawing.Point(52, 185);
=======
            this.buttonNewBackup.Location = new System.Drawing.Point(77, 185);
>>>>>>> origin/master
            this.buttonNewBackup.Name = "buttonNewBackup";
            this.buttonNewBackup.Size = new System.Drawing.Size(40, 32);
            this.buttonNewBackup.TabIndex = 5;
            this.toolTip.SetToolTip(this.buttonNewBackup, "Create new entry");
            this.buttonNewBackup.UseVisualStyleBackColor = true;
            this.buttonNewBackup.Click += new System.EventHandler(this.buttonNewBackup_Click);
            this.buttonNewBackup.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonNewBackup_MouseDown);
            this.buttonNewBackup.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonNewBackup_Mouseup);
            // 
            // ButtonImageList
            // 
            this.ButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageList.ImageStream")));
            this.ButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageList.Images.SetKeyName(0, "refreshflat.png");
            this.ButtonImageList.Images.SetKeyName(1, "plus.png");
            this.ButtonImageList.Images.SetKeyName(2, "minus.png");
            this.ButtonImageList.Images.SetKeyName(3, "refreshflatclicked.png");
            this.ButtonImageList.Images.SetKeyName(4, "plusclicked.png");
            this.ButtonImageList.Images.SetKeyName(5, "minusclicked.png");
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.buttonRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefresh.ImageIndex = 0;
            this.buttonRefresh.ImageList = this.ButtonImageList;
<<<<<<< HEAD
            this.buttonRefresh.Location = new System.Drawing.Point(123, 185);
=======
            this.buttonRefresh.Location = new System.Drawing.Point(148, 185);
>>>>>>> origin/master
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(31, 37);
            this.buttonRefresh.TabIndex = 7;
            this.toolTip.SetToolTip(this.buttonRefresh, "Refresh list");
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            this.buttonRefresh.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonRefresh_MouseDown);
            this.buttonRefresh.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonRefresh_MouseUp);
            // 
            // checkBoxAutoRun
            // 
            this.checkBoxAutoRun.AutoSize = true;
            this.checkBoxAutoRun.Checked = true;
            this.checkBoxAutoRun.CheckState = System.Windows.Forms.CheckState.Checked;
<<<<<<< HEAD
            this.checkBoxAutoRun.Location = new System.Drawing.Point(106, 239);
=======
            this.checkBoxAutoRun.Location = new System.Drawing.Point(131, 239);
>>>>>>> origin/master
            this.checkBoxAutoRun.Name = "checkBoxAutoRun";
            this.checkBoxAutoRun.Size = new System.Drawing.Size(71, 17);
            this.checkBoxAutoRun.TabIndex = 9;
            this.checkBoxAutoRun.Text = "Auto-Run";
            this.toolTip.SetToolTip(this.checkBoxAutoRun, "Enable/disable auto retrieval and backup");
            this.checkBoxAutoRun.UseVisualStyleBackColor = true;
            this.checkBoxAutoRun.CheckedChanged += new System.EventHandler(this.checkBoxAutoRun_CheckedChanged);
            // 
            // buttonRetrieve
            // 
<<<<<<< HEAD
            this.buttonRetrieve.Location = new System.Drawing.Point(102, 263);
=======
            this.buttonRetrieve.Location = new System.Drawing.Point(127, 262);
>>>>>>> origin/master
            this.buttonRetrieve.Name = "buttonRetrieve";
            this.buttonRetrieve.Size = new System.Drawing.Size(75, 23);
            this.buttonRetrieve.TabIndex = 10;
            this.buttonRetrieve.Text = "Retrieve";
            this.buttonRetrieve.UseVisualStyleBackColor = true;
            this.buttonRetrieve.Visible = false;
            this.buttonRetrieve.Click += new System.EventHandler(this.buttonRetrieve_Click);
            // 
            // buttonBackup
            // 
<<<<<<< HEAD
            this.buttonBackup.Location = new System.Drawing.Point(102, 286);
=======
            this.buttonBackup.Location = new System.Drawing.Point(127, 286);
>>>>>>> origin/master
            this.buttonBackup.Name = "buttonBackup";
            this.buttonBackup.Size = new System.Drawing.Size(75, 23);
            this.buttonBackup.TabIndex = 11;
            this.buttonBackup.Text = "Backup";
            this.buttonBackup.UseVisualStyleBackColor = true;
            this.buttonBackup.Visible = false;
            this.buttonBackup.Click += new System.EventHandler(this.buttonBackup_Click);
            // 
            // linkLabelVersion
            // 
            this.linkLabelVersion.AutoSize = true;
            this.linkLabelVersion.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelVersion.LinkColor = System.Drawing.Color.Black;
            this.linkLabelVersion.Location = new System.Drawing.Point(9, 9);
            this.linkLabelVersion.Name = "linkLabelVersion";
            this.linkLabelVersion.Size = new System.Drawing.Size(20, 13);
            this.linkLabelVersion.TabIndex = 12;
            this.linkLabelVersion.TabStop = true;
            this.linkLabelVersion.Tag = "";
<<<<<<< HEAD
            this.linkLabelVersion.Text = "v0.5";
=======
            this.linkLabelVersion.Text = "v#";
            this.linkLabelVersion.Visible = false;
>>>>>>> origin/master
            this.linkLabelVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVersion_LinkClicked);
            // 
            // buttonRemove
            // 
            this.buttonRemove.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRemove.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.buttonRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.buttonRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRemove.ImageIndex = 2;
            this.buttonRemove.ImageList = this.ButtonImageList;
<<<<<<< HEAD
            this.buttonRemove.Location = new System.Drawing.Point(190, 196);
=======
            this.buttonRemove.Location = new System.Drawing.Point(215, 196);
>>>>>>> origin/master
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(39, 10);
            this.buttonRemove.TabIndex = 13;
            this.toolTip.SetToolTip(this.buttonRemove, "Remove entry");
            this.buttonRemove.UseVisualStyleBackColor = false;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            this.buttonRemove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonRemove_MouseDown);
            this.buttonRemove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonRemove_Mouseup);
            // 
            // SeparatorLabel
            // 
            this.SeparatorLabel.AutoSize = true;
            this.SeparatorLabel.Location = new System.Drawing.Point(37, 217);
            this.SeparatorLabel.Name = "SeparatorLabel";
            this.SeparatorLabel.Size = new System.Drawing.Size(259, 13);
            this.SeparatorLabel.TabIndex = 14;
            this.SeparatorLabel.Text = "__________________________________________";
            // 
            // serviceLabel
            // 
            this.serviceLabel.AutoSize = true;
            this.serviceLabel.ImageIndex = 0;
            this.serviceLabel.ImageList = this.ServiceImageList;
            this.serviceLabel.Location = new System.Drawing.Point(302, 11);
            this.serviceLabel.Name = "serviceLabel";
            this.serviceLabel.Size = new System.Drawing.Size(16, 13);
            this.serviceLabel.TabIndex = 16;
            this.serviceLabel.Text = "   ";
            this.toolTip.SetToolTip(this.serviceLabel, "Click to change provider");
            this.serviceLabel.Visible = false;
            this.serviceLabel.Click += new System.EventHandler(this.serviceLabel_Click);
            // 
            // ServiceImageList
            // 
            this.ServiceImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ServiceImageList.ImageStream")));
            this.ServiceImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ServiceImageList.Images.SetKeyName(0, "Google-Drive-icon.png");
            this.ServiceImageList.Images.SetKeyName(1, "42736.png");
            this.ServiceImageList.Images.SetKeyName(2, "dbnoborder.png");
            // 
            // listBoxWriteTime
            // 
            this.listBoxWriteTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxWriteTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.listBoxWriteTime.FormattingEnabled = true;
            this.listBoxWriteTime.ItemHeight = 16;
            this.listBoxWriteTime.Location = new System.Drawing.Point(175, 36);
            this.listBoxWriteTime.Name = "listBoxWriteTime";
            this.listBoxWriteTime.Size = new System.Drawing.Size(143, 144);
            this.listBoxWriteTime.TabIndex = 15;
            this.listBoxWriteTime.SelectedIndexChanged += new System.EventHandler(this.listBoxWriteTime_SelectedIndexChanged);
            // 
            // ButtonImageList
            // 
            this.ButtonImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonImageList.ImageStream")));
            this.ButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonImageList.Images.SetKeyName(0, "refreshflat.png");
            this.ButtonImageList.Images.SetKeyName(1, "plus.png");
            this.ButtonImageList.Images.SetKeyName(2, "minus.png");
            this.ButtonImageList.Images.SetKeyName(3, "refreshflatclicked.png");
            this.ButtonImageList.Images.SetKeyName(4, "plusclicked.png");
            this.ButtonImageList.Images.SetKeyName(5, "minusclicked.png");
            // 
            // SeparatorLabel
            // 
            this.SeparatorLabel.AutoSize = true;
            this.SeparatorLabel.Location = new System.Drawing.Point(12, 217);
            this.SeparatorLabel.Name = "SeparatorLabel";
            this.SeparatorLabel.Size = new System.Drawing.Size(259, 13);
            this.SeparatorLabel.TabIndex = 14;
            this.SeparatorLabel.Text = "__________________________________________";
            // 
            // WriteTimeListBox
            // 
            this.WriteTimeListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.WriteTimeListBox.FormattingEnabled = true;
            this.WriteTimeListBox.ItemHeight = 16;
            this.WriteTimeListBox.Location = new System.Drawing.Point(138, 34);
            this.WriteTimeListBox.Name = "WriteTimeListBox";
            this.WriteTimeListBox.Size = new System.Drawing.Size(135, 148);
            this.WriteTimeListBox.TabIndex = 15;
            this.WriteTimeListBox.SelectedIndexChanged += new System.EventHandler(this.WriteTimeListBox_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
<<<<<<< HEAD
            this.ClientSize = new System.Drawing.Size(284, 321);
            this.Controls.Add(this.WriteTimeListBox);
=======
            this.ClientSize = new System.Drawing.Size(332, 321);
            this.Controls.Add(this.serviceLabel);
            this.Controls.Add(this.listBoxWriteTime);
>>>>>>> origin/master
            this.Controls.Add(this.SeparatorLabel);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.linkLabelVersion);
            this.Controls.Add(this.buttonBackup);
            this.Controls.Add(this.buttonRetrieve);
            this.Controls.Add(this.checkBoxAutoRun);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonNewBackup);
            this.Controls.Add(this.buttonSelect);
            this.Controls.Add(this.InfoLabel);
            this.Controls.Add(this.listBoxBakkups);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "bakkup";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxBakkups;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonNewBackup;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.CheckBox checkBoxAutoRun;
        private System.Windows.Forms.Button buttonRetrieve;
        private System.Windows.Forms.Button buttonBackup;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ImageList ButtonImageList;
        private System.Windows.Forms.Label SeparatorLabel;
        private System.Windows.Forms.ToolTip toolTip;
<<<<<<< HEAD
        private System.Windows.Forms.ListBox WriteTimeListBox;
=======
        private System.Windows.Forms.ListBox listBoxWriteTime;
        private System.Windows.Forms.ImageList ServiceImageList;
        private System.Windows.Forms.Label serviceLabel;
        private System.Windows.Forms.LinkLabel linkLabelVersion;
>>>>>>> origin/master
    }
}