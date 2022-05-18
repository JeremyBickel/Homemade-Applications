using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class STEPBibleHebrewLexicon
    {
        //Gen.46.18-12	Gen.46.18-12	לְיַעֲקֹב	לְ/יַעֲקֹ֔ב	HR/Npm	H9005=ל=to/H3290=יַעֲקֹב=Jacob_§Jacob|Israel@Gen.25.26
        public string strReference = ""; //Gen.46.18-12
        public Dictionary<int, Dictionary<int, Lexicon>> dLexicon = new Dictionary<int, Dictionary<int, Lexicon>>(); //D<WordID, D<AlternateID, Lexicon>>
        
        public void Read()
        {
            StreamReader srTOTHT = new StreamReader(@"Data\TOTHT.txt");

            int intLineCounter = 0;

            while (!srTOTHT.EndOfStream)
            {
                string strLine = srTOTHT.ReadLine();

                if (strLine != "")
                {
                    string strExtended = "";
                    int intPartCounter = 0;
                    string strAlternateParse = ""; //the alternate parse is Ncmpc/Sp3ms in HC/Npm//Ncmpc/Sp3ms <- double // splits an alternate

                    //Sometimes a morphology has more than one possibility.  Note the /_/ in the example which separates these possibilities:
                    //2Ki.19.37-08.Q	2Ki.19.37-08q	וְשַׂרְאֶצֶר בָּנָיו	וְ/שַׂרְאֶ֤צֶר/ /בָּנָי/ו֙	HC/Npm//Ncmpc/Sp3ms	H9002=ו=and/H8272=שַׁרְאֶ֫צֶר=Sharezer_§Sharezer@2Ki.19.37/_/H1121a=בֵּן=son_§1_child|son/H9023=Ps3m=his
                    
                    intLineCounter++;

                    if (intLineCounter == 11188 || intLineCounter == 79574)
                    {
                        string s = "";
                    }

                    strReference = strLine.Split()[0];

                    foreach(string strPart in strLine.Split('\t'))
                    {
                        intPartCounter++;

                        if (intPartCounter == 5)
                        {
                            int intParsePartCounter = 0;

                            if (!strPart.Contains("//"))
                            {
                                foreach (string strParsePart in strPart.Split('/'))
                                {
                                    intParsePartCounter++;

                                    if (!dLexicon.ContainsKey(intLineCounter))
                                    {
                                        dLexicon.Add(intLineCounter, new Dictionary<int, Lexicon>());
                                    }

                                    dLexicon[intLineCounter].Add(dLexicon[intLineCounter].Count() + 1, new Lexicon());
                                    dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strParse = strParsePart;
                                }
                            }
                            else
                            {
                                int intAlternatePartCounter = 0;

                                foreach (string strAlternatePart in strPart.Split("//"))
                                {
                                    intAlternatePartCounter++;

                                        foreach (string strParsePart in strAlternatePart.Split('/'))
                                        {
                                            intParsePartCounter++;

                                            if (!dLexicon.ContainsKey(intLineCounter))
                                            {
                                                dLexicon.Add(intLineCounter, new Dictionary<int, Lexicon>());
                                                dLexicon[intLineCounter].Add(1, new Lexicon());
                                            }
                                        if (intAlternatePartCounter == 1)
                                        {
                                            dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strParse = strParsePart;
                                        }
                                        else if (intAlternatePartCounter == 2)
                                        {
                                            dLexicon[intLineCounter].Add(dLexicon[intLineCounter].Count() + 1, new Lexicon());
                                            dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strParse = strParsePart;
                                        }
                                        else
                                        {
                                            throw (new Exception("More than two alternate parses using '//'"));
                                        }
                                    }
                                }
                            }
                        }
                        else if (intPartCounter > 5) //strExtended might have spaces in it
                        {
                            strExtended += strPart;
                        }
                    }

                    intPartCounter = 0;

                    //Process strExtended
                    foreach (string strPart in strExtended.Split('/'))
                    {
                        int intPart2Counter = 0;

                        if (strPart == "_") //alternate
                        {
                            dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].intWordOrderID =
                                dLexicon[intLineCounter][dLexicon[intLineCounter].Count() - 1].intWordOrderID;
                            intPartCounter = 0; //the next strPart starts a new morphology
                        }
                        else
                        {
                            
                                intPartCounter++;
                                dLexicon[intLineCounter][intPartCounter].intWordOrderID = intPartCounter;
                            
                        }

                        if (intPartCounter > 0)
                        {
                            foreach (string strPart2 in strPart.Split('='))
                            {
                                intPart2Counter++;

                                if (intPart2Counter == 1) //Strongs number
                                {  
                                   dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strStrongs = strPart2;
                                }
                                else if (intPart2Counter == 3) //Translations
                                {
                                    int intPart3Counter = 0;

                                    foreach (string strPart3 in strPart2.Split('|'))
                                    {
                                        //Jacob_§Jacob|Israel@Gen.25.26|sea_§1_sea_§Ephron@Jos.18.15

                                        intPart3Counter++;

                                        dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].intTranslationOrderID = intPart3Counter;

                                        if (strPart3.Contains('§'))
                                        {
                                            int intPart4Counter = 0;

                                            foreach (string strPart4 in strPart3.Split('§'))
                                            {
                                                string strPart4Cleaned = strPart4;

                                                while (strPart4Cleaned.Contains("_"))
                                                {
                                                    strPart4Cleaned = strPart4Cleaned.Trim('_').Substring(strPart4Cleaned.IndexOf('_'));
                                                }

                                                intPart4Counter++;

                                                if (intPart4Counter == 1)
                                                {
                                                    dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strTranslation = strPart4.Trim('_');
                                                }
                                                else if (intPart4Counter == 2)
                                                {
                                                    dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strTranslationAlternate = strPart4Cleaned;
                                                }
                                                else if (intPart4Counter == 3)
                                                {
                                                    dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strTranslationAlternate2 = strPart4Cleaned;
                                                }
                                                else if (intPart4Counter > 3)
                                                {
                                                    throw (new Exception("intPart4Counter > 3"));
                                                }
                                            }
                                        }
                                        else if (strPart3.Contains('@'))
                                        {
                                            dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strTranslationSemantic = strPart3;
                                        }
                                        else
                                        {
                                            dLexicon[intLineCounter][dLexicon[intLineCounter].Count()].strTranslation = strPart3;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } //end while

            var a = dLexicon.Where(a=>a.Value.Count() > 1).Select(a => a);
        }
    }

    internal class Lexicon
    {
        //Gen.46.18-12	Gen.46.18-12	לְיַעֲקֹב	לְ/יַעֲקֹ֔ב	HR/Npm	H9005=ל=to/H3290=יַעֲקֹב=Jacob_§Jacob|Israel@Gen.25.26

        public string strParse = "";
        public string strStrongs = "";
        public int intWordOrderID = 0; //separated by /
        public int intTranslationOrderID = 0; //separated by |
        public string strTranslation = ""; //if intWordOrderID == 1 then strTranslation == "to"; if intWordOrderID == 2 then strTranslation == "Jacob"
        public string strTranslationAlternate = ""; //if intWordOrderID == 1 then strTranslation == ""; if intWordOrderID == 2 then strTranslation == "Jacob"
        public string strTranslationAlternate2 = ""; //if intWordOrderID == 1 then strTranslation == ""; if intWordOrderID == 2 then strTranslation == ""
        public string strTranslationSemantic = ""; //if intWordOrderID == 1 then strTranslation == ""; if intWordOrderID == 2 then strTranslation == "Israel@Gen.25.26"

        public Lexicon(){}
    }
}
