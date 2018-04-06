using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VBot
{
    public partial class frmFindAndReplace : Form
    {
        public string user;
        public string password;
        public frmFindAndReplace()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] lines = txtListItem1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int cont = lines.Count()-1;
            Site Wikidata = new Site();
            Wikidata = new Site("https://www.wikidata.org", user, password);

            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string strJson = Wikidata.LoadWD(lines[idx]);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {

                }
                //backgroundWorker1.ReportProgress(100*idx/cont);
            }
            textBox3.Text = "Fatto";
        }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;


        }

        private void brnFindAndReplaceStart_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
