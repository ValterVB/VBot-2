using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;


namespace VBot
{
    public partial class frmVBot : Form
    {
        private string user = "";
        private string password = "";
        private string userAdm = "";
        private string passwordAdm = "";
        private string ReportsPath = "";
        private string DumpPath = "";
        private string DumpFile = "";
        private string MainWiki = "";
        private string Language = "";

        public frmVBot()
        {
            InitializeComponent();
            ServicePointManager.DefaultConnectionLimit = 36; //N° of contemporary connection

            Show();
            frmOption f2 = new frmOption();
            f2.ShowDialog(this);
            f2.Dispose();
            Cursor.Current = Cursors.WaitCursor;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(path + "VBotConfig.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path + "VBotConfig.xml");
                user = xmlDoc.SelectSingleNode("/VBot/botUser").Attributes["user"].Value;
                password = xmlDoc.SelectSingleNode("/VBot/botUser").Attributes["password"].Value;
                userAdm = xmlDoc.SelectSingleNode("/VBot/adminUser").Attributes["user"].Value;
                passwordAdm = xmlDoc.SelectSingleNode("/VBot/adminUser").Attributes["password"].Value;
                MainWiki = xmlDoc.SelectSingleNode("/VBot/mainWiki").Attributes["project"].Value;
                Language = xmlDoc.SelectSingleNode("/VBot/mainWiki").Attributes["language"].Value;
                ReportsPath = xmlDoc.SelectSingleNode("/VBot/reportsPath").InnerText;
                DumpPath = xmlDoc.SelectSingleNode("/VBot/dumpPath").InnerText;
                DumpFile = xmlDoc.SelectSingleNode("/VBot/dumpFile").InnerText;

                if (File.Exists(DumpPath + DumpFile))
                {
                    toolStripStatusWikidata.BackColor = System.Drawing.Color.Green;
                    toolStripStatusWikidata.Text = DumpPath + DumpFile;
                }
                Cursor.Current = Cursors.Default;
            }
        }

        //Counters of the rows in text box
        private void txtIn_TextChanged(object sender, EventArgs e)
        {
            lblInput.Text = "Input: " + txtIn.Lines.Count() + " rows";
        }
        private void txtOut_TextChanged(object sender, EventArgs e)
        {
            lblOutput.Text = "Output: " + txtOut.Lines.Count() + " rows";
        }
        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "Message: " + txtMessage.Lines.Count() + " rows";
        }

        private void ClearTextBox()
        {
            txtOut.Text = "";
            txtMessage.Text = "";
        }

        /// <summary>Generate files of items of categories, disambiguations and templates from the dump to add labels and/or descriptions in a second moment</summary>
        private void btnReadForLabelAndDescription_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            txtMessage.Text = LabelAndDescription.ReadDumpForLabelAndDescription(DumpPath + DumpFile, ReportsPath);
        }

        /// <summary>Write missing labels based on sitelink for some type of item</summary>
        private void btnWriteLabelFromSitelink_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WD = new Site("https://www.wikidata.org", user, password);
            if (WD.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            frmWriteLabel Form = new frmWriteLabel();
            Form.Show();
            Form.SetData(user, password, ReportsPath);
        }

        /// <summary>Write missing labels/descriptions for disambiguation item</summary>
        private void btnWriteDisambiguation_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            if (user == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            frmWriteDisambiguation Form = new frmWriteDisambiguation();
            Form.Show();
            Form.SetData(user, password, ReportsPath);
        }

        /// <summary>Write missing descriptions for category item</summary>
        private void btnWriteCategory_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WD = new Site("https://www.wikidata.org", user, password);
            if (WD.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            frmWriteCategory Form = new frmWriteCategory();
            Form.Show();
            Form.SetData(user, password, ReportsPath);
        }

        /// <summary>Write missing descriptions for template item</summary>
        private void btnWriteTemplate_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WD = new Site("https://www.wikidata.org", user, password);
            if (WD.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            frmWriteTemplate Form = new frmWriteTemplate();
            Form.Show();
            Form.SetData(user, password, ReportsPath);
        }

        /// <summary>Check if description in AutoEdit is the same for the bot (disambiguation, category, template and list)</summary>
        private void btnCheckStdDesc_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            txtMessage.Text = Utility.CheckDesc(user, password);
            Cursor = Cursors.Default;
        }

        /// <summary>Add same label in all latin language</summary>
        private void btnAddLatinLabel_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WD = new Site("https://www.wikidata.org", user, password);
            if (WD.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            txtMessage.Text = LabelAndDescription.AddLabelLatin(WD, txtLabItem.Text, txtLabLabel.Text);
        }

        /// <summary>List of pages in the category, only level 0</summary>
        private void btnListFromCat_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WP = new Site("https://it.wikipedia.org", user, password);
            if (WP.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            var res = ListGenerator.ListFromCategory(WP, txtCategoryName.Text);
            if (res.count != 0) { txtOut.Text = res.list.Replace("|", Environment.NewLine); }
        }

        /// <summary>List of pages from the search</summary>
        private void btnListFromSearch_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WP = new Site("https://it.wikipedia.org", user, password);
            if (WP.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            var res = ListGenerator.ListFromSearch(WP, txtStringToSearch.Text);
            if (res.count != 0) { txtOut.Text = res.list.Replace("|", Environment.NewLine); }
        }

        /// <summary>List of items from the SPARQL query</summary>
        private void btnWQS_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";

            var res = ListGenerator.WQS(txtIn.Text, "item");
            if (res.count != 0) { txtOut.Text = res.list.Replace("|", Environment.NewLine); }
        }

        /// <summary>List of items or page from Whats link here</summary>
        private void btnWhatsLinkHere_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";

            Site WP = new Site("https://it.wikipedia.org", user, password);
            if (WP.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            var res = ListGenerator.WhatsLinkHere(WP, txtWhatsLinkHere.Text);
            if (res.count != 0) { txtOut.Text = res.list; }
        }

        /// <summary>Delete a list of item on Wikidata</summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (userAdm == "" || passwordAdm == "")
            {
                toolStripStatusMessage.Text = "Need an administrator user and or password";
                return;
            }

            frmDeleteItem Form = new frmDeleteItem();
            Form.Show(this);
            Form.SetData("https://www.wikidata.org", userAdm, passwordAdm);
        }

        private void btnCreatingItems_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }

            frmCreateEmtyItems Form = new frmCreateEmtyItems();
            Form.Show(this);
            Form.SetData("https://www.wikidata.org", user, password);
        }

        private void btnListLintError_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }

            frmLintErrors Form = new frmLintErrors();
            Form.Show(this);
            Form.SetData(user, password);
        }

        // ==============================================================================================

        /// <summary>Try to fix label conflict, the list musb be: Q1 Q2 lan tab separated</summary>
        private void btnFixLabelConflict_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            LabelAndDescription.FixLabelConflict(txtMessage, txtOut, txtIn.Text, user, password);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            StreamWriter log1 = new StreamWriter(ReportsPath + "Disambiguation1.txt", false, Encoding.UTF8);
            //StreamWriter log2 = new StreamWriter(FilePath + "DisambigMess.txt", false, Encoding.UTF8);
            StreamWriter log3 = new StreamWriter(ReportsPath + "DisambigCandList1.txt", false, Encoding.UTF8);
            StreamWriter log4 = new StreamWriter(ReportsPath + "DisambigToMerge1.txt", false, Encoding.UTF8);
            StreamWriter log5 = new StreamWriter(ReportsPath + "DisambigToMergeSameWiki1.txt", false, Encoding.UTF8);

            string strJson = "";
            Entities EntityList = new Entities();
            Datavalue dvValore = Utility.CreateDataValue("Q4167410", Utility.TypeData.Item); //disambigua

            string result = "{| class='wikitable sortable'" + Environment.NewLine;
            result += "!Item 1" + Environment.NewLine;
            result += "!Item 2" + Environment.NewLine;
            result += "!Distinct labels 1 " + Environment.NewLine;
            result += "!Distinct labels 2" + Environment.NewLine;
            result += "!Distinct sitelink 1" + Environment.NewLine;
            result += "!Distinct sitelink 2" + Environment.NewLine;
            result += "!Note" + Environment.NewLine;
            result += "!Disambiguation 1" + Environment.NewLine;
            result += "!Disambiguation 2" + Environment.NewLine;

            log1.Write(result);
            //log2.Write(result);
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                List<string> lista1 = new List<string>(); //label primo item
                List<string> lista2 = new List<string>(); //label secondo item
                List<string> links1 = new List<string>(); //link primo item
                List<string> links2 = new List<string>(); //link secondo item
                string tmp1 = lines[idx].Split('-')[0]; //Qnumber primo item
                string tmp2 = lines[idx].Split('-')[1]; //Qnumber second item
                string tmp3 = lines[idx].Split('-')[2]; //lingua
                tmp1 = tmp1.Replace("# [[", "").Replace("]]", "");
                tmp2 = tmp2.Split('|')[0].Replace("[[", "");
                try
                {
                    strJson = WD.LoadWD(tmp1 + "|" + tmp2);
                    EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());

                    if (!EntityList.entities[tmp1].IsRedirect && !EntityList.entities[tmp2].IsRedirect && EntityList.entities[tmp1].PropertyValueExist("P31", dvValore) && EntityList.entities[tmp2].PropertyValueExist("P31", dvValore)) //temporaneo, redirect non funziona
                    {
                        foreach (Labels label in EntityList.entities[tmp1].labels.Values)
                        {
                            if (!lista1.Contains(label.value)) { lista1.Add(label.value); }
                        }
                        foreach (Labels label in EntityList.entities[tmp2].labels.Values)
                        {
                            if (!lista2.Contains(label.value)) { lista2.Add(label.value); }
                        }
                        foreach (SiteLink sitelink in EntityList.entities[tmp1].sitelinks.Values)
                        {
                            if (!links1.Contains(sitelink.title)) { links1.Add(sitelink.title); }
                        }
                        foreach (SiteLink sitelink in EntityList.entities[tmp2].sitelinks.Values)
                        {
                            if (!links2.Contains(sitelink.title)) { links2.Add(sitelink.title); }
                        }
                        string lang1 = "";
                        foreach (string tmp in lista1)
                        {
                            lang1 += tmp + "<br>";
                        }
                        if (lang1 == "")
                        {
                            lang1 = "No label";
                        }
                        else
                        {
                            lang1 = lang1.Remove(lang1.LastIndexOf("<br>"));
                        }

                        string lang2 = "";
                        foreach (string tmp in lista2)
                        {
                            lang2 += tmp + "<br>";
                        }
                        if (lang2 == "")
                        {
                            lang2 = "No label";
                        }
                        else
                        {
                            lang2 = lang2.Remove(lang2.LastIndexOf("<br>"));
                        }

                        string link1 = "";
                        foreach (string tmp in links1)
                        {
                            link1 += tmp + "<br>";
                        }
                        if (link1 == "")
                        {
                            link1 = "No link";
                        }
                        else
                        {
                            link1 = link1.Remove(link1.LastIndexOf("<br>"));
                        }

                        string link2 = "";
                        foreach (string tmp in links2)
                        {
                            link2 += tmp + "<br>";
                        }
                        if (link2 == "")
                        {
                            link2 = "No link";
                        }
                        else
                        {
                            link2 = link2.Remove(link2.LastIndexOf("<br>"));
                        }
                        result = "";
                        result += "|-" + Environment.NewLine;
                        result += "| [[" + tmp1 + "]]" + Environment.NewLine;
                        result += "| [[" + tmp2 + "]]" + Environment.NewLine;
                        result += "| " + lang1 + Environment.NewLine;
                        result += "| " + lang2 + Environment.NewLine;
                        result += "| " + link1 + Environment.NewLine;
                        result += "| " + link2 + Environment.NewLine;

                        log3.Write("# [[" + tmp1 + "]]-[[" + tmp2 + "|" + tmp2 + "]]-" + tmp3 + Environment.NewLine);
                        if (lang1 == lang2)
                        {
                            if (EntityList.entities[tmp1].sitelinks.Count() == 1 && EntityList.entities[tmp2].sitelinks.Count() == 1)
                            {
                                if (EntityList.entities[tmp1].sitelinks.First().Value.site == EntityList.entities[tmp2].sitelinks.First().Value.site)
                                {
                                    result += "| Same wiki" + Environment.NewLine;
                                    log5.Write("# [[" + tmp1 + "]]-[[" + tmp2 + "]]-" + tmp3 + Environment.NewLine);
                                }
                                else if (Utility.IsWikiDisambiguation(EntityList.entities[tmp1].sitelinks.First().Value.site, EntityList.entities[tmp1].sitelinks.First().Value.title) == Utility.IsWikiDisambiguation(EntityList.entities[tmp2].sitelinks.First().Value.site, EntityList.entities[tmp2].sitelinks.First().Value.title))
                                {
                                    result += "| To merge" + Environment.NewLine;
                                    log4.Write("# [[" + tmp1 + "]]-[[" + tmp2 + "]]-" + tmp3 + Environment.NewLine);
                                }
                            }
                            else
                            {
                                result += "| Candidate to merge" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            result += "|  " + Environment.NewLine;
                        }
                        bool dis = true;
                        foreach (SiteLink sl in EntityList.entities[tmp1].sitelinks.Values)
                        {
                            if (!Utility.IsWikiDisambiguation(sl.site, sl.title))
                            {
                                dis = false;
                                break;
                            }
                        }
                        if (dis)
                        {
                            result += "| All disambiguation" + Environment.NewLine;
                        }
                        else
                        {
                            result += "| Mixed disambiguation" + Environment.NewLine;
                        }
                        dis = true;
                        foreach (SiteLink sl in EntityList.entities[tmp2].sitelinks.Values)
                        {
                            if (!Utility.IsWikiDisambiguation(sl.site, sl.title))
                            {
                                dis = false;
                                break;
                            }
                        }
                        if (dis)
                        {
                            result += "| All disambiguation" + Environment.NewLine;
                        }
                        else
                        {
                            result += "| Mixed disambiguation" + Environment.NewLine;
                        }
                        log1.Write(result);
                    }
                }
                catch (Exception err)
                {
                    txtMessage.AppendText("Error: " + err);
                    txtMessage.Refresh();
                }
            }
            result = "|}";
            log1.Write(result);
            //log2.Write(result);
            log1.Close();
            //log2.Close();
            log3.Close();
            log4.Close();
            log5.Close();
            MessageBox.Show("FINITO");
        }


        // Sequenza, item1, item2 ling conf.
        private void button41_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            string[] lines = txtIn.Text.Replace("\r", "").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Entities EntityList = new Entities();
            string result = "{| border=\"1\" class=\"sortable\"" + Environment.NewLine;
            result += "!Seq!!Item (1)!!P31 (1)!!Item (2)!!P31 (2)!!Confl. Lang" + Environment.NewLine;
            result += "|-" + Environment.NewLine;
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] riga = lines[idx].Split('\t');
                string strJson = WD.LoadWD(riga[1]);
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity entity1 = EntityList.entities.Values.First();
                strJson = WD.LoadWD(riga[2]);
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity entity2 = EntityList.entities.Values.First();

                string p1 = entity1.PropertyValueString("P31"); ;
                string p2 = entity2.PropertyValueString("P31");
                string lab1 = "";
                string lab2 = "";

                if (p1 == "Q4167410") { p1 = "Wikimedia disambiguation page"; }
                if (p2 == "Q4167410") { p2 = "Wikimedia disambiguation page"; }
                if (p1 == "Q11266439") { p1 = "Wikimedia template"; }
                if (p2 == "Q11266439") { p2 = "Wikimedia template "; }
                if (p1 == "Q101352") { p1 = "family name"; }
                if (p2 == "Q101352") { p2 = "family name"; }
                if (p1 == "Q4167836") { p1 = "Wikimedia category"; }
                if (p2 == "Q4167836") { p2 = "Wikimedia category"; }

                if (entity1.labels.ContainsKey(riga[3]))
                {
                    lab1 = entity1.labels[riga[3]].value;
                }
                else
                {
                    lab1 = "no label";
                }
                if (entity2.labels.ContainsKey(riga[3]))
                {
                    lab2 = entity2.labels[riga[3]].value;
                }
                else
                {
                    lab2 = "no label";
                }
                result += "|" + riga[0] + "||[[" + entity1.id + "|" + lab1 + " (" + entity1.id + ")]]||" + p1 + "||[[" + entity2.id + "|" + lab2 + " (" + entity2.id + ")]]||" + p2 + "||" + riga[3] + Environment.NewLine;
                result += "|-" + Environment.NewLine;
            }
            result += "|}";
            txtOut.Text = result;
        }

        private void CheckForF_Click(object sender, EventArgs e)
        {
            Wikipedia.CheckForF_Dump(txtOut, DumpPath + "itwiki-20170801-pages-meta-current.xml", ReportsPath + "WikipediaCheckForF.txt");
        }



        /// <summary>Generate a table with conflict item </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            txtOut.Text = ReportsWD.CreateTableForConflict(txtIn.Text, txtMessage, user, password);
        }

        private void btnQuickstatementsLabel_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            LabelAndDescription.WriteDescriptionOnly(txtMessage, txtIn.Text, ReportsPath, user, password);
        }

        private void btnCleanErrorLog_Click(object sender, EventArgs e)
        {
            txtOut.Text = Utility.CleanErrorLog(txtIn.Text);
        }

        private void btnLabelFromSitelink_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            LabelAndDescription.WriteLabelOnly(txtMessage, txtIn.Text, ReportsPath, user, password);
        }

        private void btnSitelinkFromItem_Click(object sender, EventArgs e)
        {
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            txtOut.Text = Utility.SitelinkFromItem(txtIn.Text, txtReportSitelink.Text, user, password);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            Wikipedia.WriteF(txtMessage, txtIn.Text, "", "", user, password);
        }

        /// <summary>Null edit</summary>
        private void button9_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            if (user == "" || password == "")
            {
                toolStripStatusMessage.Text = "Need a BOT user and or password";
                return;
            }
            txtOut.Text = Wikipedia.Purge("https://it.wikipedia.org", txtIn.Text, user, password);
            txtMessage.Text = "Completed";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Site WP = new Site("https://it.wikipedia.org", user, password);
            string[] res = ListGenerator.TranscludeWP("Template:NoDisambiguante", WP);

            Dictionary<string, int> exclude = new Dictionary<string, int>();
            string[] tmplist = res[1].Split('|');
            foreach (string l in tmplist)
            {
                exclude.Add(l, 1);
            }

            string line = "";
            StreamReader file = new StreamReader(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\Disambiguanti.txt", Encoding.UTF8);
            Dictionary<string, int> list = new Dictionary<string, int>();
            while ((line = file.ReadLine()) != null)
            {
                string[] tmp = line.Split('\t');
                if (tmp[2] != "1") //NO redirect
                {
                    if (!exclude.ContainsKey(tmp[0])) //Non deve evere il template NoDisambiguante
                    {
                        if (tmp[1] != "*") //Titoli che iniziano con (
                        {
                            if (Char.IsUpper(tmp[1][1])) //Upper case
                            {
                                if (list.ContainsKey(tmp[1]))
                                {
                                    list[tmp[1]]++;
                                }
                                else
                                {
                                    list.Add(tmp[1], 1);
                                }
                            }
                        }
                    }
                }
            }
            file.Close();
            var sortList = list.ToList();
            sortList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            StreamWriter UP1 = new StreamWriter(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\DisambiguantiUCMulti.txt", false, Encoding.UTF8);
            StreamWriter UP2 = new StreamWriter(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\DisambiguantiUCMono.txt", false, Encoding.UTF8);
            UP1.WriteLine("{| class=\"wikitable sortable\""); UP2.WriteLine("{| class=\"wikitable sortable\"");
            UP1.WriteLine("! Diambiguante !! N° voci"); UP2.WriteLine("! Diambiguante !! Link");
            UP1.WriteLine("|-"); UP2.WriteLine("|-");
            foreach (var pair in sortList)
            {
                if (pair.Value > 1)
                {
                    UP1.WriteLine("| " + pair.Key + " || " + pair.Value);
                    UP1.WriteLine("|-");
                }
                else
                {
                    UP2.WriteLine("| " + pair.Key + " || " + CercaVoce(pair.Key));
                    UP2.WriteLine("|-");
                }
            }
            UP1.WriteLine("|}"); UP2.WriteLine("|}");
            UP1.Close(); UP2.Close();
            // --------------------
            file = new StreamReader(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\Disambiguanti.txt", Encoding.UTF8);
            list = new Dictionary<string, int>();
            while ((line = file.ReadLine()) != null)
            {
                string[] tmp = line.Split('\t');
                if (tmp[2] != "1") //NO redirect
                {
                    if (!exclude.ContainsKey(tmp[0])) //Non deve evere il template NoDisambiguante
                    {
                        if (tmp[1] != "*") //Titoli che iniziano con (
                        {
                            if (Char.IsLower(tmp[1][1])) //Upper case
                            {
                                if (list.ContainsKey(tmp[1]))
                                {
                                    list[tmp[1]]++;
                                }
                                else
                                {
                                    list.Add(tmp[1], 1);
                                }
                            }
                        }
                    }
                }
            }
            file.Close();
            sortList = list.ToList();
            sortList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            StreamWriter UP3 = new StreamWriter(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\DisambiguantiLCMulti.txt", false, Encoding.UTF8);
            StreamWriter UP4 = new StreamWriter(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\DisambiguantiLCMono.txt", false, Encoding.UTF8);
            UP3.WriteLine("{| class=\"wikitable sortable\""); UP4.WriteLine("{| class=\"wikitable sortable\"");
            UP3.WriteLine("! Diambiguante !! N° voci"); UP4.WriteLine("! Diambiguante !! Link");
            UP3.WriteLine("|-"); UP4.WriteLine("|-");
            foreach (var pair in sortList)
            {
                if (pair.Value > 1)
                {
                    UP3.WriteLine("| " + pair.Key + " || " + pair.Value);
                    UP3.WriteLine("|-");
                }
                else
                {
                    UP4.WriteLine("| " + pair.Key + " || " + CercaVoce(pair.Key));
                    UP4.WriteLine("|-");
                }
            }
            UP3.WriteLine("|}"); UP4.WriteLine("|}");
            UP3.Close(); UP4.Close();


        }

        private static string CercaVoce(string disambiguante)
        {
            string line = "";
            StreamReader file = new StreamReader(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\Disambiguanti.txt", Encoding.UTF8);
            while ((line = file.ReadLine()) != null)
            {
                string[] tmp = line.Split('\t');
                if (tmp[1] == disambiguante && tmp[2] == "0")
                {
                    return "[[" + tmp[0] + "]]";
                }
            }
            return "";
        }

        private void btnTestRegEx_Click(object sender, EventArgs e)
        {
            Regex regex = new Regex("(\\{\\{(?:[nN]d[ |]|[nN]ota disambigua|[tT]orna a)[^}]*}}|__[^_]+__)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            bool IsMatch = regex.IsMatch(txtIn.Text);

            if (IsMatch)
            {
                txtOut.Text = "TROVATA";
            }
            else
            {
                txtOut.Text = "NON TROVATA";
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            // Regex per leggere i template
            //Regex regex = new Regex("{{Sportivo(?>[^\\{\\}]+|\\{(?<DEPTH>)|\\}(?<-DEPTH>))*(?(DEPTH)(?!))}}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled );

            Site WP = new Site("https://it.wikipedia.org", user, password);
            Site WD = new Site("https://www.wikidata.org", user, password);

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Trim().Split('\t');
                string strJson = "";
                Pages pages = new Pages();
                strJson = WP.LoadWP(tmp[0]);
                pages = JsonConvert.DeserializeObject<Pages>(strJson);
                foreach (Page p in pages.query.pages.Values)
                {
                    string testo = p.revisions[0].text;
                    //Regex regex = new Regex(@"\|\s*Altezza\s*=[^\\n\\r|}]*", RegexOptions.IgnoreCase);
                    Regex regex = new Regex("\\|\\s*Altezza\\s*=[^\\n\\r]*", RegexOptions.IgnoreCase);
                    //string regexReplace = "";
                    //string result = regex.Replace(testo, regexReplace);
                    string alt = regex.Match(testo).Value;

                    //regex = new Regex(@"\|\s*Peso\s*=[^\\n\\r|}]*", RegexOptions.IgnoreCase);
                    regex = new Regex("\\|\\s*Peso\\s*=[^\\n\\r]*", RegexOptions.IgnoreCase);
                    //regexReplace = "";
                    //result = regex.Replace(result, regexReplace);
                    string peso = regex.Match(testo).Value;
                    string strJsonWD = WD.LoadWD(p.item);
                    Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJsonWD, new DatavalueConverter());

                    Entity entity = EntityList.entities[p.item];
                    Datavalue Valore = Utility.CreateDataValue("+" + tmp[1] + "|Q174728||", Utility.TypeData.Quantity); //Category
                    string id = entity.PropertyValueId("P2048", Valore);
                    if (id != "")
                    {
                        string res = WD.SetReferenceURL(id, tmp[2]);
                        txtOut.AppendText(res + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText("No save" + Environment.NewLine);
                    }
                }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            HashSet<string> uniqueRef = new HashSet<string>();

            StreamReader file = new System.IO.StreamReader($"{DumpPath}wikidata-20170814-all.json", Encoding.UTF8);
            string line = file.ReadLine(); // first line in dump is "[" so we must skip

            EntitiesDump item = new EntitiesDump();
            while ((line = file.ReadLine()) != null)
            {
                line = line.Remove(line.Length - 1);
                string strJson = "{\"entity\":" + line + "}"; //necessary for different structure of json dump
                try
                {
                    item = JsonConvert.DeserializeObject<EntitiesDump>(strJson, new DatavalueConverter());
                    if (item.entity.type == "item")
                    {
                        foreach (KeyValuePair<string, List<Claim>> tmpClaims in item.entity.claims)
                        {
                            List<Claim> tmpListClaim = tmpClaims.Value;
                            foreach (Claim tmpClaim in tmpListClaim)
                            {
                                if (tmpClaim.references != null)
                                {
                                    foreach (Reference ref1 in tmpClaim.references)
                                    {
                                        string val = "";
                                        foreach (var re in ref1.snaks.Keys)
                                        {
                                            val += re + '\t';
                                        }
                                        uniqueRef.Add(val);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    //log.WriteLine("#[[" + item.entity.id + "]] " + ex.Message);
                }
            }

            StreamWriter refSW = new StreamWriter(ReportsPath + "Reference.txt", false, Encoding.UTF8);
            foreach (string re in uniqueRef)
            {
                refSW.WriteLine(re);
            }
            refSW.Close();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            List<string> l = new List<string>();
            HashSet<string> uniqueRef = new HashSet<string>();

            StreamReader file = new StreamReader(ReportsPath + "Reference.txt", Encoding.UTF8);
            string line = "";
            while ((line = file.ReadLine()) != null)
            {
                string[] tmp = line.Split('\t');
                l = new List<string>();
                foreach (string s in tmp)
                {
                    l.Add(s);
                }
                l.Sort();
                string linea = "";
                foreach (string s in l)
                {
                    if (s != "")
                    {
                        linea += s + '\t';
                    }
                }
                uniqueRef.Add(linea);
            }
            foreach (string re in uniqueRef)
            {
                txtOut.AppendText(re + Environment.NewLine);
            }
            Console.Write("");
        }

        private void button27_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                if (lines[idx].IndexOf("P") == 1)
                {
                    tmpList += "Property:" + lines[idx] + "|";
                }
                else
                {
                    tmpList += lines[idx] + "|";
                }
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);

            foreach (string s in list)
            {
                //string strJson = WD.LoadWD(s, WikimediaAPI.LoadTypeWD.Label);
                string strJson = WD.LoadWD(s);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        if (entity.labels.ContainsKey("it"))
                        {
                            //txtOut.AppendText("# [[" + entity.id + "|" + entity.labels["it"].value + "]]" + Environment.NewLine);
                            txtOut.AppendText(entity.labels["it"].value + " (" + entity.id + ") " + entity.datatype + Environment.NewLine);
                        }
                        else if (entity.labels.ContainsKey("en"))
                        {
                            txtOut.AppendText(entity.labels["en"].value + " [" + entity.id + "]" + entity.datatype + Environment.NewLine);
                        }
                        else
                        {
                            txtOut.AppendText("(" + entity.id + ")" + entity.datatype + Environment.NewLine);
                        }
                    }
                }
            }

            MessageBox.Show("FINITO");
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            txtIn.Text = txtOut.Text;
            txtOut.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";

            Site WD = new Site("https://www.wikidata.org", user, password);
            if (WD.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            txtMessage.Text = LabelAndDescription.WriteLabelSameAsSitelink(WD, txtIn.Text, ReportsPath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            //txtOut.AppendText("{| class='wikitable sortable' style='width:100%'" + Environment.NewLine);
            //txtOut.AppendText("!item 1" + Environment.NewLine);
            //txtOut.AppendText("!Label 1" + Environment.NewLine);
            //txtOut.AppendText("!Wiki 1" + Environment.NewLine);
            //txtOut.AppendText("!item 2" + Environment.NewLine);
            //txtOut.AppendText("!Label 2" + Environment.NewLine);
            //txtOut.AppendText("!Wiki 2" + Environment.NewLine);
            //txtOut.AppendText("!Lang" + Environment.NewLine);
            string tmpRes = "Item" + '\t' + "Label" + '\t' + "Sitelink" + '\t' + "Lang" + '\t' + "Description" + Environment.NewLine;
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Split('\t');
                string strJson = WD.LoadWD(tmp[0] + "|" + tmp[1]);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());

                Entity first = EntityList.entities[tmp[0]];
                Entity second = EntityList.entities[tmp[1]];

                string lab1 = "";
                string lab2 = "";
                if (first.labels.ContainsKey(tmp[2]))
                {
                    lab1 = first.labels[tmp[2]].value;
                }
                if (second.labels.ContainsKey(tmp[2]))
                {
                    lab2 = second.labels[tmp[2]].value;
                }

                string link1 = "";
                string link2 = "";
                if (first.sitelinks.ContainsKey(tmp[3] + "wiki"))
                {
                    link1 = first.sitelinks[tmp[3] + "wiki"].title;
                }
                if (second.sitelinks.ContainsKey(tmp[3] + "wiki"))
                {
                    link2 = second.sitelinks[tmp[3] + "wiki"].title;
                }
                string desc1 = "";
                string desc2 = "";
                if (first.descriptions.ContainsKey(tmp[2]))
                {
                    desc1 = first.descriptions[tmp[2]].value;
                }
                if (second.descriptions.ContainsKey(tmp[2]))
                {
                    desc2 = second.descriptions[tmp[2]].value;
                }

                tmpRes += tmp[0] + '\t' + lab1 + '\t' + link1 + '\t' + tmp[2] + '\t' + desc1 + '\t' + Environment.NewLine;
                tmpRes += tmp[1] + '\t' + lab2 + '\t' + link2 + '\t' + tmp[2] + '\t' + desc2 + '\t' + Environment.NewLine;

                //txtOut.AppendText("|-" + Environment.NewLine);
                //txtOut.AppendText("| [[" + tmp[0] + "]]" + Environment.NewLine);
                //txtOut.AppendText("| " + lab1  + Environment.NewLine);
                //txtOut.AppendText("| " + link1 + Environment.NewLine);
                //txtOut.AppendText("| [[" + tmp[1] + "]]" + Environment.NewLine);
                //txtOut.AppendText("| " + lab2 + Environment.NewLine);
                //txtOut.AppendText("| " + link2 + Environment.NewLine);
                //txtOut.AppendText("| " + tmp[2] + Environment.NewLine);
            }
            //txtOut.AppendText("|}" + Environment.NewLine);
            txtOut.Text = tmpRes;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string query = "";
            query += "?language=" + txtPetLang.Text;
            query += "&project=" + txtPetProject.Text;
            query += "&categories=" + txtPetCat.Text;
            //query += "&templates_yes=" + txtPetCat.Text;
            query += "&ns[14]=1";
            query += "&wikidata_item=with";
            query += "&interface_language=en";

            //query = "?language=commons&project=wikimedia&categories=Category%20redirects&ns%5B14%5D=1&wikidata_item=with&wikidata_prop_item_use=Q4167836&interface_language=en&active_tab=";
            string tmp = ListGenerator.PetScan(query);

            Site WD = new Site("https://www.wikidata.org", user, password);
            List<string> list = Utility.SplitInChunk(tmp, "|", 500);
            foreach (string s in list)
            {
                //string strJson = WD.LoadWD(s, WikimediaAPI.LoadTypeWD.Label);
                string strJson = WD.LoadWD(s);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        if (entity.sitelinks.Count() == 1)
                        {
                            txtOut.AppendText(entity.id + '\t' + "delete" + '\t' + Environment.NewLine);
                        }
                        else
                        {
                            txtOut.AppendText(entity.id + '\t' + txtPetLang.Text + "wiki" + '\t' + Environment.NewLine);
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Site WD = new Site("https://www.wikidata.org", user, password);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Split('\t');
                string res = WD.DelSiteLink(tmp[0], tmp[1], ": is a Soft redirect");
                txtOut.AppendText(res + Environment.NewLine);
            }
        }

        private void btnAreDisambiguation_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            Entities EntityList = new Entities();
            string strJson = WD.LoadWD(txtIn.Text);
            EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
            Entity item = EntityList.entities[txtIn.Text];
            txtOut.Text = "";
            foreach (SiteLink sl in item.sitelinks.Values)
            {
                string site = sl.site;
                string page = sl.title;
                if (Utility.IsWikiDisambiguation(site, page))
                {
                    txtOut.AppendText(site + "(*)" + Environment.NewLine);
                }
                else
                {
                    txtOut.AppendText(site + Environment.NewLine);
                }
            }
        }

        private void btnNumberSitelink_Click(object sender, EventArgs e)
        {
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);
            Site WD = new Site("https://www.wikidata.org", user, password);
            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        txtOut.AppendText(entity.id + ":" + entity.sitelinks.Count() + Environment.NewLine);
                    }
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            toolStripStatusMessage.Text = "";
            Site WD = new Site("https://www.wikidata.org", user, password);
            if (WD.User == "")
            {
                toolStripStatusMessage.Text = "Need a valid BOT user and or password";
                return;
            }
            var res = ListGenerator.ListFromSearch(WD, txtIn.Text, "0", 500);
            txtIn.Text = res.list.Replace("|", Environment.NewLine);

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";

            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);

            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        if (entity.descriptions.ContainsKey("es"))
                        {
                            txtOut.AppendText(entity.id + ":" + entity.descriptions["es"].value + Environment.NewLine);
                        }

                    }
                }
            }


        }

        private void button11_Click(object sender, EventArgs e)
        {
            string lis = "wd:Q36941204 wd:Q28549562 wd:Q28549526 wd:Q24863872 wd:Q24859728 wd:Q4078232 wd:Q4078232 wd:Q4044852 wd:Q3380287 wd:Q2896171 wd:Q2896026 wd:Q2880879 wd:Q2701235 wd:Q2699470 wd:Q2680022 wd:Q2678485 wd:Q2364746 wd:Q2307454 wd:Q2301871 wd:Q2295423 wd:Q1648143 wd:Q952938 wd:Q952233 wd:Q840855 wd:Q392072 wd:Q342524 wd:Q312976 wd:Q307185 wd:Q294576 wd:Q293454 wd:Q293450 wd:Q44260 wd:Q13034 wd:Q43273338 wd:Q43264314 wd:Q42560415 wd:Q35124488 wd:Q25304515 wd:Q25167517 wd:Q24285801 wd:Q20890567 wd:Q19860867 wd:Q18921427 wd:Q18921427 wd:Q18065402 wd:Q17621031 wd:Q16816656 wd:Q11273989 wd:Q9039516 wd:Q9020700 wd:Q8647882 wd:Q7579438 wd:Q7578518 wd:Q7574238 wd:Q7287786 wd:Q6940442 wd:Q6936912 wd:Q6398301 wd:Q6299199 wd:Q6294288 wd:Q4208941 wd:Q3191464 wd:Q1457861 wd:Q703936 wd:Q610885 wd:Q116721 wd:Q11502 wd:Q5019 wd:Q4949 wd:Q3128 wd:Q2683 wd:Q1492 wd:Q1124 wd:Q38913315 wd:Q36697566 wd:Q31218995 wd:Q30711277 wd:Q30164380 wd:Q28936076 wd:Q28380549 wd:Q21984962 wd:Q21049132 wd:Q20829929 wd:Q20829929 wd:Q20470750 wd:Q18515201 wd:Q17635038 wd:Q16609686 wd:Q16286588 wd:Q16280700 wd:Q13014035 wd:Q11685764 wd:Q11452329 wd:Q11164220 wd:Q10290303 wd:Q8630355 wd:Q8470939 wd:Q8465439 wd:Q8464950 wd:Q8464738 wd:Q8463867 wd:Q8463781 wd:Q8463743 wd:Q8462957 wd:Q8462813 wd:Q8106679 wd:Q8049049 wd:Q791541 wd:Q773591 wd:Q507317 wd:Q497429 wd:Q496027 wd:Q41532668 wd:Q37912621 wd:Q25466818 wd:Q20423129 wd:Q12957718 wd:Q12957509 wd:Q12957509 wd:Q12957509 wd:Q12957509 wd:Q12848430 wd:Q12016474 wd:Q11402380 wd:Q9417719 wd:Q9410287 wd:Q9227468 wd:Q8758980 wd:Q8498286 wd:Q8485176 wd:Q8484604 wd:Q8481604 wd:Q8307757 wd:Q8302017 wd:Q8297777 wd:Q7412189 wd:Q7239651 wd:Q7239651 wd:Q7239651 wd:Q7239651 wd:Q7235789 wd:Q7234449 wd:Q6708981 wd:Q6703318 wd:Q6260893 wd:Q6260624 wd:Q6260610 wd:Q6254305 wd:Q5541790 wd:Q5541790 wd:Q5541790 wd:Q3814268 wd:Q3386062 wd:Q2105066 wd:Q1617427 wd:Q483718 wd:Q483225 wd:Q451968 wd:Q399565 wd:Q353258 wd:Q233243 wd:Q200718 wd:Q67953 wd:Q41155 wd:Q40859 wd:Q39857 wd:Q9957 wd:Q8704 wd:Q4641 wd:Q4061 wd:Q3659 wd:Q3601 wd:Q3130 wd:Q218 wd:Q29937273 wd:Q29937152 wd:Q29858951 wd:Q28966668 wd:Q25647445 wd:Q25647445 wd:Q11584087 wd:Q10044287 wd:Q8611041 wd:Q8369229 wd:Q8357795 wd:Q7806808 wd:Q273652 wd:Q272276 wd:Q40950 wd:Q42901232 wd:Q37323951 wd:Q30723715 wd:Q29430154 wd:Q29430154 wd:Q29430154 wd:Q29430154 wd:Q28521908 wd:Q27999521 wd:Q26031087 wd:Q25213915 wd:Q25199826 wd:Q24067722 wd:Q22738100 wd:Q22737388 wd:Q22727336 wd:Q22265817 wd:Q22081400 wd:Q22007000 wd:Q21970572 wd:Q21561319 wd:Q21388457 wd:Q21005130 wd:Q20854901 wd:Q20847048 wd:Q20371315 wd:Q18575713 wd:Q18211167 wd:Q16911350 wd:Q15831442 wd:Q15830963 wd:Q14448130 wd:Q14439684 wd:Q13301252 wd:Q13285908 wd:Q13280381 wd:Q13034067 wd:Q11521611 wd:Q11196201 wd:Q9501583 wd:Q9475595 wd:Q9475354 wd:Q9293883 wd:Q9182245 wd:Q9106980 wd:Q9097870 wd:Q9096388 wd:Q9089569 wd:Q9082435 wd:Q9079436 wd:Q9079402 wd:Q8951589 wd:Q8922699 wd:Q8899559 wd:Q8702730 wd:Q8691751 wd:Q8689047 wd:Q8281043 wd:Q8144395 wd:Q8136114 wd:Q8135581 wd:Q8132341 wd:Q8131989 wd:Q8131453 wd:Q8130148 wd:Q8128019 wd:Q7908679 wd:Q7798656 wd:Q7795635 wd:Q7736776 wd:Q7405407 wd:Q7400576 wd:Q7394673 wd:Q7216786 wd:Q7156626 wd:Q7142012 wd:Q7057166 wd:Q7055755 wd:Q7037380 wd:Q7033782 wd:Q6671237 wd:Q6670766 wd:Q6655864 wd:Q6648552 wd:Q6648123 wd:Q6647153 wd:Q6644083 wd:Q6642856 wd:Q6422629 wd:Q6369191 wd:Q6316270 wd:Q6120793 wd:Q5623657 wd:Q5518037 wd:Q5110198 wd:Q5109734 wd:Q5095847 wd:Q4465021 wd:Q4290464 wd:Q4198988 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3839895 wd:Q3781715 wd:Q3196760 wd:Q3196760 wd:Q3196760 wd:Q3189533 wd:Q2693222 wd:Q2641834 wd:Q2581386 wd:Q2073200 wd:Q2073200 wd:Q2073200 wd:Q1347154 wd:Q1323892 wd:Q1304656 wd:Q1048476 wd:Q573348 wd:Q562558 wd:Q560242 wd:Q555056 wd:Q549005 wd:Q266445 wd:Q236987 wd:Q184258 wd:Q93373 wd:Q20732 wd:Q11948 wd:Q7934 wd:Q7903 wd:Q4636 wd:Q2222 wd:Q912 wd:Q72 wd:Q41556020 wd:Q37662290 wd:Q22162719 wd:Q21013216 wd:Q21013206 wd:Q21000781 wd:Q20999513 wd:Q20998465 wd:Q20804377 wd:Q20239122 wd:Q19848471 wd:Q17220280 wd:Q15118820 wd:Q13502110 wd:Q12609027 wd:Q10955985 wd:Q10514357 wd:Q9677421 wd:Q9400300 wd:Q9376733 wd:Q8496300 wd:Q8487616 wd:Q8486972 wd:Q8485285 wd:Q8484417 wd:Q8483940 wd:Q8483802 wd:Q8482642 wd:Q8478486 wd:Q8079069 wd:Q8078917 wd:Q8076768 wd:Q8067043 wd:Q7716233 wd:Q7966791 wd:Q7962728 wd:Q7580126 wd:Q7579100 wd:Q6642526 wd:Q6641081 wd:Q6315383 wd:Q6237333 wd:Q6237328 wd:Q6237298 wd:Q5042363 wd:Q4328190 wd:Q4054271 wd:Q4049727 wd:Q4049721 wd:Q2874705 wd:Q2004991 wd:Q1535438 wd:Q990651 wd:Q913471 wd:Q911237 wd:Q539766 wd:Q539766 wd:Q527501 wd:Q215346 wd:Q211928 wd:Q209015 wd:Q208534 wd:Q43196390 wd:Q42737692 wd:Q42576021 wd:Q39070768 wd:Q27978936 wd:Q25012539 wd:Q22744177 wd:Q16249384 wd:Q12270002 wd:Q10189809 wd:Q10082787 wd:Q9010016 wd:Q8808499 wd:Q8806796 wd:Q8795108 wd:Q8795108 wd:Q8549247 wd:Q8545894 wd:Q8542977 wd:Q7763486 wd:Q7053863 wd:Q7041350 wd:Q6196794 wd:Q4740163 wd:Q4740163 wd:Q4716651 wd:Q4235534 wd:Q3433253 wd:Q3425245 wd:Q2246720 wd:Q1353800 wd:Q1350797 wd:Q1342731 wd:Q888673 wd:Q888673 wd:Q450763 wd:Q423656 wd:Q131374 wd:Q43283219 wd:Q37301419 wd:Q32408331 wd:Q30907404 wd:Q26252448 wd:Q25329447 wd:Q22771678 wd:Q22725361 wd:Q22722937 wd:Q21935586 wd:Q20693064 wd:Q19628225 wd:Q18921873 wd:Q17311550 wd:Q16807398 wd:Q16618261 wd:Q16225904 wd:Q14358107 wd:Q14357914 wd:Q14357659 wd:Q14347284 wd:Q14347272 wd:Q14346233 wd:Q14346132 wd:Q13617203 wd:Q13253791 wd:Q13219956 wd:Q13219733 wd:Q12794304 wd:Q12329823 wd:Q12329823 wd:Q12329823 wd:Q9756138 wd:Q9592189 wd:Q9536974 wd:Q9536974 wd:Q9533089 wd:Q9229321 wd:Q9228661 wd:Q9228119 wd:Q9219308 wd:Q9218226 wd:Q8699809 wd:Q8693125 wd:Q8693085 wd:Q8691069 wd:Q8690041 wd:Q8486620 wd:Q8486620 wd:Q8427813 wd:Q8366647 wd:Q8361339 wd:Q1351877 wd:Q1293490 wd:Q1122783 wd:Q350306 wd:Q346735 wd:Q157781 wd:Q42236841 wd:Q41445080 wd:Q41445080 wd:Q41445080 wd:Q39134549 wd:Q39134549 wd:Q39134549";

            var res = ListGenerator.WQS("SELECT ?item {VALUES ?item{" + lis + "}.?item wdt:P31 wd:Q4167836}", "item");
            if (res.count != 0) { txtOut.Text = res.list.Replace("|", Environment.NewLine); }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string from = dateRecentChange.Value.Year.ToString() + "-" + dateRecentChange.Value.Month.ToString() + "-" + dateRecentChange.Value.Day.ToString()+"T00:00:000Z";
            string to = dateRecentChange.Value.Year.ToString() + "-" + dateRecentChange.Value.Month.ToString() + "-" + (dateRecentChange.Value.AddDays(1).Day.ToString()) + "T00:00:000Z";
            Site WD = new Site("https://www.wikidata.org", user, password);
            string res = ListGenerator.RecentChanges(from, to, WD);

            txtOut.Text = res.Replace("|", Environment.NewLine);

        }

        private void button15_Click(object sender, EventArgs e)
        {
            string from = "";
            string to = "";
            from = txtFind.Text;
            to = txtReplace.Text;
            Site WD = new Site("https://www.wikidata.org", user, password);

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Trim().Split('\t');
                string strJson = "";
                Pages pages = new Pages();
                strJson = WD.LoadWP(tmp[0]);
                pages = JsonConvert.DeserializeObject<Pages>(strJson);
                foreach (Page p in pages.query.pages.Values)
                {
                    string testo = p.revisions[0].text;
                    testo=testo.Replace(from, to);
                    string resp=WD.SavePage(p.title, testo, "BOT: Fix of [[Special:LintErrors]]");
                    txtOut.AppendText(resp + Environment.NewLine);
                }
            }


            //frmFindAndReplace frmReplace = new frmFindAndReplace();
            //frmReplace.user = user;
            //frmReplace.password = password;
            //frmReplace.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string t = "https://" + txtWikiLang.Text + "." + txtWikiProject.Text + ".org";

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);
            string strJson = "";

            Site WP = new Site("https://" + txtWikiLang.Text + "." + txtWikiProject.Text + ".org", user, password);

            foreach (string s in list)
            {
                Pages pages = new Pages();
                strJson = WP.LoadWPnoText(s);
                pages = JsonConvert.DeserializeObject<Pages>(strJson);
                foreach (Page p in pages.query.pages.Values)
                {
                    txtOut.AppendText(p.item + '\t' + p.title + Environment.NewLine);
                    Console.WriteLine("");
                }
            }
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            //https://www.wikidata.org/wiki/Q19017186
            //https://www.wikidata.org/w/index.php?title=Q19017186&action=history
            //https://www.wikidata.org/wiki/Special:WhatLinksHere/Q19017186?namespace=0
            //https://www.wikidata.org/w/index.php?title=Q11321048&action=info#Propriet%C3%A0_della_pagina

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string sOut= "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string tmp= string.Format(@"https://www.wikidata.org/wiki/{0}"+'\t'+ "https://www.wikidata.org/w/index.php?title={0}&action=history" + '\t' + "https://www.wikidata.org/wiki/Special:WhatLinksHere/{0}?namespace=0" + '\t' + "https://www.wikidata.org/w/index.php?title={0}&action=info#Propriet%C3%A0_della_pagina" + Environment.NewLine , lines[idx]);
                sOut += tmp;
            }
            txtOut.Text = sOut;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);

            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s, VBot.Site.LoadTypeWD.Label);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        if (entity.labels.ContainsKey("it"))
                        {
                            txtOut.AppendText(entity.labels["it"].value + '\t' + entity.id + Environment.NewLine);
                        }
                        else
                        {
                            txtOut.AppendText("(" + entity.id + ")" + Environment.NewLine);
                        }
                    }
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Site WP = new Site("https://it.wikipedia.org", user, password);
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            string res = "";
            

            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);

            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            foreach (string s in list)
            {
                string respStr = WP.Info(s);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(respStr);
                foreach (XmlNode node in doc.SelectNodes("/api/query/pages/page"))
                {
                    string item = node.Attributes["title"].Value;
                    if (node.Attributes["redirect"] == null)
                    {
                        res += item + Environment.NewLine;

                    }
                }
            }
            txtOut.Text = res;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            //string tmp = "Q17629955";

            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string tmp = lines[idx];
                string strJson = WD.LoadWD(tmp);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity item = EntityList.entities[tmp];
                string claim = item.claims["P179"][0].id;
                if (item.PropertyExist("P155"))
                {
                    txtMessage.AppendText(tmp + '\t' + "P155" + Environment.NewLine );
                }
                if (item.PropertyExist("P156"))
                {
                    txtMessage.AppendText(tmp + '\t' + "P156" + Environment.NewLine);
                }

                if (!item.claims["P179"][0].qualifiers.ContainsKey("P155"))
                {
                    string res = WD.SetQualifier(claim, "P155");
                    if (res.IndexOf("\"success\":1") !=-1)
                    {
                        txtOut.AppendText(tmp + ": " + "OK" + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText(tmp + ": " + res + Environment.NewLine);
                    }
                }
                else
                {
                    txtOut.AppendText(tmp + ": " + "Esiste già" + Environment.NewLine);
                }
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            // "Q13495685"
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string tmp = lines[idx];
                string strJson = WD.LoadWD(tmp);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity item = EntityList.entities[tmp];
                string claim = item.claims["P179"][0].id;
                if(item.claims["P179"][0].qualifiers.ContainsKey("P155"))
                {
                    string qualifierHash = item.claims["P179"][0].qualifiers["P155"][0].hash;
                    string res = WD.RemoveQualifier(claim, qualifierHash);
                    if (res.IndexOf("\"success\":1") != -1)
                    {
                        //txtOut.AppendText(tmp + ": " + "OK" + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText(tmp + ": " + res + Environment.NewLine);
                    }
                }

                if (item.claims["P179"][0].qualifiers.ContainsKey("P156"))
                {
                    string qualifierHash = item.claims["P179"][0].qualifiers["P156"][0].hash;
                    string res = WD.RemoveQualifier(claim, qualifierHash);
                    if (res.IndexOf("\"success\":1") != -1)
                    {
                        txtOut.AppendText(tmp + ": " + "OK" + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText(tmp + ": " + res + Environment.NewLine);
                    }
                }
            }
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            // "Q13495685"
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string tmp = lines[idx];
                string strJson = WD.LoadWD(tmp);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity item = EntityList.entities[tmp];
                string claim = item.claims["P361"][0].id;
                if (item.claims["P361"][0].qualifiers.ContainsKey("P155"))
                {
                    string qualifierHash = item.claims["P361"][0].qualifiers["P155"][0].hash;
                    string res = WD.RemoveQualifier(claim, qualifierHash);
                    if (res.IndexOf("\"success\":1") != -1)
                    {
                        txtOut.AppendText(tmp + ": " + "OK" + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText(tmp + ": " + res + Environment.NewLine);
                    }
                }

                if (item.claims["P361"][0].qualifiers.ContainsKey("P156"))
                {
                    string qualifierHash = item.claims["P361"][0].qualifiers["P156"][0].hash;
                    string res = WD.RemoveQualifier(claim, qualifierHash);
                    if (res.IndexOf("\"success\":1") != -1)
                    {
                        txtOut.AppendText(tmp + ": " + "OK" + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText(tmp + ": " + res + Environment.NewLine);
                    }
                }
            }
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
        }

        private void button30_Click(object sender, EventArgs e)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            // "Q13495685"
            string[] lines = txtIn.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string tmp = lines[idx];
                string strJson = WD.LoadWD(tmp);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity item = EntityList.entities[tmp];
                string claim = item.claims["P179"][0].id;
                if (item.claims["P179"][0].qualifiers.ContainsKey("P1545"))
                {
                    string qualifierHash = item.claims["P179"][0].qualifiers["P1545"][0].hash;
                    string res = WD.RemoveQualifier(claim, qualifierHash);
                    if (res.IndexOf("\"success\":1") != -1)
                    {
                        //txtOut.AppendText(tmp + ": " + "OK" + Environment.NewLine);
                    }
                    else
                    {
                        txtOut.AppendText(tmp + ": " + res + Environment.NewLine);
                    }
                }
            }
            for (int i = 0; i <= 5; i++)
            {
                Console.Beep();
            }
        }

        private void btnBilancioDemografico_Click(object sender, EventArgs e)
        {
            string Anno = txtAnnoBD.Text;
            string Mese = txtMeseBD.Text;
            string Title = "";
            string res = "";
            res = "local data = {}" + Environment.NewLine + Environment.NewLine; ;
            res += "data.popolazione={" + Environment.NewLine;

            for (int i = 1; i <= 112; i++)
            {
                string result = ListGenerator.BilancioDemografico(Anno, Mese, i.ToString());
                if (result.IndexOf("Bilancio demografico")!=-1)
                {
                    int from = 0;
                    int to = 0;
                    from = result.IndexOf("<table");
                    to = result.IndexOf("</table>", from);
                    string table1 = result.Substring(from, to - from);
                    if (Title == "")
                    {
                        from = table1.IndexOf("<b>");
                        to = table1.IndexOf("<br>");
                        Title = table1.Substring(from + 3, to - from - 3);
                    }

                    from = result.IndexOf("<table", result.IndexOf("</table>"));
                    to = result.IndexOf("</table>", from);
                    string table2 = result.Substring(from, to - from + 8);

                    string RowExpression = "<tr[^>]*>(.*?)</tr>";
                    string ColumnExpression = "<td[^>]*>(.*?)</td>";

                    MatchCollection Rows = Regex.Matches(table2,
                        RowExpression,
                        RegexOptions.Singleline |
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    foreach (Match Row in Rows)
                    {
                        if (Row.Value.IndexOf("<br>Comune</td>") == -1)
                        {
                            if (Row.Value.IndexOf("Maschi") != -1) { break; }
                            if (Row.Value.IndexOf("Femmine") != -1) { break; }

                            MatchCollection Columns = Regex.Matches(Row.Value,
                            ColumnExpression,
                            RegexOptions.Singleline |
                            RegexOptions.Multiline |
                            RegexOptions.IgnoreCase);
                            
                            if (Columns[0].Groups[1].Captures[0].Value != "Totale")
                            {
                                string tmp = "[\"" + Columns[0].Groups[1].ToString() + "\"]={\"";
                                tmp += Columns[10].Groups[1].ToString() + "\",\"";
                                tmp += Columns[1].Groups[1].ToString() + "\"}," + Environment.NewLine;
                                res += tmp;
                            }
                        }
                    }
                }
                else
                {
                    txtMessage.AppendText(i.ToString() + ": " + result);
                }
            }
            
            res = res.Remove(res.LastIndexOf(","));
            res += Environment.NewLine;
            res += "}" + Environment.NewLine;

            //res += "data.nota=\"[http://demo.istat.it/bilmens" + Anno + "gen/query1.php?&allrp=4&Pro=55&periodo=" + Mese + " Dato Istat] - " + title + "\"" + Environment.NewLine;
            res += "data.nota=\"[http://demo.istat.it/bilmens" + Anno + "gen/index.html Dato Istat] - " + Title + "\"" + Environment.NewLine + Environment.NewLine;

            res += "return data" + Environment.NewLine;
            txtOut.Text = res;

            Site WP = new Site("https://it.wikipedia.org", user, password);
            WP.SavePage("Modulo:Sandbox/ValterVB/Bilancio/Data", res, "Aggiornamento");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string Anno = txtAnnoBD.Text;
            string Mese = txtMeseBD.Text;
            string Title = "";
            string res = "";
            res = "" ;

            for (int i = 1; i <= 113; i++)
            {
                string result = ListGenerator.BilancioDemografico(Anno, Mese, i.ToString());
                if (result.IndexOf("Bilancio demografico") != -1)
                {
                    int from = 0;
                    int to = 0;
                    from = result.IndexOf("<table");
                    to = result.IndexOf("</table>", from);
                    string table1 = result.Substring(from, to - from);
                    if (Title == "")
                    {
                        from = table1.IndexOf("<b>");
                        to = table1.IndexOf("<br>");
                        Title = table1.Substring(from + 3, to - from - 3);
                    }

                    from = result.IndexOf("<table", result.IndexOf("</table>"));
                    to = result.IndexOf("</table>", from);
                    string table2 = result.Substring(from, to - from + 8);

                    string RowExpression = "<tr[^>]*>(.*?)</tr>";
                    string ColumnExpression = "<td[^>]*>(.*?)</td>";

                    MatchCollection Rows = Regex.Matches(table2,
                        RowExpression,
                        RegexOptions.Singleline |
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    foreach (Match Row in Rows)
                    {
                        if (Row.Value.IndexOf("<br>Comune</td>") == -1)
                        {
                            if (Row.Value.IndexOf("Maschi") != -1) { break; }
                            if (Row.Value.IndexOf("Femmine") != -1) { break; }

                            MatchCollection Columns = Regex.Matches(Row.Value,
                            ColumnExpression,
                            RegexOptions.Singleline |
                            RegexOptions.Multiline |
                            RegexOptions.IgnoreCase);

                            if (Columns[0].Groups[1].Captures[0].Value != "Totale")
                            {
                                string tmp = Columns[0].Groups[1].ToString() + '\t' +  Columns[10].Groups[1].ToString() + '\t' +  Columns[1].Groups[1].ToString() + '\t' + "http://demo.istat.it/bilmens" + Anno + "gen/query1.php?&allrp=4&Pro=" + i.ToString() + "&periodo=" + Mese + Environment.NewLine;
                                res += tmp;
                            }
                        }
                    }
                }
                else
                {
                    txtMessage.AppendText(i.ToString() + ": " + result);
                }
                txtOut.Text = res;
            }
        }
    }
}
