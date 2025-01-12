namespace SecretLabNAudio.Core.Extensions;

public static class ReferenceHubExtensions
{

    public static string NotNullUserId(this ReferenceHub hub) => !hub
            ? throw new ArgumentNullException(nameof(hub))
            : hub.authManager.UserId ?? throw new ArgumentException("ReferenceHub's user ID was null.", nameof(hub));

}
