using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.Collections;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIBuilders.Protobuf.EXT.HealingStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders {

    public static class ProtobufBuilder
    {
        public static void WriteTo(Stream output, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var ext = new HealingStats(log, usedSkills, usedBuffs);
            using var os = new CodedOutputStream(output, true);
            ext.WriteTo(os);
        }
    }

}

namespace GW2EIBuilders.Protobuf.EXT.HealingStats {
    using static HealingStatsExtensionHandler;
    using HealingDistribution = PlayerDetails.Types.HealingDistribution;

    partial class HealingStats
    {
        public HealingStats(ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : this()
        {
            var phases = log.FightData.GetPhases(log);
            this.healingPhases_.Capacity = phases.Count;
            this.playerHealingCharts_.Capacity = phases.Count;
            foreach (var phase in phases)
            {
                this.healingPhases_.Add(new Phase(phase, log));
                this.playerHealingCharts_.Add(PlayerChart.BuildPlayersHealingGraphData(log, phase));
            }

            this.playerHealingDetails_.Capacity = log.Friendlies.Count;
            foreach (var actor in log.Friendlies)
            {
                this.playerHealingDetails_.Add(PlayerDetails.BuildPlayerHealingData(log, actor, usedSkills, usedBuffs));
            }
        }
    }

    partial class Phase
    {
        public Phase(PhaseData phase, ParsedEvtcLog log)
        {
            this.outgoingHealingStats_.Capacity = log.Friendlies.Count;
            foreach (var actor in log.Friendlies)
            {
                var outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(null, log, phase.Start, phase.End);
                this.outgoingHealingStats_.Add(GetOutgoingHealingStatData(outgoingHealingStats));
            }

            this.outgoingHealingStatsTargets_.Capacity = log.Friendlies.Count;
            foreach (var actor in log.Friendlies)
            {
                var playerData = new Int32LL(log.Friendlies.Count);
                foreach (var target in log.Friendlies)
                {
                    playerData.Add(GetOutgoingHealingStatData(actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End)));
                }
                this.outgoingHealingStatsTargets_.Add(playerData);
            }

            this.incomingHealingStats_.Capacity = log.Friendlies.Count;
            foreach (var actor in log.Friendlies)
            {
                var incomingHealintStats = actor.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End);
                this.incomingHealingStats_.Add(GetIncomingHealingStatData(incomingHealintStats));
            }
        }

        static Int32L GetOutgoingHealingStatData(EXTFinalOutgoingHealingStat outgoingHealingStats)
        {
            return new(4) { Inner = {
                    outgoingHealingStats.Healing,
                    outgoingHealingStats.HealingPowerHealing + outgoingHealingStats.HybridHealing,
                    outgoingHealingStats.ConversionHealing,
                    //outgoingHealingStats.HybridHealing,
                    outgoingHealingStats.DownedHealing
                }
            };
        }

        static Int32L GetIncomingHealingStatData(EXTFinalIncomingHealingStat incomingHealintStats)
        {
            return new(4) { Inner = {
                    incomingHealintStats.Healed,
                    incomingHealintStats.HealingPowerHealed + incomingHealintStats.HybridHealed,
                    incomingHealintStats.ConversionHealed,
                    //incomingHealintStats.HybridHealed,
                    incomingHealintStats.DownedHealed
                }
            };
        }
    }

    partial class PlayerChart
    {
        public static List<PlayerChart> BuildPlayersHealingGraphData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<PlayerChart>(log.Friendlies.Count);
            foreach (var actor in log.Friendlies)
            {
                list.Add(new PlayerChart(log, phase, actor));
            }
            return list;
        }

        private PlayerChart(ParsedEvtcLog log, PhaseData phase, SingleActor p)
        {

            this.healing_ = new PlayerDamageChart_int()
            {
                Total   = { p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, EXTHealingType.All) },
                Taken   = { p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, EXTHealingType.All) },
                Targets = { Capacity = log.Friendlies.Count },
            };

            var hybridHealingPower = Int32L.From(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, EXTHealingType.HealingPower));
            var hybrid = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, EXTHealingType.Hybrid);
            for (int i = 0; i < hybrid.Count; i++)
            {
                hybridHealingPower.Inner[i] += hybrid[i];
            }
            var hybridHealingPowerReceived = Int32L.From(p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, EXTHealingType.HealingPower));
            var hybridReceived = p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, EXTHealingType.Hybrid);
            for (int i = 0; i < hybridReceived.Count; i++)
            {
                hybridHealingPowerReceived.Inner[i] += hybridReceived[i];
            }
            this.healingPowerHealing_ = new PlayerDamageChart_int()
            {
                Total   = { hybridHealingPower.Inner },
                Taken   = { hybridHealingPowerReceived.Inner },
                Targets = { Capacity = log.Friendlies.Count },
            };

            this.conversionBasedHealing_ = new PlayerDamageChart_int()
            {
                Total   = { p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, null, EXTHealingType.ConversionBased) },
                Taken   = { p.EXTHealing.Get1SHealingReceivedList(log, phase.Start, phase.End, null, EXTHealingType.ConversionBased) },
                Targets = { Capacity = log.Friendlies.Count },
            };

            foreach (var target in log.Friendlies)
            {
                this.healing_.Targets.Add(Int32L.From(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, EXTHealingType.All)));
                
                hybridHealingPower.Inner.Clear();
                hybridHealingPower.AddRange(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, EXTHealingType.HealingPower));
                hybrid = p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, EXTHealingType.Hybrid);
                for (int i = 0; i < hybrid.Count; i++)
                {
                    hybridHealingPower.Inner[i] += hybrid[i];
                }
                this.healingPowerHealing_.Targets.Add(hybridHealingPower);
                
                this.conversionBasedHealing_.Targets.Add(Int32L.From(p.EXTHealing.Get1SHealingList(log, phase.Start, phase.End, target, EXTHealingType.ConversionBased)));
            }
        }
    }

    partial class PlayerDetails
    {
        public static PlayerDetails BuildPlayerHealingData(ParsedEvtcLog log, SingleActor actor, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var phases = log.FightData.GetPhases(log);
            var minions = actor.GetMinions(log);
            var dto = new PlayerDetails
            {
                HealingDistributions = { Capacity = phases.Count },
                HealingDistributionsTargets = { Capacity = phases.Count },
                IncomingHealingDistributions = { Capacity = phases.Count },
                Minions = { Capacity = minions.Count  },
            };
            foreach (var phase in phases)
            {
                dto.HealingDistributions.Add(HealingDistribution.BuildFriendlyHealingDistData(log, actor, null, phase, usedSkills, usedBuffs));
                var dmgTargetsDto = new Types.HealingDistributionL();
                dmgTargetsDto.Inner.Capacity = log.Friendlies.Count;
                foreach (var target in log.Friendlies)
                {
                    dmgTargetsDto.Add(HealingDistribution.BuildFriendlyHealingDistData(log, actor, target, phase, usedSkills, usedBuffs));
                }
                dto.HealingDistributionsTargets.Add(dmgTargetsDto);
                dto.IncomingHealingDistributions.Add(HealingDistribution.BuildIncomingHealingDistData(log, actor, phase, usedSkills, usedBuffs));
            }
            foreach (var pair in minions)
            {
                dto.Minions.Add(BuildFriendlyMinionsHealingData(log, actor, pair.Value, usedSkills, usedBuffs));
            }

            return dto;
        }

        private static PlayerDetails BuildFriendlyMinionsHealingData(ParsedEvtcLog log, SingleActor actor, Minions minion, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            var phases = log.FightData.GetPhases(log);
            var dto = new PlayerDetails
            {
                HealingDistributions = { Capacity = phases.Count },
                HealingDistributionsTargets = { Capacity = phases.Count },
                IncomingHealingDistributions = { Capacity = phases.Count },
            };
            foreach (var phase in phases)
            {
                var dmgTargetsDto = new Types.HealingDistributionL();
                dmgTargetsDto.Inner.Capacity = log.Friendlies.Count;
                foreach (var target in log.Friendlies)
                {
                    dmgTargetsDto.Add(HealingDistribution.BuildFriendlyMinionHealingDistData(log, actor, minion, target, phase, usedSkills, usedBuffs));
                }
                dto.HealingDistributionsTargets.Add(dmgTargetsDto);
                dto.HealingDistributions.Add(HealingDistribution.BuildFriendlyMinionHealingDistData(log, actor, minion, null, phase, usedSkills, usedBuffs));
                dto.IncomingHealingDistributions.Add(HealingDistribution.BuildFriendlyMinionIncomingHealingDistData(log, minion, null, phase, usedSkills, usedBuffs));
            }
            return dto;
        }

        partial class Types {
            partial class HealingDistribution
            {
                static HealingDistributionItem GetHealingToItem(SkillItem skill, IEnumerable<EXTHealingEvent> healingLogs, Dictionary<SkillItem, IEnumerable<CastEvent>>? castLogsBySkill, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBoons, BuffsContainer boons, PhaseData phase)
                {
                    int totalhealing = 0,
                        totaldownedhealing = 0,
                        minhealing = int.MaxValue,
                        maxhealing = int.MinValue,
                        hits = 0;
                    bool isIndirectHealing = false;
                    foreach (EXTHealingEvent dl in healingLogs)
                    {
                        isIndirectHealing = isIndirectHealing || dl is EXTNonDirectHealingEvent;
                        int curdmg = dl.HealingDone;
                        totalhealing += curdmg;
                        hits++;
                        if (curdmg < minhealing) { minhealing = curdmg; }
                        if (curdmg > maxhealing) { maxhealing = curdmg; }
                        if (dl.AgainstDowned)
                        {
                            totaldownedhealing += dl.HealingDone;
                        }

                    }
                    if (isIndirectHealing)
                    {
                        if (!usedBoons.ContainsKey(skill.ID))
                        {
                            if (boons.BuffsByIds.TryGetValue(skill.ID, out var buff))
                            {
                                usedBoons.Add(buff.ID, buff);
                            }
                            else
                            {
                                SkillItem aux = skill;
                                var auxBoon = new Buff(aux.Name, aux.ID, aux.Icon);
                                usedBoons.Add(auxBoon.ID, auxBoon);
                            }
                        }
                    }
                    else
                    {
                        usedSkills.TryAdd(skill.ID, skill);
                    }

                    IEnumerable<CastEvent>? clList = null;
                    if (castLogsBySkill != null && castLogsBySkill.Remove(skill, out clList))
                    {
                        isIndirectHealing = false;
                    }

                    long timeSpentCasting = 0, timeSpentCastingNoInterrupt = 0;
                    int numberOfCast = 0, numberOfCastNoInterrupt = 0, timeWasted = 0, timeSaved = 0;
                    long minTimeSpentCasting = 0, maxTimeSpentCasting = 0;
                    if (clList != null)
                    {
                        (timeSpentCasting, timeSpentCastingNoInterrupt, minTimeSpentCasting, maxTimeSpentCasting, numberOfCast, numberOfCastNoInterrupt, timeSaved, timeWasted) = DamageDistributionDto.GetCastValues(clList, phase);
                    }
                    return new(isIndirectHealing, skill, totalhealing, minhealing, maxhealing, numberOfCast, timeWasted, timeSaved, hits, timeSpentCasting, totaldownedhealing, minTimeSpentCasting, maxTimeSpentCasting, timeSpentCastingNoInterrupt, numberOfCastNoInterrupt);
                }

                public static HealingDistribution BuildIncomingHealingDistData(ParsedEvtcLog log, SingleActor p, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
                {
                    var incomingHealingStats = p.EXTHealing.GetIncomingHealStats(null, log, phase.Start, phase.End);
                    var dto = new HealingDistribution
                    {
                        contributedHealing_ = incomingHealingStats.Healed,
                        contributedDownedHealing_ = incomingHealingStats.DownedHealed
                    };
                    var healingLogs = p.EXTHealing.GetIncomingHealEvents(null, log, phase.Start, phase.End);
                    foreach (var group in healingLogs.GroupBy(x => x.Skill))
                    {
                        dto.Distribution.Add(GetHealingToItem(group.Key, group, null, usedSkills, usedBuffs, log.Buffs, phase));
                    }
                    return dto;
                }


                static void BuildHealingDistBodyData(RepeatedField<HealingDistributionItem> list, ParsedEvtcLog log, IEnumerable<CastEvent> casting, IEnumerable<EXTHealingEvent> healingLogs, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, PhaseData phase)
                {
                    var castLogsBySkill = casting.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.AsEnumerable());
                    foreach (var group in healingLogs.GroupBy(x => x.Skill))
                    {
                        list.Add(GetHealingToItem(group.Key, group, castLogsBySkill, usedSkills, usedBuffs, log.Buffs, phase));
                    }
                }

                public static HealingDistribution BuildFriendlyHealingDistData(ParsedEvtcLog log, SingleActor actor, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
                {
                    var outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End);
                    var casting = actor.GetIntersectingCastEvents(log, phase.Start, phase.End);
                    var healingLogs = actor.EXTHealing.GetJustActorOutgoingHealEvents(target, log, phase.Start, phase.End);
                    var dist = new HealingDistribution() {
                        contributedHealing_ = outgoingHealingStats.ActorHealing,
                        contributedDownedHealing_ = outgoingHealingStats.ActorDownedHealing,
                        totalHealing_ = outgoingHealingStats.Healing,
                        totalCasting_ = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start)),
                    };
                    BuildHealingDistBodyData(dist.distribution_, log, casting, healingLogs, usedSkills, usedBuffs, phase);
                    return dist;
                }

                public static HealingDistribution BuildFriendlyMinionHealingDistData(ParsedEvtcLog log, SingleActor actor, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
                {
                    var outgoingHealingStats = actor.EXTHealing.GetOutgoingHealStats(target, log, phase.Start, phase.End);
                    var casting = minions.GetIntersectingCastEvents(log, phase.Start, phase.End);
                    var healingLogs = minions.EXTHealing.GetOutgoingHealEvents(target, log, phase.Start, phase.End);                    
                    var dist = new HealingDistribution() {
                        contributedHealing_ = healingLogs.Sum(x => x.HealingDone),
                        contributedDownedHealing_ = healingLogs.Sum(x => x.AgainstDowned ? x.HealingDone : 0),
                        totalHealing_ = outgoingHealingStats.Healing,
                        totalCasting_ = casting.Sum(cl => Math.Min(cl.EndTime, phase.End) - Math.Max(cl.Time, phase.Start)),
                        
                    };
                    BuildHealingDistBodyData(dist.distribution_, log, casting, healingLogs, usedSkills, usedBuffs, phase);
                    return dist;
                }

                public static HealingDistribution BuildFriendlyMinionIncomingHealingDistData(ParsedEvtcLog log, Minions minions, SingleActor? target, PhaseData phase, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
                {
                    var healingLogs = minions.EXTHealing.GetIncomingHealEvents(target, log, phase.Start, phase.End).ToList();
                    var dto = new HealingDistribution
                    {
                        ContributedHealing = healingLogs.Sum(x => x.HealingDone),
                        ContributedDownedHealing = healingLogs.Sum(x => x.AgainstDowned ? x.HealingDone : 0),
                    };
                    foreach (var group in healingLogs.GroupBy(x => x.Skill))
                    {
                        dto.Distribution.Add(GetHealingToItem(group.Key, group, null, usedSkills, usedBuffs, log.Buffs, phase));
                    }
                    return dto;
                }
            }

            partial class HealingDistributionItem
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public HealingDistributionItem(bool isIndirectHealing, SkillItem skill, int totalhealing, int minhealing, int maxhealing, int numberOfCast, int timeWasted, int timeSaved, int hits, long timeSpentCasting, int totaldownedhealing, long minTimeSpentCasting, long maxTimeSpentCasting, long timeSpentCastingNoInterrupt, int numberOfCastNoInterrupt)
                {
                    this.IsIndirect_                  = isIndirectHealing;
                    this.SkillId_                     = skill.ID;
                    this.TotalHealing_                = totalhealing;
                    this.MinHealing_                  = minhealing == int.MaxValue ? 0 : minhealing;
                    this.MaxHealing_                  = maxhealing == int.MinValue ? 0 : maxhealing;
                    this.NumberOfCasts_               = isIndirectHealing ? 0 : numberOfCast;
                    this.TimeWasted_                  = isIndirectHealing ? 0 : -timeWasted / 1000.0f;
                    this.TimeSaved_                   = isIndirectHealing ? 0 : timeSaved / 1000.0f;
                    this.Hits_                        = hits;
                    this.TimeSpentCasting_            = isIndirectHealing ? 0 : timeSpentCasting;
                    this.Totaldownedhealing_          = totaldownedhealing;
                    this.MinTimeSpentCasting_         = isIndirectHealing ? 0 : minTimeSpentCasting;
                    this.MaxTimeSpentCasting_         = isIndirectHealing ? 0 : maxTimeSpentCasting;
                    this.TimeSpentCastingNoInterrupt_ = isIndirectHealing ? 0 : timeSpentCastingNoInterrupt;
                    this.NumberOfCastNoInterrupt_     = isIndirectHealing ? 0 : numberOfCastNoInterrupt;
                }
            }

            partial class HealingDistributionL
            {
                 public HealingDistributionL(int capacity) : this() { this.Inner_.Capacity = capacity; }
                public void Add(HealingDistribution v) { this.Inner_.Add(v); }
            }
        }
    }

    partial class Int32L
    {
        public Int32L(int capacity) : this() { this.Inner_.Capacity = capacity; }
        public static Int32L From<L>(L list) where L : IReadOnlyList<int> {
            var t = new Int32L(list.Count);
            t.Inner_.AddRange(list);
            return t;
        }
        public void Add(int v) { this.Inner_.Add(v); }
        public void AddRange<L>(L l) where L : IReadOnlyList<int> { this.Inner_.AddRange(l); }
    }
    partial class Int32LL
    {
        public Int32LL(int capacity) : this() { this.Inner_.Capacity = capacity; }
        public static Int32LL From<L>(L list) where L : IReadOnlyList<Int32L> {
            var t = new Int32LL(list.Count);
            t.Inner_.AddRange(list);
            return t;
        }
        public void Add(Int32L v) { this.Inner_.Add(v); }
        public void AddRange<L>(L l) where L : IReadOnlyList<Int32L> { this.Inner_.AddRange(l); }
    }
}
