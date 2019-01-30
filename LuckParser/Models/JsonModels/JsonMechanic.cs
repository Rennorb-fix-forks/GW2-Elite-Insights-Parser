﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    /// <summary>
    /// Class corresponding to a mechanic event
    /// </summary>
    public class JsonMechanic
    {
        /// <summary>
        /// Time a which the event happened
        /// </summary>
        public long Time;
        /// <summary>
        /// The actor who was hit by the mechanic
        /// </summary>
        public string Actor;
    }
}
