using System;
using System.Diagnostics;

namespace Talos.Utility
{
    public sealed class Timer
    {
        private readonly TimeSpan TimeoutValue;
        private readonly long StartTime;

        public Timer(TimeSpan timeoutValue)
        {
            TimeoutValue = timeoutValue;
            StartTime = Stopwatch.GetTimestamp();
        }

        public static Timer FromSeconds(int seconds)
        {
            return new Timer(TimeSpan.FromSeconds(seconds));
        }

        public static Timer FromMinutes(int minutes)
        {
            return new Timer(TimeSpan.FromMinutes(minutes));
        }

        public static Timer FromHours(int hours)
        {
            return new Timer(TimeSpan.FromHours(hours));
        }

        public static Timer FromMilliseconds(int milliseconds)
        {
            return new Timer(TimeSpan.FromMilliseconds(milliseconds));
        }

        public bool IsTimeExpired
        {
            get
            {
                // Calculate elapsed time using Stopwatch.GetTimestamp and Stopwatch.Frequency
                double elapsedSeconds = (Stopwatch.GetTimestamp() - StartTime) / (double)Stopwatch.Frequency;
                TimeSpan elapsed = TimeSpan.FromSeconds(elapsedSeconds);

                return elapsed >= TimeoutValue;
            }
        }
    }
}
