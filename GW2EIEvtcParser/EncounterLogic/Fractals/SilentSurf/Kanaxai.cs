﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Kanaxai : SilentSurf
    {
        public Kanaxai(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(RendingStorm, "Rending Storm", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Red), "RendStm.H", "Hit by Rending Storm (Axe AoE)", "Rending Storm Hit", 0),
                new PlayerDstHitMechanic(new long [] { HarrowshotDeath, HarrowshotExposure, HarrowshotFear, HarrowshotLethargy, HarrowshotTorment }, "Harrowshot", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Harrowshot.H", "Harrowshot (Lost all boons)", "Harrowshot (Boonstrip)", 0),
                new PlayerDstBuffApplyMechanic(ExtremeVulnerability, "Extreme Vulnerability", new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "ExtVuln.A", "Applied Extreme Vulnerability", "Extreme Vulnerability Application", 150),
                new PlayerDstBuffApplyMechanic(ExposedPlayer, "Exposed", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Pink), "Expo.A", "Applied Exposed", "Exposed Application (Player)", 0),
                new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Fear.A", "Fear Applied", "Fear Application", 150),
                new PlayerDstBuffApplyMechanic(Phantasmagoria, "Phantasmagoria", new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Phant.A", "Phantasmagoria Applied (Aspect visible on Island)", "Phantasmagoria Application", 150),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Pink), "Expo.A", "Applied Exposed to Kanaxai", "Exposed Application (Kanaxai)", 150),
                new PlayerDstBuffRemoveMechanic(ExtremeVulnerability, "Dread Visage", new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkRed), "Eye.D", "Died to Dread Visage (Eye)", "Dread Visage Death", 150)
                    .UsingChecker((remove, log) =>
                    {
                        // 5s extreme vulnerability from dread visage
                        const int duration = 5000;
                        // find last apply
                        BuffApplyEvent apply = log.CombatData.GetBuffData(ExtremeVulnerability)
                            .OfType<BuffApplyEvent>()
                            .Where(e => e.Time <= remove.Time && e.To == remove.To)
                            .MaxBy(e => e.Time);
                        // check for removed duration, applied duration & death within 1s after
                        return remove.RemovedDuration > ServerDelayConstant
                            && Math.Abs(apply.AppliedDuration - duration) < ServerDelayConstant
                            && log.CombatData.GetDeadEvents(remove.To).Any(dead =>
                            {
                                long diff = dead.Time - remove.Time;
                                return diff > -ServerDelayConstant && diff <= 1000;
                            });
                    }),
                new PlayerDstBuffRemoveMechanic(ExtremeVulnerability, "Frightening Speed", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Numbers.D", "Died to Frightening Speed (Numbers)", "Frightening Speed Death", 150)
                    .UsingChecker((remove, log) =>
                    {
                        // 60s extreme vulnerability from frightening speed
                        const int duration = 60000;
                        // find last apply
                        BuffApplyEvent apply = log.CombatData.GetBuffData(ExtremeVulnerability)
                            .OfType<BuffApplyEvent>()
                            .Where(e => e.Time <= remove.Time && e.To == remove.To)
                            .MaxBy(e => e.Time);
                        // check for removed duration, applied duration & death within 1s after
                        return remove.RemovedDuration > ServerDelayConstant
                            && Math.Abs(apply.AppliedDuration - duration) < ServerDelayConstant
                            && log.CombatData.GetDeadEvents(remove.To).Any(dead =>
                            {
                                long diff = dead.Time - remove.Time;
                                return diff > -ServerDelayConstant && diff <= 1000;
                            });
                    }),
                new PlayerDstBuffApplyMechanic(new long [] { RendingStormAxeTargetBuff1, RendingStormAxeTargetBuff2 }, "Rending Storm Target", new MechanicPlotlySetting(Symbols.CircleX, Colors.LightPurple), "RendStm.T", "Targetted by Rending Storm (Axe Throw)", "Rending Storm Target", 150),
            });
            Extension = "kanaxai";
            Icon = EncounterIconKanaxai;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayKanaxai,
                           (334, 370),
                           (-6195, -295, -799, 5685));
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM,
                (int)ArcDPSEnums.TrashID.AspectOfTorment,
                (int)ArcDPSEnums.TrashID.AspectOfLethargy,
                (int)ArcDPSEnums.TrashID.AspectOfExposure,
                (int)ArcDPSEnums.TrashID.AspectOfDeath,
                (int)ArcDPSEnums.TrashID.AspectOfFear,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            var aspectCounts = new Dictionary<int, int>();
            foreach (AbstractSingleActor actor in Targets)
            {
                switch (actor.ID)
                {
                    case (int)ArcDPSEnums.TrashID.AspectOfTorment:
                    case (int)ArcDPSEnums.TrashID.AspectOfLethargy:
                    case (int)ArcDPSEnums.TrashID.AspectOfExposure:
                    case (int)ArcDPSEnums.TrashID.AspectOfDeath:
                    case (int)ArcDPSEnums.TrashID.AspectOfFear:
                        if (aspectCounts.TryGetValue(actor.ID, out int count))
                        {
                            actor.OverrideName(actor.Character + " " + count);
                            aspectCounts[actor.ID] = count + 1;
                        }
                        else
                        {
                            actor.OverrideName(actor.Character + " 1");
                            aspectCounts[actor.ID] = 2;
                        }
                        break;
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai == null)
            {
                throw new MissingKeyActorsException("Kanaxai not found");
            }
            phases[0].AddTarget(kanaxai);
            if (!requirePhases)
            {
                return phases;
            }
            // Phases
            List<PhaseData> encounterPhases = GetPhasesByInvul(log, DeterminedToDestroy, kanaxai, true, true);

            var worldCleaverPhaseStarts = log.CombatData.GetBuffData(DeterminedToDestroy).OfType<BuffApplyEvent
                >().Where(x => x.To == kanaxai.AgentItem).Select(x => x.Time).ToList();
            int worldCleaverCount = 0;
            int repeatedCount = 0;
            var isRepeatedWorldCleaverPhase = new List<bool>();
            for (int i = 0; i < encounterPhases.Count; i++)
            {
                PhaseData curPhase = encounterPhases[i];
                if (worldCleaverPhaseStarts.Any(x => curPhase.Start == x))
                {
                    var baseName = "World Cleaver ";
                    long midPhase = (curPhase.Start + curPhase.End) / 2;
                    if (kanaxai.GetCurrentHealthPercent(log, midPhase) > 50)
                    {
                        if (repeatedCount == 0)
                        {
                            isRepeatedWorldCleaverPhase.Add(false);
                            curPhase.Name = baseName + (++worldCleaverCount);
                        }
                        else
                        {
                            isRepeatedWorldCleaverPhase.Add(true);
                            curPhase.Name = baseName + (worldCleaverCount) + " Repeated " + repeatedCount;
                        }
                        repeatedCount++;
                    }
                    else if (kanaxai.GetCurrentHealthPercent(log, midPhase) > 25)
                    {
                        if (worldCleaverCount == 1)
                        {
                            repeatedCount = 0;
                        }
                        if (repeatedCount == 0)
                        {
                            isRepeatedWorldCleaverPhase.Add(false);
                            curPhase.Name = baseName + (++worldCleaverCount);
                        }
                        else
                        {
                            isRepeatedWorldCleaverPhase.Add(true);
                            curPhase.Name = baseName + (worldCleaverCount) + " Repeated " + repeatedCount;
                        }
                        repeatedCount++;
                    }
                    else
                    {
                        // No hp update events, buggy log
                        return phases;
                    }
                    foreach (AbstractSingleActor aspect in Targets)
                    {
                        switch (aspect.ID)
                        {
                            case (int)ArcDPSEnums.TrashID.AspectOfTorment:
                            case (int)ArcDPSEnums.TrashID.AspectOfLethargy:
                            case (int)ArcDPSEnums.TrashID.AspectOfExposure:
                            case (int)ArcDPSEnums.TrashID.AspectOfDeath:
                            case (int)ArcDPSEnums.TrashID.AspectOfFear:
                                if (log.CombatData.GetBuffRemoveAllData(Determined762).Any(x => x.To == aspect.AgentItem && x.Time >= curPhase.Start && x.Time <= curPhase.End))
                                {
                                    curPhase.AddTarget(aspect);
                                }
                                break;
                        }
                    }
                    curPhase.AddTarget(kanaxai);
                }
                else
                {
                    isRepeatedWorldCleaverPhase.Add(false);
                }
            }
            // Handle main phases after world cleave phases as we need to know if it is a repeated phase
            int phaseCount = 0;
            for (int i = 0; i < encounterPhases.Count; i++)
            {
                PhaseData curPhase = encounterPhases[i];
                if (!worldCleaverPhaseStarts.Any(x => curPhase.Start == x))
                {
                    var baseName = "Phase ";
                    if (i < isRepeatedWorldCleaverPhase.Count - 1)
                    {
                        if (isRepeatedWorldCleaverPhase[i + 1])
                        {
                            curPhase.Name = baseName + (phaseCount) + " Repeated " + (++repeatedCount);
                        }
                        else
                        {
                            curPhase.Name = baseName + (++phaseCount);
                            repeatedCount = 0;
                        }
                    }
                    else
                    {
                        curPhase.Name = baseName + (++phaseCount);
                    }
                    curPhase.AddTarget(kanaxai);
                }
            }
            phases.AddRange(encounterPhases);

            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai == null)
            {
                throw new MissingKeyActorsException("Kanaxai not found");
            }
            BuffApplyEvent invul762Gain = combatData.GetBuffData(Determined762).OfType<BuffApplyEvent>().Where(x => x.To == kanaxai.AgentItem).FirstOrDefault();
            if (invul762Gain != null)
            {
                fightData.SetSuccess(true, invul762Gain.Time);
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            // Orange Tether from Aspect to player
            IEnumerable<AbstractBuffEvent> tethers = log.CombatData.GetBuffData(AspectTetherBuff).Where(x => x.To == player.AgentItem);
            IEnumerable<BuffApplyEvent> tetherApplies = tethers.OfType<BuffApplyEvent>();
            IEnumerable<BuffRemoveAllEvent> tetherRemoves = tethers.OfType<BuffRemoveAllEvent>();
            AgentItem tetherAspect = _unknownAgent;
            foreach (BuffApplyEvent apply in tetherApplies)
            {
                tetherAspect = apply.By == _unknownAgent ? tetherAspect : apply.By;
                int start = (int)apply.Time;
                BuffApplyEvent replace = tetherApplies.FirstOrDefault(x => x.Time >= apply.Time && x.By != tetherAspect);
                BuffRemoveAllEvent remove = tetherRemoves.FirstOrDefault(x => x.Time >= apply.Time);
                long end = Math.Min(replace?.Time ?? long.MaxValue, remove?.Time ?? long.MaxValue);
                if (end != long.MaxValue)
                {
                    replay.Decorations.Add(new LineDecoration(0, (start, (int)end), "rgba(255, 200, 0, 0.5)", new AgentConnector(tetherAspect), new AgentConnector(player)));
                }
            }

            // Blue tether from Aspect to player, appears when the player gains phantasmagoria
            // Custom decoration not visible in game
            IEnumerable<AbstractBuffEvent> phantasmagoria = log.CombatData.GetBuffData(Phantasmagoria).Where(x => x.To == player.AgentItem);
            IEnumerable<BuffRemoveAllEvent> phantasmagoriaRemoves = phantasmagoria.OfType<BuffRemoveAllEvent>();
            foreach (BuffApplyEvent apply in phantasmagoria.OfType<BuffApplyEvent>())
            {
                int start = (int)apply.Time;
                BuffRemoveAllEvent remove = phantasmagoriaRemoves.FirstOrDefault(x => x.Time >= apply.Time);
                if (remove != null)
                {
                    replay.Decorations.Add(new LineDecoration(0, (start, (int)remove.Time), "rgba(0, 100, 255, 0.5)", new AgentConnector(apply.By), new AgentConnector(player)));
                }
            }

            // Axe AoE attached to players
            IEnumerable<Segment> buffs69195 = player.GetBuffStatus(log, RendingStormAxeTargetBuff1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            IEnumerable<Segment> buffs69208 = player.GetBuffStatus(log, RendingStormAxeTargetBuff2, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            var axes = new List<Segment>();
            axes.AddRange(buffs69208);
            axes.AddRange(buffs69195);
            foreach (Segment segment in axes)
            {
                replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)segment.Start, (int)segment.End), "rgba(200, 120, 0, 0.2)", new AgentConnector(player)));
                replay.Decorations.Add(new CircleDecoration(true, (int)segment.End, 180, ((int)segment.Start, (int)segment.End), "rgba(200, 120, 0, 0.2)", new AgentConnector(player)));
            }

            // Numbers spread AoEs
            IEnumerable<Segment> spreads = player.GetBuffStatus(log, KanaxaiSpreadOrangeAoEBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            foreach (Segment spreadSegment in spreads)
            {
                replay.Decorations.Add(new CircleDecoration(true, 0, 380, ((int)spreadSegment.Start, (int)spreadSegment.End), "rgba(200, 120, 0, 0.2)", new AgentConnector(player)));
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Red AoE during Frightening Speed attack
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FrighteningSpeedRedAoE, out IReadOnlyList<EffectEvent> frighteningSpeedRedAoEs))
            {
                foreach (EffectEvent aoe in frighteningSpeedRedAoEs)
                {
                    int duration = 1500;
                    int start = (int)aoe.Time;
                    int effectEnd = start + duration;
                    EnvironmentDecorations.Add(new CircleDecoration(true, 0, 380, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(aoe.Position)));
                    EnvironmentDecorations.Add(new CircleDecoration(false, 0, 380, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(aoe.Position), 10));
                }
            }

            // Axe AoE
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AxeGroundAoE, out IReadOnlyList<EffectEvent> axeAoEs))
            {
                foreach (EffectEvent aoe in axeAoEs)
                {
                    // Get World Cleaver casts and find the first cast time event present after the AoE effect time
                    AgentItem agentKanaxai = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM);
                    var kanaxai = new NPC(agentKanaxai);
                    IReadOnlyList<AbstractCastEvent> casts = kanaxai.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
                    AbstractCastEvent cast = casts.Where(x => x.SkillId == WorldCleaver).FirstOrDefault(x => x.Time > aoe.Time);
                    long worldCleaverTime = cast != null ? cast.Time : 0;

                    // Get Axe AoE Buffs
                    IReadOnlyList<AbstractBuffEvent> buffs69195 = log.CombatData.GetBuffData(RendingStormAxeTargetBuff1);
                    IReadOnlyList<AbstractBuffEvent> buffs69208 = log.CombatData.GetBuffData(RendingStormAxeTargetBuff2);
                    var axes = new List<AbstractBuffEvent>();
                    axes.AddRange(buffs69208);
                    axes.AddRange(buffs69195);
                    var orderedAxes = axes.OrderBy(x => x.Time).ToList();

                    // Find the first BuffRemoveAllEvent after the AoE effect Time or next World Cleaver cast time
                    // World Cleaver is the time-limit of when the AoEs reset, in third phase we use FightEnd
                    if (worldCleaverTime != 0)
                    {
                        AbstractBuffEvent axeBuffRemoval = orderedAxes.FirstOrDefault(buff => buff.Time > aoe.Time && buff.Time < worldCleaverTime && buff is BuffRemoveAllEvent);
                        AddAxeAoeDecoration(aoe, axeBuffRemoval, worldCleaverTime);
                    }
                    else
                    {
                        AbstractBuffEvent axeBuffRemoval = orderedAxes.FirstOrDefault(buff => buff.Time > aoe.Time && buff is BuffRemoveAllEvent);
                        AddAxeAoeDecoration(aoe, axeBuffRemoval, log.FightData.FightEnd);
                    }
                }
            }

            // Harrowshot - Boonstrip AoE
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarrowshotAoE, out IReadOnlyList<EffectEvent> harrowshots))
            {
                foreach (EffectEvent harrowshot in harrowshots)
                {
                    int duration = 3000;
                    int start = (int)harrowshot.Time;
                    int end = (int)harrowshot.Time + duration;
                    EnvironmentDecorations.Add(new CircleDecoration(true, 0, 280, (start, end), "rgba(255, 120, 0, 0.2)", new PositionConnector(harrowshot.Position)));
                    EnvironmentDecorations.Add(new CircleDecoration(true, end, 280, (start, end), "rgba(255, 120, 0, 0.2)", new PositionConnector(harrowshot.Position)));
                }
            }
        }

        /// <summary>
        /// Adds the Axe AoE decoration.<br></br>
        /// If the next orange AoE <see cref="BuffRemoveAllEvent"/> on players is after <see cref="WorldCleaver"/> cast time or not present,<br></br>
        /// utilise the <see cref="WorldCleaver"/> cast time or <see cref="FightData.LogEnd"/>.
        /// </summary>
        /// <param name="aoe">Effect of the AoE.</param>
        /// <param name="axeBuffRemoval">Buff removal of the orange AoE.</param>
        /// <param name="time">Last time possible.</param>
        private void AddAxeAoeDecoration(EffectEvent aoe, AbstractBuffEvent axeBuffRemoval, long time)
        {
            int duration;
            if (axeBuffRemoval != null)
            {
                duration = (int)(axeBuffRemoval.Time - aoe.Time);
            }
            else
            {
                duration = (int)(time - aoe.Time);
            }
            int start = (int)aoe.Time;
            int effectEnd = start + duration;

            EnvironmentDecorations.Add(new CircleDecoration(true, 0, 180, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(aoe.Position)));
            EnvironmentDecorations.Add(new CircleDecoration(false, 0, 180, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(aoe.Position), 10));
        }
    }
}
