namespace bakkup
{
    partial class TestForm
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
            this.buttonStartLogin = new System.Windows.Forms.Button();
            this.progressBarProgress = new System.Windows.Forms.ProgressBar();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.buttonTestFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStartLogin
            // 
            this.buttonStartLogin.Location = new System.Drawing.Point(83, 96);
            this.buttonStartLogin.Name = "buttonStartLogin";
            this.buttonStartLogin.Size = new System.Drawing.Size(75, 23);
            this.buttonStartLogin.TabIndex = 0;
            this.buttonStartLogin.Text = "Login";
            this.buttonStartLogin.UseVisualStyleBackColor = true;
            this.buttonStartLogin.Click += new System.EventHandler(this.buttonStartLogin_Click);
            // 
            // progressBarProgress
            // 
            this.progressBarProgress.Location = new System.Drawing.Point(12, 226);
            this.progressBarProgress.Name = "progressBarProgress";
            this.progressBarProgress.Size = new System.Drawing.Size(260, 23);
            this.progressBarProgress.Step = 1;
            this.progressBarProgress.TabIndex = 1;
            // 
            // buttonUpload
            // 
            this.buttonUpload.Enabled = false;
            this.buttonUpload.Location = new System.Drawing.Point(71, 154);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(101, 23);
            this.buttonUpload.TabIndex = 2;
            this.buttonUpload.Text = "Upload Test File";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // buttonTestFolder
            // 
            this.buttonTestFolder.Enabled = false;
            this.buttonTestFolder.Location = new System.Drawing.Point(61, 125);
            this.buttonTestFolder.Name = "buttonTestFolder";
            this.buttonTestFolder.Size = new System.Drawing.Size(120, 23);
            this.buttonTestFolder.TabIndex = 3;
            this.buttonTestFolder.Text = "Create Test Folder";
            this.buttonTestFolder.UseVisualStyleBackColor = true;
            this.buttonTestFolder.Click += new System.EventHandler(this.buttonTestFolder_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonTestFolder);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.progressBarProgress);
            this.Controls.Add(this.buttonStartLogin);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartLogin;
        private System.Windows.Forms.ProgressBar progressBarProgress;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.Button buttonTestFolder;
    }
}