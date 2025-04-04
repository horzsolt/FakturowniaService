using System;

namespace FakturowniaService.util
{
    public static class Time
    {
        public static TimeSpan GetDurationFromString(string duration)
        {
            if (string.IsNullOrEmpty(duration) || duration == "0")
            {
                return TimeSpan.Zero;
            }

            int minutes = 0, seconds = 0;

            if (duration.Length == 1 || duration.Length == 2) // Only seconds
            {
                seconds = int.Parse(duration);
            }
            else if (duration.Length == 3) // First digit is minutes, last two are seconds
            {
                minutes = int.Parse(duration.Substring(0, 1));
                seconds = int.Parse(duration.Substring(1, 2));
            }
            else if (duration.Length >= 4) // First part is minutes, last two are seconds
            {
                minutes = int.Parse(duration.Substring(0, duration.Length - 2));
                seconds = int.Parse(duration.Substring(duration.Length - 2, 2));
            }

            return new TimeSpan(0, minutes, seconds);
        }

    }
}
