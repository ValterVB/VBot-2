using System;
using System.Text;
using System.Reflection;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Xml;


namespace VBot
{
    class Site
    {
        #region Variables

        /// <summary>Used to select which data return</summary>
        public enum LoadTypeWD
        {
            All,
            Label,
            Description,
            LabelDescriptionSitelink,
            LabelSitelink
        };

        private string _url; // URL of repository
        private CookieContainer _cookies = new CookieContainer();  // Cookie from the site
        private string _user; // User name for connection
        private string _password; // Password for the connection
        private string _api = "/w/api.php"; // API root
        private string _version = "VBot ver." + Assembly.GetExecutingAssembly().GetName().Version.ToString(); // Version of the BOT
        private string _editSessionToken = ""; // Token to do edit
        private string _maxlag; // Value of the maxlag to be use, default is 5

        public string URL() { return _url; }
        public string API() { return _api; }
        public string User { get { return _user; } }
        public string Maxlag { get { return _maxlag; } }
        #endregion

        #region Constructor
        /// <summary>Empty site</summary>
        public Site()
        {
            _url = "";
            _user = "";
            _password = "";
            _maxlag = "";
        }

        /// <summary>Connection to the site</summary>
        /// <param name="url">url of the Mediawiki site. Ex. https://www.wikidata.org</param>
        /// <param name="user">User name for the site</param>
        /// <param name="password">Password for the site</param>
        public Site(string url, string user, string password, string maxlag = "5")
        {
            _url = url;
            _user = user;
            _password = password;
            _maxlag = maxlag;
            Login();
        }

        /// <summary>Login to Mediawiki site and obtain an edit token</summary>
        /// <remarks>See also "https://www.mediawiki.org/wiki/API:Login" and "https://www.mediawiki.org/wiki/API:Tokens"</remarks>
        private void Login()
        {
            string resp = PostRequest(_url + _api + "?action=query","meta=tokens&type=login&format=xml", true);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);
            XmlNode node = xmlDoc.SelectSingleNode("/api/query/tokens");
            string token = node.Attributes["logintoken"].Value;

            resp = PostRequest(_url + _api + "?action=login", "lgname=" + WebUtility.UrlEncode(_user) + "&lgpassword=" + WebUtility.UrlEncode(_password) + "&format=xml" + "&lgtoken=" + WebUtility.UrlEncode(token), true);
            if (resp.IndexOf("login result=\"Success\"") == -1)
            {
                _user = "";
                _password = "";
                _editSessionToken = "";
            }
            else
            {
                GetToken();
            }
        }

        /// <summary>Obtain an edit token</summary>
        public void GetToken()
        {
            string resp = PostRequest(_url + _api+ "?action=query","meta=tokens&format=xml");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(resp);
            XmlNode node = xmlDoc.SelectSingleNode("/api/query/tokens");
            string token = node.Attributes["csrftoken"].Value;
            _editSessionToken = token;
        }
        #endregion

        #region Wikidata
        /// <summary>Return the JSON string of the entities using title + wiki</summary>
        /// <param name="langSite">Mediawiki site (ex. itwiki)</param>
        /// <param name="pages">List of pages separated by | (ex. Page1|Page2)</param>
        /// <param name="type">What retrieve, ex. LoadTypeWD.Label. Default is LoadTypeWD.All</param>
        /// <returns>JSON string of the items</returns>
        /// <remarks>See also https://www.mediawiki.org/wiki/Wikibase/API/it#wbgetentities </remarks>
        public string LoadWD(string langSite, string pages, LoadTypeWD type = LoadTypeWD.All)
        {
            string post = "";
            switch (type)
            {
                case LoadTypeWD.All:
                    post = string.Format("action=wbgetentities&format=json&redirects=yes&sites={0}&titles={1}", WebUtility.UrlEncode(langSite), WebUtility.UrlEncode(pages));
                    break;
                case LoadTypeWD.Label:
                    post = string.Format("action=wbgetentities&format=json&redirects=yes&sites={0}&titles={1}&props=labels", WebUtility.UrlEncode(langSite), WebUtility.UrlEncode(pages));
                    break;
                case LoadTypeWD.Description:
                    post = string.Format("action=wbgetentities&format=json&redirects=yes&sites={0}&titles={1}&props=descriptions", WebUtility.UrlEncode(langSite), WebUtility.UrlEncode(pages));
                    break;
                case LoadTypeWD.LabelDescriptionSitelink:
                    post = string.Format("action=wbgetentities&format=json&redirects=yes&sites={0}&titles={1}&props=labels|descriptions|sitelinks", WebUtility.UrlEncode(langSite), WebUtility.UrlEncode(pages));
                    break;
                default:
                    break;
            }
            return PostRequest(_url + _api, post);
        }

        /// <summary>Return the JSON string of the entities using Qnumber</summary>
        /// <param name="entities">List of entities separated by | (ex. Q1|Q2)</param>
        /// <param name="type">What retrieve, default is LoadTypeWD.All (ex. LoadTypeWD.Label)</param>
        /// <returns>JSON string of the items</returns>
        /// /// <remarks>See also https://www.mediawiki.org/wiki/Wikibase/API/it#wbgetentities </remarks>
        public string LoadWD(string entities, LoadTypeWD type = LoadTypeWD.All)
        {
            string post = "";
            switch (type)
            {
                case LoadTypeWD.All:
                    post = string.Format("format=json&redirects=yes&ids={0}", entities);
                    break;
                case LoadTypeWD.Label:
                    post = string.Format("format=json&redirects=yes&ids={0}&props=labels", entities);
                    break;
                case LoadTypeWD.Description:
                    post = string.Format("format=json&redirects=yes&ids={0}&props=descriptions", entities);
                    break;
                case LoadTypeWD.LabelDescriptionSitelink:
                    post = string.Format("format=json&redirects=yes&ids={0}&props=labels|descriptions|sitelinks", entities);
                    break;
                case LoadTypeWD.LabelSitelink:
                    post = string.Format("format=json&redirects=yes&ids={0}&props=labels|sitelinks", entities);
                    break;
                default:
                    break;
            }
            return PostRequest(_url + _api + "?action=wbgetentities", post);
        }

        public string SetQualifier(string claim, string property)
        {
            string request = "";
            request = string.Format("action=wbsetqualifier&format=json&claim={0}&property={1}&snaktype=novalue&token={2}", WebUtility.UrlEncode(claim), WebUtility.UrlEncode(property), WebUtility.UrlEncode(_editSessionToken));
            return PostRequest(_url + _api, request);
        }

        public string SetQualifier(string claim, string property, string value)
        {
            string request = "";
            request = string.Format("action=wbsetqualifier&format=json&claim={0}&property={1}&snaktype=value&value={2}&token={3}", WebUtility.UrlEncode(claim), WebUtility.UrlEncode(property), WebUtility.UrlEncode(value), WebUtility.UrlEncode(_editSessionToken));
            return PostRequest(_url + _api, request);
        }

        public string RemoveQualifier(string claim, string QualifierHash)
        {
            string request = "";
            request = string.Format("action=wbremovequalifiers&format=json&claim={0}&qualifiers={1}&token={2}", WebUtility.UrlEncode(claim), WebUtility.UrlEncode(QualifierHash), WebUtility.UrlEncode(_editSessionToken));
            return PostRequest(_url + _api, request);
        }

        public string CreateItem()
        {
            string request = "";
            request = "action=wbeditentity&format=json&new=item&data={}&token=" + WebUtility.UrlEncode(_editSessionToken);
            return PostRequest(_url + _api, request);
        }
        #endregion

        #region Wikipedia
        /// <summary>Load wikimedia pages</summary>
        /// <param name="Pages">Title of the page, multiple pages must separated by "|" (ex. Page1|Page2)</param>
        /// <returns>JSON string of the pages</returns>
        /// <remarks>See also "https://www.mediawiki.org/wiki/API:Query"</remarks>
        public string LoadWP(string pages)
        {
            string request = "";
            request = "action=query&prop=pageprops|revisions&format=json&rvprop=content&titles=" + WebUtility.UrlEncode(pages);
            return PostRequest(_url + _api, request);
        }

        /// <summary>Load pages without text page</summary>
        /// <param name="Pages">Title of the page, multiple pages must separated by | (ex. Page1|Page2)</param>
        /// <returns>JSON string of the pages</returns>
        /// <remarks>See also "https://www.mediawiki.org/wiki/API:Query"</remarks>
        public string LoadWPnoText(string pages)
        {
            string request = "";
            request = "action=query&prop=pageprops&format=json&titles=" + WebUtility.UrlEncode(pages);
            return PostRequest(_url + _api, request);
        }

        /// <summary>Delete an item or a page, need an admin user</summary>
        /// <param name="page">Single page to delete (ex. Q1234 or Page1)</param>
        /// <param name="reason">Reason for the deletion</param>
        /// <returns>API response</returns>
        public string DeletePage(string page, string reason)
        {
            string request = "";
            request = string.Format("action=delete&format=json&title={0}&reason={1}&token={2}&maxlag={3}&assert=user", WebUtility.UrlEncode(page), reason, WebUtility.UrlEncode(_editSessionToken), _maxlag);
            return PostRequest(_url + _api, request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public string Purge(string titles)
        {
            string request = "action=purge&format=json&forcelinkupdate=1&titles=" + WebUtility.UrlEncode(titles);
            string result = PostRequest(_url + _api, request);
            return result;
        }

        public string Linkshere(string titles)
        {
            ///w/api.php?action=query&format=json&prop=linkshere&list=&titles=File%3AStub%20algerini.png
            string request = "action=query&format=xml&prop=linkshere&lhlimit=max&titles=" + WebUtility.UrlEncode(titles);
            string result = PostRequest(_url + _api, request);
            return result;
        }

        /// <summary>Move a page grom one title to another</summary>
        /// <param name="from">Title from</param>
        /// <param name="to">Title to</param>
        /// <param name="reason">It's the edit object</param>
        /// <param name="movetalk">If true move also the talk</param>
        /// <param name="noredirect">If true don't create redirect</param>
        /// <returns>API result</returns>
        public string Move(string from, string to, string reason, Boolean movetalk, Boolean noredirect)
        {
            string request = "";
            request = string.Format("action=move&format=xml&from={0}&to={1}&reason={2}&token={3}", WebUtility.UrlEncode(from), WebUtility.UrlEncode(to), WebUtility.UrlEncode(reason), WebUtility.UrlEncode(_editSessionToken));
            if (movetalk) { request += "&movetalk"; }
            if (noredirect) { request += "&noredirect"; }
            string result = PostRequest(_url + _api, request);
            return result;
        }

        public string Info(string titles)
        {
            // / w / api.php ? action = query & format = xml & prop = info & titles =% 22I % 20Quit % 22 % 20match % 7C % 22I % 20quit % 22 % 20match
            string request = "action=query&format=xml&prop=info&titles=" + WebUtility.UrlEncode(titles);
            string result = PostRequest(_url + _api, request);
            return result;
        }


        /// <summary>Undelete a a page on Wikipedia</summary>
        /// <param name="title">Title of the page to be undelete</param>
        /// <param name="reason">It's the edit object</param>
        /// <returns>API result</returns>
        public string Undelete(string title, string reason)
        {
            string request = "";
            request = string.Format("action=undelete&format=xml&title={0}&reason={1}&token={2}", WebUtility.UrlEncode(title), WebUtility.UrlEncode(reason), WebUtility.UrlEncode(_editSessionToken));
            string result = PostRequest(_url + _api, request);
            return result;
        }

        public string LintError(string lntcategories,string limit)
        {
            string request = "action=query&format=xml&list=linterrors&lntcategories=" + lntcategories + "&lntlimit=" + limit;
            string result = PostRequest(_url + _api, request);
            return result;
        }

        public string LintError(string lntcategories,string ns, string limit)
        {
            string request = "action=query&format=xml&list=linterrors&lntcategories=" + lntcategories + "&lntlimit="+limit + "&lntnamespace=" + ns;
            string result = PostRequest(_url + _api, request);
            return result;
        }
        #endregion

        #region Edit entity
        /// <summary>Edit/create item</summary>
        /// <param name="id">Q number, empty string if you create a new item</param>
        /// <param name="links">Dictionary of sitelink</param>
        /// <param name="labels">Dictionary of label</param>
        /// <param name="descriptions">Dictionary of desription</param>
        /// <param name="aliases">Dictionary of list of alias</param>
        /// <param name="claims">List of claim object</param>
        /// <param name="New">true to create a new item</param>
        /// <param name="summary">Comment of the edit, empty string to auto descriptio</param>
        /// <returns>entity id for new entity, empty string for edit OK, result of post reques for errors</returns>
        public string EditEntity(string id, Dictionary<string, string> links, Dictionary<string, string> labels, Dictionary<string, string> descriptions, Dictionary<string, List<string>> aliases, List<Claim> claims, string summary)
        {
            string data = "{";
            if (links != null && links.Count > 0)
            {
                data += GetJsonLinks(links);
            }
            if (labels != null && labels.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonLabels(labels);
                }
                else
                {
                    data += ", " + GetJsonLabels(labels);
                }
            }
            if (descriptions != null && descriptions.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonDescriptions(descriptions);
                }
                else
                {
                    data += ", " + GetJsonDescriptions(descriptions);
                }
            }
            if (aliases != null && aliases.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonAliases(aliases);
                }
                else
                {
                    data += ", " + GetJsonAliases(aliases);
                }
            }
            if (claims != null && claims.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonClaims(claims);
                }
                else
                {
                    data += ", " + GetJsonClaims(claims);
                }
            }
            data += "}";
            data = data.Replace("\n", " ");
            string respStr;

            if (id == null | id == "")
            {
                try
                {
                    string postData = string.Format("token={0}&bot=bot&data={1}&summary={2}&new=item&maxlag=5&assert=bot", WebUtility.UrlEncode(_editSessionToken), WebUtility.UrlEncode(data), WebUtility.UrlEncode(summary));
                    respStr = PostRequest(_url + _api + "?action=wbeditentity&format=xml", postData);
                }
                catch (Exception ex)
                {
                    respStr = ex.Message;
                    return respStr;
                }
                if (respStr.IndexOf("<api success=\"1\">") == -1)
                {
                    return respStr;
                }
                //Extract new entity id
                int from = respStr.IndexOf("<entity id=\"") + 12;
                int to = respStr.IndexOf("\"", from);
                string tmpQ = respStr.Substring(from, to - from);
                return tmpQ;
            }
            else
            {
                try
                {
                    string postData = string.Format("id={0}&token={1}&bot=bot&data={2}&summary={3}&maxlag=5&assert=bot", id, WebUtility.UrlEncode(_editSessionToken), WebUtility.UrlEncode(data), WebUtility.UrlEncode(summary));
                    //string postData = string.Format("id={0}&token={1}&bot=bot&data={2}&summary={3}&assert=bot", id, WebUtility.UrlEncode(editSessionToken), WebUtility.UrlEncode(data), WebUtility.UrlEncode(summary));
                    respStr = PostRequest(_url + _api + "?action=wbeditentity&format=xml", postData);
                }
                catch (Exception ex)
                {
                    respStr = ex.Message;
                    return respStr;
                }
                if (respStr.IndexOf("<api success=\"1\">") == -1)
                {
                    return respStr;
                }
            }
            return "";
        }

        /// <summary>Clear and Edit item</summary>
        /// <param name="id">Q number, empty string if you create a new item</param>
        /// <param name="links">Dictionary of sitelink</param>
        /// <param name="labels">Dictionary of label</param>
        /// <param name="descriptions">Dictionary of desription</param>
        /// <param name="aliases">Dictionary of list of alias</param>
        /// <param name="claims">List of claim object</param>
        /// <param name="New">true to create a new item</param>
        /// <param name="summary">Comment of the edit, empty string to auto descriptio</param>
        /// <returns>entity id for new entity, empty string for edit OK, result of post reques for errors</returns>
        public string ClearEditEntity(string id, Dictionary<string, string> links, Dictionary<string, string> labels, Dictionary<string, string> descriptions, Dictionary<string, List<string>> aliases, List<Claim> claims, string summary)
        {
            string data = "{";
            if (links != null && links.Count > 0)
            {
                data += GetJsonLinks(links);
            }
            if (labels != null && labels.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonLabels(labels);
                }
                else
                {
                    data += ", " + GetJsonLabels(labels);
                }
            }
            if (descriptions != null && descriptions.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonDescriptions(descriptions);
                }
                else
                {
                    data += ", " + GetJsonDescriptions(descriptions);
                }
            }
            if (aliases != null && aliases.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonAliases(aliases);
                }
                else
                {
                    data += ", " + GetJsonAliases(aliases);
                }
            }
            if (claims != null && claims.Count > 0)
            {
                if (data == "{")
                {
                    data += GetJsonClaims(claims);
                }
                else
                {
                    data += ", " + GetJsonClaims(claims);
                }
            }
            data += "}";
            data = data.Replace("\n", " ");
            string respStr;

            if (id == null | id == "")
            {
                string postData = string.Format("token={0}&bot=bot&data={1}&summary={2}&new=item&maxlag=5&assert=bot", WebUtility.UrlEncode(_editSessionToken), WebUtility.UrlEncode(data), WebUtility.UrlEncode(summary));
                respStr = PostRequest(_url + _api + "?action=wbeditentity&format=xml&clear=true", postData);
                if (respStr.IndexOf("<api success=\"1\">") == -1)
                {
                    return respStr;
                }
                int from = respStr.IndexOf("<entity id=\"") + 12;
                int to = respStr.IndexOf("\"", from);
                string tmpQ = respStr.Substring(from, to - from);
                return tmpQ;
            }
            else
            {
                string postData = string.Format("id={0}&token={1}&bot=bot&data={2}&summary={3}&maxlag=5&assert=bot", id, WebUtility.UrlEncode(_editSessionToken), WebUtility.UrlEncode(data), WebUtility.UrlEncode(summary));
                respStr = PostRequest(_url + _api + "?action=wbeditentity&format=xml&clear=true", postData);
                if (respStr.IndexOf("<api success=\"1\">") == -1)
                {
                    return respStr;
                }
            }
            return "";
        }

        /// <summary>Create JSON string from a dictionary of sitelinks to use with API</summary>
        /// <param name="links">Dictionary links</param>
        /// <returns>JSON string</returns>
        private string GetJsonLinks(Dictionary<string, string> links)
        {
            string Json = "\"sitelinks\":{"; // "sitelinks":{
            foreach (KeyValuePair<string, string> pair in links)
            {
                Json += "\"" + pair.Key.Replace("-", "_") + "\":{\"site\":\"" + pair.Key.Replace("-", "_") + "\",\"title\":\"" + pair.Value + "\"},";
            }
            Json = Json.Remove(Json.LastIndexOf(","));
            Json += "}";
            return Json;
        }

        /// <summary>Create JSON string from a dictionary of labels to use with API</summary>
        /// <param name="labels">Dictionary of labels</param>
        /// <returns>JSON string</returns>
        private string GetJsonLabels(Dictionary<string, string> labels)
        {
            string Json = "\"labels\":{";
            foreach (KeyValuePair<string, string> pair in labels)
            {
                Json += "\"" + pair.Key + "\":{\"language\":\"" + pair.Key + "\",\"value\":\"" + pair.Value + "\"},";
            }
            Json = Json.Remove(Json.LastIndexOf(","));
            Json += "}";
            return Json;
        }

        /// <summary>Create JSON string from a dictionary of descriptions to use with API</summary>
        /// <param name="descriptions">Dictionary of decriptions</param>
        /// <returns>JSON string</returns>
        private string GetJsonDescriptions(Dictionary<string, string> descriptions)
        {
            string Json = "\"descriptions\":{";
            foreach (KeyValuePair<string, string> pair in descriptions)
            {
                Json += "\"" + pair.Key + "\":{\"language\":\"" + pair.Key + "\",\"value\":\"" + pair.Value.Replace("\"", "'") + "\"},";
            }
            Json = Json.Remove(Json.LastIndexOf(","));
            Json += "}";
            return Json;
        }

        /// <summary>Create JSON string from a dictionary of aliases to use with API</summary>
        /// <param name="aliases">Dictionary of aliases</param>
        /// <returns>JSON string</returns>
        private string GetJsonAliases(Dictionary<string, List<string>> aliases)
        {
            string Json = "\"aliases\": {";
            foreach (KeyValuePair<string, List<string>> pair in aliases)
            {
                Json += "\"" + pair.Key + "\":[";
                foreach (string alias in pair.Value)
                {
                    Json += "{" + "\"language\": \"" + pair.Key + "\", \"value\": \"" + alias + "\"},";
                }
                Json = Json.Remove(Json.LastIndexOf(","));
                Json += "],";
            }
            Json = Json.Remove(Json.LastIndexOf(","));
            Json += "}";
            return Json;
        }

        /// <summary>Create JSON string from a list of claims to use with API</summary>
        /// <param name="snaks">List of snak</param>
        /// <returns>JSON string</returns>
        private string GetJsonClaims(List<Claim> claims)
        {
            //Claim
            string Json = "\"claims\":[";
            foreach (Claim claim in claims)
            {
                if (claim.mainsnak.snaktype == "somevalue")
                {
                    Json += "\"mainsnak\":{\"snaktype\":\"somevalue\",\"property\":\"" + claim.mainsnak.property + "\"},";
                }
                else if (claim.mainsnak.snaktype == "novalue")
                {
                    Json += "\"mainsnak\":{\"snaktype\":\"novalue\",\"property\":\"" + claim.mainsnak.property + "\"},";
                }
                else
                {
                    if (claim.id != null && claim.id != "")
                    {
                        Json += "{\"id\":\"" + claim.id + "\",";
                    }
                    else
                    {
                        Json += "{";
                    }
                    Json += "\"mainsnak\":{\"snaktype\":\"value\",\"property\":\"" + claim.mainsnak.property + "\",\"datavalue\":" + claim.mainsnak.datavalue.Json;
                    Json += "\"type\":\"statement\",";
                    Json += "\"rank\":\"normal\","; //preferred  normal
                }

                //Qualificatori
                if (claim.qualifiers != null && claim.qualifiers.Count > 0)
                {
                    Json += "\"qualifiers\":{";
                    foreach (KeyValuePair<string, List<Qualifier>> qualifiers in claim.qualifiers)
                    {
                        Json += "\"" + qualifiers.Key + "\":[";
                        List<Qualifier> _qualifiers = qualifiers.Value;
                        foreach (Qualifier qualifier in _qualifiers)
                        {
                            Json += "{\"snaktype\":\"value\",\"property\":\"" + qualifiers.Key + "\",\"datavalue\":" + qualifier.datavalue.Json;
                        }
                        Json = Json.Remove(Json.LastIndexOf(","));
                        Json += "],";
                    }
                    Json = Json.Remove(Json.LastIndexOf(","));
                    Json += "},";
                }

                //References
                if (claim.references != null && claim.references.Count > 0)
                {
                    Json += "\"references\":[";
                    foreach (Reference reference in claim.references)
                    {
                        foreach (KeyValuePair<string, List<Snak>> snaks in reference.snaks)
                        {
                            Json += "{\"snaks\":{\"" + snaks.Key + "\":[";
                            List<Snak> _snaks = snaks.Value;
                            foreach (Snak snak in _snaks)
                            {
                                Json += "{\"snaktype\":\"value\",\"property\":\"" + snaks.Key + "\",\"datavalue\":" + snak.datavalue.Json;
                            }
                            Json = Json.Remove(Json.LastIndexOf(","));
                            Json += "]}},";
                        }
                    }
                    Json = Json.Remove(Json.LastIndexOf(","));
                    Json += "],";
                }

                Json = Json.Remove(Json.LastIndexOf(","));
                Json += "},";
            }
            Json = Json.Remove(Json.LastIndexOf(","));
            Json += "]";
            return Json;
        }

        public string SetReferenceURL(string statementID, string url)
        {
            string Ref= "{\"P854\":[{\"snaktype\":\"value\",\"property\":\"P854\",\"datavalue\":{\"type\":\"string\",\"value\":\"" + url + "\"}}]}";

            string postData = string.Format ("maxlag=5&assert=bot&statement={0}&snaks={1}&token={2}", WebUtility.UrlEncode(statementID), WebUtility.UrlEncode(Ref), WebUtility.UrlEncode(_editSessionToken));

            string respStr = PostRequest(_url + _api + "?action=wbsetreference&format=json", postData);
            return respStr;
        }


        /// <summary>Add or delete a Badge</summary>
        /// <param name="Item">Item number with Q (ex. Q42)</param>
        /// <param name="Sitelink">Sitelink where to add the badge (ex. itwiki)</param>
        /// <param name="Badge">Item of the badge, empty string to delete badge</param>
        /// <returns>Result of post request</returns>
        public string SetBadge(string Item, string Sitelink, string Badge)
        {
            string postData = string.Format("id={0}&token={1}&bot=bot&linksite={2}&badges={3}&maxlag=5&assert=bot", Item, WebUtility.UrlEncode(_editSessionToken), WebUtility.UrlEncode(Sitelink), Badge);
            string respStr = PostRequest(_url + _api + "?action=wbsetsitelink&format=json", postData);
            return respStr;
        }

        /// <summary>Merge 2 item in the item with lower id</summary>
        /// <param name="Item1"></param>
        /// <param name="Item2"></param>
        /// <returns></returns>
        public string MergeItem(string Item1, string Item2)
        {
            string LowerItem = "";
            string HigherItem = "";

            if(Int32.Parse(Item1.Replace("Q",""))> Int32.Parse(Item2.Replace("Q", "")))
            {
                LowerItem = Item2;
                HigherItem = Item1;
            }
            else
            {
                LowerItem = Item1;
                HigherItem = Item2;
            }

            string postData = string.Format("fromid={0}&toid={1}&token={2}&bot=bot&maxlag=5&assert=bot", HigherItem, LowerItem,WebUtility.UrlEncode(_editSessionToken));
            string respStr = PostRequest(_url + _api + "?action=wbmergeitems&format=json", postData);
            return respStr;
        }

        /// <summary>Delete a serie of properties</summary>
        /// <param name="idItem">id of the item</param>
        /// <param name="Allid">List of the id of the properties to be deleted</param>
        public string DelProperty(string idItem, List<string> Allid, string summary)
        {
            string data = "{\"claims\":[";
            foreach (string id in Allid)
            {
                data += "{\"id\":\"" + id + "\",\"remove\":\"\"},";
            }
            data = data.TrimEnd(',');
            data += "]}";

            string postData = string.Format("id={0}&data={1}&token={2}&bot=bot&summary={3}&maxlag=5&assert=bot", idItem, WebUtility.UrlEncode(data), WebUtility.UrlEncode(_editSessionToken), summary);
            string respStr = PostRequest(_url + _api + "?action=wbeditentity&format=json", postData);
            return respStr;
        }

        /// <summary>Delete a label in a specific language</summary>
        /// <param name="Item">Qnumber of the item</param>
        /// <param name="summary">Summary of the edit</param>
        /// <param name="lang">Language of the label that mus be delete</param>
        /// <returns>API response</returns>
        public string DelLabel(string Item, string summary, string lang)
        {
            string data = "{ \"labels\":[{\"language\":\"" + lang + "\",\"remove\":\"\"}]}";
            string postData = string.Format("id={0}&data={3}&token={1}&bot=bot&summary={2}&maxlag=5&assert=bot", Item, WebUtility.UrlEncode(_editSessionToken), summary, WebUtility.UrlEncode(data));
            string respStr = PostRequest(_url + _api + "?action=wbeditentity&format=json&assert=user", postData);
            return respStr;
        }

        /// <summary>Wiki that using this entity</summary>
        /// <param name="Item"></param>
        /// <returns>API response</returns>
        public string ItemUsage(string Item)
        {
            string postData = string.Format("list=wbsubscribers&wblsentities={0}", Item);
            string respStr = PostRequest(_url + _api + "?action=query&format=xml", postData);
            return respStr;
        }

        /// <summary>User that has created the item</summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        public string ItemCreator(string Item)
        {
            string postData = string.Format("prop=revisions&list=&titles={0}&rvlimit=1&rvdir=newer", Item);
            string respStr = PostRequest(_url + _api + "?action=query&format=xml", postData);
            if (respStr.IndexOf("missing=\"\"") != -1)
            {
                return "DELETED";
            }
            int Da = respStr.IndexOf("user=\"") + 6;
            int A = respStr.IndexOf("\"", Da);
            respStr = respStr.Substring(Da, A - Da);
            return respStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idItem"></param>
        /// <param name="Sitelink"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        public string DelSiteLink(string idItem,string Sitelink, string Reason="")
        {
            string summary = "Delete sitelink from "+ Sitelink + Reason;
            string postData = string.Format("id={0}&linksite={1}&token={2}&bot=bot&summary={3}&maxlag=5&assert=bot", idItem, WebUtility.UrlEncode(Sitelink), WebUtility.UrlEncode(_editSessionToken), summary);
            string respStr = PostRequest(_url + _api + "?action=wbsetsitelink&format=json", postData);
            return respStr;
        }

        #endregion

        #region Edit page
        /// <summary>Save a wiki page. Creates it if it doesn't exist</summary>
        /// <param name="Page">Title of the page</param>
        /// <param name="Text">Wiki Text to be saved</param>
        /// <param name="Summary">Comment for the edit</param>
        /// <returns>Result of post request</returns>
        public string SavePage(string Page, string Text, string Summary)
        {
            //string respStr = PostRequest(this._URL + this._API, "action=edit&format=xml&title=" + WebUtility.UrlEncode(Page) + "&summary=" + Summary + "&text=" + WebUtility.UrlEncode(Text) + "&minor=1&bot=1&token=" + WebUtility.UrlEncode(editSessionToken));
            string respStr = PostRequest(_url + _api, "action=edit&format=xml&title=" + WebUtility.UrlEncode(Page) + "&summary=" + Summary + "&text=" + WebUtility.UrlEncode(Text) + "&minor=1&bot=1&token=" + WebUtility.UrlEncode(_editSessionToken));
            return respStr;
        }
        #endregion

        #region HTTP request
        /// <summary>Function to do an HTTP Post request</summary>
        /// <param name="URL"></param>
        /// <param name="PostData">Data to use in Post request</param>
        /// <param name="Logon"></param>
        /// <returns>the result of the request</returns>
        /// <remarks>Base on "http://msdn.microsoft.com/it-it/library/debx8sh9(v=vs.110).aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-27"</remarks>
        /// <remarks>Modified to use cookies and compression</remarks>
        public string PostRequest(string URL, string PostData, bool Logon = false)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL); //Create a request using a URL that can receive a post
                request.UserAgent = _version + " by ValterVB";
                request.Method = "POST";
                request.UserAgent = _version; //Set User agent Test with false
                request.AllowAutoRedirect = true;
                request.AutomaticDecompression= DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.Proxy = null; //For performance 
                                      //request.Timeout = 30000;
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                request.ContentType = "application/x-www-form-urlencoded"; // Set the ContentType property of the WebRequest
                byte[] byteArray = Encoding.UTF8.GetBytes(PostData); // Create POST data and convert it to a byte array
                request.ContentLength = byteArray.Length; // Set the ContentLength property of the WebRequest

                if (_cookies.Count == 0)
                {
                    request.CookieContainer = new CookieContainer();
                }
                else
                {
                    request.CookieContainer = _cookies;
                }

                Stream dataStream = request.GetRequestStream(); // Get the request stream
                dataStream.Write(byteArray, 0, byteArray.Length); // Write the data to the request stream
                dataStream.Close(); // Close the Stream object

                HttpWebResponse response = (HttpWebResponse)request.GetResponse(); // Get the response
                Stream respStream = response.GetResponseStream();

                if (Logon)
                {
                    foreach (Cookie cookie in response.Cookies)
                    {
                        _cookies.Add(cookie);
                    }
                }
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8); // Open the stream using a StreamReader for easy access
                string strResponse = reader.ReadToEnd(); // Read the content
                reader.Close();
                response.Close();
                return strResponse;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
