namespace LittleViet.Infrastructure.DateTime;

public interface IDateTimeService
{
    TimeZoneInfo GetDefaultLittleVietTimeZone();
    System.DateTime ConvertToTimeZone(System.DateTime utc = default, TimeZoneInfo tzi = null);
}

internal class DateTimeService : IDateTimeService
{
    public TimeZoneInfo GetDefaultLittleVietTimeZone() => TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
    public System.DateTime ConvertToTimeZone(System.DateTime utc = default, TimeZoneInfo tzi = null)
    {
      if (utc == default)
        utc = System.DateTime.UtcNow;
        
      tzi ??= GetDefaultLittleVietTimeZone();
      
      return TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
    }
}