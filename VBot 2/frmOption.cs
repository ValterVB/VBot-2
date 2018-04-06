using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace VBot
{
    public partial class frmOption : Form
    {
        public frmOption()
        {
            InitializeComponent();
            //StreamReader file = new StreamReader(@"C:\A\Wikiproject.txt", Encoding.UTF8);
            //string line = "";
            //while ((line = file.ReadLine()) != null)
            //{
            //    cboWikiLang.Items.Add(line.Split('\t')[0]);
            //}
            //file.Close();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(path + "VBotConfig.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path + "VBotConfig.xml");
                txtBotUser.Text = xmlDoc.SelectSingleNode("/VBot/botUser").Attributes["user"].Value;
                txtBotPassword.Text = xmlDoc.SelectSingleNode("/VBot/botUser").Attributes["password"].Value;
                txtAdminUser.Text = xmlDoc.SelectSingleNode("/VBot/adminUser").Attributes["user"].Value;
                txtAdminPassword.Text = xmlDoc.SelectSingleNode("/VBot/adminUser").Attributes["password"].Value;
                cboWikiProject.Text = xmlDoc.SelectSingleNode("/VBot/mainWiki").Attributes["project"].Value;
                cboWikiLang.Text = xmlDoc.SelectSingleNode("/VBot/mainWiki").Attributes["language"].Value;
                txtReportsPath.Text = xmlDoc.SelectSingleNode("/VBot/reportsPath").InnerText;
                txtDumpPath.Text = xmlDoc.SelectSingleNode("/VBot/dumpPath").InnerText;
                txtDumpFile.Text = xmlDoc.SelectSingleNode("/VBot/dumpFile").InnerText;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("VBot");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode = xmlDoc.CreateElement("botUser");
            XmlAttribute attribute = xmlDoc.CreateAttribute("user");
            attribute.Value = txtBotUser.Text ;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("password");
            attribute.Value = txtBotPassword.Text ;
            userNode.Attributes.Append(attribute);
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("adminUser");
            attribute = xmlDoc.CreateAttribute("user");
            attribute.Value = txtAdminUser.Text;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("password");
            attribute.Value = txtAdminPassword.Text;
            userNode.Attributes.Append(attribute);
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("mainWiki");
            attribute = xmlDoc.CreateAttribute("project");
            attribute.Value = cboWikiProject.Text;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute("language");
            attribute.Value = cboWikiLang.Text;
            userNode.Attributes.Append(attribute);
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("reportsPath");
            userNode.InnerText = txtReportsPath.Text;
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("dumpPath");
            userNode.InnerText = txtDumpPath.Text;
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("dumpFile");
            userNode.InnerText = txtDumpFile.Text;
            rootNode.AppendChild(userNode);

            xmlDoc.Save(path + "VBotConfig.xml");
            
        }
    }
}
