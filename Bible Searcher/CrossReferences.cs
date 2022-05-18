using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class CrossReferences
    {
        //encode data from Open Bible's cross_references.txt
        //ignore the third column, which are the number of votes given for the cross reference
        StreamReader srCRs = new StreamReader(@"Data\cross_references.txt");
        public Dictionary<string, List<CrossReferenceDataFrame>> dCrossReferences = new Dictionary<string, List<CrossReferenceDataFrame>>(); //D<Referenced Scripture, L<CrossReferenceDataFrame>>

        public void FillCrossReferences()
        {
            while (!srCRs.EndOfStream)
            {
                CrossReferenceDataFrame data = new CrossReferenceDataFrame(); 
                string strLine = srCRs.ReadLine();
                string[] strsLine = strLine.Split();
                string strNormalFormReference = "";
                Dictionary<string, string> dNormalizedReference = CreateNormalFormReference(strsLine[1]);

                data.strReferenced = CreateNormalFormReference(strsLine[0]).First().Key;
                data.strReferencingBeginning = CreateNormalFormReference(strsLine[1]).First().Key;
                data.strReferencingEnding = CreateNormalFormReference(strsLine[1]).First().Value;
                data.intVotes = Convert.ToInt32(strsLine[2]);

                if (!dCrossReferences.ContainsKey(data.strReferenced))
                {
                    dCrossReferences.Add(data.strReferenced, new List<CrossReferenceDataFrame>());
                }

                dCrossReferences[data.strReferenced].Add(data);
            }
        }

        private Dictionary<string, string> CreateNormalFormReference(string strReferenceIn)
        {
            Dictionary<string, string> dReturn = new Dictionary<string, string>();
            Dictionary<string, string> dNormalize = new Dictionary<string, string>(); //D<short, normal>
            string strBeginningIn = "";
            string strEndingIn = "";
            string strBeginningOut = "";
            string strEndingOut = "";
            int intPartCounter = 0;

            dNormalize.Add("Gen", "Genesis");
            dNormalize.Add("Exod", "Exodus");
            dNormalize.Add("Lev", "Leviticus");
            dNormalize.Add("Num", "Numbers");
            dNormalize.Add("Deut", "Deuteronomy");
            dNormalize.Add("Josh", "Joshua");
            dNormalize.Add("Judg", "Judges");
            dNormalize.Add("Ruth", "Ruth");
            dNormalize.Add("1Sam", "1 Samuel");
            dNormalize.Add("2Sam", "2 Samuel");
            dNormalize.Add("1Kgs", "1 Kings");
            dNormalize.Add("2Kgs", "2 Kings");
            dNormalize.Add("1Chr", "1 Chronicles");
            dNormalize.Add("2Chr", "2 Chronicles");
            dNormalize.Add("Ezra", "Ezra");
            dNormalize.Add("Neh", "Nehemiah");
            dNormalize.Add("Esth", "Esther");
            dNormalize.Add("Job", "Job");
            dNormalize.Add("Ps", "Psalms");
            dNormalize.Add("Prov", "Proverbs");
            dNormalize.Add("Eccl", "Ecclesiastes");
            dNormalize.Add("Song", "Song of Solomon");
            dNormalize.Add("Isa", "Isaiah");
            dNormalize.Add("Jer", "Jeremiah");
            dNormalize.Add("Lam", "Lamentations");
            dNormalize.Add("Ezek", "Ezekiel");
            dNormalize.Add("Dan", "Daniel");
            dNormalize.Add("Hos", "Hosea");
            dNormalize.Add("Joel", "Joel");
            dNormalize.Add("Amos", "Amos");
            dNormalize.Add("Obad", "Obadiah");
            dNormalize.Add("Jonah", "Jonah");
            dNormalize.Add("Mic", "Micah");
            dNormalize.Add("Nah", "Nahum");
            dNormalize.Add("Hab", "Habakkuk");
            dNormalize.Add("Zeph", "Zephaniah");
            dNormalize.Add("Hag", "Haggai");
            dNormalize.Add("Zech", "Zechariah");
            dNormalize.Add("Mal", "Malachi");
            dNormalize.Add("Matt", "Matthew");
            dNormalize.Add("Mark", "Mark");
            dNormalize.Add("Luke", "Luke");
            dNormalize.Add("John", "John");
            dNormalize.Add("Acts", "Acts");
            dNormalize.Add("Rom", "Romans");
            dNormalize.Add("1Cor", "1 Corinthians");
            dNormalize.Add("2Cor", "2 Corinthians");
            dNormalize.Add("Gal", "Galatians");
            dNormalize.Add("Eph", "Ephesians");
            dNormalize.Add("Phil", "Philippians");
            dNormalize.Add("Col", "Colossians");
            dNormalize.Add("1Thess", "1 Thessalonians");
            dNormalize.Add("2Thess", "2 Thessalonians");
            dNormalize.Add("1Tim", "1 Timothy");
            dNormalize.Add("2Tim", "2 Timothy");
            dNormalize.Add("Titus", "Titus");
            dNormalize.Add("Phlm", "Philemon");
            dNormalize.Add("Heb", "Hebrews");
            dNormalize.Add("Jas", "James");
            dNormalize.Add("1Pet", "1 Peter");
            dNormalize.Add("2Pet", "2 Peter");
            dNormalize.Add("1John", "1 John");
            dNormalize.Add("2John", "2 John");
            dNormalize.Add("3John", "3 John");
            dNormalize.Add("Jude", "Jude");
            dNormalize.Add("Rev", "Revelation");

            if (strReferenceIn.Contains('-'))
            {
                string[] strsReferences = strReferenceIn.Split('-');
                strBeginningIn = strsReferences[0];
                strEndingIn = strsReferences[1];
            }
            else
            {
                strBeginningIn = strReferenceIn;
            }


            foreach (string strReferencePart in strBeginningIn.Split('.'))
            {
                intPartCounter++;

                switch (intPartCounter)
                {
                    case 1:
                        strBeginningOut = dNormalize[strReferencePart];
                        break;
                    case 2:
                        strBeginningOut += " " + strReferencePart.ToString();
                        break;
                    case 3:
                        strBeginningOut += ":" + strReferencePart.ToString();
                        break;
                }
            }

            if (strEndingIn != "")
            {
                intPartCounter = 0;

                foreach (string strReferencePart in strEndingIn.Split('.'))
                {
                    intPartCounter++;

                    switch (intPartCounter)
                    {
                        case 1:
                            strEndingOut = dNormalize[strReferencePart];
                            break;
                        case 2:
                            strEndingOut += " " + strReferencePart.ToString();
                            break;
                        case 3:
                            strEndingOut += ":" + strReferencePart.ToString();
                            break;
                    }
                }
            }
            else
            {
                strEndingOut = "";
            }

            dReturn.Add(strBeginningOut, strEndingOut);

            return dReturn;
        }

    }

    class CrossReferenceDataFrame
    {
        public string strReferenced = "";
        public string strReferencingBeginning = "";
        public string strReferencingEnding = "";
        public int intVotes = 0;
    }
}
