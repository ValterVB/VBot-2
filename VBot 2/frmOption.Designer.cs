namespace VBot
{
    partial class frmOption
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtAdminPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAdminUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBotPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBotUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboWikiProject = new System.Windows.Forms.ComboBox();
            this.cboWikiLang = new System.Windows.Forms.ComboBox();
            this.txtDumpPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDumpFile = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtReportsPath = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(256, 289);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 36);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtAdminPassword
            // 
            this.txtAdminPassword.Location = new System.Drawing.Point(182, 77);
            this.txtAdminPassword.Name = "txtAdminPassword";
            this.txtAdminPassword.Size = new System.Drawing.Size(155, 20);
            this.txtAdminPassword.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(182, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Admin password";
            // 
            // txtAdminUser
            // 
            this.txtAdminUser.Location = new System.Drawing.Point(12, 77);
            this.txtAdminUser.Name = "txtAdminUser";
            this.txtAdminUser.Size = new System.Drawing.Size(155, 20);
            this.txtAdminUser.TabIndex = 23;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Admin user";
            // 
            // txtBotPassword
            // 
            this.txtBotPassword.Location = new System.Drawing.Point(182, 25);
            this.txtBotPassword.Name = "txtBotPassword";
            this.txtBotPassword.Size = new System.Drawing.Size(155, 20);
            this.txtBotPassword.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Bot password";
            // 
            // txtBotUser
            // 
            this.txtBotUser.Location = new System.Drawing.Point(12, 25);
            this.txtBotUser.Name = "txtBotUser";
            this.txtBotUser.Size = new System.Drawing.Size(155, 20);
            this.txtBotUser.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Bot user";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(182, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Language";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Favorite Wikiproject";
            // 
            // cboWikiProject
            // 
            this.cboWikiProject.FormattingEnabled = true;
            this.cboWikiProject.Items.AddRange(new object[] {
            "Wikipedia",
            "Wiktionary",
            "Wikibooks",
            "Wikinews",
            "Wikiquote",
            "Wikisource",
            "Wikiversity",
            "Wikivoyage"});
            this.cboWikiProject.Location = new System.Drawing.Point(15, 132);
            this.cboWikiProject.Name = "cboWikiProject";
            this.cboWikiProject.Size = new System.Drawing.Size(155, 21);
            this.cboWikiProject.TabIndex = 30;
            // 
            // cboWikiLang
            // 
            this.cboWikiLang.FormattingEnabled = true;
            this.cboWikiLang.Location = new System.Drawing.Point(182, 132);
            this.cboWikiLang.Name = "cboWikiLang";
            this.cboWikiLang.Size = new System.Drawing.Size(155, 21);
            this.cboWikiLang.TabIndex = 31;
            // 
            // txtDumpPath
            // 
            this.txtDumpPath.Location = new System.Drawing.Point(12, 189);
            this.txtDumpPath.Name = "txtDumpPath";
            this.txtDumpPath.Size = new System.Drawing.Size(155, 20);
            this.txtDumpPath.TabIndex = 33;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Dump path";
            // 
            // txtDumpFile
            // 
            this.txtDumpFile.Location = new System.Drawing.Point(182, 189);
            this.txtDumpFile.Name = "txtDumpFile";
            this.txtDumpFile.Size = new System.Drawing.Size(155, 20);
            this.txtDumpFile.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(182, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Dump file";
            // 
            // txtReportsPath
            // 
            this.txtReportsPath.Location = new System.Drawing.Point(12, 243);
            this.txtReportsPath.Name = "txtReportsPath";
            this.txtReportsPath.Size = new System.Drawing.Size(155, 20);
            this.txtReportsPath.TabIndex = 37;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 227);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "Reports path";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Location = new System.Drawing.Point(12, 289);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(81, 36);
            this.btnSave.TabIndex = 38;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmOption
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 341);
            this.ControlBox = false;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtReportsPath);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtDumpFile);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtDumpPath);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboWikiLang);
            this.Controls.Add(this.cboWikiProject);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtAdminPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAdminUser);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtBotPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBotUser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmOption";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Option";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtAdminPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAdminUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBotPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBotUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboWikiProject;
        private System.Windows.Forms.ComboBox cboWikiLang;
        private System.Windows.Forms.TextBox txtDumpPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDumpFile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtReportsPath;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSave;
    }
}