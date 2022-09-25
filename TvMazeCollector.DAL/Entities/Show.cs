using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace TvMazeCollector.DAL.Entities;

public class Show
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ActorShow> ActorShows { get; set; }
    public ShowStatus Status { get; set; }
}

public enum ShowStatus
{
    Created,
    CastAdded
}

public class ShowConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(150);
    }
}