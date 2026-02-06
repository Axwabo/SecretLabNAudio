using System.Linq;
using SecretLabNAudio.Core.Groups;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="SpeakerToyGroup"/>s.
/// </summary>
public static class SpeakerToyGroupExtensions
{

    /// <param name="group">The group to add speakers to.</param>
    extension(SpeakerToyGroup group)
    {

        /// <summary>
        /// Creates a new <see cref="HashSet{T}"/> of all <see cref="SpeakerToyGroup"/>s.
        /// </summary>
        public static HashSet<SpeakerToyGroup> All
        {
            get
            {
                var set = new HashSet<SpeakerToyGroup>();
                foreach (var grouped in GroupedSpeaker.Instances)
                    set.Add(grouped.Group);
                return set;
            }
        }

        /// <summary>
        /// Enumerates all speakers that are grouped.
        /// </summary>
        public static IEnumerable<(SpeakerToy Speaker, SpeakerToyGroup Group)> GroupedSpeakers
            => GroupedSpeaker.Instances.Select(static e => (e.Speaker, e.Group));

        /// <summary>
        /// Rents one speaker from the <see cref="SpeakerToyPool"/>, and adds it to the group.
        /// </summary>
        /// <param name="position">The position of the speaker.</param>
        /// <param name="settings">The settings to apply. If null, the <see cref="SpeakerToyGroup.Controller"/>'s settings will be used.</param>
        /// <returns>The group itself.</returns>
        public SpeakerToyGroup AddFromPool(Vector3 position, SpeakerSettings? settings = null)
        {
            var toy = SpeakerToyPool.Rent(null, position, false);
            group.Add(toy);
            toy.ApplySettings(settings ?? SpeakerSettings.From(group.Controller));
            toy.Spawn();
            return group;
        }

        /// <summary>
        /// Rents multiple speaker from the <see cref="SpeakerToyPool"/>, and adds them to the group.
        /// The speakers' settings will match that of the <see cref="SpeakerToyGroup.Controller"/>.
        /// </summary>
        /// <param name="positions">The positions of the speakers.</param>
        /// <returns>The group itself.</returns>
        public SpeakerToyGroup AddFromPool(params IEnumerable<Vector3> positions)
        {
            foreach (var position in positions)
                group.AddFromPool(position);
            return group;
        }

        /// <summary>
        /// Rents multiple speaker from the <see cref="SpeakerToyPool"/>, and adds them to the group.
        /// </summary>
        /// <param name="settings">The settings to apply to the speakers.</param>
        /// <param name="positions">The positions of the speakers.</param>
        /// <returns>The group itself.</returns>
        public SpeakerToyGroup AddFromPool(SpeakerSettings settings, params IEnumerable<Vector3> positions)
        {
            foreach (var position in positions)
                group.AddFromPool(position, settings);
            return group;
        }

    }

}
