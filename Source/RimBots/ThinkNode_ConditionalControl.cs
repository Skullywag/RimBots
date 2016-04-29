using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
namespace RimBots
{
    public class ThinkNode_ConditionalControl : ThinkNode_Conditional
    {
        private bool rcuActive = false;
        protected override bool Satisfied(Pawn pawn)
        {
            IEnumerable<Building> rcus = Find.ListerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("RCU"));
            foreach (Building_RCU current in rcus)
            {
                if (current.UsableNow)
                {
                    rcuActive = true;
                }
            }
            if (rcuActive)
            {
                rcuActive = false;
                return false;
            } else
            {
                rcuActive = false;
                return true;
            }
        }
    }
}
