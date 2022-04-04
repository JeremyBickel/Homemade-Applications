using System.Text.RegularExpressions;
using System.IO;

namespace Bible_Searcher
{
    public partial class Welcome : Form
    {
        Dictionary<int, Verse> dVerses = new Dictionary<int, Verse>();
        Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries = new Dictionary<int, BLBGreekLexicon>();
        Dictionary<int, Dictionary<int, string>> dBLBHebrewConcordance = new Dictionary<int, Dictionary<int, string>>(); //D<Concordance ID, D<Reference ID, Reference>>
        Dictionary<int, Dictionary<int, string>> dBLBGreekConcordance = new Dictionary<int, Dictionary<int, string>>(); //D<Concordance ID, D<Reference ID, Reference>>

        Dictionary<string, int> dEnglishPhraseCountsOrderedByCount = new Dictionary<string, int>();
        Dictionary<string, int> dEnglishPhraseCountsOrderedByPhrase = new Dictionary<string, int>();

        //Tests for BLB Hebrew Lexicon
        Dictionary<int, BLBHebrewLexicon> dHRoots = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHNonRoots = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHAramaic = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHNonAramaic = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHRootedAramaic = new Dictionary<int, BLBHebrewLexicon>();

        public Welcome()
        {
            InitializeComponent();

            //
            //UI
            //
            FillLBBooks();

            //
            //BLB
            //
            FillBLBHebrewLexicon();
            FillBLBGreekLexicon();
            FillBLBConcordance("hebrew");
            FillBLBConcordance("greek");

            //
            //Bible
            //
            FillKJVStrongs();
            ParseKJVStrongs();
            CalculateEnglishPhraseCounts();

            //Test BLB Hebrew Lexicon
            dHRoots = HRoots();
            dHNonRoots = HNonRoots();
            dHAramaic = Aramaic();
            dHNonAramaic = NotAramaic();
            dHRootedAramaic = RootedAramaic();

            //
            //Bible
            //
            CalculateSVOs();
        }

        public void FillLBBooks()
        {
            lbBooks.Items.Add("Genesis");
            lbBooks.Items.Add("Exodus");
            lbBooks.Items.Add("Leviticus");
            lbBooks.Items.Add("Numbers");
            lbBooks.Items.Add("Deuteronomy");
            lbBooks.Items.Add("Joshue");
            lbBooks.Items.Add("Judges");
            lbBooks.Items.Add("Ruth");
            lbBooks.Items.Add("1 Samuel");
            lbBooks.Items.Add("2 Samuel");
            lbBooks.Items.Add("1 Kings");
            lbBooks.Items.Add("2 Kings");
            lbBooks.Items.Add("1 Chronicles");
            lbBooks.Items.Add("2 Chronicles");
            lbBooks.Items.Add("Ezra");
            lbBooks.Items.Add("Nehemiah");
            lbBooks.Items.Add("Esther");
            lbBooks.Items.Add("Job");
            lbBooks.Items.Add("Psalms");
            lbBooks.Items.Add("Proverbs");
            lbBooks.Items.Add("Ecclesiastes");
            lbBooks.Items.Add("Song of Solomon");
            lbBooks.Items.Add("Isaiah");
            lbBooks.Items.Add("Jeremiah");
            lbBooks.Items.Add("Lamentations");
            lbBooks.Items.Add("Ezekiel");
            lbBooks.Items.Add("Daniel");
            lbBooks.Items.Add("Hosea");
            lbBooks.Items.Add("Joel");
            lbBooks.Items.Add("Amos");
            lbBooks.Items.Add("Obediah");
            lbBooks.Items.Add("Jonah");
            lbBooks.Items.Add("Micah");
            lbBooks.Items.Add("Nahum");
            lbBooks.Items.Add("Habakkuk");
            lbBooks.Items.Add("Zephaniah");
            lbBooks.Items.Add("Haggai");
            lbBooks.Items.Add("Zechariah");
            lbBooks.Items.Add("Malachi");
            lbBooks.Items.Add("Matthew");
            lbBooks.Items.Add("Mark");
            lbBooks.Items.Add("Luke");
            lbBooks.Items.Add("John");
            lbBooks.Items.Add("Acts");
            lbBooks.Items.Add("Romans");
            lbBooks.Items.Add("1 Corinthians");
            lbBooks.Items.Add("2 Corinthians");
            lbBooks.Items.Add("Galatians");
            lbBooks.Items.Add("Ephesians");
            lbBooks.Items.Add("Philippians");
            lbBooks.Items.Add("Colossians");
            lbBooks.Items.Add("1 Thessalonians");
            lbBooks.Items.Add("2 Thessalonians");
            lbBooks.Items.Add("1 Timothy");
            lbBooks.Items.Add("2 Timothy");
            lbBooks.Items.Add("Titus");
            lbBooks.Items.Add("Philemon");
            lbBooks.Items.Add("Hebrews");
            lbBooks.Items.Add("James");
            lbBooks.Items.Add("1 Peter");
            lbBooks.Items.Add("2 Peter");
            lbBooks.Items.Add("1 John");
            lbBooks.Items.Add("2 John");
            lbBooks.Items.Add("3 John");
            lbBooks.Items.Add("Jude");
            lbBooks.Items.Add("Revelation");
        }

        public void FillKJVStrongs()
        {
            StreamReader srBible = new StreamReader(@"Data\kjvstrongs.csv");
            string strLastBookName = "";
            int intBookNumber = 0;
            int intVerseID = 0;

            while (!srBible.EndOfStream)
            {
                string strLine = srBible.ReadLine();
                Verse v = new Verse();
                int intComma = strLine.IndexOf(",");

                intVerseID++;

                v.intVerseID = intVerseID;

                v.strBookName = strLine.Substring(0, intComma);

                if (v.strBookName != strLastBookName)
                {
                    strLastBookName = v.strBookName;
                    intBookNumber++;
                }

                v.intBookNumber = intBookNumber;

                strLine = strLine.Remove(0, intComma + 1);
                intComma = strLine.IndexOf(",");
                v.intChapterNumber = Convert.ToInt32(strLine.Substring(0, intComma));

                strLine = strLine.Remove(0, intComma + 1);
                intComma = strLine.IndexOf(",");
                v.intVerseNumber = Convert.ToInt32(strLine.Substring(0, intComma));

                strLine = strLine.Remove(0, intComma + 1);
                strLine = strLine.Trim();
                v.strText = strLine;

                dVerses.Add(v.intVerseID, v);
            }
        }

        public void ParseKJVStrongs()
        {
            Regex rgxText = new(@"(?<phrase>[a-z'A-Z ]{0,})(?<strongs>(\{\({0,} [0-9]{1,} \){0,}\}){1,})");

            foreach (int intVerseID in dVerses.Keys)
            {
                int intPhraseID = 0;

                foreach (Match mPhrase in rgxText.Matches(dVerses[intVerseID].strText))
                {
                    int intCaptureID = 0;
                    string strPhrase = mPhrase.Groups["phrase"].Value.Trim();

                    intPhraseID++;

                    dVerses[intVerseID].dPhrases.Add(intPhraseID, new Phrase());
                    dVerses[intVerseID].dPhrases[intPhraseID].intPhraseID = intPhraseID;
                    dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText = strPhrase;

                    foreach (Capture capStrongs in mPhrase.Groups["1"].Captures)
                    {
                        Regex rgxLowerLetters = new Regex(@"[^a-z\/ ]");
                        bool bParenthecized = capStrongs.Value.Contains("(");
                        string strStrongsNumber = capStrongs.Value.TrimStart('{').TrimEnd('}').TrimStart('(').TrimEnd(')').Trim();
                        string strPOS = "";
                        int intStrongsNumber = Convert.ToInt32(strStrongsNumber);

                        intCaptureID++;

                        dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences.Add(intCaptureID, new StrongsSequence());
                        dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].intStrongsSequenceID = intCaptureID;
                        dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].strStrongsNumber = strStrongsNumber;
                        dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bParenthecized = bParenthecized;

                        //Combine POS into main categories
                        strPOS = rgxLowerLetters.Replace(dBLBHebrewLexiconEntries[Convert.ToInt32(strStrongsNumber)].strPOS.ToLower(), "").Trim();

                        foreach (string strPOSPart in strPOS.Split())
                        {
                            if (intStrongsNumber == 853 || intStrongsNumber == 3487) //H853, H3487 means "Direct Object"
                            {
                                dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bObject = true;
                            }
                            else if (strPOSPart == "n" || strPOSPart == "adj" || strPOSPart == "adv" || strPOSPart == "pr" ||
                                strPOSPart == "pron" || strPOSPart == "adjective" || strPOSPart == "adverb" || strPOSPart == "adj/subst")
                            {
                                dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bThing = true;
                            }
                            else if (strPOSPart == "v" || strPOSPart == "v/n" || strPOSPart == "verb" || strPOSPart == "verbal")
                            {
                                dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bAction = true;
                            }

                            else if (strPOSPart == "prep" || strPOSPart == "prep/conj" || strPOSPart == "preposition")
                            {
                                dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bPreposition = true;
                            }
                        }
                    }
                }
            }
        }

        public void CalculateEnglishPhraseCounts()
        {
            Dictionary<string, int> dEnglishPhraseCounts = new Dictionary<string, int>();
            
            foreach (Verse v in dVerses.Values)
            {
                foreach (Phrase p in v.dPhrases.Values)
                {
                    string strPhrase = p.strPhraseText.ToLower();

                    if (!dEnglishPhraseCounts.ContainsKey(strPhrase))
                    {
                        dEnglishPhraseCounts.Add(strPhrase, 1);
                    }
                    else
                    {
                        dEnglishPhraseCounts[strPhrase]++;
                    }
                }
            }

            foreach (string strKey in dEnglishPhraseCounts.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                dEnglishPhraseCountsOrderedByCount.Add(strKey, dEnglishPhraseCounts[strKey]);
            }

            foreach (string strKey in dEnglishPhraseCounts.OrderBy(a => a.Key).Select(a => a.Key))
            {
                dEnglishPhraseCountsOrderedByPhrase.Add(strKey, dEnglishPhraseCounts[strKey]);
            }
        }

        public void CalculateSVOs()
        {
            int intHighestHLexID = dBLBHebrewLexiconEntries.Keys.Max();
            int intHighestGLexID = dBLBGreekLexiconEntries.Keys.Max();
            Regex rgxNumbers = new Regex(@"[0-9]{1,}");
            Dictionary<int, SVO> dSVOs = new Dictionary<int, SVO>();
            
            
        }

        public void FillBLBHebrewLexicon()
        {
            //Lex["01"] = "'ab {awb}|AV - father 1205, chief 2, families 2,
            //desire 1, fatherless + 0369 1,<BR> &nbsp;&nbsp;&nbsp;&nbsp;
            //&nbsp;forefathers + 07223 1, patrimony 1, prince 1, principal 1;
            //1215|a root|TWOT - 4a|n m|INDENT0--<b>1)</b> father of an
            //individualINDENT0--<b>2)</b> of God as father of his
            //peopleINDENT0--<b>3)</b> head or founder of a household, group,
            //family, or clanINDENT0--<b>4)</b> ancestorINDENT3-- <b>a)</b>
            //grandfather, forefathers -- of personINDENT3-- <b>b)</b> of
            //peopleINDENT0--<b>5)</b> originator or patron of a class,
            //profession, or artINDENT0--<b>6)</b> of producer,
            //generator (fig.)INDENT0--<b>7)</b> of benevolence and
            //protection (fig.)INDENT0--<b>8)</b> term of respect and
            //honourINDENT0--<b>9)</b> ruler or chief (spec.)";

            //Lex["02"] = "'ab (Aramaic) {ab}|AV - father 9; 9|corresponding
            //to 01|TWOT - 2553|n m|INDENT0--<b>1)</b> father";

            StreamReader srBLBLex = new StreamReader(@"Data\HebLex.js");
            int intPartNumber = 0; //Which part of the ';'-separated line are we in?
            int intLexID = 0;
            Regex rgxEnumeration = new Regex(@"(?<ind>INDENT)(?<inum>[0-9]{1,})\-\-[ ]{0,}(?<enum>[a-z0-9]{1,})\)");
            List<string> lParses = new List<string>();
            List<string> lRoots = new List<string>();
            List<string> lNonRoots = new List<string>();
            StreamReader srRoots = new StreamReader(@"Data\HebrewRoots.txt");
            StreamReader srNonRoots = new StreamReader(@"Data\HebrewNon-Roots.txt");

            while (!srRoots.EndOfStream)
            {
                lRoots.Add(srRoots.ReadLine().Trim());
            }

            srRoots.Close();

            while (!srNonRoots.EndOfStream)
            {
                lNonRoots.Add(srNonRoots.ReadLine().Trim());
            }

            srNonRoots.Close();

            while (!srBLBLex.EndOfStream)
            {
                string strLine = srBLBLex.ReadLine();
                int intEqualsIndex = strLine.IndexOf('=');
                string strFirstPart = strLine.Substring(0, intEqualsIndex);
                string strSecondPart = strLine.Substring(intEqualsIndex + 1);
                string strLexID = strFirstPart.Substring(5); //Lex[" = 5
                int intNextQuoteIndex = strLexID.IndexOf('"');
                Dictionary<string, string> dEntries = new Dictionary<string, string>();

                BLBHebrewLexicon blbLex = new BLBHebrewLexicon();

                intLexID = Convert.ToInt32(strLexID.Substring(0, intNextQuoteIndex));

                //Clean up strSecondPart
                strSecondPart = strSecondPart.Substring(2);
                strSecondPart = strSecondPart.Substring(0, strSecondPart.Length - 2);
                strSecondPart = strSecondPart.Replace("<BR>", "").Replace("&nbsp;", "");
                //strSecondPart = rgxINDENT.Replace(strSecondPart, "");

                blbLex.intLexID = intLexID;

                if (intLexID > 0) //account for metadata at top of data file HebLex.js
                {
                    intPartNumber = 0;

                    foreach (string strPart in strSecondPart.Split('|'))
                    {
                        intPartNumber++;

                        if (intPartNumber == 1) //Transliteration ?(Aramaic)? {Pronunciation}
                        {
                            foreach (string strPart1Part in strPart.Split())
                            {
                                if (strPart1Part == "(Aramaic)")
                                {
                                    blbLex.bAramaic = true;
                                }
                                else if (strPart1Part.StartsWith('{'))
                                {
                                    blbLex.strPronunciation = strPart1Part.TrimStart('{').TrimEnd('}');
                                }
                                else
                                {
                                    blbLex.strTransliteration = strPart1Part;
                                }
                            }
                        }
                        else if (intPartNumber == 2)//2 - AV
                        {
                            string strPart2 = strPart.Substring(5); //Removes "AV - " from the beginning
                            string strPart2Part1 = strPart2.Split(";")[0];

                            blbLex.intTotalTranslated = Convert.ToInt32(strPart2.Split(";")[1]);

                            foreach (string strPart2Part in strPart2Part1.Split(',').Select(a => a.Trim()))
                            {
                                int intTranslationCountIndex = -1;
                                int intTranslationCount = 0;
                                string strTranslation = "";

                                if (Regex.IsMatch(strPart2Part, "[0-9]"))
                                {
                                    intTranslationCountIndex = Regex.Matches(strPart2Part, " [0-9]{1,}").Last().Index; //index at start of sequence of space then numbers
                                    int intTranslationCountIndexFinal = Regex.Matches(strPart2Part, "[0-9]").Last().Index; //index of last number
                                    intTranslationCount = Convert.ToInt32(strPart2Part.Substring(intTranslationCountIndex, intTranslationCountIndexFinal - intTranslationCountIndex + 1));
                                    strTranslation = strPart2Part.Substring(0, intTranslationCountIndex).Trim();
                                }
                                else
                                {
                                    strTranslation = strPart2Part.Trim();
                                }

                                if (blbLex.dAVTranslations.ContainsKey(strTranslation))
                                {
                                    blbLex.dAVTranslations[strTranslation] += intTranslationCount;
                                }
                                else
                                {
                                    blbLex.dAVTranslations.Add(strTranslation, intTranslationCount);
                                }
                            }

                        }
                        else if (intPartNumber == 3) //3 - a root or from which words meaning what?
                        {
                            string strConnectionPart = Regex.Replace(strPart, @"\s+", " ").Trim(); //condenses multiple whitespace to a single space; important since I manually stripped these while creating the "Root" and "Non-Root" phrase files

                            foreach (string strRoot in lRoots)
                            {
                                if (strPart == Regex.Replace(strRoot, @"\s+", " ").Trim()) //just in case I missed condensing whitespace in a connection phrase
                                {
                                    blbLex.bRoot = true;
                                    break;
                                }
                            }

                            blbLex.strConnection = strPart;
                        }
                        else if (intPartNumber == 4) //4 - TWOT #
                        {
                            if (strPart.Trim().Length > 0)
                            {
                                blbLex.strTWOTNumber = strPart.Split("-")[1].Trim();
                            }
                        }
                        else if (intPartNumber == 5) //5 - pos ?pr? gender
                        {
                            if (!lParses.Contains(strPart))
                            {
                                lParses.Add(strPart);
                            }

                            blbLex.strPOS = strPart;
                        }
                        else if (intPartNumber == 6) //6 - entries
                        {
                            //INDENT0--<b>1)</b> perish, vanish, go astray, be destroyed
                            //INDENT3-- <b>a)</b> (Qal)
                            //INDENT7--<b>1)</b> perish, die, be exterminated
                            //INDENT7--<b>2)</b> perish, vanish (fig.)
                            //INDENT7--<b>3)</b> be lost, strayed
                            //INDENT3-- <b>b)</b> (Piel)
                            //INDENT7--<b>1)</b> to destroy, kill, cause to perish, to give up (as lost), exterminate
                            //INDENT7--<b>2)</b> to blot out, do away with, cause to vanish, (fig.)
                            //INDENT7--<b>3)</b> cause to stray, lose
                            //INDENT3-- <b>c)</b> (Hiphil)
                            //INDENT7--<b>1)</b> to destroy, put to death
                            //INDENT12--   <b>a)</b> of divine judgment
                            //INDENT7--<b>2)</b> object name of kings (fig.)

                            string strPart6 = strPart.Replace("<b>", "").Replace("</b>", "").Trim();
                            string strCurrentNumber = "";
                            MatchCollection mmEntries = rgxEnumeration.Matches(strPart6);
                            Dictionary<int, int> dIndentIndexes = new Dictionary<int, int>();
                            string strWholeEntry = "";
                            string strLast0 = "";
                            string strLast3 = "";
                            string strLast7 = "";
                            string strLast12 = "";
                            string strLast18 = "";

                            foreach (Match mEntry in mmEntries)
                            {
                                dIndentIndexes.Add(dIndentIndexes.Count() + 1, mEntry.Index);
                            }

                            foreach (int intIndentOrder in dIndentIndexes.OrderBy(a => a.Key).Select(a => a.Key))
                            {
                                if (intIndentOrder < dIndentIndexes.Count())
                                {
                                    strWholeEntry = strPart6.Substring(dIndentIndexes[intIndentOrder], dIndentIndexes[intIndentOrder + 1] - dIndentIndexes[intIndentOrder]);
                                }
                                else
                                {
                                    strWholeEntry = strPart6.Substring(dIndentIndexes[intIndentOrder]);
                                }

                                Match mEntry = rgxEnumeration.Match(strWholeEntry);
                                string strEntryText = strWholeEntry.Substring(mEntry.Captures[0].Length).Trim();
                                string strEnum = "";

                                //0, 3, 7, 12, 18
                                if (mEntry.Groups["inum"].Value == "0")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast0 = strEnum;
                                    strLast3 = "";
                                    strLast7 = "";
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0;
                                }
                                else if (mEntry.Groups["inum"].Value == "3")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast3 = strEnum;
                                    strLast7 = "";
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3;
                                }
                                else if (mEntry.Groups["inum"].Value == "7")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast7 = strEnum;
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3 + strLast7;
                                }
                                else if (mEntry.Groups["inum"].Value == "12")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast12 = strEnum;
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3 + strLast7 + strLast12;
                                }
                                else if (mEntry.Groups["inum"].Value == "18")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast18 = strEnum;
                                    strCurrentNumber = strLast0 + strLast3 + strLast7 + strLast12 + strLast18;
                                }
                                else
                                {
                                    throw new Exception("Lexical Entry Error");
                                }

                                dEntries.Add(strCurrentNumber, strEntryText);
                            }

                            blbLex.dLexicalEntries = dEntries;
                        }
                    }

                    dBLBHebrewLexiconEntries.Add(intLexID, blbLex);
                }
            }
        }

        Dictionary<int, BLBHebrewLexicon> HRoots()
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new Dictionary<int, BLBHebrewLexicon>();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bRoot == true)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        Dictionary<int, BLBHebrewLexicon> HNonRoots()
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new Dictionary<int, BLBHebrewLexicon>();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bRoot == false)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        Dictionary<int, BLBHebrewLexicon> Aramaic()
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new Dictionary<int, BLBHebrewLexicon>();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bAramaic == true)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        Dictionary<int, BLBHebrewLexicon> NotAramaic()
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new Dictionary<int, BLBHebrewLexicon>();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bAramaic == false)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                }
            }

            return dReturn;
        }

        Dictionary<int, BLBHebrewLexicon> RootedAramaic()
        {
            Dictionary<int, BLBHebrewLexicon> dReturn = new Dictionary<int, BLBHebrewLexicon>();

            foreach (int intKey in dBLBHebrewLexiconEntries.Keys)
            {
                if (dBLBHebrewLexiconEntries[intKey].bAramaic == true && dBLBHebrewLexiconEntries[intKey].bRoot == true)
                {
                    dReturn.Add(intKey, dBLBHebrewLexiconEntries[intKey]);
                } 
            }

            return dReturn;
        }

        public void FillBLBGreekLexicon()
        {
            ///
            ///NOTE: There's no G2717, G3203-G3302
            ///NOTE: G4483 inserts 9 as the count for the AV translation "say" (there was no count given),
            /// even though there are 11 instances in KJV, in order to fit the total of 26
            ///

            //Lex["18"] = "
            //agathos {ag-ath-os'}|
            //AV - good 77, good thing 14, that which is good+3588 8, <BR> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;the thing which is good+3588 1, well 1, benefit 1; 102|
            //a primary word|
            //TDNT - 1:10,3|
            //adj|
            //INDENT0--<b>1)</b> of good constitution or nature
            //INDENT0--<b>2)</b> useful, salutary
            //INDENT0--<b>3)</b> good, pleasant, agreeable, joyful, happy
            //INDENT0--<b>4)</b> excellent, distinguished
            //INDENT0--<b>5)</b> upright, honourable
            //";

            StreamReader srBLBLex = new StreamReader(@"Data\GrkLex.js");
            int intPartNumber = 0; //Which part of the ';'-separated line are we in?
            int intLexID = 0;
            Regex rgxEnumeration = new Regex(@"(?<ind>INDENT)(?<inum>[0-9]{1,})\-\-[ ]{0,}(?<enum>[a-z0-9]{1,})\)");
            Regex rgxAVTranslationCountWithParenthesis = new Regex(@" \({1}[0-9]{1,}\){1}");
            Regex rgxAVTranslationCountWithoutParenthesis = new Regex(@" [0-9]{1,}");
            Regex rgxSingleDigit = new Regex(@"[0-9]");
            Regex rgxExtraTDNTInformation = new Regex(@"\{TDNT [0-9 \:\,]{1,}\}");
            List<string> lParses = new List<string>();
            List<string> lRoots = new List<string>();
            List<string> lNonRoots = new List<string>();
            //StreamReader srRoots = new StreamReader(@"Data\GreekRoots.txt");
            //StreamReader srNonRoots = new StreamReader(@"Data\GreekNon-Roots.txt");

            //while (!srRoots.EndOfStream)
            //{
            //    lRoots.Add(srRoots.ReadLine().Trim());
            //}

            //srRoots.Close();

            //while (!srNonRoots.EndOfStream)
            //{
            //    lNonRoots.Add(srNonRoots.ReadLine().Trim());
            //}

            //srNonRoots.Close();

            while (!srBLBLex.EndOfStream)
            {
                string strLine = srBLBLex.ReadLine();
                int intEqualsIndex = strLine.IndexOf('=');
                string strFirstPart = strLine.Substring(0, intEqualsIndex);
                string strSecondPart = strLine.Substring(intEqualsIndex + 1);
                string strLexID = strFirstPart.Substring(5); //Lex[" = 5
                int intNextQuoteIndex = strLexID.IndexOf('"');
                Dictionary<string, string> dEntries = new Dictionary<string, string>();

                BLBGreekLexicon blbLex = new BLBGreekLexicon();

                intLexID = Convert.ToInt32(strLexID.Substring(0, intNextQuoteIndex));

                //Clean up strSecondPart
                strSecondPart = strSecondPart.Substring(2);
                strSecondPart = strSecondPart.Substring(0, strSecondPart.Length - 2);
                strSecondPart = strSecondPart.Replace("<BR>", "").Replace("&nbsp;", "");
                //strSecondPart = rgxINDENT.Replace(strSecondPart, "");

                blbLex.intLexID = intLexID;

                if (intLexID > 0) //account for metadata at top of data file HebLex.js
                {
                    intPartNumber = 0;

                    foreach (string strPart in strSecondPart.Split('|'))
                    {
                        intPartNumber++;

                        if (intPartNumber == 1) //Transliteration {Pronunciation}
                        {
                            foreach (string strPart1Part in strPart.Split())
                            {
                                if (strPart1Part.StartsWith('{'))
                                {
                                    blbLex.strPronunciation = strPart1Part.TrimStart('{').TrimEnd('}');
                                }
                                else
                                {
                                    blbLex.strTransliteration = strPart1Part;
                                }
                            }
                        }
                        else if (intPartNumber == 2)//2 - AV
                        {
                            string strPart2 = strPart.Substring(5); //Removes "AV - " from the beginning
                            string strPart2Part1 = strPart2.Split(";")[0];
                            
                            blbLex.intTotalTranslated = Convert.ToInt32(strPart2.Split(";")[1]);

                            if (rgxExtraTDNTInformation.IsMatch(strPart2Part1))
                            {
                                foreach (Match mExtra in rgxExtraTDNTInformation.Matches(strPart2Part1))
                                {
                                    int intLastCommaIndex = strPart2Part1.Substring(0, mExtra.Index - 1).LastIndexOf(',');
                                    string strTranslationText = strPart2Part1.Substring(intLastCommaIndex + 1, mExtra.Index - intLastCommaIndex - 1);

                                    blbLex.dExtraTDNTInformation.Add(strTranslationText, mExtra.Value);
                                }

                                strPart2Part1 = rgxExtraTDNTInformation.Replace(strPart2Part1, "");
                            }

                            foreach (string strPart2Part in strPart2Part1.Split(',').Select(a => a.Trim()))
                            {
                                int intTranslationCountIndex = -1;
                                int intTranslationCount = 0;
                                int intTranslationCountIndexFinal = -1;
                                string strTranslation = "";

                                if (rgxAVTranslationCountWithoutParenthesis.IsMatch(strPart2Part))
                                {
                                    intTranslationCountIndex = rgxAVTranslationCountWithoutParenthesis.Matches(strPart2Part).Last().Index; //index at start of sequence of space then numbers
                                    intTranslationCountIndexFinal = rgxSingleDigit.Matches(strPart2Part).Last().Index; //index of last number
                                    intTranslationCount = Convert.ToInt32(strPart2Part.Substring(intTranslationCountIndex, intTranslationCountIndexFinal - intTranslationCountIndex + 1));
                                    strTranslation = strPart2Part.Substring(0, intTranslationCountIndex).Trim();
                                }
                                else
                                {
                                    if (strPart2Part.Trim().Contains(' '))
                                    {
                                        strTranslation = strPart2Part.Trim();
                                        strTranslation = strTranslation.Split()[0];
                                    }
                                    else
                                    {
                                        strTranslation = strPart2Part.Trim();
                                    }
                                }

                                if (rgxAVTranslationCountWithParenthesis.IsMatch(strPart2Part))
                                {
                                    blbLex.lConjugated.Add(strTranslation);
                                }

                                if (blbLex.dAVTranslations.ContainsKey(strTranslation))
                                {
                                    blbLex.dAVTranslations[strTranslation] += intTranslationCount;
                                }
                                else
                                {
                                    blbLex.dAVTranslations.Add(strTranslation, intTranslationCount);
                                }
                            }

                        }
                        else if (intPartNumber == 3) //3 - a root or from which words meaning what?
                        {
                            string strConnectionPart = Regex.Replace(strPart, @"\s+", " ").Trim(); //condenses multiple whitespace to a single space; important since I manually stripped these while creating the "Root" and "Non-Root" phrase files

                            foreach (string strRoot in lRoots)
                            {
                                if (strPart == Regex.Replace(strRoot, @"\s+", " ").Trim()) //just in case I missed condensing whitespace in a connection phrase
                                {
                                    blbLex.bRoot = true;
                                    break;
                                }
                            }

                            blbLex.strConnection = strPart;
                        }
                        else if (intPartNumber == 4) //4 - TWOT #
                        {
                            if (strPart.Trim().Length > 0)
                            {
                                try
                                {
                                    blbLex.strTWOTNumber = strPart.Split("-")[1].Trim();
                                }
                                catch { }
                            }
                        }
                        else if (intPartNumber == 5) //5 - pos ?pr? gender
                        {
                            if (!lParses.Contains(strPart))
                            {
                                lParses.Add(strPart);
                            }

                            blbLex.strPOS = strPart;
                        }
                        else if (intPartNumber == 6) //6 - entries
                        {
                            //INDENT0--<b>1)</b> perish, vanish, go astray, be destroyed
                            //INDENT3-- <b>a)</b> (Qal)
                            //INDENT7--<b>1)</b> perish, die, be exterminated
                            //INDENT7--<b>2)</b> perish, vanish (fig.)
                            //INDENT7--<b>3)</b> be lost, strayed
                            //INDENT3-- <b>b)</b> (Piel)
                            //INDENT7--<b>1)</b> to destroy, kill, cause to perish, to give up (as lost), exterminate
                            //INDENT7--<b>2)</b> to blot out, do away with, cause to vanish, (fig.)
                            //INDENT7--<b>3)</b> cause to stray, lose
                            //INDENT3-- <b>c)</b> (Hiphil)
                            //INDENT7--<b>1)</b> to destroy, put to death
                            //INDENT12--   <b>a)</b> of divine judgment
                            //INDENT7--<b>2)</b> object name of kings (fig.)

                            string strPart6 = strPart.Replace("<b>", "").Replace("</b>", "").Trim();
                            string strCurrentNumber = "";
                            MatchCollection mmEntries = rgxEnumeration.Matches(strPart6);
                            Dictionary<int, int> dIndentIndexes = new Dictionary<int, int>();
                            string strWholeEntry = "";
                            string strLast0 = "";
                            string strLast3 = "";
                            string strLast7 = "";
                            string strLast12 = "";
                            string strLast18 = "";

                            foreach (Match mEntry in mmEntries)
                            {
                                dIndentIndexes.Add(dIndentIndexes.Count() + 1, mEntry.Index);
                            }

                            foreach (int intIndentOrder in dIndentIndexes.OrderBy(a => a.Key).Select(a => a.Key))
                            {
                                if (intIndentOrder < dIndentIndexes.Count())
                                {
                                    strWholeEntry = strPart6.Substring(dIndentIndexes[intIndentOrder], dIndentIndexes[intIndentOrder + 1] - dIndentIndexes[intIndentOrder]);
                                }
                                else
                                {
                                    strWholeEntry = strPart6.Substring(dIndentIndexes[intIndentOrder]);
                                }

                                Match mEntry = rgxEnumeration.Match(strWholeEntry);
                                string strEntryText = strWholeEntry.Substring(mEntry.Captures[0].Length).Trim();
                                string strEnum = "";

                                //0, 3, 7, 12, 18
                                if (mEntry.Groups["inum"].Value == "0")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast0 = strEnum;
                                    strLast3 = "";
                                    strLast7 = "";
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0;
                                }
                                else if (mEntry.Groups["inum"].Value == "3")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast3 = strEnum;
                                    strLast7 = "";
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3;
                                }
                                else if (mEntry.Groups["inum"].Value == "7")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast7 = strEnum;
                                    strLast12 = "";
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3 + strLast7;
                                }
                                else if (mEntry.Groups["inum"].Value == "12")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast12 = strEnum;
                                    strLast18 = "";
                                    strCurrentNumber = strLast0 + strLast3 + strLast7 + strLast12;
                                }
                                else if (mEntry.Groups["inum"].Value == "18")
                                {
                                    strEnum = mEntry.Groups["enum"].Value;

                                    strLast18 = strEnum;
                                    strCurrentNumber = strLast0 + strLast3 + strLast7 + strLast12 + strLast18;
                                }
                                else
                                {
                                    throw new Exception("Lexical Entry Error");
                                }

                                dEntries.Add(strCurrentNumber, strEntryText);
                            }

                            blbLex.dLexicalEntries = dEntries;
                        }
                    }

                    dBLBGreekLexiconEntries.Add(intLexID, blbLex);
                }
            }
        }

        public void FillBLBConcordance(string strLanguage)
        {
            StreamReader srConcordance;
            Regex rgxNumbers = new Regex("[0-9]{1,}");

            if (strLanguage.ToLower() == "hebrew")
            {
                srConcordance = new StreamReader(@"Data\HebConc.js");

            }
            else if (strLanguage.ToLower() == "greek")
            {
                srConcordance = new StreamReader(@"Data\GrkConc.js");
            }
            else
            {
                throw (new Exception("Incorrect language given to FillBLBConcordance(string strLanguage)"));
            }

            while (!srConcordance.EndOfStream)
            {
                string[] strsLine = srConcordance.ReadLine().Split('=');
                int intNumber = Convert.ToInt32(rgxNumbers.Match(strsLine[0]).Value) + 1; //NOTE: This makes the number deviate from the data file by +1.
                string strConc = strsLine[1].Trim();

                strConc = strConc.TrimEnd(';');
                strConc = strConc.Trim((char)"'".ToCharArray()[0]);

                if (strLanguage.ToLower() == "hebrew")
                {
                    dBLBHebrewConcordance.Add(intNumber, new Dictionary<int, string>());
                }
                else //this must be greek due to the if then else trap above
                {
                    dBLBGreekConcordance.Add(intNumber, new Dictionary<int, string>());
                }

                if (strConc.Contains("|"))
                {
                    int intConcordanceID = 0;

                    foreach (string strConcRef in strConc.Split("|"))
                    {
                        intConcordanceID++;

                        if (strLanguage.ToLower() == "hebrew")
                        {
                            dBLBHebrewConcordance[intNumber].Add(intConcordanceID, strConcRef);
                        }
                        else //this must be greek due to the if then else trap above
                        {
                            dBLBGreekConcordance[intNumber].Add(intConcordanceID, strConcRef);
                        }
                    }
                }
                else
                {
                    {
                        if (strLanguage.ToLower() == "hebrew")
                        {
                            dBLBHebrewConcordance[intNumber].Add(1, strConc);
                        }
                        else //this must be greek due to the if then else trap above
                        {
                            dBLBGreekConcordance[intNumber].Add(1, strConc);
                        }
                    }
                }
            }
        }
    }
}