using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace VBot
{
    class Wikipedia
    {
        public static void CheckForF_Dump(TextBox Mess, string DumpFile, string OutFile)
        {
            string line;
            string title = "";
            string ns = "";
            string text = "";
            string redirect_to = "";

            int conta = 0;
            long tot = 0;
            System.IO.StreamWriter log = new System.IO.StreamWriter(OutFile + "Candidate to F"+ ".txt", false, Encoding.UTF8);
            System.IO.StreamReader file = new System.IO.StreamReader(DumpFile, Encoding.UTF8);
            while ((line = file.ReadLine()) != null)
            {
                if (line.IndexOf("<title>") != -1) //title of the page
                {
                    title = line.Replace("<title>", "").Replace("</title>", "");
                    title = title.Substring(4);
                    redirect_to = "";
                    text = "";
                    tot += 1;
                }
                else if (line.IndexOf("<ns>") != -1 && line.IndexOf("</ns>") != -1) //ns of the page
                {
                    ns = line.Replace("<ns>", "").Replace("</ns>", "").Trim();
                }
                else if (line.IndexOf("<redirect title=") != -1) //is a redirect
                {
                    redirect_to = line.Replace("<redirect title=\"", "").Replace("\" />", "").Trim();
                }
                else if (line.IndexOf("<text xml:space=\"preserve\">") != -1 && ns == "0" && redirect_to == "") //Text of the page
                {
                    line = line.Replace("<text xml:space=\"preserve\">", "");
                    line = line.Substring(6);
                    text = line;

                    while ((line = file.ReadLine()).IndexOf("</text>") == -1)
                    {
                        text += Environment.NewLine + line;
                    }
                    if (line.Replace("</text>", "").Trim() != "")
                    {
                        text += Environment.NewLine + line.Replace("</text>", "").Trim();
                    }
                    text = System.Net.WebUtility.HtmlDecode(text); //wiki text

                    if (Regex.Match(text, @"{{\s*F\s*}}", RegexOptions.IgnoreCase).Success) { } //{{F}}
                    else if (Regex.Match(text, @"{{\s*F[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{F
                    else if (Regex.Match(text, @"{{\s*S\s*}}", RegexOptions.IgnoreCase).Success) { } //{{S}}
                    else if (Regex.Match(text, @"{{\s*S[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{S
                    else if (Regex.Match(text, @"{{\s*A\s*}}", RegexOptions.IgnoreCase).Success) { } //{{A}}
                    else if (Regex.Match(text, @"{{\s*A[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{A
                    else if (Regex.Match(text, @"{{\s*NN\s*}}", RegexOptions.IgnoreCase).Success) { } //{{NN}}
                    else if (Regex.Match(text, @"{{\s*NN[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{NN
                    else if (Regex.Match(text, @"{{\s*Disambigua\s*}}", RegexOptions.IgnoreCase).Success) { } //{{Disambigua}}
                    else if (Regex.Match(text, @"{{\s*Disambigua[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Disambigua
                    else if (Regex.Match(text, @"{{\s*Controllo di autorità\s*}}", RegexOptions.IgnoreCase).Success) { } //{{Controllo di autorità}}
                    else if (Regex.Match(text, @"{{\s*Torna a[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Torna a
                    else if (Regex.Match(text, @"{{\s*Cita\s*}}", RegexOptions.IgnoreCase).Success) { } //{{Cita}}
                    else if (Regex.Match(text, @"{{\s*Cita[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Cita
                    else if (Regex.Match(text, @"{{\s*Cita.+[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Cita

                    /// Esclusione pagine sulle date
                    else if (Regex.Match(text, @"{{\s*Decennio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Decennio
                    else if (Regex.Match(text, @"{{\s*Anno\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Anno
                    else if (Regex.Match(text, @"{{\s*Secolo\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Secolo
                    else if (Regex.Match(text, @"{{\s*Gennaio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Febbraio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Marzo\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Aprile\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Maggio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Giugno\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Luglio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Agosto\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Settembre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Ottobre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Novembre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                    else if (Regex.Match(text, @"{{\s*Dicembre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese

                    else if (Regex.Match(text, @"{{\s*Numero intero\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{numeri

                    else if (Regex.Match(text, @"===?=?\s*Note\s*===?=?", RegexOptions.IgnoreCase).Success) { } //{{S}}
                    else if (Regex.Match(text, @"===?=?\s*Bibliografia\s*===?=?", RegexOptions.IgnoreCase).Success) { } //{{S}}
                    else if (Regex.Match(text, @"===?=?\s*Collegamenti esterni\s*===?=?", RegexOptions.IgnoreCase).Success) { } //{{S}}

                    //else if (Utility.SectionStart(text, "Note") > 0) { }
                    //else if (Utility.SectionStart(text, "Bibliografia") > 0) { }
                    //else if (Utility.SectionStart(text, "Collegamenti esterni") > 0) { }
                    else if (text.IndexOf("<ref", StringComparison.CurrentCultureIgnoreCase) != -1) { }
                    else if (text.IndexOf("http://", StringComparison.CurrentCultureIgnoreCase) != -1) { }
                    else if (text.IndexOf("https://", StringComparison.CurrentCultureIgnoreCase) != -1) { }
                    else
                    {
                        log.WriteLine(title);
                        //Mess.AppendText("* [[" + title + "]]" + Environment.NewLine);
                        conta+=1;
                    }

                }
            }
            log.Close();
            Mess.AppendText("risultato: " + conta.ToString());
        }

        public static void WriteF(TextBox Mess, string strList, string DumpFile, string OutFile, string user, string password)
        {
            #region Tabella template agomento
            Dictionary<string, string> TemArg = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "[[Template:Album]]", "album discografici" },
                { "[[Template:Azienda]]", "aziende" },
                { "[[Template:Bio]]", "biografie" },
                { "[[Template:Brano musicale]]", "brani musicali" },
                { "[[Template:College]]", "università" },
                { "[[Template:Università]]", "università" },
                { "[[Template:Composto chimico]]", "sostanze chimiche" },
                { "[[Template:Corpo celeste]]", "astronomia" },
                { "[[Template:Asteroide]]", "astronomia" },
                { "[[Template:Discografia]]", "discografie" },
                { "[[Template:Divisione amministrativa]]", "geografia" },
                { "[[Template:Dramma]]", "teatro" },
                { "[[Template:Opera]]", "teatro" },
                { "[[Template:Spettacolo teatrale]]", "teatro" },
                { "[[Template:Teatro]]", "teatro" },
                { "[[Template:Edificio civile]]", "architettura" },
                { "[[Template:Edificio religioso]]", "architettura" },
                { "[[Template:Festival musicale]]", "festival musicali" },
                { "[[Template:Fiction TV]]", "fiction televisive" },
                { "[[Template:Film]]", "film" },
                { "[[Template:Formazione geologica]]", "geologia" },
                { "[[Template:Roccia]]", "geologia" },
                { "[[Template:Terremoto]]", "geologia" },
                { "[[Template:Episodio Anime]]", "anime e manga" },
                { "[[Template:Stagione anime]]", "anime e manga" },
                { "[[Template:Videogioco]]", "videogiochi" },
                { "[[Template:Infobox aeromobile]]", "aviazione" },
                { "[[Template:Infobox aeroporto]]", "aviazione" },
                { "[[Template:Auto]]", "automobili" },
                { "[[Template:Auto1]]", "automobili" },
                { "[[Template:Infobox linea ferroviaria]]", "ferrovie" },
                { "[[Template:Infobox stazione ferroviaria]]", "ferrovie" },
                { "[[Template:Infobox linea metropolitana]]", "metropolitane" },
                { "[[Template:Infobox stazione della metropolitana]]", "metropolitane" },
                { "[[Template:Infobox metropolitana]]", "metropolitane" },
                { "[[Template:Partito politico]]", "partiti politici" },
                { "[[Template:Infobox ponte]]", "ponti" },
                { "[[Template:Libro]]", "opere letterarie" },
                { "[[Template:Minerale]]", "mineralogia" },
                { "[[Template:Montagna]]", "montagna" },
                { "[[Template:Catena montuosa]]", "montagna" },
                { "[[Template:Valico]]", "montagna" },
                { "[[Template:Rifugio]]", "montagna" },
                { "[[Template:Museo]]", "musei" },
                { "[[Template:Opera d'arte]]", "arte" },
                { "[[Template:Prenome]]", "antroponimi" },
                { "[[Template:Sito archeologico]]", "siti archeologici" },
                { "[[Template:Software]]", "software" },
                { "[[Template:Tassobox]]", "biologia" },
                { "[[Template:Isola]]", "geografia" },
                { "[[Template:Infobox isola]]", "geografia" },
                { "[[Template:Competizione cestistica nazionale]]", "competizioni cestistiche" },
                { "[[Template:Edizione di competizione sportiva]]", "sport" }
            };

            #endregion

            #region Tabella categorie agomento
            Dictionary<string, string> CatArg = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "[[Categoria:Araldica]]", "araldica" },
                { "[[Categoria:Cucina]]", "cucina" },
                { "[[Categoria:Giappone]]", "Giappone" },
                { "[[Categoria:Mitologia]]", "mitologia" },
                { "[[Categoria:Scacchi]]", "scacchi" },
                { "[[Categoria:Vessillologia]]", "vessillologia" },
                { "[[Categoria:Prenome]]", "antroponimi" },
                { "[[Categoria:Personaggi cinematografici]]", "personaggi cinematografici" },
                { "[[Categoria:Ordini di grandezza]]", "metrologia" },
                { "[[Categoria:Ordini di grandezza \\(lunghezza\\)]]", "metrologia" },
                { "[[Categoria:Ordini di grandezza \\(temperatura\\)]]", "metrologia" }
            };
            #endregion

            #region Tabella portale agomento
            Dictionary<string, string> PorArg = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Astronomia", "astronomia" },
                { "Vessillologia", "vessillologia" },
                { "Religioni", "religione" },
                { "Aviazione", "aviazione" },
                { "Chimica", "chimica" },
                { "Geografia", "geografia" },
                { "Tennis", "tennis" },
                { "Matematica", "matematica" },
                { "Astronautica", "astronautica" },
                { "Mitologia", "mitologia" },
                { "Letteratura", "letteratura" },
                { "Oggetti del profondo cielo", "astronomia" },
                { "Sistema solare", "astronomia" },
                { "Psicologia", "psicologia" },
                { "Finlandia", "Finlandia" },
                { "Danimarca", "Danimarca" },
                { "Cinema", "cinema" },
                { "Musica", "musica" },
                { "Metrologia", "metrologia" },
                { "Politica", "politica" },
                { "Calcio", "calcio" },
                { "Polonia", "Polonia" },
                { "Nuoto", "nuoto" },
                { "Tolkien", "Terra di Mezzo" },
                { "Australia", "Australia" },
                { "Islanda", "Islanda" },
                { "Atletica leggera", "atletica leggera" },
                { "Automobilismo", "automobilismo" },
                { "Trasporti", "trasporti" },
                { "Anime e manga", "anime e manga" },
                { "Fisica", "fisica" }
            };
            #endregion

            Mess.AppendText("Iniziato alle " + DateTime.Now.ToString() + Environment.NewLine);
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            int cont = 0;
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);
            string strJson = "";
            Site WP = new Site("https://it.wikipedia.org", user, password);

            System.IO.StreamWriter log = new System.IO.StreamWriter(@"C:\Users\ValterVB\Documents\Visual Studio 2015\Projects\VBot\File\F da sistemare" + ".txt", false, Encoding.UTF8);

            string res = "";
            foreach (string s in list)
            {
                Pages pages = new Pages();
                strJson = WP.LoadWP(s);
                pages = JsonConvert.DeserializeObject<Pages>(strJson);
                foreach (Page p in pages.query.pages.Values)
                {
                    if (p.pageid!=null)
                    {
                        string text = p.revisions[0].text;
                        text = System.Net.WebUtility.HtmlDecode(text); //wiki text
                                                                       // Esclusione pagine con certi template
                        if (Regex.Match(text, @"{{\s*F\s*}}", RegexOptions.IgnoreCase).Success) { } //{{F}}
                        else if (Regex.Match(text, @"{{\s*F[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{F
                        else if (Regex.Match(text, @"{{\s*S\s*}}", RegexOptions.IgnoreCase).Success) { } //{{S}}
                        else if (Regex.Match(text, @"{{\s*S[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{S
                        else if (Regex.Match(text, @"{{\s*A\s*}}", RegexOptions.IgnoreCase).Success) { } //{{A}}
                        else if (Regex.Match(text, @"{{\s*A[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{A
                        else if (Regex.Match(text, @"{{\s*NN\s*}}", RegexOptions.IgnoreCase).Success) { } //{{NN}}
                        else if (Regex.Match(text, @"{{\s*NN[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{NN
                        else if (Regex.Match(text, @"{{\s*Disambigua\s*}}", RegexOptions.IgnoreCase).Success) { } //{{Disambigua}}
                        else if (Regex.Match(text, @"{{\s*Disambigua[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Disambigua
                        else if (Regex.Match(text, @"{{\s*Controllo di autorità\s*}}", RegexOptions.IgnoreCase).Success) { } //{{Controllo di autorità}}
                        else if (Regex.Match(text, @"{{\s*Torna a\s*[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Torna a
                        else if (Regex.Match(text, @"{{\s*Cita\s*}}", RegexOptions.IgnoreCase).Success) { } //{{Cita}}
                        else if (Regex.Match(text, @"{{\s*Cita[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Cita
                        else if (Regex.Match(text, @"{{\s*Cita.+[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success) { } //{{Cita

                        // Esclusione pagine sulle date
                        else if (Regex.Match(text, @"{{\s*Decennio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Decennio
                        else if (Regex.Match(text, @"{{\s*Anno\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Anno
                        else if (Regex.Match(text, @"{{\s*Secolo\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Secolo
                        else if (Regex.Match(text, @"{{\s*Millennio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Millennio
                        else if (Regex.Match(text, @"{{\s*Gennaio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Febbraio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Marzo\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Aprile\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Maggio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Giugno\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Luglio\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Agosto\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Settembre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Ottobre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Novembre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese
                        else if (Regex.Match(text, @"{{\s*Dicembre\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{mese

                        else if (Regex.Match(text, @"{{\s*Numero intero\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{numeri
                        else if (Regex.Match(text, @"{{\s*MSC\s*}}", RegexOptions.IgnoreCase).Success) { } //{{MSC}}

                        else if (Regex.Match(text, @"{{\s*Incipit lista cognomi\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Incipit lista cognomi
                        else if (Regex.Match(text, @"{{\s*Incipit lista nomi\s*[|\r\n]*", RegexOptions.IgnoreCase).Success) { } //{{Incipit lista nomi


                        else if (Regex.Match(text, @"===?=?\s*Note\s*===?=?", RegexOptions.IgnoreCase).Success) { } //{{S}}
                        else if (Regex.Match(text, @"===?=?\s*Bibliografia\s*===?=?", RegexOptions.IgnoreCase).Success) { } //{{S}}
                        else if (Regex.Match(text, @"===?=?\s*Collegamenti esterni\s*===?=?", RegexOptions.IgnoreCase).Success) { } //{{S}}

                        else if (text.IndexOf("<ref", StringComparison.CurrentCultureIgnoreCase) != -1) { }
                        else if (text.IndexOf("http://", StringComparison.CurrentCultureIgnoreCase) != -1) { }
                        else if (text.IndexOf("https://", StringComparison.CurrentCultureIgnoreCase) != -1) { }
                        else
                        {
                            // Controllo i template
                            string F = "{{F|";
                            foreach (KeyValuePair<string, string> templ in TemArg)
                            {
                                string t = templ.Key.Replace("[[Template:", "").Replace("]]", "");
                                if (Regex.Match(text, @"{{\s*" + t + @"[\r\n]?[\r\n]?\|", RegexOptions.IgnoreCase).Success)
                                {
                                    F += templ.Value + "|" + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year + "}}";
                                    break;
                                }
                            }
                            //Controllo le categorie
                            if (F.IndexOf("}") == -1)
                            {
                                foreach (KeyValuePair<string, string> cat in CatArg)
                                {
                                    string c = cat.Key.Replace("[", "").Replace("]", "");
                                    if (Regex.Match(text, @"\[\[" + c + @"\]\]", RegexOptions.IgnoreCase).Success || Regex.Match(text, @"\[\[" + c + @"\s*\|", RegexOptions.IgnoreCase).Success)
                                    {
                                        F += cat.Value + "|" + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year + "}}";
                                        break;
                                    }
                                }
                            }
                            //Controllo i portali
                            if (F.IndexOf("}") == -1)
                            {
                                Regex regex = new Regex("({{portale)(\\|.*)+(}})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                Match ms = regex.Match(text);
                                string tempValues = ms.Groups[2].Value;
                                foreach (KeyValuePair<string, string> por in PorArg)
                                {
                                    if (tempValues.IndexOf(por.Key, StringComparison.CurrentCultureIgnoreCase) != -1)
                                    {
                                        F += por.Value + "|" + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year + "}}";
                                        break;
                                    }
                                }
                            }

                            if (F.IndexOf("}") == -1) // no arg
                            {
                                F += "" + "|" + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year + "}}";
                                log.WriteLine(p.title + '\t' + "No argomenti");
                            }
                            else
                            {
                                Regex regex = new Regex("(\\{\\{(?:[nN]d[ |]|[nN]ota disambigua|[tT]orna a)[^}]*}}|__[^_]+__)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
                                bool IsMatch = regex.IsMatch(text);

                                if (IsMatch)
                                {
                                    log.WriteLine(p.title + '\t' + "Template in testa");
                                    //res += "*[[" + p.title + "]] \t <nowiki>Template da sistemare</nowiki>" + Environment.NewLine;
                                }
                                else
                                {
                                    text = F + Environment.NewLine + p.revisions[0].text;
                                    WP.SavePage(p.title, text, "BOT: Add template " + F);
                                    res += "*[[" + p.title + "]] \t <nowiki>" + F + "</nowiki>" + Environment.NewLine;
                                    cont++;
                                }
                            }
                        }
                    }
                }
            }
            log.Close();
            Mess.AppendText(Environment.NewLine );
            Mess.AppendText(res);
            Mess.AppendText(Environment.NewLine);
            Mess.AppendText("Finito alle " + DateTime.Now.ToString() + Environment.NewLine);
        }

        /// <summary>Do a null edit with API purge</summary>
        /// <param name="site">full url of the site</param>
        /// <param name="strList">list of pages to be processed separated by newline</param>
        /// <param name="user">user for the connection</param>
        /// <param name="password">password for the connection</param>
        /// <returns>list of pages with some problem separated by newline</returns>
        public static string Purge(string site, string strList, string user, string password)
        {
            string res = "";
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx] + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 500);
            Site WP = new Site(site, user, password);
            foreach (string s in list)
            {
                res+=WP.Purge(s) + Environment.NewLine ;
            }

            return res;
        }

        public static string ReadParTemplate(string strList, string user, string password)
        {
            Site WP = new Site("https://it.wikipedia.org", user, password);
            string[] lines = strList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string tmpList = "";
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                tmpList += lines[idx].Trim() + "|";
            }
            tmpList = tmpList.Remove(tmpList.LastIndexOf("|"));
            List<string> list = Utility.SplitInChunk(tmpList, "|", 1);
            string strJson = "";
            Dictionary<string, string> res = new Dictionary<string, string>();
            string result = "";
            foreach (string s in list)
            {
                Pages pages = new Pages();
                strJson = WP.LoadWP(s);
                pages = JsonConvert.DeserializeObject<Pages>(strJson);
                foreach (Page p in pages.query.pages.Values)
                {
                    string testo = p.revisions[0].text;
                    string Altezza=Utility.GetTemplateParameter(testo, "Sportivo", "Altezza");
                    string Peso = Utility.GetTemplateParameter(testo, "Sportivo", "Peso");
                    result += p.title + '\t' + Altezza + '\t' + Peso;
                }
            }
            return result;
        }

    }
}
