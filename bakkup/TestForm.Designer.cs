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
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.buttonStartLogin);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartLogin;
    }
}