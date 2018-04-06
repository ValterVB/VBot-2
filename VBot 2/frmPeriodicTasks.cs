using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace VBot
{
    public partial class frmPeriodicTasks : Form
    {
        private string user = "";
        private string password = "";

        private BackgroundWorker bw;

        public frmPeriodicTasks()
        {
            InitializeComponent();
            cboDelReason.SelectedIndex = 0;
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.WorkerReportsProgress = true;
        }

        public void SetData(string User, string Password)
        {
            user = User;
            password = Password;
        }


        private void btnFixNoDisambiguation_Click(object sender, EventArgs e)
        {
            string res = ListGenerator.PetScan("?language=it&project=wikipedia&ns%5B0%5D=1&templates_yes=NoDisambiguante&interface_language=en&wikidata_item=any");
            List<string> list = Utility.SplitInChunk(res, "|", 500);
            string result = "";
            Site WD = new Site("https://www.wikidata.org", user, password);
            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s, VBot.Site.LoadTypeWD.LabelSitelink);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        string label = "";
                        string sitelink = "";
                        if (entity.labels.ContainsKey("it")) { label = entity.labels["it"].value; }
                        if (entity.sitelinks.ContainsKey("itwiki")) { sitelink = entity.sitelinks["itwiki"].title; }

                        if (label!=sitelink && sitelink!="" )
                        {
                            if (label.ToLower()==sitelink.ToLower())
                            {
                                txtList.AppendText(entity.id + '\t' + "it" + '\t' + sitelink + '\t' + label + Environment.NewLine);
                            }
                            else
                            {
                                result += entity.id + '\t' + "it" + '\t' + sitelink + '\t' + label + Environment.NewLine;
                            }
                            
                        }
                    }
                }
            }
            txtResult.Text = result;
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            bw.RunWorkerAsync();
        }

        /// <summary>Close the form</summary>
        private void btnExit_Click(object sender, EventArgs e) => Close();

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = "";
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
