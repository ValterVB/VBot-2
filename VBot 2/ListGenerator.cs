using System;
using System.Net;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Xml;

namespace VBot
{
    class ListGenerator
    {
        private static string Version = "VBot ver." + Assembly.GetExecutingAssembly().GetName().Version.ToString()+ " - https://www.wikidata.org/wiki/User:ValterVB";

        /// <summary>Search the string and return all the matching pages. Work also on Wikidata.</summary>
        /// <param name="site">Site object</param>
        /// <param name="toSearch">String to search is possible use also inline, intitle etc.</param>
        /// <param name="ns">Namespace number separated by pipe. default is all (example: 0|4|2)</param>
        /// <param name="max">Max number of line to retrieve, default is 5000</param>
        /// <returns>Number of elements and list of pages/items founded separated by pipe</returns>
        public static (int count, string list) ListFromSearch(Site site, string toSearch, string ns = "", int max = 5000)
        {
            string _ret = "";
            int _count = 0;
            string sroffset = "";
            while (true) //Infinite loop
            {
                string PostData = "format=xml&list=search&srsearch=" + WebUtility.UrlEncode(toSearch) + "&srlimit=" + max.ToString() + "&sroffset=" + sroffset + "&srnamespace=" + ns;
                string respStr = site.PostRequest(site.URL() + site.API() + "?action=query", PostData);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(respStr);
                foreach (XmlNode node in doc.SelectNodes("/api/query/search/p[@title]"))
                {
                    _ret += node.Attributes["title"].Value + "|";
                    _count++;
                }
                if (doc.SelectSingleNode("/api/continue") == null) { break; }
                sroffset = doc.SelectSingleNode("/api/continue").Attributes["sroffset"].Value;
            }
            if (_ret != "") { _ret = _ret.Remove(_ret.LastIndexOf("|")); }
            return (_count,_ret);
        }

        /// <summary>List of pages in a category.</summary>
        /// <param name="site">Site object</param>
        /// <param name="category"></param>
        /// <param name="ns">Namespace number separated by pipe. default is all (example: 0|4|2)</param>
        /// <param name="max">Max number of line to retrieve, default is 5000 for bot</param>
        /// <returns>Number of elements and list of pages founded separated by pipe</returns>
        public static (int count, string list) ListFromCategory(Site site, string category, string ns = "", int max = 5000)
        {
            string _ret = "";
            int _count = 0;
            string cmContinue = "";
            while (true) //Infinite loop
            {
                string PostData = "&list=categorymembers&format=xml&cmlimit=" + max.ToString() + "&cmtitle=Category:" + WebUtility.UrlEncode(category) + "&cmnamespace=" + ns + "&cmcontinue=" + cmContinue;
                string respStr = site.PostRequest(site.URL() + site.API() + "?action=query", PostData);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(respStr);
                foreach (XmlNode node in doc.SelectNodes("/api/query/categorymembers/cm"))
                {
                    _ret += node.Attributes["title"].Value + "|";
                    _count++;
                }
                if (doc.SelectSingleNode("/api/continue") == null) { break; }
                cmContinue = doc.SelectSingleNode("/api/continue").Attributes["cmcontinue"].Value;
            }
            if (_ret != "") { _ret = _ret.Remove(_ret.LastIndexOf("|")); };
            return (_count, _ret);
        }

        /// <summary>List of items returned from a SPARQL query</summary>
        /// <param name="sparql">Query SPARQL</param>
        /// <param name="nameOfColoumn">Name of the column with item number returned by query</param>
        /// <returns>Number of elements and list of items founded separated by pipe</returns>
        public static (int count, string list) WQS(string sparql, string nameOfColoumn)
        {
            string _ret = "";
            int _count = 0;
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", Version);
            Stream data = client.OpenRead("https://query.wikidata.org/sparql?query=" + WebUtility.UrlEncode(sparql));
            StreamReader reader = new StreamReader(data);
            string respStr = reader.ReadToEnd();
            data.Close();
            reader.Close();

            respStr = respStr.Replace(" xmlns='http://www.w3.org/2005/sparql-results#'", ""); //Workaround to fix problem with namespace in NET
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(respStr);

            foreach (XmlNode node in doc.SelectNodes("/sparql/results/result/binding[@name='" + nameOfColoumn + "']"))
            {
                _ret += node.SelectSingleNode("uri").InnerText.Replace("http://www.wikidata.org/entity/", "") + "|";
                _count++;
            }
            if (_ret != "") { _ret = _ret.Remove(_ret.LastIndexOf("|")); };
            return (_count, _ret);
        }

        /// <summary>List of items or page from Whats link here</summary>
        /// <param name="site">Site object</param>
        /// <param name="element">Page or item to check with Whats link here</param>
        /// <param name="ns">Namespace number separated by pipe. default is 0 (example: 0|4|2)</param>
        /// <param name="Max">Max number of line to retrieve, default is 5000</param>
        /// <returns>Number of elements and list of pages founded separated by pipe</returns>
        public static (int count, string list) WhatsLinkHere(Site site, string element, string ns = "*", int Max = 5000)
        {
            string _ret = "";
            int _count = 0;
            string[] lines = element.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            string respStr = site.Linkshere(element);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(respStr);
            foreach (XmlNode node in doc.SelectNodes("/api/query/pages/page[@title]"))
            {
                string tmp = "";
                foreach (XmlNode Linknode in node.SelectNodes("linkshere/lh[@title]"))
                {
                    tmp += Linknode.Attributes["title"].Value + "|";
                    _count++;
                }
                if (tmp!="")
                {
                    tmp = tmp.Remove(tmp.LastIndexOf("|"));
                }
                _ret+=node.Attributes["title"].Value + '\t' + tmp + Environment.NewLine;
            }
            return (_count, _ret);
        }

        public static string AreDisambiguation(Site site, string list)
        {
            string _ret = "";
            string PostData = "format=xml&prop=pageprops&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(list);
            string respStr = site.PostRequest(site.URL() + site.API() + "?action=query", PostData);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(respStr);
            foreach (XmlNode node in doc.SelectNodes("/api/query/pages/page"))
            {
                if (node.SelectSingleNode("pageprops")!=null)
                {
                    _ret += node.Attributes["title"].Value + "|";
                }
            }
            return _ret;
        }

        public static string RecentChanges(string From, string To, Site WP, int Max = 5000)
        {
            string ret = "";
            string rccontinue = "";
            string PostData = "";
            while (true) //Infinite loop
            {
                if (rccontinue=="")
                {
                    PostData = "format=xml&list=recentchanges&rcstart=" + To + "&rcend=" + From + "&rctoponly=1&rcnamespace=0&rcprop=title|user&rclimit=" + Max;
                }
                else
                {
                    PostData = "format=xml&list=recentchanges&rcstart=" + To + "&rcend=" + From + "&rctoponly=1&rcnamespace=0&rcprop=title|user&rclimit=" + Max + "&rccontinue=" + rccontinue;
                }
                string respStr = WP.PostRequest(WP.URL() + WP.API() + "?action=query", PostData);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(respStr);
                foreach (XmlNode node in doc.SelectNodes("/api/query/recentchanges/rc"))
                {
                    //string user = node.Attributes["user"].Value;
                    ret += node.Attributes["title"].Value + "|";
                }
                if (doc.SelectSingleNode("/api/continue") == null) { break; }
                rccontinue = doc.SelectSingleNode("/api/continue").Attributes["rccontinue"].Value;
            }
            if (ret != "") { ret = ret.Remove(ret.LastIndexOf("|")); }
            return ret;
        }


        public static string PetScan(string query)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", Version);
            //Stream data = client.OpenRead("https://petscan.wmflabs.org/" + WebUtility.UrlEncode(query+"&doit="));
            Stream data = client.OpenRead("https://petscan.wmflabs.org/" + query + "&doit=&format=tsv");
            StreamReader reader = new StreamReader(data);
            string respStr = reader.ReadToEnd();
            data.Close();
            reader.Close();

            string ret = "";

            string[] lines = respStr.Split('\n');
            
            for (int idx = 1; idx < lines.Count(); idx++)
            {
                if (lines[idx]=="") { break; }

                string[] tmp = lines[idx].Split('\t');
                ret += tmp[6]+"|";
            }
            if (ret != "") { ret = ret.Remove(ret.LastIndexOf("|")); };
            return ret;
        }
        
		public static string[] TranscludeWP(string Title, Site WP, string Continue = "", int Max = 5000)
		{
			string PostData = WP.URL() + WP.API() + "?action=query&prop=transcludedin&format=xml&tinamespace=0&tilimit=" + Max.ToString() + "&ticontinue=" + Continue + "&titles=" + Title;
			string respStr = WP.PostRequest(PostData, "");

			// Extract of continue number
			int da = -1; int a = -1;
			da = respStr.IndexOf("<continue ticontinue=") + 22;
			if (da != 22)
			{
				a = respStr.IndexOf("\"", da);
				Continue = respStr.Substring(da, a - da);
			}

			// if Conttinue = <api batchcomplete= then is last page
			//Extract of Qnumber
			string ret = "";
			if (respStr.IndexOf("<transcludedin>") == -1)
			{
				return new string[] { Continue, "" };
			}
			else
			{
				da = respStr.IndexOf("<transcludedin>") + 15;
				a = respStr.IndexOf("</transcludedin>", da);
				respStr = respStr.Substring(da, a - da);
				string[] list = respStr.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string item in list)
				{
					int del1= item.IndexOf(" title=\"")+8;
					int del2 = item.IndexOf("\" /",del1);
					ret += WebUtility.HtmlDecode(item.Substring(del1, del2 - del1)) + "|";
				}
				ret = ret.Remove(ret.LastIndexOf("|"));
				return new string[] { Continue, ret };
			}
		}

    }
}
