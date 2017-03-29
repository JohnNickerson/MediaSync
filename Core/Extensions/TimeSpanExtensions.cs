using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Extensions
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Verbalises a TimeSpan to a human-friendly two-part approximation.
        /// </summary>
        /// <param name="duration">The TimeSpan to verbalise.</param>
        /// <returns>The timespan represented as the two most significant non-zero values of weeks, days, hours, minutes and seconds (eg "6d 3h").</returns>
        public static string Verbalise(this TimeSpan duration)
        {
            StringBuilder summary = new StringBuilder();
            if (duration.Ticks < 0)
            {
                duration = new TimeSpan(Math.Abs(duration.Ticks));
                summary.Append("-");
            }
            if (duration.TotalDays >= 7)
            {
                // wks + days
                summary.AppendFormat("{0}w {1}d", Math.Floor(duration.TotalDays / 7), duration.Days % 7);
            }
            else if (duration.TotalDays >= 1)
            {
                // days + hrs
                summary.AppendFormat("{0}d {1}h", Math.Floor(duration.TotalDays), duration.Hours);
            }
            else if (duration.TotalHours >= 1)
            {
                // hours + mins
                summary.AppendFormat("{0}h {1}m", Math.Floor(duration.TotalHours), duration.Minutes);
            }
            else if (duration.TotalMinutes >= 1)
            {
                // mins + secs
                summary.AppendFormat("{0}m {1}s", Math.Floor(duration.TotalMinutes), duration.Seconds);
            }
            else
            {
                summary.AppendFormat("{0}s", Math.Floor(duration.TotalSeconds));
            }
            return summary.ToString();
        }

    }
}
