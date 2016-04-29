using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using RimWorld;
namespace RimBots
{
    public class StockGenerator_Bots : StockGenerator
    {
        public List<string> pawnList;
        public override IEnumerable<Thing> GenerateThings()
        {
            int typeCount = 3;
            int amountCount = 1;
            pawnList.Add("Quadcopter");
            pawnList.Add("Rimba");
            //pawnList.Add("Grower");
            //pawnList.Add("Milker");
            //pawnList.Add("Shearer");
            //pawnList.Add("Miner");
            for (int i = 0; i < typeCount; i++)
            {
                for (int j = 0; j < amountCount; j++)
                {
                    System.Random rand = new System.Random();
                    int pick = rand.Next(0, pawnList.Count);
                    Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named(pawnList[pick]), Faction.OfColony);
                    yield return pawn;
                }
            }
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef.category == ThingCategory.Pawn;
        }
    }
}