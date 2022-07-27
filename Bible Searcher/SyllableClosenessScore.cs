using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bible_Searcher
{
    internal class SyllableClosenessScore
    {
        //Creates a Syllable-wise Closeness score

        Dictionary<int, Dictionary<int, string>> dWordSyllables = new Dictionary<int, Dictionary<int, string>>(); //D<Strong's Number, D<Syllable Order ID, Syllable>>
        Dictionary<int, Dictionary<int, double>> dCloseness = new Dictionary<int, Dictionary<int, double>>(); //D<Strong's Number of Target Word, D<Strong's Number of Comparing Word, Relative Closeness>> where Relative Closeness is from 0.0 to <1.0 and is calculated as the number of same syllables IN ORDER (without stress punctuation) divided by greatest number of syllables in either word; if all syllables, for instance were the same except the first one, the score would be 0.0, because closeness is counted in order through the syllables

        public void Parse(string strInputFileRelativePath)
        {
            StreamReader srInput = new(strInputFileRelativePath);
            int intLineNumber = 0;

            while (!srInput.EndOfStream)
            {   
                string strLine = srInput.ReadLine();
                
                intLineNumber++;

                if (intLineNumber > 1) //The first line has Column Headers
                {
                    int intColumnNumber = 0;
                    int intStrongsNumber = 0;
                    int intPronunciationColumn = 0;

                    if (strInputFileRelativePath.ToLower().Contains("blbgreek"))
                    {
                        intPronunciationColumn = 2;
                    }

                    if (strInputFileRelativePath.ToLower().Contains("blbhebrew"))
                    {
                        intPronunciationColumn = 3;
                    }
    
                    foreach (string strColumn in strLine.Split('^'))
                    {
                        intColumnNumber++;

                        if (intColumnNumber == 1)
                        {
                            intStrongsNumber = Convert.ToInt32(strColumn.Trim());
                            dWordSyllables.Add(intStrongsNumber, new Dictionary<int, string>());
                        }

                        if (intColumnNumber == intPronunciationColumn)
                        {
                            int intSyllableOrderID = 0;

                            foreach (string strSyllable in strColumn.Split('-'))
                            {
                                intSyllableOrderID++;

                                dWordSyllables[intStrongsNumber].Add(intSyllableOrderID, strSyllable.Trim("'".ToCharArray()[0]).Trim()); //Trim away stress punctuation
                            }
                        }
                    } 
                }
            }
        }

        public void Examine(string strOutputFileRelativePath)
        {
            Dictionary<string, List<int>> dNonZero = new Dictionary<string, List<int>>(); //D<First Syllable, Strong's Numbers of Words which first syllable matches>
            string strCurrentSyllable = "";
            StreamWriter swOutput = new StreamWriter(strOutputFileRelativePath);
            int intCurrentGroupID = 0;

            swOutput.WriteLine("GroupID ^ StrongsNumberOne ^ StrongsNumberTwo ^ SyllableWiseClosenessScore ^ NumberOfMatchingSyllables" +
                " ^ WordOneSyllableCount ^ WordTwoSyllableCount ^ WordOneText ^ WordTwoText");

            foreach (int intStrongsNumber in dWordSyllables.Keys.OrderBy(a => a))
            {
                strCurrentSyllable = dWordSyllables[intStrongsNumber][1];

                if (!dNonZero.ContainsKey(strCurrentSyllable))
                {
                    dNonZero.Add(strCurrentSyllable, new List<int>());
                }

                dNonZero[strCurrentSyllable].Add(intStrongsNumber);
            }

            foreach (string strFirstSyllable in dNonZero.Keys.OrderBy(a => a))
            {
                intCurrentGroupID++;

                foreach (int intStrongsNumber in dNonZero[strFirstSyllable])
                {
                    int intCurrentSyllableCount = dWordSyllables[intStrongsNumber].Count;

                    foreach (int intStrongsNumberCompare in dNonZero[strFirstSyllable].Where(a=>a>intStrongsNumber)) //Where: Don't compare a word with itself or any word that has already compared with it; eg. 5 ^ 4 && 4 ^ 5
                    {
                        int intCompareSyllableCount = dWordSyllables[intStrongsNumberCompare].Count;
                        double dblScore = 0.0;
                        int intCommonSyllableCount = 0;
                        bool bSwap = false; //if the first word has more syllables than the second word, then swap them, making the first word to always have fewer or the same number of syllables

                        foreach (int intSyllablePosition in dWordSyllables[intStrongsNumber].Keys) //does the first word have more syllables than the second word?
                        {
                            if (dWordSyllables[intStrongsNumberCompare].ContainsKey(intSyllablePosition))
                            {

                                if (dWordSyllables[intStrongsNumber][intSyllablePosition] == //do the two words share this syllable?
                                    dWordSyllables[intStrongsNumberCompare][intSyllablePosition])
                                {
                                    intCommonSyllableCount++;
                                }
                                else
                                {
                                    break; 
                                }
                            }
                            else
                            {
                                break; 
                            }
                        }

                        if (intCurrentSyllableCount > intCompareSyllableCount)
                        {
                            bSwap = true;
                        }

                        if (bSwap == false)
                        {
                            Write(ref swOutput, intCommonSyllableCount, intCompareSyllableCount, intStrongsNumber, intStrongsNumberCompare, intCurrentGroupID);
                        }
                        else
                        {
                            Write(ref swOutput, intCommonSyllableCount, intCurrentSyllableCount, intStrongsNumberCompare, intStrongsNumber, intCurrentGroupID);
                        }

                        
                    }
                }
            }

            swOutput.Close();
        }

        public double Write(ref StreamWriter swOutput, int intCommonSyllableCount, int intCompareSyllableCount, int intStrongsNumber, int intStrongsNumberCompare, int intCurrentGroupID)
        {
            double dblReturn = (double)intCommonSyllableCount / (double)intCompareSyllableCount;

            if (!dCloseness.ContainsKey(intStrongsNumber))
            {
                dCloseness.Add(intStrongsNumber, new Dictionary<int, double>());
            }

            dCloseness[intStrongsNumber].Add(intStrongsNumberCompare, dblReturn);

            swOutput.Write(intCurrentGroupID.ToString() + " ^ " + intStrongsNumber.ToString() + " ^ " +
                intStrongsNumberCompare.ToString() + " ^ " + dCloseness[intStrongsNumber][intStrongsNumberCompare].ToString("0.00") +
                " ^ " + intCommonSyllableCount.ToString() + " ^ " + dWordSyllables[intStrongsNumber].Count.ToString() +
                " ^ " + dWordSyllables[intStrongsNumberCompare].Count.ToString() + " ^ ");

            for (int intSyllablePosition = 1; intSyllablePosition <= dWordSyllables[intStrongsNumber].Count; intSyllablePosition++)
            {
                if (intSyllablePosition < dWordSyllables[intStrongsNumber].Count)
                {
                    swOutput.Write(dWordSyllables[intStrongsNumber][intSyllablePosition] + "-");
                }
                else
                {
                    swOutput.Write(dWordSyllables[intStrongsNumber][intSyllablePosition]);
                }
            }

            swOutput.Write(" ^ ");


            for (int intSyllablePosition = 1; intSyllablePosition <= dWordSyllables[intStrongsNumberCompare].Count; intSyllablePosition++)
            {
                if (intSyllablePosition < dWordSyllables[intStrongsNumberCompare].Count)
                {
                    swOutput.Write(dWordSyllables[intStrongsNumberCompare][intSyllablePosition] + "-");
                }
                else
                {
                    swOutput.Write(dWordSyllables[intStrongsNumberCompare][intSyllablePosition]);
                }
            }

            swOutput.WriteLine();

            return dblReturn;
        }

    }
}
