using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class BLBHebrewLexicon
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

        public int intLexID = 0;
        public string strTransliteration = "";
        public string strPronunciation = "";
        public Dictionary<string, int> dAVTranslations = new Dictionary<string, int>();
        public int intTotalTranslated = 0;
        public bool bRoot = false;
        public bool bAramaic = false;
        public string strConnection = ""; //corresponding to #, from #, from an unused root (apparently meaning to turn), etc.
        public string strTWOTNumber = "";
        public string strPOS = "";
        public string strGender = "";
        public Dictionary<string, string> dLexicalEntries = new Dictionary<string, string>(); //D<Entry number with letter sub-indexes, Lexical Entry>
    }
}
