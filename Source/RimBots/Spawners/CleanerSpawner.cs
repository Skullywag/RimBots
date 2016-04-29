using RimWorld;
using Verse;

namespace RimBots
{
    class CleanerSpawner : Building
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDef.Named("Rimba"), Faction.OfColony);
            GenSpawn.Spawn(pawn, this.Position);
        }
        public override void Tick()
        {
            base.Tick();
            this.Destroy();
        }
    }
}
