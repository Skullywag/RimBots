using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimBots
{
    public class JobGiver_Mine : ThinkNode_JobGiver
    {
        public PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }
        public Job JobOnThing(Pawn pawn, Thing t)
        {
            if (!t.def.mineable)
            {
                return null;
            }
            if (Find.DesignationManager.DesignationAt(t.Position, DesignationDefOf.Mine) == null)
            {
                return null;
            }
            if (!pawn.CanReserve(t, 1))
            {
                return null;
            }
            bool flag = false;
            for (int i = 0; i < 8; i++)
            {
                IntVec3 c = t.Position + GenAdj.AdjacentCells[i];
                if (c.InBounds() && c.Standable())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int j = 0; j < 8; j++)
                {
                    IntVec3 c2 = t.Position + GenAdj.AdjacentCells[j];
                    if (c2.InBounds())
                    {
                        if (c2.Walkable() && !c2.Standable())
                        {
                            Thing firstHaulable = c2.GetFirstHaulable();
                            if (firstHaulable != null && firstHaulable.def.passability == Traversability.PassThroughOnly)
                            {
                                return HaulAIUtility.HaulAsideJobFor(pawn, firstHaulable);
                            }
                        }
                    }
                }
                return null;
            }
            return new Job(JobDefOf.Mine, t, 1500, true);
        }
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            WorkGiver_Scanner scanner = new WorkGiver_Miner();
            TargetInfo targetInfo = TargetInfo.Invalid;
            Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
            IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
            Thing thing;
            if (scanner.Prioritized)
            {
                IEnumerable<Thing> enumerable2 = enumerable;
                if (enumerable2 == null)
                {
                    enumerable2 = Find.ListerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
                }
                Predicate<Thing> validator = predicate;
                thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
            }
            else
            {
                Predicate<Thing> validator = predicate;
                thing = GenClosest.ClosestThingReachable(pawn.Position, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, enumerable, scanner.LocalRegionsToScanFirst, enumerable != null);
            }
            if (thing != null)
            {
                targetInfo = thing;
                //workGiver_Scanner = scanner;
            }
            if (targetInfo.HasThing)
            {
                WorkGiver_Miner miner = new WorkGiver_Miner();
                return miner.JobOnThing(pawn, targetInfo.Thing);
            }
            return null;
        }
    }
}
