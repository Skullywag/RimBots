using System;using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimBots
{
    public class JobGiver_Clean : ThinkNode_JobGiver
    {
        private int MinTicksSinceThickened = 600;
        public PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.OnCell;
            }
        }
        public ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Filth);
            }
        }
        public int LocalRegionsToScanFirst
        {
            get
            {
                return 4;
            }
        }
        public IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return ListerFilthInHomeArea.FilthInHomeArea;
        }
        public bool HasJobOnThing(Pawn pawn, Thing t)
        {
            if (pawn.Faction != Faction.OfColony)
            {
                return false;
            }
            Filth filth = t as Filth;
            return filth != null && Find.AreaHome[filth.Position] && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1) && filth.TicksSinceThickened >= this.MinTicksSinceThickened;
        }
        public Job JobOnThing(Pawn pawn, Thing t)
        {
            return new Job(JobDefOf.Clean, t);
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            TargetInfo targetInfo = TargetInfo.Invalid;
            Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && this.HasJobOnThing(pawn, t);
            IEnumerable<Thing> enumerable = this.PotentialWorkThingsGlobal(pawn);
            Predicate<Thing> validator = predicate;
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, this.PotentialWorkThingRequest, this.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, enumerable, this.LocalRegionsToScanFirst, enumerable != null);
            if (thing != null)
            {
                targetInfo = thing;
            }
            if (targetInfo.HasThing)
            {
                return this.JobOnThing(pawn, targetInfo.Thing);
            }
            return null;
        }
    }
}
