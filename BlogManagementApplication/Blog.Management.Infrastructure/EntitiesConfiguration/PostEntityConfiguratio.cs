using Blog.Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Management.Infrastructure.EntitiesConfiguration
{
    public class PostEntityConfiguratio : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(p => p.Content)
                   .IsRequired()
                    .HasColumnType("nvarchar(max)");

            builder.Property(p => p.Author)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(p => p.CoverImageUrl)
                   .HasMaxLength(250);

            builder.Property(p => p.IsActive)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.Property(p => p.UpdatedAt);

            builder.HasOne(p => p.Category)
                   .WithMany()
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
