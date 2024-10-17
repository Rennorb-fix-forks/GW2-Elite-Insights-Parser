﻿using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconOverheadDecoration;

namespace GW2EIEvtcParser.EIData
{
    public class IconOverheadDecorationRenderingDescription : IconDecorationRenderingDescription
    {

        internal IconOverheadDecorationRenderingDescription(ParsedEvtcLog log, IconOverheadDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(log, decoration, map, usedSkills, usedBuffs, metadataSignature)
        {
            Type = "IconOverheadDecoration";
            if (decoration.IsSquadMarker)
            {
                Type = "OverheadSquadMarkerDecoration";
            }
        }
    }

}
