﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    public class BuffsGraphModel
    {
        public readonly Buff Buff;

        public IReadOnlyList<Segment> BuffChart => _buffChart;
        private List<Segment> _buffChart;

        // Constructor
        internal BuffsGraphModel(Buff buff)
        {
            Buff = buff;
            _buffChart = new();
        }
        internal BuffsGraphModel(Buff buff, List<Segment> buffChartWithSource)
        {
            Buff = buff;
            _buffChart = buffChartWithSource;
            FuseSegments();
        }

        public Segment GetBuffStatus(long time)
        {
            if (BuffChart.Count == 0)
            {
                return new Segment(long.MinValue, long.MaxValue, 0);
            }
            
            int foundIndex = BuffChart.BinarySearchRecursive(time, 0, BuffChart.Count - 1);
            Segment found = BuffChart[foundIndex];
            if (found.ContainsPoint(time))
            {
                return found;
            }

            return new Segment(long.MinValue, long.MaxValue, 0);
        }

        public IReadOnlyList<Segment> GetBuffStatus(long start, long end)
        {
            var res = new List<Segment>(BuffChart.Count);
            foreach (Segment seg in BuffChart)
            {
                if (seg.Intersects(start, end))
                {
                    res.Add(seg);
                }
            }
            return res;
        }

        public int GetStackCount(long time)
        {
            return (int)GetBuffStatus(time).Value;
        }


        public bool IsPresent(long time, long window = 0)
        {
            if (window != 0)
            {
                long absWindow = Math.Abs(window);
                return GetBuffStatus(time - absWindow, time + absWindow).Any(x => x.Value > 0);
            }
            return GetStackCount(time) > 0;
        }

        //TODO(Rennorb) @perf
        /// <summary>
        /// Fuse consecutive segments with same value
        /// </summary>
        internal void FuseSegments()
        {
            _buffChart.RemoveAll(x => x.Start > x.End);
            _buffChart.FuseConsecutive();
        }

        /// <summary>
        /// This method will integrate the graph "from" to "to"
        /// It is going to add +1 to "to" when "from" has a value > 0
        /// </summary>
        internal void MergePresenceInto(IReadOnlyList<Segment> from)
        {
            if (_buffChart.Count == 0)
            {
                _buffChart.AddRange(from.Select(x => new Segment(x.Start, x.End, x.Value > 0 ? 1 : 0)));
            }
            else
            {
                //TODO(Rennorb) @perf
                var segmentsToFill = new LinkedList<Segment>(_buffChart);
                LinkedListNode<Segment> node = segmentsToFill.Find(segmentsToFill.First());
                foreach (Segment seg in from)
                {
                    long start = seg.Start;
                    long end = seg.End;
                    int presence = seg.Value > 0 ? 1 : 0;
                    // No need to process this segment
                    if (presence == 0)
                    {
                        continue;
                    }

                    while (node != null)
                    {
                        Segment curSeg = node.Value;
                        long curEnd = curSeg.End;
                        long curStart = curSeg.Start;
                        int curVal = (int)curSeg.Value;
                        if (curStart > end)
                        {
                            break;
                        }

                        if (curEnd < start)
                        {
                            node = node.Next;
                            continue;
                        }

                        // The segment in inside current one
                        if (end <= curEnd)
                        {
                            curSeg.End = start;
                            segmentsToFill.AddAfter(node, new Segment(start, end, curVal + presence));
                            node = node.Next;
                            segmentsToFill.AddAfter(node, new Segment(end, curEnd, curVal));
                            node = node.Next;
                            break;
                        }
                        else
                        {
                            // the segment straddles cur and next
                            curSeg.End = start;
                            segmentsToFill.AddAfter(node, new Segment(start, curEnd, curVal + presence));
                            node = node.Next;
                            start = curEnd;
                        }
                        node = node.Next;
                    }
                }
                _buffChart = segmentsToFill.ToList();
            }
            // Merge consecutive segments with same value, otherwise expect exponential growth
            FuseSegments();
        }

    }
}
