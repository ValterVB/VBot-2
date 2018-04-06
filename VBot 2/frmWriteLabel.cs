using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VBot
{
    public partial class frmWriteLabel : Form
    {
        private string user = "";
        private string password = "";
        private string LogFile = "";
        private Site WD;
        private Datavalue dvCat = Utility.CreateDataValue("Q4167836", Utility.TypeData.Item);  //Category
        private Datavalue dvDis = Utility.CreateDataValue("Q4167410", Utility.TypeData.Item);  //Disambiguation
        private Datavalue dvTem = Utility.CreateDataValue("Q11266439", Utility.TypeData.Item); //Template
        private BackgroundWorker bw;

        public frmWriteLabel()
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

        private void btnWrite_Click(object sender, EventArgs e)
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
                    string ret = WriteLabel(entity);
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
            System.IO.StreamWriter log = new System.IO.StreamWriter(LogFile + "LogLabel" + rnd.Next(1, 10000000).ToString() + ".txt", false, Encoding.UTF8);

            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        string ret = WriteLabel(entity);
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

        private string WriteLabel(Entity entity)
        {    
            Dictionary<string, string> Labels = new Dictionary<string, string>();
            if (entity.sitelinks != null)
            {
                Boolean InList = false;
                foreach (Datavalue dv in Utility.dicP31ForLabel)
                {
                    if (entity.PropertyValueExist("P31", dv))
                    {
                        InList = true;
                        break;
                    }
                }
                foreach (SiteLink sl in entity.sitelinks.Values)
                {
                    string link = sl.site;
                    if (link.EndsWith("wiki") && link != "commonswiki" && link != "wikidatawiki" && link != "specieswiki" && link != "metawiki" && link != "mediawikiwiki")
                    {
                        string lang = Utility.SitelinkToLanguages(link);
                        if (lang == "") { lang = sl.site.Replace("wiki", "").Replace("_", "-"); }

                        if (entity.PropertyValueExist("P31", dvCat))
                        {
                            if (entity.labels == null || !entity.labels.ContainsKey(lang))
                            {
                                if (!Labels.ContainsKey(lang)) { Labels.Add(lang, sl.title); }
                            }
                        }
                        else if (InList || Utility.goodChars.Contains(sl.title[0]))
                        {
                            if (sl.title.IndexOf(",") == -1)
                            {
                                if (entity.labels == null || !entity.labels.ContainsKey(lang)) // Check if is empty
                                {
                                    if (!Labels.ContainsKey(lang))
                                    {
                                        if (sl.title.EndsWith(")"))
                                        {
                                            Labels.Add(lang, Utility.DelDisambiguation(sl.title, "()"));
                                        }
                                        else
                                        {
                                            Labels.Add(lang, sl.title);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (Labels.Count() > 0)
                {
                    try
                    {
                        string ret = WD.EditEntity(entity.id, null, Labels, null, null, null, "BOT:Add labels (Upper case)");
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
                                if (entity.PropertyValueExist("P31", dvCat))
                                {
                                    return "Category\t" + entity.id + '\t' + Utility.CleanApiError(ret);
                                }
                                else if (entity.PropertyValueExist("P31", dvDis))
                                {
                                    return "Disamb\t" + entity.id + '\t' + Utility.CleanApiError(ret);
                                }
                                else if (entity.PropertyValueExist("P31", dvTem))
                                {
                                    return "Template\t" + entity.id + '\t' + Utility.CleanApiError(ret);
                                }
                                else
                                {
                                    return "Other\t" + entity.id + '\t' + Utility.CleanApiError(ret);
                                }
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
            return "";
        }
    }
}
