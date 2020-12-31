﻿using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.AbstractBuffSimulator;

namespace GW2EIEvtcParser.EIData
{
    internal class HealingLogic : QueueLogic
    {
        public HealingLogic(int capacity) : base(capacity)
        {

        }

        private struct CompareHealing
        {

            private static uint GetHealing(BuffStackItem stack)
            {
                return stack.SeedSrc.Healing;
            }

            public static int Compare(BuffStackItem x, BuffStackItem y)
            {
                return -GetHealing(x).CompareTo(GetHealing(y));
            }
        }

        protected override void Sort(ParsedEvtcLog log, List<BuffStackItem> stacks)
        {
            stacks.Sort(CompareHealing.Compare);
        }
    }
}
