﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    public class AgentItem
    {

        private static int AgentCount = 0;
        public enum AgentType { NPC, Gadget, Player, NonSquadPlayer }

        public bool IsPlayer => Type == AgentType.Player || Type == AgentType.NonSquadPlayer;
        public bool IsNPC => Type == AgentType.NPC || Type == AgentType.Gadget;

        // Fields
        public ulong Agent { get; }
        public int ID { get; protected set; } = ArcDPSEnums.NonIdentifiedSpecies;
        public int UniqueID { get; }
        public AgentItem Master { get; protected set; }
        public ushort InstID { get; protected set; }
        public AgentType Type { get; protected set; } = AgentType.NPC;
        public long FirstAware { get; protected set; }
        public long LastAware { get; protected set; } = long.MaxValue;
        public string Name { get; protected set; } = "UNKNOWN";
        public ParserHelper.Spec Spec { get; private set; } = ParserHelper.Spec.Unknown;
        public ParserHelper.Spec BaseSpec { get; private set; } = ParserHelper.Spec.Unknown;
        public ushort Toughness { get; protected set; }
        public ushort Healing { get; }
        public ushort Condition { get; }
        public ushort Concentration { get; }
        public uint HitboxWidth { get; }
        public uint HitboxHeight { get; }

        private bool Unamed { get; }

        public bool IsFake { get; }
        public bool IsNotInSquadFriendlyPlayer { get; private set; }

        // Constructors
        internal AgentItem(ulong agent, string name, ParserHelper.Spec spec, int id, AgentType type, ushort toughness, ushort healing, ushort condition, ushort concentration, uint hbWidth, uint hbHeight)
        {
            UniqueID = ++AgentCount;
            Agent = agent;
            Name = name;
            Spec = spec;
            BaseSpec = ParserHelper.SpecToBaseSpec(spec);
            ID = id;
            Type = type;
            Toughness = toughness;
            Healing = healing;
            Condition = condition;
            Concentration = concentration;
            HitboxWidth = hbWidth;
            HitboxHeight = hbHeight;
            //
            try
            {
                if (type == AgentType.Player)
                {
                    HitboxWidth = 48;
                    HitboxHeight = 240;
                    string[] splitStr = Name.Split('\0');
                    if (splitStr.Length < 2 || (splitStr[1].Length == 0 || splitStr[2].Length == 0 || splitStr[0].Contains("-")))
                    {
                        Type = AgentType.NonSquadPlayer;
                    }
                }
            }
            catch (Exception)
            {

            }
            Unamed = Name.Contains("ch"+ID+"-");
        }

        internal AgentItem(ulong agent, string name, ParserHelper.Spec spec, int id, ushort instid, ushort toughness, ushort healing, ushort condition, ushort concentration, uint hbWidth, uint hbHeight, long firstAware, long lastAware, bool isFake) : this(agent, name, spec, id, AgentType.NPC, toughness, healing, condition, concentration, hbWidth, hbHeight)
        {
            InstID = instid;
            FirstAware = firstAware;
            LastAware = lastAware;
            IsFake = isFake;
        }

        internal AgentItem(AgentItem other)
        {
            UniqueID = ++AgentCount;
            Agent = other.Agent;
            Name = other.Name;
            Spec = other.Spec;
            BaseSpec = other.BaseSpec;
            ID = other.ID;
            Type = other.Type;
            Toughness = other.Toughness;
            Healing = other.Healing;
            Condition = other.Condition;
            Concentration = other.Concentration;
            HitboxWidth = other.HitboxWidth;
            HitboxHeight = other.HitboxHeight;
            InstID = other.InstID;
            Master = other.Master;
            IsFake = other.IsFake;
            Unamed = other.Unamed;
        }

        internal AgentItem()
        {
            UniqueID = ++AgentCount;
        }

        internal void OverrideSpec(ParserHelper.Spec spec)
        {
            Spec = spec;
            BaseSpec = ParserHelper.SpecToBaseSpec(spec);
        }

        internal void OverrideIsNotInSquadFriendlyPlayer(bool status)
        {
            IsNotInSquadFriendlyPlayer = status;
        }

        internal void OverrideType(AgentType type)
        {
            Type = type;
        }

        internal void OverrideName(string name)
        {
            Name = name;
        }

        internal void SetInstid(ushort instid)
        {
            InstID = instid;
        }

        internal void OverrideID(int id)
        {
            ID = id;
        }

        internal void OverrideID(ArcDPSEnums.TrashID id)
        {
            ID = (int)id;
        }

        internal void OverrideID(ArcDPSEnums.TargetID id)
        {
            ID = (int)id;
        }

        internal void OverrideID(ArcDPSEnums.MinionID id)
        {
            ID = (int)id;
        }

        internal void OverrideID(ArcDPSEnums.ChestID id)
        {
            ID = (int)id;
        }

        internal void OverrideToughness(ushort toughness)
        {
            Toughness = toughness;
        }

        internal void OverrideAwareTimes(long firstAware, long lastAware)
        {
            FirstAware = firstAware;
            LastAware = lastAware;
        }

        internal void SetMaster(AgentItem master)
        {
            if (IsPlayer || master == this)
            {
                return;
            }
            Master = master;
        }

        internal AgentItem? GetMainAgentWhenAttackTarget(ParsedEvtcLog log, long time)
        {
            var atEvents = log.CombatData.GetAttackTargetEventsByAttackTarget(this);
            return atEvents.Any() ? atEvents.LastOrDefault(y => time >= y.Time)?.Src : this;
        }

        private static void AddValueToStatusList(List<Segment> dead, List<Segment> down, List<Segment> dc, AbstractStatusEvent cur, long nextTime, long minTime, int index)
        {
            long cTime = cur.Time;

            if (cur is DownEvent)
            {
                down.Add(new Segment(cTime, nextTime, 1));
            }
            else if (cur is DeadEvent)
            {
                dead.Add(new Segment(cTime, nextTime, 1));
            }
            else if (cur is DespawnEvent)
            {
                dc.Add(new Segment(cTime, nextTime, 1));
            }
            else if (index == 0 && cTime - minTime > 50)
            {
                dc.Add(new Segment(minTime, cTime, 1));
            }
        }

        internal void GetAgentStatus(List<Segment> dead, List<Segment> down, List<Segment> dc, CombatData combatData)
        {
            //TODO(Rennorb) @perf: find average complexity
            var downEvents = combatData.GetDownEvents(this);
            var aliveEvents = combatData.GetAliveEvents(this);
            var deadEvents = combatData.GetDeadEvents(this);
            var spawnEvents = combatData.GetSpawnEvents(this);
            var despawnEvents = combatData.GetDespawnEvents(this);

            var status = new List<AbstractStatusEvent>(
                downEvents.Count +
                aliveEvents.Count +
                deadEvents.Count +
                spawnEvents.Count +
                despawnEvents.Count
            );
            status.AddRange(downEvents);
            status.AddRange(aliveEvents);
            status.AddRange(deadEvents);
            status.AddRange(spawnEvents);
            status.AddRange(despawnEvents);
            dc.Add(new Segment(long.MinValue, FirstAware, 1));
            // State changes are not reliable on non squad actors, so we check if arc provided us with some, we skip events created by EI.
            if (Type == AgentType.NonSquadPlayer && !status.Any(x => !x.IsCustom))
            {
                return;
            }

            if (status.Count == 0)
            {
                dc.Add(new Segment(LastAware, long.MaxValue, 1));
                return;
            }

            status.SortByTime();

            for (int i = 0; i < status.Count - 1; i++)
            {
                AbstractStatusEvent cur = status[i];
                AbstractStatusEvent next = status[i + 1];
                AddValueToStatusList(dead, down, dc, cur, next.Time, FirstAware, i);
            }

            // check last value
            if (status.Count > 0)
            {
                AbstractStatusEvent cur = status.Last();
                AddValueToStatusList(dead, down, dc, cur, LastAware, FirstAware, status.Count - 1);
                if (cur is DeadEvent)
                {
                    dead.Add(new Segment(LastAware, long.MaxValue, 1));
                }
                else
                {
                    dc.Add(new Segment(LastAware, long.MaxValue, 1));
                }
            }
        }

        internal void GetAgentBreakbarStatus(List<Segment> nones, List<Segment> actives, List<Segment> immunes, List<Segment> recovering, CombatData combatData)
        {
            var status = new List<BreakbarStateEvent>(combatData.GetBreakbarStateEvents(this));
            // State changes are not reliable on non squad actors, so we check if arc provided us with some, we skip events created by EI.
            if (Type == AgentType.NonSquadPlayer && !status.Any(x => !x.IsCustom))
            {
                return;
            }

            if (status.Count == 0)
            {
                nones.Add(new Segment(FirstAware, LastAware, 1));
                return;
            }

            status.SortByTime();
            for (int i = 0; i < status.Count - 1; i++)
            {
                BreakbarStateEvent cur = status[i];
                if (i == 0 && cur.Time > FirstAware)
                {
                    nones.Add(new Segment(FirstAware, cur.Time, 1));
                }
                BreakbarStateEvent next = status[i + 1];
                switch (cur.State)
                {
                    case ArcDPSEnums.BreakbarState.Active:
                        actives.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                    case ArcDPSEnums.BreakbarState.Immune:
                        immunes.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                    case ArcDPSEnums.BreakbarState.None:
                        nones.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                    case ArcDPSEnums.BreakbarState.Recover:
                        recovering.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                }
            }
            // check last value
            if (status.Count > 0)
            {
                BreakbarStateEvent cur = status.Last();
                if (LastAware - cur.Time >= ParserHelper.ServerDelayConstant)
                {
                    switch (cur.State)
                    {
                        case ArcDPSEnums.BreakbarState.Active:
                            actives.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                        case ArcDPSEnums.BreakbarState.Immune:
                            immunes.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                        case ArcDPSEnums.BreakbarState.None:
                            nones.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                        case ArcDPSEnums.BreakbarState.Recover:
                            recovering.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                    }
                }

            }
        }

        public AgentItem GetFinalMaster()
        {
            AgentItem cur = this;
            while (cur.Master != null)
            {
                cur = cur.Master;
            }
            return cur;
        }

        public bool InAwareTimes(long time)
        {
            return FirstAware <= time && LastAware >= time;
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time, long window = 0)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.HasBuff(log, buffId, time, window);
        }

        /// <summary>
        /// Checks if a buff is present on the actor and applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        public bool HasBuff(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            return log.FindActor(this).HasBuff(log, by, buffId, time);
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, long buffId, long time)
        {
            return log.FindActor(this).GetBuffStatus(log, buffId, time);
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffId, long start, long end)
        {
            return log.FindActor(this).GetBuffStatus(log, buffId, start, end);
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            return log.FindActor(this).GetBuffStatus(log, by, buffId, time);
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long start, long end)
        {
            return log.FindActor(this).GetBuffStatus(log, by, buffId, start, end);
        }


        /// <summary>
        /// Checks if the agent will go into downstate before the next time they go above 90% health, or the fight ends.
        /// </summary>
        /// <param name="time">Current log time</param>
        /// <returns><see langword="true"/> if the agent will down before the next time they go above 90% health, otherwise <see langword="false"/>.</returns>
        public bool IsDownedBeforeNext90(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).IsDownBeforeNext90(log, time);
        }

        /// <summary>
        /// Checks if the agent is downed at given time.
        /// </summary>
        /// <param name="time">Downed time.</param>
        /// <returns><see langword="true"/> if the agent is downed, otherwise <see langword="false"/>.</returns>
        public bool IsDowned(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).IsDowned(log, time);
        }

        /// <summary>
        /// Checks if the agent is downed during a segment of time.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End Time.</param>
        /// <returns><see langword="true"/> if the agent is downed, otherwise <see langword="false"/>.</returns>
        public bool IsDowned(ParsedEvtcLog log, long start, long end)
        {
            return log.FindActor(this).IsDowned(log, start, end);
        }

        /// <summary>
        /// Checks if the agent is dead at given time
        /// </summary>
        /// <param name="time">Death time.</param>
        /// <returns><see langword="true"/> if the agent is dead, otherwise <see langword="false"/>.</returns>
        public bool IsDead(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).IsDead(log, time);
        }

        /// <summary>
        /// Checks if the agent is dead during a segment of time.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End Time.</param>
        /// <returns><see langword="true"/> if the agent is dead, otherwise <see langword="false"/>.</returns>
        public bool IsDead(ParsedEvtcLog log, long start, long end)
        {
            return log.FindActor(this).IsDead(log, start, end);
        }

        /// <summary>
        /// Checks if the agent is dc/not spawned at given time
        /// </summary>
        /// <param name="time">Presence time.</param>
        /// <returns><see langword="true"/> if the agent isn't present, otherwise <see langword="false"/>.</returns>
        public bool IsDC(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).IsDC(log, time);
        }

        /// <summary>
        /// Checks if the agent is dc/not spawned during a segment of time.
        /// </summary>
        /// <param name="start">Start time.</param>
        /// <param name="end">End Time.</param>
        /// <returns><see langword="true"/> if the agent isn't present, otherwise <see langword="false"/>.</returns>
        public bool IsDC(ParsedEvtcLog log, long start, long end)
        {
            return log.FindActor(this).IsDC(log, start, end);
        }

        public double GetCurrentHealthPercent(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).GetCurrentHealthPercent(log, time);
        }

        public double GetCurrentBarrierPercent(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).GetCurrentBarrierPercent(log, time);
        }

        public Point3D GetCurrentPosition(ParsedEvtcLog log, long time, long forwardWindow = 0)
        {
            return log.FindActor(this).GetCurrentPosition(log, time, forwardWindow);
        }

        public Point3D GetCurrentRotation(ParsedEvtcLog log, long time, long forwardWindow = 0)
        {
            return log.FindActor(this).GetCurrentRotation(log, time, forwardWindow);
        }

        public ArcDPSEnums.BreakbarState GetCurrentBreakbarState(ParsedEvtcLog log, long time)
        {
            return log.FindActor(this).GetCurrentBreakbarState(log, time);
        }

        public bool IsUnamedSpecies()
        {
            if (IsPlayer)
            {
                return false;
            }
            return IsNonIdentifiedSpecies() || Unamed;
        }

        public bool IsNonIdentifiedSpecies()
        {
            return IsSpecies(ArcDPSEnums.NonIdentifiedSpecies);
        }

        public bool IsSpecies(int id)
        {
            return !IsPlayer && ID == id;
        }

        public bool IsSpecies(ArcDPSEnums.TrashID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsSpecies(ArcDPSEnums.TargetID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsSpecies(ArcDPSEnums.MinionID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsSpecies(ArcDPSEnums.ChestID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsAnySpecies(IEnumerable<int> ids)
        {
            return ids.Any(IsSpecies);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.TrashID> ids)
        {
            return ids.Any(IsSpecies);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.TargetID> ids)
        {
            return ids.Any(IsSpecies);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.MinionID> ids)
        {
            return ids.Any(IsSpecies);
        }

        public bool IsAnySpecies(IEnumerable<ArcDPSEnums.ChestID> ids)
        {
            return ids.Any(IsSpecies);
        }
    }

    public static partial class ListExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? FirstByAware<T>(this IReadOnlyList<T> agents) where T : AgentItem
        {
            (T? Agent, long FirstAware) result = (default, long.MaxValue);
            foreach(var agent in agents)
            {
                if(agent.FirstAware < result.FirstAware)
                {
                    result = (agent, agent.FirstAware);
                }
            }
            return result.Agent;
        }
    }
}
