using System.Text.RegularExpressions;
using System.Text;

namespace WordTactics
{
    internal class OWLDB
    {
        Dictionary<int, DL> dDLs = new Dictionary<int, DL>(); //D<OrderID, DL>
        private static readonly HttpClient client = new HttpClient();

        public OWLDB()
        {
            client.Timeout = TimeSpan.FromSeconds(300);

            //the bool parameter ensures that this is only downloaded once,
            //or set it to true and adjust the code in the Go function to start downloading from any point in the files hierarchy
            //Download(false);
            //DBToClasses();
        }

        public async void Download(bool bContinue = false)
        {
            StreamReader srDLHierarchy = new StreamReader(@"DLHierarchy.txt");
            Regex rgxURL = new Regex(@"href=""(?<url>[^""]{1,})"">(?<name>[^<]{1,})");
            string strBaseDirectory = Directory.GetCurrentDirectory() + "\\";
            StreamWriter swHierarchy = new StreamWriter(strBaseDirectory + "\\OWLHierarchy.csv");
            int intDLOrderID = 0;

            foreach (Match mURL in rgxURL.Matches(srDLHierarchy.ReadToEnd())) //href="general_writing/index.html">General Writing Introduction</a>
            {
                string strURL = mURL.Groups["url"].Value;
                string strName = "";
                string strFilename = strURL.Substring(strURL.LastIndexOf('/') + 1);
                Dictionary<int, string> dHierarchy = new Dictionary<int, string>(); //D<level, name>; example: <level=1, name="general_writing">, <level=2, name=...>
                int intLevelCounter = 0;
                int intLastSlash = strURL.LastIndexOf('/');

                foreach (string strLevelPart in strURL.Substring(0, intLastSlash).Split('/'))
                {
                    intLevelCounter++;

                    dHierarchy.Add(intLevelCounter, strLevelPart.Trim());
                }


                strName = mURL.Groups["name"].Value;

                DL dL = new DL();
                dL.strURL = @"https://owl.purdue.edu/owl/" + strURL;
                dL.strName = strName;
                dL.strFilename = strFilename;
                dL.dHierarchy = dHierarchy;

                intDLOrderID++;

                dDLs.Add(intDLOrderID, dL);
            }

            foreach (int intOrderID in dDLs.Keys.OrderBy(a => a))
            {
                string strHierarchy = "";
                string strFullDirectory = "";

                foreach (int intHierarchyLevel in dDLs[intOrderID].dHierarchy.Keys.OrderBy(a => a))
                {
                    strHierarchy += dDLs[intOrderID].dHierarchy[intHierarchyLevel] + @"\";
                }

                strFullDirectory = strBaseDirectory + strHierarchy;

                if (!Directory.Exists(strFullDirectory))
                {
                    Directory.CreateDirectory(strFullDirectory);
                }

                //
                //bContinue Adjustment
                //
                //if (strFullDirectory == "C:\\Users\\Jeremy\\source\\repos\\Homemade Applications\\WordTactics\\bin\\Debug\\net6.0-windows\\general_writing\\grammar\\pronouns\\"
                //    && dL.strFilename == "index.html")
                //{
                //    bContinue = true;
                //}

                if (bContinue == true)
                {
                    await Download_Task(dDLs[intOrderID].strURL, strFullDirectory + dDLs[intOrderID].strFilename);
                }
            }

            foreach (int intOrderID in dDLs.Keys.OrderBy(a => a))
            {
                swHierarchy.Write(dDLs[intOrderID].strName + " ^ ");

                foreach (int intHierarchyLevel in dDLs[intOrderID].dHierarchy.Keys.OrderBy(a => a))
                {
                    swHierarchy.Write(dDLs[intOrderID].dHierarchy[intHierarchyLevel] + "\\");
                }

                swHierarchy.WriteLine(dDLs[intOrderID].strFilename);
            }

            swHierarchy.Close();
        }

        private static async Task Download_Task(string strURL, string strFilename)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            ////client.DefaultRequestHeaders.Accept.Add(
            ////    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", "This");

            var stringTask = client.GetStringAsync(strURL);

            var msg = await stringTask;

            File.WriteAllText(strFilename, msg);

            //using (HttpResponseMessage response = client.GetAsync(url).Result)
            //{
            //    using (HttpContent content = response.Content)
            //    {
            //        string result = content.ReadAsStringAsync().Result;
            //    }
            //}
        }

        public void DBToClasses()
        {
            StreamReader srOWLHierarchy = new StreamReader(@"OWLHierarchy.csv");
            string strBaseDirectory = Directory.GetCurrentDirectory();
            Regex rgxGetDataStart = new Regex(@"<div class=""printarea"">(?<data>.*)", RegexOptions.Singleline);

            while (!srOWLHierarchy.EndOfStream)
            {
                string strLine = srOWLHierarchy.ReadLine();
                string[] strsLine = strLine.Split('^'); //Name ^ Relative Location
                string strNewDirectory = "";

                strsLine[0] = strsLine[0].Trim();
                strsLine[1] = strsLine[1].Trim();
                strNewDirectory = @"NewClasses\" + strsLine[1].Substring(0, strsLine[1].LastIndexOf('\\'));

                if (!Directory.Exists(strNewDirectory))
                {
                    Directory.CreateDirectory(strNewDirectory);
                }

                StreamReader srData = new StreamReader(strBaseDirectory + @"\" + strsLine[1].Trim());
                Match mPrintArea = rgxGetDataStart.Match(srData.ReadToEnd());

                if (mPrintArea.Success)
                {
                    string strData = mPrintArea.Groups["data"].Value;
                    int intDivCounter = 0;
                    string strDataCleaned = "";

                    //remember that the "<div" this splits on is at the end of the text segment
                    foreach (string strDataPart in strData.Split("<div"))
                    {
                        intDivCounter++; //bank the last "<div"

                        if (strDataPart.Contains("</div>"))
                        {
                            for (int intCloseDivCounter = 1; intCloseDivCounter <= Regex.Matches(strDataPart, @"</div>").Count; intCloseDivCounter++)
                            {
                                intDivCounter--;
                            }

                            //foreach (string strCloseDiv in strDataPart.Split("</div>"))
                            //{
                            //    intDivCounter--;
                            //}
                        }

                        if (intDivCounter == 0)
                        {
                            break;
                        }

                        strDataCleaned += "<div" + strDataPart + " ";
                    }

                    strDataCleaned = Regex.Replace(strDataCleaned, @"</{0,1}.*?>", "").Replace(@"<div", "").Trim();

                    //Create Class Stub from strDataCleaned
                    DataToClass(strDataCleaned, strsLine);
                }
            }
        }

        public void DataToClass(string strData, string[] strsMetadata)
        {
            StreamWriter swClass = new StreamWriter(@"NewClasses\" + strsMetadata[1].Substring(0, strsMetadata[1].LastIndexOf('.')) + @".cs");

            StringBuilder sbTemplate = new StringBuilder();
            sbTemplate.AppendLine("namespace WordTactics");
            sbTemplate.AppendLine("{");
            sbTemplate.AppendLine("internal class " + Regex.Replace(strsMetadata[0], @"[^\w]{1,}", ""));
            sbTemplate.AppendLine("{");

            foreach (string strLine in strData.Split('\n'))
            {
                sbTemplate.AppendLine(@"//" + strLine);
            }

            sbTemplate.AppendLine("}");
            sbTemplate.AppendLine("}");

            swClass.Write(sbTemplate.ToString());
            swClass.Close();
        }
    }

    //Helper class for Download
    public class DL
    {
        public string strURL = "";
        public string strName = "";
        public string strFilename = "";
        public Dictionary<int, string> dHierarchy = new Dictionary<int, string>();
    }
}
