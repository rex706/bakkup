﻿namespace bakkup
{
    partial class ServicePickerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServicePickerForm));
            this.SelectProviderLabel = new System.Windows.Forms.Label();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.DropBoxButton = new System.Windows.Forms.Button();
            this.GoogleDriveButton = new System.Windows.Forms.Button();
            this.OneDriveButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // SelectProviderLabel
            // 
            this.SelectProviderLabel.AutoSize = true;
            this.SelectProviderLabel.Font = new System.Drawing.Font("Calibri Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectProviderLabel.Location = new System.Drawing.Point(78, 13);
            this.SelectProviderLabel.Name = "SelectProviderLabel";
            this.SelectProviderLabel.Size = new System.Drawing.Size(135, 17);
            this.SelectProviderLabel.TabIndex = 0;
            this.SelectProviderLabel.Text = "Select Service Provider";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Google-Drive-icon.png");
            this.imageList.Images.SetKeyName(1, "gdstroke.png");
            this.imageList.Images.SetKeyName(2, "Dropbox.png");
            this.imageList.Images.SetKeyName(3, "Dropboxbw.png");
            this.imageList.Images.SetKeyName(4, "42736.png");
            this.imageList.Images.SetKeyName(5, "onedrivebw.png");
            this.imageList.Images.SetKeyName(6, "gdstroke1.png");
            // 
            // DropBoxButton
            // 
            this.DropBoxButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.DropBoxButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.DropBoxButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.DropBoxButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DropBoxButton.ImageIndex = 3;
            this.DropBoxButton.ImageList = this.imageList;
            this.DropBoxButton.Location = new System.Drawing.Point(103, 228);
            this.DropBoxButton.Name = "DropBoxButton";
            this.DropBoxButton.Size = new System.Drawing.Size(75, 67);
            this.DropBoxButton.TabIndex = 1;
            this.DropBoxButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolTip.SetToolTip(this.DropBoxButton, "DropBox");
            this.DropBoxButton.UseVisualStyleBackColor = true;
            this.DropBoxButton.Click += new System.EventHandler(this.DropBoxButton_Click);
            // 
            // GoogleDriveButton
            // 
            this.GoogleDriveButton.BackColor = System.Drawing.Color.Transparent;
            this.GoogleDriveButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.GoogleDriveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.GoogleDriveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.GoogleDriveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GoogleDriveButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.GoogleDriveButton.ImageIndex = 0;
            this.GoogleDriveButton.ImageList = this.imageList;
            this.GoogleDriveButton.Location = new System.Drawing.Point(106, 52);
            this.GoogleDriveButton.Name = "GoogleDriveButton";
            this.GoogleDriveButton.Size = new System.Drawing.Size(72, 60);
            this.GoogleDriveButton.TabIndex = 2;
            this.toolTip.SetToolTip(this.GoogleDriveButton, "Google Drive");
            this.GoogleDriveButton.UseVisualStyleBackColor = false;
            this.GoogleDriveButton.Click += new System.EventHandler(this.GoogleDriveButton_Click);
            // 
            // OneDriveButton
            // 
            this.OneDriveButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.OneDriveButton.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.OneDriveButton.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.OneDriveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OneDriveButton.ImageIndex = 5;
            this.OneDriveButton.ImageList = this.imageList;
            this.OneDriveButton.Location = new System.Drawing.Point(103, 136);
            this.OneDriveButton.Name = "OneDriveButton";
            this.OneDriveButton.Size = new System.Drawing.Size(75, 69);
            this.OneDriveButton.TabIndex = 3;
            this.toolTip.SetToolTip(this.OneDriveButton, "OneDrive");
            this.OneDriveButton.UseVisualStyleBackColor = true;
            this.OneDriveButton.Click += new System.EventHandler(this.OneDriveButton_Click);
            // 
            // ServicePickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 321);
            this.Controls.Add(this.OneDriveButton);
            this.Controls.Add(this.GoogleDriveButton);
            this.Controls.Add(this.DropBoxButton);
            this.Controls.Add(this.SelectProviderLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ServicePickerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "bakkup";
            this.Load += new System.EventHandler(this.ServicePickerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SelectProviderLabel;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button DropBoxButton;
        private System.Windows.Forms.Button GoogleDriveButton;
        private System.Windows.Forms.Button OneDriveButton;
        private System.Windows.Forms.ToolTip toolTip;
    }
}