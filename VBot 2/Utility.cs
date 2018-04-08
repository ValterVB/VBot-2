using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Xml;

namespace VBot
{
    public static class Utility
    {
        public enum TypeData
        {
            String,
            Monolingual,
            Item,
            Coordinate,
            Time,
            Quantity
        };

        #region List of languages with latin alphabet
        public static readonly List<string> lstLatin = new List<string>()
        {
            "en","de","fr","it","es","af","an","ast","bar","br","ca","co","cs","cy","da","de-at","de-ch","en-ca","en-gb","eo","et","eu","fi","frp","fur","ga","gd","gl","gsw","hr","ia","id","ie","is","io","kg","lb","li","lij","mg","min","ms","nap","nb","nds","nds-nl","nl","nn","nrm","oc","pcd","pl","pms","pt","pt-br","rm","ro","sc","scn","sco","sk","sl","sr-el","sv","sw","vec","vi","vls","vo","wa","wo","zu"
        };
        #endregion
        #region List of starting chars without problem of uppercase/lowercase
        public static readonly char[] goodChars = new char[] { '(', '!', '?', '"', '$', '\'', '.', ',', '/', ':', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        #endregion
        #region List of chars that do not cause problems in Latin alphabet
        public static readonly string shortAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789- ()";
        #endregion
        #region List of descriptions for list items
        public static readonly Dictionary<string, string> dicList = new Dictionary<string, string>
        {
            {"ar", "قائمة ويكيميديا"},
            {"as", "ৱিকিপিডিয়া:ৰচনাশৈলীৰ হাতপুথি"},
            {"be", "старонка-спіс у праекце Вікімедыя"},
            {"bn", "উইকিমিডিয়ার তালিকা নিবন্ধ"},
            {"bs", "spisak na Wikimediji"},
            {"ca", "article de llista de Wikimedia"},
            {"cs", "seznam na projektech Wikimedia"},
            {"da", "Wikimedia liste"},
            {"de", "Wikimedia-Liste"},
            {"de-at", "Wikimedia-Liste"},
            {"de-ch", "Wikimedia-Liste"},
            {"el", "κατάλογος εγχειρήματος Wikimedia"},
            {"en", "Wikimedia list article"},
            {"eo", "listartikolo en Vikimedio"},
            {"es", "artículo de lista de Wikimedia"},
            {"eu", "Wikimediako zerrenda artikulua"},
            {"fr", "liste d\'un projet Wikimedia"},
			{"fy", "Wikimedia-list"},
			{"gl", "artigo de listas da Wikimedia"},
            {"he", "רשימת ערכים במיזמי ויקימדיה"},
            {"hr", "popis na Wikimediji"},
            {"hy", "Վիքիմեդիայի նախագծի ցանկ"},
            {"it", "lista di un progetto Wikimedia"},
            {"ja", "ウィキメディアの一覧記事"},
            {"ko", "위키미디어 목록 항목"},
            {"lb", "Wikimedia-Lëschtenartikel"},
            {"nb", "Wikimedia-listeartikkel"},
            {"nl", "Wikimedia-lijst"},
            {"oc", "lista d\'un projècte Wikimèdia"},
            {"pl", "lista w projekcie Wikimedia"},
            {"ru", "статья-список в проекте Викимедиа"},
            {"si", "විකිමීඩියා ලැයිස්තු ලිපිය"},
            {"sk", "zoznamový článok projektov Wikimedia"},
            {"sl", "seznam Wikimedije"},
            {"sq", "artikull-listë e Wikimedias"},
            {"sr", "списак на Викимедији"},
            {"sv", "Wikimedia-listartikel"},
            {"sw", "orodha ya makala za Wikimedia"},
            {"ta", "விக்கிப்பீடியா:பட்டியலிடல்"},
            {"uk", "стаття-список у проекті Вікімедіа"},
            {"vi", "bài viết danh sách Wikimedia"},
            {"yi", "וויקימעדיע ליסטע אַרטיקל"},
            {"zh", "维基媒体列表条目"},
            {"zh-cn", "维基媒体列表条目"},
            {"zh-hans", "维基媒体列表条目"},
            {"zh-hant", "維基媒體列表條目"},
            {"zh-hk", "維基媒體列表條目"},
            {"zh-mo", "維基媒體列表條目"},
            {"zh-my", "维基媒体列表条目"},
            {"zh-sg", "维基媒体列表条目"},
            {"zh-tw", "維基媒體列表條目"}
        };
        #endregion
        #region List of descriptions for disambiguation items
        public static readonly Dictionary<string, string> dicDis = new Dictionary<string, string>
        {
            {"ar", "صفحة توضيح لويكيميديا"},
            {"bn", "উইকিমিডিয়া দ্ব্যর্থতা নিরসন পাতা"},
			{"be", "старонка неадназначнасці ў праекце Вікімедыя"},
			{"bg", "Уикимедия пояснителна страница"},
            {"bs", "čvor stranica na Wikimediji"},
            {"ca", "pàgina de desambiguació de Wikimedia"},
            {"ckb", "پەڕەی ڕوونکردنەوەی ویکیمیدیا"},
            {"cs", "rozcestník na projektech Wikimedia"},
            {"da", "Wikimedia-flertydigside"},
            {"de", "Wikimedia-Begriffsklärungsseite"},
            {"de-at", "Wikimedia-Begriffsklärungsseite"},
            {"de-ch", "Wikimedia-Begriffsklärungsseite"},
            {"el", "σελίδα αποσαφήνισης εγχειρημάτων Wikimedia"},
            {"en", "Wikimedia disambiguation page"},
            {"en-ca", "Wikimedia disambiguation page"},
            {"en-gb", "Wikimedia disambiguation page"},
            {"eo", "Vikimedia apartigilo"},
            {"es", "página de desambiguación de Wikimedia"},
            {"et", "Wikimedia täpsustuslehekülg"},
            {"eu", "Wikimediako argipen orri"},
            {"fa", "یک صفحهٔ ابهام‌زدایی در ویکی‌پدیا"},
            {"fi", "Wikimedia-täsmennyssivu"},
			{"fy", "Wikimedia-betsjuttingsside"},
			{"fr", "page d\'homonymie de Wikimedia"},
            {"gl", "páxina de homónimos de Wikimedia"},
            {"gsw", "Wikimedia-Begriffsklärigssite"},
            {"gu", "સ્પષ્ટતા પાનું"},
            {"he", "דף פירושונים"},
            {"hi", "बहुविकल्पी पृष्ठ"},
            {"hr", "razdvojbena stranica na Wikimediji"},
            {"hu", "Wikimédia-egyértelműsítőlap"},
            {"hy", "Վիքիմեդիայի նախագծի բազմիմաստության փարատման էջ"},
            {"id", "halaman disambiguasi"},
            {"is", "aðgreiningarsíða á Wikipediu"},
            {"it", "pagina di disambiguazione di un progetto Wikimedia"},
            {"ja", "ウィキメディアの曖昧さ回避ページ"},
            {"ka", "მრავალმნიშვნელოვანი"},
            {"ko", "위키미디어 동음이의어 문서"},
            {"lb", "Wikimedia-Homonymiesäit"},
            {"lv", "Wikimedia projekta nozīmju atdalīšanas lapa"},
            {"min", "laman disambiguasi"},
            {"mk", "појаснителна страница на Викимедија"},
            {"ms", "laman nyahkekaburan"},
            {"nb", "Wikimedia-pekerside"},
            {"nds", "Sied för en mehrdüdig Begreep op Wikimedia"},
            {"nl", "Wikimedia-doorverwijspagina"},
            {"nn", "Wikimedia-fleirtydingsside"},
            {"or", "ବହୁବିକଳ୍ପ ପୃଷ୍ଠା"},
            {"pl", "strona ujednoznaczniająca w projekcie Wikimedia"},
            {"pt", "página de desambiguação da Wikimedia"},
            {"pt-br", "página de desambiguação da Wikimedia"},
            {"ro", "pagină de dezambiguizare Wikimedia"},
            {"ru", "страница значений в проекте Викимедиа"},
            {"sco", "Wikimedia disambiguation page"},
            {"sk", "rozlišovacia stránka projektov Wikimedia"},
            {"sl", "razločitvena stran Wikimedije"},
            {"sq", "faqe kthjelluese e Wikimedias"},
            {"sr", "вишезначна одредница на Викимедији"},
            {"sv", "Wikimedia-förgreningssida"},
            {"sw", "ukarasa wa maana wa Wikimedia"},
            {"tr", "Wikimedia anlam ayrımı sayfası"},
            {"uk", "сторінка значень в проекті Вікімедіа"},
            {"vi", "trang định hướng Wikimedia"},
			{"yi", "וויקימעדיע באַדייטן בלאַט"},
			{"yue", "維基媒體搞清楚頁"},
            {"zh", "维基媒体消歧义页"},
            {"zh-cn", "维基媒体消歧义页"},
            {"zh-hans", "维基媒体消歧义页"},
            {"zh-hant", "維基媒體消歧義頁"},
            {"zh-hk", "維基媒體消歧義頁"},
            {"zh-mo", "維基媒體消歧義頁"},
            {"zh-my","维基媒体消歧义页"},
            {"zh-sg","维基媒体消歧义页" },
            {"zh-tw", "維基媒體消歧義頁"}
        };
        #endregion
        #region List of descriptions for category items
        public static readonly Dictionary<string, string> dicCat = new Dictionary<string, string>
        {
            {"af", "Wikimedia-kategorie"},
			{"an", "categoría de Wikimedia"},
			{"ar", "تصنيف ويكيميديا"},
			{"ast", "categoría de Wikimedia"},
			{"be", "катэгорыя ў праекце Вікімедыя"},
            {"be-tarask", "катэгорыя ў праекце Вікімэдыя"},
            {"bg", "категория на Уикимедия"},
            {"bn", "উইকিমিডিয়া বিষয়শ্রেণী"},
            {"bs", "kategorija na Wikimediji"},
            {"ca", "categoria de Wikimedia"},
            {"ckb", "پۆلی ویکیمیدیا"},
            {"cs", "kategorie na projektech Wikimedia"},
            {"cy", "tudalen categori Wikimedia"},
            {"da", "Wikimedia-kategori"},
            {"de-at", "Wikimedia-Kategorie"},
            {"de-ch", "Wikimedia-Kategorie"},
            {"de", "Wikimedia-Kategorie"},
            {"el", "κατηγορία εγχειρημάτων Wikimedia"},
            {"en", "Wikimedia category"},
            {"eo", "kategorio en Vikimedio"},
            {"es", "categoría de Wikimedia"},
            {"et", "Wikimedia kategooria"},
            {"eu", "Wikimediako kategoria"},
            {"fa", "ردهٔ ویکی‌پدیا"},
            {"fi", "Wikimedia-luokka"},
			{"fy", "Wikimedia-kategory"},
			{"fr", "page de catégorie de Wikimedia"},
            {"gl", "categoría de Wikimedia"},
            {"gsw", "Wikimedia-Kategorie"},
            {"gu", "વિકિપીડિયા શ્રેણી"},
            {"he", "קטגוריה במיזמי ויקימדיה"},
            {"hr", "kategorija na Wikimediji"},
            {"hu", "Wikimédia-kategória"},
            {"hy", "Վիքիմեդիայի նախագծի կատեգորիա"},
            {"ilo", "kategoria ti Wikimedia"},
            {"it", "categoria di un progetto Wikimedia"},
            {"ja", "ウィキメディアのカテゴリ"},
            {"ko", "위키미디어 분류"},
            {"lb", "Wikimedia-Kategorie"},
            {"lv", "Wikimedia projekta kategorija"},
            {"mk", "Викимедиина категорија"},
            {"nap", "categurìa \'e nu pruggette Wikimedia"},
            {"nb", "Wikimedia-kategori"},
            {"nds", "Wikimedia-Kategorie"},
            {"nl", "Wikimedia-categorie"},
            {"nn", "Wikimedia-kategori"},
            {"pl", "kategoria w projekcie Wikimedia"},
            {"pt", "categoria de um projeto da Wikimedia"},
            {"pt-br", "categoria de um projeto da Wikimedia"},
            {"ro", "categorie a unui proiect Wikimedia"},
            {"ru", "категория в проекте Викимедиа"},
            {"sco", "Wikimedia category"},
            {"sk", "kategória projektov Wikimedia"},
            {"sl", "kategorija Wikimedije"},
            {"sq", "kategori e Wikimedias"},
            {"sr", "категорија на Викимедији"},
            {"sv", "Wikimedia-kategori"},
            {"sw", "jamii ya Wikimedia"},
            {"uk", "категорія в проекті Вікімедіа"},
            {"vi", "thể loại Wikimedia"},
			{"yi", "וויקימעדיע קאַטעגאָריע"},
			{"yue", "維基媒體分類"},
            {"zh", "维基媒体分类"},
            {"zh-cn", "维基媒体分类"},
            {"zh-hans", "维基媒体分类"},
            {"zh-hant", "維基媒體分類"},
            {"zh-hk", "維基媒體分類"},
            {"zh-mo", "維基媒體分類"},
            {"zh-my", "维基媒体分类"},
            {"zh-sg", "维基媒体分类"},
            {"zh-tw", "維基媒體分類"}
        };
        #endregion
        #region List of descriptions for template items
        public static readonly Dictionary<string, string> dicTempl = new Dictionary<string, string>
        {
            {"ab", "ашаблон Авикипедиа"},
            {"ace", "pola Wikimèdia"},
            {"af", "sjabloon Wikimedia"},
            {"ak", "şablon Wikimedia"},
            {"am", "መለጠፊያ ውክፔዲያ"},
            {"an", "plantilla de Wikimedia"},
            {"ang", "Ƿikipǣdia bysen"},
            {"ar", "قالب ويكيميديا"},
            {"arc", "ܩܠܒܐ ܘܝܩܝܦܕܝܐ"},
            {"arz", "قالب ويكيبيديا"},
            {"as", "সাঁচ ৱিকিপিডিয়া"},
            {"ast", "plantía de Wikimedia"},
            {"av", "шаблон Википедия"},
            {"ay", "plantilla Wikipidiya"},
            {"az", "şablon Vikipediya"},
            {"ba", "ҡалып Википедия"},
            {"bar", "Wikimedia-Vorlog"},
            {"bcl", "plantilya Wikimedia"},
            {"be", "шаблон праекта Вікімедыя"},
            {"be-tarask", "шаблён Вікімэдыя"},
            {"bg", "Уикимедия шаблон"},
            {"bho", "टेम्पलेट विकिपीडिया"},
            {"bi", "Wikimedia template"},
            {"bjn", "citakan Wikimedia"},
            {"bm", "modèle Wikipedi"},
            {"bn", "উইকিমিডিয়া টেমপ্লেট"},
            {"bo", "དཔེ་པང་། ལྦེ་ཁེ་རིག་མཛོད།"},
            {"bpy", "মডেল উইকিপিডিয়া"},
            {"br", "patrom Wikimedia"},
            {"bs", "šablon Wikimedia"},
            {"bug", "templat Wikimedia"},
            {"bxr", "википеэди template"},
            {"ca", "plantilla de Wikimedia"},
            {"cbk-zam", "plantilla Wikimedia"},
            {"cdo", "Wikimedia template"},
            {"ce", "куцкеп Википеди"},
            {"ceb", "plantilya Wikipedya"},
            {"ch", "Wikimedia template"},
            {"chr", "template ᏫᎩᏇᏗᏯ"},
            {"chy", "Vekepete\'a template"},
            {"ckb", "داڕێژەی ویکیمیدیا"},
            {"co", "Wikimedia template"},
            {"cr", "ᐧᐃᑭᐱᑎᔭ template"},
            {"crh-latn", "şablon Vikipediya"},
            {"cs", "šablona na projektech Wikimedia"},
            {"csb", "szablóna Wikipedijô"},
            {"cu", "обраꙁьць Википєдїꙗ"},
            {"cv", "шаблон Википеди"},
            {"cy", "Wicipedia nodyn"},
            {"da", "Wikimedia-skabelon"},
            {"de", "Wikimedia-Vorlage"},
            {"de-at", "Wikimedia-Vorlage"},
            {"de-ch", "Wikimedia-Vorlage"},
            {"diq", "şablon Wikipediya"},
            {"dsb", "Wikimedija pśedłoga"},
            {"dv", "ފަންވަތް ވިކިޕީޑިޔާ"},
            {"ee", "Wikimedia template"},
            {"el", "Πρότυπο εγχειρήματος Wikimedia"},
            {"eml", "template Vichipedia"},
            {"en", "Wikimedia template"},
            {"en-ca", "Wikimedia template"},
            {"en-gb", "Wikimedia template"},
            {"eo", "Vikimedia ŝablono"},
            {"es", "plantilla de Wikimedia"},
            {"et", "Wikimedia mall"},
            {"eu", "Wikimediako txantiloi"},
            {"ext", "prantilla Güiquipeya"},
            {"fa", "الگو ویکی‌پدیا"},
            {"ff", "modèle Wikipeediya"},
            {"fi", "Wikimedia-malline"},
            {"fj", "Wikimedia template"},
            {"fo", "fyrimynd Wikimedia"},
            {"fr", "modèle de Wikimedia"},
            {"frp", "modèlo Wikimedia"},
            {"frr", "Wikimediavorlage"},
            {"fur", "model Vichipedie"},
            {"fy", "Wikimedia-berjocht"},
            {"ga", "Vicipéid teimpléad"},
            {"gag", "şablon Vikipediya"},
            {"gan", "模板 維基百科"},
            {"gd", "Uicipeid teamplaid"},
            {"gl", "modelo de Wikimedia"},
            {"glk", "الگو ویکیپدیا"},
            {"gn", "tembiecharã Vikipetã"},
            {"got", "𐍅𐌹𐌺𐌹𐍀𐌰𐌹𐌳𐌾𐌰 𐍆𐌰𐌿𐍂𐌰𐌼𐌴𐌻𐌴𐌹𐌽𐍃"},
            {"gsw", "Wikimedia-Vorlage"},
            {"gu", "ઢાંચો વિકિપીડિયા"},
            {"gv", "clowan Wikimedia"},
            {"ha", "Wikimedia template"},
            {"hak", "Wikimedia template"},
            {"haw", "anakuhi ʻo Wikipikia"},
            {"he", "דף תבנית במיזמי ויקימדיה"},
            {"hi", "साँचा विकिपीडिया"},
            {"hif", "Wikimedia template"},
            {"hr", "predložak Wikimedija"},
            {"hsb", "předłoha Wikimedije"},
            {"ht", "modèl Wikipedya"},
            {"hu", "Wikimédia-sablon"},
            {"hy", "Վիքիմեդիայի նախագծի կաղապար"},
            {"ia", "Wikimedia patrono"},
            {"id", "templat Wikimedia"},
            {"ie", "avise Wikimedia"},
            {"ig", "àtụ Wikimedia"},
            {"ilo", "plantilia ti Wikimedia"},
            {"io", "shablono Wikipedio"},
            {"is", "snið Wikimedia"},
            {"it", "template di un progetto Wikimedia"},
            {"ja", "ウィキメディアのテンプレート"},
            {"jv", "cithakan Wikimedia"},
            {"ka", "თარგი ვიკიპედია"},
            {"kaa", "shablon Wikimedia"},
            {"kg", "Wikimedia template"},
            {"ki", "Wikimedia template"},
            {"kk", "улгі Уикимедиа"},
            {"kl", "ilisserut Wikimedia"},
            {"km", "វិគីភីឌា ទំព័រគំរូ"},
            {"kn", "ಟೆಂಪ್ಲೇಟು ವಿಕಿಪೀಡಿಯ"},
            {"ko", "위키미디어 틀"},
            {"koi", "шаблон Википедия"},
            {"ks", "فرماویکیپیٖڈیا"},
            {"ku", "şablon Wîkîpediya"},
            {"kw", "skantlyn Wikipedya"},
            {"ky", "калып Уикипедия"},
            {"la", "formula Vicimedia"},
            {"lad", "xablón Vikipedya"},
            {"lb", "Wikimedia-Schabloun"},
            {"li", "sjabloon Wikimedia"},
            {"lmo", "Wikimedia mudel"},
            {"ln", "Wikimedia modèle"},
            {"lo", "ແມ່ແບບ ວິກິພີເດຍ"},
            {"lt", "šablonas Vikipedija"},
            {"lv", "Wikimedia projekta veidne"},
            {"lzh", "template 維基大典"},
            {"map-bms", "cithakan Wikimedia"},
            {"mdf", "шаблон Википедиесь"},
            {"mg", "Wikimedia endrika"},
            {"mhr", "кышкар Википедий"},
            {"mi", "Wikimedia template"},
            {"min", "templat Wikimedia"},
            {"mk", "шаблон на Викимедија"},
            {"ml", "ഫലകം വിക്കിപീഡിയ"},
            {"mn", "загвар Википедиа"},
            {"mr", "विकिपीडिया साचा"},
            {"ms", "templat Wikimedia"},
            {"mt", "mudell Wikimedia"},
            {"mwl", "modelo Biquipédia"},
            {"my", "ဝီကီပီးဒီးယား template"},
            {"mzn", "شابلون ویکی‌پدیا"},
            {"nb", "Wikimedia-mal"},
            {"nds", "Wikimedia-Vörlaag"},
            {"ne", "ढाँचा विकिपीडिया"},
            {"new", "विकिपिडिया template"},
            {"nl", "Wikimedia-sjabloon"},
            {"nn", "Wikimedia-mal"},
            {"nov", "Wikipedie template"},
            {"nso", "Wikimedia template"},
            {"oc", "modèl Wikimedia"},
            {"om", "Wikimedia template"},
            {"or", "ଉଇକିପିଡ଼ିଆ ଛାଞ୍ଚ"},
            {"os", "хуызæг Википеди"},
            {"pa", "ਫਰਮਾ ਵਿਕੀਪੀਡੀਆ"},
            {"pag", "Wikimedia template"},
            {"pam", "Wikimedia template"},
            {"pap", "Wikimedia template"},
            {"pi", "पटिरूप विकिपीडिया"},
            {"pl", "szablon w projekcie Wikimedia"},
            {"pnb", "template وکیپیڈیا"},
            {"pnt", "πρότυπον Βικιπαίδεια"},
            {"ps", "کينډۍ ويکيپېډيا"},
            {"pt", "predefinição Wikimedia"},
            {"qu", "plantilla Wikipidiya"},
            {"rm", "model Vichipedia"},
            {"rmy", "sikavno Vikipidiya"},
            {"ro", "format Wikimedia"},
            {"ru", "шаблон в проекте Викимедиа"},
            {"rue", "шаблона Вікімедія"},
            {"rup", "Wikimedia template"},
            {"sa", "फलकम् विकिपीडिया"},
            {"sah", "халыып Бикипиэдьийэ"},
            {"sc", "Wikimedia template"},
            {"scn", "Wikimedia template"},
            {"sco", "Wikimedia template"},
            {"sd", "سانچو وڪيپيڊيا"},
            {"sg", "modèle Wïkïpêdïyäa"},
            {"sgs", "šabluons Vikipedėjė"},
            {"sh", "šablon Wikipediju"},
            {"si", "සැකිල්ල විකිපීඩියා"},
            {"sk", "šablóna projektov Wikimedia"},
            {"sl", "predloga Wikimedije"},
            {"so", "Wikimedia template"},
            {"sq", "stampë e Wikimedias"},
            {"sr", "Викимедијин шаблон"},
            {"srn", "ankra Wikipedia"},
            {"stq", "foarloage Wikipedia"},
            {"su", "citakan Wikipédia"},
            {"sv", "Wikimedia-mall"},
            {"sw", "kigezo cha Wikimedia"},
            {"ta", "வார்ப்புரு விக்கிப்பீடியா"},
            {"te", "మూస వికీపీడియా"},
            {"tg", "шаблон Википедиа"},
            {"th", "แม่แบบ วิกิพีเดีย"},
            {"tk", "şablon Wikipediýa"},
            {"tl", "padron Wikimedia"},
            {"tn", "Wikimedia template"},
            {"tpi", "Wikimedia templet"},
            {"tr", "Wikimedia şablonu"},
            {"ts", "template Wikimedia"},
            {"tt", "калып Викимедиа"},
            {"tw", "Wikimedia template"},
            {"ug", "قېلىپ ۋىكىپېدىيە"},
            {"uk", "шаблон проекту Вікімедіа"},
            {"ur", "سانچہ ویکیپیڈیا"},
            {"uz", "andoza Vikipediya"},
            {"vi", "bản mẫu Wikimedia"},
            {"vro", "näüdüs Vikipeediä"},
            {"wa", "modele Wikimedia"},
            {"war", "batakan Wikimedia"},
            {"wuu", "模板 维基百科"},
            {"xal", "кевләр Бикипеди"},
            {"xmf", "თარგი ვიკიპედია"},
            {"yi", "וויקימעדיע מוסטער"},
			{"yo", "àdàkọ Wikimedia"},
			{"yue", "維基媒體模"},
            {"za", "模板 Veizgiek Bakgoh"},
            {"zh", "维基媒体模板"},
            {"zh-cn", "维基媒体模板"},
            {"zh-hans", "维基媒体模板"},
            {"zh-hant", "維基媒體模板"},
            {"zh-hk", "維基媒體模板"},
            {"zh-mo", "維基媒體模板"},
            {"zh-sg", "维基媒体模板"},
            {"zh-tw", "維基媒體模板"},
            {"zu", "Wikimedia template"}
        };
        #endregion
        #region List of accepted values for which we can add labels without problem of uppercase/lowercase
        public static readonly List<Datavalue> dicP31ForLabel = new List<Datavalue>
        {
            CreateDataValue("Q4167836", TypeData.Item), //category
            CreateDataValue("Q29848066", TypeData.Item), //category of event
            CreateDataValue("Q3624078", TypeData.Item), //stato sovrano
            CreateDataValue("Q5", TypeData.Item), //umano
            CreateDataValue("Q515", TypeData.Item), //città
            CreateDataValue("Q11424", TypeData.Item), //film
            CreateDataValue("Q24862", TypeData.Item), //cortometraggio
            CreateDataValue("Q747074", TypeData.Item), //comune italiano
            CreateDataValue("Q484170", TypeData.Item), //comune francese
            CreateDataValue("Q5398426", TypeData.Item), //serie televisiva
            CreateDataValue("Q3863", TypeData.Item), //asteroide
            CreateDataValue("Q11266439", TypeData.Item), //template
            CreateDataValue("Q215380", TypeData.Item), //gruppo musicale
            CreateDataValue("Q571", TypeData.Item), //libro
            CreateDataValue("Q783794", TypeData.Item), //azienda
            CreateDataValue("Q482994", TypeData.Item), //album discografico
            CreateDataValue("Q134556", TypeData.Item), //singolo discografico
            CreateDataValue("Q253019", TypeData.Item), //distretto municipale tedesco
            CreateDataValue("Q4167410", TypeData.Item), //disambigua
            CreateDataValue("Q7889", TypeData.Item), //videogioco
            CreateDataValue("Q101352", TypeData.Item), //cognome
            CreateDataValue("Q202444", TypeData.Item), //prenome
            CreateDataValue("Q169930", TypeData.Item), //EP
            CreateDataValue("Q222910", TypeData.Item), //compilation
            CreateDataValue("Q209939", TypeData.Item), //album dal vivo
            CreateDataValue("Q44559", TypeData.Item), //pianeta extrasolare
            CreateDataValue("Q523", TypeData.Item) //stella
        };
        #endregion

        /// <summary>Check if description in AutoEdit is the same for VBot 2 (disambiguation, category, template and list)</summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>List of the difference</returns>
        public static string CheckDesc(string user, string password)
        {
            Site WP = new Site("https://www.wikidata.org", user, password);
            if (WP.User == "") { return "Wrong user/password"; }

            string strJson = WP.LoadWP(@"MediaWiki:Gadget-autoEdit.js");
            Pages pages = JsonConvert.DeserializeObject<Pages>(strJson, new DatavalueConverter());
            string text = pages.query.FirstPageText;

            string message = "";
            Dictionary<string, string> tmpDic = AutoEdit("Wikimedia disambiguation page", text);
            foreach (KeyValuePair<string, string> pair in tmpDic)
            {
                if (dicDis.ContainsKey(pair.Key))
                {
                    if (dicDis[pair.Key] != Regex.Unescape(tmpDic[pair.Key]))
                    {
                        message += "Dis: Change " + '\t' + pair.Key + "\t" + dicDis[pair.Key] + '\t' + Regex.Unescape(tmpDic[pair.Key]) + Environment.NewLine;
                    }
                }
                else
                {
                    message += "Dis: Add lang " + '\t' + pair.Key + '\t' + '\t' + pair.Value + Environment.NewLine;
                }
            }
            tmpDic = AutoEdit("Wikimedia category", text);
            foreach (KeyValuePair<string, string> pair in tmpDic)
            {
                if (dicCat.ContainsKey(pair.Key))
                {
                    if (dicCat[pair.Key] != Regex.Unescape(tmpDic[pair.Key]))
                    {
                        message += "Cat: Change " + '\t' + pair.Key + "\t" + dicCat[pair.Key] + '\t' + Regex.Unescape(tmpDic[pair.Key]) + Environment.NewLine;
                    }
                }
                else
                {
                    message += "Cat: Add lang " + '\t' + pair.Key + '\t' + '\t' + pair.Value + Environment.NewLine;
                }
            }
            tmpDic = AutoEdit("Wikimedia template", text);
            foreach (KeyValuePair<string, string> pair in tmpDic)
            {
                if (dicTempl.ContainsKey(pair.Key))
                {
                    if (dicTempl[pair.Key] != Regex.Unescape(tmpDic[pair.Key]))
                    {
                        message += "Templ: Change " + '\t' + pair.Key + "\t" + dicTempl[pair.Key] + '\t' + Regex.Unescape(tmpDic[pair.Key]) + Environment.NewLine;
                    }
                }
                else
                {
                    message += "Templ: Add lang " + '\t' + pair.Key + '\t' + '\t' + pair.Value + Environment.NewLine;
                }
            }
            tmpDic = AutoEdit("Wikimedia list article", text);
            foreach (KeyValuePair<string, string> pair in tmpDic)
            {
                if (dicList.ContainsKey(pair.Key))
                {
                    if (dicList[pair.Key] != Regex.Unescape(tmpDic[pair.Key]))
                    {
                        message += "List: Change " + '\t' + pair.Key + "\t" + dicList[pair.Key] + '\t' + Regex.Unescape(tmpDic[pair.Key]) + Environment.NewLine;
                    }
                }
                else
                {
                    message += "List: Add lang " + '\t' + pair.Key + '\t' + '\t' + pair.Value + Environment.NewLine;
                }
            }
            return message + Environment.NewLine + "Done";
        }

        /// <summary>Return a dictionary with description listed in Autoedit gadget</summary>
        /// <param name="Descriptions">Descriptions group to be import</param>
        /// <param name="Text">Text of autoedit page</param
        /// <returns>Dictionary Lang/Description</returns>
        /// <example>Dictionary<string, string> dicDis = AutoEdit("Wikimedia disambiguation page");</example>
        private static Dictionary<string, string> AutoEdit(string Descriptions, string Text)
        {
            Dictionary<string, string> tmpDic = new Dictionary<string, string>();

            string[] lines = Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                if (lines[idx].IndexOf("'" + Descriptions + "': {") > -1)
                {
                    for (int idx2 = idx + 1; idx2 < lines.Count(); idx2++)
                    {
                        if (lines[idx2].IndexOf("\t}") > -1) { break; }
                        string line = lines[idx2].Replace("\t", "");
                        line = line.Replace("': '", "\t");

                        string[] tmp = line.Split('\t');
                        tmp[0] = tmp[0].Replace("'", "");
                        if (tmp[1].EndsWith(","))
                        {
                            tmp[1] = tmp[1].Substring(0, tmp[1].Length - 2);
                        }
                        else
                        {
                            tmp[1] = tmp[1].Substring(0, tmp[1].Length - 1);
                        }
                        tmpDic.Add(tmp[0], tmp[1]);
                    }
                }
            }
            return tmpDic;
        }

        /// <summary>To split a string in chunks with a limited dimension</summary>
        /// <param name="Text">List of item separated by |</param>
        /// <param name="Divider">Char to use like divider, is possible to use Environment.NewLine</param>
        /// <param name="MaxItem">Max number of item in a single chunk, default is 500 (BOT or admin limit)</param>
        /// <returns>List of item grouped by Chunk</returns>
        public static List<string> SplitInChunk(string Text, string Divider, int MaxItem=500)
        {
            int cont = 0;
            List<string> chunks = new List<string>();

            string[] list = Text.Split(new string[] { Divider }, StringSplitOptions.RemoveEmptyEntries);

            String chunk = "";
            foreach (string item in list)
            {
                cont ++;
                chunk += item + "|";
                if (cont == MaxItem)
                {
                    cont = 0;
                    chunks.Add(chunk.Remove(chunk.LastIndexOf("|")));
                    chunk = "";
                }
            }
            if (chunk != "") { chunks.Add(chunk.Remove(chunk.LastIndexOf("|"))); }

            return chunks;
        }

        /// <summary>Create a specific data value, if are necessary more values, pass them separated by |</summary>
        /// <param name="value">Value of datavalue. ex. 100|90|5</param>
        /// <param name="type">Type of datavalue, see typeData ex. Utility.typeData.Item</param>
        /// <returns>Compiled data value</returns>
        public static Datavalue CreateDataValue(string value, TypeData type)
        {
            // TODO: Add missing datatype
            string[] val = value.Split('|');
            switch (type)
            {
                case TypeData.String: //0=string
                    DatavalueString tmpS = new DatavalueString();
                    tmpS.type = "string";
                    tmpS.value = val[0];
                    return tmpS;
                case TypeData.Monolingual: //0=language, 1=text
                    DatavalueMonolingual tmpM = new DatavalueMonolingual();
                    tmpM.type = "monolingualtext";
                    tmpM.value.language = val[0];
                    tmpM.value.text = val[1];
                    return tmpM;
                case TypeData.Item: //0=item with or without "Q"
                    DatavalueItem tmpW = new DatavalueItem();
                    tmpW.type = "wikibase-entityid";
                    tmpW.value.numeric_id = Convert.ToInt32(val[0].Replace("Q", "").Replace("q", ""));
                    tmpW.value.entity_type = "item";
                    return tmpW;
                case TypeData.Coordinate: //0=latitude, 1=longitude, 2=precision 3=globe (if not declared use Q2)
                    DatavalueCoordinate tmpC = new DatavalueCoordinate();
                    tmpC.type = "globecoordinate";
                    tmpC.value.latitude = val[0]; // decimal: no default, 9 digits after the dot and two before, signed
                    tmpC.value.longitude = val[1]; // decimal: no default, 9 digits after the dot and three before, signed
                    tmpC.value.precision = val[2]; // decimal, representing degrees of distance, defaults to 0, 9 digits after the dot and three before, unsigned, used to save the precision of the representation
                    tmpC.value.altitude = null; //unmanaged
                    tmpC.value.globe = val.Count() < 4 ? "http://www.wikidata.org/entity/Q2" : tmpC.value.globe = val[3];
                    return tmpC;
                case TypeData.Time: //0=time, 1=timezone, 2=before, 3=after, 4=precision, 5=calendarmodel (if not declared use Proleptic Gregorian)
                    DatavalueTime tmpT = new DatavalueTime();
                    tmpT.type = "time";
                    tmpT.value.time = val[0]; // string isotime: point in time, represented per ISO8601, they year always having 11 digits, the date always be signed, in the format +00000002013-01-01T00:00:00Z
                    tmpT.value.timezone = val[1]; // signed integer: Timezone information as an offset from UTC in minutes
                    tmpT.value.before = val[2]; // integer: If the date is uncertain, how many units before the given time could it be? the unit is given by the precision
                    tmpT.value.after = val[3]; // integer: If the date is uncertain, how many units after the given time could it be? the unit is given by the precision
                    tmpT.value.precision = val[4]; // shortint: 0 - billion years, 1 - hundred million years, ..., 6 - millenia, 7 - century, 8 - decade, 9 - year, 10 - month, 11 - day, 12 - hour, 13 - minute, 14 - second
                    tmpT.value.calendarmodel = val.Count() < 6 ? "http://www.wikidata.org/entity/Q1985727" : val[5]; // URI identifying the calendar model that should be used to display this time value. Note that time is always saved in proleptic Gregorian, this URI states how the value should be displayed
                    return tmpT;
                case TypeData.Quantity: //0=item without
                    DatavalueQuantity tmpQ = new DatavalueQuantity();
                    tmpQ.type = "quantity";
                    tmpQ.value.amount = val[0];
                    tmpQ.value.unit = val[1];
                    tmpQ.value.upperBound = val[2];
                    tmpQ.value.lowerBound = val[3];
                    return tmpQ;
                default:
                    return null;
            }
        }

        /// <summary>Convert from sitelink in language code</summary>
        /// <param name="Sitelink">Sitelink, ex. simplewiki</param>
        /// <returns>language code or empty string</returns>
        public static string SitelinkToLanguages(string Sitelink)
        {
            string lang = "";
            if (Sitelink == "alswiki") { return "gsw"; }
            else if (Sitelink == "crhwiki") { return "crh-latn"; }
            else if (Sitelink == "nowiki") { return "nb"; }
            else if (Sitelink == "simplewiki") { return "en"; }
            else if (Sitelink == "bat_smgwiki") { return "sgs"; }
            else if (Sitelink == "be_x_oldwiki") { return "be-tarask"; }
            else if (Sitelink == "fiu_vrowiki") { return "vro"; }
            else if (Sitelink == "roa_rupwiki") { return "rup"; }
            else if (Sitelink == "zh_classicalwiki") { return "lzh"; }
            else if (Sitelink == "zh_min_nanwiki") { return "nan"; }
            else if (Sitelink == "zh_yuewiki") { return "yue"; }
            else if (Sitelink == "bhwiki") { return "bho"; }
            else { lang=""; }

            return lang;
        }

        /// <summary>Return the dictionary for a specific template</summary>
        /// <param name="Text">Text with template</param>
        /// <param name="TemplateName">Name of template </param>
        /// <returns>Dictionary with parameter name, parameter value</returns>
        public static StringDictionary GetTemplate(string Text, string TemplateName)
        {
            Match match = Regex.Match(Text, @"{{\s*" + TemplateName, RegexOptions.IgnoreCase);
            int Bracket = 0;
            string template = "";
            StringDictionary Template = new StringDictionary();

            if (match.Success)
            {
                int start = match.Index;
                int end = 0;
                Bracket = 2;
                int Cont = 0;
                for (int idx = start + 2; idx <= Text.Length; idx++)
                {
                    if (Text[idx] == '}') { Bracket -= 1; }
                    if (Text[idx] == '{') { Bracket += 1; }
                    if (Bracket == 0)
                    {
                        end = idx;
                        template = Text.Substring(start, end - start + 1);
                        template = CleanWiki(template);
                        template = template.Remove(0, 2);
                        template = template.Remove(template.Length - 2, 2);
                        string[] fields = template.Split('|');
                        for (int idx2 = 0; idx2 < fields.Count(); idx2++)
                        {
                            string[] split = fields[idx2].Split(new char[] { '=' }, 2);
                            if (split.Count() == 2)
                            {
                                // TODO: Check for double parameter
                                Template.Add(split[0].Trim(), split[1].Trim());
                                Cont += 1;
                            }
                            else
                            {
                                Template.Add(Cont.ToString(), split[0].Trim());
                                Cont += 1;
                            }
                        }
                        break;
                    }
                }
            }
            return Template;
        }

        public static string GetTemplateParameter(string Text, string TemplateName, string Parameter)
        {
            //Dictionary<string, string> template = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            StringDictionary template = new StringDictionary();
            template = GetTemplate(Text, TemplateName);
            if (template.ContainsKey(Parameter))
            {
                return template[Parameter];
            }
            else
            {
                return "";
            }
        }
        /// <summary>Delete piped from wikilink</summary>
        /// <param name="Text">Text with wikilink with piped</param>
        /// <returns>Text with wikilink without piped</returns>
        private static string DelPipe(string Text)
        {
            int length = Text.Length;
            for (int idx = 0; idx < length; idx++)
            {
                if (Text[idx] == '[' && Text[idx + 1] == '[')
                {
                    int pipe = -1;
                    for (int idx2 = idx; idx2 < Text.Length; idx2++)
                    {
                        if (Text[idx2] == '|')
                        {
                            pipe = idx2;
                        }
                        if (Text[idx2] == ']' && Text[idx2 + 1] == ']')
                        {
                            if (pipe != -1)
                            {
                                Text = Text.Remove(pipe, idx2 - pipe);
                                length = Text.Length;
                            }
                            idx = idx2;
                            break;
                        }
                    }
                }
            }
            return Text;
        }

        /// <summary>Clean of wikitext: del comment, nowiki and ref</summary>
        /// <param name="Text">Wikitext</param>
        /// <returns>Clenaed wikitext</returns>
        private static string CleanWiki(string Text)
        {
            Regex regex = new Regex("<!--.*-->", RegexOptions.Compiled);
            string result = regex.Replace(Text, "");
            regex = new Regex("(?is)<nowiki>(.*?)</nowiki>", RegexOptions.Compiled);
            result = regex.Replace(result, "");
            regex = new Regex("<ref *>.*</ref *>", RegexOptions.Compiled);
            result = regex.Replace(result, "");
            regex = new Regex("<ref name=*.*</ref *>", RegexOptions.Compiled);
            result = regex.Replace(result, "");
            result = DelPipe(result);
            return result;
        }

        /// <summary>Find position of a section</summary>
        /// <param name="Text">Wiki text</param>
        /// <param name="Section">Section to find with level (ex. == External link ==)</param>
        /// <returns>Position</returns>
        public static int SectionStart(string Text, string Section)
        {
            Regex regex = new Regex(@"==\s*" + Section + @"\s*==", RegexOptions.IgnoreCase);
            Match match = regex.Match(Text);
            return match.Index;
        }

        /// <summary>Delete disambiguation from a title</summary>
        /// <param name="Title">Title</param>
        /// <param name="Disambig">Must be , or ()</param>
        /// <returns>Title without disambiguation</returns>
        public static string DelDisambiguation(string Title, string Disambig)
        {
            if (Disambig == "()")
            {
                int lung = Title.Length;
                if (Title.Substring(lung - 1) == ")")
                {
                    int da = Title.LastIndexOf("(");
                    if (da == -1 || da == 0)
                    {
                        return Title;
                    }
                    else
                    {
                        return Title.Substring(0, da - 1);
                    }
                }
            }
            else if (Disambig == ",")
            {
                int da = Title.IndexOf(",");
                if (da != -1)
                {
                    string temp = Title.Substring(0, da);
                    return temp;
                }
                else
                {
                    return Title;
                }
            }
            return Title;
        }

        /// <summary>Check if a page is a disambiguation page</summary>
        /// <param name="SiteLink"></param>
        /// <param name="page"></param>
        /// <returns>True is a disambiguation, False no</returns>
        public static bool IsWikiDisambiguation(string SiteLink, string page)
        {
            string api = "";
            if (SiteLink.IndexOf("wikisource") != -1)
            {
                api = "https://" + SiteLink.Replace("wikisource", "").Replace("_", "-") + ".wikisource.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("quote") != -1)
            {
                api = "https://" + SiteLink.Replace("wikiquote", "").Replace("_", "-") + ".wikiquote.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("wikivoyage") != -1)
            {
                api = "https://" + SiteLink.Replace("wikivoyage", "").Replace("_", "-") + ".wikivoyage.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("wikinews") != -1)
            {
                api = "https://" + SiteLink.Replace("wikinews", "").Replace("_", "-") + ".wikinews.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("wikibooks") != -1)
            {
                api = "https://" + SiteLink.Replace("wikibooks", "").Replace("_", "-") + ".wikibooks.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("wikiversity") != -1)
            {
                api = "https://" + SiteLink.Replace("wikiversity", "").Replace("_", "-") + ".wikiversity.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("specieswiki") != -1)
            {
                api = "https://species.wikimedia.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("commonswiki") != -1)
            {
                api = "https://commons.wikimedia.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("metawiki") != -1)
            {
                api = "https://meta.wikimedia.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("mediawikiwiki") != -1)
            {
                api = "https://mediawiki.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else if (SiteLink.IndexOf("wikidatawiki") != -1)
            {
                api = "https://www.wikidata.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            else
            {
                api = "https://" + SiteLink.Replace("wiki", "").Replace("_", "-") + ".wikipedia.org/w/api.php?action=query&prop=pageprops&format=json&ppprop=disambiguation&titles=" + WebUtility.UrlEncode(page);
            }
            WebRequest request = WebRequest.Create(api);
            WebResponse response = request.GetResponse();
            string tmp = ((HttpWebResponse)response).StatusDescription;

            Stream dataStream = response.GetResponseStream();  // Get the stream containing content returned by the server.
            StreamReader reader = new StreamReader(dataStream);  // Open the stream using a StreamReader for easy access.
            string responseFromServer = reader.ReadToEnd();  // Read the content.
            if (responseFromServer.IndexOf("{\"disambiguation\":\"\"}") != -1)
            {
                reader.Close();  // Clean up the streams and the response.
                response.Close();
                return true;
            }
            else
            {
                reader.Close();  // Clean up the streams and the response.
                response.Close();
                return false;
            }
        }

        /// <summary>Receive raw message error and return formatted error</summary>
        /// <param name="error">raw error</param>
        /// <returns>format error</returns>
        public static string CleanApiError(string error)
        {
            if (error.IndexOf("<message name=\"wikibase-validator-label-with-description-conflict\">") != -1)
            {
                Regex regex = new Regex(
                  "<\\?xml version=\"1\\.0\"\\?><api servedby=\"mw\\d\\d\\d\\d\"><error code=\"modification-failed\" info=\"Item \\[\\[" + 
                  "(Q\\d*)" + 
                  "(\\|Q\\d*\\]\\])" + 
                  "( already has label &quot;)" + 
                  "(.*)(&quot; associated with language code )" + 
                  "(.*)" + 
                  "(, using the same description text)",
                RegexOptions.CultureInvariant
                | RegexOptions.Compiled
                );
                Match ms = regex.Match(error);
                return "Same label/description" + '\t' + ms.Groups[1].Value + '\t' + ms.Groups[6].Value;
            }
            else if (error.IndexOf("<error code=\"maxlag\" info=\"Waiting for") != -1)
            {
                return "Maxlag";
            }
            else if (error.IndexOf("<error code=\"invalid-json\" info=\"Invalid json in request.") != -1)
            {
                return "Invalid json";
            }
            else if (error.IndexOf("<error code=\"readonly\" info=\"") != -1)
            {
                return "Read only";
            }
            else
            {
                return error;
            }
        }

        public static string CleanErrorLog(string ErrorList)
        {
            string resp = "";
            string[] lines = ErrorList.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Object locker = new Object();
            Parallel.For(0, lines.Count(), idx =>
            {
                string line = lines[idx];
                if (line.IndexOf("<message name=\"wikibase-validator-label-with-description-conflict\">") != -1)
                {
                    string[] tmp = line.Split('\t');
                    Regex regex = new Regex(
                      "(<\\?xml version=\"1\\.0\"\\?><api servedby=\"mw\\d\\d\\d\\d" +
                      "\"><error code=\"modification-failed\" info=\"Item \\[\\[)(Q" +
                      "\\d*)(\\|Q\\d*\\]\\])( already has label &quot;)(.*)(&quot; " +
                      "associated with language code )(.*)(, using the same descrip" +
                      "tion text)",
                    RegexOptions.CultureInvariant
                    | RegexOptions.Compiled
                    );
                    Match ms = regex.Match(tmp[2]);
                    lock (locker) { resp += tmp[0].Trim() + '\t' + tmp[1] + '\t' + ms.Groups[2].Value + '\t' + ms.Groups[7].Value + Environment.NewLine; }
                }
                else if (line.IndexOf("<error code=\"maxlag\" info=\"Waiting for") != -1)
                {
                    string[] tmp = line.Split('\t');
                    lock (locker) { resp += tmp[0].Trim() + '\t' + tmp[1] + '\t' + "Maxlag" + Environment.NewLine; }
                }
                else if (line.IndexOf("<error code=\"invalid-json\" info=\"Invalid json in request.") != -1)
                {
                    string[] tmp = line.Split('\t');
                    lock (locker) { resp += tmp[0].Trim() + '\t' + tmp[1] + '\t' + "Json format" + Environment.NewLine; }
                }
                else if (line.IndexOf("<error code=\"readonly\" info=\"") != -1)
                {
                    string[] tmp = line.Split('\t');
                    lock (locker) { resp += tmp[0].Trim() + '\t' + tmp[1] + '\t' + "Read only" + Environment.NewLine; }
                }
                else
                {
                    lock (locker) { resp += line + Environment.NewLine; }
                }
            });
            return resp;
        }

        /// <summary></summary>
        /// <param name="strList"></param>
        /// <param name="strSite"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string SitelinkFromItem(string strList, string strSite, string user, string pwd)
        {
            List<string> list = Utility.SplitInChunk(strList, Environment.NewLine, 500);

            string res = "";
            Site WD = new Site("https://www.wikidata.org", user, pwd);
            Object locker = new Object();
            foreach (string s in list)
            {
                string strJson = WD.LoadWD(s);
                Entities EntityList = JsonConvert.DeserializeObject<Entities>(strJson, new DatavalueConverter());
                if (EntityList.entities != null)
                {
                    Parallel.ForEach(EntityList.entities.Values, entity =>
                    {
                        foreach (SiteLink sl in entity.sitelinks.Values)
                        {
                            if (sl.site==strSite)
                            {
                                lock (locker) { res += entity.id + '\t' + sl.title + Environment.NewLine; }
                            }
                        }
                    });
                }
            }
            return res;
        }

        public static Dictionary<string, string> AWBtypos(string user, string pwd)
        {
            Site WP = new Site("https://it.wikipedia.org", user, pwd);
            string strJson = WP.LoadWP(@"Wikipedia:AutoWikiBrowser/Typos");
            Pages pages = JsonConvert.DeserializeObject<Pages>(strJson, new DatavalueConverter());
            string text = pages.query.FirstPageText;
            int Da = text.IndexOf("<source lang=\"xml\" enclose=div>") + 32;
            text = text.Substring(Da);
            string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Regex regex = new Regex("(<Typo word=\")(.*)(\" find=\")(.*)(\" replace=\")(.*)(\" />)", RegexOptions.CultureInvariant | RegexOptions.Compiled );
            Dictionary<string, string> tmpDic = new Dictionary<string, string>();
            for (int idx = 0; idx < lines.Count(); idx++)
            {
                MatchCollection matches = Regex.Matches(lines[idx], "(<Typo word=\")(.*)(\" find=\")(.*)(\" replace=\")(.*)(\" />)");
                foreach (Match match in matches)
                {
                    tmpDic.Add(match.Groups[4].Value, match.Groups[6].Value);
                }       
            }
            return tmpDic;
        }

        /// <summary>Get Wikimedia sites list</summary>
        /// <returns>String tab separated of all the site of Wikimedia, one site for row</returns>
        public static string SiteMatrix()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "User:ValterVB");
            Stream data = client.OpenRead("https://it.wikipedia.org/w/api.php?action=sitematrix&format=xml&smstate=nonglobal%7Call&smlangprop=code%7Cname%7Csite%7Clocalname&smsiteprop=url%7Cdbname%7Ccode%7Csitename");
            StreamReader reader = new StreamReader(data);
            string result = reader.ReadToEnd();
            data.Close();
            reader.Close();

            string ret = "";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            foreach (XmlNode node in doc.SelectNodes("/api/sitematrix/specials/special"))
            {
                if (node.Attributes["closed"]==null && node.Attributes["private"]==null)
                {
                    string lang = "";
                    string langName = "";
                    string dbname = node.Attributes["dbname"].Value;
                    string sitename = node.Attributes["sitename"].Value;
                    string url = node.Attributes["url"].Value;
                    ret += lang + '\t' + langName + '\t' + dbname + '\t' + sitename + '\t' + url + Environment.NewLine;
                }
            }

            foreach (XmlNode node in doc.SelectNodes("/api/sitematrix/language"))
            {
                string lang = node.Attributes["code"].Value;
                string langName = node.Attributes["localname"].Value;
                foreach (XmlNode node1 in node.SelectNodes("site/site"))
                {
                    if (node1.Attributes["closed"] == null && node1.Attributes["private"] == null)
                    {
                        string dbname = node1.Attributes["dbname"].Value;
                        string sitename = node1.Attributes["sitename"].Value;
                        string url = node1.Attributes["url"].Value;
                        ret += lang + '\t' + langName + '\t' + dbname + '\t' + sitename + '\t' + url + Environment.NewLine;
                    }
                }
            }
            return ret;
        }
    }
}
