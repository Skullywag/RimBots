using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimBots
{
    public class JobGiver_Milk : ThinkNode_JobGiver
    {
        protected JobDef JobDef
        {
            get
            {
                return JobDefOf.Milk;
            }
        }

        protected CompHasGatherableBodyResource GetComp(Pawn animal)
        {
            return animal.TryGetComp<CompMilkable>();
        }
        public PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }
        public bool HasJobOnThing(Pawn pawn, Thing t)
        {
            Pawn pawn2 = t as Pawn;
            return pawn2 != null && pawn2.RaceProps.Animal && !pawn2.Downed && pawn2.CasualInterruptibleNow() && this.GetComp(pawn2) != null && this.GetComp(pawn2).ActiveAndFull && pawn.CanReserve(pawn2, 1);
        }

        public Job JobOnThing(Pawn pawn, Thing t)
        {
            return new Job(this.JobDef, t);
        }
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            WorkGiver_Scanner scanner = new WorkGiver_Milk();
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
                WorkGiver_Milk milker = new WorkGiver_Milk();
                return milker.JobOnThing(pawn, targetInfo.Thing);
            }
            return null;
        }
    }
}
