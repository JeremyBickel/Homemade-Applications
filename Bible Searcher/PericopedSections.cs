using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class PericopedSections
    {
        public Dictionary<int, PericopeData> dPericopes = new Dictionary<int, PericopeData>();
        public Regex rgxNumber = new Regex(@"^[^0-9]{0,}[0-9]{1,}[^0-9]{0,}$");
        public Regex rgxLetter = new Regex(@"^[^a-hj-z]{0,}[a-hj-z]{1,}[^a-hj-z]{0,}$"); //no i, which is roman numeral
        public Regex rgxRomanNumeral = new Regex(@"^[^i]{0,}[i]{1,}[^i]{0,}$");
        List<string> lHeadings = new List<string>();

        public class PericopeData
        {
            public int intID = 0; //unique to the whole Bible
            
            //The following reference information is the start and end of the referenced passage
            public string strBookName = "";
            
            public int intStartChapterNumber = 0;
            public int intStartVerseNumber = 0;
            
            public int intEndChapterNumber = 0;
            public int intEndVerseNumber = 0;
            
            public string strStructure = ""; //outline ID unique to the whole book
            
            public string strPericope1Number = ""; //First Number Heading
            public string strPericopeLetter = ""; //Letter Heading
            public string strPericopeRomanNumeral = ""; //Roman Numeral Heading
            public string strPericope2Number = ""; //Second Number Heading
        }

        public void ReadData()
        {
            StreamReader srWholeBiblePericope = new StreamReader(@"Data\Pericopes\Whole Bible.txt"); //The whole Bible, from Berean Bible
            string strBookName = ""; //current Bible book name
            string strLastStructureElement = ""; //the last single element structure
            string strCurrentNumber1 = ""; //track the single elements of the current structure
            string strCurrentLetter = ""; //track the single elements of the current structure
            string strCurrentRomanNumeral = ""; //track the single elements of the current structure
            string strCurrentNumber2 = ""; //track the single elements of the current structure
            string strCurrentUnknown = ""; //track the single elements of the current structure
            int intPericopeID = 0;
            int intLineNumber = 0;

            while (!srWholeBiblePericope.EndOfStream)
            {
                string strLine = srWholeBiblePericope.ReadLine().Trim();

                if (intLineNumber > 0)
                {
                    if (strLine != "") //skip empty lines
                    {
                        string[] strsWords = strLine.Split();
                        bool bBookName = false;
                        string strReference = "";
                        string strPericope = "";
                        string strStructureElement = ""; //single element structure

                        if (!strsWords[0].EndsWith('.')) //Bible book name
                        {
                            bBookName = true;
                            strBookName = strLine;

                            //Normalize strBookName to the book names in the Verse objects (from kjvstrongs.csv)
                            strBookName = strBookName.Replace(" ", "").ToLower();
                        }
                        else //outline data
                        {
                            bBookName = false; 
                            intPericopeID++;

                            for (int intWordCounter = 0; intWordCounter < strsWords.Length; intWordCounter++)
                            {
                                string strWord = strsWords[intWordCounter];

                                if (intWordCounter == 0) //data structure ID
                                {
                                    strStructureElement = strWord.TrimEnd('.');
                                }
                                else if (strWord.StartsWith("(")) //reference data has no spaces, so it's all one word here
                                {
                                    strReference = strWord.TrimStart('(').TrimEnd(')');
                                }
                                else
                                {
                                    strPericope += strWord + " ";
                                }
                            }
                        }

                        if (bBookName == false)
                        {
                            //parse data to class
                            PericopeData pdData = new PericopeData();
                            Dictionary<int, string> dReference = new Dictionary<int, string>(); //D<order id, reference element>; Example for 1 Chronicles 1:2 - D<1, "1">, D<2, "Chronicles">, D<3, "1">, D<4, ":">, D<5, "2"> 
                            bool bDash = false; //have we seen a dash between references yet?
                            bool bVerse = false; //is this number a verse number?
                            string strLastChapterNumber = "";
                            string[] strsStructureData;

                            pdData.intID = intPericopeID;
                            pdData.strBookName = strBookName;

                            //a composite structure based on the single element structures
                            strsStructureData = NextStructureID(strStructureElement, strLastStructureElement, ref strCurrentNumber1,
                                ref strCurrentLetter, ref strCurrentRomanNumeral, ref strCurrentNumber2, ref strCurrentUnknown);
                            pdData.strStructure = strsStructureData[0];

                            switch (strsStructureData[1])
                            {
                                case "First Number":
                                    pdData.strPericope1Number = strPericope.Trim();
                                    break;
                                case "Letter":
                                    pdData.strPericopeLetter = strPericope.Trim();
                                    break;
                                case "Roman Numeral":
                                    pdData.strPericopeRomanNumeral = strPericope.Trim();
                                    break;
                                case "Second Number":
                                    pdData.strPericope2Number = strPericope.Trim();
                                    break;
                                default:
                                    throw new Exception("Error in Pericope Heading Type");
                            }

                            strLastStructureElement = strStructureElement;

                            //reference data
                            foreach (string strReferencePart in strReference.Split("-"))
                            {
                                int intChapterNumber = 0;
                                int intVerseNumber = 0;
                                string strChapterNumberBuilder = "";
                                string strVerseNumberBuilder = "";

                                bVerse = false;
                            
                                foreach (char ch in strReferencePart) //this loop goes character by character in the space-separated part of the reference
                                {
                                    if (!strReferencePart.Contains(':')) //a single number is a chapter or verse depending on whether it's before or after the dash
                                    {
                                        if (bDash == true)
                                        {
                                            bVerse = true;
                                        }
                                    }

                                    if (ch == ':')
                                    {
                                        bVerse = true;
                                    }
                                    else if (rgxNumber.IsMatch(ch.ToString()))
                                    {
                                        if (bVerse == false) //chapter number
                                        {
                                            strChapterNumberBuilder += ch;
                                        }
                                        else //verse number
                                        {
                                            strVerseNumberBuilder += ch;
                                        }
                                    }
                                    else 
                                    {
                                        throw new Exception("ERROR splitting strReference on dash");
                                    }
                                }

                                if (strChapterNumberBuilder == "")
                                {
                                    strChapterNumberBuilder = strLastChapterNumber;
                                }

                                if (strVerseNumberBuilder == "")
                                {
                                    strVerseNumberBuilder = "1";
                                }

                                intVerseNumber = Convert.ToInt32(strVerseNumberBuilder);
                                intChapterNumber = Convert.ToInt32(strChapterNumberBuilder);

                                strLastChapterNumber = strChapterNumberBuilder;

                                if (bDash == false)
                                {
                                    pdData.intStartChapterNumber = intChapterNumber;
                                    pdData.intStartVerseNumber = intVerseNumber;
                                }
                                else
                                {
                                    pdData.intEndChapterNumber = intChapterNumber;
                                    pdData.intEndVerseNumber = intVerseNumber;
                                }

                                bDash = true;
                            }

                            dPericopes.Add(intPericopeID, pdData);
                        }
                    }
                }

                intLineNumber++;
            }

            srWholeBiblePericope.Close();
        }

        public string[] NextStructureID(string strIncomingStructureElement, string strLastStructureElement,
            ref string strCurrentNumber1, ref string strCurrentLetter, ref string strCurrentRomanNumeral, 
            ref string strCurrentNumber2, ref string strCurrentUnknown)
        {
            // Number, lowercase letter, roman numeral, number
            string[] strsReturn = (string[]) Array.CreateInstance(typeof(string), 2);

            if (rgxNumber.IsMatch(strIncomingStructureElement))
            {
                if (rgxRomanNumeral.IsMatch(strLastStructureElement) && Convert.ToInt32(strIncomingStructureElement) > 1) //add after the roman numeral (strCurrentNumber2)
                {
                    strCurrentNumber2 = strIncomingStructureElement;
                    strCurrentUnknown = "";
                    strsReturn[1] = "Second Number";
                }
                else //1 && after a roman numeral OR >1 && not after a roman numeral
                {
                    strCurrentNumber1 = strIncomingStructureElement;
                    strCurrentLetter = "";
                    strCurrentRomanNumeral = "";
                    strCurrentNumber2 = "";
                    strCurrentUnknown = "";
                    strsReturn[1] = "First Number";
                }
            }
            else if (rgxLetter.IsMatch(strIncomingStructureElement))
            {
                strCurrentLetter = strIncomingStructureElement;
                strCurrentRomanNumeral = "";
                strCurrentNumber2 = "";
                strCurrentUnknown = "";
                strsReturn[1] = "Letter";
            }
            else if (rgxRomanNumeral.IsMatch(strIncomingStructureElement))
            {
                strCurrentRomanNumeral = strIncomingStructureElement;
                strCurrentNumber2 = "";
                strCurrentUnknown = "";
                strsReturn[1] = "Roman Numeral";
            }
            else
            {
                //throw new Exception("ERROR finding Structure ID");
                strCurrentUnknown = strIncomingStructureElement;

                string strBreakHereForTesting = "";
            }

            strsReturn[0] = strCurrentNumber1 + strCurrentLetter + strCurrentRomanNumeral + strCurrentNumber2 + strCurrentUnknown;

            return strsReturn;
        }

        public List<string> GetHeadings()
        {
            foreach (PericopeData pdData in dPericopes.Values)
            {
                if (pdData.strPericope1Number != "")
                {
                    lHeadings.Add(pdData.strPericope1Number);
                }

                if (pdData.strPericopeLetter != "")
                {
                    lHeadings.Add(pdData.strPericopeLetter);
                }

                if (pdData.strPericopeRomanNumeral != "")
                {
                    lHeadings.Add(pdData.strPericopeRomanNumeral);
                }

                if (pdData.strPericope2Number != "")
                {
                    lHeadings.Add(pdData.strPericope2Number);
                }
            }

            return lHeadings;
        }

        public List<string> SearchHeadings(List<string> lSearchTerms, bool bAny = true) //if bAny == false, then require all the search terms to be present to include a heading
        {
            List<string> lReturn = new List<string>();

            if (bAny == true)
            {
                foreach (string strSearchTerm in lSearchTerms)
                {
                    foreach (string strHeading in lHeadings)
                    {
                        if (strHeading.Contains(strSearchTerm, StringComparison.OrdinalIgnoreCase))
                        {
                            lReturn.Add(strHeading);
                        }
                    }
                }
            }
            else
            {   
                foreach (string strHeading in lHeadings) 
                {
                    int intFoundTerms = 0;

                    foreach (string strSearchTerm in lSearchTerms)
                    {
                        if (strHeading.Contains(strSearchTerm, StringComparison.OrdinalIgnoreCase))
                        {
                            intFoundTerms++;
                        }
                    }

                    if (lSearchTerms.Count == intFoundTerms)
                    {
                        lReturn.Add(strHeading);
                    }
                }
            }

            return lReturn;
        }

        public PericopeData GetPericopeDataFromHeading(string strHeading)
        {
            PericopeData pdReturn;

            if (dPericopes.Count(a => a.Value.strPericope1Number == strHeading) == 1)
            {
                pdReturn = dPericopes.Where(a => a.Value.strPericope1Number == strHeading).Select(a => a.Value).First();
            }
            else if (dPericopes.Count(a => a.Value.strPericopeLetter == strHeading) == 1)
            {
                pdReturn = dPericopes.Where(a => a.Value.strPericopeLetter == strHeading).Select(a => a.Value).First();
            }
            else if (dPericopes.Count(a => a.Value.strPericopeRomanNumeral == strHeading) == 1)
            {
                pdReturn = dPericopes.Where(a => a.Value.strPericopeRomanNumeral == strHeading).Select(a => a.Value).First();
            }
            else if (dPericopes.Count(a => a.Value.strPericope2Number == strHeading) == 1)
            {
                pdReturn = dPericopes.Where(a => a.Value.strPericope2Number == strHeading).Select(a => a.Value).First();
            }
            else 
            {
                throw new Exception("Error in PericopedSections.GetPericopeFromHeading");
            }

            return pdReturn;
        }

        public string[] GetHeadingsFromReference(ref Dictionary<int, Verse> dVerses, string strBookName, int intChapterNumber, int intVerseNumber)
        {
            string[] strsReturn = (string[]) Array.CreateInstance(typeof(string), 4);
            
            foreach (PericopeData pdCurrent in dPericopes.Values)
            {
                bool bContinue = false;

                if (strBookName == pdCurrent.strBookName &&
                    intChapterNumber == pdCurrent.intStartChapterNumber && 
                    intVerseNumber >= pdCurrent.intStartVerseNumber &&
                    intVerseNumber <= pdCurrent.intEndVerseNumber)
                {
                    if (pdCurrent.strPericope1Number.Trim() != "")
                    {
                        strsReturn[0] = pdCurrent.strPericope1Number.Trim();
                        bContinue = true;
                    }
                    
                    if (pdCurrent.strPericopeLetter.Trim() != "")
                    {
                        strsReturn[1] = pdCurrent.strPericopeLetter.Trim();
                        bContinue = true;
                    }
                    
                    if (pdCurrent.strPericopeRomanNumeral.Trim() != "")
                    {
                        strsReturn[2] = pdCurrent.strPericopeRomanNumeral.Trim();
                        bContinue = true;
                    }
                    
                    if(pdCurrent.strPericope2Number.Trim() != "")
                    {
                        strsReturn[3] = pdCurrent.strPericope2Number.Trim();
                        bContinue = true;
                    }

                    if (bContinue == false)
                    {
                        throw new Exception("Error in PericopedSections.GetHeadingFromReference 1");
                    }
                }
                else if (strBookName == pdCurrent.strBookName && 
                    intChapterNumber == pdCurrent.intEndChapterNumber &&
                    intVerseNumber <= pdCurrent.intEndVerseNumber)
                {
                    if (pdCurrent.strPericope1Number.Trim() != "")
                    {
                        strsReturn[0] = pdCurrent.strPericope1Number.Trim();
                        bContinue = true;
                    }

                    if (pdCurrent.strPericopeLetter.Trim() != "")
                    {
                        strsReturn[1] = pdCurrent.strPericopeLetter.Trim();
                        bContinue = true;
                    }

                    if (pdCurrent.strPericopeRomanNumeral.Trim() != "")
                    {
                        strsReturn[2] = pdCurrent.strPericopeRomanNumeral.Trim();
                        bContinue = true;
                    }

                    if (pdCurrent.strPericope2Number.Trim() != "")
                    {
                        strsReturn[3] = pdCurrent.strPericope2Number.Trim();
                        bContinue = true;
                    }

                    if (bContinue == false)
                    {
                        throw new Exception("Error in PericopedSections.GetHeadingFromReference 2");
                    }
                }
                else if (strBookName == pdCurrent.strBookName &&
                    intChapterNumber > pdCurrent.intStartChapterNumber &&
                    intChapterNumber < pdCurrent.intEndChapterNumber)
                {
                    if (pdCurrent.strPericope1Number.Trim() != "")
                    {
                        strsReturn[0] = pdCurrent.strPericope1Number.Trim();
                        bContinue = true;
                    }

                    if (pdCurrent.strPericopeLetter.Trim() != "")
                    {
                        strsReturn[1] = pdCurrent.strPericopeLetter.Trim();
                        bContinue = true;
                    }

                    if (pdCurrent.strPericopeRomanNumeral.Trim() != "")
                    {
                        strsReturn[2] = pdCurrent.strPericopeRomanNumeral.Trim();
                        bContinue = true;
                    }

                    if (pdCurrent.strPericope2Number.Trim() != "")
                    {
                        strsReturn[3] = pdCurrent.strPericope2Number.Trim();
                        bContinue = true;
                    }

                    if (bContinue == false)
                    {
                        throw new Exception("Error in PericopedSections.GetHeadingFromReference 3");
                    }
                }
            }
                        
            return strsReturn;
        }

        public List<PericopeData> GetPericopeDataObjectsFromHeadings(List<string> lHeadings)
        {
            List<PericopeData> lAllSearchedPericopeData = new List<PericopeData>();

            foreach (string strPericope in lHeadings)
            {
                lAllSearchedPericopeData.Add(GetPericopeDataFromHeading(strPericope));
            }

            return lAllSearchedPericopeData;
        }

        public Dictionary<int, List<string>> Search(List<string> lSearchTerms) //return D<1, L<ANY-search results>> D<2, L<ALL-search results>>
        {
            Dictionary<int, List<string>> dReturn = new Dictionary<int, List<string>>();

            List<string> lSearchAny = SearchHeadings(lSearchTerms, true);
            List<string> lSearchAll = SearchHeadings(lSearchTerms, false);
            
            dReturn.Add(1, lSearchAny);
            dReturn.Add(2, lSearchAll);

            return dReturn;
        }

        public Dictionary<int, string[]> GetPericopeHeadingsForEachVerse(ref Dictionary<int, Verse> dVerses)
        {
            //the string[] are ordered by heading structure (ie. first number, letter, roman numeral, second number)
            Dictionary<int, string[]> dReturn = new Dictionary<int, string[]>(); //D<VerseID, A[headings]>
            
            foreach (Verse v in dVerses.Values)
            {
                //the string[] are ordered by heading structure (ie. first number, letter, roman numeral, second number)
                string[] strsHeadings = GetHeadingsFromReference(ref dVerses, v.strBookName, v.intChapterNumber, v.intVerseNumber);

                dReturn.Add(v.intVerseID, strsHeadings);
            }

            return dReturn;
        }

        public Dictionary<string, List<int>> GetVerseIDsFromPericopeHeading(ref Dictionary<int, Verse> dVerses, ref Dictionary<int, string[]> dVerseIDsPericopeHeadings, string strPericopeHeading)
        {
            Dictionary<string, List<int>> dReturn = new Dictionary<string, List<int>>();

            foreach (int intVerseID in dVerseIDsPericopeHeadings.Keys)
            {
                foreach (string strHeading in dVerseIDsPericopeHeadings[intVerseID])
                {
                    if (strHeading != null)
                    {
                        if (!dReturn.ContainsKey(strHeading))
                        {
                            dReturn.Add(strHeading, new List<int>());
                        }

                        if (!dReturn[strHeading].Contains(intVerseID))
                        {
                            try
                            {
                                dReturn[strHeading].Add(intVerseID);
                            }
                            catch
                            {
                                throw new Exception("dPericopeHeadingsVerses needs a bigger Verse array");
                            }
                        }
                    }
                }
            }

            return dReturn;
        }

    }
}
