using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace TvMazeCollector.DAL.Entities;

public class ActorShow
{
    public int ActorId { get; set; }
    public Actor Actor { get; set; }
    public int ShowId { get; set; }
    public Show Show { get; set; }
}

public class ActorShowsConfiguration : IEntityTypeConfiguration<ActorShow>
{
    public void Configure(EntityTypeBuilder<ActorShow> builder)
    {
        builder.HasKey(x => new {x.ActorId, x.ShowId});

        builder.HasOne(x => x.Actor).WithMany(x => x.ActorShows).HasForeignKey(x => x.ActorId);
        builder.HasOne(x => x.Show).WithMany(x => x.ActorShows).HasForeignKey(x => x.ShowId);
    }
}