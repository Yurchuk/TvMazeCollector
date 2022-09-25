using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TvMazeCollector.DAL;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(d => d.ToDateTime(TimeOnly.MinValue), d => DateOnly.FromDateTime(d))
    { }
}