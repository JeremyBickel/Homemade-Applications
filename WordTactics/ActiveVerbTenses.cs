using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTactics
{
    internal class ActiveVerbTenses
    {
        // Active Verb Tenses
        bool bSimplePresent_PresentOrActionCondition = false; //I hear you.  Here comes the bus.
        bool bSimplePresent_GeneralTruths = false; //There are thirty days in September.
        bool bSimplePresent_NonActionOrHabitualAction = false; //I like music.  I run on Tuesdays and Sundays.
        bool bSimplePresent_FutureTime = false; //The train leaves at 4:00 p.m.
        bool bPresentProgressive_ActivityInProgress = false; //I am playing soccer now.
        bool bPresentProgressive_VerbsOfPerception = false; //He is feeling sad.
        bool bSimplePast_CompletedAction = false; //We visited the museum yesterday.
        bool bSimplePast_CompletedCondition = false; //The weather was rainy last week.
        bool bPastProgressive_PastActionOverPeriodOfTime = false; //They were climbing for twenty-seven days.
        bool bPastProgressive_PastActionInterruptedByAnother = false; //We were eating dinner when she told me.
        bool bFuture_WithWillOrWont = false; //[Activity or event that will or won't exist or happen in the future] I'll get up late tomorrow.  I won't get up early.
        bool bFuture_WithGoingTo = false; //[future in relation to circumstances in the present] I'm hungry. I'm going to get something to eat.
        bool bPresentPerfect_VerbsOfStateFromPastThroughPresent = false; //[With verbs of state that begin in the past and lead up to and include the present] He has lived here for many years.
        bool bPresentPerfect_HabitualOrContinuedAction = false; //He has worn glasses all his life.
        bool bPresentPerfect_IndefinitePast = false; //[With events occurring at an indefinite or unspecified time in the past using the words "ever", "never" or "before"] Have you ever been to Tokyo before?
        bool PresentPerfectProgressive_FromPastThroughPossibleFuture = false; //[To express duration of an action that began in the past, has continued into the present, and may continue into the future] David has been working for two hours, and he hasn't finished yet.
        bool bPastPerfect_PastCompletedBeforeOtherPast = false; //[To describe a past event or condition completed before another event in the past] When I arrived home, he had already called.
        bool bPastPerfect_InReportedSpeech = false; //Jane said that she had gone to the movies.
        bool bFuturePerfect = false; //[To express action that will be completed by or before a specified time in the future] By next month we will have finished the job. He won't have finished his work until 2:00.
    }
}
