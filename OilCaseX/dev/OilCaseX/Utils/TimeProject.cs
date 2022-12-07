using System.Globalization;

namespace OilCaseApi.Utils
{
    public static class TimeProject
    {
        public static CultureInfo Culture { get; set; } = new CultureInfo("ru-RU");
        public static string TimeNow()
        {
            return DateTime.Now.ToString(Culture);
        }
    }
}
