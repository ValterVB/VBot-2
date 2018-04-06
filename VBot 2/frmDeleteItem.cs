using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace VBot
{
    public partial class frmDeleteItem : Form
    {
        string reason = "";
        string adminUser = "";
        string adminPassword = "";
        string Wikibase = "";
        Site WD;
        private BackgroundWorker bw;

        public frmDeleteItem()
        {
            InitializeComponent();
            cboDelReason.SelectedIndex = 0;
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.WorkerReportsProgress = true;
        }

        public void SetData(string site, string user, string password)
        {
            Cursor.Current = Cursors.WaitCursor;
            adminUser = user;
            adminPassword = password;
            Wikibase = site;
            Refresh();
            Login();
            Cursor.Current = Cursors.Default;
        }

        private void Login()
        {
            WD = new Site(Wikibase, adminUser, adminPassword);
            if (WD.User == "")
            {
                toolStripStatusLabelUser.Text = "User: No connected";
                toolStripStatusLabelSite.Text = "Site: No connected";
                return;
            }
            else
            {
                toolStripStatusLabelUser.Text = "User: " + adminUser;
                toolStripStatusLabelSite.Text = "Site: " + Wikibase;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            reason = cboDelReason.SelectedItem.ToString();
            bw.RunWorkerAsync();
        }

        /// <summary>Close the form</summary>
        private void btnExit_Click(object sender, EventArgs e) => Close();

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string results = "";
            string resp = "";

            string[] lines = txtList.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Count() == 0)
            {
                toolStripStatusLabelProgress.Text = "No item to elaborate";
                return;
            }
            
            BackgroundWorker worker = (BackgroundWorker)sender;
            toolStripStatusLabelProgress.Text = "Item to delete: " + lines.Count();

            for (int idx = 0; idx < lines.Count(); idx++)
            {
                resp = WD.DeletePage(lines[idx], reason);
                if (resp.StartsWith("{\"delete\":{\"title\""))
                {
                    results += lines[idx] + '\t' + "OK" + Environment.NewLine;
                }
                else if (resp.StartsWith("{\"error\":{\"code\":\"maxlag\""))
                {
                    results += lines[idx] + '\t' + "Maxlag" + Environment.NewLine;
                }
                else if (resp.StartsWith("{\"error\":{\"code\":\"missingtitle\""))
                {
                    results += lines[idx] + '\t' + "doesn't exist" + Environment.NewLine;
                }
                else
                {
                    results += lines[idx] + '\t' + resp + Environment.NewLine;
                }
                worker.ReportProgress(0, (lines.Count()-idx-1).ToString());
            }
            e.Result = results;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabelProgress.Text = "Item to delete: " + e.UserState as String;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtResult.Text = e.Result.ToString();
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
        }
    }
}
