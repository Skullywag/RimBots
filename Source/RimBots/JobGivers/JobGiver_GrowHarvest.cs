using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimBots
{
    public class JobGiver_GrowHarvest : ThinkNode_JobGiver
    {
        protected static ThingDef wantedPlantDef;
        protected void DetermineWantedPlantDef(IntVec3 c)
        {
            IPlantToGrowSettable plantToGrowSettable = c.GetEdifice() as IPlantToGrowSettable;
            if (plantToGrowSettable == null)
            {
                plantToGrowSettable = (Find.ZoneManager.ZoneAt(c) as IPlantToGrowSettable);
            }
            if (plantToGrowSettable == null)
            {
                wantedPlantDef = null;
            }
            else
            {
                wantedPlantDef = plantToGrowSettable.GetPlantDefToGrow();
            }
        }
        public Job JobOnCell(Pawn pawn, IntVec3 c)
        {
            if (wantedPlantDef == null)
            {
                DetermineWantedPlantDef(c);
                if (wantedPlantDef == null)
                {
                    return null;
                }
            }
            Plant plant = c.GetPlant();
            if (plant == null)
            {
                return null;
            }
            if (plant.def != wantedPlantDef)
            {
                return null;
            }
            if (!plant.def.plant.Harvestable || plant.LifeStage != PlantLifeStage.Mature)
            {
                return null;
            }
            if (!pawn.CanReserve(plant, 1))
            {
                return null;
            }
            return new Job(JobDefOf.Harvest, plant);
        }
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            WorkGiver_Scanner scanner = new WorkGiver_GrowerHarvest();
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
                WorkGiver_GrowerHarvest grower = new WorkGiver_GrowerHarvest();
                return grower.JobOnCell(pawn, targetInfo.Cell);
            }
            return null;
        }
    }
}
