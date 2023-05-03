﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MirageHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(Jaunt, Jaunt),
            // new EffectCastFinderByDst(Jaunt, EffectGUIDs.MirageJaunt).UsingDstSpecChecker(Spec.Mirage),
            new BuffGainCastFinder(MirageCloakDodge, MirageCloak), // Mirage Cloak
            //new EffectCastFinderByDst(IllusionaryAmbush, EffectGUIDs.MirageIllusionaryAmbush).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Mirage),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Mirage Cloak", MirageCloak, Source.Mirage, BuffClassification.Other, BuffImages.MirageCloak),
            new Buff("False Oasis", FalseOasis, Source.Mirage, BuffClassification.Other, BuffImages.FalseOasis),
        };

        private static HashSet<long> Minions = new HashSet<long>();
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
