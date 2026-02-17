using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisp.Domain.Entities;

namespace Whisp.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.OwnsOne(m => m.Content, content =>
        {
            content.Property(c => c.Body)
                .IsRequired()
                .HasMaxLength(4096);

            content.Property(c => c.MediaUrl)
                .HasMaxLength(2048);

            content.Property(c => c.ThumbnailUrl)
                .HasMaxLength(2048);

            content.Property(c => c.FileName)
                .HasMaxLength(255);
        });
    }
}
