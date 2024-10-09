﻿using System;
using System.Linq;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class EffectEventCBTS45 : EffectEvent
    {

        private static unsafe Point3D ReadOrientation(CombatItem evtcItem)
        {
            var orientationBytes = stackalloc byte[2 * sizeof(float)];
            orientationBytes[0] = evtcItem.IFFByte;
            orientationBytes[1] = evtcItem.IsBuff;
            orientationBytes[2] = evtcItem.Result;
            orientationBytes[3] = evtcItem.IsActivationByte;
            orientationBytes[4] = evtcItem.IsBuffRemoveByte;
            orientationBytes[5] = evtcItem.IsNinety;
            orientationBytes[6] = evtcItem.IsFifty;
            orientationBytes[7] = evtcItem.IsMoving;

            var orientationFloats = (float*)orientationBytes;
            return new Point3D(orientationFloats[0], orientationFloats[1], -BitConverter.ToSingle(BitConverter.GetBytes(evtcItem.Pad), 0));
        }

        internal EffectEventCBTS45(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
        {
            Orientation = ReadOrientation(evtcItem);
        }

        protected override long ComputeEndTime(ParsedEvtcLog log, long maxDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            if (associatedBuff != null)
            {
                BuffRemoveAllEvent remove = log.CombatData.GetBuffDataByIDByDst(associatedBuff.Value, agent)
                    .OfType<BuffRemoveAllEvent>()
                    .FirstOrDefault(x => x.Time >= Time);
                if (remove != null)
                {
                    return remove.Time;
                }
            }
            return Time + maxDuration;
        }

        public override (long, long) ComputeDynamicLifespan(ParsedEvtcLog log, long defaultDuration, AgentItem agent = null, long? associatedBuff = null)
        {
            return base.ComputeDynamicLifespan(log, 0, agent, associatedBuff);
        }

    }
}
