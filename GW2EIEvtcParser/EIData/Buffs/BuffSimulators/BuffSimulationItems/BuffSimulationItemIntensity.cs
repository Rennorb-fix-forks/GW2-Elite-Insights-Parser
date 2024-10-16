﻿using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulationItemIntensity : BuffSimulationItemStack
    {
        public BuffSimulationItemIntensity(IReadOnlyList<BuffStackItem> stacks) : base(stacks)
        {
            Duration = Stacks.Max(x => x.Duration);
        }

        public override void OverrideEnd(long end)
        {
            long maxDur = 0;
            foreach (BuffSimulationItemBase stack in Stacks)
            {
                stack.OverrideEnd(end);

                if(stack.Duration > maxDur) { maxDur = stack.Duration; }
            }
            Duration = maxDur;
        }

        public override int GetActiveStacks()
        {
            return Stacks.Length;
        }

        public override IEnumerable<AgentItem> GetActiveSources()
        {
            return GetSources();
        }

        public override int GetActiveStacks(AbstractSingleActor actor)
        {
            return GetStacks(actor);
        }
        public override long GetActualDuration()
        {
            return GetActualDurationPerStack().Max();
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long boonid)
        {
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }
            foreach (BuffSimulationItemBase item in Stacks)
            {
                item.SetBuffDistributionItem(distribs, start, end, boonid);
            }
        }
    }
}
