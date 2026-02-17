using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Whisp.Domain.Entities;
using Whisp.Domain.Enums;

namespace Whisp.Infrastructure.Persistence.Configurations;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.HasKey(cm => new { cm.ConversationId, cm.UserId });

        builder.HasOne(cm => cm.Conversation)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.User)
            .WithMany(u => u.ConversationMembers)
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cm => cm.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(cm => cm.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ConversationRole.Member);
    }
}
