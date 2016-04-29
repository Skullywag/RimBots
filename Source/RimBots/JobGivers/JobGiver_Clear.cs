using System;using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimBots
{
    public class JobGiver_Clear : ThinkNode_JobGiver
    {
        public PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }
        public IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            return Find.AreaSnowClear.ActiveCells;
        }
        public bool ShouldSkip(Pawn pawn)
        {
            return Find.AreaSnowClear.TrueCount == 0;
        }

        public bool HasJobOnCell(Pawn pawn, IntVec3 c)
        {
            return Find.SnowGrid.GetDepth(c) >= 0.2f && pawn.CanReserveAndReach(c, PathEndMode.Touch, pawn.NormalMaxDanger(), 1);
        }

        public Job JobOnCell(Pawn pawn, IntVec3 c)
        {
            return new Job(JobDefOf.ClearSnow, c);
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            WorkGiver_Scanner scanner = new WorkGiver_ClearSnow();
            IntVec3 position = pawn.Position;
            float num2 = 99999f;
            TargetInfo targetInfo = TargetInfo.Invalid;
            foreach (IntVec3 current in scanner.PotentialWorkCellsGlobal(pawn))
            {
                float lengthHorizontalSquared = (current - position).LengthHorizontalSquared;
                if (lengthHorizontalSquared < num2 && !current.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, current))
                {
                    targetInfo = current;
                    //workGiver_Scanner = scanner;
                    num2 = lengthHorizontalSquared;
                }
            }
            if (targetInfo.Cell != TargetInfo.Invalid)
            {
                WorkGiver_ClearSnow clearer = new WorkGiver_ClearSnow();
                return clearer.JobOnCell(pawn, targetInfo.Cell);
            }
            return null;
        }
    }
}
