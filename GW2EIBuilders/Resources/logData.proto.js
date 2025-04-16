(function($protobuf) {
    "use strict";

    const $Reader = $protobuf.Reader, $util = $protobuf.util;
    
    const $root = $protobuf.roots["default"] || ($protobuf.roots["default"] = {});
    
    export const GW2EIBuilders = $root.GW2EIBuilders = (() => {
    
        const GW2EIBuilders = {};
    
        GW2EIBuilders.Protobuf = (function() {
    
            const Protobuf = {};
    
            Protobuf.EXT = (function() {
    
                const EXT = {};
    
                EXT.HealingStats = (function() {
    
                    const HealingStats = {};
    
                    HealingStats.HealingStats = (function() {
    
                        function HealingStats(p) {
                            this.healingPhases = [];
                            this.playerHealingDetails = [];
                            this.playerHealingCharts = [];
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        HealingStats.prototype.healingPhases = $util.emptyArray;
                        HealingStats.prototype.playerHealingDetails = $util.emptyArray;
                        HealingStats.prototype.playerHealingCharts = $util.emptyArray;
    
                        HealingStats.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        if (!(m.healingPhases && m.healingPhases.length))
                                            m.healingPhases = [];
                                        m.healingPhases.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.decode(r, r.uint32()));
                                        break;
                                    }
                                case 2: {
                                        if (!(m.playerHealingDetails && m.playerHealingDetails.length))
                                            m.playerHealingDetails = [];
                                        m.playerHealingDetails.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.decode(r, r.uint32()));
                                        break;
                                    }
                                case 3: {
                                        if (!(m.playerHealingCharts && m.playerHealingCharts.length))
                                            m.playerHealingCharts = [];
                                        m.playerHealingCharts.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart.decode(r, r.uint32()));
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        HealingStats.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats();
                            if (d.healingPhases) {
                                if (!Array.isArray(d.healingPhases))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats.healingPhases: array expected");
                                m.healingPhases = [];
                                for (var i = 0; i < d.healingPhases.length; ++i) {
                                    if (typeof d.healingPhases[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats.healingPhases: object expected");
                                    m.healingPhases[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.fromObject(d.healingPhases[i]);
                                }
                            }
                            if (d.playerHealingDetails) {
                                if (!Array.isArray(d.playerHealingDetails))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats.playerHealingDetails: array expected");
                                m.playerHealingDetails = [];
                                for (var i = 0; i < d.playerHealingDetails.length; ++i) {
                                    if (typeof d.playerHealingDetails[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats.playerHealingDetails: object expected");
                                    m.playerHealingDetails[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.fromObject(d.playerHealingDetails[i]);
                                }
                            }
                            if (d.playerHealingCharts) {
                                if (!Array.isArray(d.playerHealingCharts))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats.playerHealingCharts: array expected");
                                m.playerHealingCharts = [];
                                for (var i = 0; i < d.playerHealingCharts.length; ++i) {
                                    if (typeof d.playerHealingCharts[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats.playerHealingCharts: object expected");
                                    m.playerHealingCharts[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart.fromObject(d.playerHealingCharts[i]);
                                }
                            }
                            return m;
                        };
    
                        HealingStats.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.arrays || o.defaults) {
                                d.healingPhases = [];
                                d.playerHealingDetails = [];
                                d.playerHealingCharts = [];
                            }
                            if (m.healingPhases && m.healingPhases.length) {
                                d.healingPhases = [];
                                for (var j = 0; j < m.healingPhases.length; ++j) {
                                    d.healingPhases[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.toObject(m.healingPhases[j], o);
                                }
                            }
                            if (m.playerHealingDetails && m.playerHealingDetails.length) {
                                d.playerHealingDetails = [];
                                for (var j = 0; j < m.playerHealingDetails.length; ++j) {
                                    d.playerHealingDetails[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.toObject(m.playerHealingDetails[j], o);
                                }
                            }
                            if (m.playerHealingCharts && m.playerHealingCharts.length) {
                                d.playerHealingCharts = [];
                                for (var j = 0; j < m.playerHealingCharts.length; ++j) {
                                    d.playerHealingCharts[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart.toObject(m.playerHealingCharts[j], o);
                                }
                            }
                            return d;
                        };
    
                        HealingStats.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        HealingStats.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.HealingStats";
                        };
    
                        return HealingStats;
                    })();
    
                    HealingStats.Phase = (function() {
    
                        function Phase(p) {
                            this.outgoingHealingStats = [];
                            this.outgoingHealingStatsTargets = [];
                            this.incomingHealingStats = [];
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        Phase.prototype.outgoingHealingStats = $util.emptyArray;
                        Phase.prototype.outgoingHealingStatsTargets = $util.emptyArray;
                        Phase.prototype.incomingHealingStats = $util.emptyArray;
    
                        Phase.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Phase();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        if (!(m.outgoingHealingStats && m.outgoingHealingStats.length))
                                            m.outgoingHealingStats = [];
                                        m.outgoingHealingStats.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.decode(r, r.uint32()));
                                        break;
                                    }
                                case 2: {
                                        if (!(m.outgoingHealingStatsTargets && m.outgoingHealingStatsTargets.length))
                                            m.outgoingHealingStatsTargets = [];
                                        m.outgoingHealingStatsTargets.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL.decode(r, r.uint32()));
                                        break;
                                    }
                                case 3: {
                                        if (!(m.incomingHealingStats && m.incomingHealingStats.length))
                                            m.incomingHealingStats = [];
                                        m.incomingHealingStats.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.decode(r, r.uint32()));
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        Phase.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Phase)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Phase();
                            if (d.outgoingHealingStats) {
                                if (!Array.isArray(d.outgoingHealingStats))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.outgoingHealingStats: array expected");
                                m.outgoingHealingStats = [];
                                for (var i = 0; i < d.outgoingHealingStats.length; ++i) {
                                    if (typeof d.outgoingHealingStats[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.outgoingHealingStats: object expected");
                                    m.outgoingHealingStats[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.fromObject(d.outgoingHealingStats[i]);
                                }
                            }
                            if (d.outgoingHealingStatsTargets) {
                                if (!Array.isArray(d.outgoingHealingStatsTargets))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.outgoingHealingStatsTargets: array expected");
                                m.outgoingHealingStatsTargets = [];
                                for (var i = 0; i < d.outgoingHealingStatsTargets.length; ++i) {
                                    if (typeof d.outgoingHealingStatsTargets[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.outgoingHealingStatsTargets: object expected");
                                    m.outgoingHealingStatsTargets[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL.fromObject(d.outgoingHealingStatsTargets[i]);
                                }
                            }
                            if (d.incomingHealingStats) {
                                if (!Array.isArray(d.incomingHealingStats))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.incomingHealingStats: array expected");
                                m.incomingHealingStats = [];
                                for (var i = 0; i < d.incomingHealingStats.length; ++i) {
                                    if (typeof d.incomingHealingStats[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Phase.incomingHealingStats: object expected");
                                    m.incomingHealingStats[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.fromObject(d.incomingHealingStats[i]);
                                }
                            }
                            return m;
                        };
    
                        Phase.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.arrays || o.defaults) {
                                d.outgoingHealingStats = [];
                                d.outgoingHealingStatsTargets = [];
                                d.incomingHealingStats = [];
                            }
                            if (m.outgoingHealingStats && m.outgoingHealingStats.length) {
                                d.outgoingHealingStats = [];
                                for (var j = 0; j < m.outgoingHealingStats.length; ++j) {
                                    d.outgoingHealingStats[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.toObject(m.outgoingHealingStats[j], o);
                                }
                            }
                            if (m.outgoingHealingStatsTargets && m.outgoingHealingStatsTargets.length) {
                                d.outgoingHealingStatsTargets = [];
                                for (var j = 0; j < m.outgoingHealingStatsTargets.length; ++j) {
                                    d.outgoingHealingStatsTargets[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL.toObject(m.outgoingHealingStatsTargets[j], o);
                                }
                            }
                            if (m.incomingHealingStats && m.incomingHealingStats.length) {
                                d.incomingHealingStats = [];
                                for (var j = 0; j < m.incomingHealingStats.length; ++j) {
                                    d.incomingHealingStats[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.toObject(m.incomingHealingStats[j], o);
                                }
                            }
                            return d;
                        };
    
                        Phase.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        Phase.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.Phase";
                        };
    
                        return Phase;
                    })();
    
                    HealingStats.PlayerDetails = (function() {
    
                        function PlayerDetails(p) {
                            this.healingDistributions = [];
                            this.healingDistributionsTargets = [];
                            this.incomingHealingDistributions = [];
                            this.minions = [];
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        PlayerDetails.prototype.healingDistributions = $util.emptyArray;
                        PlayerDetails.prototype.healingDistributionsTargets = $util.emptyArray;
                        PlayerDetails.prototype.incomingHealingDistributions = $util.emptyArray;
                        PlayerDetails.prototype.minions = $util.emptyArray;
    
                        PlayerDetails.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        if (!(m.healingDistributions && m.healingDistributions.length))
                                            m.healingDistributions = [];
                                        m.healingDistributions.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.decode(r, r.uint32()));
                                        break;
                                    }
                                case 2: {
                                        if (!(m.healingDistributionsTargets && m.healingDistributionsTargets.length))
                                            m.healingDistributionsTargets = [];
                                        m.healingDistributionsTargets.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL.decode(r, r.uint32()));
                                        break;
                                    }
                                case 3: {
                                        if (!(m.incomingHealingDistributions && m.incomingHealingDistributions.length))
                                            m.incomingHealingDistributions = [];
                                        m.incomingHealingDistributions.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.decode(r, r.uint32()));
                                        break;
                                    }
                                case 4: {
                                        if (!(m.minions && m.minions.length))
                                            m.minions = [];
                                        m.minions.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.decode(r, r.uint32()));
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        PlayerDetails.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails();
                            if (d.healingDistributions) {
                                if (!Array.isArray(d.healingDistributions))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.healingDistributions: array expected");
                                m.healingDistributions = [];
                                for (var i = 0; i < d.healingDistributions.length; ++i) {
                                    if (typeof d.healingDistributions[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.healingDistributions: object expected");
                                    m.healingDistributions[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.fromObject(d.healingDistributions[i]);
                                }
                            }
                            if (d.healingDistributionsTargets) {
                                if (!Array.isArray(d.healingDistributionsTargets))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.healingDistributionsTargets: array expected");
                                m.healingDistributionsTargets = [];
                                for (var i = 0; i < d.healingDistributionsTargets.length; ++i) {
                                    if (typeof d.healingDistributionsTargets[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.healingDistributionsTargets: object expected");
                                    m.healingDistributionsTargets[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL.fromObject(d.healingDistributionsTargets[i]);
                                }
                            }
                            if (d.incomingHealingDistributions) {
                                if (!Array.isArray(d.incomingHealingDistributions))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.incomingHealingDistributions: array expected");
                                m.incomingHealingDistributions = [];
                                for (var i = 0; i < d.incomingHealingDistributions.length; ++i) {
                                    if (typeof d.incomingHealingDistributions[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.incomingHealingDistributions: object expected");
                                    m.incomingHealingDistributions[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.fromObject(d.incomingHealingDistributions[i]);
                                }
                            }
                            if (d.minions) {
                                if (!Array.isArray(d.minions))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.minions: array expected");
                                m.minions = [];
                                for (var i = 0; i < d.minions.length; ++i) {
                                    if (typeof d.minions[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.minions: object expected");
                                    m.minions[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.fromObject(d.minions[i]);
                                }
                            }
                            return m;
                        };
    
                        PlayerDetails.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.arrays || o.defaults) {
                                d.healingDistributions = [];
                                d.healingDistributionsTargets = [];
                                d.incomingHealingDistributions = [];
                                d.minions = [];
                            }
                            if (m.healingDistributions && m.healingDistributions.length) {
                                d.healingDistributions = [];
                                for (var j = 0; j < m.healingDistributions.length; ++j) {
                                    d.healingDistributions[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.toObject(m.healingDistributions[j], o);
                                }
                            }
                            if (m.healingDistributionsTargets && m.healingDistributionsTargets.length) {
                                d.healingDistributionsTargets = [];
                                for (var j = 0; j < m.healingDistributionsTargets.length; ++j) {
                                    d.healingDistributionsTargets[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL.toObject(m.healingDistributionsTargets[j], o);
                                }
                            }
                            if (m.incomingHealingDistributions && m.incomingHealingDistributions.length) {
                                d.incomingHealingDistributions = [];
                                for (var j = 0; j < m.incomingHealingDistributions.length; ++j) {
                                    d.incomingHealingDistributions[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.toObject(m.incomingHealingDistributions[j], o);
                                }
                            }
                            if (m.minions && m.minions.length) {
                                d.minions = [];
                                for (var j = 0; j < m.minions.length; ++j) {
                                    d.minions[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.toObject(m.minions[j], o);
                                }
                            }
                            return d;
                        };
    
                        PlayerDetails.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        PlayerDetails.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails";
                        };
    
                        PlayerDetails.HealingDistribution = (function() {
    
                            function HealingDistribution(p) {
                                this.distribution = [];
                                if (p)
                                    for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                        if (p[ks[i]] != null)
                                            this[ks[i]] = p[ks[i]];
                            }
    
                            HealingDistribution.prototype.contributedHealing = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistribution.prototype.contributedDownedHealing = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistribution.prototype.totalHealing = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistribution.prototype.totalCasting = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistribution.prototype.distribution = $util.emptyArray;
    
                            HealingDistribution.decode = function decode(r, l, e) {
                                if (!(r instanceof $Reader))
                                    r = $Reader.create(r);
                                var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution();
                                while (r.pos < c) {
                                    var t = r.uint32();
                                    if (t === e)
                                        break;
                                    switch (t >>> 3) {
                                    case 1: {
                                            m.contributedHealing = r.int64();
                                            break;
                                        }
                                    case 2: {
                                            m.contributedDownedHealing = r.int64();
                                            break;
                                        }
                                    case 3: {
                                            m.totalHealing = r.int64();
                                            break;
                                        }
                                    case 4: {
                                            m.totalCasting = r.int64();
                                            break;
                                        }
                                    case 5: {
                                            if (!(m.distribution && m.distribution.length))
                                                m.distribution = [];
                                            m.distribution.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem.decode(r, r.uint32()));
                                            break;
                                        }
                                    default:
                                        r.skipType(t & 7);
                                        break;
                                    }
                                }
                                return m;
                            };
    
                            HealingDistribution.fromObject = function fromObject(d) {
                                if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution)
                                    return d;
                                var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution();
                                if (d.contributedHealing != null) {
                                    if ($util.Long)
                                        (m.contributedHealing = $util.Long.fromValue(d.contributedHealing)).unsigned = false;
                                    else if (typeof d.contributedHealing === "string")
                                        m.contributedHealing = parseInt(d.contributedHealing, 10);
                                    else if (typeof d.contributedHealing === "number")
                                        m.contributedHealing = d.contributedHealing;
                                    else if (typeof d.contributedHealing === "object")
                                        m.contributedHealing = new $util.LongBits(d.contributedHealing.low >>> 0, d.contributedHealing.high >>> 0).toNumber();
                                }
                                if (d.contributedDownedHealing != null) {
                                    if ($util.Long)
                                        (m.contributedDownedHealing = $util.Long.fromValue(d.contributedDownedHealing)).unsigned = false;
                                    else if (typeof d.contributedDownedHealing === "string")
                                        m.contributedDownedHealing = parseInt(d.contributedDownedHealing, 10);
                                    else if (typeof d.contributedDownedHealing === "number")
                                        m.contributedDownedHealing = d.contributedDownedHealing;
                                    else if (typeof d.contributedDownedHealing === "object")
                                        m.contributedDownedHealing = new $util.LongBits(d.contributedDownedHealing.low >>> 0, d.contributedDownedHealing.high >>> 0).toNumber();
                                }
                                if (d.totalHealing != null) {
                                    if ($util.Long)
                                        (m.totalHealing = $util.Long.fromValue(d.totalHealing)).unsigned = false;
                                    else if (typeof d.totalHealing === "string")
                                        m.totalHealing = parseInt(d.totalHealing, 10);
                                    else if (typeof d.totalHealing === "number")
                                        m.totalHealing = d.totalHealing;
                                    else if (typeof d.totalHealing === "object")
                                        m.totalHealing = new $util.LongBits(d.totalHealing.low >>> 0, d.totalHealing.high >>> 0).toNumber();
                                }
                                if (d.totalCasting != null) {
                                    if ($util.Long)
                                        (m.totalCasting = $util.Long.fromValue(d.totalCasting)).unsigned = false;
                                    else if (typeof d.totalCasting === "string")
                                        m.totalCasting = parseInt(d.totalCasting, 10);
                                    else if (typeof d.totalCasting === "number")
                                        m.totalCasting = d.totalCasting;
                                    else if (typeof d.totalCasting === "object")
                                        m.totalCasting = new $util.LongBits(d.totalCasting.low >>> 0, d.totalCasting.high >>> 0).toNumber();
                                }
                                if (d.distribution) {
                                    if (!Array.isArray(d.distribution))
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.distribution: array expected");
                                    m.distribution = [];
                                    for (var i = 0; i < d.distribution.length; ++i) {
                                        if (typeof d.distribution[i] !== "object")
                                            throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.distribution: object expected");
                                        m.distribution[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem.fromObject(d.distribution[i]);
                                    }
                                }
                                return m;
                            };
    
                            HealingDistribution.toObject = function toObject(m, o) {
                                if (!o)
                                    o = {};
                                var d = {};
                                if (o.arrays || o.defaults) {
                                    d.distribution = [];
                                }
                                if (o.defaults) {
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.contributedHealing = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.contributedHealing = o.longs === String ? "0" : 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.contributedDownedHealing = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.contributedDownedHealing = o.longs === String ? "0" : 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.totalHealing = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.totalHealing = o.longs === String ? "0" : 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.totalCasting = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.totalCasting = o.longs === String ? "0" : 0;
                                }
                                if (m.contributedHealing != null && m.hasOwnProperty("contributedHealing")) {
                                    if (typeof m.contributedHealing === "number")
                                        d.contributedHealing = o.longs === String ? String(m.contributedHealing) : m.contributedHealing;
                                    else
                                        d.contributedHealing = o.longs === String ? $util.Long.prototype.toString.call(m.contributedHealing) : o.longs === Number ? new $util.LongBits(m.contributedHealing.low >>> 0, m.contributedHealing.high >>> 0).toNumber() : m.contributedHealing;
                                }
                                if (m.contributedDownedHealing != null && m.hasOwnProperty("contributedDownedHealing")) {
                                    if (typeof m.contributedDownedHealing === "number")
                                        d.contributedDownedHealing = o.longs === String ? String(m.contributedDownedHealing) : m.contributedDownedHealing;
                                    else
                                        d.contributedDownedHealing = o.longs === String ? $util.Long.prototype.toString.call(m.contributedDownedHealing) : o.longs === Number ? new $util.LongBits(m.contributedDownedHealing.low >>> 0, m.contributedDownedHealing.high >>> 0).toNumber() : m.contributedDownedHealing;
                                }
                                if (m.totalHealing != null && m.hasOwnProperty("totalHealing")) {
                                    if (typeof m.totalHealing === "number")
                                        d.totalHealing = o.longs === String ? String(m.totalHealing) : m.totalHealing;
                                    else
                                        d.totalHealing = o.longs === String ? $util.Long.prototype.toString.call(m.totalHealing) : o.longs === Number ? new $util.LongBits(m.totalHealing.low >>> 0, m.totalHealing.high >>> 0).toNumber() : m.totalHealing;
                                }
                                if (m.totalCasting != null && m.hasOwnProperty("totalCasting")) {
                                    if (typeof m.totalCasting === "number")
                                        d.totalCasting = o.longs === String ? String(m.totalCasting) : m.totalCasting;
                                    else
                                        d.totalCasting = o.longs === String ? $util.Long.prototype.toString.call(m.totalCasting) : o.longs === Number ? new $util.LongBits(m.totalCasting.low >>> 0, m.totalCasting.high >>> 0).toNumber() : m.totalCasting;
                                }
                                if (m.distribution && m.distribution.length) {
                                    d.distribution = [];
                                    for (var j = 0; j < m.distribution.length; ++j) {
                                        d.distribution[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem.toObject(m.distribution[j], o);
                                    }
                                }
                                return d;
                            };
    
                            HealingDistribution.prototype.toJSON = function toJSON() {
                                return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                            };
    
                            HealingDistribution.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                                if (typeUrlPrefix === undefined) {
                                    typeUrlPrefix = "type.googleapis.com";
                                }
                                return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution";
                            };
    
                            return HealingDistribution;
                        })();
    
                        PlayerDetails.HealingDistributionItem = (function() {
    
                            function HealingDistributionItem(p) {
                                if (p)
                                    for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                        if (p[ks[i]] != null)
                                            this[ks[i]] = p[ks[i]];
                            }
    
                            HealingDistributionItem.prototype.__isIndirect = false;
                            HealingDistributionItem.prototype.__skillId = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistributionItem.prototype.__totalHealing = 0;
                            HealingDistributionItem.prototype.__minHealing = 0;
                            HealingDistributionItem.prototype.__maxHealing = 0;
                            HealingDistributionItem.prototype.__numberOfCasts = 0;
                            HealingDistributionItem.prototype.__timeWasted = 0;
                            HealingDistributionItem.prototype.__timeSaved = 0;
                            HealingDistributionItem.prototype.__hits = 0;
                            HealingDistributionItem.prototype.__timeSpentCasting = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistributionItem.prototype.__totaldownedhealing = 0;
                            HealingDistributionItem.prototype.__minTimeSpentCasting = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistributionItem.prototype.__maxTimeSpentCasting = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistributionItem.prototype.__timeSpentCastingNoInterrupt = $util.Long ? $util.Long.fromBits(0,0,false) : 0;
                            HealingDistributionItem.prototype.__numberOfCastNoInterrupt = 0;
    
                            HealingDistributionItem.decode = function decode(r, l, e) {
                                if (!(r instanceof $Reader))
                                    r = $Reader.create(r);
                                var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem();
                                while (r.pos < c) {
                                    var t = r.uint32();
                                    if (t === e)
                                        break;
                                    switch (t >>> 3) {
                                    case 1: {
                                            m.__isIndirect = r.bool();
                                            break;
                                        }
                                    case 2: {
                                            m.__skillId = r.int64();
                                            break;
                                        }
                                    case 3: {
                                            m.__totalHealing = r.int32();
                                            break;
                                        }
                                    case 4: {
                                            m.__minHealing = r.int32();
                                            break;
                                        }
                                    case 5: {
                                            m.__maxHealing = r.int32();
                                            break;
                                        }
                                    case 6: {
                                            m.__numberOfCasts = r.int32();
                                            break;
                                        }
                                    case 7: {
                                            m.__timeWasted = r.float();
                                            break;
                                        }
                                    case 8: {
                                            m.__timeSaved = r.float();
                                            break;
                                        }
                                    case 9: {
                                            m.__hits = r.int32();
                                            break;
                                        }
                                    case 10: {
                                            m.__timeSpentCasting = r.int64();
                                            break;
                                        }
                                    case 11: {
                                            m.__totaldownedhealing = r.int32();
                                            break;
                                        }
                                    case 12: {
                                            m.__minTimeSpentCasting = r.int64();
                                            break;
                                        }
                                    case 13: {
                                            m.__maxTimeSpentCasting = r.int64();
                                            break;
                                        }
                                    case 14: {
                                            m.__timeSpentCastingNoInterrupt = r.int64();
                                            break;
                                        }
                                    case 15: {
                                            m.__numberOfCastNoInterrupt = r.int32();
                                            break;
                                        }
                                    default:
                                        r.skipType(t & 7);
                                        break;
                                    }
                                }
                                return m;
                            };
    
                            HealingDistributionItem.fromObject = function fromObject(d) {
                                if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem)
                                    return d;
                                var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem();
                                if (d.__isIndirect != null) {
                                    m.__isIndirect = Boolean(d.__isIndirect);
                                }
                                if (d.__skillId != null) {
                                    if ($util.Long)
                                        (m.__skillId = $util.Long.fromValue(d.__skillId)).unsigned = false;
                                    else if (typeof d.__skillId === "string")
                                        m.__skillId = parseInt(d.__skillId, 10);
                                    else if (typeof d.__skillId === "number")
                                        m.__skillId = d.__skillId;
                                    else if (typeof d.__skillId === "object")
                                        m.__skillId = new $util.LongBits(d.__skillId.low >>> 0, d.__skillId.high >>> 0).toNumber();
                                }
                                if (d.__totalHealing != null) {
                                    m.__totalHealing = d.__totalHealing | 0;
                                }
                                if (d.__minHealing != null) {
                                    m.__minHealing = d.__minHealing | 0;
                                }
                                if (d.__maxHealing != null) {
                                    m.__maxHealing = d.__maxHealing | 0;
                                }
                                if (d.__numberOfCasts != null) {
                                    m.__numberOfCasts = d.__numberOfCasts | 0;
                                }
                                if (d.__timeWasted != null) {
                                    m.__timeWasted = Number(d.__timeWasted);
                                }
                                if (d.__timeSaved != null) {
                                    m.__timeSaved = Number(d.__timeSaved);
                                }
                                if (d.__hits != null) {
                                    m.__hits = d.__hits | 0;
                                }
                                if (d.__timeSpentCasting != null) {
                                    if ($util.Long)
                                        (m.__timeSpentCasting = $util.Long.fromValue(d.__timeSpentCasting)).unsigned = false;
                                    else if (typeof d.__timeSpentCasting === "string")
                                        m.__timeSpentCasting = parseInt(d.__timeSpentCasting, 10);
                                    else if (typeof d.__timeSpentCasting === "number")
                                        m.__timeSpentCasting = d.__timeSpentCasting;
                                    else if (typeof d.__timeSpentCasting === "object")
                                        m.__timeSpentCasting = new $util.LongBits(d.__timeSpentCasting.low >>> 0, d.__timeSpentCasting.high >>> 0).toNumber();
                                }
                                if (d.__totaldownedhealing != null) {
                                    m.__totaldownedhealing = d.__totaldownedhealing | 0;
                                }
                                if (d.__minTimeSpentCasting != null) {
                                    if ($util.Long)
                                        (m.__minTimeSpentCasting = $util.Long.fromValue(d.__minTimeSpentCasting)).unsigned = false;
                                    else if (typeof d.__minTimeSpentCasting === "string")
                                        m.__minTimeSpentCasting = parseInt(d.__minTimeSpentCasting, 10);
                                    else if (typeof d.__minTimeSpentCasting === "number")
                                        m.__minTimeSpentCasting = d.__minTimeSpentCasting;
                                    else if (typeof d.__minTimeSpentCasting === "object")
                                        m.__minTimeSpentCasting = new $util.LongBits(d.__minTimeSpentCasting.low >>> 0, d.__minTimeSpentCasting.high >>> 0).toNumber();
                                }
                                if (d.__maxTimeSpentCasting != null) {
                                    if ($util.Long)
                                        (m.__maxTimeSpentCasting = $util.Long.fromValue(d.__maxTimeSpentCasting)).unsigned = false;
                                    else if (typeof d.__maxTimeSpentCasting === "string")
                                        m.__maxTimeSpentCasting = parseInt(d.__maxTimeSpentCasting, 10);
                                    else if (typeof d.__maxTimeSpentCasting === "number")
                                        m.__maxTimeSpentCasting = d.__maxTimeSpentCasting;
                                    else if (typeof d.__maxTimeSpentCasting === "object")
                                        m.__maxTimeSpentCasting = new $util.LongBits(d.__maxTimeSpentCasting.low >>> 0, d.__maxTimeSpentCasting.high >>> 0).toNumber();
                                }
                                if (d.__timeSpentCastingNoInterrupt != null) {
                                    if ($util.Long)
                                        (m.__timeSpentCastingNoInterrupt = $util.Long.fromValue(d.__timeSpentCastingNoInterrupt)).unsigned = false;
                                    else if (typeof d.__timeSpentCastingNoInterrupt === "string")
                                        m.__timeSpentCastingNoInterrupt = parseInt(d.__timeSpentCastingNoInterrupt, 10);
                                    else if (typeof d.__timeSpentCastingNoInterrupt === "number")
                                        m.__timeSpentCastingNoInterrupt = d.__timeSpentCastingNoInterrupt;
                                    else if (typeof d.__timeSpentCastingNoInterrupt === "object")
                                        m.__timeSpentCastingNoInterrupt = new $util.LongBits(d.__timeSpentCastingNoInterrupt.low >>> 0, d.__timeSpentCastingNoInterrupt.high >>> 0).toNumber();
                                }
                                if (d.__numberOfCastNoInterrupt != null) {
                                    m.__numberOfCastNoInterrupt = d.__numberOfCastNoInterrupt | 0;
                                }
                                return m;
                            };
    
                            HealingDistributionItem.toObject = function toObject(m, o) {
                                if (!o)
                                    o = {};
                                var d = {};
                                if (o.defaults) {
                                    d.__isIndirect = false;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.__skillId = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.__skillId = o.longs === String ? "0" : 0;
                                    d.__totalHealing = 0;
                                    d.__minHealing = 0;
                                    d.__maxHealing = 0;
                                    d.__numberOfCasts = 0;
                                    d.__timeWasted = 0;
                                    d.__timeSaved = 0;
                                    d.__hits = 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.__timeSpentCasting = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.__timeSpentCasting = o.longs === String ? "0" : 0;
                                    d.__totaldownedhealing = 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.__minTimeSpentCasting = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.__minTimeSpentCasting = o.longs === String ? "0" : 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.__maxTimeSpentCasting = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.__maxTimeSpentCasting = o.longs === String ? "0" : 0;
                                    if ($util.Long) {
                                        var n = new $util.Long(0, 0, false);
                                        d.__timeSpentCastingNoInterrupt = o.longs === String ? n.toString() : o.longs === Number ? n.toNumber() : n;
                                    } else
                                        d.__timeSpentCastingNoInterrupt = o.longs === String ? "0" : 0;
                                    d.__numberOfCastNoInterrupt = 0;
                                }
                                if (m.__isIndirect != null && m.hasOwnProperty("__isIndirect")) {
                                    d.__isIndirect = m.__isIndirect;
                                }
                                if (m.__skillId != null && m.hasOwnProperty("__skillId")) {
                                    if (typeof m.__skillId === "number")
                                        d.__skillId = o.longs === String ? String(m.__skillId) : m.__skillId;
                                    else
                                        d.__skillId = o.longs === String ? $util.Long.prototype.toString.call(m.__skillId) : o.longs === Number ? new $util.LongBits(m.__skillId.low >>> 0, m.__skillId.high >>> 0).toNumber() : m.__skillId;
                                }
                                if (m.__totalHealing != null && m.hasOwnProperty("__totalHealing")) {
                                    d.__totalHealing = m.__totalHealing;
                                }
                                if (m.__minHealing != null && m.hasOwnProperty("__minHealing")) {
                                    d.__minHealing = m.__minHealing;
                                }
                                if (m.__maxHealing != null && m.hasOwnProperty("__maxHealing")) {
                                    d.__maxHealing = m.__maxHealing;
                                }
                                if (m.__numberOfCasts != null && m.hasOwnProperty("__numberOfCasts")) {
                                    d.__numberOfCasts = m.__numberOfCasts;
                                }
                                if (m.__timeWasted != null && m.hasOwnProperty("__timeWasted")) {
                                    d.__timeWasted = o.json && !isFinite(m.__timeWasted) ? String(m.__timeWasted) : m.__timeWasted;
                                }
                                if (m.__timeSaved != null && m.hasOwnProperty("__timeSaved")) {
                                    d.__timeSaved = o.json && !isFinite(m.__timeSaved) ? String(m.__timeSaved) : m.__timeSaved;
                                }
                                if (m.__hits != null && m.hasOwnProperty("__hits")) {
                                    d.__hits = m.__hits;
                                }
                                if (m.__timeSpentCasting != null && m.hasOwnProperty("__timeSpentCasting")) {
                                    if (typeof m.__timeSpentCasting === "number")
                                        d.__timeSpentCasting = o.longs === String ? String(m.__timeSpentCasting) : m.__timeSpentCasting;
                                    else
                                        d.__timeSpentCasting = o.longs === String ? $util.Long.prototype.toString.call(m.__timeSpentCasting) : o.longs === Number ? new $util.LongBits(m.__timeSpentCasting.low >>> 0, m.__timeSpentCasting.high >>> 0).toNumber() : m.__timeSpentCasting;
                                }
                                if (m.__totaldownedhealing != null && m.hasOwnProperty("__totaldownedhealing")) {
                                    d.__totaldownedhealing = m.__totaldownedhealing;
                                }
                                if (m.__minTimeSpentCasting != null && m.hasOwnProperty("__minTimeSpentCasting")) {
                                    if (typeof m.__minTimeSpentCasting === "number")
                                        d.__minTimeSpentCasting = o.longs === String ? String(m.__minTimeSpentCasting) : m.__minTimeSpentCasting;
                                    else
                                        d.__minTimeSpentCasting = o.longs === String ? $util.Long.prototype.toString.call(m.__minTimeSpentCasting) : o.longs === Number ? new $util.LongBits(m.__minTimeSpentCasting.low >>> 0, m.__minTimeSpentCasting.high >>> 0).toNumber() : m.__minTimeSpentCasting;
                                }
                                if (m.__maxTimeSpentCasting != null && m.hasOwnProperty("__maxTimeSpentCasting")) {
                                    if (typeof m.__maxTimeSpentCasting === "number")
                                        d.__maxTimeSpentCasting = o.longs === String ? String(m.__maxTimeSpentCasting) : m.__maxTimeSpentCasting;
                                    else
                                        d.__maxTimeSpentCasting = o.longs === String ? $util.Long.prototype.toString.call(m.__maxTimeSpentCasting) : o.longs === Number ? new $util.LongBits(m.__maxTimeSpentCasting.low >>> 0, m.__maxTimeSpentCasting.high >>> 0).toNumber() : m.__maxTimeSpentCasting;
                                }
                                if (m.__timeSpentCastingNoInterrupt != null && m.hasOwnProperty("__timeSpentCastingNoInterrupt")) {
                                    if (typeof m.__timeSpentCastingNoInterrupt === "number")
                                        d.__timeSpentCastingNoInterrupt = o.longs === String ? String(m.__timeSpentCastingNoInterrupt) : m.__timeSpentCastingNoInterrupt;
                                    else
                                        d.__timeSpentCastingNoInterrupt = o.longs === String ? $util.Long.prototype.toString.call(m.__timeSpentCastingNoInterrupt) : o.longs === Number ? new $util.LongBits(m.__timeSpentCastingNoInterrupt.low >>> 0, m.__timeSpentCastingNoInterrupt.high >>> 0).toNumber() : m.__timeSpentCastingNoInterrupt;
                                }
                                if (m.__numberOfCastNoInterrupt != null && m.hasOwnProperty("__numberOfCastNoInterrupt")) {
                                    d.__numberOfCastNoInterrupt = m.__numberOfCastNoInterrupt;
                                }
                                return d;
                            };
    
                            HealingDistributionItem.prototype.toJSON = function toJSON() {
                                return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                            };
    
                            HealingDistributionItem.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                                if (typeUrlPrefix === undefined) {
                                    typeUrlPrefix = "type.googleapis.com";
                                }
                                return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionItem";
                            };
    
                            return HealingDistributionItem;
                        })();
    
                        PlayerDetails.HealingDistributionL = (function() {
    
                            function HealingDistributionL(p) {
                                this.__inner = [];
                                if (p)
                                    for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                        if (p[ks[i]] != null)
                                            this[ks[i]] = p[ks[i]];
                            }
    
                            HealingDistributionL.prototype.__inner = $util.emptyArray;
    
                            HealingDistributionL.decode = function decode(r, l, e) {
                                if (!(r instanceof $Reader))
                                    r = $Reader.create(r);
                                var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL();
                                while (r.pos < c) {
                                    var t = r.uint32();
                                    if (t === e)
                                        break;
                                    switch (t >>> 3) {
                                    case 1: {
                                            if (!(m.__inner && m.__inner.length))
                                                m.__inner = [];
                                            m.__inner.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.decode(r, r.uint32()));
                                            break;
                                        }
                                    default:
                                        r.skipType(t & 7);
                                        break;
                                    }
                                }
                                return m;
                            };
    
                            HealingDistributionL.fromObject = function fromObject(d) {
                                if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL)
                                    return d;
                                var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL();
                                if (d.__inner) {
                                    if (!Array.isArray(d.__inner))
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL.__inner: array expected");
                                    m.__inner = [];
                                    for (var i = 0; i < d.__inner.length; ++i) {
                                        if (typeof d.__inner[i] !== "object")
                                            throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL.__inner: object expected");
                                        m.__inner[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.fromObject(d.__inner[i]);
                                    }
                                }
                                return m;
                            };
    
                            HealingDistributionL.toObject = function toObject(m, o) {
                                if (!o)
                                    o = {};
                                var d = {};
                                if (o.arrays || o.defaults) {
                                    d.__inner = [];
                                }
                                if (m.__inner && m.__inner.length) {
                                    d.__inner = [];
                                    for (var j = 0; j < m.__inner.length; ++j) {
                                        d.__inner[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistribution.toObject(m.__inner[j], o);
                                    }
                                }
                                return d;
                            };
    
                            HealingDistributionL.prototype.toJSON = function toJSON() {
                                return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                            };
    
                            HealingDistributionL.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                                if (typeUrlPrefix === undefined) {
                                    typeUrlPrefix = "type.googleapis.com";
                                }
                                return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDetails.HealingDistributionL";
                            };
    
                            return HealingDistributionL;
                        })();
    
                        return PlayerDetails;
                    })();
    
                    HealingStats.PlayerChart = (function() {
    
                        function PlayerChart(p) {
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        PlayerChart.prototype.healing = null;
                        PlayerChart.prototype.healingPowerHealing = null;
                        PlayerChart.prototype.conversionBasedHealing = null;
    
                        PlayerChart.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        m.healing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.decode(r, r.uint32());
                                        break;
                                    }
                                case 2: {
                                        m.healingPowerHealing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.decode(r, r.uint32());
                                        break;
                                    }
                                case 3: {
                                        m.conversionBasedHealing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.decode(r, r.uint32());
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        PlayerChart.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart();
                            if (d.healing != null) {
                                if (typeof d.healing !== "object")
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart.healing: object expected");
                                m.healing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.fromObject(d.healing);
                            }
                            if (d.healingPowerHealing != null) {
                                if (typeof d.healingPowerHealing !== "object")
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart.healingPowerHealing: object expected");
                                m.healingPowerHealing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.fromObject(d.healingPowerHealing);
                            }
                            if (d.conversionBasedHealing != null) {
                                if (typeof d.conversionBasedHealing !== "object")
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart.conversionBasedHealing: object expected");
                                m.conversionBasedHealing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.fromObject(d.conversionBasedHealing);
                            }
                            return m;
                        };
    
                        PlayerChart.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.defaults) {
                                d.healing = null;
                                d.healingPowerHealing = null;
                                d.conversionBasedHealing = null;
                            }
                            if (m.healing != null && m.hasOwnProperty("healing")) {
                                d.healing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.toObject(m.healing, o);
                            }
                            if (m.healingPowerHealing != null && m.hasOwnProperty("healingPowerHealing")) {
                                d.healingPowerHealing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.toObject(m.healingPowerHealing, o);
                            }
                            if (m.conversionBasedHealing != null && m.hasOwnProperty("conversionBasedHealing")) {
                                d.conversionBasedHealing = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.toObject(m.conversionBasedHealing, o);
                            }
                            return d;
                        };
    
                        PlayerChart.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        PlayerChart.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerChart";
                        };
    
                        return PlayerChart;
                    })();
    
                    HealingStats.PlayerDamageChart_int = (function() {
    
                        function PlayerDamageChart_int(p) {
                            this.targets = [];
                            this.total = [];
                            this.taken = [];
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        PlayerDamageChart_int.prototype.targets = $util.emptyArray;
                        PlayerDamageChart_int.prototype.total = $util.emptyArray;
                        PlayerDamageChart_int.prototype.taken = $util.emptyArray;
    
                        PlayerDamageChart_int.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        if (!(m.targets && m.targets.length))
                                            m.targets = [];
                                        m.targets.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.decode(r, r.uint32()));
                                        break;
                                    }
                                case 2: {
                                        if (!(m.total && m.total.length))
                                            m.total = [];
                                        if ((t & 7) === 2) {
                                            var c2 = r.uint32() + r.pos;
                                            while (r.pos < c2)
                                                m.total.push(r.int32());
                                        } else
                                            m.total.push(r.int32());
                                        break;
                                    }
                                case 3: {
                                        if (!(m.taken && m.taken.length))
                                            m.taken = [];
                                        if ((t & 7) === 2) {
                                            var c2 = r.uint32() + r.pos;
                                            while (r.pos < c2)
                                                m.taken.push(r.int32());
                                        } else
                                            m.taken.push(r.int32());
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        PlayerDamageChart_int.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int();
                            if (d.targets) {
                                if (!Array.isArray(d.targets))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.targets: array expected");
                                m.targets = [];
                                for (var i = 0; i < d.targets.length; ++i) {
                                    if (typeof d.targets[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.targets: object expected");
                                    m.targets[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.fromObject(d.targets[i]);
                                }
                            }
                            if (d.total) {
                                if (!Array.isArray(d.total))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.total: array expected");
                                m.total = [];
                                for (var i = 0; i < d.total.length; ++i) {
                                    m.total[i] = d.total[i] | 0;
                                }
                            }
                            if (d.taken) {
                                if (!Array.isArray(d.taken))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int.taken: array expected");
                                m.taken = [];
                                for (var i = 0; i < d.taken.length; ++i) {
                                    m.taken[i] = d.taken[i] | 0;
                                }
                            }
                            return m;
                        };
    
                        PlayerDamageChart_int.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.arrays || o.defaults) {
                                d.targets = [];
                                d.total = [];
                                d.taken = [];
                            }
                            if (m.targets && m.targets.length) {
                                d.targets = [];
                                for (var j = 0; j < m.targets.length; ++j) {
                                    d.targets[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.toObject(m.targets[j], o);
                                }
                            }
                            if (m.total && m.total.length) {
                                d.total = [];
                                for (var j = 0; j < m.total.length; ++j) {
                                    d.total[j] = m.total[j];
                                }
                            }
                            if (m.taken && m.taken.length) {
                                d.taken = [];
                                for (var j = 0; j < m.taken.length; ++j) {
                                    d.taken[j] = m.taken[j];
                                }
                            }
                            return d;
                        };
    
                        PlayerDamageChart_int.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        PlayerDamageChart_int.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.PlayerDamageChart_int";
                        };
    
                        return PlayerDamageChart_int;
                    })();
    
                    HealingStats.Int32L = (function() {
    
                        function Int32L(p) {
                            this.__inner = [];
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        Int32L.prototype.__inner = $util.emptyArray;
    
                        Int32L.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        if (!(m.__inner && m.__inner.length))
                                            m.__inner = [];
                                        if ((t & 7) === 2) {
                                            var c2 = r.uint32() + r.pos;
                                            while (r.pos < c2)
                                                m.__inner.push(r.int32());
                                        } else
                                            m.__inner.push(r.int32());
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        Int32L.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L();
                            if (d.__inner) {
                                if (!Array.isArray(d.__inner))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.__inner: array expected");
                                m.__inner = [];
                                for (var i = 0; i < d.__inner.length; ++i) {
                                    m.__inner[i] = d.__inner[i] | 0;
                                }
                            }
                            return m;
                        };
    
                        Int32L.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.arrays || o.defaults) {
                                d.__inner = [];
                            }
                            if (m.__inner && m.__inner.length) {
                                d.__inner = [];
                                for (var j = 0; j < m.__inner.length; ++j) {
                                    d.__inner[j] = m.__inner[j];
                                }
                            }
                            return d;
                        };
    
                        Int32L.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        Int32L.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L";
                        };
    
                        return Int32L;
                    })();
    
                    HealingStats.Int32LL = (function() {
    
                        function Int32LL(p) {
                            this.__inner = [];
                            if (p)
                                for (var ks = Object.keys(p), i = 0; i < ks.length; ++i)
                                    if (p[ks[i]] != null)
                                        this[ks[i]] = p[ks[i]];
                        }
    
                        Int32LL.prototype.__inner = $util.emptyArray;
    
                        Int32LL.decode = function decode(r, l, e) {
                            if (!(r instanceof $Reader))
                                r = $Reader.create(r);
                            var c = l === undefined ? r.len : r.pos + l, m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL();
                            while (r.pos < c) {
                                var t = r.uint32();
                                if (t === e)
                                    break;
                                switch (t >>> 3) {
                                case 1: {
                                        if (!(m.__inner && m.__inner.length))
                                            m.__inner = [];
                                        m.__inner.push($root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.decode(r, r.uint32()));
                                        break;
                                    }
                                default:
                                    r.skipType(t & 7);
                                    break;
                                }
                            }
                            return m;
                        };
    
                        Int32LL.fromObject = function fromObject(d) {
                            if (d instanceof $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL)
                                return d;
                            var m = new $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL();
                            if (d.__inner) {
                                if (!Array.isArray(d.__inner))
                                    throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL.__inner: array expected");
                                m.__inner = [];
                                for (var i = 0; i < d.__inner.length; ++i) {
                                    if (typeof d.__inner[i] !== "object")
                                        throw TypeError(".GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL.__inner: object expected");
                                    m.__inner[i] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.fromObject(d.__inner[i]);
                                }
                            }
                            return m;
                        };
    
                        Int32LL.toObject = function toObject(m, o) {
                            if (!o)
                                o = {};
                            var d = {};
                            if (o.arrays || o.defaults) {
                                d.__inner = [];
                            }
                            if (m.__inner && m.__inner.length) {
                                d.__inner = [];
                                for (var j = 0; j < m.__inner.length; ++j) {
                                    d.__inner[j] = $root.GW2EIBuilders.Protobuf.EXT.HealingStats.Int32L.toObject(m.__inner[j], o);
                                }
                            }
                            return d;
                        };
    
                        Int32LL.prototype.toJSON = function toJSON() {
                            return this.constructor.toObject(this, $protobuf.util.toJSONOptions);
                        };
    
                        Int32LL.getTypeUrl = function getTypeUrl(typeUrlPrefix) {
                            if (typeUrlPrefix === undefined) {
                                typeUrlPrefix = "type.googleapis.com";
                            }
                            return typeUrlPrefix + "/GW2EIBuilders.Protobuf.EXT.HealingStats.Int32LL";
                        };
    
                        return Int32LL;
                    })();
    
                    return HealingStats;
                })();
    
                return EXT;
            })();
    
            return Protobuf;
        })();
    
        return GW2EIBuilders;
    })();

    return $root;
})(protobuf);
