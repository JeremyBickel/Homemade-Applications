using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class BLBGreekLexicon
    {
        public int intLexID = 0;
        public string strTransliteration = "";
        public string strPronunciation = "";
        public Dictionary<string, int> dAVTranslations = new Dictionary<string, int>();
        public Dictionary<string, string> dExtraTDNTInformation = new Dictionary<string, string>(); //D<translation, tdnt reference>; rarely TDNT information will appear in the AV translation data in curly brackets; example G5207
        public List<string> lConjugated = new List<string>(); //these translations are representative of various conjugated forms present in the text, represented by their counts being in parenthesis
        public int intTotalTranslated = 0;
        public bool bRoot = false;
        public string strConnection = ""; //corresponding to #, from #, from an unused root (apparently meaning to turn), etc.
        public string strTWOTNumber = "";
        public string strPOS = "";
        public string strGender = "";
        public Dictionary<string, string> dLexicalEntries = new Dictionary<string, string>(); //D<Entry number with letter sub-indexes, Lexical Entry>

    }
}
