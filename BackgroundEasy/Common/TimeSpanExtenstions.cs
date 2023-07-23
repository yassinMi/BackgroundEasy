using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundEasy.Common
{
    public static class TimeSpanExtensions
    {
        private enum TimeSpanElement
        {
            Millisecond,
            Sec,
            Min,
            H,
            D
        }

        public static string ToFriendlyDisplay(this TimeSpan timeSpan, int maxNrOfElements)
        {
            maxNrOfElements = Math.Max(Math.Min(maxNrOfElements, 5), 1);
            var parts = new[]
                            {
                            Tuple.Create(TimeSpanElement.D, timeSpan.Days),
                            Tuple.Create(TimeSpanElement.H, timeSpan.Hours),
                            Tuple.Create(TimeSpanElement.Min, timeSpan.Minutes),
                            Tuple.Create(TimeSpanElement.Sec, timeSpan.Seconds),
                            // Tuple.Create(TimeSpanElement.Millisecond, timeSpan.Milliseconds)

                        }
                                        .SkipWhile(i => i.Item2 <= 0)
                                        .Take(maxNrOfElements);

            //return string.Join(", ", parts.Select(p => string.Format("{0} {1}{2}", p.Item2, p.Item1, p.Item2 > 1 ? "S" : string.Empty)));
            return string.Join(", ", parts.Select(p => string.Format("{0} {1}{2}", p.Item2, p.Item1, p.Item2 > 1 ? "" : string.Empty)));
        }
    }
}
