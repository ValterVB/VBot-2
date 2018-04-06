using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace VBot
{
    class ReportsWP
    {
        public static void TabellaDisambiguanti(string fileIn, string fileOut)
        {
            string line = "";
            string title = "";
            string dis = "";

            StreamWriter result = new StreamWriter(fileOut, false, Encoding.UTF8);
            StreamReader file = new StreamReader(fileIn, Encoding.UTF8);
            while ((line = file.ReadLine()) != null)
            {
                if (line.IndexOf("INSERT INTO `page` VALUES ") != -1)
                {
                    line = line.Replace("INSERT INTO `page` VALUES (", "");
                    string[] row = Regex.Split(line, @"\),\("); //Index 1=namespace, 2=title, 5=is redirect
                    for (int i = 0; i < row.Length; i++)
                    {
                        string[] tmp = row[i].Split(',');
                        if (tmp[1] == "0") //ns0
                        {
                            title = tmp[2].Replace("_", " ");
                            title = title.Substring(1);
                            if (title.Length > 1)
                            {
                                title = Regex.Unescape(title.Remove(title.Length - 1));
                            }

                            if (title.EndsWith(")") && title.LastIndexOf('(') != -1)
                            {
                                if (title.LastIndexOf('(') == 0)
                                {
                                    dis = "*";
                                }
                                else
                                {
                                    dis = Regex.Unescape(title.Substring(title.LastIndexOf('(')));
                                }
                                result.WriteLine(title + '\t' + dis + '\t' + tmp[5]);
                            }
                        }
                    }
                }
            }
            result.Close();
        }
    }
}
