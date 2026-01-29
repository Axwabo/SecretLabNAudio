using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core.Groups;

public sealed class SpeakerToyGroup
{

    private readonly HashSet<SpeakerToy> _children = [];

    public SpeakerToy Controller { get; }

    public IReadOnlyCollection<SpeakerToy> Children => _children;

    internal SpeakerToyGroup(SpeakerToy controller) => Controller = controller;

    internal bool MasterRemoved { private get; set; }

    public bool IsDestroyed => MasterRemoved || Controller.IsDestroyed;

    public bool IsChild(SpeakerToy speaker) => _children.Contains(speaker);

    public SpeakerToyGroup Add(SpeakerToy toy)
    {
        if (Controller == toy)
            throw new InvalidOperationException("Cannot add the controller of a group as a child");
        _children.Add(toy);
        toy.ControllerId = Controller.ControllerId;
        return this;
    }

    public SpeakerToyGroup Remove(SpeakerToy toy, bool returnToPool = true)
    {
        if (Controller == toy)
            throw new InvalidOperationException("Cannot remove the controller of a group");
        if (!_children.Remove(toy))
            return this;
        if (returnToPool)
            SpeakerToyPool.Return(toy);
        else
            toy.Destroy();
        return this;
    }

}
