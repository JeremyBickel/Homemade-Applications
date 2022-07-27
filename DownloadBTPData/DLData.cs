using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace DownloadBTPData
{
    public class DLData
    {
        public void BibleTruthPublishers()
        {
            //            if you use ClientIdMode = "Static" on the control you're trying to simulate a click on, you can just drop the actual ID name in there: <asp:linkbutton etc etc ClientIdMode="Static" id="myLink"> and in the JS: __doPostBack("myLink", "");

            //_____________

            //eachtype => < span id = "ctl00_ctl00_cphLibSiteMasterPgMainContent_cphLibListPageBody_ctl00_ctl00_lblTitle>...TYPE...</span>
            //eachitem => < a id = "ctl00_ctl00_cphLibSiteMasterPgMainContent_cphLibListPageBody_ctl00_ctl01_lnkTitle" href = "...ITEM..." > ...NAME...</ a >
            //getitem => javascript: __doPostBack('ctl00$ctl00$cphLibSiteMasterPgMainContent$cphLibListPageBody$ctl00$wscLibBookShareOptions$lnkBookRTF', '')

            StreamReader srBTPLinks = new StreamReader(@"Data\BibleTruthPublishersLinks.txt");
            StreamWriter swAuthors = new StreamWriter(@"Data\BibleTruthPublishersAuthors.txt");

            var baseAddress = new Uri("https://bibletruthpublishers.com");
            Random rndSleep = new Random();

            if (!Directory.Exists(@"Data\BibleTruthPublishers"))
            {
                Directory.CreateDirectory(@"Data\BibleTruthPublishers");
            }

            while (!srBTPLinks.EndOfStream)
            {
                string strLine = srBTPLinks.ReadLine().Trim();

                if (strLine.StartsWith("<a href="))
                {
                    string strName = "";
                    string strHREF = "";
                    string strResult = "";
                    string strOutputFilename = "";

                    strHREF = strLine.Substring(9).Trim();
                    strHREF = strHREF.Substring(0, strHREF.IndexOf('"'));

                    strName = strLine.Substring(strLine.IndexOf('>') + 1).Trim();
                    strName = strName.Remove(strName.Length - 4);

                    strOutputFilename = System.Text.RegularExpressions.Regex.Replace(
                        System.Text.RegularExpressions.Regex.Replace(
                            strName,
                            @"[\. ]", ""),
                        @",", "_") + @".txt";

                    if (!Directory.Exists(@"Data\BibleTruthPublishers\" + strName))
                    {
                        Directory.CreateDirectory(@"Data\BibleTruthPublishers\" + strName);
                    }

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                    startInfo.WorkingDirectory = @"Data\BibleTruthPublishers\" + strName;
                    //startInfo.RedirectStandardOutput = true;
                    //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                    startInfo.FileName = @"C:\Windows\system32\cmd.exe";
                    //startInfo.UseShellExecute = false;
                    startInfo.Arguments = @"/C c:\users\jeremy\Downloads\wget.exe --output-document=" + strOutputFilename + " " + baseAddress + strHREF.TrimStart('/');
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    process.Dispose();

                    Thread.Sleep(rndSleep.Next(1, 3) * 1000); //wait 1-3 seconds, so 1) the server isn't hammered, and 2) it's random to hedge against AIs shutting it down due to automation

                    StreamReader srThisAuthor = new StreamReader(@"Data\BibleTruthPublishers\" + strName + @"\" + strOutputFilename);
                    string strCurrentType = "";
                    string strCurrentWebAddress = "";
                    string strCurrentWorkName = "";
                    string strCurrentWorkingDirectory = "";

                    while (!srThisAuthor.EndOfStream)
                    {
                        string strAuthorLine = srThisAuthor.ReadLine();

                        if (strAuthorLine.Contains("lblTitle"))
                        {
                            if (!strAuthorLine.Contains("lblTitleSuffix"))
                            {
                                strCurrentType = strAuthorLine.Substring(0, strAuthorLine.Length - 7);
                                strCurrentType = strCurrentType.Substring(strCurrentType.LastIndexOf('>') + 1);
                                strCurrentWorkingDirectory = @"Data\BibleTruthPublishers\" + strName + @"\" + System.Text.RegularExpressions.Regex.Replace(strCurrentType, @"[ ]", "");

                                if (!Directory.Exists(strCurrentWorkingDirectory))
                                {
                                    Directory.CreateDirectory(strCurrentWorkingDirectory);
                                }
                            }
                        }
                        else if (strAuthorLine.Contains("lnkTitle"))
                        {
                            strCurrentWebAddress = strAuthorLine.Substring(0, strAuthorLine.Length - 4);
                            strCurrentWebAddress = strCurrentWebAddress.Substring(strCurrentWebAddress.IndexOf("href=") + 5);
                            strCurrentWorkName = strCurrentWebAddress.Substring(strCurrentWebAddress.IndexOf('>') + 1);
                            strCurrentWebAddress = strCurrentWebAddress.Substring(0, strCurrentWebAddress.LastIndexOf('"')).TrimStart('"').TrimStart('/');
                            strCurrentWebAddress = baseAddress.ToString() + strCurrentWebAddress;


                        }
                    }
                }
            }

            swAuthors.Close();
            srBTPLinks.Close();
        }

        public async Task Go()
        {
            using var playwright = await Playwright.CreateAsync();
            Random rndSleep = new Random();
            StreamWriter swManualDownloads = new StreamWriter(@"Data\ManualDownloads.txt", true);

            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });

            var context = await browser.NewContextAsync();

            // Page 1
            var page = await context.NewPageAsync();

            page.SetDefaultTimeout(40000); // 40 seconds

            await page.GotoAsync("https://bibletruthpublishers.com/all-library-authors/lua");

            //wait 1-3 seconds, so 1) the server isn't hammered, and 2) it's random to hedge against
            //AIs shutting it down due to automation
            Thread.Sleep(rndSleep.Next(1, 3) * 1177);

            // Click text=strAuthorName
            //await page.Locator("text=" + strAuthorName).ClickAsync();
            int intAuthorCount = await page.Locator("div.subsection-cont > div.author").CountAsync();

            for (int intAuthorCounter = 51; intAuthorCounter <= intAuthorCount; intAuthorCounter++)
            {
                string strDirectory = "";

                var strHREF = await page.Locator("div.subsection-cont:nth-child(" + intAuthorCounter.ToString() + ") > div.author > a").GetAttributeAsync("href");
                string strAuthorName = await page.Locator("div.subsection-cont:nth-child(" + intAuthorCounter.ToString() + ") > div.author > a").InnerHTMLAsync();

                //Page 2
                var contextWorks = await browser.NewContextAsync();
                var pageWorks = await contextWorks.NewPageAsync();

                page.SetDefaultTimeout(40000); // 40 seconds

                await pageWorks.GotoAsync("https://bibletruthpublishers.com" + strHREF);

                pageWorks.WaitForLoadStateAsync(); //wait for the new page to load

                //wait 1-3 seconds, so 1) the server isn't hammered, and 2) it's random to hedge against
                //AIs shutting it down due to automation
                Thread.Sleep(rndSleep.Next(1, 3) * 1262);

                int intTypeCount = await pageWorks.Locator("div.title > span").CountAsync(); //Work Type Heading
                var strLastTypeID = "";
                var strTypeID = "";
                string strType = "";
                string strLastType = "";
                string strWorksIDFirstPart = "";
                string strIDNumber = "";
                string strWorksIDLastPart = "";
                int intIDNumber = -1;

                if (intTypeCount > 0)
                {
                    int intTypedWorkCount = -1;

                    //intTypeCounter, for Locator.Nth, is 0 - based
                    //intTypeCount is 1 - based
                    for (int intTypeCounter = 0; intTypeCounter < intTypeCount; intTypeCounter++)
                        {
                            strType = await pageWorks.Locator("div.title > span").Nth(intTypeCounter).InnerTextAsync();
                            strTypeID = await pageWorks.Locator("div.title > span").Nth(intTypeCounter).GetAttributeAsync("id");

                            if (!Regex.IsMatch(strTypeID, @"^(?<first>.*ctl)(?<id>[0-9]{1,2})_lblTitle$"))
                            {
                                throw new Exception("Check the WorksID for" + strHREF);
                            }

                            strWorksIDFirstPart = Regex.Match(strTypeID, @"^(?<first>.*ctl)(?<id>[0-9]{1,2}).*$").Groups["first"].Value;
                            strIDNumber = Regex.Match(strTypeID, @"^(?<first>.*ctl)(?<id>[0-9]{1,2}).*$").Groups["id"].Value;
                            strWorksIDLastPart = "_lnkTitle";

                            //ctl00_ctl00_cphLibSiteMasterPgMainContent_cphLibListPageBody_ctl00_ctl00_lblTitle

                            if (intTypeCounter > 0) //start with the second Work Type Heading in order to search between headings for Work Items
                            {
                                intTypedWorkCount = await pageWorks.Locator("div.title:below(#" + strLastTypeID + "):above(#" + strTypeID + ") > a").CountAsync();
                                intIDNumber = Convert.ToInt32(strIDNumber);

                                for (int intTypedWorkCounter = intIDNumber + 1; intTypedWorkCounter <= intIDNumber + intTypedWorkCount; intTypedWorkCounter++)
                                {
                                    try
                                    {
                                        string strWorksIDComplete = strWorksIDFirstPart + intTypedWorkCounter.ToString().PadLeft(2, '0') +
                                            strWorksIDLastPart;
                                        string strWorkName = await pageWorks.Locator("#" + strWorksIDComplete).InnerTextAsync();

                                        //Page 3
                                        int intWorkNameCount = await pageWorks.Locator("text=" + strWorkName).CountAsync();

                                        for (int intWorkNameCounter = 0; intWorkNameCounter < intWorkNameCount; intWorkNameCounter++)
                                        {
                                            //await pageWorks.Locator("text=" + strWorkName).Nth(intWorkNameCounter).ClickAsync();
                                            await pageWorks.Locator("#" + strWorksIDComplete).ClickAsync();
                                            pageWorks.WaitForLoadStateAsync();
                                            Thread.Sleep(rndSleep.Next(3, 5) * 1635);
                                            // Click text=Download RTF (editable)
                                            //await pageWorks.HoverAsync("#ctl00_ctl00_cphLibSiteMasterPgMainContent_cphLibListPageBody_ctl00_wscLibBookShareOptions_lnkDdlHdg");
                                            await pageWorks.Locator("text= download …").HoverAsync();
                                            Thread.Sleep(1000);

                                            pageWorks.WaitForLoadStateAsync(); //wait for the new page to load
                                            Thread.Sleep(rndSleep.Next(2, 6) * 1934);

                                            try
                                            {
                                                var download1 = await pageWorks.RunAndWaitForDownloadAsync(async () =>
                                                 {
                                                     await pageWorks.Locator("text=Download RTF (editable)").ClickAsync();
                                                 });

                                                string strSameWorkNameSuffix = "";

                                                if (intWorkNameCounter > 0)
                                                {
                                                    strSameWorkNameSuffix = "-" + intWorkNameCounter.ToString();
                                                }

                                                //Save downloaded file
                                                strDirectory =
                                                    Regex.Replace(
                                                        Regex.Replace(
                                                            strAuthorName
                                                            , @"[\. ]"
                                                            , "")
                                                        , @","
                                                        , "_"
                                                    ) + @"\" + strLastType + @"\";

                                                if (!Directory.Exists(strDirectory))
                                                {
                                                    Directory.CreateDirectory(strDirectory);
                                                }

                                                await download1.SaveAsAsync(@"Data\"
                                                    + strDirectory
                                                    + download1.SuggestedFilename);
                                            }
                                            catch (Exception e)
                                            {
                                                string a = e.Message;
                                                swManualDownloads.WriteLine(pageWorks.Url);
                                            }


                                            pageWorks.GoBackAsync();
                                            pageWorks.WaitForLoadStateAsync();

                                            Thread.Sleep(rndSleep.Next(2, 5) * 2429);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        string a = e.Message;
                                    }
                                }

                            }

                            strLastType = strType;
                            strLastTypeID = strTypeID;
                        }

                    //now search below the last Work Type Heading for the remaining Work items

                    strType = await pageWorks.Locator("div.title > span").Nth(intTypeCount - 1).InnerTextAsync();
                    strTypeID = await pageWorks.Locator("div.title > span").Nth(intTypeCount - 1).GetAttributeAsync("id");

                    if (!Regex.IsMatch(strTypeID, @"^(?<first>.*ctl)(?<id>[0-9]{1,2})_lblTitle$"))
                    {
                        throw new Exception("Check the WorksID for" + strHREF);
                    }

                    strWorksIDFirstPart = Regex.Match(strTypeID, @"^(?<first>.*ctl)(?<id>[0-9]{1,2}).*$").Groups["first"].Value;
                    strIDNumber = Regex.Match(strTypeID, @"^(?<first>.*ctl)(?<id>[0-9]{1,2}).*$").Groups["id"].Value;
                    strWorksIDLastPart = "_lnkTitle";

                    //ctl00_ctl00_cphLibSiteMasterPgMainContent_cphLibListPageBody_ctl00_ctl00_lblTitle

                    //using strTypeID here instead of strLastTypeID should make this work in the last type whether or not there are multiple types
                    intTypedWorkCount = await pageWorks.Locator("div.title:below(#" + strTypeID + ") > a").CountAsync();

                    intIDNumber = Convert.ToInt32(strIDNumber);
                    
                    for (int intTypedWorkCounter = intIDNumber + 1; intTypedWorkCounter <= intIDNumber + intTypedWorkCount; intTypedWorkCounter++)
                    {
                        try
                        {
                            string strWorksIDComplete = strWorksIDFirstPart + intTypedWorkCounter.ToString().PadLeft(2, '0') +
                                strWorksIDLastPart;
                            string strWorkName = await pageWorks.Locator("#" + strWorksIDComplete).InnerTextAsync();

                            //Page 3
                            int intWorkNameCount = await pageWorks.Locator("text=" + strWorkName).CountAsync();

                            for (int intWorkNameCounter = 0; intWorkNameCounter < intWorkNameCount; intWorkNameCounter++)
                            {
                                await pageWorks.Locator("#" + strWorksIDComplete).ClickAsync();
                                pageWorks.WaitForLoadStateAsync();
                                Thread.Sleep(rndSleep.Next(3, 5) * 1635);
                                // Click text=Download RTF (editable)
                                await pageWorks.Locator("text= download …").HoverAsync();
                                Thread.Sleep(1000);

                                pageWorks.WaitForLoadStateAsync(); //wait for the new page to load
                                Thread.Sleep(rndSleep.Next(2, 6) * 1934);
                                try
                                {
                                    var download1 = await pageWorks.RunAndWaitForDownloadAsync(async () =>
                                    {
                                        await pageWorks.Locator("text=Download RTF (editable)").ClickAsync();
                                    });

                                    string strSameWorkNameSuffix = "";

                                    if (intWorkNameCounter > 0)
                                    {
                                        strSameWorkNameSuffix = "-" + intWorkNameCounter.ToString();
                                    }

                                    //Save downloaded file
                                    strDirectory =
                                        Regex.Replace(
                                            Regex.Replace(
                                                strAuthorName
                                                , @"[\. ]"
                                                , "")
                                            , @","
                                            , "_"
                                        ) + @"\" + strType + @"\";

                                    if (!Directory.Exists(strDirectory))
                                    {
                                        Directory.CreateDirectory(strDirectory);
                                    }

                                    await download1.SaveAsAsync(@"Data\"
                                        + strDirectory
                                        + download1.SuggestedFilename);

                                }
                                catch (Exception e)
                                {
                                    string a = e.Message;
                                    swManualDownloads.WriteLine(pageWorks.Url);
                                }

                                pageWorks.GoBackAsync();
                                pageWorks.WaitForLoadStateAsync();

                                Thread.Sleep(rndSleep.Next(2, 5) * 2429);
                            }
                        }
                        catch (Exception e)
                        {
                            string a = e.Message;
                        }
                    }
                }
                else //If no Work Type Headings exist
                {
                    string strBreak = "";
                }

                await pageWorks.CloseAsync();
            }

            swManualDownloads.Close();
            await context.CloseAsync();
        }
    }
}