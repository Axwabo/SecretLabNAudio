namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for <see cref="Player"/> wrappers.</summary>
public static class PlayerExtensions
{

    /// <summary>Gets the User ID of the specified <see cref="Player"/> and throws if it's null.</summary>
    /// <param name="player">The player to get the User ID from.</param>
    /// <returns>The User ID of the player.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="player"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="player"/>'s User ID is null (likely unauthenticated).</exception>
    public static string NotNullUserId(this Player player) => player == null
        ? throw new ArgumentNullException(nameof(player))
        : player.UserId ?? throw new ArgumentException("Player's user ID was null.", nameof(player));

}
