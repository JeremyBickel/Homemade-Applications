using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class Verse
    {
        public int intVerseID = 0;
        public string strBookName = "";
        public int intBookNumber = 0;
        public int intChapterNumber = 0;
        public int intVerseNumber = 0;
        public string strText = "";
        public Dictionary<int, Phrase> dPhrases = new Dictionary<int, Phrase>(); //D<PhraseID, Phrase>
    }

    internal class Phrase
    {
        public int intPhraseID = 0;
        public string strPhraseText = "";
        public Dictionary<int, StrongsSequence> dStrongsSequences = new Dictionary<int, StrongsSequence>(); //D<StrongsSequenceID, StrongsSequence>
    }

    internal class StrongsSequence
    {
        public int intStrongsSequenceID = 0;
        public string strStrongsNumber = "";
        public bool bParenthecized = false;
        public bool bThing = false;
        public bool bAction = false;
        public bool bObject = false;
        public bool bPreposition = false;
    }

    internal class NullTranslation
    {
        public int intVerseID = 0;
        public int intPhraseID = 0;
        public Dictionary<int, StrongsSequence> dStrongsSequences = new Dictionary<int, StrongsSequence>();
    }

    internal class SVO
    {
        public Dictionary<int, string> dSubjects = new Dictionary<int, string>(); //D<Phrase ID, Phrase Text>
        public Dictionary<int, string> dVerbs = new Dictionary<int, string>(); //D<Phrase ID, Phrase Text>
        public Dictionary<int, ObjectRelation> dObjectRelations = new Dictionary<int, ObjectRelation>(); //D<Phrase ID, ObjectRelation object>
    }

    internal class ObjectRelation
    {
        public string strObject = "";
        public string strRelation = ""; //eg. the preposition
    }
}
