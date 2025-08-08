namespace Habituary.Core.Extensions;

public static class DateExtension
{
    public static bool IsValidDate(this DateTime date)
    {
        var minDate = new DateTime(1899, 11, 11);
        return date != DateTime.MinValue && date.Year > minDate.Year;
    }
}