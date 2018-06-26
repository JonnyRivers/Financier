using System;

namespace Financier.Services
{
    public class DateTimeRange
    {
        public DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; }
        public DateTime End { get; }
    }
}
