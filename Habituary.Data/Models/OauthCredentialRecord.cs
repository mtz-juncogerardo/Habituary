using Habituary.Core.Types;

namespace Habituary.Data.Models;

public class OauthCredentialRecord : BaseIORecord
{
    public AuthenticationType AuthType { get; set; }
    public string? Hash { get; set; }
    public Guid UserIRN { get; set; }
    public UserRecord User { get; set; }
}