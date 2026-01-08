using System.Globalization;

namespace EventsManagement.Blazor.Helpers
{
    public static class PersianDateHelper
    {
        private static readonly PersianCalendar _persianCalendar = new PersianCalendar();

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی با فرمت کامل
        /// </summary>
        public static string ToPersianDate(this DateTime date)
        {
            return $"{_persianCalendar.GetYear(date):0000}/{_persianCalendar.GetMonth(date):00}/{_persianCalendar.GetDayOfMonth(date):00}";
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی با ساعت
        /// </summary>
        public static string ToPersianDateTime(this DateTime date)
        {
            var persianDate = ToPersianDate(date);
            return $"{persianDate} {date:HH:mm}";
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی با ساعت کامل
        /// </summary>
        public static string ToPersianDateTimeFull(this DateTime date)
        {
            var persianDate = ToPersianDate(date);
            return $"{persianDate} {date:HH:mm:ss}";
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی با نام روز هفته
        /// </summary>
        public static string ToPersianDateWithDayName(this DateTime date)
        {
            var persianDate = ToPersianDate(date);
            var dayName = GetPersianDayName(date.DayOfWeek);
            return $"{dayName} {persianDate}";
        }

        /// <summary>
        /// دریافت نام روز هفته به فارسی
        /// </summary>
        public static string GetPersianDayName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Saturday => "شنبه",
                DayOfWeek.Sunday => "یکشنبه",
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Tuesday => "سه‌شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Thursday => "پنجشنبه",
                DayOfWeek.Friday => "جمعه",
                _ => ""
            };
        }

        /// <summary>
        /// دریافت نام ماه شمسی
        /// </summary>
        public static string GetPersianMonthName(int month)
        {
            return month switch
            {
                1 => "فروردین",
                2 => "اردیبهشت",
                3 => "خرداد",
                4 => "تیر",
                5 => "مرداد",
                6 => "شهریور",
                7 => "مهر",
                8 => "آبان",
                9 => "آذر",
                10 => "دی",
                11 => "بهمن",
                12 => "اسفند",
                _ => ""
            };
        }
    }
}
