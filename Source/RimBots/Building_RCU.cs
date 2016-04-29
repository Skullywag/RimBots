using System;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
namespace RimBots
{
    public class Building_RCU : Building
    {
        private CompPowerTrader powerComp;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.powerComp = base.GetComp<CompPowerTrader>();
        }

        public override void TickRare()
        {
            base.TickRare();
        }

        public virtual bool UsableNow
        {
            get
            {
                return this.powerComp == null || this.powerComp.PowerOn;
            }
        }
    }
}

