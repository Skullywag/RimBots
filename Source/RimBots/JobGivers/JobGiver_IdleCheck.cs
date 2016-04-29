using RimWorld;
using System;
using Verse.AI;
using Verse;
namespace RimBots
{
    public class JobGiver_IdleCheck : ThinkNode_JobGiver
    {
        private const int WaitTime = 250;
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            return new Job(JobDefOf.WaitDowned)
            {
                expiryInterval = WaitTime
            };
        }
    }
}
