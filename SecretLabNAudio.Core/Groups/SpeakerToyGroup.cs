using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core.Groups;

/// <summary>
/// Represents a collection of speakers sharing the same <see cref="SpeakerToy.ControllerId"/>.
/// When the <see cref="Controller"/> is pooled/destroyed, all children are returned to the pool.
/// </summary>
public sealed class SpeakerToyGroup
{

    private readonly HashSet<SpeakerToy> _children = [];

    private readonly SpeakerToy _controller;

    internal SpeakerToyGroup(SpeakerToy controller) => _controller = controller;

    /// <summary>The main speaker of this group.</summary>
    /// <exception cref="ObjectDisposedException">Thrown if the group has already been destroyed.</exception>
    public SpeakerToy Controller => !IsDestroyed ? _controller : throw new ObjectDisposedException(nameof(SpeakerToyGroup));

    /// <summary>The children of this group.</summary>
    public IReadOnlyCollection<SpeakerToy> Children => _children;

    /// <summary>
    /// Whether this group has been destroyed (no longer exists).
    /// </summary>
    public bool IsDestroyed { get; private set; }

    /// <summary>
    /// Checks whether the given speaker is a child of this group.
    /// </summary>
    /// <param name="speaker">The speaker to check.</param>
    /// <returns>Whether the speaker is a child of this group.</returns>
    public bool IsChild(SpeakerToy speaker) => _children.Contains(speaker);

    /// <summary>
    /// Adds a speaker to the group, and assigns its <see cref="SpeakerToy.ControllerId"/> to that of the <see cref="Controller"/>.
    /// </summary>
    /// <param name="speaker">The speaker to add.</param>
    /// <returns>The group itself.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the group has already been destroyed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the speaker is already part of another group.</exception>
    public SpeakerToyGroup Add(SpeakerToy speaker)
    {
        if (IsDestroyed)
            throw new ObjectDisposedException(nameof(SpeakerToyGroup));
        if (Controller == speaker)
            return this;
        if (speaker.TryGetGroup(out var grouped) && grouped.Group != this)
            throw new InvalidOperationException("Speaker is part of another group.");
        speaker.ControllerId = Controller.ControllerId;
        if (_children.Add(speaker))
            speaker.GameObject.AddComponent<GroupedSpeaker>().Group = this;
        return this;
    }

    /// <summary>
    /// Removes a speaker from the group.
    /// </summary>
    /// <param name="speaker">The speaker to remove.</param>
    /// <returns>The group itself.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the group has already been destroyed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the speaker is the <see cref="Controller"/> of the group.</exception>
    public SpeakerToyGroup Remove(SpeakerToy speaker)
    {
        if (IsDestroyed)
            throw new ObjectDisposedException(nameof(SpeakerToyGroup));
        if (Controller == speaker)
            throw new InvalidOperationException("Cannot remove the controller of a group. Call Destroy instead.");
        if (!_children.Remove(speaker))
            return this;
        if (speaker.TryGetGroup(out var group))
            Object.Destroy(group);
        return this;
    }

    /// <summary>
    /// Marks the group as destroyed. Destroys all speakers, or returns all speakers to the pool.
    /// </summary>
    /// <seealso cref="SpeakerToyPool"/>
    public void Destroy(bool pool = true)
    {
        if (IsDestroyed)
            return;
        IsDestroyed = true;
        if (pool)
            SpeakerToyPool.Return(_controller);
        else
            _controller.DestroySafe();
        foreach (var toy in _children)
            if (pool)
                SpeakerToyPool.Return(toy);
            else
                toy.DestroySafe();
        _children.Clear();
    }

}
