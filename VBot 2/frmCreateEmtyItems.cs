using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VBot
{
    public partial class frmCreateEmtyItems : Form
    {
        string adminUser = "";
        string adminPassword = "";
        string Wikibase = "";
        int cont = 0;
        Site WD;
        private BackgroundWorker bw;

        public frmCreateEmtyItems()
        {
            InitializeComponent();
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

        private void btnCreateItem_Click(object sender, EventArgs e)
        {
            Int32.TryParse(txtNumberOfItems.Text, out cont);
            txtResult.Text = "";
            bw.RunWorkerAsync();
        }

        /// <summary>Close the form</summary>
        private void btnExit_Click(object sender, EventArgs e) => Close();

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string results = "";
            
            BackgroundWorker worker = (BackgroundWorker)sender;
            toolStripStatusLabelProgress.Text = "Item to create: " + cont;
            Regex regex = new Regex("\"id\":\"(Q\\d+)\",", RegexOptions.Compiled);
            if (cont != 0)
            {
                for (int i = 0; i < cont; i++)
                {
                    string res = WD.CreateItem();
                    Match match = regex.Match(res);
                    if (match.Success) { results += match.Groups[1].Value + Environment.NewLine; }
                    worker.ReportProgress(0, (cont - i - 1).ToString());
                }
            }
            e.Result = results;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabelProgress.Text = "Item to create: " + e.UserState as String;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtResult.Text = e.Result.ToString();
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
            txtNumberOfItems.Text = "0";
        }
    }
}
