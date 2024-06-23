﻿using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconOverheadDecoration : IconDecoration
    {
        internal class IconOverheadDecorationMetadata : IconDecorationMetadata
        {


            public IconOverheadDecorationMetadata(string icon, uint pixelSize, uint worldSize, float opacity) : base(icon, pixelSize, worldSize, opacity)
            {
            }

            public override string GetSignature()
            {
                return "IO" + PixelSize + Image.GetHashCode().ToString() + WorldSize + Opacity.ToString();
            }
            internal override GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable)
            {
                if (variable is VariableIconOverheadDecoration expectedVariable)
                {
                    return new IconOverheadDecoration(this, expectedVariable);
                }
                throw new InvalidOperationException("Expected VariableIconOverheadDecoration");
            }
        }
        internal class VariableIconOverheadDecoration : VariableIconDecoration
        {
            public VariableIconOverheadDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
            public override void UsingSkillMode(SkillModeDescriptor skill)
            {
            }
        }
        internal IconOverheadDecoration(IconOverheadDecorationMetadata metadata, VariableIconOverheadDecoration variable) : base(metadata, variable)
        {
        }
        public IconOverheadDecoration(string icon, uint pixelSize, float opacity, (long start, long end) lifespan, AgentConnector connector) : base()
        {
            DecorationMetadata = new IconDecorationMetadata(icon, pixelSize, Math.Min(connector.Agent.HitboxWidth / 2, 250), opacity);
            VariableDecoration = new VariableIconDecoration(lifespan, connector);
        }

        public IconOverheadDecoration(string icon, uint pixelSize, float opacity, Segment lifespan, AgentConnector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new IconOverheadDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
