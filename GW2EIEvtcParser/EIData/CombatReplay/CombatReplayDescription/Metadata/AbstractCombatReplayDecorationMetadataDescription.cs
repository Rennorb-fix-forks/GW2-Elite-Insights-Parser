﻿using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData
{
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
    [JsonDerivedType(typeof(ActorOrientationDecorationMetadataDescription))]
    [JsonDerivedType(typeof(MovingPlatformDecorationMetadataDescription))]
    [JsonDerivedType(typeof(BackgroundIconDecorationMetadataDescription))]
    [JsonDerivedType(typeof(IconDecorationMetadataDescription))]
    [JsonDerivedType(typeof(IconOverheadDecorationMetadataDescription))]
    [JsonDerivedType(typeof(CircleDecorationMetadataDescription))]
    [JsonDerivedType(typeof(DoughnutDecorationMetadataDescription))]
    [JsonDerivedType(typeof(LineDecorationMetadataDescription))]
    [JsonDerivedType(typeof(PieDecorationMetadataDescription))]
    [JsonDerivedType(typeof(RectangleDecorationMetadataDescription))]
    public abstract class AbstractCombatReplayDecorationMetadataDescription : AbstractCombatReplayDescription
    {
        internal AbstractCombatReplayDecorationMetadataDescription() 
        {
        }
    }
}
