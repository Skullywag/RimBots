using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Verse.AI;
using Verse;
namespace RimBots
{
    public class PawnGrower : Pawn
    {
        public override void Tick()
        {
            base.Tick();
            // Update glow grid
            Find.GlowGrid.MarkGlowGridDirty(Position);
            // the following two should not be necesarry, but for some reason do seem to be.
            Find.MapDrawer.MapMeshDirty(Position, MapMeshFlag.GroundGlow);
            Find.MapDrawer.MapMeshDirty(Position, MapMeshFlag.Things);
        }
    }
}
