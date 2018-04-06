using System;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace VBot
{
    class ReportsWD
    {
        public static string CreateTableForConflict(string strList, TextBox Mess, string user, string password)
        {
            Site WD = new Site("https://www.wikidata.org", user, password);
            string[] lines = strList.Replace("\r", "").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
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
                string sl1 = "";
                string sl2 = "";

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
                if (entity1.sitelinks.ContainsKey(riga[3] + "wiki"))
                {
                    sl1 = entity1.sitelinks[riga[3] + "wiki"].title;
                }
                else
                {
                    sl1= "no sitelink";
                }

                if (entity2.labels.ContainsKey(riga[3]))
                {
                    lab2 = entity2.labels[riga[3]].value;
                }
                else
                {
                    lab2 = "no label";
                }
                if (entity2.sitelinks.ContainsKey(riga[3] + "wiki"))
                {
                    sl2 = entity2.sitelinks[riga[3] + "wiki"].title;
                }
                else
                {
                    sl2 = "no sitelink";
                }
                result += "|" + riga[0] + "||[[" + entity1.id + "|" + lab1 + " (" + entity1.id + ")]] - " + sl1 + "||" + p1 + "||[[" + entity2.id + "|" + lab2 + " (" + entity2.id + ")]] - " + sl2 + "||" + p2 + "||" + riga[3] + Environment.NewLine;
                result += "|-" + Environment.NewLine;
            }
            result += "|}";
            return result;
        }
    }
}
