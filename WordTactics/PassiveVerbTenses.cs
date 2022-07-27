using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTactics
{
    internal class PassiveVerbTenses
    {
        //In the active example of simple present tense, the company ships the computers.
        //  Here, the company is doing the action.
        //In the passive example of simple present tense, computers are foregrounded instead of the company.
        //  In this case, it doesn’t matter who sent the computers.
        bool bSimplePresent_Active; //The company ships the computers to many foreign countries. 
        bool bSimplePresent_Passive; //Computers are shipped to many foreign countries.

        //Verbs ending in -ing
        //In the active example of present progressive tense, the factors of the storm are emphasized rather than the storm itself.
        //In the passive example of present progressive tense, the storm is focused on rather than the factors of the storm.
        //Use the passive voice if you do not wish to detail the factors of the storm and instead wish to present the storm
        //  as the focus of the sentence.
        bool bPresentProgressive_Active; //A combination of wind, pressure, and moisture is forming the thunderstorm.
        bool bPresentProgressive_Passive; //A thunderstorm is being formed.
        
        bool bSimplePast_Active; //The postal carrier delivered the package yesterday.
        bool bSimplePast_Passive; //The package was delivered yesterday.

        //Verbs ending in -ing
        bool bPastProgressive_Active; //The producer was making an announcement.
        bool bPastProgressive_Passive; //An announcement was being made.

        //In the active example of the future tense, the representative is specified as the person who will pick up the computer.
        //  In this case, the owners of the computer know to look out for a specific person who represents this company.
        //In the passive example of the future tense, we do not know who will pick up the computer, just that it will be picked up.
        //Use passive voice if you do not want to specify who will pick up the computer.
        bool bFuture_Active; //Our representative will pick up the computer.
        bool bFuture_Passive; //The computer will be picked up.

        bool bPresentPerfect_Active; //Someone has made the arrangements for us.
        bool bPresentPerfect_Passive; //The arrangements have been made for us.

        bool bPastPerfect_Active; //They had given us visas for three months.
        bool bPastPerfect_Passive; //Visas had been given to us for three months.

        bool bFuturePerfect_Active; //By next month we will have finished this job.
        bool bFuturePerfect_Passive; //By next month this job will have been finished.

        //can, could, be able to, may, might, must, will, would
        //In the active voice example of the modal verb, the second person pronoun, you, is directly addressed
        //  as the person who can use the computer.
        //In the passive voice example of the modal verb, no single person is addressed.  Therefore, the computer
        //  can be used by anyone.
        bool bModal_Active; //You can use the computer.
        bool bModal_Passive; //The computer can be used.
    }
}
