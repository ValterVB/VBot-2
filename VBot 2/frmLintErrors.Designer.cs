namespace VBot
{
    partial class frmLintErrors
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
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSite = new System.Windows.Forms.ToolStripStatusLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.cboLintCat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLintUser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLintPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLintProject = new System.Windows.Forms.TextBox();
            this.cboMaxLintError = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnGenerateList = new System.Windows.Forms.Button();
            this.chkLintNS0 = new System.Windows.Forms.CheckBox();
            this.chkLintBOT = new System.Windows.Forms.CheckBox();
            this.toolStripStatusLabelProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(12, 22);
            this.txtResult.MaxLength = 0;
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(300, 441);
            this.txtResult.TabIndex = 1;
            this.txtResult.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "List of lint errors";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(641, 514);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(148, 37);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelProgress,
            this.toolStripProgressBar1,
            this.toolStripStatusLabelUser,
            this.toolStripStatusLabelSite});
            this.statusStrip1.Location = new System.Drawing.Point(0, 561);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(805, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.TabStop = true;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelUser
            // 
            this.toolStripStatusLabelUser.AutoSize = false;
            this.toolStripStatusLabelUser.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelUser.Name = "toolStripStatusLabelUser";
            this.toolStripStatusLabelUser.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabelUser.Size = new System.Drawing.Size(200, 17);
            this.toolStripStatusLabelUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelSite
            // 
            this.toolStripStatusLabelSite.AutoSize = false;
            this.toolStripStatusLabelSite.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelSite.Name = "toolStripStatusLabelSite";
            this.toolStripStatusLabelSite.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabelSite.Size = new System.Drawing.Size(200, 17);
            this.toolStripStatusLabelSite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(335, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Lint categories";
            // 
            // cboLintCat
            // 
            this.cboLintCat.FormattingEnabled = true;
            this.cboLintCat.Items.AddRange(new object[] {
            "bogus-image-options",
            "deletable-table-tag",
            "fostered",
            "html5-misnesting",
            "misc-tidy-replacement-issues",
            "misnested-tag",
            "missing-end-tag",
            "multi-colon-escape",
            "multiline-html-table-in-list",
            "multiple-unclosed-formatting-tags",
            "obsolete-tag",
            "pwrap-bug-workaround",
            "self-closed-tag",
            "stripped-tag",
            "tidy-font-bug",
            "tidy-whitespace-bug",
            "unclosed-quotes-in-heading"});
            this.cboLintCat.Location = new System.Drawing.Point(338, 150);
            this.cboLintCat.Name = "cboLintCat";
            this.cboLintCat.Size = new System.Drawing.Size(267, 21);
            this.cboLintCat.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(335, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "User";
            // 
            // txtLintUser
            // 
            this.txtLintUser.Location = new System.Drawing.Point(338, 42);
            this.txtLintUser.Name = "txtLintUser";
            this.txtLintUser.Size = new System.Drawing.Size(173, 20);
            this.txtLintUser.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(522, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Password";
            // 
            // txtLintPassword
            // 
            this.txtLintPassword.Location = new System.Drawing.Point(525, 42);
            this.txtLintPassword.Name = "txtLintPassword";
            this.txtLintPassword.PasswordChar = '*';
            this.txtLintPassword.Size = new System.Drawing.Size(159, 20);
            this.txtLintPassword.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(335, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Project";
            // 
            // txtLintProject
            // 
            this.txtLintProject.Location = new System.Drawing.Point(338, 95);
            this.txtLintProject.Name = "txtLintProject";
            this.txtLintProject.Size = new System.Drawing.Size(173, 20);
            this.txtLintProject.TabIndex = 26;
            this.txtLintProject.Text = "https://it.wikipedia.org";
            // 
            // cboMaxLintError
            // 
            this.cboMaxLintError.FormattingEnabled = true;
            this.cboMaxLintError.Items.AddRange(new object[] {
            "50",
            "100",
            "500",
            "5000"});
            this.cboMaxLintError.Location = new System.Drawing.Point(611, 150);
            this.cboMaxLintError.Name = "cboMaxLintError";
            this.cboMaxLintError.Size = new System.Drawing.Size(73, 21);
            this.cboMaxLintError.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(608, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Max error";
            // 
            // btnGenerateList
            // 
            this.btnGenerateList.Location = new System.Drawing.Point(12, 514);
            this.btnGenerateList.Name = "btnGenerateList";
            this.btnGenerateList.Size = new System.Drawing.Size(148, 37);
            this.btnGenerateList.TabIndex = 30;
            this.btnGenerateList.Text = "Generate List";
            this.btnGenerateList.UseVisualStyleBackColor = true;
            this.btnGenerateList.Click += new System.EventHandler(this.btnGenerateList_Click);
            // 
            // chkLintNS0
            // 
            this.chkLintNS0.AutoSize = true;
            this.chkLintNS0.Location = new System.Drawing.Point(690, 154);
            this.chkLintNS0.Name = "chkLintNS0";
            this.chkLintNS0.Size = new System.Drawing.Size(67, 17);
            this.chkLintNS0.TabIndex = 31;
            this.chkLintNS0.Text = "Only ns0";
            this.chkLintNS0.UseVisualStyleBackColor = true;
            // 
            // chkLintBOT
            // 
            this.chkLintBOT.AutoSize = true;
            this.chkLintBOT.Location = new System.Drawing.Point(690, 45);
            this.chkLintBOT.Name = "chkLintBOT";
            this.chkLintBOT.Size = new System.Drawing.Size(68, 17);
            this.chkLintBOT.TabIndex = 32;
            this.chkLintBOT.Text = "Is a BOT";
            this.chkLintBOT.UseVisualStyleBackColor = true;
            // 
            // toolStripStatusLabelProgress
            // 
            this.toolStripStatusLabelProgress.AutoSize = false;
            this.toolStripStatusLabelProgress.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelProgress.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelProgress.Name = "toolStripStatusLabelProgress";
            this.toolStripStatusLabelProgress.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripStatusLabelProgress.Size = new System.Drawing.Size(157, 17);
            this.toolStripStatusLabelProgress.Spring = true;
            this.toolStripStatusLabelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            // 
            // frmLintErrors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 583);
            this.Controls.Add(this.chkLintBOT);
            this.Controls.Add(this.chkLintNS0);
            this.Controls.Add(this.btnGenerateList);
            this.Controls.Add(this.cboMaxLintError);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtLintProject);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLintPassword);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLintUser);
            this.Controls.Add(this.cboLintCat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtResult);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLintErrors";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "List of Lint errors";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelUser;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSite;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboLintCat;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLintUser;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLintPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLintProject;
        private System.Windows.Forms.ComboBox cboMaxLintError;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnGenerateList;
        private System.Windows.Forms.CheckBox chkLintNS0;
        private System.Windows.Forms.CheckBox chkLintBOT;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelProgress;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
    }
}