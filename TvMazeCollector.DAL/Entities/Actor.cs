using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace TvMazeCollector.DAL.Entities;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly? Birthday { get; set; }

    public ICollection<ActorShow> ActorShows { get; set; }
}

public class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(150);
        builder.Property(x => x.Birthday);
    }
}