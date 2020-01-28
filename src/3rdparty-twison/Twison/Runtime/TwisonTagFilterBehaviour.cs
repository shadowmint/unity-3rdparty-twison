using System;
using System.Collections.Generic;
using System.Linq;
using Twison;

namespace DefaultNamespace
{
    /// <summary>
    /// A sample class to show how to use ITwisonRuntimeBehaviour.
    /// 
    /// Keeps track of what tags have been seen. If a tag is on a passage in the form:
    ///
    ///     Requires:XXXX
    ///
    /// Then links are filtered if the runtime state does not match any known tag XXXX.
    /// 
    /// If a tag is on a passage in the form:
    ///
    ///     Locked:XXXX
    ///
    /// Then links are filtered if the runtime state does match any known tag XXXX.
    /// 
    /// If a tag is on a passage in the form:
    ///
    ///     Drop:XXXX
    ///
    /// Then the tag XXX is dropped from the set of known states.
    /// </summary>
    public class TwisonTagFilterBehaviour : ITwisonRuntimeBehaviour
    {
        private readonly List<string> _tags = new List<string>();

        public List<TwisonLink> Filter(TwisonPassage active)
        {
            var requiredList = active.links
                .Select(i => Tuple.Create(i, i.passage.tags.Where(j => j.StartsWith("Requires:")).ToList()))
                .Where(i => i.Item2.Count > 0);

            var requiredFilters = requiredList.Where(i => i.Item2.Any(j =>
            {
                var requiredTag = j.Substring("Requires:".Length);
                return !_tags.Contains(requiredTag);
            }));
            
            var lockedList = active.links
                .Select(i => Tuple.Create(i, i.passage.tags.Where(j => j.StartsWith("Locked:")).ToList()))
                .Where(i => i.Item2.Count > 0);
            
            var lockedFilters = lockedList.Where(i => i.Item2.Any(j =>
            {
                var lockedTag = j.Substring("Locked:".Length);
                return _tags.Contains(lockedTag);
            }));

            return active.links.Where(i =>
            {
                return requiredFilters.All(j => j.Item1 != i) &&
                       lockedFilters.All(j => j.Item1 != i);
            }).ToList();
        }

        public void OnEnterState(TwisonPassage active)
        {
            var snapshot = _tags.ToList();
            active?.tags?.Where(i => !snapshot.Contains(i)).ToList().ForEach(i =>
            {
                if (i.StartsWith("Drop"))
                {
                    var dropTag = i.Substring("Drop:".Length);
                    if (_tags.Contains(dropTag))
                    {
                        _tags.Remove(dropTag);
                    }
                }
                else
                {
                    _tags.Add(i);    
                }
            });
            
            
        }

        public void OnExitState(TwisonPassage active)
        {
        }

        public void Reset()
        {
            _tags.Clear();
        }
    }
}