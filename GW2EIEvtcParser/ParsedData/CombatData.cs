﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using Tracing;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.ParsedData
{
    public class CombatData
    {
        public bool HasMovementData { get; }

        //private List<CombatItem> _healingData;
        //private List<CombatItem> _healingReceivedData;
        private readonly StatusEventsContainer _statusEvents = new StatusEventsContainer();
        private readonly MetaEventsContainer _metaDataEvents = new MetaEventsContainer();
        private readonly HashSet<long> _skillIds;
        private readonly Dictionary<long, List<AbstractBuffEvent>> _buffData;
        private Dictionary<long, Dictionary<uint, List<AbstractBuffEvent>>> _buffDataByInstanceID;
        private Dictionary<long, List<BuffRemoveAllEvent>> _buffRemoveAllData;
        private readonly Dictionary<AgentItem, List<AbstractBuffEvent>> _buffDataByDst;
        private Dictionary<long, Dictionary<AgentItem, List<AbstractBuffEvent>>> _buffDataByIDByDst;
        private readonly Dictionary<AgentItem, List<AbstractHealthDamageEvent>> _damageData;
        private readonly Dictionary<AgentItem, List<BreakbarDamageEvent>> _breakbarDamageData;
        private readonly Dictionary<AgentItem, List<CrowdControlEvent>> _crowControlData;
        private readonly Dictionary<long, List<BreakbarDamageEvent>> _breakbarDamageDataById;
        private readonly Dictionary<long, List<AbstractHealthDamageEvent>> _damageDataById;
        private readonly Dictionary<long, List<CrowdControlEvent>> _crowControlDataById;
        private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _animatedCastData;
        private readonly Dictionary<AgentItem, List<InstantCastEvent>> _instantCastData;
        private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;
        private readonly Dictionary<long, List<AnimatedCastEvent>> _animatedCastDataById;
        private readonly Dictionary<long, List<InstantCastEvent>> _instantCastDataById;
        private readonly Dictionary<AgentItem, List<AbstractHealthDamageEvent>> _damageTakenData;
        private readonly Dictionary<AgentItem, List<BreakbarDamageEvent>> _breakbarDamageTakenData;
        private readonly Dictionary<AgentItem, List<CrowdControlEvent>> _crowControlTakenData;
        private readonly List<RewardEvent> _rewardEvents = new List<RewardEvent>();
        // EXTENSIONS
        public EXTHealingCombatData EXTHealingCombatData { get; internal set; }
        public EXTBarrierCombatData EXTBarrierCombatData { get; internal set; }
        public bool HasEXTHealing => EXTHealingCombatData != null;
        public bool HasEXTBarrier => EXTBarrierCombatData != null;

        internal bool UseBuffInstanceSimulator { get; } = false;

        internal bool HasStackIDs { get; }

        public bool HasBreakbarDamageData { get; } = false;
        public bool HasEffectData { get; } = false;

        private void EIBuffParse(IReadOnlyList<Player> players, SkillData skillData, FightData fightData, EvtcVersionEvent evtcVersion)
        {
            //TODO(Rennorb) @perf @mem: find average complexity
            var toAdd = new List<AbstractBuffEvent>(players.Count * 10);
            foreach (Player p in players)
            {
                if (p.Spec == Spec.Weaver)
                {
                    toAdd.AddRange(WeaverHelper.TransformWeaverAttunements(GetBuffDataByDst(p.AgentItem), _buffData, p.AgentItem, skillData));
                }
                if (p.Spec == Spec.Virtuoso)
                {
                    toAdd.AddRange(VirtuosoHelper.TransformVirtuosoBladeStorage(GetBuffDataByDst(p.AgentItem), p.AgentItem, skillData, evtcVersion));
                }
                if (p.BaseSpec == Spec.Elementalist && p.Spec != Spec.Weaver)
                {
                    ElementalistHelper.RemoveDualBuffs(GetBuffDataByDst(p.AgentItem), _buffData, skillData);
                }
            }
            toAdd.AddRange(fightData.Logic.SpecialBuffEventProcess(this, skillData));

            var buffIDsToSort = new HashSet<long>(toAdd.Count);
            var buffAgentsToSort = new HashSet<AgentItem>(toAdd.Count);
            foreach (AbstractBuffEvent bf in toAdd)
            {
                //TODO(Rennorb) @perf @mem: find average complexity
                _buffDataByDst.AddToList(bf.To, bf, toAdd.Count / 4);
                buffAgentsToSort.Add(bf.To);

                //TODO(Rennorb) @perf @mem: find average complexity
                _buffData.AddToList(bf.BuffID, bf, toAdd.Count / 4);
                buffIDsToSort.Add(bf.BuffID);
            }

            foreach (long buffID in buffIDsToSort)
            {
                _buffData[buffID].SortByTime();
            }

            foreach (AgentItem a in buffAgentsToSort)
            {
                _buffDataByDst[a].SortByTime();
            }

            if (toAdd.Count != 0)
            {
                BuildBuffDependentContainers();
            }
        }

        private void EIDamageParse(SkillData skillData, FightData fightData)
        {
            var toAdd = fightData.Logic.SpecialDamageEventProcess(this, skillData);

            var idsToSort = new HashSet<long>(toAdd.Count);
            var dstToSort = new HashSet<AgentItem>(toAdd.Count);
            var srcToSort = new HashSet<AgentItem>(toAdd.Count);
            foreach (AbstractHealthDamageEvent de in toAdd)
            {
                //TODO(Rennorb) @perf @mem: find average complexity
                _damageTakenData.AddToList(de.To, de, toAdd.Count / 4);
                dstToSort.Add(de.To);

                //TODO(Rennorb) @perf @mem: find average complexity
                _damageData.AddToList(de.From, de, toAdd.Count / 4);
                srcToSort.Add(de.From);

                //TODO(Rennorb) @perf @mem: find average complexity
                _damageDataById.AddToList(de.SkillId, de);
                idsToSort.Add(de.SkillId);
            }

            foreach (long buffID in idsToSort)
            {
                _damageDataById[buffID].SortByTime();
            }

            foreach (AgentItem a in dstToSort)
            {
                _damageTakenData[a].SortByTime();
            }

            foreach (AgentItem a in srcToSort)
            {
                _damageData[a].SortByTime();
            }
        }

        private List<InstantCastEvent> ComputeInstantCastEventsFromFinders(AgentData agentData, SkillData skillData, HashSet<InstantCastFinder> instantCastFinders)
        {
            //TODO(Rennorb) @perf @mem: find average complexity
            var res = new List<InstantCastEvent>();
            foreach (InstantCastFinder icf in instantCastFinders)
            {
                if (icf.Available(this))
                {
                    if (icf.NotAccurate)
                    {
                        skillData.NotAccurate.Add(icf.SkillID);
                    }
                    switch (icf.CastOrigin)
                    {
                        case InstantCastFinder.InstantCastOrigin.Trait:
                            skillData.TraitProc.Add(icf.SkillID);
                            break;
                        case InstantCastFinder.InstantCastOrigin.Gear:
                            skillData.GearProc.Add(icf.SkillID);
                            break;
                        case InstantCastFinder.InstantCastOrigin.Skill:
                        default:
                            break;
                    }
                    res.AddRange(icf.ComputeInstantCast(this, skillData, agentData));
                }
            }
            return res;
        }

        private void EICastParse(IReadOnlyList<Player> players, SkillData skillData, FightData fightData, AgentData agentData)
        {
            List<AbstractCastEvent> toAdd = fightData.Logic.SpecialCastEventProcess(this, skillData);
            ulong gw2Build = GetGW2BuildEvent().Build;
            foreach (Player p in players)
            {
                switch (p.Spec)
                {
                    case Spec.Willbender:
                        toAdd.AddRange(ProfHelper.ComputeEndWithBuffApplyCastEvents(p, this, skillData, FlowingResolveSkill, 440, 500, FlowingResolveBuff));
                        break;
                    default:
                        break;
                }
                switch (p.BaseSpec)
                {
                    case Spec.Necromancer:
                        if (gw2Build < GW2Builds.March2024BalanceAndCerusLegendary)
                        {
                            toAdd.AddRange(ProfHelper.ComputeEndWithBuffApplyCastEvents(p, this, skillData, PathOfGluttony, 750, 750, PathOfGluttonyFlipBuff));
                        }
                        break;
                    case Spec.Ranger:
                        toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, AncestralGraceSkill, AncestralGraceBuff));
                        break;
                    case Spec.Elementalist:
                        toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, Updraft, EffectGUIDs.ElementalistUpdraft2, 0, 1000));
                        toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, RideTheLightningSkill, RideTheLightningBuff));
                        break;
                    case Spec.Engineer:
                        // Collides with Detonate Supply Crate Turrets
                        //toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, Devastator, EffectGUIDs.EngineerSpearDevastator1, -1000, 1000));
                        toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, ConduitSurge, ConduitSurgeBuff));
                        break;
                    case Spec.Revenant:
                        toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, AbyssalBlitz, EffectGUIDs.RevenantSpearAbyssalBlitz1, 0, 3000, 
                            (abyssalBlitz, effect, combatData, skllData) =>
                            {
                                return abyssalBlitz.Where(x => x.Time < effect.Time && Math.Abs(x.Time - effect.Time) < 300).Count() == 0;
                            }));
                        break;
                    default:
                        break;
                }
            }
            // Generic instant cast finders
            var instantCastsFinder = new HashSet<InstantCastFinder>(ProfHelper.GetProfessionInstantCastFinders(players));
            foreach(var x in fightData.Logic.GetInstantCastFinders()) { instantCastsFinder.Add(x); }
            toAdd.AddRange(ComputeInstantCastEventsFromFinders(agentData, skillData, instantCastsFinder));


            var castIDsToSort       = new HashSet<long>(toAdd.Count / 3);
            var castAgentsToSort    = new HashSet<AgentItem>(toAdd.Count / 3);
            var wepSwapAgentsToSort = new HashSet<AgentItem>(toAdd.Count / 3);
            var instantAgentsToSort = new HashSet<AgentItem>(toAdd.Count / 3);
            var instantIDsToSort    = new HashSet<long>(toAdd.Count / 3);
            foreach (AbstractCastEvent cast in toAdd)
            {
                if (cast is AnimatedCastEvent ace)
                {
                    //TODO(Rennorb) @perf @mem: find average complexity
                    _animatedCastData.AddToList(ace.Caster, ace, toAdd.Count / (players.Count + 2));
                    castAgentsToSort.Add(ace.Caster);

                    //TODO(Rennorb) @perf @mem: find average complexity
                    _animatedCastDataById.AddToList(ace.SkillId, ace, 10);
                    castIDsToSort.Add(ace.SkillId);
                }

                if (cast is WeaponSwapEvent wse)
                {
                    //TODO(Rennorb) @perf @mem: find average complexity
                    _weaponSwapData.AddToList(wse.Caster, wse, toAdd.Count / (players.Count + 2));
                    wepSwapAgentsToSort.Add(wse.Caster);
                }

                if (cast is InstantCastEvent ice)
                {
                    //TODO(Rennorb) @perf @mem: find average complexity
                    _instantCastData.AddToList(ice.Caster, ice, toAdd.Count / (players.Count + 2));
                    instantAgentsToSort.Add(ice.Caster);

                    //TODO(Rennorb) @perf @mem: find average complexity
                    _instantCastDataById.AddToList(ice.SkillId, ice, 10);
                    instantIDsToSort.Add(ice.SkillId);
                }
            }

            foreach (long castID in castIDsToSort)
            {
                _animatedCastDataById[castID].SortByTime();
            }

            foreach (AgentItem a in castAgentsToSort)
            {
                _animatedCastData[a].SortByTime();
            }

            foreach (AgentItem a in wepSwapAgentsToSort)
            {
                _weaponSwapData[a].SortByTime();
            }

            foreach (AgentItem a in instantAgentsToSort)
            {
                _instantCastData[a].SortByTime();
            }

            foreach (long instantID in instantIDsToSort)
            {
                _instantCastDataById[instantID].SortByTime();
            }
        }

        private void EIMetaAndStatusParse(FightData fightData, EvtcVersionEvent evtcVersion)
        {
            foreach (var (agent, events) in _damageTakenData)
            {
                if (agent.IsSpecies(TargetID.WorldVersusWorld))
                {
                    continue;
                }
                
                bool setDeads = false;
                if (!_statusEvents.DeadEvents.TryGetValue(agent, out var agentDeaths))
                {
                    agentDeaths = new();
                    setDeads = true;
                }

                bool setDowns = false;
                if (!_statusEvents.DownEvents.TryGetValue(agent, out var agentDowns))
                {
                    agentDowns = new();
                    setDowns = true;
                }

                foreach (AbstractHealthDamageEvent evt in events)
                {
                    if (evt.HasKilled)
                    {
                        if (!agentDeaths.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDeaths.Add(new DeadEvent(agent, evt.Time));
                        }
                    }
                    if (evt.HasDowned)
                    {
                        if (!agentDowns.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                        {
                            agentDowns.Add(new DownEvent(agent, evt.Time));
                        }
                    }
                }

                if (setDeads && agentDeaths.Count > 0)
                {
                    agentDeaths.SortByTime();
                    _statusEvents.DeadEvents[agent] = agentDeaths;
                }

                if (setDowns && agentDowns.Count > 0)
                {
                    agentDowns.SortByTime();
                    _statusEvents.DownEvents[agent] = agentDowns;
                }
            }
            _metaDataEvents.ErrorEvents.AddRange(fightData.Logic.GetCustomWarningMessages(fightData, evtcVersion));
        }

        private void EIExtraEventProcess(IReadOnlyList<Player> players, SkillData skillData, AgentData agentData, FightData fightData, ParserController operation, EvtcVersionEvent evtcVersion)
        {
            using var _t = new AutoTrace("Process Extra Events");

            // Add missing breakbar active state
            foreach (var pair in _statusEvents.BreakbarStateEvents)
            {
                var first = pair.Value.FirstOrDefault();
                if (first != null && first.State != BreakbarState.Active && first.Time > pair.Key.FirstAware + 500)
                {
                    pair.Value.Insert(0, new BreakbarStateEvent(pair.Key, pair.Key.FirstAware, BreakbarState.Active));
                }
            }

            // master attachements
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Warrior Gadgets");
            WarriorHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Engineer Gadgets");
            EngineerHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Ranger Gadgets");
            RangerHelper.ProcessGadgets(players, this);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Revenant Gadgets");
            RevenantHelper.ProcessGadgets(players, this, agentData);
            operation.UpdateProgressWithCancellationCheck("Parsing: Processing Racial Gadget");
            ProfHelper.ProcessRacialGadgets(players, this);
            
            // Custom events
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Buff Events");
            EIBuffParse(players, skillData, fightData, evtcVersion);
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Damage Events");
            EIDamageParse(skillData, fightData);
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Cast Events");
            EICastParse(players, skillData, fightData, agentData);
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Status Events");
            EIMetaAndStatusParse(fightData, evtcVersion);
        }

        private void OffsetBuffExtensionEvents(EvtcVersionEvent evtcVersion)
        {
            using var _t = new AutoTrace("Buff Extension");
            if (evtcVersion.Build <= ArcDPSBuilds.BuffExtensionBroken)
            {
                return;
            }

            foreach (var events in _buffDataByDst.Values)
            {
                //TODO(Rennorb) @perf: wtf
                var dictApply = events.OfType<BuffApplyEvent>()
                    .Where(x => x.BuffInstance != 0)
                    .GroupBy(x => x.BuffInstance)
                    .ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));
                var dictStacks = events.OfType<AbstractBuffStackEvent>()
                    .Where(x => x.BuffInstance != 0)
                    .GroupBy(x => x.BuffInstance)
                    .ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));
                var dictExtensions = events.OfType<BuffExtensionEvent>()
                    .Where(x => x.BuffInstance != 0)
                    .GroupBy(x => x.BuffInstance);
                   
                foreach (var extensionEventsPerId in dictExtensions)
                {
                    if (!dictApply.TryGetValue(extensionEventsPerId.Key, out var appliesPerBuffID)) { continue; }

                    foreach (var extensionEvents in extensionEventsPerId.GroupBy(y => y.BuffID))
                    {
                        if (!appliesPerBuffID.TryGetValue(extensionEvents.Key, out var applies)) { continue; }

                        BuffExtensionEvent? previousExtension = null;
                        foreach (BuffExtensionEvent extensionEvent in extensionEvents)
                        {
                            BuffApplyEvent initialStackApplication = applies.LastOrDefault(x => x.Time <= extensionEvent.Time);
                            if (initialStackApplication == null) { continue; }

                            var sequence = new List<AbstractBuffEvent>(2) { initialStackApplication };
                            if (dictStacks.TryGetValue(extensionEvent.BuffInstance, out var stacksPerBuffID))
                            {
                                if (stacksPerBuffID.TryGetValue(extensionEvent.BuffID, out var stacks))
                                {
                                    sequence.AddRange(stacks.Where(x => x.Time >= initialStackApplication.Time && x.Time <= extensionEvent.Time));
                                }
                            }

                            if (previousExtension != null && previousExtension.Time >= initialStackApplication.Time)
                            {
                                sequence.Add(previousExtension);
                            }

                            previousExtension = extensionEvent;
                            sequence.SortByTime();
                            extensionEvent.OffsetNewDuration(sequence, evtcVersion);
                        }
                    }
                }
            }
        }

        internal CombatData(IReadOnlyList<CombatItem> allCombatItems, FightData fightData, AgentData agentData, SkillData skillData, IReadOnlyList<Player> players, ParserController operation, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, EvtcVersionEvent evtcVersion)
        {
            using var _t = new AutoTrace("CombatData");
            _metaDataEvents.EvtcVersionEvent = evtcVersion;

            var combatEvents = allCombatItems.ToList();
            combatEvents.SortByTime();

            //TODO(Rennorb) @perf: find average complexity
            _skillIds = new HashSet<long>(combatEvents.Count / 2);
            var castCombatEvents = new Dictionary<ulong, List<CombatItem>>(combatEvents.Count / 5);
            var buffEvents = new List<AbstractBuffEvent>(combatEvents.Count / 2);
            var wepSwaps = new List<WeaponSwapEvent>(combatEvents.Count / 50);
            var brkDamageData = new List<BreakbarDamageEvent>(combatEvents.Count / 25);
            var crowdControlData = new List<CrowdControlEvent>(combatEvents.Count / 10);
            var damageData = new List<AbstractHealthDamageEvent>(combatEvents.Count / 2);

            operation.UpdateProgressWithCancellationCheck("Parsing: Creating EI Combat Data");
            foreach (CombatItem combatItem in combatEvents)
            {
                bool insertToSkillIDs = false;
                if (combatItem.IsStateChange != StateChange.None)
                {
                    if (combatItem.IsExtension)
                    {
                        if (extensions.TryGetValue(combatItem.Pad, out AbstractExtensionHandler handler))
                        {
                            insertToSkillIDs = handler.IsSkillID(combatItem);
                            handler.InsertEIExtensionEvent(combatItem, agentData, skillData);
                        }
                    }
                    else
                    {
                        insertToSkillIDs = combatItem.IsStateChange == StateChange.BuffInitial;
                        CombatEventFactory.AddStateChangeEvent(combatItem, agentData, skillData, _metaDataEvents, _statusEvents, _rewardEvents, wepSwaps, buffEvents, evtcVersion);
                    }

                }
                else if (combatItem.IsActivation != Activation.None)
                {
                    insertToSkillIDs = true;
                    castCombatEvents.AddToList(combatItem.SrcAgent, combatItem);
                }
                else if (combatItem.IsBuffRemove != BuffRemove.None)
                {
                    insertToSkillIDs = true;
                    CombatEventFactory.AddBuffRemoveEvent(combatItem, buffEvents, agentData, skillData);
                }
                else
                {
                    insertToSkillIDs = true;
                    if (combatItem.IsBuff != 0 && combatItem.BuffDmg == 0 && combatItem.Value > 0)
                    {
                        CombatEventFactory.AddBuffApplyEvent(combatItem, buffEvents, agentData, skillData, evtcVersion);
                    }
                    else if (combatItem.IsBuff == 0)
                    {
                        CombatEventFactory.AddDirectDamageEvent(combatItem, damageData, brkDamageData, crowdControlData, agentData, skillData);
                    }
                    else if (combatItem.IsBuff != 0 && combatItem.Value == 0)
                    {
                        CombatEventFactory.AddIndirectDamageEvent(combatItem, damageData, agentData, skillData);
                    }
                }

                if (insertToSkillIDs)
                {
                    _skillIds.Add(combatItem.SkillID);
                }
            }
            _statusEvents.EffectEvents.ForEach(x => x.SetGUIDEvent(this));
            _statusEvents.MarkerEvents.ForEach(x => x.SetGUIDEvent(this));

            HasStackIDs = evtcVersion.Build > ArcDPSBuilds.ProperConfusionDamageSimulation && buffEvents.Any(x => x is BuffStackActiveEvent || x is BuffStackResetEvent);
            UseBuffInstanceSimulator = false;// evtcVersion.Build > ArcDPSBuilds.RemovedDurationForInfiniteDurationStacksChanged && HasStackIDs && (fightData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Instanced10 || fightData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Instanced5 || fightData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Benchmark);
            HasMovementData = _statusEvents.MovementEvents.Count > 1;
            HasBreakbarDamageData = brkDamageData.Count != 0;
            HasEffectData = _statusEvents.EffectEvents.Count != 0;
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Combining SkillInfo with SkillData");
            skillData.CombineWithSkillInfo(_metaDataEvents.SkillInfoEvents);
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Cast Events");
            List<AnimatedCastEvent> animatedCastData = CombatEventFactory.CreateCastEvents(castCombatEvents, agentData, skillData, fightData);
            _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            _animatedCastData = animatedCastData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
            //TODO(Rennorb) @perf
            _instantCastData = new();
            _instantCastDataById = new();
            _animatedCastDataById = animatedCastData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Buff Events");
            _buffDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _buffData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
            OffsetBuffExtensionEvents(evtcVersion);
            // damage events
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Damage Events");
            _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _damageDataById = damageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageData = brkDamageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageDataById = brkDamageData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _breakbarDamageTakenData = brkDamageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            _crowControlData = crowdControlData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
            _crowControlDataById = crowdControlData.GroupBy(x => x.SkillId).ToDictionary(x => x.Key, x => x.ToList());
            _crowControlTakenData = crowdControlData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            // buff depend events
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Buff Dependent Events");
            BuildBuffDependentContainers();
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Attaching Extension Events");
            foreach (AbstractExtensionHandler handler in extensions.Values)
            {
                handler.AttachToCombatData(this, operation, GetGW2BuildEvent().Build);
            }
            //
            operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Events");
            EIExtraEventProcess(players, skillData, agentData, fightData, operation, evtcVersion);
        }

        private void BuildBuffDependentContainers()
        {
            _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
            _buffDataByIDByDst = _buffData.ToDictionary(x => x.Key, x => x.Value.GroupBy(y => y.To).ToDictionary(y => y.Key, y => y.ToList()));
            //TODO(Rennorb) @perf @mem: find average complexity
            _buffDataByInstanceID = new(_buffData.Count / 10);
            foreach (var buffEvents in _buffData.Values)
            {
                foreach (AbstractBuffEvent abe in buffEvents)
                {
                    if (!_buffDataByInstanceID.TryGetValue(abe.BuffID, out var dict))
                    {
                        //TODO(Rennorb) @perf @mem: find average complexity
                        dict = new(10);
                        _buffDataByInstanceID[abe.BuffID] = dict;
                    }

                    uint buffInstance = (abe) switch {
                        AbstractBuffApplyEvent abae => abae.BuffInstance,
                        AbstractBuffStackEvent abse => abse.BuffInstance,
                        BuffRemoveSingleEvent brse => brse.BuffInstance,
                        _ => 0,
                    };

                    if (buffInstance != 0)
                    {
                        dict.AddToList(buffInstance, abe);
                    }
                }
            }
        }

        // getters

        public IReadOnlyCollection<long> GetSkills()
        {
            return _skillIds;
        }

        public IReadOnlyList<AliveEvent> GetAliveEvents(AgentItem src)
        {
            return _statusEvents.AliveEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEvents(AgentItem targetedAgent)
        {
            return _statusEvents.AttackTargetEvents.GetValueOrEmpty(targetedAgent);
        }

        public IReadOnlyList<AttackTargetEvent> GetAttackTargetEventsByAttackTarget(AgentItem attackTarget)
        {
            return _statusEvents.AttackTargetEventsByAttackTarget.GetValueOrEmpty(attackTarget);
        }

        public IReadOnlyList<DeadEvent> GetDeadEvents(AgentItem src)
        {
            return _statusEvents.DeadEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<DespawnEvent> GetDespawnEvents(AgentItem src)
        {
            return _statusEvents.DespawnEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<DownEvent> GetDownEvents(AgentItem src)
        {
            return _statusEvents.DownEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<EnterCombatEvent> GetEnterCombatEvents(AgentItem src)
        {
            return _statusEvents.EnterCombatEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<ExitCombatEvent> GetExitCombatEvents(AgentItem src)
        {
            return _statusEvents.ExitCombatEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<GuildEvent> GetGuildEvents(AgentItem src)
        {
            return _metaDataEvents.GuildEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<HealthUpdateEvent> GetHealthUpdateEvents(AgentItem src)
        {
            return _statusEvents.HealthUpdateEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<BarrierUpdateEvent> GetBarrierUpdateEvents(AgentItem src)
        {
            return _statusEvents.BarrierUpdateEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<MaxHealthUpdateEvent> GetMaxHealthUpdateEvents(AgentItem src)
        {
            return _statusEvents.MaxHealthUpdateEvents.GetValueOrEmpty(src);
        }

        public PointOfViewEvent GetPointOfViewEvent()
        {
            return _metaDataEvents.PointOfViewEvent;
        }

        public EvtcVersionEvent GetEvtcVersionEvent()
        {
            return _metaDataEvents.EvtcVersionEvent;
        }

        public FractalScaleEvent GetFractalScaleEvent()
        {
            return _metaDataEvents.FractalScaleEvent;
        }

        public IReadOnlyList<SpawnEvent> GetSpawnEvents(AgentItem src)
        {
            return _statusEvents.SpawnEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<TargetableEvent> GetTargetableEvents(AgentItem attackTarget)
        {
            return _statusEvents.TargetableEvents.GetValueOrEmpty(attackTarget);
        }
        /// <summary>
        /// Returns squad marker events of given marker index
        /// </summary>
        /// <param name="markerIndex">marker index</param>
        /// <returns></returns>
        public IReadOnlyList<SquadMarkerEvent> GetSquadMarkerEvents(SquadMarkerIndex markerIndex)
        {
            return _statusEvents.SquadMarkerEventsByIndex.GetValueOrEmpty(markerIndex);
        }
        /// <summary>
        /// Returns marker events owned by agent
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        public IReadOnlyList<MarkerEvent> GetMarkerEvents(AgentItem agent)
        {
            return _statusEvents.MarkerEventsBySrc.GetValueOrEmpty(agent);
        }
        /// <summary>
        /// Returns marker events of given marker ID
        /// </summary>
        /// <param name="markerID">marker ID</param>
        /// <returns></returns>
        public IReadOnlyList<MarkerEvent> GetMarkerEventsByMarkerID(long markerID)
        {
            return _statusEvents.MarkerEventsByID.GetValueOrEmpty(markerID);
        }
        /// <summary>
        /// True if marker events of given marker GUID has been found
        /// </summary>
        /// <param name="markerGUID">marker GUID</param>
        /// <param name="markerEvents">Found marker events</param>
        /// <returns></returns>
        public bool TryGetMarkerEventsByGUID(string markerGUID, [NotNullWhen(true)] out IReadOnlyList<MarkerEvent>? markerEvents)
        {
            MarkerGUIDEvent markerGUIDEvent = GetMarkerGUIDEvent(markerGUID);
            if (markerGUIDEvent != null)
            {
                markerEvents = GetMarkerEventsByMarkerID(markerGUIDEvent.ContentID);
                return true;
            }
            markerEvents = null;
            return false;
        }
        /// <summary>
        /// True if marker events of given marker GUID has been found on given agent
        /// </summary>
        /// <param name="agent">marker owner</param>
        /// <param name="markerGUID">marker GUID</param>
        /// <param name="markerEvents">Found marker events</param>
        /// <returns></returns>
        public bool TryGetMarkerEventsBySrcWithGUID(AgentItem agent, string markerGUID, [NotNullWhen(true)] out IReadOnlyList<MarkerEvent>? markerEvents)
        {
            if (TryGetMarkerEventsByGUID(markerGUID, out IReadOnlyList<MarkerEvent> markers))
            {
                markerEvents = markers.Where(effect => effect.Src == agent).ToList();
                return true;
            }
            markerEvents = null;
            return false;
        }

        public IReadOnlyList<TeamChangeEvent> GetTeamChangeEvents(AgentItem src)
        {
            return _statusEvents.TeamChangeEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<BreakbarStateEvent> GetBreakbarStateEvents(AgentItem src)
        {
            return _statusEvents.BreakbarStateEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<BreakbarPercentEvent> GetBreakbarPercentEvents(AgentItem src)
        {
            return _statusEvents.BreakbarPercentEvents.GetValueOrEmpty(src);
        }

        public GW2BuildEvent GetGW2BuildEvent()
        {
            if (_metaDataEvents.GW2BuildEvent == null)
            {
                throw new EvtcCombatEventException("Missing Build Event");
            }
            return _metaDataEvents.GW2BuildEvent;
        }

        public LanguageEvent GetLanguageEvent()
        {
            return _metaDataEvents.LanguageEvent;
        }

        public InstanceStartEvent GetInstanceStartEvent()
        {
            return _metaDataEvents.InstanceStartEvent;
        }

        public LogStartEvent GetLogStartEvent()
        {
            return _metaDataEvents.LogStartEvent;
        }

        public IReadOnlyList<LogNPCUpdateEvent> GetLogNPCUpdateEvents()
        {
            return _metaDataEvents.LogNPCUpdateEvents;
        }

        public LogEndEvent GetLogEndEvent()
        {
            return _metaDataEvents.LogEndEvent;
        }

        public IReadOnlyList<MapIDEvent> GetMapIDEvents()
        {
            return _metaDataEvents.MapIDEvents;
        }

        public IReadOnlyList<RewardEvent> GetRewardEvents()
        {
            return _rewardEvents;
        }

        public IReadOnlyList<ErrorEvent> GetErrorEvents()
        {
            return _metaDataEvents.ErrorEvents;
        }

        public IReadOnlyList<ShardEvent> GetShardEvents()
        {
            return _metaDataEvents.ShardEvents;
        }

        public IReadOnlyList<TickRateEvent> GetTickRateEvents()
        {
            return _metaDataEvents.TickRateEvents;
        }

        public BuffInfoEvent? GetBuffInfoEvent(long buffID)
        {
            return _metaDataEvents.BuffInfoEvents.GetValueOrDefault(buffID);
        }

        public IReadOnlyList<BuffInfoEvent> GetBuffInfoEvent(byte category)
        {
            return _metaDataEvents.BuffInfoEventsByCategory.GetValueOrEmpty(category);
        }

        public SkillInfoEvent? GetSkillInfoEvent(long skillID)
        {
            return _metaDataEvents.SkillInfoEvents.GetValueOrDefault(skillID);
        }

        public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents()
        {
            return _statusEvents.Last90BeforeDownEvents;
        }

        public IReadOnlyList<Last90BeforeDownEvent> GetLast90BeforeDownEvents(AgentItem src)
        {
            return _statusEvents.Last90BeforeDownEventsBySrc.GetValueOrEmpty(src);
        }

        public IReadOnlyList<AbstractBuffEvent> GetBuffData(long buffID)
        {
            return _buffData.GetValueOrEmpty(buffID);
        }

        /// <summary>
        /// Returns list of buff events applied on agent for given id
        /// </summary>
        public IReadOnlyList<AbstractBuffEvent> GetBuffDataByIDByDst(long buffID, AgentItem dst)
        {
            if (_buffDataByIDByDst.TryGetValue(buffID, out var agentDict))
            {
                if (agentDict.TryGetValue(dst, out var res))
                {
                    return res;
                }
            }
            return [ ];
        }

        public IReadOnlyList<AbstractBuffEvent> GetBuffDataByInstanceID(long buffID, uint instanceID)
        {
            if (instanceID == 0)
            {
                return GetBuffData(buffID);
            }
            if (_buffDataByInstanceID.TryGetValue(buffID, out Dictionary<uint, List<AbstractBuffEvent>> dict))
            {
                if (dict.TryGetValue(instanceID, out List<AbstractBuffEvent> list))
                {
                    return list;
                }
            }
            return [ ];
        }

        public IReadOnlyList<BuffRemoveAllEvent> GetBuffRemoveAllData(long buffID)
        {
            return _buffRemoveAllData.GetValueOrEmpty(buffID);
        }

        /// <summary>
        /// Returns list of buff events applied on agent
        /// </summary>
        public IReadOnlyList<AbstractBuffEvent> GetBuffDataByDst(AgentItem dst)
        {
            return _buffDataByDst.GetValueOrEmpty(dst);
        }

        /// <summary>
        /// Returns list of damage events done by agent
        /// </summary>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(AgentItem src)
        {
            return _damageData.GetValueOrEmpty(src);
        }

        /// <summary>
        /// Returns list of breakbar damage events done by agent
        /// </summary>
        public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageData(AgentItem src)
        {
            return _breakbarDamageData.GetValueOrEmpty(src);
        }

        /// <summary>
        /// Returns list of breakbar damage events done by skill id
        /// </summary>
        public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageData(long skillID)
        {
            return _breakbarDamageDataById.GetValueOrEmpty(skillID);
        }

        /// <summary>
        /// Returns list of damage events applied by a skill
        /// </summary>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageData(long skillID)
        {
            return _damageDataById.GetValueOrEmpty(skillID);
        }

        /// <summary>
        /// Returns list of crowd control events done by agent
        /// </summary>
        public IReadOnlyList<CrowdControlEvent> GetOutgoingCrowdControlData(AgentItem src)
        {
            return _crowControlData.GetValueOrEmpty(src);
        }

        /// <summary>
        /// Returns list of crowd control events done by skill id
        /// </summary>
        public IReadOnlyList<CrowdControlEvent> GetCrowdControlData(long skillID)
        {
            return _crowControlDataById.GetValueOrEmpty(skillID);
        }

        /// <summary>
        /// Returns list of animated cast events done by Agent
        /// </summary>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(AgentItem caster)
        {
            return _animatedCastData.GetValueOrEmpty(caster);
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(AgentItem caster)
        {
            return _instantCastData.GetValueOrEmpty(caster);
        }

        /// <summary>
        /// Returns list of instant cast events done by Agent
        /// </summary>
        public IReadOnlyList<InstantCastEvent> GetInstantCastData(long skillID)
        {
            return _instantCastDataById.GetValueOrEmpty(skillID);
        }

        /// <summary>
        /// Returns list of weapon swap events done by Agent
        /// </summary>
        public IReadOnlyList<WeaponSwapEvent> GetWeaponSwapData(AgentItem caster)
        {
            return _weaponSwapData.GetValueOrEmpty(caster);
        }

        /// <summary>
        /// Returns list of cast events from skill
        /// </summary>
        public IReadOnlyList<AnimatedCastEvent> GetAnimatedCastData(long skillID)
        {
            return _animatedCastDataById.GetValueOrEmpty(skillID);
        }

        /// <summary>
        /// Returns list of damage events taken by Agent
        /// </summary>
        public IReadOnlyList<AbstractHealthDamageEvent> GetDamageTakenData(AgentItem dst)
        {
            return _damageTakenData.GetValueOrEmpty(dst);
        }

        /// <summary>
        /// Returns list of breakbar damage events taken by Agent
        /// </summary>
        public IReadOnlyList<BreakbarDamageEvent> GetBreakbarDamageTakenData(AgentItem dst)
        {
            return _breakbarDamageTakenData.GetValueOrEmpty(dst);
        }

        /// <summary>
        /// Returns list of crowd control events taken by Agent
        /// </summary>
        public IReadOnlyList<CrowdControlEvent> GetIncomingCrowdControlData(AgentItem dst)
        {
            return _crowControlTakenData.GetValueOrEmpty(dst);
        }

        public IReadOnlyList<AbstractMovementEvent> GetMovementData(AgentItem src)
        {
            return _statusEvents.MovementEvents.GetValueOrEmpty(src);
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsBySrc(AgentItem src)
        {
            return _statusEvents.EffectEventsBySrc.GetValueOrEmpty(src);
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByDst(AgentItem dst)
        {
            return _statusEvents.EffectEventsByDst.GetValueOrEmpty(dst);
        }

        public IReadOnlyList<EffectEvent> GetEffectEventsByEffectID(long effectID)
        {
            return _statusEvents.EffectEventsByEffectID.GetValueOrEmpty(effectID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsByGUID(string effectGUID, [NotNullWhen(true)] out IReadOnlyList<EffectEvent>? effectEvents)
        {
            EffectGUIDEvent effectGUIDEvent = GetEffectGUIDEvent(effectGUID);
            if (effectGUIDEvent != null)
            {
                effectEvents = GetEffectEventsByEffectID(effectGUIDEvent.ContentID);
                if (effectEvents.Count > 0)
                {
                    return true;
                }
            }
            effectEvents = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="effectEvents"></param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsByGUIDs(Span<string> effectGUIDs, out List<EffectEvent> effectEvents)
        {
            //TODO(Rennorb) @perf: fid average complexity
            effectEvents = new(effectGUIDs.Length * 10);
            foreach (string effectGUID in effectGUIDs)
            {
                if (TryGetEffectEventsByGUID(effectGUID, out var effects))
                {
                    effectEvents.AddRange(effects);
                }
            }

            return effectEvents.Count > 0;
        }

        /// <summary>
        /// Returns effect events by the given agent and effect GUID.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns>true on found effect with entries > 0</returns>
        public bool TryGetEffectEventsBySrcWithGUID(AgentItem agent, string effectGUID, [NotNullWhen(true)] out List<EffectEvent>? effectEvents)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                var result = effects.Where(effect => effect.Src == agent).ToList();
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }

            effectEvents = null;
            return false;
        }

        /// <summary>
        /// Appends effect events by the given agent and effect GUID.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        public void AppendEffectEventsBySrcWithGUID(AgentItem agent, string effectGUID, List<EffectEvent> effectEvents)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                effectEvents.AddRange(effects.Where(effect => effect.Src == agent));
            }
        }


        /// <summary>
        /// Returns effect events on the given agent and effect GUID.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsByDstWithGUID(AgentItem agent, string effectGUID, [NotNullWhen(true)] out List<EffectEvent>? effectEvents)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                var result = effects.Where(effect => effect.Dst == agent).ToList();
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }

            effectEvents = null;
            return false;
        }
        /// <summary>
        /// Append effect events on the given agent and effect GUID.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        public void AppendEffectEventsByDstWithGUID(AgentItem agent, string effectGUID, List<EffectEvent> effectEvents)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                effectEvents.AddRange(effects.Where(effect => effect.Dst == agent));
            }
        }

        /// <summary>
        /// Returns effect events by the given agent and effect GUIDs.
        /// </summary>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsBySrcWithGUIDs(AgentItem agent, Span<string> effectGUIDs, out List<EffectEvent> effectEvents)
        {
            //TODO(Rennorb) @perf: find average complexity
            effectEvents = new List<EffectEvent>(effectGUIDs.Length * 10);
            foreach (var effectGUID in effectGUIDs)
            {
                AppendEffectEventsBySrcWithGUID(agent, effectGUID, effectEvents);
            }
            
            return effectEvents.Count > 0;
        }
        /// <summary>
        /// Returns effect events on the given agent and effect GUIDs.
        /// </summary>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsByDstWithGUIDs(AgentItem agent, Span<string> effectGUIDs, out List<EffectEvent> effectEvents)
        {
            //TODO(Rennorb) @perf: find average complexity
            effectEvents = new List<EffectEvent>(effectGUIDs.Length * 10);
            foreach (var effectGUID in effectGUIDs)
            {
                AppendEffectEventsByDstWithGUID(agent, effectGUID, effectEvents);
            }

            return effectEvents.Count > 0;
        }

        /// <summary>
        /// Returns effect events by the given agent <b>including</b> minions and the given effect GUID.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsByMasterWithGUID(AgentItem agent, string effectGUID, [NotNullWhen(true)] out List<EffectEvent>? effectEvents)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                var result = effects.Where(effect => effect.Src.GetFinalMaster() == agent).ToList();
                if (result.Count > 0)
                {
                    effectEvents = result;
                    return true;
                }
            }

            effectEvents = null;
            return false;
        }

        /// <summary>
        /// Returns effect events by the given agent <b>including</b> minions and the given effect GUID.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        public void AppendEffectEventsByMasterWithGUID(AgentItem agent, string effectGUID, List<EffectEvent> effectEvents)
        {
            if (TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                effectEvents.AddRange(effects.Where(effect => effect.Src.GetFinalMaster() == agent));
            }
        }

        /// <summary>
        /// Returns effect events by the given agent <b>including</b> minions and the given effect GUIDs.
        /// </summary>
        /// <param name="effectGUIDs">Strings in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns>true on success</returns>
        public bool TryGetEffectEventsByMasterWithGUIDs(AgentItem agent, Span<string> effectGUIDs, out List<EffectEvent> effectEvents)
        {
            effectEvents = new List<EffectEvent>();
            foreach (string effectGUID in effectGUIDs)
            {
                AppendEffectEventsByMasterWithGUID(agent, effectGUID, effectEvents);
            }

            return effectEvents.Count > 0;
        }

        /// <summary>
        /// Returns effect events by the given agent and effect GUID.
        /// Effects happening within epsilon milliseconds are grouped together.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="epsilon">Windows size</param>
        /// <returns>true on success</returns>
        public bool TryGetGroupedEffectEventsBySrcWithGUID(AgentItem agent, string effectGUID, [NotNullWhen(true)] out List<List<EffectEvent>>? groupedEffectEvents, long epsilon = ServerDelayConstant)
        {
            if(!TryGetEffectEventsBySrcWithGUID(agent, effectGUID, out var effects))
            {
                groupedEffectEvents = null;
                return false;
            }

            groupedEffectEvents = EpsilonWindowOverTime(effects, epsilon);

            return true;
        }
        /// <summary>
        /// Returns effect events for the given effect GUID.
        /// Effects happening within epsilon milliseconds are grouped together.
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <param name="epsilon">Window size</param>
        /// <returns>true on success</returns>
        public bool TryGetGroupedEffectEventsByGUID(string effectGUID, [NotNullWhen(true)] out List<List<EffectEvent>>? groupedEffectEvents, long epsilon = ServerDelayConstant)
        {
            if(!TryGetEffectEventsByGUID(effectGUID, out var effects))
            {
                groupedEffectEvents = null;
                return false;
            }

            groupedEffectEvents = EpsilonWindowOverTime(effects.ToList(), epsilon);

            return true;
        }

        static List<List<EffectEvent>> EpsilonWindowOverTime(List<EffectEvent> effects, long epsilon)
        {
            effects.SortByTime();
            //NOTE(Rennorb): Has entries due to invariant on TryGetEffectEventsBySrcWithGUID
            var startTime = effects[0].Time;
            var endTime = effects[^1].Time;
            var slices = Math.Max(1, (int)((endTime - startTime + (epsilon - 1)) / epsilon)); // ceiling of total duration / epsilon, and at least one slice
            var groupedEffectEvents = new List<List<EffectEvent>>(slices);


            var blockStart = startTime;
            var blockEnd = blockStart + epsilon;
            var currentBlock = new List<EffectEvent>(effects.Count / slices); // assume average distribution
            int index = 0;
            foreach(var @event in effects)
            {
                if(@event.Time >= blockEnd)
                {
                    groupedEffectEvents.Add(currentBlock);
                    currentBlock = new((effects.Count - index) / slices); // assume average distribution in remaining blocks
                }

                currentBlock.Add(@event);
                index++;
            }
            groupedEffectEvents.Add(currentBlock);

            return groupedEffectEvents;
        }


        public IReadOnlyList<EffectEvent> GetEffectEvents()
        {
            return _statusEvents.EffectEvents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns></returns>
        public EffectGUIDEvent GetEffectGUIDEvent(string effectGUID)
        {
            if (_metaDataEvents.EffectGUIDEventsByGUID.TryGetValue(effectGUID, out EffectGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="effectID">ID of the effect</param>
        /// <returns></returns>
        internal EffectGUIDEvent GetEffectGUIDEvent(long effectID)
        {
            if (_metaDataEvents.EffectGUIDEventsByEffectID.TryGetValue(effectID, out EffectGUIDEvent evt))
            {
                return evt;
            }
#if DEBUG
            if (GetEffectEventsByEffectID(effectID).Count > 0)
            {
                throw new EvtcCombatEventException("Missing GUID event for effect " + effectID);
            }
#endif
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markerGUID">String in hexadecimal (32 characters) or base64 (24 characters)</param>
        /// <returns></returns>
        public MarkerGUIDEvent GetMarkerGUIDEvent(string markerGUID)
        {
            if (_metaDataEvents.MarkerGUIDEventsByGUID.TryGetValue(markerGUID, out MarkerGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="markerID">ID of the marker</param>
        /// <returns></returns>
        internal MarkerGUIDEvent GetMarkerGUIDEvent(long markerID)
        {
            if (_metaDataEvents.MarkerGUIDEventsByMarkerID.TryGetValue(markerID, out MarkerGUIDEvent evt))
            {
                return evt;
            }
            return null;
        }

        public IReadOnlyList<GliderEvent> GetGliderEvents(AgentItem src)
        {
            if (_statusEvents.GliderEventsBySrc.TryGetValue(src, out List<GliderEvent> list))
            {
                return list;
            }
            return new List<GliderEvent>();
        }

        public IReadOnlyList<StunBreakEvent> GetStunBreakEvents(AgentItem src)
        {
            if (_statusEvents.StunBreakEventsBySrc.TryGetValue(src, out List<StunBreakEvent> list))
            {
                return list;
            }
            return new List<StunBreakEvent>();
        }

        /// 

        public static IEnumerable<T> FindRelatedEvents<T>(IEnumerable<T> events, long time, long epsilon = ServerDelayConstant) where T : AbstractTimeCombatEvent
        {
            return events.Where(evt => Math.Abs(evt.Time - time) < epsilon);
        }

        public bool HasRelatedHit(long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetDamageData(skillID), time, epsilon)
                .Any(hit => hit.CreditedFrom == agent);
        }

        public bool HasPreviousCast(long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetAnimatedCastData(skillID), time, epsilon)
                .Any(cast => cast.Caster == agent && cast.Time <= time);
        }

        public bool IsCasting(long skillID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return GetAnimatedCastData(skillID)
                .Any(cast => cast.Caster == agent && cast.Time - epsilon <= time && cast.EndTime + epsilon >= time);
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any();
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source);
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, long appliedDuration, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => Math.Abs(apply.AppliedDuration - appliedDuration) < epsilon);
        }

        public bool HasGainedBuff(long buffID, AgentItem agent, long time, long appliedDuration, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffApplyEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source && Math.Abs(apply.AppliedDuration - appliedDuration) < epsilon);
        }

        public bool HasLostBuff(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffRemoveAllEvent>(), time, epsilon)
                .Any();
        }

        public bool HasLostBuffStack(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<AbstractBuffRemoveEvent>(), time, epsilon)
                .Any();
        }

        public bool HasRelatedEffect(string effectGUID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            if (TryGetEffectEventsBySrcWithGUID(agent, effectGUID, out var effectEvents))
            {
                return FindRelatedEvents(effectEvents, time, epsilon).Any();
            }
            return false;
        }

        public bool HasRelatedEffectDst(string effectGUID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            if (TryGetEffectEventsByDstWithGUID(agent, effectGUID, out var effectEvents))
            {
                return FindRelatedEvents(effectEvents, time, epsilon).Any();
            }
            return false;
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any();
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source);
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, long extendedDuration, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => Math.Abs(apply.ExtendedDuration - extendedDuration) < epsilon);
        }

        public bool HasExtendedBuff(long buffID, AgentItem agent, long time, long extendedDuration, AgentItem source, long epsilon = ServerDelayConstant)
        {
            return FindRelatedEvents(GetBuffDataByIDByDst(buffID, agent).OfType<BuffExtensionEvent>(), time, epsilon)
                .Any(apply => apply.CreditedBy == source && Math.Abs(apply.ExtendedDuration - extendedDuration) < epsilon);
        }

    }
}
