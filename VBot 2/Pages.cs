using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace VBot
{
    public class Pages
    {
        public Query query { get; set; }
    }

    public class Query
    {
        public Dictionary<int, Page> pages { get; set; }
        public string FirstPageText = "";

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            foreach (KeyValuePair<int, Page> page in this.pages)
            {
                if (page.Value.missing == null)
                {
                    if (page.Value.revisions==null)
                    {
                        FirstPageText = "";
                    }
                    else
                    {
                        FirstPageText = page.Value.revisions[0].text;
                    }
                }
                break;
            }
        }
    }

    public class Page
    {
        public string pageid { get; set; } /// <summary>id of the page</summary>
        public string ns { get; set; } /// <summary>ns number of the page</summary>
        public string title { get; set; } /// <summary>Title of the page</summary>
        public List<Revision> revisions { get; set; } /// <summary>List of the revision, normally only the last one</summary>
        public Pageprops pageprops { get; set; }
        public string missing { get; set; }
        public string item { get; set; }
        public bool disambiguation { get; set; }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (this.pageprops!=null && this.pageprops.wikibase_item != null)
            {
                this.item = this.pageprops.wikibase_item; ;
            }
            else
            {
                this.item = "";
            }
            if (this.pageprops != null && this.pageprops.disambiguation != null)
            {
                this.disambiguation = true;
            }
            else
            {
                this.disambiguation = false;
            }
        }

        public bool IsRedirect()
        {
            string redirectTag = "REDIRECT|RINVIA";
            Regex regex = new Regex(@"(?i)^#(?:" + redirectTag + @")\s*:?\s*\[\[(.+?)(\|.+)?]]", RegexOptions.Compiled);
            //return this.revisions[0].text==null ? false : regex.IsMatch(this.revisions[0].text);
            return regex.IsMatch(this.revisions[0].text);
        }
        public string RedirectTo()
        {
            string redirectTag = "REDIRECT|RINVIA";
            Regex regex = new Regex(@"(?i)^#(?:" + redirectTag + @")\s*:?\s*\[\[(.+?)(\|.+)?]]", RegexOptions.Compiled);
            return regex.Match(this.revisions[0].text).Groups[1].ToString().Trim();
        }

        #region Templates

        /// <summary>Array of names of all templates in the page, duplicate are possible</summary>
        /// <returns>Array of string</returns>
        public string[] GetTemplatesName()
        {
            Regex regex = new Regex(
                @"(?s)\{\{(.+?)(}}|\|)",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.Compiled
                );
            MatchCollection templates = regex.Matches(revisions[0].text);

            string[] res=templates.Cast<Match>().Select(match => match.Value).ToArray();
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = res[i].Replace("{{", "").Replace("\r", "").Replace("\n", "");
                if (res[i].EndsWith("|")) { res[i] = res[i].Remove(res[i].Length - 1, 1); }
            }
            return res;
        }
        /// <summary>Array with all the complete templates in the page, only first level of templates, not template in template</summary>
        /// <returns>Array of string</returns>
        public string[] GetTemplatesFirst()
        {
            Regex regex = new Regex(
                "{{(?>[^\\{\\}]+|\\{(?<DEPTH>)|\\}(?<-DEPTH>))*(?(DEPTH)(?!))}}",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.Compiled
                );
            MatchCollection templates = regex.Matches(revisions[0].text);

            return templates.Cast<Match>().Select(match => match.Value).ToArray();
        }

        /// <summary>Check if a template exist in the page</summary>
        /// <param name="template">Name of the template to search</param>
        /// <returns>true or false</returns>
        public bool TemplateExist(string template)
        {
            string[] list = GetTemplatesName();

            for (int i = 0; i < list.Length; i++)
            {
                if (String.Equals(list[i], template, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        public string GetTemplateParameter(string template, string parameter)
        {
            string[] templates = GetTemplatesFirst();

            for (int i = 0; i < templates.Length; i++)
            {
                if (Regex.Match(templates[i], @"{{\s*" + template, RegexOptions.IgnoreCase).Success)
                {
                    string[] par = templates[i].Split('|');
                    for (int idx = 0; idx < par.Length; idx++)
                    {
                        if (idx == 0) { par[idx]=par[idx].Replace("{{", ""); }
                        if (idx==par.Length-1) { par[idx]=par[idx].Remove(par[idx].Length - 2, 2); }
                        par[idx] = par[idx].Replace("\n", "").Replace("\r","");
                        string [] tmp = par[idx].Split('=');
                        if (tmp.Count()==2)
                        {
                            if (String.Equals(tmp[0].Trim(), parameter, StringComparison.OrdinalIgnoreCase))
                            {
                                return tmp[1].Trim();
                            }
                        }
                    }
                }
            }
            return "";
        }
    }
    #endregion
    public class Revision
    {
        [JsonProperty("*")]
        public string text { get; set; }
    }

    public class Pageprops
    {
        public string wikibase_item { get; set; }
        public string disambiguation { get; set; }
    }
}
