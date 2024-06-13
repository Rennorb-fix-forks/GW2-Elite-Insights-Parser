﻿using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericIconDecoration : GenericAttachedDecoration
    {
        internal abstract class ConstantGenericIconDecoration : ConstantGenericAttachedDecoration
        {
            public string Image { get; }
            public uint PixelSize { get; }
            public uint WorldSize { get; }
            protected ConstantGenericIconDecoration(string icon, uint pixelSize, uint worldSize) : base()
            {
                Image = icon;
                PixelSize = pixelSize;
                WorldSize = worldSize;
            }
        }
        internal abstract class VariableGenericIconDecoration : VariableGenericAttachedDecoration
        {
            protected VariableGenericIconDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
        }
        private new ConstantGenericIconDecoration ConstantDecoration => (ConstantGenericIconDecoration)base.ConstantDecoration;
        public string Image => ConstantDecoration.Image;
        public uint PixelSize => ConstantDecoration.PixelSize;
        public uint WorldSize => ConstantDecoration.WorldSize;

        public GenericIconDecoration() : base()
        {
        }
    }
}
