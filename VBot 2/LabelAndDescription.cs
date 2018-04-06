using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VBot
{
    class LabelAndDescription
    {
        /// <summary>
        /// Read the dump to find item to add labels and descriptions in some case
        /// Read chunk of 50k rows (see dim variable) and then elaborate in parallel
        /// </summary>
        /// <param name="DumpFile">Path and name of the dump</param>
        /// <param name="OutPathFile">Path for the output files</param>
        /// <returns></returns>
        public static string ReadDumpForLabelAndDescription(string DumpFile, string OutPathFile)
        {
            DateTime start = DateTime.Now;

            Datavalue dvCat = Utility.CreateDataValue("Q4167836", Utility.TypeData.Item);    //Category
            Datavalue dvCat2 = Utility.CreateDataValue("Q29848066", Utility.TypeData.Item);  //Category of event
            Datavalue dvDis = Utility.CreateDataValue("Q4167410", Utility.TypeData.Item);    //Disambiguation
            Datavalue dvTempl = Utility.CreateDataValue("Q11266439", Utility.TypeData.Item); //Template

            if (!System.IO.File.Exists(DumpFile)) { return "File don't exist"; }

            System.IO.StreamReader file = new System.IO.StreamReader(DumpFile, Encoding.UTF8);

            var Dis = new ConcurrentBag<string>();
            var Cat = new ConcurrentBag<string>();
            var Lab = new ConcurrentBag<string>();
            var Tem = new ConcurrentBag<string>();
            var Err = new ConcurrentBag<string>();

            string line = file.ReadLine(); // to skip the first row in dump because is "["
            int Nitem = 0;

            string error = "";
            int dim = 50000; //Dimension of the array
            int idx = 0;
            string[] arr = new string[dim];

            Object locker = new Object();
            while ((line = file.ReadLine()) != null)
            {
                arr[idx] = line;
                Nitem += 1;
                if (line == "]") // Last line of the dump
                {
                    Array.Resize(ref arr, idx); // Ridimension of the array for the last batch
                    dim = idx;
                }
                if (idx == dim - 1)
                {
                    Parallel.For(0, dim, idx2 =>
                    {
                        try
                        {
                            arr[idx2] = arr[idx2].Remove(arr[idx2].Length - 1);
                            arr[idx2] = "{\"entity\":" + arr[idx2] + "}";
                            EntitiesDump item = JsonConvert.DeserializeObject<EntitiesDump>(arr[idx2], new DatavalueConverter());

                            #region Disambiguation
                            //Disambiguation description and label block
                            //Must be a disambiguation with only one P31 and with only one sitelink
                            if (item.entity.type == "item" && item.entity.sitelinks != null && item.entity.PropertyValueExist("P31", dvDis) && item.entity.claims["P31"].Count() == 1 && item.entity.sitelinks.Count() == 1)
                            {
                                bool Write = false;
                                if (item.entity.sitelinks.First().Value.title.All(c => Utility.shortAlphabet.Contains(c))) //Check if sitelink has only these chars
                                {
                                    string label = Utility.DelDisambiguation(item.entity.sitelinks.First().Value.title, "()");
                                    foreach (string lang in Utility.lstLatin) //Check if is possible add the label
                                    {
                                        if (!item.entity.labels.ContainsKey(lang))
                                        {
                                            Write = true;
                                            break;
                                        }
                                        else if (item.entity.labels[lang].value != label)
                                        {
                                            Write = true;
                                            break;
                                        }
                                    }
                                }
                                if (!Write) //Check if is possible add some description
                                {
                                    foreach (KeyValuePair<string, string> DisDesc in Utility.dicDis) 
                                    {
                                        if (item.entity.descriptions == null || !item.entity.descriptions.ContainsKey(DisDesc.Key))
                                        {
                                            Write = true;
                                            break;
                                        }
                                    }
                                }
                                if (Write) { Dis.Add(item.entity.id); }
                            }
                            #endregion
                            #region Template
                            //Template description block
                            //Must be a template with only one P31 and with at least one sitelink
                            else if (item.entity.type == "item" && item.entity.sitelinks != null && item.entity.PropertyValueExist("P31", dvTempl) && item.entity.claims["P31"].Count() == 1)
                            {
                                bool Write = false;
                                foreach (SiteLink sl in item.entity.sitelinks.Values)
                                {
                                    string link = sl.site;
                                    if (link.EndsWith("wiki") && link != "commonswiki" && link != "wikidatawiki" && link != "mowiki") //projects to exclude, mowiki is closed
                                    {
                                        string lang = Utility.SitelinkToLanguages(link);
                                        if (lang=="") { lang = sl.site.Replace("wiki", "").Replace("_", "-"); }

                                        if (item.entity.descriptions != null && !item.entity.descriptions.ContainsKey(lang) && Utility.dicCat.ContainsKey(lang))
                                        {
                                            Write = true;
                                            break;
                                        }
                                    }
                                }
                                if (Write) { Tem.Add(item.entity.id); }
                            }
                            #endregion
                            #region Category
                            //Category description block
                            //Must be a category with only one P31 and at least one sitelink
                            else if (item.entity.type == "item" && item.entity.sitelinks != null && (item.entity.PropertyValueExist("P31", dvCat) || item.entity.PropertyValueExist("P31", dvCat2)) && item.entity.claims["P31"].Count() == 1)
                            {
                                bool Write = false;
                                foreach (SiteLink sl in item.entity.sitelinks.Values)
                                {
                                    string link = sl.site;
                                    if (link.EndsWith("wiki") && link != "commonswiki" && link != "wikidatawiki" && link != "mowiki") //projects to exclude, mowiki is closed
                                    {
                                        string lang = Utility.SitelinkToLanguages(link);
                                        if (lang == "") { lang = sl.site.Replace("wiki", "").Replace("_", "-"); }

                                        if (item.entity.descriptions != null && !item.entity.descriptions.ContainsKey(lang) && Utility.dicCat.ContainsKey(lang))
                                        {
                                            Write = true;
                                            break;
                                        }
                                    }
                                }
                                if (Write) { Cat.Add(item.entity.id); }
                            }
                            #endregion
                            #region Label
                            //Label block
                            //Must be an item with at least one sitelink
                            if (item.entity.type == "item" && item.entity.sitelinks != null)
                            {
                                Boolean InList = false;
                                foreach (Datavalue dv in Utility.dicP31ForLabel)  //P31 must be in the list
                                {
                                    if (item.entity.PropertyValueExist("P31", dv))
                                    {
                                        InList = true;
                                        break;
                                    }
                                }
                                if (InList)
                                {
                                    foreach (SiteLink sl in item.entity.sitelinks.Values)
                                    {
                                        string link = sl.site;
                                        if (link.EndsWith("wiki") && link != "commonswiki" && link != "wikidatawiki" && link != "mowiki" && link != "bhwiki" && link != "specieswiki" && link != "metawiki" && link != "mediawikiwiki") //mowiki and bhwiki are closed
                                        {
                                            string lang = Utility.SitelinkToLanguages(link);
                                            if (lang == "") { lang = sl.site.Replace("wiki", "").Replace("_", "-"); }

                                            if (InList || Utility.goodChars.Contains(sl.title[0]))
                                            {
                                                if (sl.title.IndexOf(",") == -1)
                                                {
                                                    if (!item.entity.labels.ContainsKey(lang))
                                                    {
                                                        if (lang != "??")
                                                        {
                                                            Lab.Add(item.entity.id);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            lock (locker) { error += ex.Message + "\t" + line + Environment.NewLine; }
                        }
                    });
                    idx = -1;
                }
                idx += 1;
            }

            System.IO.StreamWriter logD = new System.IO.StreamWriter(OutPathFile + "Disambigue.txt", false, Encoding.UTF8);
            System.IO.StreamWriter logC = new System.IO.StreamWriter(OutPathFile + "Category.txt", false, Encoding.UTF8);
            System.IO.StreamWriter logL = new System.IO.StreamWriter(OutPathFile + "Label.txt", false, Encoding.UTF8);
            System.IO.StreamWriter logT = new System.IO.StreamWriter(OutPathFile + "Template.txt", false, Encoding.UTF8);

            string result = string.Join(Environment.NewLine, Dis);
            logD.WriteLine(result); logD.Close();

            result = string.Join(Environment.NewLine, Cat);
            logC.WriteLine(result); logC.Close();

            result = string.Join(Environment.NewLine, Lab);
            logL.WriteLine(result); logL.Close();

            result = string.Join(Environment.NewLine, Tem);
            logT.WriteLine(result); logT.Close();

            DateTime end = DateTime.Now;
            string Mess =  "N° item: " + Nitem + Environment.NewLine;
            Mess += "Time start: " + start.ToString() + Environment.NewLine;
            Mess += "Time end: " + end.ToString() + Environment.NewLine;
            Mess += "Total time: " + (end - start).ToString() + Environment.NewLine;
            Mess += error;
            return Mess;
        }

        /// <summary>
        /// Write missing labels based on sitelink for some type of item
        /// </summary>
        /// <param name="strList">List of item (Qnumber), separated by NewLine</param>
        /// <param name="LogFile">Path for logFile</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        /// <returns>String with errors</returns>
        public static string  WriteLabel(Site WD, string strList, string LogFile)
        {
            DateTime start = DateTime.Now;
            string error = "";
            string lag = "";

            List<string> list = Utility.SplitInChunk(strList, Environment.NewLine, 500);

            Datavalue dvCat = Utility.CreateDataValue("Q4167836", Utility.TypeData.Item);  //Category
            Datavalue dvDis = Utility.CreateDataValue("Q4167410", Utility.TypeData.Item);  //Disambiguation
            Datavalue dvTem = Utility.CreateDataValue("Q11266439", Utility.TypeData.Item); //Template
            
            Random rnd = new Random();
            System.IO.StreamWriter log = new System.IO.StreamWriter(LogFile + "LogLabel" + rnd.Next(1, 10000000).ToString() + ".txt" , false, Encoding.UTF8);
            
            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
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
                                            lag += entity.id + "\t" + "maxlag" + Environment.NewLine;
                                            Thread.Sleep(5000);
                                        }
                                        else if (ret.IndexOf("Server non disponibile") != -1)
                                        {
                                            lag += entity.id + "\t" + "(503) Server non disponibile" + Environment.NewLine;
                                        }
                                        else if (ret.IndexOf("code=\"readonly\"") != -1)
                                        {
                                            lag += entity.id + "\t" + "read only" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            if (ret.IndexOf("error code=\"badtoken\"") != -1) { WD.GetToken(); }
                                            if (entity.PropertyValueExist("P31", dvCat)) { log.WriteLine("Category\t" + entity.id + "\t" + ret); }
                                            else if (entity.PropertyValueExist("P31", dvDis)) { log.WriteLine("Disamb\t" + entity.id + "\t" + ret); }
                                            else if (entity.PropertyValueExist("P31", dvTem)) { log.WriteLine("Template\t" + entity.id + "\t" + ret); }
                                            else { log.WriteLine("Other\t" + entity.id + "\t" + ret); }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error += entity.id + '\t' + ex.Message + Environment.NewLine; 
                                }
                            }
                        }
                    }
                }
            }
            log.Close();

            DateTime end = DateTime.Now;
            string Mess = "Time start: " + start.ToString() + Environment.NewLine;
            Mess += "Time end: " + end.ToString() + Environment.NewLine;
            Mess += "Total time: " + (end - start).ToString() + Environment.NewLine;
            Mess += error + Environment.NewLine;
            Mess += "===============" + Environment.NewLine ;
            Mess += lag;
            return Mess;
        }

        /// <summary>
        /// Write missing labels/descriptions for disambiguation item
        /// </summary>
        /// <param name="strList">List of item (Qnumber), separated by NewLine</param>
        /// <param name="LogFile">Path for logFile</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        /// <returns>String with errors</returns>
        public static string WriteDisambiguationLabelAndDescription(Site WD, string strList, string LogFile)
        {
            DateTime start = DateTime.Now;
            string error = "";
            string lag = "";
            string[] replacements = { "it", "ilo", "nb", "ca", "tr", "es" };

            List<string> list = Utility.SplitInChunk(strList, Environment.NewLine, 500);

            Datavalue dvDis = Utility.CreateDataValue("Q4167410", Utility.TypeData.Item); //Disambiguation

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
                                //if (entity.descriptions == null || ((DisDesc.Key == "it" || DisDesc.Key == "ilo" || DisDesc.Key == "nb" || DisDesc.Key == "ca" || DisDesc.Key == "tr" || DisDesc.Key == "es") && entity.descriptions.ContainsKey(DisDesc.Key) && entity.descriptions[DisDesc.Key].value != DisDesc.Value))
                                if (entity.descriptions == null || (replacements.Contains(DisDesc.Key)))
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
                                            lag += entity.id + "\t" + "maxlag" + Environment.NewLine;
                                            Thread.Sleep(5000);
                                        }
                                        else if (ret.IndexOf("Server non disponibile") != -1)
                                        {
                                            lag += entity.id + "\t" + "(503) Server non disponibile" + Environment.NewLine;
                                        }
                                        else if (ret.IndexOf("code=\"readonly\"") != -1)
                                        {
                                            lag += entity.id + "\t" + "read only" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            log.WriteLine("Disamb only\t" + entity.id + "\t" + ret);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error += entity.id + '\t' + ex.Message + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            log.Close();

            DateTime end = DateTime.Now;
            string Mess = "Time start: " + start.ToString() + Environment.NewLine;
            Mess += "Time end: " + end.ToString() + Environment.NewLine;
            Mess += "Total time: " + (end - start).ToString() + Environment.NewLine;
            Mess += error + Environment.NewLine;
            Mess += "===============" + Environment.NewLine;
            Mess += lag;
            return Mess;
        }

        /// <summary>
        /// Write missing descriptions for category item
        /// </summary>
        /// <param name="strList">List of item (Qnumber), separated by NewLine</param>
        /// <param name="LogFile">Path for logFile</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        /// <returns>String with errors</returns>
        public static string WriteCategoryDescription(Site WD, string strList, string LogFile)
        {
            DateTime start = DateTime.Now;
            string error = "";
            string lag = "";

            List<string> list = Utility.SplitInChunk(strList, Environment.NewLine, 500);

            Datavalue dvCat = Utility.CreateDataValue("Q4167836", Utility.TypeData.Item); //Category
            Datavalue dvCat2 = Utility.CreateDataValue("Q29848066", Utility.TypeData.Item); //Category of event

            Random rnd = new Random();
            System.IO.StreamWriter log = new System.IO.StreamWriter(LogFile + "LogCategory" + rnd.Next(1, 10000000).ToString() + ".txt", false, Encoding.UTF8);

            int cont = 0;            

            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());

                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        if (entity.sitelinks != null && (entity.PropertyValueExist("P31", dvCat)|| entity.PropertyValueExist("P31", dvCat2)) && entity.claims["P31"].Count() == 1)
                        {
                            Dictionary<string, string> Descriptions = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> CatDesc in Utility.dicCat)
                            {
                                if (entity.descriptions == null || !entity.descriptions.ContainsKey(CatDesc.Key))
                                {
                                    Descriptions.Add(CatDesc.Key, CatDesc.Value);
                                }
                            }
                            if (entity.descriptions != null && entity.descriptions.ContainsKey("no")) { Descriptions["no"] = ""; } // Per il norvegese "no" è sbagliato
                            if (Descriptions.Count() > 0)
                            {
                                try
                                {
                                    string ret = WD.EditEntity(entity.id, null, null, Descriptions, null, null, "BOT:Add category description");
                                    if (ret != "")
                                    {
                                        if (ret.IndexOf("code=\"maxlag\"") != -1)
                                        {
                                            lag += entity.id + "\t" + "maxlag" + Environment.NewLine;
                                            Thread.Sleep(5000);
                                        }
                                        else if (ret.IndexOf("Server non disponibile") != -1)
                                        {
                                            lag += entity.id + "\t" + "(503) Server non disponibile" + Environment.NewLine;
                                        }
                                        else if (ret.IndexOf("code=\"readonly\"") != -1)
                                        {
                                            lag += entity.id + "\t" + "read only" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            log.WriteLine("Category only\t" + entity.id + "\t" + ret);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error += entity.id + '\t' + ex.Message + Environment.NewLine;
                                }
                            }
                            cont++;
                        }
                    }
                }
            }
            log.Close();

            DateTime end = DateTime.Now;
            string Mess = "Time start: " + start.ToString() + Environment.NewLine;
            Mess += "Time end: " + end.ToString() + Environment.NewLine;
            Mess += "Total time: " + (end - start).ToString() + Environment.NewLine;
            Mess += error + Environment.NewLine;
            Mess += "===============" + Environment.NewLine;
            Mess += lag;
            return Mess;
        }

        /// <summary>
        /// Write missing descriptions for template item
        /// </summary>
        /// <param name="strList">List of item (Qnumber), separated by NewLine</param>
        /// <param name="LogFile">Path for logFile</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        /// <returns>String with errors</returns>
        public static string WriteTemplateDescription(Site WD, string strList, string OutFile)
        {
            DateTime start = DateTime.Now;
            string error = "";
            string lag = "";

            List<string> list = Utility.SplitInChunk(strList, Environment.NewLine, 500);

            Datavalue dvTem = Utility.CreateDataValue("Q11266439", Utility.TypeData.Item); //Template

            Random rnd = new Random();
            System.IO.StreamWriter log = new System.IO.StreamWriter(OutFile + "LogTemplate" + rnd.Next(1, 10000000).ToString() + ".txt", false, Encoding.UTF8);

            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = new Entities();
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());

                if (EntityList.entities != null)
                {
                    foreach (Entity entity in EntityList.entities.Values)
                    {
                        if (entity.sitelinks != null && entity.PropertyValueExist("P31", dvTem) && entity.claims["P31"].Count() == 1)
                        {
                            Dictionary<string, string> Descriptions = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, string> CatDesc in Utility.dicTempl)
                            {
                                if (entity.descriptions == null || !entity.descriptions.ContainsKey(CatDesc.Key))
                                {
                                    Descriptions.Add(CatDesc.Key, CatDesc.Value);
                                }
                            }
                            if (entity.descriptions != null && entity.descriptions.ContainsKey("no")) { Descriptions["no"] = ""; } // Per il norvegese "no" è sbagliato
                            if (Descriptions.Count() > 0)
                            {
                                try
                                {
                                    string ret = WD.EditEntity(entity.id, null, null, Descriptions, null, null, "BOT:Add template description");
                                    if (ret != "")
                                    {
                                        if (ret.IndexOf("code=\"maxlag\"") != -1)
                                        {
                                            lag += entity.id + "\t" + "maxlag" + Environment.NewLine;
                                            Thread.Sleep(5000);
                                        }
                                        else if (ret.IndexOf("Server non disponibile") != -1)
                                        {
                                            lag += entity.id + "\t" + "(503) Server non disponibile" + Environment.NewLine;
                                        }
                                        else if (ret.IndexOf("code=\"readonly\"") != -1)
                                        {
                                            lag += entity.id + "\t" + "read only" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            log.WriteLine("Template only\t" + entity.id + "\t" + ret);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error += entity.id + '\t' + ex.Message + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            log.Close();

            DateTime end = DateTime.Now;
            string Mess = "Time start: " + start.ToString() + Environment.NewLine;
            Mess += "Time end: " + end.ToString() + Environment.NewLine;
            Mess += "Total time: " + (end - start).ToString() + Environment.NewLine;
            Mess += error + Environment.NewLine;
            Mess += "===============" + Environment.NewLine;
            Mess += lag;
            return Mess;
        }

        /// <summary>Add the same label on latin language (only if don't exist)</summary>
        /// <param name="Item">string with Q number of the item</param>
        /// <param name="Label">Label to add</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        /// /// <returns>String with result</returns>
        public static string AddLabelLatin(Site WD, string Item, string Label)
        {
            Entities EntityList = new Entities();
            string Mess = "";
            string strJson = WD.LoadWD(Item);
            EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
            if (EntityList.entities != null)
            {
                foreach (Entity entity in EntityList.entities.Values)
                {
                    Dictionary<string, string> Labels = new Dictionary<string, string>();
                    foreach (string lang in Utility.lstLatin)
                    {
                        if (!entity.labels.ContainsKey(lang)) { Labels.Add(lang, Label.Trim()); }
                    }
                    if (Labels.Count() > 0)
                    {
                        string ret = WD.EditEntity(entity.id, null, Labels, null, null, null, "BOT:Add same label only on latin alphabet");
                        Mess+=ret != "" ? Utility.CleanApiError(ret) : "OK (" + Labels.Count() + ")";
                    }
                    else
                    {
                        Mess+="No add";
                    }
                }
            }
            return Mess;
        }

        /// <summary>Delete label of the second item then add label on the first item</summary>
        /// <param name="Mess">TextBox for error</param>
        /// <param name="strList">List of couple of item (Qnumber1, Qnumber2, Language id), separated by NewLine</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        /// <param name="Out">TextBox for result</param>
        public static void FixLabelConflict(TextBox Mess, TextBox Out, string strList, string user, string pwd)
        {
            Mess.Text  = "";
            Mess.Refresh();
            Site WD = new Site("https://www.wikidata.org", user, pwd);
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Entities EntityList = new Entities();
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                try
                {
                    string[] tmp = lines[idx].Split('\t');
                    string lang = tmp[3];
                    string strJson = WD.LoadWD(tmp[1]);
                    EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                    Entity entity1 = EntityList.entities.Values.First();
                    string sl1 = "";
                    if (lang=="de-ch")
                    {
                        if (entity1.sitelinks.ContainsKey("de" + "wiki")) { sl1 = entity1.sitelinks["de" + "wiki"].title.Replace("ß", "ss"); }
                    }
                    else if (lang == "be-tarask")
                    {
                        if (entity1.sitelinks.ContainsKey("be_x_old" + "wiki")) { sl1 = entity1.sitelinks["be_x_old" + "wiki"].title; }
                    }
                    else if (lang == "nb")
                    {
                        if (entity1.sitelinks.ContainsKey("no" + "wiki")) { sl1 = entity1.sitelinks["no" + "wiki"].title; }
                    }
                    else
                    {
                        if (entity1.sitelinks.ContainsKey(lang + "wiki")) { sl1 = entity1.sitelinks[lang + "wiki"].title; }
                    }
                    
                    strJson = WD.LoadWD(tmp[2].Replace("\r", ""));
                    EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                    Entity entity2 = EntityList.entities.Values.First();
                    string sl2 = "";
                    string lb2 = "";
                    if (lang == "de-ch")
                    {
                        if (entity2.sitelinks.ContainsKey("de" + "wiki")) { sl2 = entity2.sitelinks["de" + "wiki"].title.Replace("ß", "ss"); }
                        if (entity2.labels.ContainsKey(lang)) { lb2 = entity2.labels[lang].value; }
                    }
                    else if (lang == "be-tarask")
                    {
                        if (entity2.sitelinks.ContainsKey("be_x_old" + "wiki")) { sl2 = entity2.sitelinks["be_x_old" + "wiki"].title; }
                        if (entity2.labels.ContainsKey(lang)) { lb2 = entity2.labels[lang].value; }
                    }
                    else if (lang == "nb")
                    {
                        if (entity2.sitelinks.ContainsKey("no" + "wiki")) { sl2 = entity2.sitelinks["no" + "wiki"].title; }
                        if (entity2.labels.ContainsKey(lang)) { lb2 = entity2.labels[lang].value; }
                    }
                    else
                    {
                        if (entity2.sitelinks.ContainsKey(lang + "wiki")) { sl2 = entity2.sitelinks[lang + "wiki"].title; }
                        if (entity2.labels.ContainsKey(lang)) { lb2 = entity2.labels[lang].value; }
                    }

                    if (lb2 != "")
                    {
                        
                        string retDel=WD.DelLabel(tmp[2].Replace("\r", ""), "BOT:Delete " + lang + " label, see at  [[" + tmp[1] + "]] for possible merge", lang);
                        if (retDel.IndexOf("\"code\": \"maxlag\",") != -1)
                        {
                            Thread.Sleep(5000); ;
                            Mess.AppendText(tmp[2] + " maxlag");
                        }
                    }

                    Dictionary<string, string> Labels = new Dictionary<string, string>();
                    Labels.Add(lang, sl1);
                    string ret = WD.EditEntity(tmp[1], null, Labels, null, null, null, "BOT:Fix " + lang + " label for duplicate label + description in category or template item");
                    if (ret.IndexOf("\"code\": \"maxlag\",") != -1)
                    {
                        Thread.Sleep(5000); ;
                        Mess.AppendText(tmp[2] + " maxlag");
                    }
                    if (ret != "") { Mess.AppendText(tmp[1] + "§" + ret + Environment.NewLine); }

                    Labels = new Dictionary<string, string>();
                    Labels.Add(lang, sl2);
                    ret = WD.EditEntity(tmp[2].Replace("\r", ""), null, Labels, null, null, null, "BOT:Fix " + lang + " label for duplicate label + description in category or template item");
                    if (ret.IndexOf("\"code\": \"maxlag\",") != -1)
                    {
                        Thread.Sleep(5000); ;
                        Mess.AppendText(tmp[2] + " maxlag");
                    }
                    if (ret != "") { Mess.AppendText(tmp[2] + "§" + ret + Environment.NewLine); }

                    tmpList += tmp[1] + Environment.NewLine + tmp[2] + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    Mess.AppendText(ex.Message + Environment.NewLine);
                    Mess.Refresh();
                }
            }
            Out.Text = tmpList;
        }

        /// <summary>Write description like QuickStatements tool</summary>
        /// <param name="Mess">TextBox for error</param>
        /// <param name="strList">List of item (Qnumber, language, description Tab seprated, one item for row</param>
        /// <param name="OutFile">Path for logFile</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        public static void WriteDescriptionOnly(TextBox Mess, string strList, string OutFile, string user, string pwd)
        {
            Random rnd = new Random();
            string rand = rnd.Next(1, 10000000).ToString();
            System.IO.StreamWriter log = new System.IO.StreamWriter(OutFile + "LogDescription" + rand + ".txt", false, Encoding.UTF8);
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Site WD = new Site("https://www.wikidata.org", user, pwd);
            int cont = lines.Count();
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Split('\t');
                Dictionary<string, string> Description = new Dictionary<string, string>() { { tmp[1], tmp[2] } };
                try
                {
                    string ret = WD.EditEntity(tmp[0], null, null, Description, null, null, "BOT:Add/replace " + tmp[1] + " description");
                    if (ret != "")
                    {
                        if (ret.IndexOf("error code=\"badtoken\"") != -1) { WD.GetToken(); }
                        if (ret.IndexOf("\"code\": \"maxlag\",") != -1) { Thread.Sleep(5000); ; }
                        log.WriteLine(tmp[0] + "\t" + ret);
                    }
                    Mess.Text = cont.ToString();
                    Mess.Refresh();
                    cont--;
                }
                catch (Exception ex)
                {
                    log.WriteLine(tmp[0] + "\t" + ex);
                }  
            }
            log.Flush();
            log.Close();;
        }

        /// <summary>Write label like QuickStatements tool</summary>
        /// <param name="Mess">TextBox for error</param>
        /// <param name="strList">List of item (Qnumber, language, label Tab seprated, one item for row</param>
        /// <param name="OutFile">Path for logFile</param>
        /// <param name="user">User name for connection</param>
        /// <param name="pwd">Password for connection</param>
        public static void WriteLabelOnly(TextBox Mess, string strList, string OutFile, string user, string pwd)
        {
            Random rnd = new Random();
            string rand = rnd.Next(1, 10000000).ToString();
            System.IO.StreamWriter log = new System.IO.StreamWriter(OutFile + "LogDescription" + rand + ".txt", false, Encoding.UTF8);
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Site WD = new Site("https://www.wikidata.org", user, pwd);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Split('\t');
                Dictionary<string, string> Label = new Dictionary<string, string>() { { tmp[1], tmp[2] } };
                try
                {
                    string ret = WD.EditEntity(tmp[0], null, Label, null, null, null, "BOT:Add/Del/Replace " + tmp[1] + " label");
                    if (ret != "")
                    {
                        if (ret.IndexOf("error code=\"badtoken\"") != -1) { WD.GetToken(); }
                        if (ret.IndexOf("\"code\": \"maxlag\",") != -1) { Thread.Sleep(5000); ; }
                        log.WriteLine(tmp[0] + "\t" + ret);
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLine(tmp[0] + "\t" + ex);
                }
            }
            log.Flush();
            log.Close(); ;
        }

        public static string WriteLabelSameAsSitelink(Site WD, string strList, string LogFile)
        {
            List<string> list = Utility.SplitInChunk(strList, Environment.NewLine, 500);

            Random rnd = new Random();
            string rand = rnd.Next(1, 10000000).ToString();
            System.IO.StreamWriter log = new System.IO.StreamWriter(LogFile + "LogLabelSiteLink" + rnd.Next(1, 10000000).ToString() + ".txt", false, Encoding.UTF8);

            Entities EntityList = new Entities();
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string er = "OK" + Environment.NewLine ;
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                string[] tmp = lines[idx].Split('\t');
                string strJson = WD.LoadWD(tmp[0]);
                EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                Entity entity = EntityList.entities.Values.First();
                if (entity.sitelinks[tmp[1] + "wiki"].title == null)
                {
                    er += tmp[0] + " " + tmp[1] + Environment.NewLine;
                }
                else
                {
                    string label = entity.sitelinks[tmp[1] + "wiki"].title;
                    Dictionary<string, string> Label = new Dictionary<string, string>() { { tmp[1], label } };
                    try
                    {
                        string ret = WD.EditEntity(tmp[0], null, Label, null, null, null, "BOT:Add/Replace " + tmp[1] + " label");
                        if (ret != "")
                        {
                            if (ret.IndexOf("error code=\"badtoken\"") != -1) { WD.GetToken(); }
                            if (ret.IndexOf("\"code\": \"maxlag\",") != -1) { Thread.Sleep(5000); ; }
                            log.WriteLine(tmp[0] + "\t" + ret);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WriteLine(tmp[0] + "\t" + ex);
                    }
                }
                
            }
            log.Close();
            return er;
        }
    }
}
