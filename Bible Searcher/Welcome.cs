using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bible_Searcher
{
    public partial class Welcome : Form
    {
        Dictionary<int, Verse> dVerses = new Dictionary<int, Verse>();
        Dictionary<int, BLBHebrewLexicon> dBLBHebrewLexiconEntries = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBGreekLexicon> dBLBGreekLexiconEntries = new Dictionary<int, BLBGreekLexicon>();
        Dictionary<int, Dictionary<int, string>> dBLBHebrewConcordance = new Dictionary<int, Dictionary<int, string>>(); //D<Concordance ID, D<Reference ID, Reference>>
        Dictionary<int, Dictionary<int, string>> dBLBGreekConcordance = new Dictionary<int, Dictionary<int, string>>(); //D<Concordance ID, D<Reference ID, Reference>>

        Dictionary<string, List<string>> dPhrasalConcordance = new Dictionary<string, List<string>>(); //D<Phrase, L<ReferenceStrings>>

        Dictionary<string, int> dSSCountsOrderedBySS = new Dictionary<string, int>(); //D<strongs sequence, count> (ordered by ss)
        Dictionary<string, int> dSSCountsOrderedByCount = new Dictionary<string, int>(); //D<strongs sequence, count> (ordered by count)
        Dictionary<string, int> dSSComplexCountsOrderedByCount = new Dictionary<string, int>(); //D<multiple strongs sequences for a single phrase, count>

        //Tests for BLB Hebrew Lexicon
        Dictionary<int, BLBHebrewLexicon> dHRoots = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHNonRoots = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHAramaic = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHNonAramaic = new Dictionary<int, BLBHebrewLexicon>();
        Dictionary<int, BLBHebrewLexicon> dHRootedAramaic = new Dictionary<int, BLBHebrewLexicon>();

        CrossReferences crossReferences = new CrossReferences();

        public Welcome()
        {
            InitializeComponent();

            //
            //Step Bible Hebrew Lexicon
            //
            //FillSTEPBibleHebrewLexicon();

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

            //BLB Hebrew Lexicon Derivatives
            dHRoots = HRoots();
            dHNonRoots = HNonRoots();
            dHAramaic = Aramaic();
            dHNonAramaic = NotAramaic();
            dHRootedAramaic = RootedAramaic();

            //
            //Bible
            //
            CalculateSVOs();

            //
            //Cross References
            //
            crossReferences.FillCrossReferences();

            //
            //Phrasal Concordance
            //
            CreatePhrasalConcordance();

            //HARD DRIVE WARNING: These files become large. For instance, they top 100MB at length 24.
            //54 is the last length that produces phrases with a count greater than 1 for the kjvstrongs.csv file
            for (int i = 2; i <= 54; i++) 
            {
                CreateChainedPhrasalConcordance(i);
            }

            //
            //Write CSV Files
            //
            WriteCSVFiles();

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

            srBible.ReadLine(); //Go past the header

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

                //Some phrases contain multiple spaces between words. Cut them out.
                foreach (string strWord in strLine.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                {
                    v.strText += strWord + " ";
                }
                v.strText = v.strText.Trim();

                dVerses.Add(v.intVerseID, v);
            }
        }

        public void ParseKJVStrongs()
        {
            //Regex rgxText = new(@"(?<phrase>[a-z'A-Z ]{0,})(?<strongs>(\{\({0,} [0-9]{1,} \){0,}\}){1,})");
            Regex rgxText = new(@"(?<phrase>[^{]{0,})(?<strongs>(\{\({0,} [0-9]{1,} \){0,}\} {0,}){1,})");
            string strLanguage = "";

            foreach (int intVerseID in dVerses.Keys)
            {
                int intPhraseID = 0;

                if (intVerseID <= 23145)
                {
                    strLanguage = "Hebrew";
                }
                else
                {
                    strLanguage = "Greek";
                }

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
                        string strStrongsNumber = capStrongs.Value.Trim().TrimStart('{').TrimEnd('}').TrimStart('(').TrimEnd(')').Trim();
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
                            if (strLanguage == "Hebrew")
                            {
                                if (intStrongsNumber == 853 || intStrongsNumber == 3487) //H853, H3487 means "Direct Object"
                                {
                                    dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intCaptureID].bObject = true;
                                }
                            }
                            
                            if (strPOSPart == "n" || strPOSPart == "adj" || strPOSPart == "adv" || strPOSPart == "pr" ||
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

        public void CreatePhrasalConcordance()
        {
            //intChainLength determines how many successive phrases will be considered for a chained phrasal concordance
            // default of 1 means to output a regular phrasal concordance

            var getConcordanceByPhraseCount = dPhrasalConcordance.OrderByDescending(a => a.Value.Count());
            StreamWriter swPhrasalConcordance = new StreamWriter(@"Data\PhrasalConcordance.csv");
            
            foreach (Verse v in dVerses.Values)
            {
                foreach (Phrase p in v.dPhrases.Values)
                {
                    string strPhrase = p.strPhraseText.ToLower();
                    string strReference = v.strBookName + " " + v.intChapterNumber.ToString() + ":" + v.intVerseNumber.ToString() + "." + p.intPhraseID.ToString();

                    if (!dPhrasalConcordance.ContainsKey(strPhrase))
                    {
                        dPhrasalConcordance.Add(strPhrase, new List<string>());
                    }

                    dPhrasalConcordance[strPhrase].Add(strReference);
                }
            }

            swPhrasalConcordance.WriteLine("Phrase ^ Reference ^ Count");

            foreach (var phrase in getConcordanceByPhraseCount)
            {
                foreach (string strRef in phrase.Value)
                {
                    swPhrasalConcordance.WriteLine(phrase.Key + " ^ " + strRef + " ^ " + phrase.Value.Count.ToString());
                }
            }

            swPhrasalConcordance.Close();
        }

        public void CreateChainedPhrasalConcordance(int intChainLength = 1)
        {
            StreamWriter swChainedPhrasalConcordance;
            StreamWriter swChainedPhrasalConcordanceCounts;
            Dictionary<string, string[]> dChainedPhrases = new Dictionary<string, string[]>();
            Dictionary<string, int> dCounts = new Dictionary<string, int>(); //D<Phrase, Count>
            string[] strsChainedPhrasesHistory = (string[])Array.CreateInstance(typeof(string), intChainLength); //A[this-intChainLength+1, this-intChainLength+2, .., this-intChainLength+intChainLength]
            string strChainedPhraseBuilder = "";

            //initialize strsChainedPhrasesHistory
            for (int i = 0; i < intChainLength; i++)
            {
                strsChainedPhrasesHistory[i] = "";
            }

            foreach (Verse v in dVerses.Values)
            {
                foreach (Phrase p in v.dPhrases.Values)
                {
                    string strPhrase = p.strPhraseText.ToLower();
                    string strReference = v.strBookName + " " + v.intChapterNumber.ToString() + ":" + v.intVerseNumber.ToString() + "." + p.intPhraseID.ToString();
                    string[] strsChainedPhrasesHistoryCopy = (string[])Array.CreateInstance(typeof(string), intChainLength);

                    //insert new phrase into history
                    for (int intCurrentChainedPhraseElement = 1; intCurrentChainedPhraseElement < intChainLength; intCurrentChainedPhraseElement++)
                    {
                        strsChainedPhrasesHistory[intCurrentChainedPhraseElement - 1] = strsChainedPhrasesHistory[intCurrentChainedPhraseElement];
                    }

                    strsChainedPhrasesHistory[intChainLength - 1] = strPhrase; //populate the last element of the array with strPhrase

                    strsChainedPhrasesHistory.CopyTo(strsChainedPhrasesHistoryCopy, 0);

                    dChainedPhrases.Add(strReference, strsChainedPhrasesHistoryCopy);
                }
            }

            swChainedPhrasalConcordance = new StreamWriter(@"Data\ChainedPhrasalConcordance-" + intChainLength.ToString() + ".csv");

            swChainedPhrasalConcordance.WriteLine("Reference ^ ChainEndingWithPhrase");

            foreach (string strReference in dChainedPhrases.Keys.OrderBy(a => a))
            {
                strChainedPhraseBuilder = "";

                swChainedPhrasalConcordance.Write(strReference + " ^ ");

                for (int intCurrentChainedPhraseElement = 0; intCurrentChainedPhraseElement < dChainedPhrases[strReference].Length; intCurrentChainedPhraseElement++)
                {
                    strChainedPhraseBuilder += dChainedPhrases[strReference][intCurrentChainedPhraseElement] + " ";
                }

                swChainedPhrasalConcordance.WriteLine(strChainedPhraseBuilder.Trim());

                //
                //Counts
                //

                if (!dCounts.ContainsKey(strChainedPhraseBuilder))
                {
                    dCounts.Add(strChainedPhraseBuilder, 0);
                }

                dCounts[strChainedPhraseBuilder]++;
            }

            swChainedPhrasalConcordance.Close();

            swChainedPhrasalConcordanceCounts = new StreamWriter(@"Data\ChainedPhrasalConcordance-" + intChainLength.ToString() + "-Counts.csv");

            swChainedPhrasalConcordanceCounts.WriteLine("Phrase ^ Count");

            foreach (string strPhrase in dCounts.Where(a=>a.Value > 1).OrderByDescending(a=>a.Value).Select(a=>a.Key))
            {
                swChainedPhrasalConcordanceCounts.WriteLine(strPhrase + " ^ " + dCounts[strPhrase].ToString());
            }

            swChainedPhrasalConcordanceCounts.Close();
        }

        public IOrderedEnumerable<string> GetEnglishPhraseOrderedByCount()
        {
            var oReturn =
                from phrase in dPhrasalConcordance.Keys
                orderby dPhrasalConcordance.Values.Count() descending
                select phrase;

            return oReturn;
        }

        public IOrderedEnumerable<string> GetEnglishPhraseOrderedByPhrase()
        {
            var oReturn =
                from phrase in dPhrasalConcordance.Keys
                orderby dPhrasalConcordance.Keys
                select phrase;

            return oReturn;
        }

        public void CalculateSVOs()
        {
            //Find subject
            //Find verbs
            //Find objects
            //Find relationships between these three
            //
            //Find relationships between subjects in similar contexts??

            //Doing this in Hebrew is too hard for me, so I'm opting to do it for the KJV text,
            // using the Strongs numbers as a phrase separator.

            int intHighestHLexID = dBLBHebrewLexiconEntries.Keys.Max();
            int intHighestGLexID = dBLBGreekLexiconEntries.Keys.Max();
            Regex rgxNumbers = new Regex(@"[0-9]{1,}");
            Dictionary<int, SVO> dSVOs = new Dictionary<int, SVO>();
            Dictionary<int, int> dSentenceLocations = new Dictionary<int, int>(); //D<SentenceID, VerseID> ; some verses have more than one sentence
            Dictionary<string, int> dPhraseCounts = new Dictionary<string, int>(); //D<phrase, count>
            bool bMakeNewSentence = false;
            Dictionary<string, int> dSSs = new Dictionary<string, int>(); //D<strongs sequence, count>
            StreamWriter swSSComplex = new StreamWriter(@"Data\Counts for Phrases with Multiple Strongs Numbers.csv"); //Strong's sequence contains a "-"
            StreamWriter swSSComplexTranslation = new StreamWriter(@"Data\Counts for Phrases with Multiple Strongs Numbers, First English Translation.csv"); //First english translation when the Strong's sequence contains a "-"
            string strStrongsLanguageIdentifier = "H";
            int intSSComplexLineID = 0;

            dSVOs.Add(1, new SVO()); //Start with a new sentence

            //Output files headers
            swSSComplex.WriteLine("PhraseID ^ ComplexPhraseStrongsSequence ^ PhraseCount");
            swSSComplexTranslation.WriteLine("PhraseID ^ ComplexPhraseEnglishText ^ PhraseCount");

            foreach (Verse v in dVerses.OrderBy(a => a.Key).Select(a => a.Value))
            {
                if (v.intBookNumber <= 39)
                {
                    strStrongsLanguageIdentifier = "H";
                }
                else
                {
                    strStrongsLanguageIdentifier = "G";
                }

                foreach (Phrase p in v.dPhrases.OrderBy(a => a.Key).Select(a => a.Value))
                {
                    string strSS = "";

                    foreach (StrongsSequence ss in p.dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Value))
                    {
                        strSS += strStrongsLanguageIdentifier + ss.strStrongsNumber + "-";
                    }

                    strSS = strSS.TrimEnd('-');

                    if (dSSs.ContainsKey(strSS))
                    {
                        dSSs[strSS]++;
                    }
                    else
                    {
                        dSSs.Add(strSS, 1);
                    }

                    ////Prepositional Phrases
                    //if (p.strPhraseText.Trim().Split().Count() == 1)
                    //{
                    //}
                    //else
                    //{
                    //    // if (PrepositionList.Contains(p.strPhraseText.Trim().Split()[0])){} //Prepositional Phrase

                    //    foreach (string strWord in p.strPhraseText.Trim().Split())
                    //    {

                    //    }
                    //}
                }
            }

            foreach (string strSS in dSSs.OrderBy(a => a.Key).Select(a => a.Key))
            {
                dSSCountsOrderedBySS.Add(strSS, dSSs[strSS]);
            }

            foreach (string strSS in dSSs.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                dSSCountsOrderedByCount.Add(strSS, dSSs[strSS]);
            }

            foreach (string strSS in dSSs.OrderByDescending(a => a.Value).Select(a => a.Key))
            {
                if (strSS.Contains('-'))
                {
                    string strTranslation = "";

                    intSSComplexLineID++;

                    dSSComplexCountsOrderedByCount.Add(strSS, dSSs[strSS]);
                    swSSComplex.WriteLine(intSSComplexLineID.ToString() + " ^ " + strSS + " ^ " + dSSs[strSS]);

                    foreach (string strSS1 in strSS.Split('-'))
                    {
                        if (strSS1.StartsWith('H'))
                        {
                            strTranslation += dBLBHebrewLexiconEntries[Convert.ToInt32(strSS1.Substring(1))].dAVTranslations.OrderByDescending(a => a.Value).Select(a => a.Key).First() + "-";
                        }
                        else if (strSS1.StartsWith('G'))
                        {
                            strTranslation += dBLBHebrewLexiconEntries[Convert.ToInt32(strSS1.Substring(1))].dAVTranslations.OrderByDescending(a => a.Value).Select(a => a.Key).First() + "-";
                        }
                        else
                        {
                            throw new Exception("Counts for Phrases with Multiple Strongs Numbers, First English Translation, SS Language Error");
                        }
                    }

                    strTranslation = strTranslation.TrimEnd('-');
                    swSSComplexTranslation.WriteLine(intSSComplexLineID.ToString() + " ^ " + strTranslation + " ^ " + dSSs[strSS]);
                }
            }

            swSSComplex.Close();
            swSSComplexTranslation.Close();
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
            ///NOTE: G4483 inserts 9 as the count for the AV translation "say" (there was no count given in the input file),
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
                else //this must be greek due to the if then else trap above the current while loop
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

        public void FillSTEPBibleHebrewLexicon()
        {
            //Gen.46.18-12	Gen.46.18-12	לְיַעֲקֹב	לְ/יַעֲקֹ֔ב	HR/Npm	H9005=ל=to/H3290=יַעֲקֹב=Jacob_§Jacob|Israel@Gen.25.26

            Dictionary<string, string> dNormalize = new Dictionary<string, string>(); //D<seen, normalized>
            STEPBibleHebrewLexicon stepBibleHebrewLexicon = new STEPBibleHebrewLexicon();

            dNormalize.Add("Gen", "Genesis");
            dNormalize.Add("Exo", "Exodus");
            dNormalize.Add("Lev", "Leviticus");
            dNormalize.Add("Num", "Numbers");
            dNormalize.Add("Deu", "Deuteronomy");
            dNormalize.Add("Jos", "Joshua");
            dNormalize.Add("Jdg", "Judges");
            dNormalize.Add("Rut", "Ruth");
            dNormalize.Add("1Sa", "1 Samuel");
            dNormalize.Add("2Sa", "2 Samuel");
            dNormalize.Add("1Ki", "1 Kings");
            dNormalize.Add("2Ki", "2 Kings");
            dNormalize.Add("1Ch", "1 Chronicles");
            dNormalize.Add("2Ch", "2 Chronicles");
            dNormalize.Add("Ezr", "Ezra");
            dNormalize.Add("Neh", "Nehemiah");
            dNormalize.Add("Est", "Esther");
            dNormalize.Add("Job", "Job");
            dNormalize.Add("Psa", "Psalms");
            dNormalize.Add("Pro", "Proverbs");
            dNormalize.Add("Ecc", "Ecclesiastes");
            dNormalize.Add("Sng", "Song of Solomon");
            dNormalize.Add("Isa", "Isaiah");
            dNormalize.Add("Jer", "Jeremiah");
            dNormalize.Add("Lam", "Lamentations");
            dNormalize.Add("Ezk", "Ezekiel");
            dNormalize.Add("Dan", "Daniel");
            dNormalize.Add("Hos", "Hosea");
            dNormalize.Add("Jol", "Joel");
            dNormalize.Add("Amo", "Amos");
            dNormalize.Add("Oba", "Obadiah");
            dNormalize.Add("Jon", "Jonah");
            dNormalize.Add("Mic", "Micah");
            dNormalize.Add("Nam", "Nahum");
            dNormalize.Add("Hab", "Habakkuk");
            dNormalize.Add("Zep", "Zephaniah");
            dNormalize.Add("Hag", "Haggai");
            dNormalize.Add("Zec", "Zechariah");
            dNormalize.Add("Mal", "Malachi");

            stepBibleHebrewLexicon.Read();
        }

        public void WriteCSVFiles()
        {
            StreamWriter swVerses = new StreamWriter(@"Data\Verses.csv");
            StreamWriter swBLBHebrewLexicon = new StreamWriter(@"Data\BLBHebrewLexicon.csv");
            StreamWriter swBLBGreekLexicon = new StreamWriter(@"Data\BLBGreekLexicon.csv");
            StreamWriter swBLBHebrewConcordance = new StreamWriter(@"Data\BLBHebrewConcordance.csv");
            StreamWriter swBLBGreekConcordance = new StreamWriter(@"Data\BLBGreekConcordance.csv");
            StreamWriter swEnglishPhraseCountsByPhrase = new StreamWriter(@"Data\EnglishPhraseCountsByPhrase.csv");
            StreamWriter swEnglishPhraseCountsByCount = new StreamWriter(@"Data\EnglishPhraseCountsByCount.csv");
            StreamWriter swSSBySS = new StreamWriter(@"Data\StrongsSequencesBySS.csv");
            StreamWriter swSSByCount = new StreamWriter(@"Data\StrongsSequencesByCount.csv");
            StreamWriter swCrossrefs = new StreamWriter(@"Data\CrossReferences.csv");

            int intRowCounter = 0;

            //
            //dVerses
            //

            swVerses.WriteLine("VerseID ^ BookNumber ^ BookName ^ ChapterNumber ^ VerseNumber ^ Phrases");

            foreach (int intVerseID in dVerses.Keys.OrderBy(a => a))
            {
                string strLine = intVerseID.ToString();

                strLine += " ^ " + dVerses[intVerseID].intBookNumber.ToString() + " ^ " + dVerses[intVerseID].strBookName + 
                    " ^ " + dVerses[intVerseID].intChapterNumber.ToString() + " ^ " + dVerses[intVerseID].intVerseNumber.ToString() + " ^ ";

                foreach (int intPhraseID in dVerses[intVerseID].dPhrases.OrderBy(a => a.Key).Select(a => a.Key))
                {
                    strLine += "{" + dVerses[intVerseID].dPhrases[intPhraseID].strPhraseText + " ";

                    foreach (int intSSID in dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences.OrderBy(a => a.Key).Select(a => a.Key))
                    {
                        strLine += "[" + dVerses[intVerseID].dPhrases[intPhraseID].dStrongsSequences[intSSID].strStrongsNumber + "] ";
                    }

                    strLine = strLine.TrimEnd();
                    strLine += "} ";
                }

                swVerses.WriteLine(strLine.TrimEnd());
            }

            swVerses.Close();

            //
            //dBLBHebrewLexiconEntries
            //

            swBLBHebrewLexicon.WriteLine("LexicalID ^ Aramaic ^ Pronunciation ^ Transliteration ^ TotalTranslation ^ AVTranslationsWithCount ^ Root ^ Connections ^ TWOTNumber ^ POS ^ LexicalEntries");

            foreach (int intLexID in dBLBHebrewLexiconEntries.Keys.OrderBy(a => a))
            {
                string strLine = intLexID.ToString() + " ^ ";

                strLine += dBLBHebrewLexiconEntries[intLexID].bAramaic.ToString() + " ^ " + dBLBHebrewLexiconEntries[intLexID].strPronunciation +
                    " ^ " + dBLBHebrewLexiconEntries[intLexID].strTransliteration + " ^ " + dBLBHebrewLexiconEntries[intLexID].intTotalTranslated.ToString() +
                    " ^ ";

                foreach (string strTranslation in dBLBHebrewLexiconEntries[intLexID].dAVTranslations.OrderByDescending(a => a.Value).Select(a=>a.Key))
                {
                    strLine += strTranslation + " " + dBLBHebrewLexiconEntries[intLexID].dAVTranslations[strTranslation].ToString() + "; ";
                }

                strLine.Remove(strLine.Length - 2); //remove the trailing "; "

                strLine += " ^ " + dBLBHebrewLexiconEntries[intLexID].bRoot.ToString() + " ^ " + dBLBHebrewLexiconEntries[intLexID].strConnection +
                    " ^ " + dBLBHebrewLexiconEntries[intLexID].strTWOTNumber + " ^ " + dBLBHebrewLexiconEntries[intLexID].strPOS + " ^ ";

                foreach (string strLexicalEntry in dBLBHebrewLexiconEntries[intLexID].dLexicalEntries.Keys.OrderBy(a => a))
                {
                    strLine += strLexicalEntry + " " + dBLBHebrewLexiconEntries[intLexID].dLexicalEntries[strLexicalEntry] + "; ";
                }

                strLine.Remove(strLine.Length - 2); //remove the trailing "; "

                swBLBHebrewLexicon.WriteLine(strLine);
            }

            swBLBHebrewLexicon.Close();

            //
            //dBLBGreekLexiconEntries
            //

            swBLBGreekLexicon.WriteLine("LexicalID ^ Pronunciation ^ Transliteration ^ TotalTranslation ^ TDNT ^ Conjugations ^ AVTranslationsWithCount ^ Root ^ Connections ^ TWOTNumber ^ POS ^ LexicalEntries");

            foreach (int intLexID in dBLBGreekLexiconEntries.Keys.OrderBy(a => a))
            {
                string strLine = intLexID.ToString() + " ^ ";

                strLine += dBLBGreekLexiconEntries[intLexID].strPronunciation + " ^ " + dBLBGreekLexiconEntries[intLexID].strTransliteration +
                    " ^ " + dBLBGreekLexiconEntries[intLexID].intTotalTranslated.ToString() + " ^ ";

                if (dBLBGreekLexiconEntries[intLexID].dExtraTDNTInformation.Count() > 0)
                {
                    foreach (string strTDNT in dBLBGreekLexiconEntries[intLexID].dExtraTDNTInformation.Keys.OrderBy(a => a))
                    {
                        strLine += strTDNT + "; ";
                    }

                    strLine = strLine.Remove(strLine.Length - 2).Trim(); //remove the trailing "; "
                }

                strLine += " ^ ";

                if (dBLBGreekLexiconEntries[intLexID].lConjugated.Count() > 0)
                {
                    foreach (string strConjugated in dBLBGreekLexiconEntries[intLexID].lConjugated)
                    {
                        strLine += strConjugated + "; ";
                    }

                    strLine = strLine.Remove(strLine.Length - 2).Trim(); //remove the trailing "; "
                }

                strLine += " ^ ";

                if (dBLBGreekLexiconEntries[intLexID].dAVTranslations.Count() > 0)
                {
                    foreach (string strTranslation in dBLBGreekLexiconEntries[intLexID].dAVTranslations.OrderByDescending(a => a.Value).Select(a => a.Key))
                    {
                        strLine += strTranslation + " " + dBLBGreekLexiconEntries[intLexID].dAVTranslations[strTranslation].ToString() + "; ";
                    }

                    strLine = strLine.Remove(strLine.Length - 2).Trim(); //remove the trailing "; "
                }

                strLine += " ^ " + dBLBGreekLexiconEntries[intLexID].bRoot.ToString() + " ^ " + dBLBGreekLexiconEntries[intLexID].strConnection +
                    " ^ " + dBLBGreekLexiconEntries[intLexID].strTWOTNumber + " ^ " + dBLBGreekLexiconEntries[intLexID].strPOS + " ^ ";

                if (dBLBGreekLexiconEntries[intLexID].dLexicalEntries.Count() > 0)
                {
                    foreach (string strLexicalEntry in dBLBGreekLexiconEntries[intLexID].dLexicalEntries.Keys.OrderBy(a => a))
                    {
                        strLine += dBLBGreekLexiconEntries[intLexID].dLexicalEntries[strLexicalEntry] + "; ";
                    }

                    strLine = strLine.Remove(strLine.Length - 2).Trim(); //remove the trailing "; "
                }

                swBLBGreekLexicon.WriteLine(strLine);
            }

            swBLBGreekLexicon.Close();

            //
            //dBLBHebrewConcordance
            //

            swBLBHebrewConcordance.WriteLine("GroupID ^ ReferenceID ^ Reference");

            foreach (int intGroupID in dBLBHebrewConcordance.Keys.OrderBy(a => a))
            {
                foreach (int intReferenceID in dBLBHebrewConcordance[intGroupID].Keys.OrderBy(a => a))
                {
                    swBLBHebrewConcordance.WriteLine(intGroupID.ToString() + " ^ " + intReferenceID.ToString() + 
                        " ^ " + dBLBHebrewConcordance[intGroupID][intReferenceID]);
                }
            }

            swBLBHebrewConcordance.Close();

            //
            //dBLBGreekConcordance
            //

            swBLBGreekConcordance.WriteLine("GroupID ^ ReferenceID ^ Reference");

            foreach (int intGroupID in dBLBGreekConcordance.Keys.OrderBy(a => a))
            {
                foreach (int intReferenceID in dBLBGreekConcordance[intGroupID].Keys.OrderBy(a => a))
                {
                    swBLBGreekConcordance.WriteLine(intGroupID.ToString() + " ^ " + intReferenceID.ToString() +
                        " ^ " + dBLBGreekConcordance[intGroupID][intReferenceID]);
                }
            }

            swBLBGreekConcordance.Close();

            //
            //dEnglishPhraseCountsOrderedByPhrase
            //

            swEnglishPhraseCountsByPhrase.WriteLine("RowID ^ Phrase ^ Count");

            intRowCounter = 0;
            foreach (string strPhrase in GetEnglishPhraseOrderedByPhrase())
            {
                intRowCounter++;

                swEnglishPhraseCountsByPhrase.WriteLine(intRowCounter.ToString() + " ^ " + strPhrase + " ^ " + dPhrasalConcordance[strPhrase]);
            }

            swEnglishPhraseCountsByPhrase.Close();

            //
            //dEnglishPhraseCountsOrderedByPhrase
            //

            swEnglishPhraseCountsByCount.WriteLine("RowID ^ Count ^ Phrase");

            intRowCounter = 0;
            foreach (string strPhrase in GetEnglishPhraseOrderedByCount())
            {
                intRowCounter++;

                swEnglishPhraseCountsByCount.WriteLine(intRowCounter.ToString() + " ^ " + dPhrasalConcordance[strPhrase] + " ^ " + strPhrase);
            }

            swEnglishPhraseCountsByCount.Close();

            //
            //dSSCountsOrderedBySS
            //

            swSSBySS.WriteLine("RowID ^ StrongsSequence ^ Count");

            intRowCounter = 0;
            foreach (string strSS in dSSCountsOrderedBySS.Keys.OrderBy(a => a))
            {
                intRowCounter++;

                swSSBySS.WriteLine(intRowCounter.ToString() + " ^ " + strSS + " ^ " + dSSCountsOrderedBySS[strSS].ToString());
            }

            swSSBySS.Close();

            //
            //dSScountsOrderedByCount
            //

            swSSByCount.WriteLine("RowID ^ Count ^ StrongsSequence");

            intRowCounter = 0;
            foreach (string strSS in dSSCountsOrderedByCount.OrderByDescending(a => a.Value).Select(a=>a.Key))
            {
                intRowCounter++;

                swSSByCount.WriteLine(intRowCounter.ToString() + " ^ " + dSSCountsOrderedByCount[strSS].ToString() + " ^ " + strSS);
            }

            swSSByCount.Close();

            //
            //crossReferences
            //

            swCrossrefs.WriteLine("RowID ^ ReferencedPassage ^ ReferencingPassagesWithVoteCounts");

            intRowCounter = 0;
            foreach (string strReferenced in crossReferences.dCrossReferences.Keys.OrderBy(a => a))
            {
                string strReferencing = "";

                intRowCounter++;

                swCrossrefs.Write(intRowCounter.ToString() + " ^ " + strReferenced + " ^ ");

                foreach (CrossReferenceDataFrame crdf in crossReferences.dCrossReferences[strReferenced])
                {
                    strReferencing += crdf.strReferencingBeginning + "-" + crdf.strReferencingEnding +
                        "%" + crdf.intVotes.ToString() + ", "; //referenced ^ referencing 1:1%votecount, complex 1:1-referencing 1:1%votecount, referencing 1:2%votecount
                }

                strReferencing.Remove(strReferencing.Length - 2); //remove the trailing ", "

                swCrossrefs.WriteLine(strReferencing);
            }

            swCrossrefs.Close();
        }
    }
}