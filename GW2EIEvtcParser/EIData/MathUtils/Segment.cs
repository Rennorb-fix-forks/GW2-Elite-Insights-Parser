﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    /// <summary> A segment of time with type <see cref="double"/> with inclusive start and inclusive end. </summary>
    using Segment = GenericSegment<double>;

    public static class SegmentExt {
        public static double IntersectingArea(this in Segment self, in Segment other) => IntersectingArea(self, other.Start, other.End);

        public static double IntersectingArea(this in Segment self, long start, long end)
        {
            long maxStart = Math.Max(start, self.Start);
            long minEnd = Math.Min(end, self.End);
            return Math.Max(minEnd - maxStart, 0) * self.Value;
        }

        
        /// <summary>
        /// Fuse consecutive segments with same value. The list should not be empty.
        /// </summary>
        public static void FuseConsecutive(this List<Segment> segments)
        {

            Segment last = segments[0];
            int lastIndex = 0;
            for(int i = 1; i < segments.Count; i++)
            {
                var seg = segments[i];
                //TODO(Rennorb) perf
                if (seg.IsEmpty())
                {
                    continue;
                }

                if (seg.Value == last.Value)
                {
                    last.End = seg.End;
                }
                else
                {
                    segments[lastIndex] = last;
                    last = seg;
                    lastIndex++;
                }
            }
            segments[lastIndex++] = last;

            segments.RemoveRange(lastIndex, segments.Count - lastIndex);
        }

        /// <summary>
        /// Fuse consecutive segments with same value. The list should not be empty.
        /// </summary>
        public static List<Segment> FuseConsecutive(this IReadOnlyList<Segment> input)
        {
            //TODO(Rennorb) @mem could be trimmed if desired. dont always do it
            var newChart = new List<Segment>(input.Count);

            Segment last = input.First();
            foreach (Segment seg in input.Skip(1))
            {
                //TODO(Rennorb) perf
                if (seg.Start == seg.End)
                {
                    continue;
                }

                if (seg.Value == last.Value)
                {
                    last.End = seg.End;
                }
                else
                {
                    newChart.Add(last);
                    last = seg;
                }
            }
            newChart.Add(last);

            return newChart;
        }

        //TODO(Rennorb) @cleanup @perf
        public static List<object[]> ToObjectList(this IReadOnlyList<Segment> segments, long start, long end)
        {
            var res = new List<object[]>(segments.Count + 1);
            foreach (Segment seg in segments)
            {
                double segStart = Math.Round(Math.Max(seg.Start - start, 0) / 1000.0, ParserHelper.TimeDigit);
                res.Add(new object[] { segStart, seg.Value });
            }
            Segment lastSeg = segments.Last();
            double segEnd = Math.Round(Math.Min(lastSeg.End - start, end - start) / 1000.0, ParserHelper.TimeDigit);
            res.Add(new object[] { segEnd, lastSeg.Value });
            return res;
        }

        //TODO(Rennorb) @cleanup
        // https://www.c-sharpcorner.com/blogs/binary-search-implementation-using-c-sharp1
        public static int BinarySearchRecursive(this IReadOnlyList<Segment> segments, long time, int minIndex, int maxIndex)
        {
            if (segments[minIndex].Start > time)
            {
                return minIndex;
            }
            if (segments[maxIndex].Start < time)
            {
                return maxIndex;
            }
            if (minIndex > maxIndex)
            {
                return minIndex;
            }
            else
            {
                int midIndex = (minIndex + maxIndex) / 2;
                if (segments[midIndex].ContainsPoint(time))
                {
                    return midIndex;
                }
                else if (time < segments[midIndex].Start)
                {
                    return BinarySearchRecursive(segments, time, minIndex, midIndex - 1);
                }
                else
                {
                    return BinarySearchRecursive(segments, time, midIndex + 1, maxIndex);
                }
            }
        }
    }
}
