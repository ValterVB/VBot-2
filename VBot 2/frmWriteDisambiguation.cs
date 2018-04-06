using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VBot
{
    public partial class frmWriteDisambiguation : Form
    {
        private string user = "";
        private string password = "";
        private string LogFile = "";
        private Site WD;
        Datavalue dvDis = Utility.CreateDataValue("Q4167410", Utility.TypeData.Item); //Disambiguation
        private BackgroundWorker bw;

        public frmWriteDisambiguation()
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.WorkerReportsProgress = true;
        }

        public void SetData(string User, string Password, string file)
        {
            Cursor.Current = Cursors.WaitCursor;
            user = User;
            password=Password;
            LogFile = file;
            Refresh();
            WD = new Site("https://www.wikidata.org", user, password);
            Cursor.Current = Cursors.Default;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (chkNoLogFile.Checked)
            {
                txtResult.Text = "";
                string[] lines = txtList.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                for (int idx = 0; idx < lines.Count(); idx++)
                {
                    string item = txtList.Lines[idx];
                    string strJson = WD.LoadWD(item);
                    Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                    Entity entity = EntityList.entities[item];
                    string ret = WriteDisambiguation(entity);
                    if (ret != "") { txtResult.AppendText(entity.id + '\t' + ret + Environment.NewLine); }
                }
            }
            else
            {
                bw.RunWorkerAsync();
            }
        }

        /// <summary>Close the form</summary>
        private void btnExit_Click(object sender, EventArgs e) => Close();

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (txtList.Text == "")
            {
                toolStripStatusLabel.Text = "No item to elaborate";
                return;
            }

            BackgroundWorker worker = (BackgroundWorker)sender;

            string error = "";
            int cont = txtList.Lines.Count();
            int idx = 0;
            toolStripStatusLabel.Text = "Item to elaborate: " + cont;

            List<string> list = Utility.SplitInChunk(txtList.Text, Environment.NewLine, 500);
            
            Random rnd = new Random();
            System.IO.StreamWriter log = new System.IO.StreamWriter(LogFile + "LogDisambigue" + rnd.Next(1, 10000000).ToString() + ".txt", false, Encoding.UTF8);

            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        string ret = WriteDisambiguation(entity);
                        if (ret != "") { log.WriteLine(ret); }
                        worker.ReportProgress(0, (cont - idx - 1).ToString());
                        idx++;
                    }
                }
            }
            log.Close();
            e.Result = error;
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel.Text = "Item to elaborate: " + e.UserState as String;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtResult.Text = e.Result.ToString();
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
        }

        private string WriteDisambiguation(Entity entity)
        {
            if (entity.sitelinks != null && entity.PropertyValueExist("P31", dvDis) && entity.claims["P31"].Count() == 1 && entity.sitelinks.Count == 1)
            {
                Dictionary<string, string> Labels = new Dictionary<string, string>();
                if (entity.sitelinks.First().Value.title.All(c => Utility.shortAlphabet.Contains(c)))
                {
                    string label = Utility.DelDisambiguation(entity.sitelinks.First().Value.title, "()");
                    foreach (string lang in Utility.lstLatin)
                    {
                        if (entity.labels == null || !entity.labels.ContainsKey(lang))
                        {
                            Labels.Add(lang, label);
                        }
                        else if (entity.labels[lang].value != label)
                        {
                            Labels.Add(lang, label);
                        }

                    }
                }
                Dictionary<string, string> Descriptions = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> DisDesc in Utility.dicDis)
                {
                    if (entity.descriptions == null || ((DisDesc.Key == "it" || DisDesc.Key == "ilo" || DisDesc.Key == "nb" || DisDesc.Key == "ca" || DisDesc.Key == "tr" || DisDesc.Key == "es") && entity.descriptions.ContainsKey(DisDesc.Key) && entity.descriptions[DisDesc.Key].value != DisDesc.Value))
                    {
                        Descriptions.Add(DisDesc.Key, DisDesc.Value);
                    }
                    else if (entity.descriptions == null || !entity.descriptions.ContainsKey(DisDesc.Key))
                    {
                        Descriptions.Add(DisDesc.Key, DisDesc.Value);
                    }
                }
                if (Labels.Count() > 0 || Descriptions.Count() > 0)
                {
                    try
                    {
                        string ret = WD.EditEntity(entity.id, null, Labels, Descriptions, null, null, "BOT:Add label (only latin alphabet) and descriptions on disambiguation item");
                        if (ret != "")
                        {
                            if (ret.IndexOf("code=\"maxlag\"") != -1)
                            {
                                Thread.Sleep(5000);
                                return entity.id + '\t' + "maxlag" + Environment.NewLine;
                            }
                            else if (ret.IndexOf("Server non disponibile") != -1)
                            {
                                return entity.id + '\t' + "(503) Server non disponibile" + Environment.NewLine;
                            }
                            else if (ret.IndexOf("code=\"readonly\"") != -1)
                            {
                                return entity.id + '\t' + "read only" + Environment.NewLine;
                            }
                            else if (ret.IndexOf("error code=\"badtoken\"") != -1)
                            {
                                WD.GetToken();
                                return entity.id + '\t' + "bad token" + Environment.NewLine;
                            }
                            else
                            {
                                return "Disamb only\t" + entity.id + '\t' + Utility.CleanApiError(ret);
                            }
                        }
                        else
                        {
                            return "";
                        }
                    }
                    catch (Exception ex)
                    {
                        return entity.id + '\t' + ex.Message + Environment.NewLine;
                    }
                }
            }
            return "Null";
        }
    }
}
