﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
    [JsonDerivedType(typeof(NPCCombatReplayDescription))]
    [JsonDerivedType(typeof(PlayerCombatReplayDescription))]
    public abstract class AbstractSingleActorCombatReplayDescription : AbstractCombatReplayDescription
    {
        public long Start { get; protected set; }
        public long End { get; protected set; }
        public string Img { get; }
        public int ID { get; }
        public IReadOnlyList<float> Positions { get; }
        public IReadOnlyList<float> Angles { get; }
        public IReadOnlyList<long> Dead { get; private set; }
        public IReadOnlyList<long> Down { get; private set; }
        public IReadOnlyList<long> Dc { get; private set; }
        public IReadOnlyList<long> Hide { get; }
        public IReadOnlyList<long> BreakbarActive { get; private set; }

        public long HitboxWidth { get; }

        private static string GetActorType(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            if (actor.AgentItem.IsPlayer)
            {
                return !log.PlayerAgents.Contains(actor.AgentItem) ? "TargetPlayer" : "Player";
            }
            if (log.FightData.Logic.TargetAgents.Contains(actor.AgentItem))
            {
                return "Target";
            }
            if (log.FightData.Logic.NonPlayerFriendlyAgents.Contains(actor.AgentItem) || actor.AgentItem.GetFinalMaster().Type == ParsedData.AgentItem.AgentType.Player)
            {
                return "Friendly";
            }
            return "Mob";
        }

        internal AbstractSingleActorCombatReplayDescription(AbstractSingleActor actor, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay)
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;
            Img = actor.GetIcon();
            ID = actor.UniqueID;
            var positions = new List<float>();
            Positions = positions;
            var angles = new List<float>();
            Angles = angles;
            Type = GetActorType(actor, log);
            HitboxWidth = actor.AgentItem.HitboxWidth;
            foreach (Point3D pos in replay.PolledPositions)
            {
                (float x, float y) = map.GetMapCoord(pos.X, pos.Y);
                positions.Add(x);
                positions.Add(y);
            }
            foreach (Point3D facing in replay.PolledRotations)
            {
                angles.Add(-Point3D.GetZRotationFromFacing(facing));
            }
            if (replay.Hidden.Count != 0)
            {
                var hide = new List<long>();
                foreach (Segment seg in replay.Hidden)
                {
                    hide.Add(seg.Start);
                    hide.Add(seg.End);
                }
                Hide = hide;
            }
        }
        protected void SetStatus(ParsedEvtcLog log, AbstractSingleActor a)
        {
            var dead = new List<long>();
            Dead = dead;
            var down = new List<long>();
            Down = down;
            var dc = new List<long>();
            Dc = dc;
            (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs) = a.GetStatus(log);

            foreach (Segment seg in deads)
            {
                dead.Add(seg.Start);
                dead.Add(seg.End);
            }
            foreach (Segment seg in downs)
            {
                down.Add(seg.Start);
                down.Add(seg.End);
            }
            foreach (Segment seg in dcs)
            {
                dc.Add(seg.Start);
                dc.Add(seg.End);
            }
        }

        protected void SetBreakbarStatus(ParsedEvtcLog log, AbstractSingleActor a)
        {
            var active = new List<long>();
            BreakbarActive = active;
            (_, IReadOnlyList<Segment> actives, _, _) = a.GetBreakbarStatus(log);

            foreach (Segment seg in actives)
            {
                active.Add(seg.Start);
                active.Add(seg.End);
            }
        }

    }
}
