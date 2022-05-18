using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTactics
{
    internal class Article
    {
        //Indefinite articles ("a" and "an") are chosen based on the sound of the following word.
        //The actual first letter of the following word doesn't matter.

        //If the following word sounds like it starts with a consonant, use "a".
        //Examples: 
        //a cat
        //a dog
        //a purple onion
        //a buffalo
        //a big apple
        //a union
        //a united front
        //a unicorn
        //a used napkin
        //a U.S.ship
        //a one-legged man

        //If the following word sounds like it starts with a vowel, use "an".
        //Examples: 
        //an apricot
        //an egg
        //an Indian
        //an orbit
        //an uprising
        //an honorable peace
        //an honest error
    }
}
