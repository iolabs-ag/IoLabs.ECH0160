namespace IoLabs.ECH0160.Models
{
    /// <summary>Date range with min and max date.</summary>
    public class DateRange
    {
        /// <summary>Gets or sets the minimum date.</summary>
        public DateTime? MinDate { get; set; }

        /// <summary>Gets or sets the maximum date.</summary>
        public DateTime? MaxDate { get; set; }

        /// <summary>Try to extend the minimum date.</summary>
        /// <param name="minDate">Minimum date to extend with</param>
        public void TryExtendMin(DateTime? minDate)
        {
            if (MinDate == null || minDate < MinDate)
            {
                MinDate = minDate;
            }
        }

        /// <summary>Try to extend the maximum date.</summary>
        /// <param name="maxDate">Maximum date to extend with</param>
        public void TryExtendMax(DateTime? maxDate)
        {
            if (MaxDate == null || maxDate > MaxDate)
            {
                MaxDate = maxDate;
            }
        }

        /// <summary>Extend the date range with a new range.</summary>
        /// <param name="newRange">New range to extend with</param>
        public void Extend(DateRange newRange)
        {
            TryExtendMin(newRange.MinDate);
            TryExtendMax(newRange.MaxDate);
        }

        /// <summary>Gets the minimum date as formated string.</summary>
        public string MinDateString
        {
            get
            {
                return MinDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            }
        }

        /// <summary>Gets the maximum date as formated string.</summary>
        public string MaxDateString
        {
            get
            {
                return MaxDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            }
        }
    }
}