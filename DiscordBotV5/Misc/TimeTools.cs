using System;

namespace DiscordBotV5.Misc
{
    public static class TimeTools
    {
        // TOOLS USED IN OTHER FUNCTIONS REGARDING TIME
        // ADDED WHEN NEEDED


        public static string TimeAgo(this DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }

        public static string PeriodOfTimeOutput(TimeSpan tspan, int level = 0)
        {
            string how_long_ago = "";
            if (level >= 2) return how_long_ago;
            if (tspan.Days > 1)
                how_long_ago = string.Format("{0} Days", tspan.Days);
            else if (tspan.Days == 1)
                how_long_ago = string.Format("1 Day {0}", PeriodOfTimeOutput(new TimeSpan(tspan.Hours, tspan.Minutes, tspan.Seconds), level + 1));
            else if (tspan.Hours >= 1)
                how_long_ago = string.Format("{0} {1} {2}", tspan.Hours, (tspan.Hours > 1) ? "Hours" : "Hour", PeriodOfTimeOutput(new TimeSpan(0, tspan.Minutes, tspan.Seconds), level + 1));
            else if (tspan.Minutes >= 1)
                how_long_ago = string.Format("{0} {1} {2}", tspan.Minutes, (tspan.Minutes > 1) ? "Minutes" : "Minute", PeriodOfTimeOutput(new TimeSpan(0, 0, tspan.Seconds), level + 1));
            else if (tspan.Seconds >= 1)
                how_long_ago = string.Format("{0} {1}", tspan.Seconds, (tspan.Seconds > 1) ? "Seconds" : "Second");
            return how_long_ago;

            //TimeSpan tspan = DateTime.Now.Subtract(dateTime);
            //int years;
            //if(tspan.Days >= 365)
            //{
            //    years = tspan.Days / 365;
            //}
            //int months;
            //int days;
            //int hours;
            //int minutes;

            //how_long_ago = string.Format("{0} Years, {1} Months, {2} Days, {3} Hours, {4} Minutes", tspan.)
        }
    }
}
