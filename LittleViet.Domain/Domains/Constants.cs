namespace LittleViet.Domain.Domains;

public static class Constants
{
    public static readonly DayOfWeek[] WorkingWeekDays = new[]
    {
        DayOfWeek.Monday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday,
    };

    public static readonly (TimeOnly Start, TimeOnly End)[] OpeningHours = new[]
    {
        (new TimeOnly(13, 00), new TimeOnly(16, 00)),
        (new TimeOnly(20, 00), new TimeOnly(23, 00)),
    };
}

public static class CacheKeys
{
    public static readonly string[] CatalogKeys = new [] { LandingPageCatalogCacheKey, TakeAwayMenuCatalogCacheKey };
    public const string LandingPageCatalogCacheKey = "LandingPageCatalog";
    public const string TakeAwayMenuCatalogCacheKey = "TakeAwayMenuCatalog";
}