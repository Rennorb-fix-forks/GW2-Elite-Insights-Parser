﻿using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class ActorOrientationDecorationRenderableDescription : GenericAttachedDecorationRenderableDescription
    {

        internal ActorOrientationDecorationRenderableDescription(ParsedEvtcLog log, ActorOrientationDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(log, decoration, map, usedSkills, usedBuffs)
        {
            Type = "ActorOrientation";
            IsMechanicOrSkill = false;
        }

    }
}
