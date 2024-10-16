﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal class BuffSimulationItemBase : BuffSimulationItem
    {
        internal readonly BuffStackItem _buffStackItem;
        internal readonly long _totalDuration;

        protected internal BuffSimulationItemBase(BuffStackItem buffStackItem) : base(buffStackItem.Start, buffStackItem.Duration)
        {
            _buffStackItem = buffStackItem;
            _totalDuration = buffStackItem.TotalDuration;
        }

        public override void OverrideEnd(long end)
        {
            Duration = Math.Clamp(end - Start, 0, Duration);
        }

        public override int GetActiveStacks()
        {
            return GetStacks();
        }

        public override int GetStacks()
        {
            return 1;
        }

        public override int GetActiveStacks(AbstractSingleActor actor)
        {
            return GetStacks(actor);
        }

        public override int GetStacks(AbstractSingleActor actor)
        {
            return GetActiveSources().Any(x => x == actor.AgentItem) ? 1 : 0;
        }

        public override IEnumerable<long> GetActualDurationPerStack()
        {
            return [ GetActualDuration() ];
        }

        public override long GetActualDuration()
        {
            return _totalDuration;
        }

        public override IEnumerable<AgentItem> GetSources()
        {
            return [ _buffStackItem.Src ];
        }

        public override IEnumerable<AgentItem> GetActiveSources()
        {
            return GetSources();
        }

        public override void SetBuffDistributionItem(BuffDistribution distribs, long start, long end, long buffID)
        {
            long cDur = GetClampedDuration(start, end);
            if (cDur == 0)
            {
                return;
            }

            Dictionary<AgentItem, BuffDistributionItem> distrib = distribs.GetDistrib(buffID);
            AgentItem agent = _buffStackItem.Src;
            AgentItem seedAgent = _buffStackItem.SeedSrc;
            if (distrib.TryGetValue(agent, out BuffDistributionItem toModify))
            {
                toModify.IncrementValue(cDur);
            }
            else
            {
                distrib.Add(agent, new BuffDistributionItem(
                    cDur,
                    0, 0, 0, 0, 0));
            }

            if (_buffStackItem.IsExtension)
            {
                if (distrib.TryGetValue(agent, out toModify))
                {
                    toModify.IncrementExtension(cDur);
                }
                else
                {
                    distrib.Add(agent, new BuffDistributionItem(
                        0,
                        0, 0, 0, cDur, 0));
                }
            }

            if (agent != seedAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.IncrementExtended(cDur);
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, 0, 0, cDur));
                }
            }

            if (agent == ParserHelper._unknownAgent)
            {
                if (distrib.TryGetValue(seedAgent, out toModify))
                {
                    toModify.IncrementUnknownExtension(cDur);
                }
                else
                {
                    distrib.Add(seedAgent, new BuffDistributionItem(
                        0,
                        0, 0, cDur, 0, 0));
                }
            }
        }
    }
}
