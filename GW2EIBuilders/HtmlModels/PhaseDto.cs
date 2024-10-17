﻿using System;
using System.Collections.Generic;
using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels
{
    //TODO(Rennorb) @perf: IF we wanted more performance we could try to just get rid of this json data step all together.
    // It should be doable to just merge it with existing structures, as to not need to copy everything..
    // If this is reasonably possible it should give time savings around 20-30%
    internal class PhaseDto
    {
        public string Name;
        public long Duration;
        public double Start;
        public double End;
        public List<int> Targets;
        public List<bool> SecondaryTargets;
        public bool BreakbarPhase;

        public List<List<object>> DpsStats;
        public List<List<List<object>>> DpsStatsTargets;
        public List<List<List<object>>> OffensiveStatsTargets;
        public List<List<object>> OffensiveStats;
        public List<List<object>> GameplayStats;
        public List<List<object>> DefStats;
        public List<List<object>> SupportStats;

        public BuffsContainerDto BuffsStatContainer;
        public BuffVolumesContainerDto BuffVolumesStatContainer;

        public List<DamageModData> DmgModifiersCommon;
        public List<DamageModData> DmgModifiersItem;
        public List<DamageModData> DmgModifiersPers;


        public List<DamageModData> DmgIncModifiersCommon;
        public List<DamageModData> DmgIncModifiersItem;
        public List<DamageModData> DmgIncModifiersPers;


        public List<List<(int, int)>> MechanicStats;
        public List<List<(int, int)>> EnemyMechanicStats;
        public List<long> PlayerActiveTimes;

        public List<double>? MarkupLines;
        public List<AreaLabelDto>? MarkupAreas;
        public List<int>? SubPhases;

        public PhaseDto(PhaseData phase, IReadOnlyList<PhaseData> phases, ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> persBuffDict,
            IReadOnlyList<OutgoingDamageModifier> commonOutDamageModifiers, IReadOnlyList<OutgoingDamageModifier> itemOutDamageModifiers, IReadOnlyDictionary<Spec, IReadOnlyList<OutgoingDamageModifier>> persOutDamageModDict,
            IReadOnlyList<IncomingDamageModifier> commonIncDamageModifiers, IReadOnlyList<IncomingDamageModifier> itemIncDamageModifiers, IReadOnlyDictionary<Spec, IReadOnlyList<IncomingDamageModifier>> persIncDamageModDict)
        {
            Name          = phase.Name;
            Duration      = phase.DurationInMS;
            Start         = phase.Start / 1000.0;
            End           = phase.End / 1000.0;
            BreakbarPhase = phase.BreakbarPhase;

            var allTargets = phase.AllTargets;
            Targets          = new(allTargets.Count);
            SecondaryTargets = new(allTargets.Count);
            foreach (AbstractSingleActor target in allTargets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
                SecondaryTargets.Add(phase.IsSecondaryTarget(target));
            }

            PlayerActiveTimes = new(log.Friendlies.Count);
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                PlayerActiveTimes.Add(actor.GetActiveDuration(log, phase.Start, phase.End));
            }

            // add phase markup
            
            if (!BreakbarPhase)
            {
                MarkupLines = new(phases.Count);
                MarkupAreas = new(phases.Count);
                for (int j = 1; j < phases.Count; j++)
                {
                    PhaseData curPhase = phases[j];
                    if (curPhase.Start < phase.Start || curPhase.End > phase.End ||
                        (curPhase.Start == phase.Start && curPhase.End == phase.End) || !curPhase.CanBeSubPhase)
                    {
                        continue;
                    }

                    SubPhases ??= new List<int>(phases.Count);
                    SubPhases.Add(j);

                    long start = curPhase.Start - phase.Start;
                    long end = curPhase.End - phase.Start;
                    if (curPhase.DrawStart)
                    {
                        MarkupLines.Add(start / 1000.0);
                    }

                    if (curPhase.DrawEnd)
                    {
                        MarkupLines.Add(end / 1000.0);
                    }

                    var phaseArea = new AreaLabelDto
                    {
                        Start = start / 1000.0,
                        End = end / 1000.0,
                        Label = curPhase.DrawLabel ? curPhase.Name : null,
                        Highlight = curPhase.DrawArea
                    };

                    MarkupAreas.Add(phaseArea);
                }
            }

            if (MarkupAreas?.Count == 0)
            {
                MarkupAreas = null;
            }

            if (MarkupLines?.Count == 0)
            {
                MarkupLines = null;
            }

            BuffsStatContainer       = new BuffsContainerDto(phase, log, persBuffDict);
            BuffVolumesStatContainer = new BuffVolumesContainerDto(phase, log, persBuffDict);
            
            DpsStats              = BuildDPSData(log, phase);
            DpsStatsTargets       = BuildDPSTargetsData(log, phase);
            OffensiveStatsTargets = BuildOffensiveStatsTargetsData(log, phase);
            OffensiveStats        = BuildOffensiveStatsData(log, phase);
            GameplayStats         = BuildGameplayStatsData(log, phase);
            DefStats              = BuildDefenseData(log, phase);
            SupportStats          = BuildSupportData(log, phase);
            
            DmgModifiersCommon    = DamageModData.BuildOutgoingDmgModifiersData(log, phase, commonOutDamageModifiers);
            DmgModifiersItem      = DamageModData.BuildOutgoingDmgModifiersData(log, phase, itemOutDamageModifiers);
            DmgModifiersPers      = DamageModData.BuildPersonalOutgoingDmgModifiersData(log, phase, persOutDamageModDict);
            DmgIncModifiersCommon = DamageModData.BuildIncomingDmgModifiersData(log, phase, commonIncDamageModifiers);
            DmgIncModifiersItem   = DamageModData.BuildIncomingDmgModifiersData(log, phase, itemIncDamageModifiers);
            DmgIncModifiersPers   = DamageModData.BuildPersonalIncomingDmgModifiersData(log, phase, persIncDamageModDict);
            MechanicStats         = MechanicDto.BuildPlayerMechanicData(log, phase);
            EnemyMechanicStats    = MechanicDto.BuildEnemyMechanicData(log, phase);
        }

        private static bool HasBoons(ParsedEvtcLog log, PhaseData phase, AbstractSingleActor target)
        {
            IReadOnlyDictionary<long, FinalActorBuffs> conditions = target.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
            foreach (Buff boon in log.StatisticsHelper.PresentBoons)
            {
                if (conditions.TryGetValue(boon.ID, out FinalActorBuffs uptime))
                {
                    if (uptime.Uptime > 0.0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // helper methods

        private static List<object> GetGameplayStatData(FinalGameplayStats stats)
        {
            var data = new List<object>
                {
                    // commons
                    stats.TimeWasted, // 0
                    stats.Wasted, // 1

                    stats.TimeSaved, // 2
                    stats.Saved, // 3

                    stats.SwapCount, // 4
                    Math.Round(stats.StackDist, 2), // 5
                    Math.Round(stats.DistToCom, 2), // 6
                    stats.SkillCastUptime, // 7
                    stats.SkillCastUptimeNoAA, // 8
                };
            return data;
        }

        private static List<object> GetOffensiveStatData(FinalOffensiveStats stats)
        {
            var data = new List<object>
                {
                    stats.DirectDamageCount, // 0
                    stats.CritableDirectDamageCount, // 1
                    stats.CriticalCount, // 2
                    stats.CriticalDmg, // 3

                    stats.FlankingCount, // 4

                    stats.GlanceCount, // 5

                    stats.Missed,// 6
                    stats.Interrupts, // 7
                    stats.Invulned, // 8
                    stats.Evaded,// 9
                    stats.Blocked,// 10
                    stats.ConnectedDirectDamageCount, // 11
                    stats.Killed, // 12
                    stats.Downed, // 13
                    stats.AgainstMovingCount, // 14
                    stats.ConnectedDamageCount, // 15
                    stats.TotalDamageCount, // 16
                    stats.DownContribution, // 17
                    stats.ConnectedDmg, // 18
                    stats.ConnectedDirectDmg, // 19

                    stats.ConnectedPowerCount, // 20
                    stats.ConnectedPowerAbove90HPCount, // 21
                    stats.ConnectedConditionCount, // 22
                    stats.ConnectedConditionAbove90HPCount, // 23
                    stats.AgainstDownedCount, // 24
                    stats.AgainstDownedDamage, // 25
                    stats.TotalDmg, // 26
                    stats.AppliedCrowdControl,//27
                    stats.AppliedCrowdControlDuration,//28
                };
            return data;
        }

        private static List<object> GetDPSStatData(FinalDPS dpsAll)
        {
            var data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.PowerDamage,
                    dpsAll.CondiDamage,
                    dpsAll.BreakbarDamage,
                };
            return data;
        }

        private static List<object> GetSupportStatData(FinalToPlayersSupport support)
        {
            var data = new List<object>()
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.CondiCleanseSelf,
                    support.CondiCleanseTimeSelf,
                    support.BoonStrips,
                    support.BoonStripsTime,
                    support.Resurrects,
                    support.ResurrectTime,
                    support.StunBreak,
                    support.RemovedStunDuration,
                };
            return data;
        }

        private static List<object> GetDefenseStatData(FinalDefensesAll defenses, PhaseData phase)
        {
            int downCount = 0;
            string downTooltip = "0% Downed";
            if (defenses.DownCount > 0)
            {
                var downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
                downCount = (defenses.DownCount);
                downTooltip = (downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1) + "% Downed");
            }
            int deadCount = 0;
            string deadTooltip = "100% Alive";
            if (defenses.DeadCount > 0)
            {
                var deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
                deadCount = (defenses.DeadCount);
                deadTooltip = (deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1)) + "% Alive");
            }
            var data = new List<object>
                {
                    defenses.DamageTaken, // 0
                    defenses.DamageBarrier,// 1
                    defenses.MissedCount,// 2
                    defenses.InterruptedCount,// 3
                    defenses.InvulnedCount,// 4
                    defenses.EvadedCount,// 5
                    defenses.BlockedCount,// 6
                    defenses.DodgeCount,// 7
                    defenses.ConditionCleanses,// 8
                    defenses.ConditionCleansesTime,// 9
                    defenses.BoonStrips,// 10
                    defenses.BoonStripsTime,// 11
                    downCount, // 12
                    downTooltip,// 13
                    deadCount,// 14
                    deadTooltip,// 15
                    defenses.DownedDamageTaken, // 16
                    defenses.ReceivedCrowdControl, // 17
                    defenses.ReceivedCrowdControlDuration, // 18
                };
            return data;
        }
        public static List<List<object>> BuildDPSData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>(log.Friendlies.Count);
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalDPS dpsAll = actor.GetDPSStats(log, phase.Start, phase.End);
                list.Add(GetDPSStatData(dpsAll));
            }
            return list;
        }

        public static List<List<List<object>>> BuildDPSTargetsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<List<object>>>(log.Friendlies.Count);

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                var playerData = new List<List<object>>();

                foreach (AbstractSingleActor target in phase.AllTargets)
                {
                    playerData.Add(GetDPSStatData(actor.GetDPSStats(target, log, phase.Start, phase.End)));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildGameplayStatsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalGameplayStats stats = actor.GetGameplayStats(log, phase.Start, phase.End);
                list.Add(GetGameplayStatData(stats));
            }
            return list;
        }

        public static List<List<object>> BuildOffensiveStatsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();
            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalOffensiveStats stats = actor.GetOffensiveStats(null, log, phase.Start, phase.End);
                list.Add(GetOffensiveStatData(stats));
            }
            return list;
        }

        public static List<List<List<object>>> BuildOffensiveStatsTargetsData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<List<object>>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                var playerData = new List<List<object>>();
                foreach (AbstractSingleActor target in phase.AllTargets)
                {
                    FinalOffensiveStats statsTarget = actor.GetOffensiveStats(target, log, phase.Start, phase.End);
                    playerData.Add(GetOffensiveStatData(statsTarget));
                }
                list.Add(playerData);
            }
            return list;
        }

        public static List<List<object>> BuildDefenseData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalDefensesAll defenses = actor.GetDefenseStats(log, phase.Start, phase.End);
                list.Add(GetDefenseStatData(defenses, phase));
            }

            return list;
        }

        public static List<List<object>> BuildSupportData(ParsedEvtcLog log, PhaseData phase)
        {
            var list = new List<List<object>>();

            foreach (AbstractSingleActor actor in log.Friendlies)
            {
                FinalToPlayersSupport support = actor.GetToPlayerSupportStats(log, phase.Start, phase.End);
                list.Add(GetSupportStatData(support));
            }
            return list;
        }
    }
}
