using RimWorld;
using Verse;

namespace RimBots
{
    class HaulerSpawner : Building
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Quadcopter"), Faction.OfColony);
            GenSpawn.Spawn(pawn, this.Position);
        }
        public override void Tick()
        {
            base.Tick();
            this.Destroy();
        }
    }
}
