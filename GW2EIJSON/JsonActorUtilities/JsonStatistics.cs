﻿

namespace GW2EIJSON;

/// <summary>
/// Class representing general statistics
/// </summary>
public static class JsonStatistics
{
    /// <summary>
    /// Defensive stats
    /// </summary>
    public class JsonDefensesAll
    {
        /// <summary>
        /// Total damage taken
        /// </summary>
        public long DamageTaken;
        /// <summary>
        /// Total condition damage taken
        /// </summary>
        public long ConditionDamageTaken;
        /// <summary>
        /// Total power damage taken
        /// </summary>
        public long PowerDamageTaken;
        /// <summary>
        /// Total strike damage taken
        /// </summary>
        public long StrikeDamageTaken;
        /// <summary>
        /// Total life leech damage taken
        /// </summary>
        public long LifeLeechDamageTaken;
        /// <summary>
        /// Total damage taken while downed
        /// </summary>
        public long DownedDamageTaken;

        /// <summary>
        /// Total breakbar damage taken
        /// </summary>
        public double BreakbarDamageTaken;

        /// <summary>
        /// Number of blocks
        /// </summary>
        public int BlockedCount;

        /// <summary>
        /// Number of evades
        /// </summary>
        public int EvadedCount;

        /// <summary>
        /// Number of misses
        /// </summary>
        public int MissedCount;

        /// <summary>
        /// Number of dodges
        /// </summary>
        public int DodgeCount;

        /// <summary>
        /// Number of time an incoming attack was negated by invul
        /// </summary>
        public int InvulnedCount;

        /// <summary>
        /// Damage done against barrier
        /// </summary>
        public int DamageBarrier;

        /// <summary>
        /// Number of time interrupted
        /// </summary>
        public int InterruptedCount;

        /// <summary>
        /// Number of time downed
        /// </summary>
        public int DownCount;

        /// <summary>
        /// Time passed in downstate
        /// </summary>
        public long DownDuration;

        /// <summary>
        /// Number of time died
        /// </summary>
        public int DeadCount;

        /// <summary>
        /// Time passed in dead state
        /// </summary>
        public long DeadDuration;

        /// <summary>
        /// Number of time disconnected
        /// </summary>
        public int DcCount;

        /// <summary>
        /// Time passed in disconnected state
        /// </summary>
        public long DcDuration;
        /// <summary>
        /// Number of time boons were stripped
        /// </summary>
        public int BoonStrips;
        /// <summary>
        /// Total duration of boons stripped
        /// </summary>
        public double BoonStripsTime;
        /// <summary>
        /// Number of time conditions were cleansed
        /// </summary>
        public int ConditionCleanses;
        /// <summary>
        /// Total duration of conditions cleansed
        /// </summary>
        public double ConditionCleansesTime;
        /// <summary>
        /// Number of time crowd controlled
        /// </summary>
        public int ReceivedCrowdControl;
        /// <summary>
        /// Total crowd control duration received in ms
        /// </summary>
        public double ReceivedCrowdControlDuration;
    }

    /// <summary>
    /// DPS stats
    /// </summary>
    public class JsonDPS
    {
        /// <summary>
        /// Total dps
        /// </summary>
        public int Dps;

        /// <summary>
        /// Total damage
        /// </summary>
        public int Damage;

        /// <summary>
        /// Total condi dps
        /// </summary>
        public int CondiDps;

        /// <summary>
        /// Total condi damage
        /// </summary>
        public int CondiDamage;

        /// <summary>
        /// Total power dps
        /// </summary>
        public int PowerDps;

        /// <summary>
        /// Total power damage
        /// </summary>
        public int PowerDamage;

        /// <summary>
        /// Total breakbar damage
        /// </summary>
        public double BreakbarDamage;

        /// <summary>
        /// Total actor only dps
        /// </summary>
        public int ActorDps;

        /// <summary>
        /// Total actor only damage
        /// </summary>
        public int ActorDamage;

        /// <summary>
        /// Total actor only condi dps
        /// </summary>
        public int ActorCondiDps;

        /// <summary>
        /// Total actor only condi damage
        /// </summary>
        public int ActorCondiDamage;

        /// <summary>
        /// Total actor only power dps
        /// </summary>
        public int ActorPowerDps;

        /// <summary>
        /// Total actor only power damage
        /// </summary>
        public int ActorPowerDamage;

        /// <summary>
        /// Total actor only breakbar damage
        /// </summary>
        public double ActorBreakbarDamage;
    }

    /// <summary>
    /// Gameplay stats
    /// </summary>
    public class JsonGameplayStats
    {
        /// <summary>
        /// Number of damage hit
        /// </summary>
        public int TotalDamageCount;
        /// <summary>
        /// Total damage
        /// </summary>
        public int TotalDmg;

        /// <summary>
        /// Number of direct damage hit
        /// </summary>
        public int DirectDamageCount;
        /// <summary>
        /// Total direct damage
        /// </summary>
        public int DirectDmg;

        /// <summary>
        /// Number of connected direct damage hit
        /// </summary>
        public int ConnectedDirectDamageCount;
        /// <summary>
        /// Total connected direct damage
        /// </summary>
        public int ConnectedDirectDmg;

        /// <summary>
        /// Number of connected damage hit
        /// </summary>
        public int ConnectedDamageCount;
        /// <summary>
        /// Total connected damage
        /// </summary>
        public int ConnectedDmg;

        /// <summary>
        /// Number of critable hit
        /// </summary>
        public int CritableDirectDamageCount;

        /// <summary>
        /// Number of crit
        /// </summary>
        public int CriticalRate;

        /// <summary>
        /// Total critical damage
        /// </summary>
        public int CriticalDmg;

        /// <summary>
        /// Number of hits while flanking
        /// </summary>
        public int FlankingRate;

        /// <summary>
        /// Number of hits while target was moving
        /// </summary>
        public int AgainstMovingRate;

        /// <summary>
        /// Number of glanced hits
        /// </summary>
        public int GlanceRate;

        /// <summary>
        /// Number of missed hits
        /// </summary>
        public int Missed;

        /// <summary>
        /// Number of evaded hits
        /// </summary>
        public int Evaded;

        /// <summary>
        /// Number of blocked hits
        /// </summary>
        public int Blocked;

        /// <summary>
        /// Number of hits that interrupted a skill
        /// </summary>
        public int Interrupts;

        /// <summary>
        /// Number of hits against invulnerable targets
        /// </summary>
        public int Invulned;
        /// <summary>
        /// Number of times killed target
        /// </summary>
        public int Killed;

        /// <summary>
        /// Number of times downed target
        /// </summary>
        public int Downed;
        /// <summary>
        /// Relevant for WvW, defined as the sum of damage done from 90% to down that led to a death \n
        /// </summary>
        public int DownContribution;

        /// <summary>
        /// Number of times a Power based damage skill hits
        /// </summary>
        public int ConnectedPowerCount;
        /// <summary>
        /// Number of times a Power based damage skill hits while source is above 90% hp
        /// </summary>
        public int ConnectedPowerAbove90HPCount;
        /// <summary>
        /// Number of times a Condition based damage skill hits
        /// </summary>
        public int ConnectedConditionCount;
        /// <summary>
        /// Number of times a Condition based damage skill hits while source is above 90% hp
        /// </summary>
        public int ConnectedConditionAbove90HPCount;
        /// <summary>
        /// Number of times a skill hits while target is downed is downed
        /// </summary>
        public int AgainstDownedCount;
        /// <summary>
        /// Damage done against downed target
        /// </summary>
        public int AgainstDownedDamage;
        /// <summary>
        /// Number of time applied a cc.
        /// </summary>
        public int AppliedCrowdControl;
        /// <summary>
        /// Total crowd control duration inflicted in ms
        /// </summary>
        public double AppliedCrowdControlDuration;
    }

    /// <summary>
    /// Gameplay stats
    /// </summary>
    public class JsonGameplayStatsAll : JsonGameplayStats
    {
        /// <summary>
        /// Number of time you interrupted your cast
        /// </summary>
        public int Wasted;

        /// <summary>
        /// Time wasted by interrupting your cast
        /// </summary>
        public double TimeWasted;

        /// <summary>
        /// Number of time you skipped an aftercast
        /// </summary>
        public int Saved;

        /// <summary>
        /// Time saved while skipping aftercast
        /// </summary>
        public double TimeSaved;

        /// <summary>
        /// Distance to the epicenter of the squad
        /// </summary>
        public double StackDist;

        /// <summary>
        /// Distance to the commander of the squad. Only when a player with commander tag is present
        /// </summary>
        public double DistToCom;

        /// <summary>
        /// Average amount of boons
        /// </summary>
        public double AvgBoons;

        /// <summary>
        /// Average amount of boons over active time
        /// </summary>
        public double AvgActiveBoons;

        /// <summary>
        /// Average amount of conditions
        /// </summary>
        public double AvgConditions;

        /// <summary>
        /// Average amount of conditions over active time
        /// </summary>
        public double AvgActiveConditions;

        /// <summary>
        /// Number of time a weapon swap happened
        /// </summary>
        public int SwapCount;

        /// <summary>
        /// % of time in combat spent in animation
        /// </summary>
        public double SkillCastUptime;

        /// <summary>
        /// % of time in combat spent in animation, excluding auto attack skills
        /// </summary>
        public double SkillCastUptimeNoAA;
    }

    /// <summary>
    /// Support stats
    /// </summary>
    public class JsonPlayerSupport
    {

        /// <summary>
        /// Number of time ressurected someone
        /// </summary>
        public long Resurrects;

        /// <summary>
        /// Time passed on ressurecting
        /// </summary>
        public double ResurrectTime;

        /// <summary>
        /// Number of time a condition was cleansed on a squad mate
        /// </summary>
        public long CondiCleanse;

        /// <summary>
        /// Total duration of condition cleansed on a squad mate
        /// </summary>
        public double CondiCleanseTime;

        /// <summary>
        /// Number of time a condition was cleansed from self
        /// </summary>
        public long CondiCleanseSelf;

        /// <summary>
        /// Total duration of condition cleansed from self
        /// </summary>
        public double CondiCleanseTimeSelf;

        /// <summary>
        /// Number of time a boon was stripped
        /// </summary>
        public long BoonStrips;

        /// <summary>
        /// Total duration of boons stripped from self
        /// </summary>
        public double BoonStripsTime;
        /// <summary>
        /// Number of time stun was broken, by self or others
        /// </summary>
        public int StunBreak;
        /// <summary>
        /// Removed stun duration in s.
        /// </summary>
        public double RemovedStunDuration;
    }
}
