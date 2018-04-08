using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;

namespace VBot
{
    public partial class frmLintErrors : Form
    {
        private string ErrCat = "";
        private string Max = "";
        private Site Wiki;
        private BackgroundWorker bw;

        public frmLintErrors()
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.WorkerReportsProgress = true;
        }

        public void SetData(string user, string password)
        {
            txtLintUser.Text = user;
            txtLintPassword.Text = password;
            cboLintCat.SelectedIndex = 0;
            cboMaxLintError.SelectedIndex = 0;
        }

        private void Login()
        {
            toolStripStatusLabelProgress.Text = "Login  in going";
            Wiki = new Site(txtLintProject.Text, txtLintUser.Text, txtLintPassword.Text);
            if (Wiki.User == "")
            {
                toolStripStatusLabelUser.Text = "User: No connected";
                toolStripStatusLabelSite.Text = "Site: No connected";
                return;
            }
            else
            {
                toolStripStatusLabelUser.Text = "User: " + txtLintUser.Text;
                toolStripStatusLabelSite.Text = "Site: " + txtLintProject.Text;
            }
            toolStripStatusLabelProgress.Text = "";
        }


        private void btnGenerateList_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";
            
            toolStripStatusLabelProgress.Text = "Login on going...";
            Cursor.Current = Cursors.WaitCursor;
            Refresh();
            Login();
            Cursor.Current = Cursors.Default;
            toolStripStatusLabelProgress.Text = "";
            ErrCat = cboLintCat.Text;
            Max = cboMaxLintError.Text;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar1.MarqueeAnimationSpeed = 30;
            bw.RunWorkerAsync();
        }


        /// <summary>Close the form</summary>
        private void btnExit_Click(object sender, EventArgs e) => Close();

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {           
            BackgroundWorker worker = (BackgroundWorker)sender;
            toolStripStatusLabelProgress.Text = "API request on going...";
            
            // Read the errors from Wiki project
            string res = "";
            if (chkLintNS0.Checked)
            {
                res = Wiki.LintError(ErrCat, "0",Max);
            }
            else
            {
                res = Wiki.LintError(ErrCat, Max);
            }

            List<LintError> errList = new List<LintError>(); //Used for store the errors, see the class  for detail
            Dictionary<string, string> pageDic = new Dictionary<string, string>(); //Used for store the page

            toolStripStatusLabelProgress.Text = "Reading page...";
            string results = "";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(res);
            foreach (XmlNode node in doc.SelectNodes("/api/query/linterrors/_v[@title]"))
            {
                string title = node.Attributes["title"].Value;

                string range = node.SelectSingleNode("location").InnerXml;
                range = range.Replace("</_v><_v>", "\t").Replace("<_v>", "").Replace("</_v>", "");
                int From = Convert.ToInt32(range.Split('\t')[0]);
                int To = Convert.ToInt32(range.Split('\t')[1]);

                string parameter = node.SelectSingleNode("params").Attributes["name"].Value;

                string template = "";
                if (node.SelectSingleNode("templateInfo").Attributes["name"] != null)
                {
                    template = node.SelectSingleNode("templateInfo").Attributes["name"].Value;
                }
                LintError err = new LintError(title, parameter, template, From, To);
                errList.Add(err);

                if (!(pageDic.ContainsKey(title))) { pageDic.Add(title, ""); } 
            }

            int MaxPage = 50;
            if (chkLintBOT.Checked) {  MaxPage = 500; }
            string pagesPiped = "";
            foreach (var pair in pageDic)
            {
                pagesPiped += pair.Key + "|";
            }

            if (pagesPiped!="")
            {
                pagesPiped = pagesPiped.Remove(pagesPiped.LastIndexOf("|"));
                List<string> list = Utility.SplitInChunk(pagesPiped, "|", MaxPage);
                foreach (string s in list)
                {
                    Pages pages = new Pages();
                    string strJson = Wiki.LoadWP(s);
                    pages = JsonConvert.DeserializeObject<Pages>(strJson);
                    foreach (Page p in pages.query.pages.Values)
                    {
                        pageDic[p.title]=p.revisions[0].text;
                    }
                }

                toolStripStatusLabelProgress.Text = "Reading page...";
                results = "";
                foreach (LintError err in errList)
                {
                    results += "* [[" + err.title + "]]" + Environment.NewLine;
                    results += "** Parametro: " + err.parameter + Environment.NewLine;
                    results += "** Template: " + err.template + Environment.NewLine;
                    results += "** Da carattere: " + err.from + Environment.NewLine; //Maybe is possible skip this line
                    results += "** A carattere: " + err.to + Environment.NewLine; //Maybe is possible skip this line
                    results += "** <nowiki>" + pageDic[err.title].Substring(err.from, err.to - err.from) + "</nowiki>" + Environment.NewLine;
                }
            }
            else
            {
                results = "No errors";
            }
            e.Result = results;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //toolStripStatusLabelProgress.Text = "Item to create: " + e.UserState as String;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtResult.Text = e.Result.ToString();
            toolStripStatusLabelProgress.Text = "";
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
        }

        private class LintError
        {
            public string title { get; set; }
            public string parameter { get; set; }
            public string template { get; set; }
            public int from { get; set; }
            public int to { get; set; }

            public LintError(string _title, string _parameter, string _template, int _from, int _to)
            {
                title = _title;
                parameter = _parameter;
                template = _template;
                from = _from;
                to = _to;
            }
        }
    }
}
