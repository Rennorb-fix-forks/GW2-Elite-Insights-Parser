﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class TempleOfFebe : SecretOfTheObscureStrike
    {
        public TempleOfFebe(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Icon = EncounterIconTempleOfFebe;
            Extension = "tmplfeb";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayTempleOfFebe,
                            // TODO
                            (1008, 1008),
                            (0,0,0,0));
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Cerus,
                (int)ArcDPSEnums.TrashID.EmbodimentOfDespair,
                (int)ArcDPSEnums.TrashID.EmbodimentOfEnvy,
                (int)ArcDPSEnums.TrashID.EmbodimentOfGluttony,
                (int)ArcDPSEnums.TrashID.EmbodimentOfMalice,
                (int)ArcDPSEnums.TrashID.EmbodimentOfRage,
                (int)ArcDPSEnums.TrashID.EmbodimentOfRegret,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>() { ArcDPSEnums.TrashID.MaliciousShadow };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Temple of Febe";
        }
    }
}
