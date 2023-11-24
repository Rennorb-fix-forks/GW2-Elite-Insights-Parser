﻿using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class IncomingDamageModifier : DamageModifier
    {
        internal IncomingDamageModifier(DamageModifierDescriptor damageModDescriptor) : base(damageModDescriptor)
        {
        }

        public override int GetTotalDamage(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor t, long start, long end)
        {
            FinalDefenses defenseData = actor.GetDefenseStats(t, log, start, end);
            switch (CompareType)
            {
                case DamageType.All:
                    return defenseData.DamageTaken ;
                case DamageType.Condition:
                    return defenseData.CondiDamageTaken ;
                case DamageType.Power:
                    return defenseData.PowerDamageTaken;
                case DamageType.LifeLeech:
                    return defenseData.LifeLeechDamageTaken;
                case DamageType.Strike:
                    return defenseData.StrikeDamageTaken;
                case DamageType.StrikeAndCondition:
                    return defenseData.StrikeDamageTaken + defenseData.CondiDamageTaken;
                case DamageType.StrikeAndConditionAndLifeLeech:
                    return defenseData.StrikeDamageTaken + defenseData.CondiDamageTaken + defenseData.LifeLeechDamageTaken;
                default:
                    throw new NotImplementedException("Not implemented damage type " + CompareType);
            }
        }

        public override IReadOnlyList<AbstractHealthDamageEvent> GetHitDamageEvents(AbstractSingleActor actor, ParsedEvtcLog log, AbstractSingleActor t, long start, long end)
        {
            return actor.GetHitDamageTakenEvents(t, log, start, end, SrcType) ;
        }
        internal override AgentItem GetFoe(AbstractHealthDamageEvent evt)
        {
            return evt.From;
        }
    }
}
