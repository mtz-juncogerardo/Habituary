using Habituary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habituary.Data.Config;

public class OauthCredentialRecordConfiguration : IEntityTypeConfiguration<OauthCredentialRecord>
{
    public void Configure(EntityTypeBuilder<OauthCredentialRecord> builder)
    {
        builder
            .HasIndex(r => new { r.UserIRN, r.AuthType })
            .IsUnique();
    }
}