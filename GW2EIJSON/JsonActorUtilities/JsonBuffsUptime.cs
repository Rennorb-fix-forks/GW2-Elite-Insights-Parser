﻿using System.Collections.Generic;


namespace GW2EIJSON
{
    /// <summary>
    /// Class representing buff uptimes
    /// </summary>
    public class JsonBuffsUptime
    {
        /// <summary>
        /// Buff uptime data
        /// </summary>
        public class JsonBuffsUptimeData
        {
            
            /// <summary>
            /// Uptime of the buff
            /// </summary>
            public double Uptime { get; set; }
            
            /// <summary>
            /// Presence of the buff (intensity only)
            /// </summary>
            public double Presence { get; set; }
            
            /// <summary>
            /// Buff generated by
            /// </summary>
            public IReadOnlyDictionary<string, double> Generated { get; set; }
            
            /// <summary>
            /// Buff overstacked by
            /// </summary>
            public IReadOnlyDictionary<string, double> Overstacked { get; set; }
            
            /// <summary>
            /// Buff wasted by
            /// </summary>
            public IReadOnlyDictionary<string, double> Wasted { get; set; }
            
            /// <summary>
            /// Buff extended by unknown for
            /// </summary>
            public IReadOnlyDictionary<string, double> UnknownExtended { get; set; }
            
            /// <summary>
            /// Buff by extension
            /// </summary>
            public IReadOnlyDictionary<string, double> ByExtension { get; set; }
            
            /// <summary>
            /// Buff extended for
            /// </summary>
            public IReadOnlyDictionary<string, double> Extended { get; set; }

            
            public JsonBuffsUptimeData()
            {

            }      
        }

        
        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonBuffsUptimeData"/>
        public IReadOnlyList<JsonBuffsUptimeData> BuffData { get; set; }
        
        /// <summary>
        /// Array of int[2] that represents the number of buff \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of buff present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public IReadOnlyList<IReadOnlyList<int>> States { get; set; }


        /// <summary>
        /// Key corresponds to the name of the source \n
        /// Array of int[2] that represents the number of buff \n
        /// Array[i][0] will be the time, Array[i][1] will be the number of buff present from Array[i][0] to Array[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public IReadOnlyDictionary<string,IReadOnlyList<IReadOnlyList<int>>> StatesPerSource { get; set; }


        public JsonBuffsUptime()
        {

        }     
    }

}
