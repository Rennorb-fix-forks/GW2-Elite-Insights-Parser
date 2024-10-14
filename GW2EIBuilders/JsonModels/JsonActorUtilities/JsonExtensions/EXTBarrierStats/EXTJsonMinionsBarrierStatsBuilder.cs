﻿using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonExtensions.EXTBarrier
{
    internal static class EXTJsonMinionsBarrierStatsBuilder
    {

        public static EXTJsonMinionsBarrierStats BuildMinionsBarrierStats(Minions minions, ParsedEvtcLog log, Dictionary<long, SkillItem> skillMap, Dictionary<long, Buff> buffMap)
        {
            IReadOnlyList<PhaseData> phases = log.FightData.GetPhases(log);
            var totalAlliedBarrier = new List<List<int>>(log.Friendlies.Count);
            var alliedBarrierDist = new List<List<List<EXTJsonBarrierDist>>>(log.Friendlies.Count);
            foreach (AbstractSingleActor friendly in log.Friendlies)
            {
                var totalAllyBarrier = new List<int>(phases.Count);
                var allyBarrierDist = new List<List<EXTJsonBarrierDist>>(phases.Count);
                foreach (PhaseData phase in phases)
                {
                    var list = minions.EXTBarrier.GetOutgoingBarrierEvents(friendly, log, phase.Start, phase.End);
                    totalAllyBarrier.Add(list.Sum(x => x.BarrierGiven));
                    allyBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(list.GroupBy(x => x.SkillId), log, skillMap, buffMap).ToList());
                }
                totalAlliedBarrier.Add(totalAllyBarrier);
                alliedBarrierDist.Add(allyBarrierDist);
            }

            var totalBarrier = new List<int>(phases.Count);
            var totalBarrierDist = new List<List<EXTJsonBarrierDist>>(phases.Count);
            foreach (PhaseData phase in phases)
            {
                var list = minions.EXTBarrier.GetOutgoingBarrierEvents(null, log, phase.Start, phase.End);
                totalBarrier.Add(list.Sum(x => x.BarrierGiven));
                totalBarrierDist.Add(EXTJsonBarrierStatsBuilderCommons.BuildBarrierDistList(list.GroupBy(x => x.SkillId), log, skillMap, buffMap).ToList());
            }

            return new(totalBarrier, totalAlliedBarrier, totalBarrierDist, alliedBarrierDist);
        }

    }
}
