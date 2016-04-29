using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimBots
{
    public class JobGiver_GrowSow : ThinkNode_JobGiver
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
            if (c.IsForbidden(pawn))
            {
                return null;
            }
            if (!GenPlant.GrowthSeasonNow(c))
            {
                return null;
            }
            if (wantedPlantDef == null)
            {
                DetermineWantedPlantDef(c);
                if (wantedPlantDef == null)
                {
                    return null;
                }
            }
            Plant plant = c.GetPlant();
            if (plant != null)
            {
                if (plant.def == wantedPlantDef)
                {
                    return null;
                }
                if (plant.def.plant.blockAdjacentSow)
                {
                    if (!pawn.CanReserve(plant, 1) || plant.IsForbidden(pawn))
                    {
                        return null;
                    }
                    return new Job(JobDefOf.CutPlant, plant);
                }
            }
            Thing thing = GenPlant.AdjacentSowBlocker(wantedPlantDef, c);
            if (thing != null)
            {
                Plant plant2 = thing as Plant;
                if (plant2 != null && pawn.CanReserve(plant2, 1) && !plant2.IsForbidden(pawn))
                {
                    Zone_Growing zone_Growing = Find.ZoneManager.ZoneAt(plant2.Position) as Zone_Growing;
                    if (zone_Growing == null || zone_Growing.GetPlantDefToGrow() != plant2.def)
                    {
                        return new Job(JobDefOf.CutPlant, plant2);
                    }
                }
                return null;
            }
            List<Thing> list = Find.ThingGrid.ThingsListAt(c);
            int i = 0;
            while (i < list.Count)
            {
                Thing thing2 = list[i];
                if (thing2.def == wantedPlantDef)
                {
                    return null;
                }
                if (thing2.def.BlockPlanting)
                {
                    if (!pawn.CanReserve(thing2, 1))
                    {
                        return null;
                    }
                    if (thing2.def.category == ThingCategory.Plant)
                    {
                        if (!thing2.IsForbidden(pawn))
                        {
                            return new Job(JobDefOf.CutPlant, thing2);
                        }
                        return null;
                    }
                    else
                    {
                        if (thing2.def.EverHaulable)
                        {
                            return HaulAIUtility.HaulAsideJobFor(pawn, thing2);
                        }
                        return null;
                    }
                }
                else
                {
                    i++;
                }
            }
            if (!wantedPlantDef.CanEverPlantAt(c) || !GenPlant.GrowthSeasonNow(c) || !pawn.CanReserve(c, 1))
            {
                return null;
            }
            return new Job(JobDefOf.Sow, c)
            {
                plantDefToSow = wantedPlantDef
            };
        }
        protected override Job TryGiveTerminalJob(Pawn pawn)
        {
            WorkGiver_Scanner scanner = new WorkGiver_GrowerSow();
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
                WorkGiver_GrowerSow grower = new WorkGiver_GrowerSow();
                return grower.JobOnCell(pawn, targetInfo.Cell);
            }
            return null;
        }
    }
}
