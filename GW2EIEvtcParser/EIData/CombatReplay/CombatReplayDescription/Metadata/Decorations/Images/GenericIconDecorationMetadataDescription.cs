﻿using System.IO;
using static GW2EIEvtcParser.EIData.GenericIconDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericIconDecorationMetadataDescription : GenericAttachedDecorationMetadataDescription
    {
        public string Image { get; }
        public uint PixelSize { get; }
        public uint WorldSize { get; }

        internal GenericIconDecorationMetadataDescription(GenericIconDecorationMetadata decoration) : base(decoration)
        {
            Image = decoration.Image;
            PixelSize = decoration.PixelSize;
            WorldSize = decoration.WorldSize;
            if (WorldSize == 0 && PixelSize == 0)
            {
                throw new InvalidDataException("Icon Decoration must have at least one size strictly positive");
            }
        }
    }

}
